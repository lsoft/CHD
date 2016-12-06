using System;
using System.Windows.Controls;
using CHD.Client.View;
using CHD.Client.ViewModel;
using Ninject;
using Ninject.Syntax;

namespace CHD.Client.CompositionRoot.WpfFactory
{
    internal class MarkerHistoryViewFactory : IViewFactory
    {
        private readonly IResolutionRoot _root;

        public Type ViewType
        {
            get
            {
                return
                    typeof(MarkerHistoryView);
            }
        }

        public MarkerHistoryViewFactory(
            IResolutionRoot root
            )
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            _root = root;
        }

        Control IViewFactory.CreateView()
        {
            var vm = _root.Get<MarkerHistoryViewModel>();
            var v = _root.Get<MarkerHistoryView>();

            v.DataContext = vm;

            return
                v;
        }
    }
}