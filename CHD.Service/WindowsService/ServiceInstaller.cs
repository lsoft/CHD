using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.ServiceProcess;
using Microsoft.Win32.SafeHandles;

namespace CHD.Service.WindowsService
{
    /// <summary>
    ///     Кастомизированный класс установщика экземпляра службы
    /// </summary>
    internal sealed class ServiceInstaller : System.ServiceProcess.ServiceInstaller
    {
        private const int SC_ACTION_TYPE_RESTART = 1;


        public override void Install(IDictionary stateSaver)
        {
            if (Context.Parameters.Count > 0)
            {
                if (Context.Parameters.ContainsKey("alias"))
                {
                    ServiceName = Context.Parameters["alias"];
                }

                if (Context.Parameters.ContainsKey("display"))
                {
                    DisplayName = Context.Parameters["display"];
                }

                if (Context.Parameters.ContainsKey("start"))
                {
                    if (Context.Parameters["start"].ToLower() == "auto")
                    {
                        StartType = ServiceStartMode.Automatic;
                    }
                    else if (Context.Parameters["start"].ToLower() == "disabled")
                    {
                        StartType = ServiceStartMode.Disabled;
                    }
                    else if (Context.Parameters["start"].ToLower() == "manual")
                    {
                        StartType = ServiceStartMode.Manual;
                    }
                    else
                    {
                        throw new ArgumentException(
                            String.Format("Значение {0} параметра start нераспознано", Context.Parameters["start"]));
                    }
                }

                if (Context.Parameters.ContainsKey("depends"))
                {
                    ServicesDependedOn = new[] { Context.Parameters["depends"] };
                }
            }

            base.Install(stateSaver);

            ChangeRecoveryProperty();
        }


        public override void Uninstall(IDictionary savedState)
        {
            if (Context.Parameters.Count > 0)
            {
                if (Context.Parameters.ContainsKey("alias"))
                {
                    ServiceName = Context.Parameters["alias"];
                }

                if (Context.Parameters.ContainsKey("display"))
                {
                    DisplayName = Context.Parameters["display"];
                }
            }

            base.Uninstall(savedState);
        }


        private void ChangeRecoveryProperty()
        {
            SafeServiceHandle hScManager = null;
            SafeServiceHandle hService = null;
            var lpFailureActions = IntPtr.Zero;

            try
            {
                hScManager = Win32.OpenSCManager(null, null, Win32.SERVICE_QUERY_CONFIG);
                if (hScManager.IsInvalid)
                {
                    throw new Win32Exception("Win32.OpenSCManager");
                }

                hService = Win32.OpenService(hScManager, ServiceName, Win32.SERVICE_ALL_ACCESS);
                if (hService.IsInvalid)
                {
                    throw new Win32Exception("Win32.OpenService");
                }

                var falureActions = new int[6];
                for (int idx = 0; idx < 3; idx++)
                {
                    falureActions[idx * 2] = SC_ACTION_TYPE_RESTART;
                    falureActions[idx * 2 + 1] = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                }
                lpFailureActions = Marshal.AllocHGlobal(falureActions.Length * Marshal.SizeOf(typeof(int)));
                Marshal.Copy(falureActions, 0, lpFailureActions, falureActions.Length);

                var scFailureActions = new SERVICE_FAILURE_ACTIONS
                {
                    cActions = 3,
                    dwResetPeriod = 0,
                    lpCommand = null,
                    lpRebootMsg = null,
                    lpsaActions = lpFailureActions
                };
                if (!Win32.ChangeServiceFailureActions(hService, Win32.SERVICE_CONFIG_FAILURE_ACTIONS, ref scFailureActions))
                {
                    throw new Win32Exception("Win32.ChangeServiceFailureActions");
                }

                var flag = new SERVICE_FAILURE_ACTIONS_FLAG { fFailureActionsOnNonCrashFailures = true };

                if (!Win32.FailureActionsOnNonCrashFailures(hService, Win32.SERVICE_CONFIG_FAILURE_ACTIONS_FLAG, ref flag))
                {
                    Console.WriteLine("Произошла ошибка. Вероятно, инсталлятор запущен на ОС Windows XP. Если это так, просто игнорируйте это сообщение, в противном случае сообщите об этой проблеме разработчикам");
                }
            }
            finally
            {
                if (hScManager != null
                    && !hScManager.IsInvalid)
                {
                    hScManager.Dispose();
                }

                if (hService != null
                    && !hService.IsInvalid)
                {
                    hService.Dispose();
                }

                if (lpFailureActions != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpFailureActions);
                }
            }
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct SERVICE_FAILURE_ACTIONS
        {
            public int dwResetPeriod;

            public string lpRebootMsg;

            public string lpCommand;

            public int cActions;

            public IntPtr lpsaActions;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct SERVICE_FAILURE_ACTIONS_FLAG
        {
            public bool fFailureActionsOnNonCrashFailures;
        }


        [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        internal sealed class SafeServiceHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            internal SafeServiceHandle()
                : base(true)
            {
            }


            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            protected override bool ReleaseHandle()
            {
                return Win32.CloseServiceHandle(handle);
            }
        }


        [SuppressUnmanagedCodeSecurity]
        internal static class Win32
        {
            public const int SERVICE_ALL_ACCESS = 0xF01FF;

            public const int SERVICE_QUERY_CONFIG = 0x0001;

            public const int SERVICE_CONFIG_FAILURE_ACTIONS = 0x2;

            public const int SERVICE_CONFIG_FAILURE_ACTIONS_FLAG = 0x4;


            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern SafeServiceHandle OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);


            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern SafeServiceHandle OpenService(SafeServiceHandle hSCManager, string lpServiceName, int dwDesiredAccess);


            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseServiceHandle(IntPtr hSCObject);


            [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ChangeServiceFailureActions(
                SafeServiceHandle hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_FAILURE_ACTIONS lpInfo);


            [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FailureActionsOnNonCrashFailures(
                SafeServiceHandle hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref SERVICE_FAILURE_ACTIONS_FLAG lpInfo);
        }
    }
}