using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using CHD.Common;
using CHD.Common.Others;
using Ninject;

namespace CHD.Plugin.Infrastructure
{
    public sealed class PluginBinder : IPluginBinder
    {
        private readonly IPluginLoader _pluginLoader;
        private readonly IKernel _container;
        private readonly IDisorderLogger _logger;

        private readonly Dictionary<IPluginXmlDescription, ICHDPlugin> _pluginDictionary;

        public ReadOnlyCollection<ICHDPlugin> Plugins
        {
            get
            {
                return 
                    _pluginDictionary.Values.ToList().AsReadOnly();
            }
        }

        public PluginBinder(
            IPluginLoader pluginLoader,
            IKernel container,
            IDisorderLogger logger
            )
        {
            if (pluginLoader == null)
            {
                throw new ArgumentNullException("pluginLoader");
            }
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pluginLoader = pluginLoader;
            _container = container;
            _logger = logger;

            _pluginDictionary = new Dictionary<IPluginXmlDescription, ICHDPlugin>();
        }

        public void InitPluginsFromFolder(string pluginRootFolder)
        {
            if (pluginRootFolder == null)
            {
                throw new ArgumentNullException("pluginRootFolder");
            }

            var xmlDescriptions = _pluginLoader.LoadFrom(pluginRootFolder);

            foreach (var xmld in xmlDescriptions)
            {
                try
                {
                    var a = Assembly.LoadFrom(xmld.RootFilePath);

                    var initTypes = a.GetTypes().ToList().FindAll(
                        j => j.Name == xmld.RootClass
                             && j.GetInterfaces().Contains(typeof (ICHDPlugin)));

                    if (initTypes == null || initTypes.Count == 0)
                    {
                        throw new CHDException(
                            string.Format(
                                "Сборка {0} не содержит {1}",
                                xmld.RootFilePath,
                                xmld.RootClass
                                ),
                            CHDExceptionTypeEnum.PluginRootDoesNotFound
                            );
                    }

                    if (initTypes.Count > 1)
                    {
                        throw new CHDException(
                            string.Format(
                                "Сборка {0} содержит более одного {1}",
                                xmld.RootFilePath,
                                xmld.RootClass),
                            CHDExceptionTypeEnum.AssemblyContainsMoreThanOnePlugin
                            );
                    }

                    var initType = initTypes[0];

                    //request.Target.Member.ReflectedType.Assembly.Location

                    //схема регистрации плагина через контейнер нужна для того, чтобы
                    //автоматически вызвался Dispose при закрытии приложения

                    _container
                        .Bind<ICHDPlugin>()
                        .To(initType)
                        .InSingletonScope()
                        .Named(initType.Assembly.FullName)
                        ;

                    var initer = _container.Get<ICHDPlugin>(
                        initType.Assembly.FullName
                        );

                    initer.Init(_container);

                    _pluginDictionary.Add(xmld, initer);
                }
#if !WindowsCE
                catch (ReflectionTypeLoadException rtle)
                {
                    _logger.LogException(rtle, "ReflectionTypeLoadException: ");

                    if (rtle.LoaderExceptions != null && rtle.LoaderExceptions.Length > 0)
                    {
                        foreach (var ie in rtle.LoaderExceptions)
                        {
                            _logger.LogException(ie, "LOADER EXCEPTION: ");
                        }
                    }
                }
#endif
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }

        public void StartPlugins()
        {
            foreach (var pair in this.PushToLine())
            {
                try
                {
                    _logger.LogFormattedMessage(
                        "Плагин {0} по адресу {1} со стартовой точкой {2} запускается",
                        pair.Item1.PluginGuid,
                        pair.Item1.RootFilePath,
                        pair.Item1.RootClass);

                    pair.Item2.Start();

                    _logger.LogFormattedMessage(
                        "Плагин {0} по адресу {1} со стартовой точкой {2} запущен",
                        pair.Item1.PluginGuid,
                        pair.Item1.RootFilePath,
                        pair.Item1.RootClass);
                }
                catch (Exception excp)
                {
                    _logger.LogException(
                        excp,
                        string.Format("Плагин {0} по адресу {1} со стартовой точкой {2} сгенерировал ошибку при старте",
                            pair.Item1.PluginGuid,
                            pair.Item1.RootFilePath,
                            pair.Item1.RootClass
                            )
                        );
                }
            }
        }

        public void StopPlugins()
        {
            var line = this.PushToLine();
            line.Reverse();

            foreach (var pair in line)
            {
                try
                {
                    _logger.LogFormattedMessage(
                        "Плагин {0} по адресу {1} со стартовой точкой {2} деинициализируется",
                        pair.Item1.PluginGuid,
                        pair.Item1.RootFilePath,
                        pair.Item1.RootClass);

                    pair.Item2.Stop();

                    _logger.LogFormattedMessage(
                        "Плагин {0} по адресу {1} со стартовой точкой {2} деинициализирован",
                        pair.Item1.PluginGuid,
                        pair.Item1.RootFilePath,
                        pair.Item1.RootClass);
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }
        }

        public void ReleasePlugins()
        {
            var line = this.PushToLine();
            line.Reverse();

            foreach (var pair in line)
            {
                try
                {
                    _logger.LogFormattedMessage(
                        "Плагин {0} по адресу {1} со стартовой точкой {2} утилизируется",
                        pair.Item1.PluginGuid,
                        pair.Item1.RootFilePath,
                        pair.Item1.RootClass);

                    pair.Item2.Dispose();
                    _container.Release(pair.Item2);

                    _logger.LogFormattedMessage(
                        "Плагин {0} по адресу {1} со стартовой точкой {2} утилизирован",
                        pair.Item1.PluginGuid,
                        pair.Item1.RootFilePath,
                        pair.Item1.RootClass);
                }
                catch (Exception excp)
                {
                    _logger.LogException(excp);
                }
            }

            _pluginDictionary.Clear();

            _logger.LogFormattedMessage(
                "Все плагины остановлены"
                );
        }

        public List<IPluginXmlDescription> GetLoadedPluginDescription()
        {
            return
                _pluginDictionary.Keys.ToList();
        }

        #region private code

        private List<Tuple<IPluginXmlDescription, ICHDPlugin>> PushToLine()
        {
            var unsorted = _pluginDictionary.ToList();

            var result = new List<Tuple<IPluginXmlDescription, ICHDPlugin>>();

            while (unsorted.Count > 0)
            {
                var removed = false;
                foreach (var a in unsorted)
                {
                    var ag = a.Key.PluginGuid;

                    //если на него нет ссылок, то круто
                    var exists = unsorted.Any(j => j.Key.Dependencies.Any(k => k == ag));

                    if (!exists)
                    {
                        result.Add(new Tuple<IPluginXmlDescription, ICHDPlugin>(a.Key, a.Value));
                        unsorted.Remove(a);

                        removed = true;
                        break;
                    }
                }

                if (!removed)
                {
                    throw new CHDException(
                        string.Format(
                            "Обнаружена кольцевая зависимость между плагинами {0}",
                            string.Join(
                                " <-> ",
                                unsorted.ConvertAll(j => j.Key.PluginGuid.ToString()).ToArray()
                                )
                            ),
                        CHDExceptionTypeEnum.IncorrectValue
                        );
                }
            }

            result.Reverse();

            return 
                result;
        }

        #endregion
    }
}