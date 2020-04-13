using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Pong
{
    class Utilities
    {
        public static byte[] StringToByteArray(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        public static string ByteArrayToString(byte[] arr)
        {
            return Encoding.ASCII.GetString(arr);
        }
    }
}
