using System;
using System.Collections.Generic;
using System.Linq;

namespace CHD.Common.Structure.Cleaner
{
    public sealed class RemoteStructureCleaner : IStructureCleaner
    {
        private readonly int _storedStructureCount;

        public RemoteStructureCleaner(
            int storedStructureCount
            )
        {
            _storedStructureCount = storedStructureCount;
        }

        public IEnumerable<IStoredStructure> FilterToCleanup(IReadOnlyList<IStoredStructure> structures)
        {
            if (structures == null)
            {
                throw new ArgumentNullException("structures");
            }

            //var list = structures.ToList();

            //if (list.Count < _storedStructureCount)
            //{
            //    return
            //        new List<IStoredStructure>();
            //}

            var toDelete = structures.OrderByDescending(j => j.Version).Skip(_storedStructureCount);

            return
                toDelete;
        }
    }
}