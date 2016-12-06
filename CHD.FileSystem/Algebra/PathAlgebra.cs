using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHD.FileSystem.Algebra
{
    public class PathAlgebra
    {
        public static Suffix GetSuffix(
            string filePath,
            string folderPath
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            var di = new DirectoryInfo(folderPath);
            var fi = new FileInfo(filePath);

            var ddi = di.FullName;
            var ffi = fi.FullName;

            if (ffi.StartsWith(ddi))
            {
                var ddil = ddi.Length;

                if (ddil < ffi.Length)
                {
                    if (ffi[ddil] == Path.DirectorySeparatorChar || ffi[ddil] == Path.AltDirectorySeparatorChar)
                    {
                        ddil++;
                    }
                }

                var suffix = ffi.Substring(ddil);

                return
                    new Suffix(
                        suffix
                    );
            }

            throw new NotImplementedException();
        }
    }
}
