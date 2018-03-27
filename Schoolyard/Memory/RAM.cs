using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.Memory
{
    public class RAM : MemoryDevice
    {
        protected byte[] values;

        public RAM(string name, ushort addressBase, ushort size)
        {
            this.name = name;
            this.addressBase = addressBase;
            this.size = size;
            values = new byte[size];
        }

        public override ushort Read16(ushort address)
        {
            byte lsb = Read8(address);
            byte msb = Read8((ushort)(address + 1));
            ushort result = (ushort)((ushort)(msb << 8) + lsb);
            return result;
        }

        public override byte Read8(ushort address)
        {
            // Translated address is in the context of the memory device (0 base)
            int translatedAddress = address - addressBase;
            return values[translatedAddress];
        }

        public override void Write16(ushort address, ushort val)
        {
            // Little endian, so we write the LSB first
            Write8(address, (byte)(val & 0x00FF));
            Write8((ushort)(address + 1), (byte)((val & 0xFF00) >> 8));
        }

        public override void Write8(ushort address, byte val)
        {
            int translatedAddress = address - addressBase;
            values[translatedAddress] = val;
        }
    }
}
