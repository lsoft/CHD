using System;
using System.Windows.Controls;
using CHD.Client.View;
using CHD.Client.ViewModel;
using Ninject;
using Ninject.Syntax;

namespace CHD.Client.CompositionRoot.WpfFactory
{
    internal class CurrentStatusViewFactory : IViewFactory
    {
        private readonly IResolutionRoot _root;

        public Type ViewType
        {
            get
            {
                return
                    typeof(CurrentStatusView);
            }
        }
        
        public CurrentStatusViewFactory(
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
            var vm = _root.Get<CurrentStatusViewModel>();
            var v = _root.Get<CurrentStatusView>();

            v.DataContext = vm;

            return
                v;
        }
    }
}
