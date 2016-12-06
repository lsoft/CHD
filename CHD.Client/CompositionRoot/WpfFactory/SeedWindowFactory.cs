using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Client.ViewModel;
using Ninject;
using Ninject.Syntax;

namespace CHD.Client.CompositionRoot.WpfFactory
{
    public class SeedWindowFactory
    {
        private readonly IResolutionRoot _root;

        public SeedWindowFactory(
            IResolutionRoot root
            )
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            _root = root;
        }

        public SeedWindow Create()
        {
            var vm = _root.Get<SeedViewModel>();
            var v = _root.Get<SeedWindow>();

            v.DataContext = vm;

            return
                v;
        }

    }
}
