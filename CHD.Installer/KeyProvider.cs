using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using CHD.Common;

namespace CHD.Installer
{
    public sealed class KeyProvider
    {
        private readonly IDisorderLogger _logger;

        public KeyProvider(
            IDisorderLogger logger
            )
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
            _logger = logger;
        }

        public bool TryParseKey(
            string encodeSeed,
            out byte[] key
            )
        {
            key = null;
            var result = false;

            try
            {
                key = ParseKey(encodeSeed);
                result = (key != null);
            }
            catch(Exception excp)
            {
                _logger.LogException(excp);
            }

            return
                result;
        }

        public byte[] ParseKey(
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
