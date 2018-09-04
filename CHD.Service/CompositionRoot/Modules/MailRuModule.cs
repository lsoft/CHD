using System;
using System.Linq;
using CHD.Common;
using CHD.Common.FileSystem;

using CHD.Common.FileSystem.Surgeon;
using CHD.Common.Letter;
using CHD.Common.Letter.Container.Factory;
using CHD.Common.Letter.Executor;
using CHD.Common.Letter.Factory;
using CHD.Common.Saver;
using CHD.Common.Saver.Body;
using CHD.Common.Saver.Structure;
using CHD.Common.ServiceCode;
using CHD.Common.ServiceCode.Executor;
using CHD.Common.Structure.Cleaner;
using CHD.Common.Structure.Container.Factory;
using CHD.MailRuCloud;
using CHD.MailRuCloud.Letter;
using CHD.MailRuCloud.Native;
using CHD.MailRuCloud.Network;
using CHD.MailRuCloud.Settings;
using CHD.MailRuCloud.Structure;
using CHD.MailRuCloud.Token;
using CHD.Remote.FileSystem;
using CHD.Settings.Controller;
using CHD.Token;
using MailRu.Cloud.WebApi.Connection.Factory;
using Ninject;
//using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using ProxyGenerator.NInject;

namespace CHD.Service.CompositionRoot.Modules
{
    public sealed class MailRuModule : NinjectModule
    {
        //public const string MailRuScopeName = "MailRu.ScopeName";

        private readonly string _remoteRootFolderName;

        public MailRuModule(
            string remoteRootFolderName
            )
        {
            if (remoteRootFolderName == null)
            {
                throw new ArgumentNullException("remoteRootFolderName");
            }

            _remoteRootFolderName = remoteRootFolderName;
        }

        public override void Load()
        {
            Bind<IRemoteFileSystemCleaner>()
                .To<MailRuCleaner>()
                .InSingletonScope()
                ;

            Bind<ISettings>()
                .To<Settings.Controller.Settings>()
                .WhenInjectedExactlyInto<MailRuSettings>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filePath",
                    c => c.Kernel.Get<ServiceSettings>().RemoteFileSystemSettingsFile
                )
                ;

            Bind<MailRuSettings, IVersionedSettings, IRemoteSettings>()
                .To<MailRuSettings>()
                .InSingletonScope()
                ;

            Bind<IStructureCleaner>()
                .To<RemoteStructureCleaner>()
                .When(request => request.Parameters.Any(j => j.Name == Root2.RemoteStructureCleanerKey))
                .InSingletonScope()//.InNamedScope(MailRuScopeName)
                .WithConstructorArgument(
                    "storedStructureCount",
                    c => c.Kernel.Get<MailRuSettings>().StoredSnapshotCount
                    )
                ;

            Bind<IAddress, MailRuAddress>()
                .ToMethod(c => c.Kernel.Get<MailRuSettings>().StructureAddress)
                .WhenInjectedInto<IStructureContainerFactory>()
                .InSingletonScope()//.InNamedScope(MailRuScopeName)
                ;

            //Bind<IMailRuConnectionFactory>()
            //    .To<MailRuConnectionFactory>()
            //    .InSingletonScope()
            //    ;
            Bind<IMailRuConnectionFactory>()
                .ToProxy<IMailRuConnectionFactory, MailRuConnectionFactory>(
                    (methodInfo) => true
                )
                .WhenInjectedExactlyInto<CachedMailRuConnectionFactory>()
                .InSingletonScope()
                ;
            Bind<IMailRuConnectionFactory>()
                .To<CachedMailRuConnectionFactory>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "aliveTimeoutInSeconds",
                    60
                    )
                ;

            //Bind<INativeClientExecutor<MailRuNativeMessage, MailRuSendableMessage>>()
            //    .To<MailRuClientExecutor>()
            //    .InSingletonScope()
            //    ;
            Bind<INativeClientExecutor<MailRuNativeMessage, MailRuSendableMessage>>()
                .ToProxy < INativeClientExecutor<MailRuNativeMessage, MailRuSendableMessage>, MailRuClientExecutor>(
                    (methodInfo) => true
                    )
                .InSingletonScope()
                ;

            Bind<ISendableMessageFactory<MailRuSendableMessage>>()
                .To<MailRuSendableMessageFactory>()
                .InSingletonScope()
                ;

            //Bind<IBinarySaver<MailRuAddress>>()
            //    .To<RemoteSaver<MailRuAddress, MailRuNativeMessage, MailRuSendableMessage>>()
            //    .InSingletonScope()
            //    ;
            Bind<IBinarySaver<MailRuAddress>>()
                .ToProxy < IBinarySaver<MailRuAddress>, RemoteSaver<MailRuAddress, MailRuNativeMessage, MailRuSendableMessage>>(
                    (methodInfo) => true
                    )
                .InSingletonScope()
                ;

            //Bind<ITokenController>()
            //    .To<MailRuTokenController>()
            //    .InSingletonScope()
            //    .Named(MailRuBindingName)
            //    ;
            Bind<ITokenFactory, ITokenController>()
                .ToProxy<ITokenFactory, ITokenController, MailRuTokenController>(
                    (methodInfo) => true
                    )
                .WhenInjectedExactlyInto<StatusTokenController>()
                .InSingletonScope()//.InNamedScope(MailRuScopeName)
                ;

            Bind<ITokenFactory, ITokenController>()
                .To<StatusTokenController>()
                .InSingletonScope()
                ;

            Bind<ILetterFactory<MailRuNativeMessage>>()
                .To<LetterFactory<MailRuNativeMessage, MailRuSendableMessage>>()
                .InSingletonScope()
                ;

            Bind<ILettersContainerFactory<MailRuNativeMessage>>()
                .To<LettersContainerFactory<MailRuNativeMessage>>()
                .InSingletonScope()
                ;

            //Bind<ILetterExecutor<MailRuNativeMessage>>()
            //    .To<RemoteLetterExecutor<MailRuNativeMessage, MailRuSendableMessage>>()
            //    .InSingletonScope()
            //    ;
            Bind<ILetterExecutor<MailRuNativeMessage>>()
                .ToProxy < ILetterExecutor<MailRuNativeMessage>, RemoteLetterExecutor<MailRuNativeMessage, MailRuSendableMessage>>(
                    (methodInfo) => true
                    )
                .InSingletonScope()
                ;

            //Bind<IBodyProcessor>()
            //    .To<BodyProcessor<MailRuNativeMessage>>()
            //    .InSingletonScope()
            //    .Named(MailRuBindingName)
            //    ;
            Bind<IBodyProcessor>()
                .ToProxy<IBodyProcessor, BodyProcessor<MailRuNativeMessage>>(
                    (methodInfo) => true
                    )
                .InSingletonScope()//.InNamedScope(MailRuScopeName)
                ;

            //Bind<IStructureContainerFactory>()
            //    .To<StructureContainerFactory<MailRuAddress>>()
            //    .InSingletonScope()
            //    .Named(MailRuBindingName)
            //    .WithConstructorArgument(
            //        "storedStructureCount",
            //        c => c.Kernel.Get<MailRuSettings>().StoredSnapshotCount
            //    )
            //    ;
            Bind<IStructureContainerFactory>()
                .ToProxy<IStructureContainerFactory, StructureContainerFactory<MailRuAddress>>(
                    (methodInfo) => true
                    )
                .InSingletonScope()//.InNamedScope(MailRuScopeName)
                ;

            Bind<IFileSystemConnector, RemoteFileSystemConnector>()
                .To<RemoteFileSystemConnector>()
                .InSingletonScope()//.InNamedScope(MailRuScopeName)
                .WithConstructorArgument(
                    "name",
                    "MailRu Cloud File System"
                )
                .WithConstructorArgument(
                    "rootFolderName",
                    _remoteRootFolderName
                )
                //.DefinesNamedScope(MailRuScopeName)
                ;

            //Bind<IFileSystemSurgeonConnector, RemoteFileSystem.RemoteFileSystemConnector.RemoteSurgeonConnector>()
            //    .To<RemoteFileSystem.RemoteFileSystemConnector.RemoteSurgeonConnector>()
            //    .DefinesNamedScope(MailRuScopeName)
            //    ;
        }
    }
}