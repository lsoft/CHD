using System;
using System.Windows;
using CHD.Client.Gui.ViewModel.Main;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace CHD.Client.Gui.CompositionRoot.Helper
{
    public sealed class DetailsWindowFactory : IDetailsWindowFactory
    {
        private readonly IResolutionRoot _root;

        public DetailsWindowFactory(
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
            JournalItemWrapper wrapper
            )
        {
            var w = _root.Get<DetailsWindow>();
            w.Owner = App.Current.MainWindow;

            w.DataContext = _root.Get<DetailsViewModel>(
                new ConstructorArgument(
                    "wrapper",
                    wrapper
                    )
                );

            return
                w;
        }
    }
}