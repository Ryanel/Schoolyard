using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.Memory
{
    public class Reserved : MemoryDevice
    {
        public Reserved(string name, ushort addressBase, ushort size)
        {
            this.name = name;
            this.addressBase = addressBase;
            this.size = size;
        }

        public override ushort Read16(ushort address)
        {
            return 0xFFFF;
        }

        public override byte Read8(ushort address)
        {
            return 0xFF;
        }

        public override void Write16(ushort address, ushort val)
        {
        }

        public override void Write8(ushort address, byte val)
        {
        }
    }
}
