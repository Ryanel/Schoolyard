using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.MBC
{
    public class MBC2 : Memory.MemoryDevice
    {
        public int romSize = 0x0;

        private int currentBank = 1;

        byte[] rom;
        byte[] ram;

        private int numRamBanks = 0;
        private bool ramEnabled = false;

        public const int romBankSize = 0x4000;
        public int ramBankSize = 0x512;

        public MBC2(string name, ushort addressBase, byte[] data, int size)
        {
            this.name = name;
            this.addressBase = addressBase;
            this.size = 0xC000;

            romSize = size;
            rom = data;

            ram = new byte[numRamBanks * ramBankSize];
        }


        public override byte Read8(ushort address)
        {
            if (address <= 0x3FFF) // 0x0000 -> 0x3FFF are always bank 0.
            { 
                return rom[address];
            }
            else if (address >= 0x4000 && address < 0x8000) // Bank n
            { 
                int bankTranslated = romBankSize * currentBank;
                int finalTranslated = bankTranslated + (address - romBankSize);
                return rom[finalTranslated];
            }
            else if (address >= 0xA000 && address <= 0xA1FF) // RAM (4bit)
            {
                if(ramEnabled)
                {
                    int addressTranslated = address - 0xA000;
                    return (byte)(ram[address] & 0x0F); // Only use the lower four bits
                }  
            }
            return 0xFF; // 0xFF is the default value of unmapped memory.
        }

        public override void Write8(ushort address, byte val)
        {
            if (address <= 0x1FFF) // Ram enable
            {
                if ((address & 0x0100) != 0) { // LSB of Upper byte is set  
                    ramEnabled = (val & 0xA) != 0;
                }
                
            }
            else if (address <= 0x3FFF) // Switch active ROM bank number
            {
                if((address & 0x0100) != 0) // LSB of Upper byte is set
                {
                    currentBank = val & 0xF;
                }
            }
            else if (address >= 0xA000 && address <= 0xA1FF) // RAM (4bit)
            {
                if (ramEnabled)
                {
                    int addressTranslated = address - 0xA000;
                    ram[address] = (byte)(val & 0xF);
                }
            }
        }
    }
}
