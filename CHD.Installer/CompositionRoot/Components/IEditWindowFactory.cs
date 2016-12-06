using CHD.Installer.View;
using CHD.Settings.Controller;

namespace CHD.Installer.CompositionRoot.Components
{
    internal interface IEditWindowFactory
    {
        EditWindow Create(
            ISettings settings,
            string seed
            );
    }
}