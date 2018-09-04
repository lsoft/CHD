using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using CHD.Common;
using CHD.Common.Others;

namespace CHD.Plugin.Infrastructure
{
    /// <summary>
    /// ��������� ��������
    /// </summary>
    public sealed class PluginLoader : IPluginLoader
    {
        #region private class

        /// <summary>
        /// �������� �������
        /// </summary>
#if !WindowsCE
        [DebuggerDisplay("{PluginGuid} {RootFilePath}")]
#endif
        private sealed class PluginXmlDescription : IPluginXmlDescription
        {
            /// <summary>
            /// ������������� �������
            /// </summary>
            public Guid PluginGuid
            {
                get;
                private set;
            }

            /// <summary>
            /// �������� �������
            /// </summary>
            public string Description
            {
                get;
                private set;
            }

            /// <summary>
            /// ��������� dll �������
            /// </summary>
            public string RootFilePath
            {
                get;
                private set;
            }

            /// <summary>
            /// ��������� ����� �������, ����������� ICHDPlugin
            /// </summary>
            public string RootClass
            {
                get;
                private set;
            }


            /// <summary>
            /// ��������������, �� ������� ������� ������
            /// </summary>
            public List<Guid> Dependencies
            {
                get;
                private set;
            }

            public PluginXmlDescription(
                Guid pluginGuid,
                string description,
                string rootFilePath,
                string rootClass,
                List<Guid> dependencies
                )
            {
                if (description == null)
                {
                    throw new ArgumentNullException("description");
                }
                if (rootFilePath == null)
                {
                    throw new ArgumentNullException("rootFilePath");
                }
                if (rootClass == null)
                {
                    throw new ArgumentNullException("rootClass");
                }
                if (dependencies == null)
                {
                    throw new ArgumentNullException("dependencies");
                }

                PluginGuid = pluginGuid;
                Description = description;
                RootFilePath = rootFilePath;
                RootClass = rootClass;
                Dependencies = dependencies;
            }

            #region equality

            protected bool Equals(PluginXmlDescription other)
            {
                return PluginGuid.Equals(other.PluginGuid);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }
                return Equals((PluginXmlDescription) obj);
            }

            public override int GetHashCode()
            {
                return PluginGuid.GetHashCode();
            }

            #endregion
        }

        #endregion

        private readonly IDisorderLogger _logger;

        public PluginLoader(
            IDisorderLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;
        }

        /// <summary>
        /// �������� �������� �������� �� �����
        /// </summary>
        /// <param name="pluginRootFolder">�����, � ������� ����� �������</param>
        /// <returns>������ �������� ��������</returns>
        public List<IPluginXmlDescription> LoadFrom(string pluginRootFolder)
        {
            if (pluginRootFolder == null)
            {
                throw new ArgumentNullException("pluginRootFolder");
            }

            var result = new List<IPluginXmlDescription>();

            foreach (var dir in Directory.GetDirectories(pluginRootFolder))
            {
                try
                {
                    var descriptionXmlFile = Path.Combine(dir, "description.xml");

                    using (var reader = XmlReader.Create(descriptionXmlFile))
                    {
                        var xmld = new XmlDocument();
                        xmld.Load(reader);

#if !WindowsCE
                        var guid = Guid.Parse(xmld.SelectSingleNode("/plugin/guid").InnerText);
#else
                        var guid = new Guid(xmld.SelectSingleNode("/plugin/guid").InnerText);
#endif
                        var description = xmld.SelectSingleNode("/plugin/description").InnerText;
                        var rootFile = xmld.SelectSingleNode("/plugin/root/file").InnerText;
                        var rootClass = xmld.SelectSingleNode("/plugin/root/class").InnerText;

                        var dependencyGuidList = new List<Guid>();

                        var dependencies = xmld.SelectNodes("/plugin/dependencies/dependency");
                        if (dependencies != null && dependencies.Count > 0)
                        {
                            foreach (XmlNode dep in dependencies)
                            {
#if !WindowsCE
                                var dg = Guid.Parse(dep.InnerText);
#else
                                var dg = GuidHelper.Parse(dep.InnerText);
#endif
                                if (dg == guid)
                                {
                                    throw new CHDException(
                                        string.Format(
                                            "���� ����������� ��������� �� ����: {0}",
                                            dg
                                            ),
                                        CHDExceptionTypeEnum.IncorrectValue                                        
                                        );
                                }

                                if (dependencyGuidList.Contains(dg))
                                {
                                    throw new CHDException(
                                        string.Format(
                                            "���� ����������� {0} �����������",
                                            dg
                                            ),
                                        CHDExceptionTypeEnum.IncorrectValue
                                        );
                                }

                                dependencyGuidList.Add(dg);
                            }
                        }

                        var pxmld = new PluginLoader.PluginXmlDescription(
                            guid,
                            description,
                            Path.Combine(dir, rootFile),
                            rootClass,
                            dependencyGuidList
                            );

                        _logger.LogFormattedMessage(
                            "��������� ������������ ������� {0} �� ������ {1} �� ��������� ������ {2}",
                            pxmld.PluginGuid,
                            pxmld.RootFilePath,
                            pxmld.RootClass);

                        result.Add(pxmld);
                    }
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }

            return result;
        }

        private void xmlReaderSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                throw e.Exception;
            }

            _logger.LogMessage("WARNING: " + e.Message);
        }
    }
}