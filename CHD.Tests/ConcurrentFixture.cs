using System.Diagnostics;
using System.Threading;
using CHD.Common.Logger;
using CHD.Email.ServiceCode;
using CHD.Email.Token;
using CHD.Graveyard.Token;
using CHD.Graveyard.Token.Factory;
using CHD.MailRuCloud.ServiceCode;
using CHD.MailRuCloud.Token;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CHD.Tests
{
    [TestClass]
    public class ConcurrentFixture
    {
        [TestMethod]
        public void EmailTest()
        {
            //var settings = new EmailSettings("put your email here", "put your password here", "imap.mail.ru", 993);
            var settings = new EmailSettings("put your email here", "put your password here", "put you imap server here", -1, 2048, 1024); //replace -1 with an imap port
            var logger = new Mock<IDisorderLogger>();

            
            var backgroundReleaser0 = new TestBackgroundReleaser();
            var tokenController0 = new EmailTokenController(
                backgroundReleaser0,
                settings,
                logger.Object
                );

            var backgroundReleaser1 = new TestBackgroundReleaser();
            var tokenController1 = new EmailTokenController(
                backgroundReleaser1,
                settings,
                logger.Object
                );

            DoManyTests(
                tokenController0,
                tokenController1
                );
        }

        [TestMethod]
        public void MailRuCloudTest()
        {
            var settings = new MailRuSettings("put your mail.ru cloud login here", "put your mail.ru cloud password here", 1024 * 1024 * 2, 1024 * 1024);
            var logger = new Mock<IDisorderLogger>();


            var backgroundReleaser0 = new TestBackgroundReleaser();
            var tokenController0 = new MailRuTokenController(
                backgroundReleaser0,
                settings,
                logger.Object
                );

            var backgroundReleaser1 = new TestBackgroundReleaser();
            var tokenController1 = new MailRuTokenController(
                backgroundReleaser1,
                settings,
                logger.Object
                );

            DoManyTests(
                tokenController0, 
                tokenController1
                );
        }

        private void DoManyTests(
            ITokenController tokenController0,
            ITokenController tokenController1
            )
        {
            const int Iteration = 100;

            for (var i = 0; i < Iteration; i++)
            {
                var payload = new ThreadPayload(
                    tokenController0,
                    tokenController1
                    );

                var isOk = DoSingleTest(payload);

                Assert.IsTrue(isOk);

                Debug.WriteLine(string.Empty);
            }

            Debug.WriteLine(
                "{0} iterations is OK! Feel dry bro it's working!",
                Iteration
                );
        }

        private bool DoSingleTest(
            ThreadPayload payload
            )
        {
            var t0 = new Thread(WorkThread);
            t0.Start(payload);

            var t1 = new Thread(WorkThread);
            t1.Start(payload);

            Thread.Sleep(500);

            payload.LetShowBegin();

            Thread.Sleep(100);

            while (!payload.IsAllowedToCleanup())
            {
                //cycling until IsAllowedToCleanup fired!
                Thread.Yield();
            }

            t0.Join();
            t1.Join();

            var isOk = payload.IsTestOk();

            return
                isOk;
        }

        private void WorkThread(object tc)
        {
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;

                var payload = tc as ThreadPayload;

                while (!payload.IsAllowedToStartIteration())
                {
                    //cycling until IsAllowedToStartIteration fired!
                }

                var tokenController = payload.GetMyTokenController();

                IToken token;
                var obtainResult = tokenController.TryToObtainToken(
                    out token
                    );

                payload.NotifyAboutResult(obtainResult);

                while (!payload.IsAllowedToCleanup())
                {
                    //cycling until IsAllowedToCleanup fired!
                }

                if (obtainResult)
                {
                    token.Dispose();
                }
            }
            finally
            {
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
            }
        }
    }
}
