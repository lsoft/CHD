using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CHD.Client.Gui.Resources
{
    public sealed class GridViewConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            var scale = int.Parse(parameter as string) / 100.0;

            var l = o as ListView;

            //var  visibileProperty = (Visibility)l.GetValue(ScrollViewer.ComputedVerticalScrollBarVisibilityProperty);

            return
                l.ActualWidth * scale * 0.945;//0.98;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
