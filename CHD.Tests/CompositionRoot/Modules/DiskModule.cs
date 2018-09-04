//using CHD.Tests.FolderStructureProvider;
using System;
using System.Collections.Generic;
using System.IO;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.Surgeon;
using CHD.Common.Others;
using CHD.Common.Saver;
using CHD.Common.Scanner;
using CHD.Common.Structure.Cleaner;
using CHD.Common.Structure.Container.Factory;
using CHD.Common.Sync.Factory;
using CHD.Common.Watcher;
using CHD.Disk;
using CHD.Disk.FileSystem;
using CHD.Tests.ExtendedContext;
using CHD.Tests.FileSystem.Surgeon.Factory;
using CHD.Tests.Sync;
using Ninject.Modules;
using FileSystemWatcher = System.IO.FileSystemWatcher;

namespace CHD.Tests.CompositionRoot.Modules
{
    public sealed class DiskModule : NinjectModule
    {
        private readonly string _rootFolderName;
        private readonly string _structureFileName;
        private readonly RoleEnum _type;

        #region static

        static DiskModule(
            )
        {
            string chdtFolder = Path.Combine(
                Path.GetTempPath(),
                "__ CHD TESTS __"
                );

            if (!Directory.Exists(chdtFolder))
            {
                Directory.CreateDirectory(chdtFolder);
            }
            else
            {
                //чистим совсем старьё
                string[] dirs = Directory.GetDirectories(chdtFolder);

                foreach (string dir in dirs)
                {
                    try
                    {
                        var diri = new DirectoryInfo(dir);
                        TimeSpan alive = DateTime.Now - diri.CreationTime;
                        if (alive.TotalDays > 2)
                        {
                            Directory.Delete(diri.FullName, true);
                        }
                    }
                    catch
                    {
                        //nothing to do
                    }
                }
            }

            CHDTFolder = chdtFolder;
        }

        public static string CHDTFolder
        {
            get;
            private set;
        }

        #endregion

        public DiskModule(
            string rootFolderName,
            string structureFileName,
            RoleEnum type
            )
        {
            if (rootFolderName == null)
            {
                throw new ArgumentNullException("rootFolderName");
            }
            if (structureFileName == null)
            {
                throw new ArgumentNullException("structureFileName");
            }

            _rootFolderName = rootFolderName;
            _structureFileName = structureFileName;
            _type = type;
        }

        public override void Load(
            )
        {
            string suffix = _type.ToString();

            string uniqueSuffix = DateTime.Now.ToString("yyyyMMddHHmmss") + "                    " + Guid.NewGuid();

            string preRootPath = Path.Combine(
                CHDTFolder,
                "____ " + suffix + " " + uniqueSuffix
                );

            if (!Directory.Exists(preRootPath))
            {
                Directory.CreateDirectory(preRootPath);
            }

            var rootFolderPath = Path.Combine(
                preRootPath,
                _rootFolderName
                );

            string structureFilePath = Path.Combine(
                preRootPath,
                _structureFileName
                );


            Bind<ISurgeonFactory>()
                .To<DiskSurgeonFactory>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "rootFolderPath",
                    rootFolderPath
                )
                ;

            Bind<ISynchronizerFactory>()
                .To<SynchronizerFactory>()
                .WhenInjectedExactlyInto<DebugLoggerSynchronizerFactory>()
                .InSingletonScope()
                ;

            Bind<ISynchronizerFactory>()
                .To<DebugLoggerSynchronizerFactory>()
                .InSingletonScope()
                ;

            Bind<IScanner>()
                .To<DefaultScanner>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "rootFolderPath",
                    rootFolderPath
                )
                .WithConstructorArgument(
                    "skipFolders",
                    new List<string>()
                    )
                ;

            if (_type.In(RoleEnum.Local, RoleEnum.Another))
            {
                Bind<IStructureCleaner>()
                    .To<LocalStructureCleaner>()
                    .InSingletonScope()
                    ;
            }
            else
            {
                Bind<IStructureCleaner>()
                    .To<RemoteStructureCleaner>()
                    .InSingletonScope()
                    .WithConstructorArgument(
                        "storedStructureCount",
                        16
                    )
                    ;
            }

            Bind<IBinarySaver<NotVersionedFileAddress>>()
                .To<FileSaver>()
                .InSingletonScope()
                ;

            Bind<IAddress, NotVersionedFileAddress>()
                .To<NotVersionedFileAddress>()
                .WhenInjectedInto<IStructureContainerFactory>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filePath",
                    structureFilePath
                )
                ;

            Bind<IStructureContainerFactory>()
                .To<StructureContainerFactory<NotVersionedFileAddress>>()
                .WhenInjectedExactlyInto<DiskFileSystemConnector>()
                .InSingletonScope()
                ;

            Bind<IFileSystemConnector, DiskFileSystemConnector>()
                .To<DiskFileSystemConnector>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "rootFolderName",
                    _rootFolderName
                )
                .WithConstructorArgument(
                    "preRootPath",
                    preRootPath
                )
                ;

            Bind<IFileSystemSurgeonConnector>()
                .To<FileSystemSurgeonConnector>()
                .InSingletonScope()
                ;

            Bind<IFileSystemWatcher>()
                .To<Common.Watcher.FileSystemWatcher>()
                .WhenInjectedExactlyInto<FileWatcherController>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "targetPath",
                    rootFolderPath
                    )
                ;

            Bind<IFileWatcherController>()
                .To<FileWatcherController>()
                .InSingletonScope()
                ;

        }
    }
}