using System;
using Ninject;

namespace CHD.Plugin
{
    /// <summary>
    /// Плагин системы
    /// </summary>
    public interface ICHDPlugin : IDisposable
    {
        /// <summary>
        /// Текущее состояние плагина
        /// </summary>
        string CurrentInformation
        {
            get;
        }

        /// <summary>
        /// Инициализация плагина
        /// </summary>
        /// <param name="root">Контейнер с абстракциями</param>
        void Init(IKernel root);

        /// <summary>
        /// Протестировать конфигурацию контейнеров плагина
        /// </summary>
        /// <returns>true - успешно</returns>
        bool TestContainerConfiguration();

        /// <summary>
        /// Запуск плагина
        /// </summary>
        void Start();

        /// <summary>
        /// Останов плагина
        /// </summary>
        void Stop();
    }
}
