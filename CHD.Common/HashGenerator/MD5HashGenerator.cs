using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CHD.Common.HashGenerator
{
    /// <summary>
    /// Генератор хеша MD5
    /// </summary>
    public class MD5HashGenerator : IHashGenerator
    {
        private readonly MD5 _hashGenerator;

        private bool _disposed = false;

        public MD5HashGenerator(
            )
        {
            this._hashGenerator = MD5.Create();
        }

        public byte[] CalculateHash(byte[] body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            var hash = _hashGenerator.ComputeHash(body);

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

            var hash = _hashGenerator.ComputeHash(inputStream);

            return hash;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

#if !WindowsCE
                _hashGenerator.Dispose();
#else
                _hashGenerator.Clear();
#endif
            }
        }
    }
}