using System;
using System.Collections.Generic;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Operation.FileOperation;
using CHD.Common.Operation.Fixer;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.Others;

namespace CHD.Common.FileSystem.FFolder
{
    public interface IFolder : INamedFolder
    {
        bool IsReadOnly
        {
            get;
        }

        IFolder Parent
        {
            get;
        }

        IReadOnlyCollection<IFolder> Folders
        {
            get;
        }

        IReadOnlyCollection<IFile> Files
        {
            get;
        }

        int RecursivelyFolderCount
        {
            get;
        }

        int RecursivelyFileCount
        {
            get;
        }

        int ChildCount
        {
            get;
        }


        bool IsSame(
            IFolder folder
            );

        bool IsSame(
            string fullPath
            );

        IFolder GetFolderByName(
            string folderName
            );

        IFile GetFileByName(
            IFile file
            );

        IFile GetFileByName(
            string fileName
            );

        IFolder GetDeepChildFolder(
            INamedFolder folder
            );

        bool GetFileByPath(
            PathSequence searchFilePathSequence,
            out IFile foundFile
            );


        bool IsNameEquals(
            string folderName
            );




        bool IsByPathFolderContains(
            PathSequence folderPathSequence
            );


        //bool IsByPathContains(
        //    string fullPath
        //    );

        bool IsByPathContains(
            IFolder child
            );

        bool IsByPathContains(
            IFile child
            );

        bool GetFileByPath(
            IFile searchFile,
            out IFile foundFile
            );



        void AddChildFolder(
            IFolder folder
            );

        void CreateOrUpdateChildFile(
            IFile file
            );

        IFile RemoveChild(
            IFile file
            );

        IFolder RemoveChild(
            IFolder folder
            );

        void RecursiveSetReadOnlyMode(
            );

    }
}