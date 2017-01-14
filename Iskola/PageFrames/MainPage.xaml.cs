using Iskola.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Iskola.PageFrames
{
    public sealed partial class MainPage : Page
    {
        public IskolaClient Client { get { return App.Client; } }
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            marks.DataContext = Client.MarksTable;
        }
    }
}
