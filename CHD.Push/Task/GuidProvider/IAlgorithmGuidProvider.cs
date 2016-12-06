using System;
using CHD.FileSystem.FileWrapper;

namespace CHD.Push.Task.GuidProvider
{
    public interface IAlgorithmGuidProvider
    {
        Guid GenerateGuid(
            IFileWrapper fileWrapper
            );
    }
}