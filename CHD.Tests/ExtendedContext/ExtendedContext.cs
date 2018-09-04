using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using CHD.Common;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.Surgeon;
//using CHD.Common.KeyValueContainer.SyncedVersion;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.Scanner;
using CHD.Common.Serializer;
using CHD.Common.Structure.Cleaner;
using CHD.Common.Structure.Container.Factory;
using CHD.Common.Sync;
using CHD.Common.Sync.Factory;
using CHD.Email.Settings;
using CHD.Email.Structure;
using CHD.MailRuCloud.Settings;
using CHD.MailRuCloud.Structure;
using CHD.Ninject;
using CHD.Remote.FileSystem;
using CHD.Tests.CompositionRoot.Modules;
using CHD.Tests.CompositionRoot.Modules.Fakes.Stat;
using CHD.Tests.ExtendedContext.StatRecord;
using CHD.Tests.FileSystem;
using CHD.Tests.FileSystem.Schema;
using CHD.Tests.FileSystem.Surgeon.Factory;
using CHD.Tests.Other;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.ChildKernel;
using Ninject.Parameters;
using Ninject.Planning.Targets;

namespace CHD.Tests.ExtendedContext
{
    public sealed class ExtendedContext
    {
        private readonly DebugLogger _logger;
        private readonly StandardKernel _kernel;
        private readonly Dictionary<RoleEnum, ChildKernel> _childs = new Dictionary<RoleEnum, ChildKernel>(); 

        public StatisticRecordRecord StatisticRecordRecord
        {
            get;
            private set;
        }

        public DateTime DefaultLastWriteTimeUtc
        {
            get;
            private set;
        }

        public ExtendedContext(
            DebugLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;

            DefaultLastWriteTimeUtc = DateTime.UtcNow;

            StatisticRecordRecord = new StatisticRecordRecord(
                "Complete test",
                logger
                );

            _kernel = new StandardKernel();

            _kernel.Load(
                new CommonModule(logger)
                ,new ProxyModule(logger)
                );
        }

        public void BindDisk(
            string rootFolderName,
            string structureFileName,
            RoleEnum type
            )
        {
            var child = new ChildKernel(
                _kernel
                );

            child.Load(
                new DiskModule(
                    rootFolderName,
                    structureFileName,
                    type
                    )
                );

            _childs.Add(
                type,
                child
                );
        }

        public void BindMailRu(
            string remoteRootFolderName,
            MailRuSettings mailRuSettings
            )
        {
            if (remoteRootFolderName == null)
            {
                throw new ArgumentNullException("remoteRootFolderName");
            }
            if (mailRuSettings == null)
            {
                throw new ArgumentNullException("mailRuSettings");
            }

            var module = new MailRuModule(
                remoteRootFolderName,
                mailRuSettings
                );

            _kernel.Load(module);
        }

        public void BindEmail(
            string remoteRootFolderName,
            EmailSettings emailSettings
            )
        {
            if (remoteRootFolderName == null)
            {
                throw new ArgumentNullException("remoteRootFolderName");
            }
            if (emailSettings == null)
            {
                throw new ArgumentNullException("emailSettings");
            }

            var module = new EmailModule(
                remoteRootFolderName,
                emailSettings
                );

            _kernel.Load(module);
        }

        public void Log()
        {
            _logger.ResetPrefix(
                );

            _logger.LogMessage(
                new string('-', 80)
                );

            StatisticRecordRecord.Log();

            _logger.LogMessage(string.Empty);


            var recordContainer = _kernel.Get<StatRecordContainer>();

            recordContainer.Log(_logger);

            _logger.LogMessage(
                new string('-', 80)
                );
        }

        public void Close()
        {
            foreach (var childKernel in _childs)
            {
                childKernel.Value.Dispose();
            }

            _kernel.Dispose();
        }

        #region get

        public T Get<T>()
        {
            var result = _kernel.Get<T>();

            return
                result;
        }


        public IFileSystemSurgeonConnector GetDiskSugeonConnector(
            RoleEnum type
            )
        {
            var result = _childs[type].Get<IFileSystemSurgeonConnector>();

            return
                result;
        }

        public IFileSystemSurgeonConnector GetEmailSugeonConnector(
            )
        {
            var result = _kernel.Get<IFileSystemSurgeonConnector>(
                new IParameter[]
                {
                    new TargetTypeConstructorArgument<RemoteStructureCleaner>(
                        "storedStructureCount",
                        16
                        ),
                    new Parameter(
                        EmailModule.RemoteStructureCleanerKey,
                        (object)null,
                        true
                        ), 
                }
                );

            return
                result;
        }

        public IFileSystemSurgeonConnector GetMailRuSugeonConnector(
            )
        {
            var result = _kernel.Get<IFileSystemSurgeonConnector>(
                new IParameter[]
                {
                    new TargetTypeConstructorArgument<RemoteStructureCleaner>(
                        "storedStructureCount",
                        16
                        ),
                    new Parameter(
                        MailRuModule.RemoteStructureCleanerKey,
                        (object)null,
                        true
                        ), 
                }
                );

            return
                result;
        }

        public IFileSystemSchema GetSchema(
            IFileSystemConnector fileSystemConnector
            )
        {
            var navigator = CreateNavigator(fileSystemConnector);

            var result = navigator.CloseFileSystem();

            return
                result;
        }


        public FileSystemNavigator CreateNavigator(
            IFileSystemConnector fileSystemConnector
            )
        {
            if (fileSystemConnector == null)
            {
                throw new ArgumentNullException("fileSystemConnector");
            }

            var navigator = _kernel.Get<FileSystemNavigator>(
                new ConstructorArgument(
                    "fileSystemConnector",
                    fileSystemConnector
                    )
                );

            return
                navigator;
        }

        public ISurgeonFactory GetSurgeonFactory(RoleEnum role)
        {
            return
                _childs[role].Get<ISurgeonFactory>();
        }

        public ISynchronizer CreateSynchronizer(
            RoleEnum type,
            IFileSystemConnector local,
            IFileSystemConnector remote
            )
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }
            if (remote == null)
            {
                throw new ArgumentNullException("remote");
            }

            var factory = _childs[type].Get<ISynchronizerFactory>();

            var synchronizer = factory.CreateSynchronizer(
                local,
                remote
                );

            return
                synchronizer;
        }

        public FileSystemBuilder CreateBuilder(
            IFileSystemSurgeonConnector surgeonConnector
            )
        {
            if (surgeonConnector == null)
            {
                throw new ArgumentNullException("surgeonConnector");
            }

            var builder = _kernel.Get<FileSystemBuilder>(
                new ConstructorArgument(
                    "surgeonConnector",
                    surgeonConnector
                    ),
                new ConstructorArgument(
                    "defaultLastWriteTimeUtc",
                    DefaultLastWriteTimeUtc
                    )
                );

            return
                builder;
        }

        #endregion

    }
}
