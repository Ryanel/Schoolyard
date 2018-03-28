using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.CPU
{
    public class Timer : Memory.RAM
    {
        private Memory.MemoryController mem;
        public const int BaseFreqency = 16384;
        private byte timer;

        public Timer(Memory.MemoryController m) : base("timer", 0xFF04, 4)
        {
            mem = m;
        }

        public override void Write8(ushort address, byte val)
        {
            if(address == 0xFF04)
            {
                //DIV = 0;
            }

            int translatedAddress = address - addressBase;
            values[translatedAddress] = val;
        }

        public void Step(ulong cyclesDelta)
        {
        }

        public override byte Read8(ushort address)
        {
            return base.Read8(address);
        }
    }
}
