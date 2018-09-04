using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.Surgeon;
//using CHD.Common.KeyValueContainer.SyncedVersion;
using CHD.Common.PathComparer;
using CHD.Common.Serializer;
using CHD.Common.Structure.Container.Factory;
using CHD.Tests.ExtendedContext;
using CHD.Tests.FileSystem;
using CHD.Tests.FileSystem.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CHD.Tests
{
    [TestClass]
    public sealed class DiskDiskSyncFixture : SyncFixture
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
       }

        protected override IFileSystemSurgeonConnector PrepareLocalConfiguration0()
        {
            ExtendedContext.BindDisk(
                RootFolderName,
                StructureFileName,
                RoleEnum.Local
                );

            var surgeonConnector = ExtendedContext.GetDiskSugeonConnector(
                RoleEnum.Local
                );

            var builder = ExtendedContext.CreateBuilder(surgeonConnector);

            IFileSystemSchema schema;
            var fileSystem = builder
                .Folder(A0FolderName, A1FolderName)
                        .PseudoRandomFile(A1FileName, A1FileSeed, A1FileLength)
                    .Up(2)
                .Folder(B0FolderName)
                    .EmptyFile(B0FileName)
                .Create(out schema)
                ;

            return fileSystem;
        }

        protected override IFileSystemSurgeonConnector PrepareAnotherConfiguration0()
        {
            ExtendedContext.BindDisk(
                RootFolderName,
                StructureFileName,
                RoleEnum.Another
                );

            var surgeonConnector = ExtendedContext.GetDiskSugeonConnector(
                RoleEnum.Another
                );

            var builder = ExtendedContext.CreateBuilder(surgeonConnector);

            IFileSystemSchema schema;
            var fileSystem = builder
                .Folder(A0FolderName, A1FolderName)
                        .PseudoRandomFile(A1FileName, A1FileSeed, A1FileLength)
                    .Up(2)
                .Folder(B0FolderName)
                    .EmptyFile(B0FileName)
                .Create(out schema)
                ;

            return fileSystem;
        }

        protected override IFileSystemSurgeonConnector PrepareRemoteConfiguration0(
            out IFileSystemSchema schema
            )
        {
            ExtendedContext.BindDisk(
                RootFolderName,
                StructureFileName,
                RoleEnum.Remote
                );

            var surgeonConnector = ExtendedContext.GetDiskSugeonConnector(
                RoleEnum.Remote
                );

            var builder = ExtendedContext.CreateBuilder(surgeonConnector);

            var fileSystem = builder
                .Folder(A0FolderName, A1FolderName)
                        .PseudoRandomFile(A1FileName, A1FileSeed, A1FileLength)
                    .Up(2)
                .Folder(B0FolderName)
                    .EmptyFile(B0FileName)
                .Create(out schema)
                ;

            return fileSystem;
        }
    }
}