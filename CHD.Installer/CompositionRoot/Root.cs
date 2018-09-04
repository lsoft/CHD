using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Installer.Scanner;
using Ninject;

namespace CHD.Installer.CompositionRoot
{
    internal sealed class Root : IDisposable
    {
        internal const string MainKey = "main";
        internal const string EmailKey = "email";
        internal const string MailRuKey = "mailru";

        private readonly StandardKernel _kernel;

        private volatile bool _disposed = false;

        private SettingsModule _settingsModule;

        public Root(
            )
        {
            _kernel = new StandardKernel();
        }

        public void Init(
            )
        {
            var lm = new LoggerModule(
                64
                );
            _kernel.Load(lm);

            var uim = new UserInterfaceModule(
                );
            _kernel.Load(uim);

            var sm = new SettingsModule(
                );
            _kernel.Load(sm);
            _settingsModule = sm;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _kernel.Dispose();
            }
        }

        public T Get<T>()
        {
            return
                _kernel.Get<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return
                _kernel.GetAll<T>();
        }

    }
}
