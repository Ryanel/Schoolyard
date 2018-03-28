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
        public byte[,,] tiles = new byte[384, 8, 8]; // Decoded tile data, in tileID, y, x format.
        public ulong framesRendered = 0;

        public PPUMode currentMode = PPUMode.H_BLANK;
        public ulong modeClock = 0;

        public PPURegisters regs;
        public PPUCharacterRAM cram;
        public RAM bgram;
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
        public event EventHandler OnHBlank;
        public PPU(MemoryController mem)
        {
            regs = new PPURegisters("ppuregs", 0xFF40, 0xB);
            cram = new PPUCharacterRAM("cram", 0x8000, 0x1800, this);
            bgram = new RAM("bg", 0x9800, 0x800);
            this.mem = mem;
        }

        public void Reset() {
            currentMode = PPUMode.H_BLANK;
            modeClock = 0;

            framebuffer = new byte[width, height]; // Clear framebuffer
            tiles = new byte[384, 8, 8];
            framesRendered = 0;
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
            byte flags = mem.Read8(0xFF0F);
            byte enable = mem.Read8(0xFFFF);

            if ((enable & (byte)CPU.Registers.InterruptFlags.VBlank) != 0)
            {
                flags |= (byte)CPU.Registers.InterruptFlags.VBlank;
                mem.Write8(0xFF0F, flags);
            }

            currentMode = PPUMode.V_BLANK;
        }

        private void ToHBlank()
        {
            if ((regs.Status & (byte)PPURegisters.LCDStatusFlags.HBlankInterrupt) != 0)
            {
                byte flags = mem.Read8(0xFF0F);
                flags |= (byte)CPU.Registers.InterruptFlags.LCDStat;
                mem.Write8(0xFF0F, flags);
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
            if(regs.ScanLine >= height) { // Should never hit this, but here to be safe
                return; 
            }

            // Draw scanline components
            if (regs.LCDBackgroundEnabled) {
                DrawBackgroundScanLine();
            }

            if (regs.LCDSpritesEnabled || true) {
                DrawSpriteScanLine();
            }
        }

        private void DrawBackgroundScanLine() {
            // Cache properties we'll need
            byte scanLine = regs.ScanLine;
            byte scrollY = regs.ScrollY;
            byte windowY = regs.WindowY;
            byte windowX = 0x00; // Load this only if we use the window

            bool signedTileIndex = !regs.LCDAddressMode;
            int tileDataAddress = !regs.LCDWindowTileMap ? 0x8000 : 0x8800;
            int tileMapAddress = !regs.LCDTileMap ? 0x9800 : 0x9C00;
            int screenX = regs.ScrollX & 7;
            int yPosition = (scanLine + scrollY) % 256;
            bool window = false;

            if (regs.LCDWindowOn && windowY > scrollY) // Determine if window is on
            {
                window = true;
                tileMapAddress = regs.LCDWindowTileMap ? 0x9800 : 0x9C00;
                yPosition = (scrollY - windowY) % 256;
                windowX = regs.WindowX;
            }

            int tileRow = (yPosition / 8) * 32;

            for (int x = 0; x < 160; x++) {
                int xPosition = (x + screenX) % 256;

                if (window) {
                    xPosition = x - windowX;
                }

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

                // Decode here

                

                // Write pixel to framebuffer
                byte pixel = tiles[tileIndex, yPosition % 8, xPosition % 8];

                framebuffer[x, scanLine] = regs.bgPalette[pixel];
            }
        }

        private void DrawSpriteScanLine()
        {
            int currentLine = regs.ScanLine;
            
            if (currentLine >= 144)
            {
                return;
            }

            const byte transparent = 0xFF;
            byte[] scanline = new byte[160];
            int numSprites = 0;

            // Clear scanline
            for (int i = 0; i < 160; i++)
            {
                scanline[i] = transparent; // Transparent
            }

            for (int i = 0; i < 40 && numSprites < 10; i++)
            {
                // Read this sprite's properties
                ushort objectAddress = (ushort)((i * 4) + 0xFE00);
                byte yPosition = (byte)(mem.Read8((ushort)(objectAddress + 0)) - 0);
                byte xPosition = (byte)(mem.Read8((ushort)(objectAddress + 1)) - 8);
                byte tileIndex = (byte)(mem.Read8((ushort)(objectAddress + 2)));
                byte flags = (byte)(mem.Read8((ushort)(objectAddress + 3)));
                byte height = regs.LCDSpriteSize ? (byte)16 : (byte)8;

                int top = yPosition - 16;
                int bottom = top + height;

                if (top <= currentLine && bottom > currentLine)
                {
                    numSprites++;

                    bool verticalMirror = false; 
                    bool horizontalMirror = false;
                    bool aboveBackground = true;

                    int vLine = currentLine - top;
                    if (verticalMirror)
                    {
                        vLine -= height;
                        vLine *= -1;
                    }
                    
                    byte[] objectPallete = regs.objPalette1; // TODO: Switch palettes

                    for (int x = 7; x >= 0; x--)
                    {
                        if (false) // horizontalMirror
                        {
                            x -= 8;
                            x *= -1;
                        }
                        
                        int currentBit = 7 - x;
                        byte color = tiles[tileIndex, (vLine % 8), x % 8];

                        // Calculate sprite priority
                        if (color == transparent)
                        {
                            continue;
                        }

                        if (xPosition + x < 160 && xPosition + x > 0)
                        {
                            scanline[xPosition + x] = objectPallete[color];
                        }
                    }

                }
            }
            if(numSprites > 0)
            {
                //Console.WriteLine("Outputting " + numSprites + " sprites");
            }
            // Apply scanline
            for (int i = 0; i < 160; i++)
            {
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
