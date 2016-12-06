using System;
using System.Collections.Generic;
using CHD.FileSystem.Algebra;

namespace CHD.FileSystem.FileWrapper
{
    public class SuffixEqualityComparer : IEqualityComparer<Suffix>
    {
        public bool Equals(Suffix x, Suffix y)
        {
            return
                EqualsStatic(x, y);
        }

        public int GetHashCode(Suffix obj)
        {
            return
                GetHashCodeStatic(obj);
        }



        public static bool EqualsStatic(Suffix x, Suffix y)
        {
            if (x == null && y == null)
            {
                return
                    false;
            }

            if (x != null && y != null)
            {
                return
                    StringComparer.InvariantCultureIgnoreCase.Equals(x.FilePathSuffix, y.FilePathSuffix);
            }

            return
                true;
        }

        public static int GetHashCodeStatic(Suffix obj)
        {
            if (obj == null)
            {
                return
                    0;
            }

            unchecked
            {
                var hashCode = (obj.FilePathSuffix != null ? obj.FilePathSuffix.GetHashCode() : 0);

                //int hashCode = (obj.FileName != null ? obj.FileName.GetHashCode() : 0);
                //hashCode = (hashCode * 397) ^ (obj.FolderPath != null ? obj.FolderPath.GetHashCode() : 0);
                //hashCode = (hashCode * 397) ^ (obj.FilePath != null ? obj.FilePath.GetHashCode() : 0);
                //hashCode = (hashCode * 397) ^ (obj.FilePathSuffix != null ? obj.FilePathSuffix.GetHashCode() : 0);
                //hashCode = (hashCode * 397) ^ obj.ModificationTime.GetHashCode();
                //hashCode = (hashCode * 397) ^ obj.Exists.GetHashCode();

                return hashCode;
            }
        }
    }
}