using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Schoolyard;
namespace SchoolyardUI
{
    public partial class Debugger : Form
    {
        private Gameboy target;

        private ulong lastInstructions = 0;
        private ulong lastFrame = 0;
        private Bitmap tileImage;
        private bool tileImageDirty = true;
        public Debugger(Gameboy target)
        {
            InitializeComponent();
            this.target = target;
            target.ppu.OnTileUpdate += PPUTileRendered;
        }

        private void Debugger_Load(object sender, EventArgs e)
        {
            Size toResize = Size;
            toResize.Width = 173;
            Size = toResize;
        }

        private void DebugTimer_Tick(object sender, EventArgs e)
        {
            // CPU
            labelAF.Text = String.Format("AF: {0:X4}", target.cpu.AF);
            labelBC.Text = String.Format("BC: {0:X4}", target.cpu.BC);
            labelDE.Text = String.Format("DE: {0:X4}", target.cpu.DE);
            labelHL.Text = String.Format("HL: {0:X4}", target.cpu.HL);
            labelSP.Text = String.Format("SP: {0:X4}", target.cpu.SP);
            labelPC.Text = String.Format("PC: {0:X4}", target.cpu.PC);

            zFlag.Checked = target.cpu.FlagZero;
            nFlag.Checked = target.cpu.FlagNegative;
            hFlag.Checked = target.cpu.FlagHalfCarry;
            cFlag.Checked = target.cpu.FlagCarry;

            if(tileImageDirty)
            {
                UpdatePPUTiles();
            }
        }

        private void PerSecondTimer_Tick(object sender, EventArgs e)
        {
            ulong instructionsDelta = target.cpu.instructionsExecuted - lastInstructions;
            lastInstructions = target.cpu.instructionsExecuted;

            ulong frameDelta = target.ppu.framesRendered - lastFrame;
            lastFrame = target.ppu.framesRendered;

            ipsLabel.Text = "IPS: " + instructionsDelta;
            fpsLabel.Text = "FPS: " + frameDelta;
        }

        private void Debugger_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void PPUTileRendered(object sender, EventArgs e)
        {
            tileImageDirty = true;
        }

        private void UpdatePPUTiles()
        {
            if(tileImage == null)
            {
                tileImage = new Bitmap(128, 128);
            }
            
            int currentTile = 0;

            Color white = Color.White; // 0x0
            Color lightGray = Color.FromArgb(192, 192, 192); // 0x1
            Color darkGray = Color.FromArgb(96, 96, 96); // 0x2
            Color black = Color.Black; // 0x3

            byte[] palette = target.ppu.regs.bgPalette;

            for (int ty = 0; ty < 16; ty++)
            {
                for (int tx = 0; tx < 16; tx++)
                {
                    if (currentTile > 384)
                    {
                        break;
                    }
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            int mapX = (tx * 8) + x;
                            int mapY = (ty * 8) + y;
                            byte color = palette[target.ppu.tiles[currentTile, y, x]];
                            switch (color)
                            {
                                case 0x0:
                                    tileImage.SetPixel(mapX, mapY, white);
                                    break;
                                case 0x1:
                                    tileImage.SetPixel(mapX, mapY, lightGray);
                                    break;
                                case 0x2:
                                    tileImage.SetPixel(mapX, mapY, darkGray);
                                    break;
                                case 0x3:
                                default:
                                    tileImage.SetPixel(mapX, mapY, black);
                                    break;
                            }
                        }
                    }
                    currentTile++;
                }
            }
            ppuTiles.Image = tileImage;
        }

        private void ppuTiles_Paint(object sender, PaintEventArgs e)
        {
            if (tileImage == null) { return; }
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(
               tileImage,
                new Rectangle(0, 0, ppuTiles.Width, ppuTiles.Height),
                // destination rectangle 
                0,
                0,           // upper-left corner of source rectangle
                tileImage.Width,       // width of source rectangle
                tileImage.Height,      // height of source rectangle
                GraphicsUnit.Pixel);
        }
    }
}
