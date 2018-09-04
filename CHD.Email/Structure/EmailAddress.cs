using System;
using System.Collections.Generic;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Email.Structure
{
    public sealed class EmailAddress : IAddress
    {
        public string Email
        {
            get;
            private set;
        }

        public string TargetFolder
        {
            get;
            private set;
        }

        public string EmailPrefix
        {
            get;
            private set;
        }

        public Func<string, bool> Filter
        {
            get
            {
                return IsFilterPassed;
            }
        }

        public EmailAddress(
            string email,
            string targetFolder,
            string emailPrefix
            )
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }
            if (targetFolder == null)
            {
                throw new ArgumentNullException("targetFolder");
            }
            if (emailPrefix == null)
            {
                throw new ArgumentNullException("emailPrefix");
            }

            Email = email;
            TargetFolder = targetFolder;
            EmailPrefix = emailPrefix;
        }

        private bool IsFilterPassed(
            string subject
            )
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            if (subject.StartsWith(EmailPrefix, StringComparison.OrdinalIgnoreCase))
            {
                //это то сообщение, что нам надо
                return true;
            }

            return false;
        }

        public int ParseIndex(string subject)
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            var index = int.Parse(subject.Substring(EmailPrefix.Length + 1));

            return
                index;
        }

        public string ComposeNewSubject()
        {
            var result = ComposeNewSubject(null);

            return
                result;
        }

        public IComparer<TNativeMessage> GetComparer<TNativeMessage>()
            where TNativeMessage : NativeMessage
        {
            return
                new EmailComparer<TNativeMessage>(this);
        }

        public string ComposeNewSubject(string subject)
        {
            var index = 0;

            if (subject != null)
            {
                index = ParseIndex(subject);
            }

            var result = string.Format(
                "{0} {1}",
                EmailPrefix,
                index + 1
                );

            return
                result;
        }

        private sealed class EmailComparer<TNativeMessage> : IComparer<TNativeMessage>
            where TNativeMessage : NativeMessage
        {
            private readonly EmailAddress _emailAddress;

            public EmailComparer(
                EmailAddress emailAddress
                )
            {
                if (emailAddress == null)
                {
                    throw new ArgumentNullException("emailAddress");
                }
                _emailAddress = emailAddress;
            }

            public int Compare(
                TNativeMessage leftValue,
                TNativeMessage rightValue
                )
            {
                if (leftValue == null)
                {
                    throw new ArgumentNullException("leftValue");
                }
                if (rightValue == null)
                {
                    throw new ArgumentNullException("rightValue");
                }

                var oldSubject = leftValue.Subject;
                var newSubject = rightValue.Subject;

                if (oldSubject == null)
                {
                    throw new ArgumentNullException("oldSubject");
                }
                if (newSubject == null)
                {
                    throw new ArgumentNullException("newSubject");
                }

                if (!_emailAddress.IsFilterPassed(oldSubject))
                {
                    //это совсем не то сообщение, что нам надо
                    throw new ArgumentException("newSubject");
                }
                if (!_emailAddress.IsFilterPassed(newSubject))
                {
                    //это совсем не то сообщение, что нам надо
                    throw new ArgumentException("newSubject");
                }

                //это нужное нам сообщение

                var oldindex = _emailAddress.ParseIndex(oldSubject);
                var newindex = _emailAddress.ParseIndex(newSubject);

                if (oldindex < newindex)
                {
                    //новое свежее
                    return -1;
                }

                if (oldindex == newindex)
                {
                    //одинаково
                    return 0;
                }

                //старое свежее
                return 1;
            }

        }

    }
}