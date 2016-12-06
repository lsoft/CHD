using System;
using System.Xml;
using CHD.FileSystem.FileWrapper;
using CHD.Push.ActivityPool;

namespace CHD.Push.Task.Factory
{
    public interface IAlgorithmFactory
    {
        IAlgorithm Load(
            XmlNode source
            );

        IAlgorithm Create(
            Guid taskGuid,
            ActivityTypeEnum type,
            IFileWrapper fileWrapper,
            int pushTimeoutAfterFailureMsec
            );
    }
}