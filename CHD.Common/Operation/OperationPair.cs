using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Common.Operation
{
    public sealed class OperationPair
    {
        public IOperation Local
        {
            get;
            private set;
        }

        public IOperation Remote
        {
            get;
            private set;
        }

        public OperationPair(
            IOperation local, 
            IOperation remote
            )
        {
            if (local == null)
            {
                throw new ArgumentNullException("local");
            }
            if (remote == null)
            {
                throw new ArgumentNullException("remote");
            }
            Local = local;
            Remote = remote;
        }
    }
}
