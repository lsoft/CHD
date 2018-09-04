using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common;
using CHD.Common.ServiceCode;
using CHD.Common.ServiceCode.Executor;
using CHD.MailRuCloud.Native;
using CHD.MailRuCloud.Settings;
using CHD.MailRuCloud.Token;

namespace CHD.MailRuCloud
{
    public sealed class MailRuCleaner : IRemoteFileSystemCleaner
    {
        private readonly INativeClientExecutor<MailRuNativeMessage, MailRuSendableMessage> _executor;
        private readonly MailRuSettings _settings;
        private readonly IDisorderLogger _logger;

        public MailRuCleaner(
            INativeClientExecutor<MailRuNativeMessage, MailRuSendableMessage> executor,
            MailRuSettings settings,
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
                            MailRuTokenController.TokenFolder,
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
