using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CHD.Client.Gui.ViewModel.Main;
using Ninject;
using Ninject.Syntax;

namespace CHD.Client.Gui.CompositionRoot.Helper
{
    public sealed class MainWindowFactory : IMainWindowFactory
    {
        private readonly IResolutionRoot _root;

        public MainWindowFactory(
            IResolutionRoot root
            )
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            _root = root;
        }

        public Window Create(
            )
        {
            var w = _root.Get<MainWindow>();
            w.DataContext = _root.Get<MainViewModel>();

            return
                w;
        }
    }
}
