using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.Memory
{
    public class EchoRAM : Memory.MemoryDevice
    {
        public ushort mirrorBase;
        MemoryController controller;
        public EchoRAM(string name, ushort start, ushort size, ushort mirrorBase, MemoryController controller)
        {
            this.name = name;
            this.addressBase = start;
            this.size = size;
            this.mirrorBase = mirrorBase;
            this.controller = controller;
        }

        public override byte Read8(ushort address)
        {
            ushort translated = (ushort)(address - addressBase + mirrorBase);
            return controller.Read8(translated);
        }

        public override void Write8(ushort address, byte val)
        {
            ushort translated = (ushort)(address - addressBase + mirrorBase);
            controller.Write8(translated, val);
        }
    }
}
