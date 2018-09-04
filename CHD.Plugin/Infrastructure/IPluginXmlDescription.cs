using System;
using System.Collections.Generic;

namespace CHD.Plugin.Infrastructure
{
    /// <summary>
    /// Описание плагина
    /// </summary>
    public interface IPluginXmlDescription
    {
        /// <summary>
        /// Идентификатор плагина
        /// </summary>
        Guid PluginGuid
        {
            get;
        }

        /// <summary>
        /// Описание плагина
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Стартовая dll плагина
        /// </summary>
        string RootFilePath
        {
            get;
        }

        /// <summary>
        /// Стартовый класс плагина, реализующий ICHDPlugin
        /// </summary>
        string RootClass
        {
            get;
        }

        /// <summary>
        /// Идентификаторы, от которых зависит плагин
        /// </summary>
        List<Guid> Dependencies
        {
            get;
        }
    }
}