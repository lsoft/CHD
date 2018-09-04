using System.Reflection;

namespace CHD.Service.CompositionRoot
{
    public static class VersionHelper
    {
        public static string GetId()
        {
            var ea = Assembly.GetCallingAssembly();

            var n = ea.GetName().Name;

            return
                n;
        }

        public static string GetVersion()
        {
            var ea = Assembly.GetCallingAssembly();

            var v = ea.GetName().Version;

            return
                v.ToString();
        }

        public static string GetIdAndVersion()
        {
            var ea = Assembly.GetCallingAssembly();

            var n = ea.GetName().Name;
            var v = ea.GetName().Version;

            return
                string.Format(
                    "{0} {1}",
                    n,
                    v
                    );
        }
    }
}