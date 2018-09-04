namespace CHD.Plugin
{
    /// <summary>
    /// ���������� � ������� ��������� �������
    /// </summary>
    public interface ICHDPluginInformator
    {
        /// <summary>
        /// ������� ��������� �������
        /// </summary>
        string CurrentInformation
        {
            get;
        }

        /// <summary>
        /// �������� ���������� � ������� ��������� �������
        /// </summary>
        /// <param name="information">����������</param>
        void UpdateInformation(string information);
    }
}