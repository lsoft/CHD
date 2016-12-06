using System;
using System.Windows.Controls;

namespace CHD.Client.CompositionRoot.WpfFactory
{
    internal interface IViewFactory
    {
        Type ViewType
        {
            get;
        }

        Control CreateView();
    }
}