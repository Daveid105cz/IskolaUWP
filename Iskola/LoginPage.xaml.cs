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
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _ac = new AccountControl();
            _ac.GetUsers();
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            SetControls(false);
            App.Client = new IskolaClient();
            ConnectionResult Result = await App.Client.Login(login.Text, password.Password, school.Text);
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
            login.IsEnabled = State;
            password.IsEnabled = State;
            school.IsEnabled = State;
            loginButton.IsEnabled = State;
            loggingInStatusRing.IsActive = !State;
            loggingInStatusRing.Visibility = (State) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ClearCurrent_Click(object sender, RoutedEventArgs e)
        {
            login.Text = "";
            password.Password = "";
            school.Text = "";
        }
        private async  void AddNew_Click(object sender,RoutedEventArgs e)
        {
            UserAddDialog uad = new UserAddDialog(Accounts);
            await uad.ShowAsync();
        }

        private async void SelectUser_Click(object sender, RoutedEventArgs e)
        {
            UsersSelectionDialog dialog = new UsersSelectionDialog(this);
            dialog.MaxWidth = this.ActualWidth;
            dialog.DataContext = Accounts.Users;
            await dialog.ShowAsync();
        }
        internal void SelectUser(UserCredential uc)
        {
            login.Text = uc.Username;
            password.Password = uc.Password;
            school.Text = uc.School;
        }
    }
}
