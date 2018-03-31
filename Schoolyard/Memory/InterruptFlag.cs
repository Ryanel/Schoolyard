using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.Memory
{
    public class InterruptFlag : MemoryDevice
    {
        byte value;
        private CPU.Registers regs;
        public InterruptFlag(string name, ushort addressBase, CPU.Registers regs)
        {
            this.name = name;
            this.addressBase = addressBase;
            this.size = 1;
            this.regs = regs;
        }

        public override byte Read8(ushort address)
        {
            return value;
        }

        public override void Write8(ushort address, byte val)
        {
            value = val;
            regs.interruptFlag = val;
        }
    }
}
