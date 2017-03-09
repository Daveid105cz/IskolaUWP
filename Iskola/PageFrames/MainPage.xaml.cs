using System;
using Iskola.Controls;
using Iskola.Data;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

namespace Iskola.PageFrames
{
    public sealed partial class MainPage : Page
    {
        public IskolaClient Client { get { return App.Client; } }
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            foreach(TabItem pivotItem in MainPivot.Items)
            {
                pivotItem.DataTab = Client.DataTabs[MainPivot.Items.IndexOf(pivotItem)];
            }
            Client.OnTimeoutRaised += Client_OnTimeoutRaised;
            Client.OnLoginFailed += Client_OnLoginFailed;
        }

        private async void Client_OnLoginFailed()
        {
            MessageDialog dialog = new MessageDialog("Prihlášení selhalo!");
            await dialog.ShowAsync();
        }

        private async Task<bool> Client_OnTimeoutRaised()
        {
            MessageDialog md = new MessageDialog("Z důvodu neaktivity došlo k odhlášení ze systému\nChcete se znovu přihlásit ?");
            md.Commands.Add(new UICommand("Ano"));
            md.Commands.Add(new UICommand("Ne"));
            md.CancelCommandIndex = 1;
            md.DefaultCommandIndex = 0;
            IUICommand chosenCommand = await md.ShowAsync();
            return (chosenCommand == md.Commands[0]);
        }

        private async void LogoutAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MessageDialog md = new MessageDialog("Opravdu se chcete odhlásit ?");
            md.Commands.Add(new UICommand("Ano"));
            md.Commands.Add(new UICommand("Ne"));
            md.CancelCommandIndex = 1;
            md.DefaultCommandIndex = 0;
            IUICommand chosenCommand = await md.ShowAsync();
            if (chosenCommand == md.Commands[0])
            {
                MainCommandBar.IsOpen = false;
                MainCommandBar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
                App.Logout();
            }
        }
        private async void RefreshAppBarButton_Click(object sender,Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Client.DataTabs[MainPivot.SelectedIndex].DownloadDataAsync();
        }
        private async void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int TabIndex = MainPivot.Items.IndexOf(e.AddedItems[0]);
            if (!Client.DataTabs[TabIndex].IsAnythingLoaded)
                await Client.DataTabs[TabIndex].DownloadDataAsync();
        }

    }
}
