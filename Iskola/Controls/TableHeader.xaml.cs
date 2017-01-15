using Iskola.Data;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Iskola.Controls
{
    public sealed partial class TableHeader : UserControl
    {
        public TableHeader()
        {
            this.InitializeComponent();
            this.DataContextChanged += TableHeader_DataContextChanged;
        }

        private void TableHeader_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            InitItems();
        }
        private bool Created = false;
        private void InitItems()
        {
            if (!Created && DataContext != null)
            {
                List<HourDefinition> definitions = this.DataContext as List<HourDefinition>;
                DefinitionsStack.Children.Clear();
                int actualColumnsCount = DefinitionsStack.ColumnDefinitions.Count;
                if (actualColumnsCount < definitions.Count)
                    AddColumnsTo(definitions.Count);
                foreach (HourDefinition actualHourDefinition in definitions)
                {
                    var v = new TableHourDefinition()
                    {
                        HourDefinition = actualHourDefinition
                    };
                    Grid.SetColumn(v, actualHourDefinition.HourNumber);
                    DefinitionsStack.Children.Add(v);
                }
                Created = true;
            }
        }
        /// <summary>
        /// Add needed ColumnDefinitions to a SubjectStack Grid
        /// </summary>
        /// <param name="ToCount"></param>
        private void AddColumnsTo(int countNeeded)
        {
            for (int i = DefinitionsStack.ColumnDefinitions.Count; i < countNeeded; i++)
            {
                DefinitionsStack.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Pixel) });
            }
        }
    }
}
