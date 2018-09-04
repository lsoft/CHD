using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common.PathComparer;
using CHD.Service.CompositionRoot;

namespace CHD.Service
{
    internal sealed class ClientStarter
    {
        private readonly Arguments _arguments;

        private readonly Root2 _root2;

        public ClientStarter(
            Arguments arguments
            )
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            _arguments = arguments;

            // В качестве текущего каталога установить каталог расположения службы
            // (по умолчанию используется каталог System32)
            var strFullFileName = GetType().Module.FullyQualifiedName;
            var strShortFileName = GetType().Module.Name;
            var strDir = strFullFileName.Substring(0, strFullFileName.Length - strShortFileName.Length - 1);
            Directory.SetCurrentDirectory(strDir);

            //резолвинг загруженных сборок (нужен для телеметрии - ProxyGenerator + HierarchyPayload)
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var result = AppDomain.CurrentDomain.GetAssemblies().ToList().Find(j => j.ToString() == eventArgs.Name);
                return result;
            };

            XLogger.Helper.StackHelper.StackFrame = 3;

            _root2 = new Root2(_arguments);

        }

        public void Start()
        {
            //-------------------------------------------INIT ------------------------------------------

            _root2.BindAll(
                
                );

            //----------------------------------------------- RUN -----------------------------------------------

            //старт
            _root2.Start();
        }

        public void SyncStop()
        {
            //--------------------------------------------FINISH --------------------------------------------

            _root2.Stop();

            //и окончательный диспоуз
            _root2.Dispose();
        }

        public void SetWindowsIsShuttingDown()
        {
            _root2.SetWindowsIsShuttingDown();
        }
    }
}
