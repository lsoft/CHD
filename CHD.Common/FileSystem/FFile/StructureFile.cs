using System;
using System.IO;
using System.Xml;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.Saver;
using CHD.Common.Serializer;

namespace CHD.Common.FileSystem.FFile
{
    [Serializable]
    public sealed class StructureFile : IFile
    {
        public SerializationVersionProvider<StructureFile> SerializationVersion = new SerializationVersionProvider<StructureFile>();

        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IFolder _folder;
        private readonly string _fullPath;

        public string Name
        {
            get;
            private set;
        }

        public string FullPath
        {
            get
            {
                return
                    _fullPath;
            }
        }

        public PathSequence FullPathSequence
        {
            get;
            private set;
        }

        public IFolder Folder
        {
            get
            {
                return _folder;
            }
        }

        public long Size
        {
            get;
            private set;
        }

        public DateTime LastWriteTimeUtc
        {
            get;
            private set;
        }

        public Guid ChangeIdentifier
        {
            get;
            private set;
        }

        public StructureFile(
            IPathComparerProvider pathComparerProvider,
            IFolder folder,
            string name,
            long size,
            DateTime lastWriteTimeUtc,
            Guid changeIdentifier
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            _pathComparerProvider = pathComparerProvider;
            _folder = folder;
            Name = name;
            Size = size;
            LastWriteTimeUtc = lastWriteTimeUtc;
            ChangeIdentifier = changeIdentifier;

            _fullPath = Path.Combine(folder.FullPath, name);

            FullPathSequence = new PathSequence(folder.FullPathSequence, name);
        }
        

        public bool IsSame(string fullPath)
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }

            var r = string.Compare(_fullPath, fullPath, _pathComparerProvider.Comparison);

            return
                r == 0;
        }

        public bool IsNameEquals(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var result = IsNameEquals(file.Name);

            return
                result;
        }

        public bool IsNameEquals(
            string fileName
            )
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            var r = string.Compare(Name, fileName, _pathComparerProvider.Comparison);

            return
                r == 0;
        }

        public bool IsSame(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var fullPath = file.FullPath;

            var result = this.IsSame(fullPath);

            return
                result;
        }
    }
}
