using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Iskola.Data;
using Windows.UI.Popups;
using Iskola.Security;
using Iskola.Dialogs;

namespace Iskola
{
    public sealed partial class LoginPage : Page
    {
        AccountControl _ac;
        private AccountControl Accounts
        {
            get { return _ac; }
        }
        public LoginPage()
        {
            this.InitializeComponent();
            _ac = new AccountControl();
            _ac.GetUsers();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            SetControls(false);
            App.CreateClient();
            ConnectionResult Result = await App.Client.Login(Login.Text, Password.Password, School.Text);
            SetControls(true);
            if(Result==ConnectionResult.Success)
            {
                App.MainFrame.Navigate(typeof(PageFrames.MainPage));
            }
            else if(Result==ConnectionResult.IncorrectCredentials)
            {
                MessageDialog md = new MessageDialog("Zkontrolujte prosím zadané údaje.", "Přihlášení se nezdařilo");
                await md.ShowAsync();
            }
            else
            {
                MessageDialog md = new MessageDialog("Došlo k problému s připojením k serveru", "Problém s připojením");
                await md.ShowAsync();
            }
        }
        private void SetControls(bool State)
        {
            Login.IsEnabled = State;
            Password.IsEnabled = State;
            School.IsEnabled = State;
            LoginButton.IsEnabled = State;
            LoggingStatusRing.IsActive = !State;
            LoggingStatusRing.Visibility = (State) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ClearCurrent_Click(object sender, RoutedEventArgs e)
        {
            Login.Text = "";
            Password.Password = "";
            School.Text = "";
        }
        private async  void AddNew_Click(object sender,RoutedEventArgs e)
        {
            UserAddDialog uad = new UserAddDialog(Accounts);
            await uad.ShowDialogAsync();
        }

        private async void SelectUser_Click(object sender, RoutedEventArgs e)
        {
            UsersSelectionDialog dialog = new UsersSelectionDialog(this);
            dialog.MaxWidth = this.ActualWidth;
            dialog.DataContext = Accounts.Users;
            await dialog.ShowDialogAsync();
        }
        internal void SelectUser(UserCredential uc)
        {
            Login.Text = uc.Username;
            Password.Password = uc.Password;
            School.Text = uc.School;
        }
    }
}
