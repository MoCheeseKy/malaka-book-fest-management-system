using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MalakaBookFestManagement_GUI.Views
{
    public class ActiveStyleConverter : IValueConverter
    {
        public Style ActiveStyle { get; set; } = null!;
        public Style InactiveStyle { get; set; } = null!;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive && isActive)
            {
                return ActiveStyle;
            }
            return InactiveStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
