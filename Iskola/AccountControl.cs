using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Security.Credentials;

namespace Iskola.Security
{
    public class AccountControl
    {
        internal const String ResourceName = "ISkola client";
        PasswordVault _vault;

        public AccountControl()
        {
            _vault = new PasswordVault();
            _usersCollection = new ObservableCollection<UserCredential>();
        }
        ObservableCollection<UserCredential> _usersCollection;
        public ObservableCollection<UserCredential> Users
        {
            get { return _usersCollection; }
        }
        private void GetUsers(Action<UserCredential> deleg)
        {
            IReadOnlyList<PasswordCredential> passwordCredentials = _vault.RetrieveAll();
            _usersCollection.Clear();
            foreach (PasswordCredential actualCredential in passwordCredentials)
            {
                deleg.Invoke(new UserCredential(actualCredential));
            }
        }
        public void GetUsers()
        {
            GetUsers((p) => { _usersCollection.Add(p); });
        }
        public void Remove(UserCredential credent)
        {
            _vault.Remove(credent._credential);
        }
        public void CreateNew(String UserName,String Password,String School)
        {
            PasswordCredential credential = new PasswordCredential(ResourceName, MergeStrings(UserName,School), Password);
            _vault.Add(credential);
            GetUsers();
        }
        public void DeleteAll()
        {
            GetUsers((p) => { p.Remove(); });
            GetUsers();
        }
        private const char _SpacingChar = (char)1;
        internal static String MergeStrings(String Username,String School)
        {
            return School + _SpacingChar + Username;
        }
        internal static Tuple<String,String> ReverseStringsMerge(String Value)
        {
            String[] values = Value.Split(_SpacingChar);
            return new Tuple<string, string>(values[0], values[1]);
        }
    }
    public class UserCredential
    {
        internal PasswordCredential _credential;
        public UserCredential(PasswordCredential creden)
        {
            _credential = creden;
            _credential.RetrievePassword();
            Tuple<String, String> value = AccountControl.ReverseStringsMerge(_credential.UserName);
            _name = value.Item2;
            _school = value.Item1;
            _password = _credential.Password;
        }
        string _name;
        public String Username
        {
            get { return _name; }
            set { _name = value;}
        }
        string _password;
        public String Password
        {
            get {  return _password; }
            set { _password = value; }
        }
        string _school;
        public String School
        {
            get { return _school; }
            set { _school = value; }
        }
        public void ApplyChanges()
        {
            PasswordCredential pdc = new PasswordCredential(AccountControl.ResourceName, AccountControl.MergeStrings(_name,_school), _password);
            Remove();
            new PasswordVault().Add(pdc);
            _credential = pdc;
        }
        public void Remove()
        {
            var vault = new PasswordVault();
            vault.RetrieveAll();
            vault.Remove(_credential);
        }
    }
}
