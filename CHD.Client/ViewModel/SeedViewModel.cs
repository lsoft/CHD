using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using CHD.Client.Crypto;
using CHD.Installer;
using CHD.Wpf;

namespace CHD.Client.ViewModel
{
    internal class SeedViewModel : BaseViewModel
    {
        private readonly ICryptoKeyController _keyController;

        private string _seed;

        public string Seed
        {
            get
            {
                return
                    _seed;
            }

            set
            {
                _seed = value;

                var key = KeyProvider.ProvideKey(value);
                _keyController.SetKey(key);
            }
        }

        public SeedViewModel(
            Dispatcher dispatcher,
            ICryptoKeyController keyController
            ) : base(dispatcher)
        {
            if (keyController == null)
            {
                throw new ArgumentNullException("keyController");
            }
            _keyController = keyController;
        }
    }
}
