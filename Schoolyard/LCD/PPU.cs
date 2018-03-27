using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.LCD
{
    public class PPU
    {
        public PPURegisters regs;
        public PPUCharacterRAM cram;
        public PPU()
        {
            regs = new PPURegisters("ppuregs", 0xFF40, 0x9);
            cram = new PPUCharacterRAM("cram", 0x8000, 0x1800);
        }

        public void Reset()
        {

        }

        public void Step(long cycles)
        {

        }
    }
}
