using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicHue
{
    public static class Extensions
    {
        public static byte[] GetBytes(this int[] data)
        {
            return data.Select(x => Convert.ToByte(x)).ToArray();
        }
    }
}
