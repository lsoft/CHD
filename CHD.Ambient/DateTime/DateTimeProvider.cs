using System;
using System.Runtime.InteropServices;

namespace CHD.Ambient.DateTime
{
    public class DateTimeProvider : IDateTimeProvider
    {
#if WindowsCE
        private static readonly HiResolutionTimer _preTimer;
        private static readonly DateTime _preTime;
#endif

#if WindowsCE
        static DateTimeProvider()
        {
            //повышаем приоритет потока на 1 секунду, чтобы погрешность была меньше
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            try
            {
                //адски циклимся, чтобы уловить начало новой секунды
                var preTime = DateTime.Now;
                var presec = preTime.Second;
                var insec = presec;


                do
                {
                    insec = DateTime.Now.Second;
                } while (presec == insec);

                //секунда началась, соответственно мы можем зафиксировать тики
                //таким трюком мы связали тики, считаемые от старта компьютера
                //и глобальное "человеческое" время

                //сначала создаем таймер, чтобы тики зафиксировать
                _preTimer = new HiResolutionTimer();

                //теперь сохраняем глобальное время, добавляя секунду, которую мы прождали
                _preTime = preTime.AddMilliseconds(-preTime.Millisecond).AddSeconds(1);
            }
            finally
            {
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
            }
        }

        public static void DoNothing()
        {
            //make sure to static constructor is executed
        }
#endif

        public System.DateTime MinDate
        {
            get
            {
                return System.DateTime.MinValue;
            }
        }

        public System.DateTime DefaultDate
        {
            get { return new System.DateTime(1970, 1, 1); }
        }

#if !WindowsCE
        public System.DateTime GetCurrentTime()
        {
            return
                System.DateTime.Now;
        }
#else
        public DateTime GetCurrentTime()
        {
            var elapsedSeconds = _preTimer.ElapsedSeconds;

            var result = _preTime;
            result = result.AddSeconds(elapsedSeconds);

            return result;
        }
#endif

        public System.DateTime GetCurrentDate()
        {
            return
                this.GetCurrentTime().Date;
        }

        public static readonly int[] MonthMaxDays =
        { 
            31, //jan
            29, //feb
            31, //mar
            30, //apr
            31, //may
            30, //jun
            31, //jul
            31, //aug
            30, //sep
            31, //oct
            30, //nov
            31 //dec
        };

        public string GetUpperMonthName(string monthName)
        {
            return
                monthName[0].ToString().ToUpper() + monthName.Substring(1);
        }

        public static readonly string[] MonthNames = 
        {
            "январь",
            "февраль",
            "март",
            "апрель",
            "май",
            "июнь",
            "июль",
            "август",
            "сентябрь",
            "октябрь",
            "ноябрь",
            "декабрь"
        };

        public static readonly string[] MonthNames2 = 
        {
            "января",
            "февраля",
            "марта",
            "апреля",
            "мая",
            "июня",
            "июля",
            "августа",
            "сентября",
            "октября",
            "ноября",
            "декабря"
        };

        public void SetSystemDateTime(System.DateTime newTime)
        {
            var uTime = newTime.ToUniversalTime();

            var st = new SYSTEMTIME();
            st.wYear = (short)uTime.Year;
            st.wMonth = (short)uTime.Month;
            st.wDay = (short)uTime.Day;
            st.wHour = (short)uTime.Hour;
            st.wMinute = (short)uTime.Minute;
            st.wSecond = (short)uTime.Second;
            st.wMilliseconds = (short)uTime.Millisecond;

            SetSystemTime(ref st);
        }

        public bool IsHoliday(System.DateTime dt)
        {
            return
                dt.DayOfWeek == DayOfWeek.Saturday
                || dt.DayOfWeek == DayOfWeek.Sunday;
        }

        #region private

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

#if WindowsCE
        [DllImport("coredll.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);
#else
        [DllImport("kernel32.dll", SetLastError = true)]
        private extern static bool SetSystemTime([In] ref SYSTEMTIME st);
#endif

        #endregion

    }
}
