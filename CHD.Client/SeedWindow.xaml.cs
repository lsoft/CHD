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
using CHD.Client.ViewModel;
using CHD.Common;

namespace CHD.Client
{
    /// <summary>
    /// Interaction logic for SeedWindow.xaml
    /// </summary>
    public partial class SeedWindow : Window
    {
        public SeedWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.NotIn(
                Key.D0,
                Key.D1,
                Key.D2,
                Key.D3,
                Key.D4,
                Key.D5,
                Key.D6,
                Key.D7,
                Key.D8,
                Key.D9,
                Key.NumPad0,
                Key.NumPad1,
                Key.NumPad2,
                Key.NumPad3,
                Key.NumPad4,
                Key.NumPad5,
                Key.NumPad6,
                Key.NumPad7,
                Key.NumPad8,
                Key.NumPad9,
                Key.Delete,
                Key.Back
                ))
            {
                e.Handled = true;
            }
        }

        private void SeedWindow_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _passwordBox.Focus();
        }

        private void SeedWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _passwordBox.Focus();
        }

        private void _passwordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as SeedViewModel;
            if (vm != null)
            {
                vm.Seed = _passwordBox.Password;
            }
        }

        private void _passwordBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.In(Key.Enter))
            {
                this.Close();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
