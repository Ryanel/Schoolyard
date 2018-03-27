using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.LCD
{
    public class PPU
    {
        public PPURegisters regs;
        public PPUCharacterRAM cram;

        public const int width = 160;
        public const int height = 144;

        // Holds full image
        public byte[,] framebuffer = new byte[width, height];
        public int framesRendered = 0;

        public ulong modeClock = 0;

        public PPU()
        {
            regs = new PPURegisters("ppuregs", 0xFF40, 0x9);
            cram = new PPUCharacterRAM("cram", 0x8000, 0x1800);
        }

        public void Reset() {
            modeClock = 0;
            framebuffer = new byte[width, height]; // Clear framebuffer
            framesRendered = 0;
        }

        public void Step(long cycles)
        {
            // Advance PPU by x cycles
        }

        public void DrawScanline()
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

        public void DrawBackgroundScanLine()
        {

        }

        public void DrawSpriteScanLine()
        {

        }
    }
}
