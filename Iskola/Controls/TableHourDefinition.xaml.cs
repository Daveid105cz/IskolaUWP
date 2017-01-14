using Iskola.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Iskola.Controls
{
    public sealed partial class TableHourDefinition : UserControl
    {

        public HourDefinition HourDefinition
        {
            get { return (HourDefinition)GetValue(HourDefinitionProperty); }
            set { SetValue(HourDefinitionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourDefinitino.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HourDefinitionProperty =
            DependencyProperty.Register("HourDefinition", typeof(HourDefinition), typeof(TableHourDefinition), new PropertyMetadata(null));



        public TableHourDefinition()
        {
            this.InitializeComponent();
        }
    }
}
