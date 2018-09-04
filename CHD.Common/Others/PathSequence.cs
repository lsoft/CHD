using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.PathComparer;
using CHD.Common.Serializer;

namespace CHD.Common.Others
{
    [Serializable]
    [DebuggerDisplay("{Path}")]
    public sealed class PathSequence
    {
        public SerializationVersionProvider<PathSequence> SerializationVersion = new SerializationVersionProvider<PathSequence>();

        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly List<string> _sequence;

        public string Path
        {
            get;
            private set;
        }

        public IReadOnlyList<string> Sequence
        {
            get
            {
                return
                    _sequence;
            }
        }

        public int Count
        {
            get
            {
                return
                    _sequence.Count;
            }
        }

        private PathSequence(
            IEnumerable<string> sequence
            )
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            _sequence = sequence.ToList();

            Path = System.IO.Path.Combine(_sequence.ToArray());
        }

        public PathSequence(
            PathSequence parent,
            string additionalPiece
            )
            : this(parent._pathComparerProvider, System.IO.Path.Combine(parent.Path, additionalPiece))
        {
        }

        public PathSequence(
            IPathComparerProvider pathComparerProvider,
            string path
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _pathComparerProvider = pathComparerProvider;
            Path = path;

            _sequence = path.Split(
                new[]
                {
                    System.IO.Path.DirectorySeparatorChar,
                    System.IO.Path.AltDirectorySeparatorChar
                }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public PathSequence Up()
        {
            return
                this.CutTail(1);
        }

        public PathSequence CutTail(
            int cutted = 1
            )
        {
            return 
                new PathSequence(
                    _sequence.TakeWithoutTail(cutted)
                    );
        }

        public bool IsEquals(
            PathSequence sequence
            )
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (this._sequence.Count != sequence._sequence.Count)
            {
                return false;
            }

            var r = Helper.ArrayEquals(
                this._sequence,
                sequence._sequence,
                _pathComparerProvider.Comparer
                );

            return
                r;
        }

        public bool IsEquals(
            string fullPath
            )
        {
            var cmp = string.Compare(this.Path, fullPath, _pathComparerProvider.Comparison);

            return cmp == 0;
        }

        public bool IsChild(
            PathSequence parent
            )
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (this._sequence.Count != parent._sequence.Count - 1)
            {
                return false;
            }

            var r = Helper.ArrayEquals(
                this._sequence,
                parent._sequence,
                this._sequence.Count,
                _pathComparerProvider.Comparer
                );

            return
                r;
        }

        public string this[int cc]
        {
            get
            {
                return
                    _sequence[cc];
            }
        }




    }
}
