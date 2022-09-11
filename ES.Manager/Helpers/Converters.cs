using System;
using System.Globalization;
using System.Windows.Data;

namespace ES.Manager.Helpers
{
    public class BoolInvertConverter : IValueConverter
    {
        #region IValueConverter Members2

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            return !(bool)value;
        }

        #endregion
    }
}
