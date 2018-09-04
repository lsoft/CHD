using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace CHD.Service.WindowsService
{
    [RunInstaller(true)]
    public sealed class ServiceInstall : Installer
    {
        /// <summary>
        ///     Экземпляр объекта инсталятора процесса для сервиса
        /// </summary>
        private readonly ServiceProcessInstaller _processInstaller;


        /// <summary>
        ///     Экземпляр объекта инсталятора сервиса
        /// </summary>
        private readonly System.ServiceProcess.ServiceInstaller _serviceInstaller;


        /// <summary>
        ///     Required designer variable
        /// </summary>
        private Container _components;


        /// <summary>
        ///     Конструктор инсталлятора службы
        /// </summary>
        public ServiceInstall()
        {
            // This call is required by the Designer.
            InitializeComponent();

            _serviceInstaller = new ServiceInstaller
            {
                StartType = ServiceStartMode.Manual,
                ServiceName = ServiceMode.ServerServiceName,
                DisplayName = ServiceMode.ServerDisplayName,
                ServicesDependedOn = new string[] { }
            };
            Installers.Add(_serviceInstaller);

            _processInstaller = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };
            Installers.Add(_processInstaller);
        }


        #region Component Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
