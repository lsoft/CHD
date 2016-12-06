using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Common
{
    public class Triple<T,U,V>
    {
        public T First
        {
            get;
            private set;
        }

        public U Second
        {
            get;
            private set;
        }

        public V Third
        {
            get;
            private set;
        }

        public Triple(T first, U second, V third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }
}
