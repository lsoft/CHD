using System;
using CHD.Common.Diff;
using CHD.Common.Diff.Constructor;
using CHD.Common.FileSystem;

using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.Surgeon;
using CHD.Common.Others;
using CHD.Common.Scanner;
using CHD.Tests.ExtendedContext;
using CHD.Tests.FileSystem;
using CHD.Tests.FileSystem.Schema;
using CHD.Tests.Other;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CHD.Tests
{
    public abstract class SyncFixture
    {
        protected const string StructureFileName = "$StructureData";
        protected const string RootFolderName = "Root";


        /* Configuration 0:
         * 
         *      RootFolder
         *          A0
         *              A1
         *                  a1file.txt
         *          B0
         *              b0file.txt
         *              B1                          <---------- created by the subset of the unit tests
         *                  b1file.txt              <---------- created by the subset of the unit tests
         *                  B2                      <---------- created by the subset of the unit tests
         *                      b2file.txt          <---------- created by the subset of the unit tests
         */

        protected const string A0FolderName = "A0";
        protected const string A1FolderName = "A1";
        protected const string B0FolderName = "B0";
        protected const string B1FolderName = "B1";
        protected const string B2FolderName = "B2";

        protected const string A1FileName = "a1file.txt";
        protected const string B0FileName = "b0file.txt";
        protected const string B1FileName = "b1file.txt";
        protected const string B2FileName = "b2file.txt";

        protected const int A1FileSeed = 0xa1;
        protected const int A1FileLength = 1024;

        protected const int B1FileSeed = 0xb1;
        protected const int B1FileLength = 2048;

        protected const int B2FileSeed = 0xb2;
        protected const int B2FileLength = 1536;

        protected abstract IFileSystemSurgeonConnector PrepareLocalConfiguration0();
        protected abstract IFileSystemSurgeonConnector PrepareAnotherConfiguration0();
        protected abstract IFileSystemSurgeonConnector PrepareRemoteConfiguration0(out IFileSystemSchema schema);

// ReSharper disable once InconsistentNaming
        protected ExtendedContext.ExtendedContext ExtendedContext
        {
            get;
            private set;
        }

        protected SyncFixture()
        {
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            ExtendedContext = new ExtendedContext.ExtendedContext(
                new DebugLogger()
                );
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            ExtendedContext.Log();
            ExtendedContext.Close();
        }

        [TestMethod]
        public void NoActionTest()
        {
            var slocal = PrepareLocalConfiguration0();
            IFileSystemSchema beforeRemoteSchema;
            var sremote = PrepareRemoteConfiguration0(out beforeRemoteSchema);

            var local = slocal.Connector;
            var remote = sremote.Connector;

            var sync = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport0 = sync.Sync();

            Assert.AreEqual(0, syncReport0.Local.TotalTouched);
            Assert.AreEqual(0, syncReport0.Remote.TotalTouched);
        }

        [TestMethod]
        public void DeleteFileTest()
        {
            var slocal = PrepareLocalConfiguration0();
            IFileSystemSchema beforeRemoteSchema;
            var sremote = PrepareRemoteConfiguration0(out beforeRemoteSchema);

            var local = slocal.Connector;
            var remote = sremote.Connector;

            var beforeSchema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(B0FolderName)
                    .DeleteFile(B0FileName, true)
                .CloseFileSystem()
                ;

            var sync = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport = sync.Sync();

            Assert.AreEqual(0, syncReport.Local.TotalTouched);
            Assert.AreEqual(1, syncReport.Remote.TotalTouched);
            Assert.AreEqual(1, syncReport.Remote.DeletedFilesCount);


            var afterSchema = ExtendedContext.GetSchema(remote);
            Assert.IsTrue(beforeSchema.SchemaEquals(afterSchema));
        }

        [TestMethod]
        public void DeleteFolderTest()
        {
            var slocal = PrepareLocalConfiguration0();
            IFileSystemSchema beforeRemoteSchema;
            var sremote = PrepareRemoteConfiguration0(out beforeRemoteSchema);

            var local = slocal.Connector;
            var remote = sremote.Connector;

            var beforeLocalSchema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(A0FolderName)
                    .DeleteFolder(A1FolderName, true)
                .CloseFileSystem()
                ;
            beforeLocalSchema.Log("BEFORE LOCAL");

            beforeRemoteSchema.Log("BEFORE REMOTE");

            var sync = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport = sync.Sync();

            Assert.AreEqual(0, syncReport.Local.TotalTouched);
            Assert.AreEqual(2, syncReport.Remote.TotalTouched);
            Assert.AreEqual(1, syncReport.Remote.DeletedFilesCount);
            Assert.AreEqual(1, syncReport.Remote.DeletedFoldersCount);
            Assert.AreEqual(0, syncReport.Remote.CreatedFoldersCount);
            Assert.AreEqual(0, syncReport.Remote.RecreatedFoldersCount);

            var afterSchema = ExtendedContext.GetSchema(remote);
            afterSchema.Log("AFTER REMOTE");
            Assert.IsTrue(beforeLocalSchema.SchemaEquals(afterSchema));
        }


        [TestMethod]
        public void UpdateFileTest()
        {
            const int A1FileSeedNew = 3433;

            var slocal = PrepareLocalConfiguration0();
            IFileSystemSchema beforeRemoteSchema;
            var sremote = PrepareRemoteConfiguration0(out beforeRemoteSchema);

            var local = slocal.Connector;
            var remote = sremote.Connector;

            byte[] a1FileLocalBody;
            var beforeSchema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(A0FolderName, A1FolderName)
                    .ChangeFile(A1FileName, A1FileSeedNew, out a1FileLocalBody)
                .CloseFileSystem()
                ;

            IFile a1FileRemote0;
            ExtendedContext.CreateNavigator(remote)
                .Enter(A0FolderName, A1FolderName)
                    .GetFile(A1FileName, out a1FileRemote0)
                .CloseFileSystem()
                ;
            
            var a1FileRemote0Body = a1FileRemote0.ExtractBody(remote);

            var sync = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport = sync.Sync();

            Assert.AreEqual(0, syncReport.Local.TotalTouched);
            Assert.AreEqual(1, syncReport.Remote.TotalTouched);
            Assert.AreEqual(0, syncReport.Remote.CreatedFilesCount);
            Assert.AreEqual(1, syncReport.Remote.RecreatedFilesCount);
            Assert.AreEqual(0, syncReport.Remote.DeletedFilesCount);

            IFile a1FileRemote1;
            var afterSchema = ExtendedContext.CreateNavigator(remote)
                .Enter(A0FolderName, A1FolderName)
                    .GetFile(A1FileName, out a1FileRemote1)
                .CloseFileSystem()
                ;

            var a1FileRemote1Body = a1FileRemote1.ExtractBody(remote);

            Assert.IsFalse(a1FileLocalBody.ArrayEquals(a1FileRemote0Body));
            Assert.IsFalse(a1FileRemote1Body.ArrayEquals(a1FileRemote0Body));
            Assert.IsTrue(a1FileRemote1Body.ArrayEquals(a1FileLocalBody));

            Assert.IsTrue(beforeSchema.SchemaEquals(afterSchema));
        }

        [TestMethod]
        public void CreateFileTest()
        {
            const string A0FileName = "a0file.txt";
            const int A0FileSeed = 8844;
            const int A0FileLength = 2048;

            var slocal = PrepareLocalConfiguration0();
            IFileSystemSchema beforeRemoteSchema;
            var sremote = PrepareRemoteConfiguration0(out beforeRemoteSchema);

            var local = slocal.Connector;
            var remote = sremote.Connector;

            var a0FileBody = TestHelper.GenerateBufferWithRandomData(A0FileSeed, A0FileLength);
            //for (var cc = 0; cc < a0FileBody.Length; cc++) a0FileBody[cc] = (byte) ((cc/256) + 1);
            //for (var cc = 0; cc < a0FileBody.Length; cc++) a0FileBody[cc] = 0x0a;

            var beforeSchema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(A0FolderName)
                    .CreatePseudoRandomFile(A0FileName, a0FileBody)
                .CloseFileSystem()
                ;

            //IFile a0FileLocal;
            //ExtendedContext.CreateNavigator(local)
            //    .Enter(A0FolderName)
            //        .GetFile(A0FileName, out a0FileLocal)
            //    .CloseFileSystem()
            //    ;

            //var a0FileLocalBody = a0FileLocal.ExtractBody(local);

            //Assert.IsTrue(a0FileBody.ArrayEquals(a0FileLocalBody));

            var sync = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport = sync.Sync();

            Assert.AreEqual(0, syncReport.Local.TotalTouched);
            Assert.AreEqual(1, syncReport.Remote.TotalTouched);
            Assert.AreEqual(1, syncReport.Remote.CreatedFilesCount);
            Assert.AreEqual(0, syncReport.Remote.RecreatedFilesCount);
            Assert.AreEqual(0, syncReport.Remote.DeletedFilesCount);

            IFile a0FileRemote;
            var afterSchema = ExtendedContext.CreateNavigator(remote)
                .Enter(A0FolderName)
                    .GetFile(A0FileName, out a0FileRemote)
                .CloseFileSystem()
                ;

            var a0FileRemoteBody = a0FileRemote.ExtractBody(remote);

            Assert.IsTrue(a0FileBody.ArrayEquals(a0FileRemoteBody));

            Assert.IsTrue(beforeSchema.SchemaEquals(afterSchema));
        }

        [TestMethod]
        public void CreateFolderTest()
        {
            var slocal = PrepareLocalConfiguration0();
            IFileSystemSchema beforeRemoteSchema;
            var sremote = PrepareRemoteConfiguration0(out beforeRemoteSchema);

            var local = slocal.Connector;
            var remote = sremote.Connector;

            var beforeSchema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(B0FolderName)
                    .CreateFolder(B1FolderName)
                .CloseFileSystem()
                ;

            var sync = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport = sync.Sync();

            Assert.AreEqual(0, syncReport.Local.TotalTouched);
            Assert.AreEqual(1, syncReport.Remote.TotalTouched);
            Assert.AreEqual(0, syncReport.Remote.CreatedFilesCount);
            Assert.AreEqual(0, syncReport.Remote.RecreatedFilesCount);
            Assert.AreEqual(0, syncReport.Remote.DeletedFilesCount);
            Assert.AreEqual(1, syncReport.Remote.CreatedFoldersCount);
            Assert.AreEqual(0, syncReport.Remote.RecreatedFoldersCount);

            var afterSchema = ExtendedContext.GetSchema(remote);

            Assert.IsTrue(beforeSchema.SchemaEquals(afterSchema));
        }

        [TestMethod]
        public void CreateTreeTest()
        {
            var slocal = PrepareLocalConfiguration0();
            IFileSystemSchema beforeRemoteSchema;
            var sremote = PrepareRemoteConfiguration0(out beforeRemoteSchema);

            var local = slocal.Connector;
            var remote = sremote.Connector;

            var b1FileBody = TestHelper.GenerateBufferWithRandomData(B1FileSeed, B1FileLength);
            var b2FileBody = TestHelper.GenerateBufferWithRandomData(B2FileSeed, B2FileLength);

            var beforeSchema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(B0FolderName)
                .CreateFolder(B1FolderName)
                .CreatePseudoRandomFile(B1FileName, b1FileBody)
                .CreateFolder(B2FolderName)
                .CreatePseudoRandomFile(B2FileName, b2FileBody)
                .CloseFileSystem()
                ;

            var sync = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport = sync.Sync();

            Assert.AreEqual(0, syncReport.Local.TotalTouched);
            Assert.AreEqual(4, syncReport.Remote.TotalTouched);
            Assert.AreEqual(2, syncReport.Remote.CreatedFilesCount);
            Assert.AreEqual(0, syncReport.Remote.RecreatedFilesCount);
            Assert.AreEqual(0, syncReport.Remote.DeletedFilesCount);
            Assert.AreEqual(2, syncReport.Remote.CreatedFoldersCount);
            Assert.AreEqual(0, syncReport.Remote.RecreatedFoldersCount);

            var afterSchema = ExtendedContext.GetSchema(remote);

            Assert.IsTrue(beforeSchema.SchemaEquals(afterSchema));
        }

        [TestMethod]
        public void TwoSyncsTest()
        {
            var slocal = PrepareLocalConfiguration0();
            IFileSystemSchema beforeRemoteSchema;
            var sremote = PrepareRemoteConfiguration0(out beforeRemoteSchema);

            var local = slocal.Connector;
            var remote = sremote.Connector;

            #region sync 0

            var b1FileBody = TestHelper.GenerateBufferWithRandomData(B1FileSeed, B1FileLength);

            var before0Schema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(B0FolderName)
                    .CreateFolder(B1FolderName)
                        .CreatePseudoRandomFile(B1FileName, b1FileBody)
                .CloseFileSystem()
                ;

            var synchronizer0 = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport0 = synchronizer0.Sync();

            Assert.AreEqual(0, syncReport0.Local.TotalTouched);
            Assert.AreEqual(2, syncReport0.Remote.TotalTouched);
            Assert.AreEqual(1, syncReport0.Remote.CreatedFilesCount);
            Assert.AreEqual(0, syncReport0.Remote.RecreatedFilesCount);
            Assert.AreEqual(0, syncReport0.Remote.DeletedFilesCount);
            Assert.AreEqual(1, syncReport0.Remote.CreatedFoldersCount);
            Assert.AreEqual(0, syncReport0.Remote.RecreatedFoldersCount);

            var after0Schema = ExtendedContext.GetSchema(remote);
            Assert.IsTrue(before0Schema.SchemaEquals(after0Schema));

            #endregion

            #region sync 1

            var before1Schema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(B0FolderName, B1FolderName)
                    .DeleteFile(B1FileName, true)
                .CloseFileSystem()
                ;

            var synchronizer1 = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport1 = synchronizer1.Sync();

            Assert.AreEqual(0, syncReport1.Local.TotalTouched);
            Assert.AreEqual(1, syncReport1.Remote.TotalTouched);
            Assert.AreEqual(0, syncReport1.Remote.CreatedFilesCount);
            Assert.AreEqual(0, syncReport1.Remote.RecreatedFilesCount);
            Assert.AreEqual(1, syncReport1.Remote.DeletedFilesCount);
            Assert.AreEqual(0, syncReport1.Remote.CreatedFoldersCount);
            Assert.AreEqual(0, syncReport1.Remote.RecreatedFoldersCount);

            var after1Schema = ExtendedContext.GetSchema(remote);
            Assert.IsTrue(before1Schema.SchemaEquals(after1Schema));

            #endregion
        }


        [TestMethod]
        public void TwoClientsSyncingTest()
        {
            var slocal = PrepareLocalConfiguration0();
            var sanother = PrepareAnotherConfiguration0();
            IFileSystemSchema schemaRemoteBefore;
            var sremote = PrepareRemoteConfiguration0(out schemaRemoteBefore);

            var local = slocal.Connector;
            var another = sanother.Connector;
            var remote = sremote.Connector;

            #region sync local <-> remote

            var b1FileBody = TestHelper.GenerateBufferWithRandomData(B1FileSeed, B1FileLength);

            var schemaLocalBefore0 = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(B0FolderName)
                    .CreateFolder(B1FolderName)
                        .CreatePseudoRandomFile(B1FileName, b1FileBody)
                .CloseFileSystem()
                ;

            var synchronizer0 = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport0 = synchronizer0.Sync();

            Assert.AreEqual(0, syncReport0.Local.TotalTouched);
            Assert.AreEqual(2, syncReport0.Remote.TotalTouched);
            Assert.AreEqual(1, syncReport0.Remote.CreatedFilesCount);
            Assert.AreEqual(0, syncReport0.Remote.RecreatedFilesCount);
            Assert.AreEqual(0, syncReport0.Remote.DeletedFilesCount);
            Assert.AreEqual(1, syncReport0.Remote.CreatedFoldersCount);
            Assert.AreEqual(0, syncReport0.Remote.RecreatedFoldersCount);

            var schemaRemoteAfter0 = ExtendedContext.GetSchema(remote);
            Assert.IsTrue(schemaLocalBefore0.SchemaEquals(schemaRemoteAfter0));

            #endregion

            #region sync another <-> remote

            var synchronizer1 = ExtendedContext.CreateSynchronizer(
                RoleEnum.Another,
                another,
                remote
                );

            var syncReport1 = synchronizer1.Sync();

            Assert.AreEqual(2, syncReport1.Local.TotalTouched);
            Assert.AreEqual(1, syncReport1.Local.CreatedFilesCount);
            Assert.AreEqual(0, syncReport1.Local.RecreatedFilesCount);
            Assert.AreEqual(0, syncReport1.Local.DeletedFilesCount);
            Assert.AreEqual(1, syncReport1.Local.CreatedFoldersCount);
            Assert.AreEqual(0, syncReport1.Local.RecreatedFoldersCount);
            Assert.AreEqual(0, syncReport1.Remote.TotalTouched);

            var schemaAnotherAfter1 = ExtendedContext.GetSchema(another);
            var schemaRemoteAfter1 = ExtendedContext.GetSchema(remote);
            Assert.IsTrue(schemaLocalBefore0.SchemaEquals(schemaAnotherAfter1));
            Assert.IsTrue(schemaLocalBefore0.SchemaEquals(schemaRemoteAfter1));

            #endregion

            #region sync local <-> remote

            var schemaLocalBefore2 = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(B0FolderName, B1FolderName)
                        .DeleteFile(B1FileName, true)
                .CloseFileSystem()
                ;

            var synchronizer2 = ExtendedContext.CreateSynchronizer(
                RoleEnum.Local,
                local,
                remote
                );

            var syncReport2 = synchronizer2.Sync();

            Assert.AreEqual(0, syncReport2.Local.TotalTouched);
            Assert.AreEqual(1, syncReport2.Remote.TotalTouched);
            Assert.AreEqual(0, syncReport2.Remote.CreatedFilesCount);
            Assert.AreEqual(0, syncReport2.Remote.RecreatedFilesCount);
            Assert.AreEqual(1, syncReport2.Remote.DeletedFilesCount);
            Assert.AreEqual(0, syncReport2.Remote.CreatedFoldersCount);
            Assert.AreEqual(0, syncReport2.Remote.RecreatedFoldersCount);

            var schemaRemoteAfter2 = ExtendedContext.GetSchema(remote);
            Assert.IsTrue(schemaLocalBefore2.SchemaEquals(schemaRemoteAfter2));

            #endregion

        }

    }
}