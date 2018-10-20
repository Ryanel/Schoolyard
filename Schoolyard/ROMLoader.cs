using System;
using System.IO;

namespace Schoolyard
{
    public class ROMLoader
    {
        private Gameboy gameboy;
        public ROMLoader(Gameboy gameboy)
        {
            this.gameboy = gameboy;
        }

        public bool LoadROM(string path)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(path);
                return LoadROM(fileBytes);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool LoadROM(byte[] rom)
        {
            int size = rom.Length;

            Console.WriteLine("ROM Size: " + size);

            byte cartType = rom[0x147];

            Memory.MemoryDevice mbc = null;

            switch (cartType)
            {
                // MBC1
                case 00:
                    Console.WriteLine("CART: No MBC, straight ROM");
                    mbc = new MBC.MBCNone("rom(none)", 0x0000, rom, size);
                    break;
                case 01:
                    Console.WriteLine("CART: MBC1");
                    mbc = new MBC.MBC1("rom(mbc1)", 0x0000, rom, size);
                    break;
                case 02:
                    Console.WriteLine("CART: MBC1+RAM");
                    mbc = new MBC.MBC1("rom(mbc1)", 0x0000, rom, size);
                    break;
                case 03:
                    Console.WriteLine("CART: MBC1+RAM+BATTERY");
                    mbc = new MBC.MBC1("rom(mbc1)", 0x0000, rom, size);
                    break;
                
                // MBC2
                case 05:
                    Console.WriteLine("CART: MBC2");
                    mbc = new MBC.MBC2("rom(mbc2)", 0x0000, rom, size);
                    break;

                case 06:
                    Console.WriteLine("CART: MBC2+BATTERY");
                    mbc = new MBC.MBC2("rom(mbc2)", 0x0000, rom, size);
                    break;

                default:
                    Console.WriteLine("Unknown Cart type (" + cartType + ")!");
                    return false;
            }

            gameboy.memory.Map(mbc, true);


            return true;
        }
    }
}
