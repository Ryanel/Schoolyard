using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.LCD
{
    public class DMAController : Memory.MemoryDevice
    {
        public Gameboy gameboy;
        public byte sourceHi;
        public const ulong oamDMACyclesTotal = 671;
        public ulong cyclesTaken = 0;
        public bool isTransfering = false;

        public DMAController(Gameboy gameboy)
        {
            this.gameboy = gameboy;
            this.name = "dma";
            this.addressBase = 0xFF46;
            this.size = 0x1;
        }

        public override byte Read8(ushort address)
        {
            return 0;
        }

        public override void Write8(ushort address, byte val)
        {
            PerformOamDmaTransfer(val);
            gameboy.cpu.cycles += oamDMACyclesTotal;
        }

        private void PerformOamDmaTransfer(byte dma)
        {
            Console.WriteLine("Attempt to start DMA");
            byte[] oamData = new byte[0xA0];
            for (int i = 0; i < 0xA0; i++)
            {
                ushort address = (ushort)(dma * 0x100);
                gameboy.memory.Write8((ushort)(0xFE00+i), gameboy.memory.Read8(address));
            }
        }

        public void StopVRAMDMA()
        {

        }
    }
}
