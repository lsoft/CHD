using System;
using System.Collections.Generic;
using System.Linq;
//using CHD.Common.KeyValueContainer.SyncedVersion;

namespace CHD.Common.Structure.Cleaner
{
    public sealed class LocalStructureCleaner : IStructureCleaner
    {
        public LocalStructureCleaner(
            )
        {
        }

        public IEnumerable<IStoredStructure> FilterToCleanup(IReadOnlyList<IStoredStructure> structures)
        {
            if (structures == null)
            {
                throw new ArgumentNullException("structures");
            }

            var toDelete = structures.OrderByDescending(j => j.Version).Skip(1);

            return
                toDelete;
        }
    }
}