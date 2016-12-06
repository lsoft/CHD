using System.Security;

namespace CHD.Wpf
{
    /// <summary>
    /// ����������� ������
    /// </summary>
    public interface IConsumePassword
    {
        /// <summary>
        /// ���������� ������ ����, ��� ��� ���������� �� ���� �����, ��� �� ���������
        /// </summary>
        /// <param name="password">������</param>
        void SetPassword(SecureString password);
    }
}