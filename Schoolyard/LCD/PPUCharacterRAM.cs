using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.LCD
{
    public class PPUCharacterRAM : Memory.RAM
    {
        PPU ppu;
        public PPUCharacterRAM(string name, ushort addressBase, ushort size, PPU ppu) : base(name, addressBase,size)
        {
            this.ppu = ppu;
        }

        public override void Write8(ushort address, byte val)
        {
            base.Write8(address, val);
            ppu.DecodeTile(address, val);
        }
    }
}
