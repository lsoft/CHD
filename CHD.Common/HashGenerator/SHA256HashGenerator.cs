using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CHD.Common.HashGenerator
{
    /// <summary>
    /// ’еш генератор дл€ SHA-256
    /// ¬ проектах, кодова€ база которых должна работать на Compact Framework его
    /// использовать нежелательно, так как в CF нету SHA256, а кастомный SHA256 € не реализовал еще
    /// </summary>
    public class SHA256HashGenerator : IHashGenerator
    {
        private readonly SHA256 _hashGenerator;

        private bool _disposed = false;

        public SHA256HashGenerator(
            )
        {
            this._hashGenerator = SHA256.Create();
            
        }

        public byte[] CalculateHash(byte[] body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            var hash32 = _hashGenerator.ComputeHash(body);

            var hash = new byte[16];
            for (var cc = 0; cc < 16; cc++)
            {
                hash[cc] = (byte)(hash32[cc] ^ hash32[16 + cc]);
            }

            return hash;
        }

        public Guid CalculateHashGuid(byte[] body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            var hash = this.CalculateHash(body);

            var result = new Guid(hash);

            return result;
        }

        public Guid CalculateHashGuid(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);

            return
                CalculateHashGuid(bytes);
        }

        public byte[] CalculateHash(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            var hash32 = _hashGenerator.ComputeHash(inputStream);

            var hash = new byte[16];
            for (var cc = 0; cc < 16; cc++)
            {
                hash[cc] = (byte)(hash32[cc] ^ hash32[16 + cc]);
            }

            return hash;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _hashGenerator.Dispose();
            }
        }

    }
}