using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Iskola.StyleSelectors
{
    public class MarksStyleSelector : StyleSelector
    { 
        protected override Style SelectStyleCore(object item,DependencyObject container)
        {
            Style style = new Style();
            style.TargetType = typeof(ListViewItem);
            Setter paddingSetter = new Setter();
            paddingSetter.Property = ListViewItem.PaddingProperty;
            paddingSetter.Value = new Thickness(0);
            Setter horizontalContentAlignmentSetter = new Setter();
            horizontalContentAlignmentSetter.Property = ListViewItem.HorizontalContentAlignmentProperty;
            horizontalContentAlignmentSetter.Value = HorizontalAlignment.Stretch;
            Setter backgroundSetter = new Setter();
            backgroundSetter.Property = ListViewItem.BackgroundProperty;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(container) as ListView;
            int index =
                listView.IndexFromContainer(container);
            if (index % 2 == 0)
            {
                backgroundSetter.Value = new SolidColorBrush(Color.FromArgb(255,223,223,223));
            }
            else
            {
                backgroundSetter.Value = new SolidColorBrush(Colors.Beige);
            }
            style.Setters.Add(backgroundSetter);
            style.Setters.Add(horizontalContentAlignmentSetter);
            style.Setters.Add(paddingSetter);
            return style;
        }
    }
}
