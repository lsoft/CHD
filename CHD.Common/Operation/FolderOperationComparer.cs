using System;
using System.Collections;
using System.Collections.Generic;
using CHD.Common.Operation.FolderOperation;

namespace CHD.Common.Operation
{
    public sealed class FolderOperationComparer : IComparer, IEqualityComparer, IComparer<IFolderOperation>, IEqualityComparer<IFolderOperation>
    {
        private readonly StringComparer _comparer;

        public FolderOperationComparer(
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

            var xx = (IFolderOperation)x;
            var yy = (IFolderOperation)y;

            return
                Compare(xx, yy);
        }

        public new bool Equals(object x, object y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            var xx = (IFolderOperation)x;
            var yy = (IFolderOperation)y;

            return
                Equals(xx, yy);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var xobj = obj as IFolderOperation;

            return
                GetHashCode(xobj);
        }

        public int Compare(IFolderOperation x, IFolderOperation y)
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

            if (x == null && y == null)
            {
                return 0;
            }

            return
                _comparer.Compare(x.FullPath, y.FullPath);
        }

        public bool Equals(IFolderOperation x, IFolderOperation y)
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

            return
                _comparer.Equals(x.FullPath, y.FullPath);
        }

        public int GetHashCode(IFolderOperation obj)
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