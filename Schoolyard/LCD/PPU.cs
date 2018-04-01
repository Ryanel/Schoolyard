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
        public enum PPUMode {
            H_BLANK = 0,
            V_BLANK = 1,
            OAM_READ = 2,
            VRAM_READ = 3,
        }

        // Holds full image
        public byte[,] framebuffer = new byte[width, height];
        public byte[,,] tiles = new byte[512, 8, 8]; // Decoded tile data, in tileID, y, x format.
        public ulong framesRendered = 0;

        public PPUMode currentMode = PPUMode.H_BLANK;
        public ulong modeClock = 0;

        public PPURegisters regs;
        public PPUCharacterRAM cram;
        public RAM bgram;
        public RAM oam;
        private MemoryController mem;
        private Gameboy gameboy;

        // Constants
        public const int width = 160;
        public const int height = 144;
        public const ulong clocksHBlank = 204;
        public const ulong clocksVBlank = 456;
        public const ulong clocksOAMRead = 80;
        public const ulong clocksVRAMRead = 172;
        public const byte transparent = 0xFF; // Used with sprites

        // Events
        public event EventHandler OnTileUpdate;
        public event EventHandler OnDisplayRendered;
        public event EventHandler OnHBlank;

        // Cached variables
        private int currentLine;

        public PPU(Gameboy gb)
        {
            regs = new PPURegisters("ppuregs", 0xFF40, 0xC);
            cram = new PPUCharacterRAM("cram", 0x8000, 0x1800, this);
            bgram = new RAM("bg", 0x9800, 0x800);
            oam = new RAM("oam", 0xFE00, 0xA0);
            this.gameboy = gb;
            this.mem = gameboy.memory;
        }

        public void Reset() {
            currentMode = PPUMode.H_BLANK;
            modeClock = 0;

            framebuffer = new byte[width, height]; // Clear framebuffer
            tiles = new byte[384, 8, 8];
            framesRendered = 0;

            currentLine = 0;
        }

        public void Step(ulong cycles)
        {
            modeClock += cycles;
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
            regs.StatusMode = (byte)currentMode;
        }

        private void ModeHBlank()
        {
            if (modeClock >= clocksHBlank)
            {
                modeClock -= clocksHBlank;
                if (regs.ScanLine >= height)
                {
                    ToVBlank();
                    OnRenderComplete();
                    return;
                }
                else
                {
                    currentMode = PPUMode.OAM_READ;
                }
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
                    ToHBlank();
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
                ToHBlank();
                DrawScanline(); 
                regs.ScanLine++;
            }
        }

        private void ToVBlank()
        {
            gameboy.cpu.IssueInterrupt(CPU.Registers.InterruptFlags.VBlank);
            currentMode = PPUMode.V_BLANK;
        }

        private void ToHBlank()
        {
            if ((regs.Status & (byte)PPURegisters.LCDStatusFlags.HBlankInterrupt) != 0)
            {
                gameboy.cpu.IssueInterrupt(CPU.Registers.InterruptFlags.LCDStat);
            }
            currentMode = PPUMode.H_BLANK;

            if(OnHBlank != null)
            {
                OnHBlank.Invoke(this, null);
            }
        }

        public void DecodeTile(ushort address, byte value)
        {
            address &= 0x1ffe;

            ushort tile = (ushort)((address >> 4) & 511);
            ushort y = (ushort)((address >> 1) & 7);

            for (byte x = 0; x < 8; x++)
            {
                byte bitIndex = (byte)(1 << ( 7 - x));
                byte a = mem.Read8((ushort)(address + 0x8000));
                byte b = mem.Read8((ushort)(address + 0x8001));
                int lo = ((a & bitIndex) != 0) ? 0x1 : 0x0;
                int hi = ((b & bitIndex) != 0) ? 0x2 : 0x0; 
                tiles[tile, y, x] = (byte)(lo | hi);
                /*
                byte a = mem.Read8((ushort)(address + 0x8000));
                byte b = mem.Read8((ushort)(address + 0x8001));
                byte shift = (byte)((7 - x) & 7);
                byte pixel = (byte)(((b >> shift) & 1) << 1 | ((a >> shift) & 1));
                tiles[tile, y, x] = pixel;
                */
            }

            if (OnTileUpdate != null)
            {
                OnTileUpdate.Invoke(this, null);
            }
        }

        private void DrawScanline() {
            currentLine = regs.ScanLine;
            if (currentLine >= height) { // Should never hit this, but here to be safe
                return; 
            }

            // Draw scanline components
            if (regs.LCDBackgroundEnabled) {
                DrawBackgroundScanLine();
            }

            if (regs.LCDWindowOn)
            {
                DrawWindowScan();
            }

            if (regs.LCDSpritesEnabled) {
                DrawSpriteScanLine();
            }
        }

        private void DrawBackgroundScanLine() {
            // Cache properties we'll need
            byte scanLine = (byte)currentLine;
            byte scrollY = regs.ScrollY;

            bool signedTileIndex = !regs.LCDAddressMode;
            int tileDataAddress = !regs.LCDBGTileMap ? 0x8000 : 0x8800;
            int tileMapAddress = !regs.LCDBGTileMap ? 0x9800 : 0x9C00;
            byte screenX = regs.ScrollX;
            byte yPosition = (byte)(scanLine + scrollY);

            int tileRow = (yPosition / 8) * 32;

            for (byte x = 0; x < width; x++) {
                byte xPosition = (byte)(x + screenX);

                // Get tile address
                int tileColumn = (xPosition / 8) % 256;
                ushort tileAddress = (ushort)(tileMapAddress + tileRow + tileColumn);

                // Get tile index
                int tileIndex;
                if (signedTileIndex)
                {
                    tileIndex = (sbyte)bgram.Read8(tileAddress); // Read directly from cram for increased performance
                    if (tileIndex < 128)
                    {
                        tileIndex += 256;
                    }
                }
                else
                {
                    tileIndex = bgram.Read8(tileAddress);
                }

                // Write pixel to framebuffer
                byte pixel = tiles[tileIndex, yPosition % 8, xPosition % 8];
                framebuffer[x, scanLine] = regs.bgPalette[pixel];
            }
        }

        private void DrawWindowScan()
        {
            // Cache properties we'll need
            byte scanLine = (byte)currentLine;
            byte scrollY = regs.ScrollY;
            byte windowY = regs.WindowY;
            byte windowX = regs.WindowX;

            if(scanLine < windowY) {
                return;
            }

            bool signedTileIndex = !regs.LCDAddressMode;
            int tileDataAddress = !regs.LCDWindowTileMap ? 0x8000 : 0x8800;
            int tileMapAddress = !regs.LCDWindowTileMap ? 0x9800 : 0x9C00;
            byte screenX = regs.ScrollX;
            byte yPosition = (byte)(scanLine - windowY);

            int tileRow = (yPosition / 8) * 32;

            for (byte x = 0; x < width; x++)
            {
                byte xPosition = (byte)(x - windowX);

                // Get tile address
                int tileColumn = (xPosition / 8) % 256;
                ushort tileAddress = (ushort)(tileMapAddress + tileRow + tileColumn);

                // Get tile index
                int tileIndex;
                if (signedTileIndex)
                {
                    tileIndex = (sbyte)bgram.Read8(tileAddress); // Read directly from cram for increased performance
                    if (tileIndex < 128) {
                        tileIndex += 256;
                    }
                }
                else
                {
                    tileIndex = bgram.Read8(tileAddress);
                }

                // Write pixel to framebuffer
                byte pixel = tiles[tileIndex, yPosition % 8, xPosition % 8];
                framebuffer[x, scanLine] = regs.bgPalette[pixel];
            }
        }

        private void DrawSpriteScanLine()
        {
            byte[] scanline = new byte[width];
            int numSprites = 0;

            // Clear scanline
            for (int i = 0; i < width; i++) {
                scanline[i] = transparent; // Transparent
            }

            for (int i = 0; i < 40 && numSprites < 10; i++) {
                // Read this sprite's properties
                ushort objectAddress = (ushort)((i * 4) + 0xFE00);
                byte yPosition = (byte)(oam.Read8((ushort)(objectAddress + 0)) - 16);
                byte xPosition = (byte)(oam.Read8((ushort)(objectAddress + 1)) - 8);
                byte tileIndex = oam.Read8((ushort)(objectAddress + 2));
                byte flags = oam.Read8((ushort)(objectAddress + 3));
                byte height = regs.LCDSpriteSize ? (byte)16 : (byte)8;

                int top = yPosition;
                int bottom = top + height;

                if (top <= currentLine && bottom > currentLine) {
                    numSprites++;

                    // Object properties
                    bool palette1         = (flags & 0b00010000) != 0;
                    bool horizontalMirror = (flags & 0b00100000) != 0;
                    bool verticalMirror   = (flags & 0b01000000) != 0;
                    bool aboveBackground  = (flags & 0b10000000) != 0;

                    int vLine = currentLine - top;
                    if (verticalMirror) {
                        vLine -= height;
                        vLine *= -1;
                    }

                    byte[] objectPallete;
                    if (palette1) {
                        objectPallete = regs.objPalette0; 
                    }
                    else {
                        objectPallete = regs.objPalette0;
                    }

                    for (int x = 7; x >= 0; x--)
                    {
                        int tileX = x;
                        if(horizontalMirror) { tileX = 7 - x; }

                        byte color = tiles[tileIndex + (vLine < 8 ? 0:1), (vLine % 8), tileX % 8];
                        if (color == 0) { continue; }
                        int pos = xPosition + x;
                        // Output sprite
                        if (pos < width && pos >= 0) {
                  
                            if(scanline[pos] != transparent) { // Do we have to check for priority?
                                if(aboveBackground) { // If this sprite can be displayed above...
                                    scanline[pos] = objectPallete[color];
                                }
                            }
                            else {
                                scanline[pos] = objectPallete[color];
                            } 
                        }
                    }
                }
            }

            // Apply scanline
            for (int i = 0; i < width; i++) {
                if (scanline[i] == transparent) { continue; }
                framebuffer[i, currentLine] = scanline[i];
            }
        }

        private void OnRenderComplete()
        {
            framesRendered++;
            if(OnDisplayRendered != null) { OnDisplayRendered.Invoke(this, null); }
        }
    }
}
