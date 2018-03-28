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
        public ulong bytesCopied = 0;

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
            if(isTransfering)
            {
                return;
            }
            sourceHi = dma;
            bytesCopied = 0;
            cyclesTaken = 0;
            isTransfering = true;
        }

        public void StopVRAMDMA()
        {

        }

        public void Step(ulong cycles)
        {
            if(!isTransfering)
            {
                return;
            }
            cyclesTaken += cycles;

            

            ushort source = (ushort)(sourceHi * 0x100);
            ulong bytesToHaveCopied = (cyclesTaken / 4) - 1;
            for (; bytesCopied < bytesToHaveCopied; bytesCopied++)
            {
                byte data = gameboy.memory.Read8((ushort)(source + bytesCopied));
                gameboy.memory.Write8((ushort)(0xFE00 + bytesCopied), data);
            }

            if(cyclesTaken >= oamDMACyclesTotal)
            {
                isTransfering = false;
            }
        }
    }
}
