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
        private int currentRamBank = 0;
        byte[] rom;
        byte[] ram;
        public bool hasRam = false;
        private int numRamBanks = 0;
        private bool ramEnabled = false;
        private bool ramModeSelect = false;

        public const int romBankSize = 0x4000;
        public int ramBankSize = 0x2000;

        public MBC1(string name, ushort addressBase, byte[] data, int size)
        {
            this.name = name;
            this.addressBase = addressBase;
            this.size = 0xC000;

            romSize = size;
            rom = data;

            IdentifyInfo();

            ram = new byte[numRamBanks * ramBankSize];
        }

        private void IdentifyInfo()
        {
            byte ramSize = rom[0x149];

            switch (ramSize)
            {
                case 0:
                    numRamBanks = 0;
                    hasRam = false;
                    break;

                case 2:
                    numRamBanks = 1;
                    ramBankSize = 0x2000;
                    hasRam = true;
                    break;

                case 3:
                    numRamBanks = 4;
                    ramBankSize = 0x2000;
                    hasRam = true;
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public override byte Read8(ushort address)
        {
            
            if (address < 0x4000) // 0x0000 -> 0x3FFF are always bank 0.
            { 
                return rom[address];
            }
            else if (address >= 0x4000 && address < 0x8000) // Bank n
            { 
                int bankTranslated = romBankSize * currentBank;
                int finalTranslated = bankTranslated + (address - romBankSize);
                return rom[finalTranslated];
            }
            else // We are accessing RAM now.
            { 
                if(ramEnabled)
                {
                    int ramAddress = (address - 0xA000) + (currentRamBank * ramBankSize);
                    return ram[ramAddress];
                }
                else {
                    return 0xFF; // 0xFF is the default value of unmapped memory.
                }
            }
        }

        public override void Write8(ushort address, byte val)
        {
            if (address < 0x2000) // Ram enable
            {
                ramEnabled = (val & 0xA) != 0;
            }
            else if (address <= 0x3FFF) // Switch active ROM bank number
            {
                int bank = 0;

                // Simulate Bank bug
                if (val == 0) { bank = 1; }
                else if (val == 0x20) { bank = 0x21; }
                else if (val == 0x40) { bank = 0x41; }
                else if (val == 0x60) { bank = 0x61; }
                else { bank = val; }
                currentBank = bank;
            }
            else if (address <= 0x5FFF)  // Set RAM bank Number, or upper two bits of ROM bank number.
            {
                currentRamBank = val & 0b0000_0011;
            }
            else if (address <= 0x7FFF)  // ROM/RAM Mode Select
            {
                throw new NotImplementedException("No ROM/RAM mode select yet");
            }
            else if (address >= 0xA000 && address <= 0xBFFF) // Write to RAM
            {
                int ramAddress = (address - 0xA000) + (currentRamBank * ramBankSize);
                ram[ramAddress] = val;
            }
        }
    }
}
