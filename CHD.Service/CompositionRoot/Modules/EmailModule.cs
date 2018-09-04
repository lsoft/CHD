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
using CHD.Email;
using CHD.Email.Letter;
using CHD.Email.Native;
using CHD.Email.Network;
using CHD.Email.Network.Imap;
using CHD.Email.Settings;
using CHD.Email.Structure;
using CHD.Email.Token;
using CHD.Remote.FileSystem;
using CHD.Settings.Controller;
using CHD.Token;
using Ninject;
//using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using ProxyGenerator.NInject;

namespace CHD.Service.CompositionRoot.Modules
{
    public sealed class EmailModule : NinjectModule
    {
        //public const string EmailScopeName = "Email.ScopeName";

        private readonly string _remoteRootFolderName;

        public EmailModule(
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
                .To<EmailCleaner>()
                .InSingletonScope()
                ;

            Bind<ISettings>()
                .To<Settings.Controller.Settings>()
                .WhenInjectedExactlyInto<EmailSettings>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "filePath",
                    c => c.Kernel.Get<ServiceSettings>().RemoteFileSystemSettingsFile
                )
                ;

            Bind<EmailSettings, IRemoteSettings, IVersionedSettings>()
                .To<EmailSettings>()
                .InSingletonScope()
                ;

            Bind<IStructureCleaner>()
                .To<RemoteStructureCleaner>()
                .When(request => request.Parameters.Any(j => j.Name == Root2.RemoteStructureCleanerKey))
                //.WhenInjectedInto<IStructureContainerFactory>()
                .InSingletonScope()//.InNamedScope(EmailScopeName)
                .WithConstructorArgument(
                    "storedStructureCount",
                    c => c.Kernel.Get<EmailSettings>().StoredSnapshotCount
                    )
                ;

            Bind<IAddress, EmailAddress>()
                .ToMethod(c => c.Kernel.Get<EmailSettings>().StructureAddress)
                .WhenInjectedInto<IStructureContainerFactory>()
                .InSingletonScope()//.InNamedScope(EmailScopeName)
                ;

            Bind<IImapClientFactory>()
                .ToProxy<IImapClientFactory, ImapClientFactory>(
                    (methodInfo) => true
                    )
                .WhenInjectedExactlyInto<CachedImapClientFactory>()
                .InSingletonScope()
                //.InNamedScope(EmailScopeName)
                ;
            Bind<IImapClientFactory>()
                .To<CachedImapClientFactory>()
                .InSingletonScope()
                //.InNamedScope(EmailScopeName)
                .WithConstructorArgument(
                    "aliveTimeoutInSeconds",
                    60
                    )
                ;

            //Bind<INativeClientExecutor<EmailNativeMessage, EmailSendableMessage>>()
            //    .To<EmailClientExecutor>()
            //    .InSingletonScope()
            //    ;
            Bind<INativeClientExecutor<EmailNativeMessage, EmailSendableMessage>>()
                .ToProxy<INativeClientExecutor<EmailNativeMessage, EmailSendableMessage>, EmailClientExecutor>(
                    (methodInfo) => true
                    )
                .InSingletonScope()
                //.InNamedScope(EmailScopeName)
                ;

            Bind<ISendableMessageFactory<EmailSendableMessage>>()
                .To<EmailSendableMessageFactory>()
                .InSingletonScope()
                //.InNamedScope(EmailScopeName)
                ;

            //Bind<IBinarySaver<EmailAddress>>()
            //    .To<RemoteSaver<EmailAddress, EmailNativeMessage, EmailSendableMessage>>()
            //    .InSingletonScope()
            //    ;
            Bind<IBinarySaver<EmailAddress>>()
                .ToProxy<IBinarySaver<EmailAddress>, RemoteSaver<EmailAddress, EmailNativeMessage, EmailSendableMessage>>(
                    (methodInfo) => true
                    )
                .InSingletonScope()
                //.InNamedScope(EmailScopeName)
                ;

            //Bind<ITokenFactory, ITokenController>()
            //    .To<EmailTokenController<EmailNativeMessage, EmailSendableMessage>>()
            //    .InNamedScope(EmailScopeName)
            //    ;
            Bind<ITokenFactory, ITokenController>()
                .ToProxy<ITokenFactory, ITokenController, EmailTokenController<EmailNativeMessage, EmailSendableMessage>>(
                    (methodInfo) => true
                    )
                .WhenInjectedExactlyInto<StatusTokenController>()
                .InSingletonScope()//.InNamedScope(EmailScopeName)
                ;
            //Bind<ITokenFactory>()
            //    .ToMethod(c => c.Kernel.Get<ITokenController>())
            //    .InNamedScope(EmailScopeName)
            //    ;

            Bind<ITokenFactory, ITokenController>()
                .To<StatusTokenController>()
                .InSingletonScope()
                ;

            Bind<ILetterFactory<EmailNativeMessage>>()
                .To<LetterFactory<EmailNativeMessage, EmailSendableMessage>>()
                .InSingletonScope()
                //.InNamedScope(EmailScopeName)
                ;

            Bind<ILettersContainerFactory<EmailNativeMessage>>()
                .To<LettersContainerFactory<EmailNativeMessage>>()
                .InSingletonScope()
                //.InNamedScope(EmailScopeName)
                ;

            //Bind<ILetterExecutor<EmailNativeMessage>>()
            //    .To<RemoteLetterExecutor<EmailNativeMessage, EmailSendableMessage>>()
            //    .InSingletonScope()
            //    ;
            Bind<ILetterExecutor<EmailNativeMessage>>()
                .ToProxy<ILetterExecutor<EmailNativeMessage>, RemoteLetterExecutor<EmailNativeMessage, EmailSendableMessage>>(
                    (methodInfo) => true
                    )
                .InSingletonScope()
                //.InNamedScope(EmailScopeName)
                ;

            //Bind<IBodyProcessor>()
            //    .To<BodyProcessor<EmailNativeMessage>>()
            //    .InSingletonScope()
            //    .Named(EmailBindingName)
            //    ;
            Bind<IBodyProcessor>()
                .ToProxy<IBodyProcessor, BodyProcessor<EmailNativeMessage>>(
                    (methodInfo) => true
                    )
                .InSingletonScope()//.InNamedScope(EmailScopeName)
                ;

            //Bind<IStructureContainerFactory>()
            //    .To<StructureContainerFactory<EmailAddress>>()
            //    .InSingletonScope()
            //    .Named(EmailBindingName)
            //    .WithConstructorArgument(
            //        "storedStructureCount",
            //        c => c.Kernel.Get<EmailSettings>().StoredSnapshotCount
            //    )
            //    ;
            Bind<IStructureContainerFactory>()
                .ToProxy<IStructureContainerFactory, StructureContainerFactory<EmailAddress>>(
                    (methodInfo) => true
                    )
                .WhenInjectedExactlyInto<RemoteFileSystemConnector>()
                .InSingletonScope()//.InNamedScope(EmailScopeName)
                ;

            Bind<IFileSystemConnector, RemoteFileSystemConnector>()
                .To<RemoteFileSystemConnector>()
                .InSingletonScope()//.InNamedScope(EmailScopeName)
                .WithConstructorArgument(
                    "name",
                    "Email File System"
                )
                .WithConstructorArgument(
                    "rootFolderName",
                    _remoteRootFolderName
                )
                //.DefinesNamedScope(EmailScopeName)
                ;

            //Bind<IFileSystemSurgeonConnector, RemoteFileSystem.RemoteFileSystemConnector.RemoteSurgeonConnector>()
            //    .To<RemoteFileSystem.RemoteFileSystemConnector.RemoteSurgeonConnector>()
            //    .DefinesNamedScope(EmailScopeName)
            //    ;
        }

    }
}