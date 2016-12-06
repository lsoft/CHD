using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Common
{
    public class Base64Helper
    {
        public static MemoryStream EncodeToStream(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            var s = System.Convert.ToBase64String(data);

            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 65536, true))
            {
                writer.Write(s);
                writer.Flush();

                stream.Position = 0;
            }

            return
                stream;
        }
        
        public static string EncodeToString(string plainText)
        {
            if (plainText == null)
            {
                return null;
            }

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            return
                System.Convert.ToBase64String(plainTextBytes);
        }

        public static string DecodeFromString(string base64EncodedData)
        {
            if (base64EncodedData == null)
            {
                return
                    null;
            }

            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            
            return
                System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
