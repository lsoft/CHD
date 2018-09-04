using System;
using System.Collections.Generic;
using System.IO;
using CHD.Common;
using CHD.Common.Operation;
using CHD.Logger;
using Ninject;
using Ninject.Modules;
using XLogger.Components.FileCycling;
using XLogger.Components.FileProvider;
using XLogger.Components.FileWrapper;
using XLogger.Components.FileWrapper.Text;
using XLogger.Components.Message;
using XLogger.Components.Message.C;
using XLogger.Components.OpenReasoning;
using XLogger.Components.Serializer;
using XLogger.Helper;
using XLogger.Logger;
using XLogger.Logger.ExternalAction;
using XLogger.Logger.File;
using XLogger.Logger.Gate;
using XLogger.Logger.ThreadSafe;

namespace CHD.Client.Gui.CompositionRoot.Module
{
    public sealed class LoggerModule : NinjectModule
    {
        internal const string SeparatorMessage = "............................................";
        internal const string FirstMessage = "...... Журнал успешно инициализирован ......";
        internal const string LastMessage = ".......... Журнал успешно закрыт ...........";
        internal const string JournalLogPath = "Log";

        private readonly int _journalLogMaxFileCount;

        private const int JournalLogMaxFileSize = 16777216;

        public LoggerModule(
            int journalLogMaxFileCount
            )
        {
            _journalLogMaxFileCount = journalLogMaxFileCount;

            var dln = new DirectoryInfo(JournalLogPath).Name;

            if (!Directory.Exists(Path.GetFullPath(dln)))
            {
                Directory.CreateDirectory(Path.GetFullPath(dln));
            }
        }

        public override void Load()
        {
            var logFileFormat = string.Format(
                "{0}.{{0}}.{{1}}.log",
                Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName)
                );


            Bind<IMessageSerializer>()
                .To<DefaultMessageSerializer>()
                .InSingletonScope()
                ;

            Bind<ICommonMessageSettings>()
                .To<CommonMessageSettings>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "separatorMessage",
                    SeparatorMessage
                    )
                .WithConstructorArgument(
                    "firstMessage",
                    FirstMessage
                    )
                .WithConstructorArgument(
                    "lastMessage",
                    LastMessage
                    )
                ;

            Bind<ILogFileWrapperFactory>()
                .To<TextLogFileWrapperFactory>()
                .InSingletonScope()
                ;

            Bind<IFileProvider>()
                .To<DefaultFileProvider>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "logFolder",
                    JournalLogPath
                    )
                .WithConstructorArgument(
                    "fileNameFormat",
                    logFileFormat
                    )
                .WithConstructorArgument(
                    "regexLogFileFormat",
                    logFileFormat
                    )
                ;

            Bind<ILogFilesSettings>()
                .To<LogFilesSettings>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "logFolder",
                    JournalLogPath
                    )
                .WithConstructorArgument(
                    "maxFileCount",
                    _journalLogMaxFileCount
                    )
                .WithConstructorArgument(
                    "maxFileSize",
                    JournalLogMaxFileSize
                    )
                ;

            Bind<ILogMessageFactory>()
                .To<CLogMessageFactory>()
                .InSingletonScope()
                ;

            Bind<IOpenReasoning>()
                .To<DefaultOpenReasoning>()
                .InSingletonScope()
                ;

            Bind<IFileCycling, IWriteable>()
                .To<DefaultFileCycling>()
                .InSingletonScope()
                ;

            Bind<FileMessageLogger>()
                .ToSelf()
                .InSingletonScope()
                ;

            Bind<IMessageLogger>()
                .To<GateLogger>()
                .WhenInjectedExactlyInto<ThreadSafeLogger>()
                .InSingletonScope()
                .WithConstructorArgument(
                    "loggers",
                    c =>
                    {
                        var list = new List<IMessageLogger>();

                        var fml = c.Kernel.Get<FileMessageLogger>();
                        list.Add(fml);

                        return list.ToArray();
                    }
                    )
                ;

            Bind<IMessageLogger>()
                .To<ThreadSafeLogger>()
                .WhenInjectedExactlyInto<DisorderLogger>()
                .InSingletonScope()
                ;
        
            Bind<IDisorderLogger>()
                .To<DisorderLogger>()
                .InSingletonScope()
                ;

            Bind<IOperationDumper>()
                .To<LoggerOperationDumper>()
                .InSingletonScope()
                ;

        }

    }
}
