using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.Operation.Visitor;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.Saver;
using CHD.Common.Serializer;

namespace CHD.Common.Structure
{
    [Serializable]
    public sealed class StoredStructure : IStoredStructure
    {
        public SerializationVersionProvider<StoredStructure> SerializationVersion = new SerializationVersionProvider<StoredStructure>();

        private readonly IPathComparerProvider _pathComparerProvider;

        private long _isReadOnly = 0L;
        private Guid? _changeIdentifier = null;

        public int Version
        {
            get;
            private set;
        }

        public bool IsReadOnly
        {
            get
            {
                return
                    Interlocked.Read(ref _isReadOnly) > 0L;
            }
        }

        public IFolder RootFolder
        {
            get;
            private set;
        }

        public StoredStructure(
            IPathComparerProvider pathComparerProvider,
            string rootFolderName
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (rootFolderName == null)
            {
                throw new ArgumentNullException("rootFolderName");
            }

            _pathComparerProvider = pathComparerProvider;

            Version = 0;
            RootFolder = new StructureFolder(
                pathComparerProvider,
                rootFolderName,
                rootFolderName,
                Guid.Empty,
                new List<IFile>(),
                new List<IFolder>()
                );

        }

        //private StoredStructure(
        //    ISerializer serializer,
        //    StoredStructure parent
        //    )
        //{
        //    if (serializer == null)
        //    {
        //        throw new ArgumentNullException("serializer");
        //    }
        //    if (parent == null)
        //    {
        //        throw new ArgumentNullException("parent");
        //    }

        //    _pathComparerProvider = parent._pathComparerProvider;
        //    Version = parent.Version + 1;
        //    RootFolder = serializer.DeepClone(parent.RootFolder);
        //}

        private StoredStructure(
            IPathComparerProvider pathComparerProvider, 
            int newVersion, 
            IFolder actualRoot
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (actualRoot == null)
            {
                throw new ArgumentNullException("actualRoot");
            }

            _pathComparerProvider = pathComparerProvider;
            Version = newVersion;
            RootFolder = actualRoot;
        }

        public Guid GetChangeIdentifier()
        {
            if (IsReadOnly && _changeIdentifier.HasValue)
            {
                return
                    _changeIdentifier.Value;
            }

            var result = RootFolder.CalculateChangeIdentifier();

            return
                result;
        }

        public void SetReadOnlyMode()
        {
            if (Interlocked.Exchange(ref _isReadOnly, 1L) == 0L)
            {
                RootFolder.RecursiveSetReadOnlyMode();

                _changeIdentifier = RootFolder.CalculateChangeIdentifier();
            }
        }

        public IStoredStructure CreateNew(
            IFolder actualRoot
            )
        {
            if (actualRoot == null)
            {
                throw new ArgumentNullException("actualRoot");
            }

            var result = new StoredStructure(
                this._pathComparerProvider,
                this.Version + 1,
                actualRoot
                );

            return result;
        }

        //public bool CleanupOperationLog(
        //    IPathComparerProvider pathComparerProvider
        //    )
        //{
        //    if (pathComparerProvider == null)
        //    {
        //        throw new ArgumentNullException("pathComparerProvider");
        //    }

        //    var result = false;

        //    lock (_locker)
        //    {
        //        //старые файлы и папки (очистка двойных операций)
        //        var figroups = this.Log
        //            .GroupBy(o => o.FullPath, j => j, pathComparerProvider.Comparer) //пути сравниваются с учетом требований файловой системы (регистрозависимость)
        //            .Select(j => j)
        //            ;

        //        foreach (var figroup in figroups)
        //        {
        //            var oplist = figroup.ToList();

        //            foreach (var op in oplist.TakeWithoutTail(1))
        //            {
        //                this.Log.Remove(op);

        //                result = true;
        //            }
        //        }


        //        var divider = new OperationDivider();
        //        //foreach (var o in this.Log)
        //        //{
        //        //    o.Accept(divider);
        //        //}
        //        divider.Accept(this.Log);

        //        var deletesPairs = new List<OperationDivider.DividerPair<IFolderOperation>>();
        //        for (var cc = 0; cc < divider.FolderOperations.Count; cc++)
        //        {
        //            var dpair = divider.FolderOperations[cc];
        //            var currentOperation = dpair.Operation;

        //            if (currentOperation.IsFolderOperation && currentOperation.Type == OperationTypeEnum.Delete)
        //            {
        //                var childpairs = dpair.GetSubFolders(deletesPairs);
        //                if (childpairs != null && childpairs.Count > 0)
        //                {
        //                    //currentOperation - операция с папкой более высокого уровня, которая включает в себя
        //                    //одну или несколько ранее "удаленных" (deletesPairs)

        //                    foreach (var childPair in childpairs)
        //                    {
        //                        deletesPairs.Remove(childPair);
        //                    }
        //                }
        //                else
        //                {
        //                    //папка еще более высокого уровня не удалялась
        //                    //просто надо добавить ее в список удаления
        //                }

        //                deletesPairs.Add(dpair);
        //            }
        //        }

        //        //очищаем папки, которые содержатся в удаленных папках
        //        for (var cc = 0; cc < divider.FolderOperations.Count; cc++)
        //        {
        //            var currentPair = divider.FolderOperations[cc];

        //            var shots = deletesPairs.FindAll(j => j.Operation.Folder.IsByPathContains(currentPair.Operation.Folder));

        //            if (shots.Count.NotIn(0, 1))
        //            {
        //                //странно! такого быть не должно

        //                throw new CHDException(
        //                    currentPair.Operation.FullPath,
        //                    CHDExceptionTypeEnum.UnknownStructureProblem
        //                    );
        //            }

        //            if (shots.Count == 1)
        //            {
        //                var shot = shots[0];

        //                if (shot.Index > currentPair.Index)
        //                {
        //                    this.Log.Remove(currentPair.Operation);
        //                }
        //            }
        //        }

        //        //очищаем файлы, которые содержатся в удаленных папках
        //        for (var cc = 0; cc < divider.FileOperations.Count; cc++)
        //        {
        //            var currentPair = divider.FileOperations[cc];

        //            var shots = deletesPairs.FindAll(j => j.Operation.Folder.IsByPathContains(currentPair.Operation.File));

        //            if (shots.Count.NotIn(0, 1))
        //            {
        //                //странно! такого быть не должно

        //                throw new CHDException(
        //                    currentPair.Operation.FullPath,
        //                    CHDExceptionTypeEnum.UnknownStructureProblem
        //                    );
        //            }

        //            if (shots.Count == 1)
        //            {
        //                var shot = shots[0];

        //                if (shot.Index > currentPair.Index)
        //                {
        //                    this.Log.Remove(currentPair.Operation);
        //                }
        //            }
        //        }
        //    }
            
        //    return
        //        result;
        //}

    }
}