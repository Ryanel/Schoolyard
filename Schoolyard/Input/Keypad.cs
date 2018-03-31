using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.Input
{
    public class Keypad : Memory.MemoryDevice
    {
        public int bank = 0;
        public byte[] values = new byte[2];

        // Bank 0
        public bool Start {
            set {
                Set(0, maskStart, value);
            }
        }
        public bool Select {
            set {
                Set(0, maskSelect, value);
            }
        }
        public bool A {
            set {
                Set(0, maskA, value);
            }
        }
        public bool B {
            set {
                Set(0, maskB, value);
            }
        }

        // Bank 1
        public bool Up {
            set {
                Set(1, maskUp, value);
            }
        }
        public bool Down {
            set {
                Set(1, maskDown, value);
            }
        }
        public bool Left {
            set {
                Set(1, maskLeft, value);
            }
        }
        public bool Right {
            set {
                Set(1, maskRight, value);
            }
        }

        // Bank 0
        private const byte maskA = 0x1;
        private const byte maskB = 0x2;
        private const byte maskSelect = 0x4;
        private const byte maskStart = 0x8;

        // Bank 1
        private const byte maskRight = 0x1;
        private const byte maskLeft = 0x2;
        private const byte maskUp = 0x4;
        private const byte maskDown = 0x8;

        public Keypad()
        {
            name = "keypad";
            addressBase = 0xFF00;
            size = 1;
        }

        public override byte Read8(ushort address)
        {
            return values[bank];
        }

        public override void Write8(ushort address, byte val)
        {
            if ((val & 0x10) != 0) {
                bank = 0;
            }
            else if ((val & 0x20) != 0) {
                bank = 1;
            }
        }

        public void Set(int bank, byte mask, bool pressed)
        {
            if(pressed)
            {
                values[bank] = (byte)(values[bank] | mask);
            }
            else
            {
                values[bank] = (byte)(values[bank] & (~mask));
            }
        }
    }
}
