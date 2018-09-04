using System.Collections.Generic;

namespace CHD.Plugin.Infrastructure
{
    /// <summary>
    /// ��������� �������� ��������
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// �������� �������� �������� �� �����
        /// </summary>
        /// <param name="pluginRootFolder">�����, � ������� ����� �������</param>
        /// <returns>������ �������� ��������</returns>
        List<IPluginXmlDescription> LoadFrom(string pluginRootFolder);
    }
}