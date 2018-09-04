using System;
using System.IO;
using System.Linq;
using CHD.Common.FileSystem;

using CHD.Common.FileSystem.Surgeon;
using CHD.Common.Saver;
using CHD.Common.Structure.Cleaner;
using CHD.Common.Structure.Container.Factory;
using CHD.Disk;
using CHD.Disk.Cleaner;
using CHD.Disk.FileSystem;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Parameters;

namespace CHD.Service.CompositionRoot.Modules
{
    public sealed class DiskModule : NinjectModule
    {
        public const string DiskScopeName = "Disk.ScopeName";

        public override void Load(
            )
        {
            Bind<IDiskFileSystemCleaner>()
                .To<DiskFileSystemCleaner>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "rootFolderPath",
                    c => c.Kernel.Get<ServiceSettings>().WatchFolder
                    )
                .WithConstructorArgument(
                    "structureFilePath",
                    c => c.Kernel.Get<ServiceSettings>().StructureFilePath
                    )
                ;

            Bind<IStructureCleaner>()
                .To<LocalStructureCleaner>()
                .When(request => request.Parameters.Any(j => j.Name == Root2.LocalStructureCleanerKey))
                .InSingletonScope()//.InNamedScope(DiskScopeName)
                ;

            //Bind<IStructureCleaner>()
            //    .To<RemoteStructureCleaner>()
            //    .When(request => request.Parameters.Any(j => j.Name == RemoteStructureCleanerKey))
            //    .InNamedScope(DiskScopeName)
            //    ;

            Bind<IBinarySaver<NotVersionedFileAddress>>()
                .To<FileSaver>()
                .InSingletonScope()//.InNamedScope(DiskScopeName)
                ;

            Bind<IAddress, NotVersionedFileAddress>()
                .To<NotVersionedFileAddress>()
                .WhenInjectedInto<IStructureContainerFactory>()
                .InSingletonScope()//.InNamedScope(DiskScopeName)
                //.WithMetadata(AddressMetadataKey, StructureAddressMetadataKey)
                .WithConstructorArgument(
                    "filePath",
                    c => c.Kernel.Get<ServiceSettings>().StructureFilePath
                    )
                ;

            Bind<IStructureContainerFactory>()
                .To<StructureContainerFactory<NotVersionedFileAddress>>()
                .WhenInjectedExactlyInto<DiskFileSystemConnector>()
                .InSingletonScope()//.InNamedScope(DiskScopeName)
                ;

            Bind<IFileSystemConnector, DiskFileSystemConnector>()
                .To<DiskFileSystemConnector>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "rootFolderName",
                    c => c.Kernel.Get<ServiceSettings>().WatchFolderName
                    )
                .WithConstructorArgument(
                    "preRootPath",
                    c => c.Kernel.Get<ServiceSettings>().PreRootPath
                    )
                //.DefinesNamedScope(DiskScopeName)
                ;

            //Bind<IFileSystemSurgeonConnector, DiskFileSystem.DiskFileSystemConnector.LocalSurgeonConnector>()
            //    .To<DiskFileSystem.DiskFileSystemConnector.LocalSurgeonConnector>()
            //    ;

        }

        internal static IFileSystemConnector GetLocalConnector(
            IContext context
            )
        {
            var result = context.Kernel.Get<DiskFileSystemConnector>(
                new IParameter[]
                {
                    new Parameter(
                        Root2.LocalStructureCleanerKey,
                        (object) null,
                        true
                        ),
                }
                );

            return
                result;
        }
    }
}