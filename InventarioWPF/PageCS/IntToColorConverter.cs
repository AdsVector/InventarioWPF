using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace InventarioWPF.PageCS
{
    class IntToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int input = (int)value;

            if (input > 10 && input < 21)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#E67E22"));
            }
            else if (input > 0 && input < 11)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#E74C3C"));
            }
            else if (input == 0)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#2C3E50"));
            }
            else
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#2ECC71"));
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
