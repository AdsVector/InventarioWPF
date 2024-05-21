using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace InventarioWPF.PageCS
{
    public partial class ViewData : Control
    {
        public string FirstText
        {
            get { return (string)GetValue(FirstProperty); }
            set { SetCurrentValue(FirstProperty, value); }
        }
        public readonly static DependencyProperty FirstProperty = DependencyProperty.Register("FirstText", typeof(string),
          typeof(ViewData), new UIPropertyMetadata(string.Empty));

        public string SecondText
        {
            get { return (string)GetValue(SecondProperty); }
            set { SetCurrentValue(SecondProperty, value); }
        }
        public readonly static DependencyProperty SecondProperty = DependencyProperty.Register("SecondText", typeof(string),
           typeof(ViewData), new UIPropertyMetadata(string.Empty));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetCurrentValue(ValueProperty, value); }
        }
        public readonly static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string),
          typeof(ViewData), new UIPropertyMetadata(string.Empty, (o, e) => {
              ViewData view = (ViewData)o;
              view.RaiseValueChangedEvent(e);
          }));

        public event EventHandler<DependencyPropertyChangedEventArgs> ValueChanged;
        private void RaiseValueChangedEvent(DependencyPropertyChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }     
    }
}
