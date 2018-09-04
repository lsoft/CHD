using CHD.Common.FileSystem.FFile;
using CHD.Common.Others;
using CHD.Tests.ExtendedContext;
using CHD.Tests.FileSystem.Schema;
using CHD.Tests.Other;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace CHD.Tests
{
    public abstract class VersionedSyncFixture : SyncFixture
    {
        [TestMethod]
        public void FileVersionTest()
        {
            const string A0FileName = "a0file.txt";
            const int A0FileSeed1 = 8844;
            const int A0FileSeed2 = 8845;
            const int A0FileLength = 2048;

            var slocal = PrepareLocalConfiguration0();
            IFileSystemSchema beforeRemoteSchema;
            var sremote = PrepareRemoteConfiguration0(out beforeRemoteSchema);

            var local = slocal.Connector;
            var remote = sremote.Connector;

            var a0FileBody0 = TestHelper.GenerateBufferWithRandomData(A0FileSeed1, A0FileLength);

            var beforeSchema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(A0FolderName)
                    .CreatePseudoRandomFile(A0FileName, a0FileBody0)
                .CloseFileSystem()
                ;

            {
                var sync0 = ExtendedContext.CreateSynchronizer(
                    RoleEnum.Local,
                    local,
                    remote
                    );

                var syncReport0 = sync0.Sync();

                Assert.AreEqual(0, syncReport0.Local.TotalTouched);
                Assert.AreEqual(1, syncReport0.Remote.TotalTouched);
                Assert.AreEqual(1, syncReport0.Remote.CreatedFilesCount);
                Assert.AreEqual(0, syncReport0.Remote.RecreatedFilesCount);
                Assert.AreEqual(0, syncReport0.Remote.DeletedFilesCount);
            }

            byte[] a0FileBody1;
            var middleSchema = ExtendedContext.GetSurgeonFactory(RoleEnum.Local).Surge()
                .Enter(A0FolderName)
                    .ChangeFile(A0FileName, A0FileSeed2, out a0FileBody1)
                .CloseFileSystem()
                ;

            {
                var sync1 = ExtendedContext.CreateSynchronizer(
                    RoleEnum.Local,
                    local,
                    remote
                    );

                var syncReport1 = sync1.Sync();

                Assert.AreEqual(0, syncReport1.Local.TotalTouched);
                Assert.AreEqual(1, syncReport1.Remote.TotalTouched);
                Assert.AreEqual(0, syncReport1.Remote.CreatedFilesCount);
                Assert.AreEqual(1, syncReport1.Remote.RecreatedFilesCount);
                Assert.AreEqual(0, syncReport1.Remote.DeletedFilesCount);
            }

            IFile a0FileRemote;
            var afterSchema = ExtendedContext.CreateNavigator(remote)
                .Enter(A0FolderName)
                    .GetFile(A0FileName, out a0FileRemote)
                .CloseFileSystem()
                ;

            var a0FileRemoteBody0 = a0FileRemote.ExtractBody(
                structureContainer =>
                {
                    return
                        structureContainer.Structure.Penult != null ? structureContainer.Structure.Penult.Version : 0;
                },
                remote
                );
            var a0FileRemoteBody1 = a0FileRemote.ExtractBody(
                remote
                );


            Assert.IsTrue(beforeSchema.SchemaEquals(middleSchema));
            Assert.IsTrue(beforeSchema.SchemaEquals(afterSchema));

            Assert.IsTrue(Helper.ArrayEquals(a0FileBody0, a0FileRemoteBody0));
            Assert.IsTrue(a0FileBody1.ArrayEquals(a0FileRemoteBody1));
        }
    }
}