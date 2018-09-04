using System;
using System.Collections;
using System.Collections.Generic;
using CHD.Common.Operation.FileOperation;

namespace CHD.Common.Operation
{
    public sealed class FileOperationComparer : IComparer, IEqualityComparer, IComparer<IFileOperation>, IEqualityComparer<IFileOperation>
    {
        private readonly StringComparer _comparer;

        public FileOperationComparer(
            StringComparer comparer
            )
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            _comparer = comparer;
        }

        public int Compare(object x, object y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            var xx = (IFileOperation)x;
            var yy = (IFileOperation)y;

            return
                Compare(xx, yy);
        }

        public new bool Equals(object x, object y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            var xx = (IFileOperation)x;
            var yy = (IFileOperation)y;

            return
                Equals(xx, yy);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var xobj = obj as IFileOperation;

            return
                GetHashCode(xobj);
        }

        public int Compare(IFileOperation x, IFileOperation y)
        {
            if (x != null && y == null)
            {
                return
                    1;
            }
            if (x == null && y != null)
            {
                return
                    -1;
            }

            if (ReferenceEquals(x, y))
            {
                return
                    0;
            }

            return
                _comparer.Compare(x.FullPath, y.FullPath);
        }

        public bool Equals(IFileOperation x, IFileOperation y)
        {

            if (x != null && y == null)
            {
                return
                    false;
            }
            if (x == null && y != null)
            {
                return
                    false;
            }

            if (ReferenceEquals(x, y))
            {
                return
                    true;
            }

            return
                _comparer.Equals(x.FullPath, y.FullPath);
        }

        public int GetHashCode(IFileOperation obj)
        {
            if (obj == null)
            {
                return 0;
            }
            if (obj.FullPath == null)
            {
                return 0;
            }

            return
                obj.FullPath.GetHashCode();
        }
    }
}