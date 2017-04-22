using Iskola.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Iskola.Controls
{
    public partial class TabItem : PivotItem
    {
        private DataTab _dataTab;

        public DataTab DataTab
        {
            get { return _dataTab; }
            set { _dataTab = value; this.DataContext = value; }
        }

        public TabItem()
        {
            this.InitializeComponent();
        }
    }
}
