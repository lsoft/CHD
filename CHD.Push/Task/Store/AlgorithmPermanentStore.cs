using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CHD.Push.Task.Factory;

namespace CHD.Push.Task.Store
{
    public class AlgorithmPermanentStore : IAlgorithmPermanentStore
    {
        private readonly string _filePath;
        private readonly IAlgorithmFactory _algorithmFactory;

        public AlgorithmPermanentStore(
            string filePath,
            IAlgorithmFactory algorithmFactory
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (algorithmFactory == null)
            {
                throw new ArgumentNullException("algorithmFactory");
            }

            _filePath = filePath;
            _algorithmFactory = algorithmFactory;

            var fi = new FileInfo(filePath);
            if (fi.Directory != null)
            {
                var di = fi.Directory.FullName;
                if (!Directory.Exists(di))
                {
                    Directory.CreateDirectory(di);
                }
            }
        }

        public IEnumerable<IAlgorithm> Load(
            )
        {
            var result = new List<IAlgorithm>();

            if (File.Exists(_filePath))
            {
                var doc = new XmlDocument();
                doc.Load(_filePath);

                var algorithmNodes = doc.SelectSingleNode("/algorithms");
                foreach (XmlNode algorithmNode in algorithmNodes.ChildNodes)
                {
                    var pusher = _algorithmFactory.Load(
                        algorithmNode
                        );

                    result.Add(pusher);
                }

                File.Delete(_filePath);
            }

            return
                result;
        }

        public void Save(
            IAlgorithm algorithm
            )
        {
            var doc = new XmlDocument();

            if (File.Exists(_filePath))
            {
                doc.Load(_filePath);
            }
            else
            {
                doc.LoadXml(MainXml);
            }

            var algorithmNodes = doc.SelectSingleNode("/algorithms");
            
            var algorithmNode = doc.CreateNode(
                XmlNodeType.Element,
                "algorithm",
                null
                );

            algorithm.Serialize(algorithmNode);

            algorithmNodes.AppendChild(algorithmNode);

            doc.Save(_filePath);
        }

        private const string MainXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<algorithms>
</algorithms>
";
    }
}
