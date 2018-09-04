using System;
using CHD.Common;
using Microsoft.Win32;

namespace CHD.Service.CompositionRoot
{
    public static class RegistryPatcher
    {
        private const string PathToKey = "SYSTEM\\CurrentControlSet\\Control";
        private const string KeyName = "WaitToKillServiceTimeout";
        private const int TimeoutMsec = 300000;

        public static void WaitToKillServiceTimeoutPatch(
            IDisorderLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            try
            {
                using (var sk = Registry.LocalMachine.OpenSubKey(PathToKey, true))
                {
                    //var r = sk.GetValue(KeyName);

                    sk.SetValue(
                        KeyName,
                        TimeoutMsec.ToString(),
                        RegistryValueKind.String
                        );

                    sk.Flush();
                    sk.Close();
                }
            }
            catch (Exception excp)
            {
                logger.LogException(excp);
            }
        }
    }
}