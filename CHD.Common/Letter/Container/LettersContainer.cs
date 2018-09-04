using System;
using System.Collections.Generic;
using System.Linq;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Native;
using CHD.Common.Saver;

namespace CHD.Common.Letter.Container
{
    public sealed class LettersContainer<TNativeMessage> : ILettersContainer<TNativeMessage>
        where TNativeMessage : NativeMessage
    {

        private readonly List<ILetter<TNativeMessage>> _letters;

        public IReadOnlyList<ILetter<TNativeMessage>> Letters
        {
            get
            {
                return
                    _letters;
            }
        }

        public long MaxOrder
        {
            get
            {
                if (_letters.Count == 0)
                {
                    return 0;
                }

                return
                    _letters.Max(j => j.Order);
            }
        }

        public LettersContainer(
            List<ILetter<TNativeMessage>> letters
            )
        {
            if (letters == null)
            {
                throw new ArgumentNullException("letters");
            }

            _letters = letters;
        }

        public List<Tuple<string, List<FileSnapshot<TNativeMessage>>>> GetSnapshots()
        {
            var result = new List<Tuple<string, List<FileSnapshot<TNativeMessage>>>>();

            var fileGroups = _letters
                .GroupBy(o => o.FullPathSequence.Path, j => j)
                ;

            foreach (var fileGroup in fileGroups)
            {
                var fileTransactionGroup = fileGroup
                    .OrderBy(j => j.Order)
                    .GroupBy(o => o.TransactionGuid, j => j)
                    ;

                var snapshots = fileTransactionGroup
                    .Select(j => new FileSnapshot<TNativeMessage>(j.ToList()))
                    .ToList()
                    ;

                var tuple = new Tuple<string, List<FileSnapshot<TNativeMessage>>>(
                    fileGroup.Key,
                    snapshots
                    );

                result.Add(tuple);
            }

            return result;
        }

        public List<FileSnapshot<TNativeMessage>> GetSnapshots(
            INamedFile sourceFile
            )
        {
            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }

            var result = new List<FileSnapshot<TNativeMessage>>();

            var groups = _letters
                .Where(j => j.FullPathSequence.IsEquals(sourceFile.FullPath))
                .OrderBy(j => j.Order)
                .GroupBy(o => o.TransactionGuid, j => j)
                ;

            foreach (var g in groups)
            {
                List<ILetter<TNativeMessage>> letters = g.ToList();

                var snapshot = new FileSnapshot<TNativeMessage>(
                    letters
                    );

                result.Add(snapshot);
            }

            return result;
        }

    }
}
