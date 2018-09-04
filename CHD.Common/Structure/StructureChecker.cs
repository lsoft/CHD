using System;
using System.Collections.Generic;
using System.Linq;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Others;

namespace CHD.Common.Structure
{
    public sealed class StructureChecker
    {
        private readonly IEnumerable<IStoredStructure> _structures;

        public int LastVersion
        {
            get
            {
                return
                    _structures.Max(j => j.Version);
            }
        }

        public StructureChecker(
            IEnumerable<IStoredStructure> structures
            )
        {
            if (structures == null)
            {
                throw new ArgumentNullException("structures");
            }

            this._structures = structures;
        }

        public bool IsFileExists(PathSequence filePathSequence)
        {
            foreach (var structure in _structures)
            {
                //этот файл есть в структуре?
                IFile foundFile;
                if (structure.RootFolder.GetFileByPath(filePathSequence, out foundFile))
                {
                    //файл есть в структуре
                    return true;
                }
            }

            //файла нет ни в одной структуре
            return
                false;
        }
    }
}