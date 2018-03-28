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

        public override byte Read8(ushort address)
        {
            // Translated address is in the context of the memory device (0 base)
            int translatedAddress = address - addressBase;
            return values[translatedAddress];
        }

        public override void Write8(ushort address, byte val)
        {
            int translatedAddress = address - addressBase;
            values[translatedAddress] = val;
        }
    }
}
