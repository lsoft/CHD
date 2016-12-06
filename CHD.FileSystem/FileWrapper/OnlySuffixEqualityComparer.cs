using System.Collections.Generic;

namespace CHD.FileSystem.FileWrapper
{
    public class OnlySuffixEqualityComparer : IEqualityComparer<IFileWrapper>
    {
        public bool Equals(IFileWrapper x, IFileWrapper y)
        {
            if (x == null && y == null)
            {
                return
                    false;
            }

            if (x != null && y != null)
            {
                return
                    SuffixEqualityComparer.EqualsStatic(x.FilePathSuffix, y.FilePathSuffix);
            }

            return
                true;
        }

        public int GetHashCode(IFileWrapper obj)
        {
            if (obj == null)
            {
                return
                    0;
            }
            
            var hashCode = SuffixEqualityComparer.GetHashCodeStatic(obj.FilePathSuffix);

            return
                hashCode;
        }
    }
}