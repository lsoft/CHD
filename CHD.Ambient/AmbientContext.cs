using CHD.Ambient.DateTime;

namespace CHD.Ambient
{
    public static class AmbientContext
    {
        private volatile static IDateTimeProvider _dateTimeProvider = new DateTimeProvider();

        public static IDateTimeProvider DateTimeProvider
        {
            get
            {
                return
                    _dateTimeProvider;
            }
        }
    }
}
