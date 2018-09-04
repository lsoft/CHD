namespace CHD.Plugin
{
    /// <summary>
    /// Информация о текущем состоянии плагина
    /// </summary>
    public interface ICHDPluginInformator
    {
        /// <summary>
        /// Текущее состояние плагина
        /// </summary>
        string CurrentInformation
        {
            get;
        }

        /// <summary>
        /// Обновить информацию о текущем состоянии плагина
        /// </summary>
        /// <param name="information">Информация</param>
        void UpdateInformation(string information);
    }
}