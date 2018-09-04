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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CHD.Client.Gui.ViewModel.Main;
using CHD.Wpf;

namespace CHD.Client.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MainViewModel).Unload();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBinding();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            for (var tabIndex = _tabControl.Items.Count - 1; tabIndex >= 0; tabIndex--)
            {
                _tabControl.SelectedIndex = tabIndex;
                _tabControl.UpdateLayout();
            }

            _tabControl.SelectedIndex = 0;
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
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
    }
}
