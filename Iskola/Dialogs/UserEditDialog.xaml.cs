using Iskola.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class UserEditDialog : ContentDialog
    {
        private UserCredential _cred;
        public UserCredential Credential
        {
            get { return _cred; }
        }
        public UserEditDialog(UserCredential Credenc)
        {
            _cred = Credenc;
            this.DataContext = this;
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _cred.Username = login.Text;
            _cred.Password = password.Password;
            _cred.School = school.Text;
            _cred.ApplyChanges();
        }
    }
}
