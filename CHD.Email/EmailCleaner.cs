using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common;
using CHD.Common.ServiceCode;
using CHD.Common.ServiceCode.Executor;
using CHD.Email.Native;
using CHD.Email.Settings;
using CHD.Email.Token;

namespace CHD.Email
{
    public sealed class EmailCleaner : IRemoteFileSystemCleaner
    {
        private readonly INativeClientExecutor<EmailNativeMessage, EmailSendableMessage> _executor;
        private readonly EmailSettings _settings;
        private readonly IDisorderLogger _logger;

        public EmailCleaner(
            INativeClientExecutor<EmailNativeMessage, EmailSendableMessage> executor,
            EmailSettings settings,
            IDisorderLogger logger
            )
        {
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _executor = executor;
            _settings = settings;
            _logger = logger;
        }

        public void SafelyClear()
        {
            _logger.LogMessage("+++++++++++++++++++++  CLEANUP STARTED  +++++++++++++++++++++");

            try
            {
                _executor.Execute(
                    client =>
                    {
                        //очищаем основную папку с телами файлов
                        var messages = client.ReadAndFilterMessages(SubjectComposer.IsSubjectCanBeParsed);
                        client.DeleteMessages(messages);

                        //удаляем папку токена
                        string unused;
                        client.DeleteChildFolder(
                            EmailTokenController<EmailNativeMessage, EmailSendableMessage>.TokenFolder,
                            out unused
                            );

                        //удаляем папку структуры
                        client.DeleteChildFolder(
                            _settings.StructureFolder,
                            out unused
                            );
                    });
            }
            catch (Exception excp)
            {
                _logger.LogException(excp, "Cleanup FAILS!");
            }

            _logger.LogMessage("+++++++++++++++++++++  CLEANUP COMPLETED  +++++++++++++++++++++");
        }
    }
}
