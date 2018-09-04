using System;
using System.Collections.Generic;

namespace CHD.Plugin.Infrastructure
{
    /// <summary>
    /// �������� �������
    /// </summary>
    public interface IPluginXmlDescription
    {
        /// <summary>
        /// ������������� �������
        /// </summary>
        Guid PluginGuid
        {
            get;
        }

        /// <summary>
        /// �������� �������
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// ��������� dll �������
        /// </summary>
        string RootFilePath
        {
            get;
        }

        /// <summary>
        /// ��������� ����� �������, ����������� ICHDPlugin
        /// </summary>
        string RootClass
        {
            get;
        }

        /// <summary>
        /// ��������������, �� ������� ������� ������
        /// </summary>
        List<Guid> Dependencies
        {
            get;
        }
    }
}