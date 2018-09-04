using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Common.Structure.Cleaner
{
    public interface IStructureCleaner
    {
        IEnumerable<IStoredStructure> FilterToCleanup(
            IReadOnlyList<IStoredStructure> structures
            );
    }
}
