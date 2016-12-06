using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Installer
{
    public class KeyProvider
    {
        public static byte[] ProvideKey(
            string encodeSeed
            )
        {
            //encodeSeed allowed to be null

            if (string.IsNullOrEmpty(encodeSeed))
            {
                return
                    null;
            }

            var seed = int.Parse(encodeSeed);

            var key = new byte[32];

            new Random(seed).NextBytes(key);

            return
                key;
        }

    }
}
