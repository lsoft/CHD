
namespace CHD.Settings.Controller
{
    public interface ISettingsFactory
    {
        ISettings LoadSettings(
            string filePath
            );
    }
}