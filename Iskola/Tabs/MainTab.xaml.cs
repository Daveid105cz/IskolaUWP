using Iskola.Controls;
using Iskola.Data.DataTabs;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Iskola.Tabs
{
    public partial class MainTab : TabItem
    {

        public MainTab()
        {
            this.InitializeComponent();
        }
        private async void Previous_Click(object sender, RoutedEventArgs e)
        {
            MainDataTab mdt = DataTab as MainDataTab;
            await mdt.PreviousWeek();
        }
        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            MainDataTab mdt = DataTab as MainDataTab;
            await mdt.NextWeek();
        }
    }
}
