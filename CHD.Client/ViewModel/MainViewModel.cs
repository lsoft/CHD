using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Castle.Core.Internal;
using CHD.Client.CompositionRoot.WpfFactory;
using CHD.Wpf;

namespace CHD.Client.ViewModel
{
    internal class MainViewModel : BaseViewModel, IViewChanger
    {
        private Type _key;
        private Dictionary<Type, Control> _views;

        public Control CurrentView
        {
            get
            {
                Control result = null;

                if (_views != null && _key != null)
                {
                    _views.TryGetValue(_key, out result);
                }

                return
                    result;
            }
        }

        public MainViewModel(
            Dispatcher dispatcher
            ) : base(dispatcher)
        {
            //_views = new Dictionary<Type, Control>();
        }

        public void InsertForms(
            IEnumerable<IViewFactory> factories
            )
        {
            if (factories == null)
            {
                throw new ArgumentNullException("factories");
            }

            var views = factories.ToDictionary(
                j => j.ViewType,
                j => j.CreateView()
                );

            if (!views.Any())
            {
                throw new ArgumentException("views.Length == 0");
            }

            _key = views.First().Key;
            _views = views;
        }

        public void ChangeForm(Type viewType)
        {
            if (!_views.ContainsKey(viewType))
            {
                throw new InvalidOperationException(string.Format("{0} does not exists", viewType.FullName));
            }

            _key = viewType;

            OnPropertyChanged(string.Empty);
        }
    }
}
