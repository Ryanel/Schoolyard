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

        private ushort internalCounter = 0;
        private byte timer = 0;

        // Timer Modulo
        public byte DIV
        {
            // DIV is the higher 8 bits of the internal counter
            get { return (byte)((internalCounter >> 8) & 0xFF); }
            set { internalCounter = 0; } 
        }

        // Timer Counter
        public int TIMA
        {
            get {
                return timer;
            }
            set {
                if(value > 0xFF)
                {
                    // Raise interrupt
                    timer = TMA; // Reset
                }
                else
                {
                    timer = (byte)value;
                }
            }
        }

        // Timer Modulo
        public byte TMA { get; set; }

        public byte TAC
        {
            get; set;
        }

        public bool TimerEnabled
        {
            get { return ((Read8(0xFF07) >> 2) & 1) == 1; }
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

        public int InternalCounter
        {
            get
            {
                return internalCounter;
            }

            set
            {
                int temp = internalCounter;
                internalCounter = (ushort)value;
                int temp2 = 0;

                int cycles = TACCycles;

                if(TimerEnabled)
                {
                    temp &= ~(TACCycles - 1);
                    temp2 = (internalCounter - temp) / cycles;
                    TIMA += (byte)temp2;
                }
            }
        }

        public Timer(Memory.MemoryController m) : base("timer", 0xFF04, 4)
        {
            mem = m;
        }

        public override void Write8(ushort address, byte val)
        {
            switch (address)
            {
                case 0xFF04:
                    DIV = 0;
                    break;
                case 0xFF05:
                    TIMA = 0; // TODO: Verify if this behavior is correct
                    break;
                case 0xFF06:
                    TMA = val;
                    break;
                case 0xFF07:
                    TAC = val;
                    break;
                default:
                    int translatedAddress = address - addressBase;
                    values[translatedAddress] = val;
                    break;
            }
        }

        public void Step(ulong cyclesDelta)
        {
            internalCounter += (ushort)cyclesDelta;
        }

        public override byte Read8(ushort address)
        {
            switch (address)
            {
                case 0xFF04:
                    return DIV;
                case 0xFF05:
                    return (byte)TIMA;
                case 0xFF06:
                    return TMA;
                case 0xFF07:
                    return TAC;
                default:
                    int translatedAddress = address - addressBase;
                    return values[translatedAddress];
            }
        }
    }
}
