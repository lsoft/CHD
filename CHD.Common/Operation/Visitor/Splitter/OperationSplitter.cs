using System;
using System.Collections.Generic;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.Operation.Visitor.Splitter
{
    //public sealed class OperationSplitter : IOperationSplitter
    //{
    //    private readonly IDictionary<IFile, IFileOperation> _fileOperations;
    //    private readonly IDictionary<IFolder, IFolderOperation> _folderOperations;

    //    public IDictionary<IFile, IFileOperation> FileOperations
    //    {
    //        get
    //        {
    //            return _fileOperations;
    //        }
    //    }

    //    public IDictionary<IFolder, IFolderOperation> FolderOperations
    //    {
    //        get
    //        {
    //            return _folderOperations;
    //        }
    //    }

    //    public OperationSplitter(
    //        )
    //        : this(StringComparer.InvariantCultureIgnoreCase)
    //    {
    //    }

    //    public OperationSplitter(
    //        StringComparer stringComparer
    //        )
    //    {
    //        _fileOperations = new Dictionary<IFile, IFileOperation>(new FileComparer(stringComparer));
    //        _folderOperations = new Dictionary<IFolder, IFolderOperation>(new FolderComparer(stringComparer));
    //    }

    //    public void Visit(
    //        IFileOperation operation
    //        )
    //    {
    //        if (operation == null)
    //        {
    //            throw new ArgumentNullException("operation");
    //        }

    //        _fileOperations.Add(
    //            operation.File,
    //            operation
    //            );
    //    }

    //    public void Visit(
    //        IFolderOperation operation
    //        )
    //    {
    //        if (operation == null)
    //        {
    //            throw new ArgumentNullException("operation");
    //        }

    //        _folderOperations.Add(
    //            operation.Folder,
    //            operation
    //            );
    //    }
    //}
}