using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Letter.Container;
using CHD.Common.Letter.Executor;
using CHD.Common.Native;
using CHD.Common.Others;
using CHD.Common.PathComparer;
using CHD.Common.Structure;

namespace CHD.Common.Saver.Body
{
    public sealed class BodyProcessor<TNativeMessage> : IBodyProcessor
        where TNativeMessage : NativeMessage
    {
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly ILetterExecutor<TNativeMessage> _executor;

        public BodyProcessor(
            IPathComparerProvider pathComparerProvider,
            ILetterExecutor<TNativeMessage> executor
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }

            _pathComparerProvider = pathComparerProvider;
            _executor = executor;
        }

        public void Cleanup(
            StructureChecker checker
            )
        {
            if (checker == null)
            {
                throw new ArgumentNullException("checker");
            }

            var toDelete = new List<FileSnapshot<TNativeMessage>>();

            var container = _executor.ReadAllLetters();
            var snapshotTuples = container.GetSnapshots();


            var td0 = ExtractFuturedSnapshots(checker, snapshotTuples);
            toDelete.AddRange(td0);

            var td1 = ExtractIncompletedSnapshots(snapshotTuples);
            toDelete.AddRange(td1);

            var td2 = ExtractObsoletedSnapshots(checker, snapshotTuples);
            toDelete.AddRange(td2);

            if (toDelete.Count > 0)
            {
                //toDelete is not distincted list! it can contains duplicates!
                //_executor.DeleteSnapshots will remove duplicates before performing, so go on!
                _executor.DeleteSnapshots(toDelete);
            }
        }

        public void SaveNewSnapshot(
            int structureCurrentVersion,
            IFileSystemCopier copier,
            ICopyableFile sourceFile,
            IFile targetFile
            )
        {
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }
            if (targetFile == null)
            {
                throw new ArgumentNullException("targetFile");
            }

            var container = _executor.ReadAllLetters();

            _executor.SaveFile(
                structureCurrentVersion,
                container.MaxOrder,
                copier,
                sourceFile,
                targetFile
                );
        }

        public void DeleteLastSnapshot(
            INamedFile file
            )
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var container = _executor.ReadAllLetters();
            var snapshots = container.GetSnapshots(file);
            var lastSnapshot = snapshots.Last();

            _executor.DeleteSnapshot(lastSnapshot);
        }

        public long CopySnapshotTo(
            int structureVersion,
            ICopyableFile sourceFile,
            Stream destinationStream,
            long position,
            long size
            )
        {
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }

            var container = _executor.ReadAllLetters();
            var snapshots = container.GetSnapshots(sourceFile);
            var snapshot = snapshots.FirstOrDefault(j => j.StructureVersion == structureVersion);

            if (snapshot == null)
            {
                throw new CHDException(
                    string.Format("Structure with version {0} does not found", structureVersion),
                    CHDExceptionTypeEnum.UnknownStructureVersion
                    );
            }

            snapshot.CopyTo(
                destinationStream,
                position,
                size
                );

            return
                size;
        }

        public long CopyLastSnapshotTo(
            ICopyableFile sourceFile,
            Stream destinationStream,
            long position,
            long size
            )
        {
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }

            var container = _executor.ReadAllLetters();
            var snapshots = container.GetSnapshots(sourceFile);
            var lastSnapshot = snapshots.Last();

            lastSnapshot.CopyTo(
                destinationStream,
                position,
                size
                );

            return
                size;
        }


        #region cleanup

        private List<FileSnapshot<TNativeMessage>> ExtractFuturedSnapshots(
            StructureChecker checker,
            List<Tuple<string, List<FileSnapshot<TNativeMessage>>>> snapshotTuples
            )
        {
            if (snapshotTuples == null)
            {
                throw new ArgumentNullException("snapshotTuples");
            }

            var toDelete = new List<FileSnapshot<TNativeMessage>>();

            snapshotTuples
                .Select(j => j.Item2)
                .ForEach(list => toDelete.AddRange(list.Where(j => j.StructureVersion > checker.LastVersion)))
                ;

            return
                toDelete;
        }

        private List<FileSnapshot<TNativeMessage>> ExtractIncompletedSnapshots(
            List<Tuple<string, List<FileSnapshot<TNativeMessage>>>> snapshotTuples
            )
        {
            if (snapshotTuples == null)
            {
                throw new ArgumentNullException("snapshotTuples");
            }

            var toDelete = new List<FileSnapshot<TNativeMessage>>();

            snapshotTuples
                .Select(j => j.Item2)
                .ForEach(list => toDelete.AddRange(list.Where(j => j.IsIncompleted)))
                ;


            return
                toDelete;
        }

        private List<FileSnapshot<TNativeMessage>> ExtractObsoletedSnapshots(
            StructureChecker checker,
            List<Tuple<string, List<FileSnapshot<TNativeMessage>>>> snapshotTuples
            )
        {
            if (checker == null)
            {
                throw new ArgumentNullException("checker");
            }
            if (snapshotTuples == null)
            {
                throw new ArgumentNullException("snapshotTuples");
            }

            var toDelete = new List<FileSnapshot<TNativeMessage>>();

            //берем снепшоты всех файлов
            foreach (var snapshots in snapshotTuples)
            {
                var filePathSequence = new PathSequence(_pathComparerProvider, snapshots.Item1);

                var exists = checker.IsFileExists(filePathSequence);

                if (exists)
                {
                    //в одной из версий структуры файл присутствует
                    //так как мы не знаем, какие ВЕРСИИ ФАЙЛА используются в этих структурах
                    //то лучше не удалять вообще никакие версии файла
                    //это гарантирует его целостность
                }
                else
                {
                    //файл в структурах не найден
                    //его можно полностью удалить
                    toDelete.AddRange(snapshots.Item2);
                }
            }

            return
                toDelete;
        }

        #endregion
    }
}
