using System;
using System.Collections.Generic;
using CHD.Common.Operation.FolderOperation;
using CHD.Common.Operation.Visitor;

namespace CHD.Common.Structure
{
    internal static class StructureHelper
    {
        public static List<OperationDivider.DividerPair<IFolderOperation>> GetSubFolders(
            this OperationDivider.DividerPair<IFolderOperation> currentPair,
            IEnumerable<OperationDivider.DividerPair<IFolderOperation>> pairs
            )
        {
            if (currentPair == null)
            {
                throw new ArgumentNullException("currentPair");
            }
            if (pairs == null)
            {
                throw new ArgumentNullException("pairs");
            }

            var result = new List<OperationDivider.DividerPair<IFolderOperation>>();

            foreach (var pair in pairs)
            {
                if (currentPair.Operation.Folder.IsByPathContains(pair.Operation.Folder))
                {
                    result.Add(pair);
                }
            }

            return result;
        }
    }
}