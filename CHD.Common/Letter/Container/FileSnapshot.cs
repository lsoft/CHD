using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHD.Common.Native;
using CHD.Common.Others;
using CHD.Common.Saver;

namespace CHD.Common.Letter.Container
{
    public sealed class FileSnapshot<TNativeMessage>
        where TNativeMessage : NativeMessage
    {
        private readonly List<ILetter<TNativeMessage>> _orderedLetters;

        public IReadOnlyList<ILetter<TNativeMessage>> Letters
        {
            get
            {
                return
                    _orderedLetters;
            }
        }

        public IReadOnlyList<TNativeMessage> Messages
        {
            get
            {
                return
                    _orderedLetters.Select(j => j.NativeMessage).ToList<TNativeMessage>();
            }
        }

        public long StructureVersion
        {
            get;
            private set;
        }

        public long MinOrder
        {
            get;
            private set;
        }

        public long MaxOrder
        {
            get;
            private set;
        }

        public long Size
        {
            get;
            private set;
        }

        public Guid TransactionGuid
        {
            get;
            private set;
        }

        public PathSequence FullPathSequence
        {
            get;
            private set;
        }

        public bool IsIncompleted
        {
            get;
            private set;
        }

        public FileSnapshot(
            List<ILetter<TNativeMessage>> orderedLetters
            
            )
        {
            if (orderedLetters == null)
            {
                throw new ArgumentNullException("orderedLetters");
            }
            if (orderedLetters.Count == 0)
            {
                throw new ArgumentException("orderedLetters");
            }

            if (orderedLetters.Any(j => j.StructureVersion != orderedLetters[0].StructureVersion))
            {
                throw new CHDException(
                    "Не совпадают версии структуры в рамках одной транзакции",
                    CHDExceptionTypeEnum.UnknownStructureProblem
                    );
            }
            if (orderedLetters.Any(j => j.Size != orderedLetters[0].Size))
            {
                throw new CHDException(
                    "Не совпадает Size в рамках одной транзакции",
                    CHDExceptionTypeEnum.UnknownStructureProblem
                    );
            }
            if (orderedLetters.Any(j => j.TransactionGuid != orderedLetters[0].TransactionGuid))
            {
                throw new CHDException(
                    "Не совпадает TransactionGuid в рамках одной транзакции",
                    CHDExceptionTypeEnum.UnknownStructureProblem
                    );
            }
            if (orderedLetters.Any(j => !j.FullPathSequence.IsEquals(orderedLetters[0].FullPathSequence)))
            {
                throw new CHDException(
                    "Не совпадает FullPathSequence в рамках одной транзакции",
                    CHDExceptionTypeEnum.UnknownStructureProblem
                    );
            }

            _orderedLetters = orderedLetters;

            StructureVersion = orderedLetters.First().StructureVersion;
            MinOrder = orderedLetters.Min(k => k.Order);
            MaxOrder = orderedLetters.Max(k => k.Order);
            Size = orderedLetters.First().Size;
            TransactionGuid = orderedLetters.First().TransactionGuid;
            FullPathSequence = orderedLetters.First().FullPathSequence;
            IsIncompleted = CalculateIncompletness(orderedLetters);
        }

        public void CopyTo(
            Stream destinationStream,
            long position,
            long size
            )
        {
            if (destinationStream == null)
            {
                throw new ArgumentNullException("destinationStream");
            }

            foreach (var l in _orderedLetters.Where(j => j.MessageType == MessageTypeEnum.BlockData).OrderBy(j => j.Order))
            {
                if (position > l.Size)
                {
                    position -= l.Size;
                    continue;
                }

                //этот блок надо скопировать полностью или частично

                //copiedSize - сколько было скопировано
                var copiedSize = l.WriteAttachmentTo(
                    destinationStream,
                    position,
                    size
                    );

                //раз сделали первое копирование, то дальше позиция равна нулю
                position = 0L;

                //ведем учет того, сколько байт было скопировано
                size -= copiedSize;

                //если скопировали все, что захотели?
                if (size == 0)
                {
                    break;
                }
            }
        }

        private static bool CalculateIncompletness(
            List<ILetter<TNativeMessage>> orderedLetters
            )
        {
            if (orderedLetters == null)
            {
                throw new ArgumentNullException("orderedLetters");
            }

            var ofc = orderedLetters.Count(j => j.MessageType == MessageTypeEnum.OpenFile);

            if (ofc != 1)
            {
                return
                    true;
            }

            var cfc = orderedLetters.Count(j => j.MessageType == MessageTypeEnum.CloseFile);

            if (cfc != 1)
            {
                return
                    true;
            }
 
            var ofi = orderedLetters.FindIndex(j => j.MessageType == MessageTypeEnum.OpenFile);
            var cfi = orderedLetters.FindIndex(j => j.MessageType == MessageTypeEnum.CloseFile);

            if (ofi > cfi)
            {
                return
                    true;
            }

            var bdsi = orderedLetters.FindIndex(j => j.MessageType == MessageTypeEnum.BlockData);
            var bdki = orderedLetters.FindLastIndex(j => j.MessageType == MessageTypeEnum.BlockData);

            if (bdsi == -1 && bdki == -1)
            {
                //it is an empty file, with size = 0
                //so there is no blocks with data

                return false;
            }

            if (ofi > bdsi || ofi > bdki)
            {
                return
                    true;
            }

            if (bdsi > cfi || bdki > cfi)
            {
                return
                    true;
            }

            return
                false;
        }
    }
}