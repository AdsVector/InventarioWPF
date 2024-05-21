using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace InventarioWPF.PageCS
{
    public enum ModeGetPicture { Productos, Proveedores, Empleados, Null}

    class StringToUriConverter : IValueConverter
    {
        public ModeGetPicture modeGetPicture { get; set; } = ModeGetPicture.Null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string pathImage = value.ToString();

            if (pathImage.Length > 0)
            {
                return new Uri(pathImage);
            }
            else
            {
                if (modeGetPicture == ModeGetPicture.Null)
                    return null;
                else if (modeGetPicture == ModeGetPicture.Productos)
                    return new Uri(Directory.GetCurrentDirectory() + "/Resources/product_48px.png");
                else if (modeGetPicture == ModeGetPicture.Proveedores)
                    return new Uri(Directory.GetCurrentDirectory() + "/Resources/supplier_48px.png");
                else if (modeGetPicture == ModeGetPicture.Empleados)
                    return new Uri(Directory.GetCurrentDirectory() + "/Resources/customers_48px.png");
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
