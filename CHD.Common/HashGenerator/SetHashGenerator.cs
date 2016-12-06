using System;
using System.IO;
using System.Text;

namespace CHD.Common.HashGenerator
{
    /// <summary>
    /// Генератор хеша из множества хеш-функций
    /// </summary>
    public class SetHashGenerator : IHashGenerator
    {
        private readonly IHashGenerator[] _hashGenerators;

        public SetHashGenerator(
            params IHashGenerator[] hashGenerators
            )
        {
            if (hashGenerators == null)
            {
                throw new ArgumentNullException("hashGenerators");
            }
            if (hashGenerators.Length == 0)
            {
                throw new ArgumentException("hashGenerators.Length == 0");
            }

            _hashGenerators = hashGenerators;
        }

        public byte[] CalculateHash(byte[] body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            var result = new byte[16];
            foreach (var hg in _hashGenerators)
            {
                var hashbody = hg.CalculateHash(body);

                for (var cc = 0; cc < 16; cc++)
                {
                    result[cc] ^= hashbody[cc];
                }
            }

            return result;
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
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            var body = Encoding.Unicode.GetBytes(s);

            var hash = this.CalculateHash(body);

            var result = new Guid(hash);

            return result;
        }

        public byte[] CalculateHash(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            var result = new byte[16];
            foreach (var hg in _hashGenerators)
            {
                var hashbody = hg.CalculateHash(inputStream);

                for (var cc = 0; cc < 16; cc++)
                {
                    result[cc] ^= hashbody[cc];
                }
            }

            return result;
        }

        public void Dispose()
        {
            foreach (var hg in _hashGenerators)
            {
                hg.Dispose();
            }
        }
    }
}