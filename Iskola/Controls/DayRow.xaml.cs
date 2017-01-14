using Iskola.Data;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Iskola.Controls
{
    public sealed partial class DayRow : UserControl
    {

        public DayRow()
        {
            this.InitializeComponent();
            this.DataContextChanged += DayRow_DataContextChanged;
        }

        private void DayRow_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            InitItems();
        }
        private bool Created = false;
        /// <summary>
        /// Initialize subjects in a day
        /// </summary>
        private void InitItems()
        {
            if (!Created&& DataContext != null)
            {
                Day actualDay = this.DataContext as Day;
                if (actualDay.IsFreeDay)
                {
                    SubjectsStack.Background = new SolidColorBrush(Colors.LightGray);
                    TextBlock freeDayNameText = new TextBlock()
                    {
                        Text = actualDay.FreeDayName,
                        FontSize = 18
                    };
                    SubjectsStack.Children.Add(freeDayNameText);
                    SubjectsStack.Height = 50;
                }
                else
                {
                    SubjectsStack.Children.Clear();
                    int actualColumnsCount = SubjectsStack.ColumnDefinitions.Count;
                    if (actualColumnsCount < actualDay.Hours.Count)
                        AddColumnsTo(actualDay.Hours.Count);
                    bool[] schoolActionPresentArray = new bool[actualDay.Hours.Count];
                    foreach (Hour actualHour in actualDay.Hours)
                    {
                        if (actualHour.Subjects.Count > 0 && actualHour.Subjects[0].IsSchoolAction)
                        {
                            Subject schoolActionSubject = actualHour.Subjects[0];
                            int actualIndex = actualHour.HourNumber;
                            for (int i = actualIndex; i < actualIndex + schoolActionSubject.SchoolActionLenght; i++)
                            {
                                schoolActionPresentArray[i] = true;
                            }
                            Rectangle rec = new Rectangle()
                            {
                                VerticalAlignment = VerticalAlignment.Top,
                                Height = 20,
                                Fill = new SolidColorBrush(Colors.Red)
                            };
                            Grid.SetColumnSpan(rec, schoolActionSubject.SchoolActionLenght);
                            Grid.SetColumn(rec, actualHour.HourNumber);
                            SubjectsStack.Children.Add(rec);
                            TextBlock teb = new TextBlock()
                            {
                                Text = schoolActionSubject.SchoolActionName,
                                VerticalAlignment = VerticalAlignment.Top
                            };
                            Grid.SetColumnSpan(teb, schoolActionSubject.SchoolActionLenght);
                            Grid.SetColumn(teb, actualHour.HourNumber);
                            SubjectsStack.Children.Add(teb);
                        }
                    }
                    foreach(var s in actualDay.Hours)
                    {
                        FrameworkElement hourElement = AddHour(s);
                        if (hourElement != null)
                        {
                            Grid.SetColumn(hourElement, s.HourNumber);
                            if (schoolActionPresentArray[s.HourNumber])
                            {
                                hourElement.Margin = new Thickness(0, 20, 0, 0);
                            }
                            SubjectsStack.Children.Add(hourElement);
                        }
                    }
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
            for(int i = SubjectsStack.ColumnDefinitions.Count;i<countNeeded;i++)
            {
                SubjectsStack.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50,GridUnitType.Pixel) });
            }
        }
        /// <summary>
        /// Adds subject to a frameworkelement depending on count of subjects in a hour
        /// </summary>
        /// <param name="hour">Hour to be added</param>
        /// <returns>FrameworkElement of hour</returns>
        private FrameworkElement AddHour(Hour hour)
        {
            if(hour.Subjects.Count>1)
            {
                StackPanel sp = new StackPanel();
                foreach (Subject actualSubject in hour.Subjects)
                {
                    if (!actualSubject.IsSchoolAction)
                    {
                        sp.Children.Add(new SubjectBox() { DataContext = actualSubject });
                    }
                }
                return sp;
            }
            else if(hour.Subjects.Count==1)
            {
                if (!hour.Subjects[0].IsSchoolAction)
                {
                    return new SubjectBox() { DataContext = hour.Subjects[0]};
                }
            }
            return null;
        }
    }
}
