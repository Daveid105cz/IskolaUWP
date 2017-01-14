using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Iskola.Converters
{
    public class SubjectStateToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color returnColor;
            switch((Data.SubjectState)value)
            {
                case Data.SubjectState.Actual:
                    returnColor =  Colors.LightGray;
                   break;
                case Data.SubjectState.Canceled:
                    returnColor =  Colors.MediumVioletRed;
                    break;
                default:returnColor = Colors.White;break;
            }
            return new SolidColorBrush(returnColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
