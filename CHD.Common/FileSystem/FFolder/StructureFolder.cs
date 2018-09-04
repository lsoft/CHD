using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.Operation.Visitor;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.Serializer;

namespace CHD.Common.FileSystem.FFolder
{
    [Serializable]
    [DebuggerDisplay("{FullPath}")]
    public sealed class StructureFolder : IFolder
    {
        public SerializationVersionProvider<StructureFolder> SerializationVersion = new SerializationVersionProvider<StructureFolder>();

        private readonly List<IFolder> _folders = new List<IFolder>();
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly List<IFile> _files = new List<IFile>();

        public bool IsReadOnly
        {
            get;
            private set;
        }

        public IFolder Parent
        {
            get;
            private set;
        }

        public Guid ChangeIdentifier
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string FullPath
        {
            get;
            private set;
        }

        public PathSequence FullPathSequence
        {
            get;
            private set;
        }

        public IReadOnlyCollection<IFolder> Folders
        {
            get
            {
                return
                    _folders;
            }
        }

        public IReadOnlyCollection<IFile> Files
        {
            get
            {
                return
                    _files;
            }
        }

        public int RecursivelyFolderCount
        {
            get
            {
                var result = DoCalculateRecursivelyFolderCount(this);

                return result;
            }
        }


        public int RecursivelyFileCount
        {
            get
            {
                var result = DoCalculateRecursivelyFileCount(this);

                return result;
            }
        }

        public int ChildCount
        {
            get
            {
                return
                    _files.Count + _folders.Count;
            }
        }

        public StructureFolder(
            IPathComparerProvider pathComparerProvider,
            string fullPath,
            string name,
            Guid changeIdentifier,
            List<IFile> files,
            List<IFolder> folders,
            IFolder parent = null
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }
            if (folders == null)
            {
                throw new ArgumentNullException("folders");
            }

            _pathComparerProvider = pathComparerProvider;
            _files = files;
            _folders = folders;
            FullPath = fullPath;
            Name = name;
            ChangeIdentifier = changeIdentifier;
            Parent = parent;
            FullPathSequence = new PathSequence(pathComparerProvider, fullPath);
            IsReadOnly = false;
        }

        public void RecursiveSetReadOnlyMode(
            )
        {
            this.IsReadOnly = true;

            foreach (var f in _folders)
            {
                f.RecursiveSetReadOnlyMode();
            }
        }

        private static int DoCalculateRecursivelyFileCount(
            IFolder structureFolder
            )
        {
            var cnt = structureFolder.Files.Count;

            foreach (var childFolder in structureFolder.Folders)
            {
                cnt += DoCalculateRecursivelyFileCount(childFolder);
            }

            return
                cnt;
        }

        private static int DoCalculateRecursivelyFolderCount(
            IFolder structureFolder
            )
        {
            var cnt = 1;

            foreach (var childFolder in structureFolder.Folders)
            {
                cnt += DoCalculateRecursivelyFolderCount(childFolder);
            }

            return
                cnt;
        }


        public bool IsSame(IFolder folder)
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }

            return
                IsSame(folder.FullPath);
        }

        public bool IsSame(string fullPath)
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }

            var r = string.Compare(this.FullPath, fullPath, _pathComparerProvider.Comparison);

            return
                r == 0;
        }

        public IFolder GetFolderByName(string folderName)
        {
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            return
                Folders.FirstOrDefault(j => j.IsNameEquals(folderName));
        }




        public bool IsNameEquals(string folderName)
        {
            var r = string.Compare(this.Name, folderName, _pathComparerProvider.Comparison);

            return
                r == 0;
        }




        public IFile GetFileByName(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            return
                Files.FirstOrDefault(j => j.IsNameEquals(file));
        }

        public IFile GetFileByName(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            return
                Files.FirstOrDefault(j => j.IsNameEquals(fileName));
        }

        public bool GetFileByPath(
            PathSequence searchFilePathSequence,
            out IFile foundFile
            )
        {
            if (searchFilePathSequence == null)
            {
                throw new ArgumentNullException("searchFilePathSequence");
            }

            var folderPathSequence = searchFilePathSequence.Up();

            if (!IsByPathFolderContains(folderPathSequence))
            {
                foundFile = null;

                return
                    false;
            }

            IFolder deepestFoundFolder;
            var r = GetDeepChildFolderPrivate(folderPathSequence, out deepestFoundFolder);
            if (!r)
            {
                foundFile = null;

                return
                    false;
            }

            foundFile = deepestFoundFolder.Files.FirstOrDefault(j => j.IsSame(searchFilePathSequence.Path));

            return
                foundFile != null;
        }

        public bool GetFileByPath(
            IFile searchFile,
            out IFile foundFile
            )
        {
            if (searchFile == null)
            {
                throw new ArgumentNullException("searchFile");
            }

            var fo = searchFile.Folder;

            if (!IsByPathContains(fo))
            {
                foundFile = null;

                return
                    false;
            }

            IFolder deepestFoundFolder;
            var r = GetDeepChildFolderPrivate(fo.FullPathSequence, out deepestFoundFolder);
            if (!r)
            {
                foundFile = null;

                return
                    false;
            }

            foundFile = deepestFoundFolder.Files.FirstOrDefault(j => j.IsSame(searchFile));

            return
                foundFile != null;
        }

        public bool IsByPathFolderContains(PathSequence folderPathSequence)
        {
            if (folderPathSequence == null)
            {
                throw new ArgumentNullException("folderPathSequence");
            }

            var result = folderPathSequence.Path.StartsWith(this.FullPath, _pathComparerProvider.Comparison);

            return
                result;
        }

        public bool IsByPathContains(IFolder child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            var result = child.FullPath.StartsWith(this.FullPath, _pathComparerProvider.Comparison);

            return
                result;
        }

        public bool IsByPathContains(IFile child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            var result = child.FullPath.StartsWith(this.FullPath, _pathComparerProvider.Comparison);

            return
                result;

        }

        public IFolder GetDeepChildFolder(INamedFolder folder)
        {
            IFolder dchild;
            if (this.GetDeepChildFolderPrivate(
                folder.FullPathSequence,
                out dchild
                ))
            {
                return dchild;
            }

            return null;
        }



        public void AddChildFolder(
            IFolder folder
            )
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }

            if (IsReadOnly)
            {
                throw new CHDException(
                    string.Format("Folder {0} is in read only", FullPath),
                    CHDExceptionTypeEnum.ReadOnly
                    );
            }

            this._folders.Add(folder);
        }

        public void AddChildFile(
            IFile file
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (IsReadOnly)
            {
                throw new CHDException(
                    string.Format("Folder {0} is in read only", FullPath),
                    CHDExceptionTypeEnum.ReadOnly
                    );
            }

            this._files.Add(file);
        }

        public void CreateOrUpdateChildFile(
            IFile file
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (IsReadOnly)
            {
                throw new CHDException(
                    string.Format("Folder {0} is in read only", FullPath),
                    CHDExceptionTypeEnum.ReadOnly
                    );
            }

            this._files.RemoveAll(j => j.IsSame(file));

            this._files.Add(file);
        }

        public IFile RemoveChild(
            IFile file
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (IsReadOnly)
            {
                throw new CHDException(
                    string.Format("Folder {0} is in read only", FullPath),
                    CHDExceptionTypeEnum.ReadOnly
                    );
            }

            var files = _files.FindAll(f => f.IsSame(file));

            if (files.Count > 1)
            {
                throw new CHDException(file.FullPath, CHDExceptionTypeEnum.TooManyFiles);
            }

            if (files.Count == 0)
            {
                return null;
            }

            var result = files[0];

            _files.Remove(result);

            return
                result;
        }

        public IFolder RemoveChild(
            IFolder folder
            )
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }

            if (IsReadOnly)
            {
                throw new CHDException(
                    string.Format("Folder {0} is in read only", FullPath),
                    CHDExceptionTypeEnum.ReadOnly
                    );
            }

            var folders = _folders.FindAll(f => f.IsSame(folder));

            if (folders.Count > 1)
            {
                throw new CHDException(folder.FullPath, CHDExceptionTypeEnum.TooManyFolders);
            }

            if (folders.Count == 0)
            {
                return null;
            }

            var result = folders[0];

            _folders.Remove(result);

            return
                result;
        }



        #region private

        private bool GetDeepChildFolderPrivate(PathSequence folderPathSequence, out IFolder deepestFoundFolder)
        {
            if (folderPathSequence == null)
            {
                throw new ArgumentNullException("folderPathSequence");
            }

            var otstup = this.FullPathSequence.Count;

            deepestFoundFolder = this;
            for (var cc = otstup; cc < folderPathSequence.Count; cc++)
            {
                var fn = folderPathSequence[cc];

                var thischild = deepestFoundFolder.Folders.FirstOrDefault(j => j.IsNameEquals(fn));

                if (thischild == null)
                {
                    return
                        false;
                }

                deepestFoundFolder = thischild;
            }

            return
                true;
        }

        #endregion
    }




}