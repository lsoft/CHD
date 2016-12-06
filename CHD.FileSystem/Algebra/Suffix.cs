using System;

namespace CHD.FileSystem.Algebra
{
    public class Suffix
    {
        public string FilePathSuffix
        {
            get;
            private set;
        }

        public int Key
        {
            get;
            private set;
        }

        public Suffix(
            string filePathSuffix
            )
        {
            if (filePathSuffix == null)
            {
                throw new ArgumentNullException("filePathSuffix");
            }

            FilePathSuffix = filePathSuffix;
            Key = filePathSuffix.ToUpper().GetHashCode();
        }

        public override string ToString()
        {
            return
                FilePathSuffix;
        }

        #region equality

        protected bool Equals(Suffix other)
        {
            return Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Suffix) obj);
        }

        public override int GetHashCode()
        {
            return Key;
        }

        #endregion
    }
}