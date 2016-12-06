namespace CHD.Ambient.DateTime
{
    /// <summary>
    /// Провайдер текущего времени
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Минимальное время, могущее быть закодированным в DateTime
        /// </summary>
        System.DateTime MinDate
        {
            get;
        }

        /// <summary>
        /// Дата 1.1.1970
        /// </summary>
        System.DateTime DefaultDate
        {
            get;
        }

        /// <summary>
        /// Получить текущее время
        /// </summary>
        System.DateTime GetCurrentTime();

        /// <summary>
        /// Получить текущую дату (время 00:00:00.000)
        /// </summary>
        /// <returns></returns>
        System.DateTime GetCurrentDate();

        /// <summary>
        /// Получить имя месяца с большой буквы
        /// </summary>
        string GetUpperMonthName(string monthName);

        /// <summary>
        /// Установить новое системное время
        /// </summary>
        void SetSystemDateTime(System.DateTime newTime);

        /// <summary>
        /// Проверка, является ли день - выходным
        /// </summary>
        bool IsHoliday(System.DateTime dt);
    }
}