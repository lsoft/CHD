namespace CHD.Plugin.Infrastructure
{
    public interface IPluginBinder
    {
        void InitPluginsFromFolder(string pluginRootFolder);

        void StartPlugins();

        void StopPlugins();

        void ReleasePlugins();
    }
}