using System;
using System.Collections;
using System.Collections.Generic;
using CHD.Common.FileSystem;
using CHD.Common.FileSystem.FFolder;

namespace CHD.Common.Operation
{
    public sealed class FolderComparer : IComparer, IEqualityComparer, IComparer<IFolder>, IEqualityComparer<IFolder>
    {
        private readonly StringComparer _comparer;

        public FolderComparer(
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

            var xx = (IFolder)x;
            var yy = (IFolder)y;

            return
                Compare(xx, yy);
        }

        public new bool Equals(object x, object y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            var xx = (IFolder)x;
            var yy = (IFolder)y;

            return
                Equals(xx, yy);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var xobj = obj as IFolder;

            return
                GetHashCode(xobj);
        }

        public int Compare(IFolder x, IFolder y)
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

        public bool Equals(IFolder x, IFolder y)
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

        public int GetHashCode(IFolder obj)
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