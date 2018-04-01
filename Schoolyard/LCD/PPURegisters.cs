using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.LCD
{
    public class PPURegisters : Memory.RAM
    {
        public enum LCDFlags : byte
        {
            Background = 1,
            Sprites = 2,
            SpritesSize = 4,
            BGTileMap = 8,
            TileData = 16,
            WindowOn = 32,
            WindowTileMap = 64,
            DisplayOn = 128,
        }

        public enum LCDStatusFlags : byte
        {
            Coincidence            = (1 << 2),
            HBlankInterrupt        = (1 << 3),
            VBlankInterrupt        = (1 << 4),
            OamInterrupt           = (1 << 5),
            CoincidenceInterrupt   = (1 << 6),
        }

        public byte LCDControl
        {
            get { return values[0x0]; }
            set { values[0] = value; }
        }

        public byte Status
        {
            get { return values[0x1]; }
            set { values[1] = value; }
        }

        public byte ScrollY
        {
            get { return values[0x2]; }
        }

        public byte ScrollX
        {
            get { return values[0x3]; }
        }

        public byte ScanLine
        {
            get { return values[0x4]; }
            set { values[4] = value; }
        }

        public byte ScanLineCompare
        {
            get { return values[0x5]; }
            set { values[5] = value; }
        }

        public byte[] bgPalette = new byte[4] { 0x0, 0x1, 0x2, 0x3 };
        public byte[] objPalette1 = new byte[4] { 0xFF, 0x1, 0x2, 0x3 };
        public byte[] objPalette2 = new byte[4] { 0XFF, 0x1, 0x2, 0x3 };

        public byte BackgroundPalette
        {
            get { return Read8(0xFF47); }
            set {
                values[7] = value;
                bgPalette[3] = PaletteToColor(value, 3);
                bgPalette[2] = PaletteToColor(value, 2);
                bgPalette[1] = PaletteToColor(value, 1);
                bgPalette[0] = PaletteToColor(value, 0);
            }
        }

        public byte Obj0Palette
        {
            get { return Read8(0xFF48); }
            set
            {
                values[8] = value;
                objPalette1[3] = PaletteToColor(value, 3);
                objPalette1[2] = PaletteToColor(value, 2);
                objPalette1[1] = PaletteToColor(value, 1);
                objPalette1[0] = 0xFF;
            }
        }

        public byte Obj1Palette
        {
            get { return Read8(0xFF49); }
            set
            {
                values[9] = value;
                objPalette2[3] = PaletteToColor(value, 3);
                objPalette2[2] = PaletteToColor(value, 2);
                objPalette2[1] = PaletteToColor(value, 1);
                objPalette2[0] = 0xFF;
            }
        }

        public byte LCDCStatusInterrupt
        {
            get { return values[0x4]; }
            set { values[4] = value; }
        }

        public byte WindowY
        {
            get { return values[0xA]; }
        }

        public byte WindowX
        {
            get { return values[0xB]; }
        }

        public bool LCDBackgroundEnabled { get { return (LCDControl & (byte)LCDFlags.Background) != 0; } }
        public bool LCDSpritesEnabled { get { return (LCDControl & (byte)LCDFlags.Sprites) != 0; } }
        public bool LCDSpriteSize { get { return (LCDControl & (byte)LCDFlags.SpritesSize) != 0; } }
        public bool LCDBGTileMap { get { return (LCDControl & (byte)LCDFlags.BGTileMap) != 0; } }
        public bool LCDAddressMode { get { return (LCDControl & (byte)LCDFlags.TileData) != 0; } }
        public bool LCDWindowOn { get { return (LCDControl & (byte)LCDFlags.WindowOn) != 0; } }
        public bool LCDWindowTileMap { get { return (LCDControl & (byte)LCDFlags.WindowTileMap) != 0; } }
        public bool LCDDisplayOn { get { return (LCDControl & (byte)LCDFlags.DisplayOn) != 0; } }

        public byte StatusMode
        {
            get { return (byte)(Read8(0xFF41) & 3); }
            set
            {
                byte status = Read8(0xFF41);
                status &= 0xFC; // Clear last two bits
                status |= value; // Set value
                values[1] = (byte)(value & 3);
            }
        }

        
        public PPURegisters(string name, ushort addressBase, ushort size) : base(name, addressBase,size) {}

        public override void Write8(ushort address, byte val)
        {
            int translatedAddress = address - addressBase;
            if (translatedAddress == 4) { return; } // Block writes to 0xFF44
            if (translatedAddress == 7) { BackgroundPalette = val; return; } // Capture writes to the background palette
            values[translatedAddress] = val;
        }

        private static byte PaletteToColor(byte palette, int paletteIndex)
        {
            return (byte)((palette >> (paletteIndex * 2)) & 3);
        }
    }
}
