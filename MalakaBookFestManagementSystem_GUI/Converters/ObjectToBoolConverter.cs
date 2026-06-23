using System;
using System.Globalization;
using System.Windows.Data;

namespace MalakaBookFestManagementSystem_GUI.Converters
{
    public class ObjectToBoolConverter : IValueConverter
    {
        public static readonly ObjectToBoolConverter Instance = new ObjectToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
