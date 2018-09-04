using System;
using System.Collections.Generic;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.MailRuCloud.Structure
{
    public sealed class MailRuAddress : IAddress
    {
        private readonly string _prefix;

        public string TargetFolder
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

        public MailRuAddress(
            string targetFolder,
            string prefix
            )
        {
            if (targetFolder == null)
            {
                throw new ArgumentNullException("targetFolder");
            }
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            TargetFolder = targetFolder;
            _prefix = prefix;
        }

        private bool IsFilterPassed(
            string subject
            )
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            if (subject.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
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

            var index = int.Parse(subject.Substring(_prefix.Length + 1));

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
                new MailRuComparer<TNativeMessage>(this);
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
                _prefix,
                index + 1
                );

            return
                result;
        }

        private sealed class MailRuComparer<TNativeMessage> : IComparer<TNativeMessage>
            where TNativeMessage : NativeMessage
        {
            private readonly MailRuAddress _address;

            public MailRuComparer(
                MailRuAddress address
                )
            {
                if (address == null)
                {
                    throw new ArgumentNullException("address");
                }
                _address = address;
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

                if (!_address.IsFilterPassed(oldSubject))
                {
                    //это совсем не то сообщение, что нам надо
                    throw new ArgumentException("newSubject");
                }
                if (!_address.IsFilterPassed(newSubject))
                {
                    //это совсем не то сообщение, что нам надо
                    throw new ArgumentException("newSubject");
                }

                //это нужное нам сообщение

                var oldindex = _address.ParseIndex(oldSubject);
                var newindex = _address.ParseIndex(newSubject);

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