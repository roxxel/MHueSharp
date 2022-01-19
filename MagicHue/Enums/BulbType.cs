using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicHue.Enums
{
    public enum BulbType : byte
    {
        RGBWW = 0x44,
        TAPE = 0x33,
        RGBWWCW = 0x35
    }
}
