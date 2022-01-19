using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicHue
{
    public static class Packets
    {
        internal const int QueryStatus1 = 0x81;
        internal const int QueryStatus2 = 0x8a;
        internal const int QueryStatus3 = 0x8b;
        internal const int ResponseLenQueryStatus = 14;
                 
        internal const int SetColor = 0x31;
        internal const int ResponseLenSetColor = 1;
        internal const int ChangeMode = 0x61;
        internal const int ResponseLenChangeMode = 1;
                 
        internal const int CustomMode = 0x51;
        internal const int ResponseLenCustomMode = 0;
                 
        internal const int CustomModeTerminator1 = 0xff;
        internal const int CustomModeTerminator2 = 0xf0;
                 
        internal const int TurnOn1 = 0x71;
        internal const int TurnOn2 = 0x23;
        internal const int TurnOn3 = 0x0f;
        internal const int TurnOff1 = 0x71;
        internal const int TurnOff2 = 0x24;
        internal const int TurnOff3 = 0x95;
        internal const int ResponseLenPower = 4;
                 
        internal const int True = 0x0f;
        internal const int False = 0xf0;
        internal const int On = 0x23;
        internal const int Off = 0x24;
    }
}
