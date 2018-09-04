using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CHD.Common.Crypto;

namespace CHD.Settings.Controller
{
    public sealed class Settings : ISettings
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
            string filePath
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            _filePath = filePath;
            _xmld = LoadXmlDocument(filePath);
            _records = GetRecords(_xmld);
        }

        public void Update(
            ISettingRecordInner updated
            )
        {
            var f = _records.FirstOrDefault(j => string.Compare(j.Name, updated.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (f != null)
            {
                f.UpdateValues(updated.Values);
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
                var joint = _xmld.SelectSingleNode(
                    string.Format(
                        "/settings/setting[@name='{0}']",
                        s.Name
                        )
                    );

                var nodes = joint.SelectNodes(
                    "value"
                    );

                foreach (XmlNode node in nodes)
                {
                    joint.RemoveChild(node);
                }

                foreach (var v in s.Values)
                {
                    var newNode = _xmld.CreateNode(
                        XmlNodeType.Element,
                        "value",
                        string.Empty
                        );
                    newNode.InnerText = v;

                    joint.AppendChild(newNode);
                }
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
                    foreach (var v in r.Values)
                    {
                        action(v);
                    }
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
                var values = new List<string>();
                foreach (XmlNode vnode in node.SelectNodes("value"))
                {
                    values.Add(vnode.InnerText);
                }

                var comment = string.Empty;
                var commentNode = node.Attributes["comment"];
                if (commentNode != null)
                {
                    comment = commentNode.InnerText;
                }

                var allowManyChildren = false;
                var allowManyChildrenNode = node.Attributes["allowManyChildren"];
                if (allowManyChildrenNode != null)
                {
                    allowManyChildren = bool.Parse(allowManyChildrenNode.InnerText);
                }

                var preferredValue = string.Empty;
                var preferredNode = node.SelectSingleNode("preferredvalue");
                if (preferredNode != null)
                {
                    preferredValue = preferredNode.InnerText;
                }

                var predefinedValues = new List<string>();
                foreach (XmlNode vvn in node.SelectNodes("predefined/value"))
                {
                    var v = vvn.InnerText;
                    predefinedValues.Add(v);
                }

                var sr = new SettingRecord(
                    arg,
                    values,
                    allowManyChildren,
                    comment,
                    preferredValue,
                    predefinedValues
                    );

                result.Add(sr);
            }

            return
                result;
        }

        private XmlDocument LoadXmlDocument(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            var xmld = new XmlDocument();

            var decodedFileBody = File.ReadAllBytes(filePath);
            //var encodedFileBody = File.ReadAllBytes(filePath);
            //var decodedFileBody = crypto.DecodeBuffer(encodedFileBody);

            using (var ms = new MemoryStream(decodedFileBody))
            {
                xmld.Load(ms);
            }

            return
                xmld;

        }
    }
}