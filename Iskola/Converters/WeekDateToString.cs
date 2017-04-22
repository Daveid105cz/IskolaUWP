using Iskola.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Iskola.Converters
{
    public class WeekDateToString:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Table table = value as Table;
            DateTime tableEndDate = table.TableDate.AddDays(4);
            int tableWeekNumber = GetIso8601WeekOfYear(table.TableDate);
            String oddOrEven = (tableWeekNumber % 2 == 1) ? "lichý" : "sudý";
            return String.Format("{0}.týden, {1} ({2} - {3})",tableWeekNumber,oddOrEven,table.TableDate.ToString("dd.MM.yyyy"), tableEndDate.ToString("dd.MM.yyyy"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
        private static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it’ll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            GregorianCalendar gc = new GregorianCalendar();
            DayOfWeek day = gc.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return gc.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
