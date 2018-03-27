using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.Utilities
{
    public class ByteUtilities
    {
        public static void Unpack16(ushort value, out byte lsb, out byte msb)
        {
            lsb = (byte)((ushort)(value >> 8));
            msb = (byte)value;
        }

        public static ushort Pack16(byte lsb, byte msb)
        {
            return (ushort)((lsb << 8) + msb);
        }

        public static string HexString(int value, bool forceShort = false)
        {
            if (value >= 0x100 && value <= 0xFFFF || forceShort)
            {
                return value.ToString("X4");
            }
            else if (value <= 0xFF)
            {
                return value.ToString("X2");
            }
            else
            {
                return value.ToString("X8");
            }
        }
    }
}
