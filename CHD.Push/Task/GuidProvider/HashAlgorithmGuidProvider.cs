using System;
using CHD.Common.HashGenerator;
using CHD.FileSystem.FileWrapper;

namespace CHD.Push.Task.GuidProvider
{
    public class HashAlgorithmGuidProvider : IAlgorithmGuidProvider
    {
        private readonly IHashGenerator _hashGenerator;

        public HashAlgorithmGuidProvider(
            IHashGenerator hashGenerator
            )
        {
            if (hashGenerator == null)
            {
                throw new ArgumentNullException("hashGenerator");
            }
            _hashGenerator = hashGenerator;
        }

        public Guid GenerateGuid(
            IFileWrapper fileWrapper
            )
        {
            var result = _hashGenerator.CalculateHashGuid(fileWrapper.FilePathSuffix.FilePathSuffix);

            return
                result;
        }
    }
}
