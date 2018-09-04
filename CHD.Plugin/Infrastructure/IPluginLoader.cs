using System.Collections.Generic;

namespace CHD.Plugin.Infrastructure
{
    /// <summary>
    /// Загрузчик описаний плагинов
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// Загрузки описаний плагинов из папки
        /// </summary>
        /// <param name="pluginRootFolder">Папка, в которой лежат плагины</param>
        /// <returns>Список описаний плагинов</returns>
        List<IPluginXmlDescription> LoadFrom(string pluginRootFolder);
    }
}