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
        private int currentRamBank = 1;
        byte[] values;
        byte[] ram;
        public bool hasRam = false;
        private int numRamBanks = 1;
        private bool ramEnabled = false;
        private bool ramModeSelect = false;

        public const int romBankSize = 0x4000;
        public const int ramBankSize = 0x2000;

        public MBC1(string name, ushort addressBase, byte[] data, int size) {
            this.name = name;
            this.addressBase = addressBase;
            this.size = 0xBFFF;
            romSize = size;
            values = data;
            IdentifyInfo();

            ram = new byte[numRamBanks * ramBankSize];
        }

        private void IdentifyInfo()
        {

        }

        public override ushort Read16(ushort address) {
            byte lsb = Read8(address);
            byte msb = Read8((ushort)(address + 1));
            ushort result = (ushort)((ushort)(msb << 8) + lsb);
            return result;
        }

        public override byte Read8(ushort address) {
            if (address < 0x4000) { // Bank Zero 
                return values[address];
            }
            else if (address >= 0x4000 && address < 0x8000) { // Bank n
                int bankTranslated = romBankSize * currentBank;
                int finalTranslated = bankTranslated + (address - romBankSize);
                return values[finalTranslated];
            }
            else { // Ram
                if(ramEnabled) {
                    int ramAddress = (address - 0xA000) + (currentRamBank * ramBankSize);
                    return ram[ramAddress];
                }
                else {
                    return 0xFF; // 0xFF is the default value of unmapped memory.
                }
            }
        }

        // This is ROM, so don't handle writes at all.
        public override void Write16(ushort address, ushort val) {
            Write8(address, (byte)(val & 0x00FF));
            Write8((ushort)(address + 1), (byte)((val & 0xFF00) >> 8));
        }

        public override void Write8(ushort address, byte val) {
            if (address < 0x2000) { // Ram enable

                if ((val & 0xA) != 0)
                {
                    ramEnabled = true;
                }
                else
                {
                    ramEnabled = false;
                }
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
