using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace CHD.Client.Gui.Resources
{
    public sealed class TabSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var tabControl = values[0] as TabControl;

            if (tabControl == null)
            {
                return 0;
            }

            if (tabControl.Items.Count == 0)
            {
                return
                    tabControl.ActualWidth;
            }

            double width = (tabControl.ActualWidth * 0.75) / tabControl.Items.Count;
            
            return
                Math.Max(0, width);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
