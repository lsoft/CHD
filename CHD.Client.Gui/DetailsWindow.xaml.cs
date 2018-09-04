using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CHD.Wpf;

namespace CHD.Client.Gui
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        public DetailsWindow()
        {
            InitializeComponent();
        }

        private void DetailsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateBinding();
        }

        private void DetailsWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateBinding();
        }

        private void UpdateBinding()
        {
            var listViews = this.FindLogicalChildren<ListView>().ToList();

            foreach (var listView in listViews)
            {
                listView.UpdateLayout();

                var gridView = listView.View as GridView;
                if (gridView != null)
                {
                    foreach (var column in gridView.Columns)
                    {
                        var binding = BindingOperations.GetBindingExpressionBase(
                            column,
                            GridViewColumn.WidthProperty
                            );

                        if (binding != null)
                        {
                            binding.UpdateTarget();
                        }
                    }
                }
            }
        }

        private void DetailsWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
