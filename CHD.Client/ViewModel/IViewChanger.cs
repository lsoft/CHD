using System;

namespace CHD.Client.ViewModel
{
    internal interface IViewChanger
    {
        void ChangeForm(Type viewType);
    }
}