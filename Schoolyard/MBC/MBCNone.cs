using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.MBC
{
    public class MBCNone : Memory.MemoryDevice
    {
        public int romSize = 0x0;
        byte[] values;

        public MBCNone(string name, ushort addressBase, byte[] data, int size)
        {
            this.name = name;
            this.addressBase = addressBase;
            this.size = 0x8000;
            romSize = size;
            values = data;
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
            return values[address - addressBase];
        }

        // This ROM has no banking, so don't handle writes at all.
        public override void Write16(ushort address, ushort val) { }
        public override void Write8(ushort address, byte val) { }
    }
}
