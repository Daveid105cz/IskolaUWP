using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Iskola.Data;
using Windows.UI.Popups;
using Iskola.Security;
using Iskola.Dialogs;
using System.Windows.Input;
using System.Diagnostics;
using Windows.UI.Xaml.Controls.Primitives;

namespace Iskola
{
    public sealed partial class LoginPage : Page
    {
        AccountControl _ac;
        private object flyoutbase;

        private AccountControl Accounts
        {
            get { return _ac; }
        }
        public LoginPage()
        {
            this.InitializeComponent();
            _ac = new AccountControl();
            _ac.GetUsers();
            UsersListView.ItemsSource = Accounts.Users;
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

        private void SelectUser_Click(object sender, RoutedEventArgs e)
        {
            UserSelectionSplitView.IsPaneOpen = !UserSelectionSplitView.IsPaneOpen;
        }
        internal void SelectUser(UserCredential uc)
        {
            Login.Text = uc.Username;
            Password.Password = uc.Password;
            School.Text = uc.School;
        }

        private void UsersListView_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            var SelectedItem = ((FrameworkElement)e.OriginalSource).DataContext;
            if (SelectedItem != null)
            {
                foreach (var Item in UserMenuFlyout.Items)
                    Item.DataContext = SelectedItem;
                UserMenuFlyout.ShowAt(listView,e.GetPosition(listView));
            }
        }
        internal void Remove(UserCredential Credential)
        {
            Accounts.Users.Remove(Credential);
            Credential.Remove();
        }
        internal async void Edit(UserCredential Credential)
        {
            UserEditDialog ued = new UserEditDialog(Credential);
            await ued.ShowDialogAsync();
        }

        private void EditFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem)
            {
                if (menuFlyoutItem.DataContext is UserCredential credential)
                {
                    Edit(credential);
                }
            }
        }
        private void RemoveFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem)
            {
                if (menuFlyoutItem.DataContext is UserCredential credential)
                {
                    Remove(credential);
                }
            }
        }

        private void UsersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectUser(e.ClickedItem as UserCredential);
        }

        private void UsersListView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            UserSelectionSplitView.IsPaneOpen = false;
        }

    }
}
