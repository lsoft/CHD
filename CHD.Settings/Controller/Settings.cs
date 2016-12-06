using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CHD.Common.Crypto;

namespace CHD.Settings.Controller
{
    public class Settings : ISettings
    {
        private readonly string _filePath;
        private readonly XmlDocument _xmld;
        private readonly List<ISettingRecord> _records;

        public IReadOnlyCollection<ISettingRecord> Records
        {
            get
            {
                return
                    _records.AsReadOnly();
            }
        }

        public Settings(
            string filePath,
            ICrypto crypto
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (crypto == null)
            {
                throw new ArgumentNullException("crypto");
            }

            _filePath = filePath;
            _xmld = LoadXmlDocument(filePath, crypto);
            _records = GetRecords(_xmld);
        }

        public void Update(
            ISettingRecordInner updated
            )
        {
            var f = _records.FirstOrDefault(j => string.Compare(j.Name, updated.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (f != null)
            {
                f.UpdateValue(updated.Value);
            }
        }

        public void Rewrite(
            ICrypto crypto
            )
        {
            if (crypto == null)
            {
                throw new ArgumentNullException("crypto");
            }

            foreach (var s in _records)
            {
                var node = _xmld.SelectSingleNode(
                    string.Format(
                        "/settings/setting[@name='{0}']/value",
                        s.Name
                        )
                    );

                node.InnerText = s.Value;
            }

            using (var ms = new MemoryStream())
            {
                _xmld.Save(ms);

                var fileBody = ms.ToArray();

                var encodedFileBody = crypto.EncodeBuffer(fileBody);
                //var decodedFileBody = _crypto.DecodeBuffer(encodedFileBody);
                
                File.WriteAllBytes(_filePath, encodedFileBody);
            }
        }

        public void Export(
            IDictionary<string, Action<string>> actions
            )
        {
            if (actions == null)
            {
                throw new ArgumentNullException("actions");
            }

            foreach (var r in _records)
            {
                Action<string> action;
                if (actions.TryGetValue(r.Name, out action))
                {
                    action(r.Value);
                }
            }
        }

        private List<ISettingRecord> GetRecords(
            XmlDocument xmld
            )
        {
            if (xmld == null)
            {
                throw new ArgumentNullException("xmld");
            }

            var result = new List<ISettingRecord>();

            foreach (XmlNode node in xmld.SelectNodes("/settings/setting"))
            {
                var arg = node.Attributes["name"].InnerText;
                var value = node.SelectSingleNode("value").InnerText;

                var comment = string.Empty;
                var commentNode = node.Attributes["comment"];
                if (commentNode != null)
                {
                    comment = commentNode.InnerText;
                }

                var preferredValue = string.Empty;
                var preferredNode = node.SelectSingleNode("preferredvalue");
                if (preferredNode != null)
                {
                    preferredValue = preferredNode.InnerText;
                }

                var values = new List<string>();
                foreach (XmlNode vvn in node.SelectNodes("values/value"))
                {
                    var v = vvn.InnerText;
                    values.Add(v);
                }

                var sr = new SettingRecord(
                    arg,
                    value,
                    comment,
                    preferredValue,
                    values
                    );

                result.Add(sr);
            }

            return
                result;
        }

        private XmlDocument LoadXmlDocument(string filePath, ICrypto crypto)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (crypto == null)
            {
                throw new ArgumentNullException("crypto");
            }

            var xmld = new XmlDocument();

            //try
            //{
            //    xmld.Load(filePath);

            //}
            //catch (XmlException)
            //{
                var encodedFileBody = File.ReadAllBytes(filePath);
                var decodedFileBody = crypto.DecodeBuffer(encodedFileBody);

                using (var ms = new MemoryStream(decodedFileBody))
                {
                    xmld.Load(ms);
                }
            //}

            return
                xmld;

        }
    }
}