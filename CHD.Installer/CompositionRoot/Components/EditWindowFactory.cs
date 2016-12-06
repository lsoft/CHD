using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.Crypto;
using CHD.Installer.View;
using CHD.Installer.ViewModel;
using CHD.Settings;
using CHD.Settings.Controller;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;

namespace CHD.Installer.CompositionRoot.Components
{
    internal class EditWindowFactory : IEditWindowFactory
    {
        private readonly IResolutionRoot _root;

        public EditWindowFactory(
            IResolutionRoot root
            )
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            _root = root;
        }

        public EditWindow Create(
            ISettings settings,
            string seed
            )
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var rc = _root.Get<ICrypto>(SettingsModule.RealCryptoKey);
            var fc = _root.Get<ICrypto>(SettingsModule.FakeCryptoKey);

            var w = _root.Get<EditWindow>();
            var vm = _root.Get<EditViewModel>(
                new ConstructorArgument(
                    "realCrypto",
                    rc
                    ),
                new ConstructorArgument(
                    "fakeCrypto",
                    fc
                    ),
                new ConstructorArgument(
                    "settings",
                    settings
                    ),
                new ConstructorArgument(
                    "seed",
                    seed
                    ),
                new ConstructorArgument(
                    "closeAction",
                    new Action(w.Close)
                    )
                );

            w.DataContext = vm;

            return
                w;
        }
    }
}
