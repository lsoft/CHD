using System;
using System.Collections.Generic;
using CHD.Client.CompositionRoot.Module;
using Ninject;

namespace CHD.Client.CompositionRoot
{
    internal class Root : IDisposable
    {
        private readonly StandardKernel _kernel;

        private volatile bool _disposed = false;

        public Root(
            )
        {
            _kernel = new StandardKernel();
        }


        public void Init(
            string settingsFilePath
            )
        {
            var lm = new LoggerModule(
                );
            _kernel.Load(lm);

            var uim = new UserInterfaceModule(
                );
            _kernel.Load(uim);

            var mm = new MainModule(
                settingsFilePath
                );
            _kernel.Load(mm);

            var rm = new RecordModule(
                );
            _kernel.Load(rm);

            var fom = new FileOperationsModule(
                );
            _kernel.Load(fom);

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
