using Iskola.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Iskola.Dialogs
{
    public sealed partial class UsersSelectionDialog : ContentDialog
    {
        private ObservableCollection<UserCredential> Users
        {
            get { return DataContext as ObservableCollection<UserCredential>; }
        }
        private DeleteCommand _rmdCmd;
        public DeleteCommand RemoveCmd
        {
            get { return _rmdCmd; }
        }
        private EditCommand _editCmd;
        public EditCommand EditCmd
        {
            get { return _editCmd; }
        }
        private LoginPage _lp;
        public UsersSelectionDialog(LoginPage lp)
        {
            this.InitializeComponent();
            _lp = lp;
            _rmdCmd = new DeleteCommand(this);
            _editCmd = new EditCommand(this);
        }
        internal void Remove(UserCredential credenc)
        {
            Users.Remove(credenc);
            credenc.Remove();
        }
        internal async void Edit(UserCredential credenc)
        {
            this.Hide();
            UserEditDialog ued = new UserEditDialog(credenc);
            await ued.ShowDialogAsync();
        }
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _lp.SelectUser(e.ClickedItem as UserCredential);
            this.Hide();
        }
    }
    public class DeleteCommand : ICommand
    {
        UsersSelectionDialog _dialog;
        public DeleteCommand(UsersSelectionDialog dialog)
        {
            _dialog = dialog;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _dialog.Remove(parameter as UserCredential);
        }
    }
    public class EditCommand : ICommand
    {
        UsersSelectionDialog _dialog;
        public EditCommand(UsersSelectionDialog dialog)
        {
            _dialog = dialog;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _dialog.Edit(parameter as UserCredential);
        }
    }
}
