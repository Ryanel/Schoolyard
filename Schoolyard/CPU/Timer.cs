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
        private ulong currentCycles = 0;
        private ulong internalCounter = 0;
        private ulong internalCounterDiv = 0;

        // Timer Modulo
        public byte DIV
        {
            get { return Read8(0xFF04); }
            set { values[0] = value; } 
        }

        // Timer Counter
        public byte TIMA
        {
            get { return Read8(0xFF05); }
            set { Write8(0xFF05, value); }
        }

        // Timer Modulo
        public byte TMA {
            get { return Read8(0xFF06); }
            set { Write8(0xFF06, value); }
        }

        public byte TAC
        {
            get { return Read8(0xFF07); }
            set { Write8(0xFF07, value); }
        }

        public int TACCycles
        {
            get
            {
                switch (TAC & 0b11)
                {
                    default:
                    case 0x0:
                        return 1024;
                    case 0x1:
                        return 16;
                    case 0x2:
                        return 64;
                    case 0x3:
                        return 256;
                }
            }
        }

        public Timer(Memory.MemoryController m) : base("timer", 0xFF04, 4)
        {
            mem = m;
        }

        public override void Write8(ushort address, byte val)
        {
            if(address == 0xFF04)
            {
                val = 0;
            }

            int translatedAddress = address - addressBase;
            values[translatedAddress] = val;
        }

        public void Step(ulong cyclesDelta)
        {
            int tima = TIMA;
            internalCounter += cyclesDelta;

            // Increment tima according to TACC Cycles

            if (tima > 0xFF)
            {
                tima = TMA;
                // Request interrupt here
            }

            // Increment DIV now
            internalCounterDiv += cyclesDelta;
            if (internalCounterDiv > 16)
            {
                internalCounterDiv -= 16;
                DIV++;
            }
        }

        public override byte Read8(ushort address)
        {
            return base.Read8(address);
        }
    }
}
