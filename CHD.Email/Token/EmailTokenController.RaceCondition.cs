using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CHD.Common.Graveyard.Token;
using CHD.Common.Graveyard.Token.Factory;
using CHD.Common.Graveyard.Token.Releaser;
using CHD.Common.Logger;
using CHD.Email.ServiceCode;
using CHD.Email.Token.UidProvider;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Search;
using MimeKit;

namespace CHD.Email.Token
{
    public class EmailTokenController : ITokenController
    {
        internal const string TokenSubject = "$token";

        private readonly object _locker = new object();

        private readonly IBackgroundReleaser _releaser;
        private readonly IUidProvider _uidProvider;
        private readonly EmailSettings _settings;
        private readonly IDisorderLogger _logger;

        private volatile bool _tokenEmailProcessed0 = false;
        private volatile bool _tokenEmailProcessed1 = false;

        public EmailTokenController(
            IBackgroundReleaser releaser,
            IUidProvider uidProvider,
            EmailSettings settings,
            IDisorderLogger logger
            )
        {
            if (releaser == null)
            {
                throw new ArgumentNullException("releaser");
            }
            if (uidProvider == null)
            {
                throw new ArgumentNullException("uidProvider");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _releaser = releaser;
            _uidProvider = uidProvider;
            _settings = settings;
            _logger = logger;
        }

        public bool TryToObtainToken(
            out IToken token
            )
        {
            if (!TryToObtainTokenInternal())
            {
                token = null;
                return false;
            }

            token = new ActionToken(
                () => _releaser.TryToReleaseAtBackgroundThread(TryToReleaseTokenInternal)
                );

            return true;
        }

        public bool TryToReleaseToken()
        {
            return
                TryToReleaseTokenInternal();
        }

        private bool _goon = false;

        private bool TryToObtainTokenInternal(
            )
        {
            var r0 = false;
            var t0 = new Thread(
                () =>
                {
                    r0 = TryToObtainTokenInternal0();
                });

            var r1 = false;
            var t1 = new Thread(
                () =>
                {
                    r1 = TryToObtainTokenInternal1();
                });

            t0.Start();
            t1.Start();

            Thread.Sleep(250);
            Volatile.Write(ref _goon, true);


            t0.Join();
            t1.Join();

            Debug.WriteLine("{0} {1}", r0, r1);

            return 
                r0 || r1;
        }

        private bool TryToObtainTokenInternal0(
            )
        {
            //var result = false;
            //using (var pop3 = new Pop3Client())
            //{
            //    pop3.Connect("pop.mail.ru", 995, true);

            //    // Note: since we don't have an OAuth2 token, disable
            //    // the XOAUTH2 authentication mechanism.
            //    pop3.AuthenticationMechanisms.Remove("XOAUTH2");

            //    pop3.Authenticate(_settings.Email, _settings.Password);

            //    if (pop3.Count > 0)
            //    {
            //        try
            //        {
            //            pop3.DeleteMessage(0);

            //            result = true;
            //        }
            //        catch
            //        {
            //            //nothing shoud be performed here
            //        }
            //    }

            //    pop3.Disconnect(true);
            //}

            //return
            //    result;

            using (var client = new ImapClientEx(_settings, _logger))
            {
                client.Connect();

                //UniqueId uid;
                //if (!_uidProvider.GetTokenUid(client, out uid))
                //{
                //    return
                //        false;
                //}

                var result = false;

                //lock (_locker)
                {
                    try
                    {
                        while (!Volatile.Read(ref _goon)) { }

                        var folder = client.Client.Inbox.Create(
                            "XXX",
                            false
                            );

                        result = true;
                    }
                    catch (ImapCommandException)
                    {
                        //should nothing to do
                    }

                    //_tokenEmailProcessed0 = false;
                    //client.Inbox.MessageFlagsChanged += InboxOnMessageFlagsChanged0;
                    //try
                    //{
                    //    while (!Volatile.Read(ref _goon))
                    //    {

                    //    }


                    //    var task = client.Inbox.AddFlagsAsync(uid, MessageFlags.Deleted, false);

                    //    task.Wait();

                    //    if (!_tokenEmailProcessed0)
                    //    {
                    //        //in case of no callback invocation occurs
                    //        //take an additional timeout for the delayed callback invocation
                    //        //(it is possibly for events and callbacks to be delayed for some reason)
                    //        Thread.Sleep(500);
                    //    }
                    //}
                    //finally
                    //{
                    //    client.Inbox.MessageFlagsChanged -= InboxOnMessageFlagsChanged0;
                    //}

                    //result = _tokenEmailProcessed0;
                }

                return
                    result;
            }
        }

        private bool TryToObtainTokenInternal1(
            )
        {
            using (var client = new ImapClientEx(_settings, _logger))
            {
                client.Connect();

                var result = false;

                //lock (_locker)
                {
                    try
                    {
                        while (!Volatile.Read(ref _goon)) { }

                        var folder = client.Client.Inbox.Create(
                            "XXX",
                            false
                            );

                        result = true;
                    }
                    catch (ImapCommandException)
                    {
                        //should nothing to do
                    }
                }

                return
                    result;
            }

            //var result = false;
            //using (var pop3 = new Pop3Client())
            //{
            //    pop3.Connect("pop.mail.ru", 995, true);

            //    // Note: since we don't have an OAuth2 token, disable
            //    // the XOAUTH2 authentication mechanism.
            //    pop3.AuthenticationMechanisms.Remove("XOAUTH2");

            //    pop3.Authenticate(_settings.Email, _settings.Password);

            //    if (pop3.Count > 0)
            //    {
            //        try
            //        {
            //            pop3.DeleteMessage(0);

            //            result = true;
            //        }
            //        catch
            //        {
            //            //nothing shoud be performed here
            //        }
            //    }

            //    pop3.Disconnect(true);
            //}

            //return
            //    result;

            //using (var client = new ImapClientEx(_settings, _logger))
            //{
            //    client.Connect();
                
            //    UniqueId uid;
            //    if (!_uidProvider.GetTokenUid(client, out uid))
            //    {
            //        return
            //            false;
            //    }

            //    var result = false;
                
            //    //lock (_locker)
            //    {
            //        _tokenEmailProcessed1 = false;
            //        client.Inbox.MessageFlagsChanged += InboxOnMessageFlagsChanged1;
            //        try
            //        {
            //            while (!Volatile.Read(ref _goon))
            //            {

            //            }

            //            var task = client.Inbox.AddFlagsAsync(uid, MessageFlags.Deleted, false);

            //            task.Wait();

            //            if (!_tokenEmailProcessed1)
            //            {
            //                //in case of no callback invocation occurs
            //                //take an additional timeout for the delayed callback invocation
            //                //(it is possibly for events and callbacks to be delayed for some reason)
            //                Thread.Sleep(500);
            //            }
            //        }
            //        finally
            //        {
            //            client.Inbox.MessageFlagsChanged -= InboxOnMessageFlagsChanged1;
            //        }

            //        result = _tokenEmailProcessed1;
            //    }

            //    return
            //        result;
            //}
        }

        private void InboxOnMessageFlagsChanged0(
            object sender,
            MessageFlagsChangedEventArgs e
            )
        {
            Debug.WriteLine("0: Flags {0}, UserFlags {1}, Index {2}, ModSeq {3}", e.Flags, string.Join(" # ", e.UserFlags.ToArray()), e.Index, e.ModSeq);

            _tokenEmailProcessed0 = true;
        }

        private void InboxOnMessageFlagsChanged1(
            object sender, 
            MessageFlagsChangedEventArgs e
            )
        {
            Debug.WriteLine("1: Flags {0}, UserFlags {1}, Index {2}, ModSeq {3}", e.Flags, string.Join(" # ", e.UserFlags.ToArray()), e.Index, e.ModSeq);

            _tokenEmailProcessed1 = true;
        }

        private bool TryToReleaseTokenInternal()
        {
            var result = false;

            try
            {
                if (IsTokenFree())
                {
                    //token already free

                    return
                        true;
                }


                using (var client = new SmtpClientEx(_settings, _logger))
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(string.Empty, _settings.Email));
                    message.To.Add(new MailboxAddress(string.Empty, _settings.Email));
                    message.Subject = TokenSubject;
                    message.Body = new TextPart("plain")
                    {
                        Text = "token has been returned automatically"
                    };

                    client.Send(
                        message
                        );
                }

                result = true;
            }
            catch (Exception excp)
            {
                _logger.LogException(excp);
            }

            return
                result;
        }

        private bool IsTokenFree(
            )
        {
            using (var client = new ImapClientEx(_settings, _logger))
            {
                client.Connect();

                UniqueId uid;

                return
                    _uidProvider.GetTokenUid(client, out uid);
            }
        }
    }
}