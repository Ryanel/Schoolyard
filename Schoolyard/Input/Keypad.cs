using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.Input
{
    public class Keypad : Memory.MemoryDevice
    {
        private byte ioPort = 0x00;

        public override ushort Read16(ushort address)
        {
            throw new NotImplementedException();
        }

        public override byte Read8(ushort address)
        {
            throw new NotImplementedException();
        }

        public override void Write16(ushort address, ushort val)
        {
            throw new NotImplementedException();
        }

        public override void Write8(ushort address, byte val)
        {
            throw new NotImplementedException();
        }
    }
}
