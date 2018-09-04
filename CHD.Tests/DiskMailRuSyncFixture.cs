using System;
using CHD.Common;
using CHD.Common.FileSystem.Surgeon;
using CHD.MailRuCloud.Settings;
using CHD.Tests.ExtendedContext;
using CHD.Tests.FileSystem;
using CHD.Tests.FileSystem.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CHD.Tests
{
    [TestClass]
    public sealed class DiskMailRuSyncFixture : VersionedSyncFixture
    {
        public const string PathToSettingsFile = @"..\..\..\_PrivateData\Test\_cloud.mail.ru.xml";


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            var settings = new Settings.Controller.Settings(
                PathToSettingsFile
                );

            ExtendedContext.BindDisk(
                RootFolderName,
                StructureFileName,
                RoleEnum.Local
                );

            var mailRuSettings = new MailRuSettings(
                settings
                );

            ExtendedContext.BindMailRu(
                RootFolderName,
                mailRuSettings
                );

            CleanupMailRu(
                );
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            CleanupMailRu(
                );

            base.TestCleanup();
        }

        protected override IFileSystemSurgeonConnector PrepareLocalConfiguration0()
        {
            IFileSystemSurgeonConnector surgeonConnector = ExtendedContext.GetDiskSugeonConnector(
                RoleEnum.Local
                );

            FileSystemBuilder builder = ExtendedContext.CreateBuilder(surgeonConnector);

            IFileSystemSchema schema;
            IFileSystemSurgeonConnector fileSystem = builder
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

            IFileSystemSurgeonConnector surgeonConnector = ExtendedContext.GetDiskSugeonConnector(
                RoleEnum.Another
                );

            FileSystemBuilder builder = ExtendedContext.CreateBuilder(surgeonConnector);

            IFileSystemSchema schema;
            IFileSystemSurgeonConnector fileSystem = builder
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
            IFileSystemSurgeonConnector surgeonConnector = ExtendedContext.GetMailRuSugeonConnector(
                );

            FileSystemBuilder builder = ExtendedContext.CreateBuilder(surgeonConnector);

            IFileSystemSurgeonConnector fileSystem = builder
                .Folder(A0FolderName, A1FolderName)
                    .PseudoRandomFile(A1FileName, A1FileSeed, A1FileLength)
                .Up(2)
                    .Folder(B0FolderName)
                        .EmptyFile(B0FileName)
                .Create(out schema)
                ;

            return fileSystem;
        }

        #region cleanup MailRu

        public void CleanupMailRu(
            )
        {
            var cleaner = ExtendedContext.Get<IRemoteFileSystemCleaner>();
            cleaner.SafelyClear();
        }

        #endregion
    }
}