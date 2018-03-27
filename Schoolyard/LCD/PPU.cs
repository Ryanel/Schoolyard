using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schoolyard.Memory;
namespace Schoolyard.LCD
{
    public class PPU
    {
        public enum PPUMode
        {
            H_BLANK = 0,
            V_BLANK = 1,
            OAM_READ = 2,
            VRAM_READ = 3,
        }

        // Holds full image
        public byte[,] framebuffer = new byte[width, height];
        public byte[,,] tiles; // Decoded tile data, in tileID, y, x format.
        public int framesRendered = 0;

        public PPUMode currentMode = PPUMode.H_BLANK;
        public ulong modeClock = 0;

        public PPURegisters regs;
        public PPUCharacterRAM cram;
        public MemoryController mem;

        // Constants
        public const int width = 160;
        public const int height = 144;
        public const ulong clocksHBlank = 204;
        public const ulong clocksVBlank = 456;
        public const ulong clocksOAMRead = 80;
        public const ulong clocksVRAMRead = 172;

        // Events
        public event EventHandler OnTileUpdate;
        public event EventHandler OnDisplayRendered;

        public PPU(MemoryController mem)
        {
            regs = new PPURegisters("ppuregs", 0xFF40, 0x9);
            cram = new PPUCharacterRAM("cram", 0x8000, 0x1800, this);
            this.mem = mem;
        }

        public void Reset() {
            currentMode = PPUMode.H_BLANK;
            modeClock = 0;

            framebuffer = new byte[width, height]; // Clear framebuffer
            framesRendered = 0;
        }

        public void Step(ulong cycles)
        {
            // Advance PPU by x cycles
            switch (currentMode)
            {
                case PPUMode.H_BLANK:
                    ModeHBlank();
                    break;
                case PPUMode.V_BLANK:
                    ModeVBlank();
                    break;
                case PPUMode.OAM_READ:
                    ModeOAMRead();
                    break;
                case PPUMode.VRAM_READ:
                    ModeVRAMRead();
                    break;
            }
        }

        private void ModeHBlank()
        {
            if (modeClock >= clocksHBlank)
            {
                modeClock -= clocksHBlank;
                regs.ScanLine++;
                if (regs.ScanLine >= height - 1)
                {
                    byte flags = mem.Read8(0xFF0F);
                    byte enable = mem.Read8(0xFFFF);

                    if ((enable & 0x1) != 0)
                    {
                        flags |= 0x1;
                        mem.Write8(0xFF0F, flags);
                    }

                    currentMode = PPUMode.V_BLANK;
                    OnRenderComplete();
                    return;
                }

                currentMode = PPUMode.OAM_READ;
            }
        }

        private void ModeVBlank()
        {
            if (modeClock >= clocksVBlank)
            {
                modeClock -= clocksVBlank;
                regs.ScanLine++;
                if (regs.ScanLine >= 153)
                {
                    regs.ScanLine = 0;
                    currentMode = PPUMode.H_BLANK;
                }
            }
        }

        private void ModeOAMRead()
        {
            if (modeClock >= clocksOAMRead)
            {
                modeClock -= clocksOAMRead;
                currentMode = PPUMode.VRAM_READ;
            }
        }

        private void ModeVRAMRead()
        {
            if (modeClock >= clocksVRAMRead)
            {
                modeClock -= clocksVRAMRead;
                currentMode = PPUMode.H_BLANK;
                DrawScanline();
            }
        }

        public void DecodeTile(ushort address, byte value)
        {
            address &= 0x1ffe;

            ushort tile = (ushort)((address >> 4) & 511);
            ushort y = (ushort)((address >> 1) & 7);

            
            for (byte x = 0; x < 8; x++)
            {
                byte bitIndex = (byte)(1 << (7 - x));
                byte a = mem.Read8((ushort)(address + 0x8000));
                byte b = mem.Read8((ushort)(address + 0x8001));
                tiles[tile, y, x] = (byte)((((byte)(a & bitIndex) > 0) ? 1 : 0 + (((byte)(b & bitIndex) > 0) ? 2 : 0)));
            }

            if (OnTileUpdate != null)
            {
                OnTileUpdate.Invoke(this, null);
            }
        }

        private void DrawScanline()
        {
            if(regs.ScanLine >= height) { // Should never hit this, but here to be safe
                return; 
            }

            // Draw scanline components
            if (regs.LCDBackgroundEnabled) {
                DrawBackgroundScanLine();
            }

            if (regs.LCDSpritesEnabled) {
                DrawSpriteScanLine();
            }
        }

        private void DrawBackgroundScanLine()
        {

        }

        private void DrawSpriteScanLine()
        {

        }

        private void OnRenderComplete()
        {
            if(OnDisplayRendered != null) { OnDisplayRendered.Invoke(this, null); }
        }
    }
}
