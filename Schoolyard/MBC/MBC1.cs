using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.MBC
{
    public class MBC1 : Memory.MemoryDevice
    {
        public int romSize = 0x0;
        private int currentBank = 1;
        byte[] values;

        public MBC1(string name, ushort addressBase, byte[] data, int size)
        {
            this.name = name;
            this.addressBase = addressBase;
            this.size = 0x8000;
            romSize = size;
            values = data;
        }

        public override ushort Read16(ushort address)
        {
            byte lsb = Read8(address);
            byte msb = Read8((ushort)(address + 1));
            ushort result = (ushort)((ushort)(msb << 8) + lsb);
            return result;
        }

        public override byte Read8(ushort address)
        {
            if (address < 0x4000) // Bank Zero
            {
                return values[address];
            }

            if (address >= 0x4000 && address < 0x8000) // Bank n
            {
                int bankSize = 0x4000; // 16k
                int bankTranslated = bankSize * currentBank;
                int finalTranslated = bankTranslated + (address - 0x4000);
                return values[finalTranslated];
            }
            return 0;
        }

        // This is ROM, so don't handle writes at all.
        public override void Write16(ushort address, ushort val) {
            Write8(address, (byte)(val & 0x00FF));
            Write8((ushort)(address + 1), (byte)((val & 0xFF00) >> 8));
        }
        public override void Write8(ushort address, byte val)
        {
            if (address < 0x1FFF) // Ram enable
            {
                Console.WriteLine("No ram support yet!");
            }

            if (address >= 0x2000 && address <= 0x3FFF) // ROM bank number
            {
                int bank = val;
                if (val == 0) { bank = 1; }
                // Bank bug
                if (val == 0x20) { bank = 0x21; }
                if (val == 0x40) { bank = 0x41; }
                if (val == 0x60) { bank = 0x61; }
                currentBank = bank;
            }

        }
    }
}
