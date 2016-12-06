using System;
using System.IO;

namespace CHD.Common.HashGenerator
{
    /// <summary>
    /// Генератор хеш-кода
    /// </summary>
    public interface IHashGenerator : IDisposable
    {
        /// <summary>
        /// Посчитать хеш и вернуть его в виде массива байт
        /// </summary>
        /// <param name="body">От кого считать хеш</param>
        /// <returns>Хеш</returns>
        byte[] CalculateHash(byte[] body);

        /// <summary>
        /// Посчитать хеш и вернуть его в виде Guid
        /// </summary>
        /// <param name="body">От кого считать хеш</param>
        /// <returns>Хеш</returns>
        Guid CalculateHashGuid(byte[] body);

        /// <summary>
        /// Посчитать хеш и вернуть его в виде Guid
        /// </summary>
        /// <param name="s">От кого считать хеш</param>
        /// <returns>Хеш</returns>
        Guid CalculateHashGuid(string s);

        /// <summary>
        /// Посчитать хеш и вернуть его в виде массива байт
        /// </summary>
        /// <param name="inputStream">Из какого потока читать данные для хеша</param>
        /// <returns>Хеш</returns>
        byte[] CalculateHash(Stream inputStream);
    }
}   
