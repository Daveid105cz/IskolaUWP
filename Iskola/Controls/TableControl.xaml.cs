using Iskola.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Iskola.Controls
{
    public sealed partial class TableControl : UserControl
    {
        public Table Table
        {
            get { return (Table)GetValue(TableProperty); }
            set { SetValue(TableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Table.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TableProperty =
            DependencyProperty.Register("Table", typeof(Table), typeof(TableControl), new PropertyMetadata(null));


        public TableControl()
        {
            this.InitializeComponent();
        }
    }
}
