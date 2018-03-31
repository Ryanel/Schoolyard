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
    public partial class MainWindow : Form
    {
        Debugger debugger;
        Gameboy gameboy;
        Bitmap displayImage;
        bool hasRenderedFrame = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            gameboy = new Gameboy();
            gameboy.Reset();

            gameboy.ppu.OnDisplayRendered += Ppu_RenderedFrame;
        }

        private void MainTimer(object sender, EventArgs e)
        {
            ulong cycles = 0;

            while(!hasRenderedFrame && gameboy.cpu.StateRunning)
            {
                cycles += gameboy.Step();
            }
            hasRenderedFrame = false;
        }

        private void Ppu_RenderedFrame(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        
        // Display
        void UpdateDisplay()
        {
            Schoolyard.LCD.PPU ppu = gameboy.ppu;
            hasRenderedFrame = true;
            if (displayImage == null)
            {
                displayImage = new Bitmap(160, 144);
            }

            Color white = Color.FromArgb(224, 248, 208);
            Color lightGray = Color.FromArgb(136, 192, 112);
            Color darkGray = Color.FromArgb(52, 104, 86);
            Color black = Color.FromArgb(8, 24, 32);

            for (int y = 0; y < 144; y++)
            {
                for (int x = 0; x < 160; x++)
                {
                    byte raw = gameboy.ppu.framebuffer[x, y];

                    switch (raw)
                    {
                        case 0x0:
                            displayImage.SetPixel(x, y, white);
                            break;
                        case 0x1:
                            displayImage.SetPixel(x, y, lightGray);
                            break;
                        case 0x2:
                            displayImage.SetPixel(x, y, darkGray);
                            break;
                        case 0x3:
                        default:
                            displayImage.SetPixel(x, y, black);
                            break;
                    }
                }
            }
            
            displayPicture.Image = displayImage;
        }

        private void DisplayPicture_Paint(object sender, PaintEventArgs e)
        {
            if (displayImage == null) { return; }
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(
               displayImage,
                new Rectangle(0, 0, displayPicture.Width, displayPicture.Height),
                // destination rectangle 
                0,
                0,           // upper-left corner of source rectangle
                displayImage.Width,       // width of source rectangle
                displayImage.Height,      // height of source rectangle
                GraphicsUnit.Pixel);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                gameboy.Reset();
                bool loaded = gameboy.loader.LoadROM(openFileDialog.FileName);

                if(loaded)
                {
                    gameboy.Start();
                }
            }
        }

        private void debuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(debugger == null)
            {
                debugger = new Debugger(gameboy);
            }
            debugger.Show();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                gameboy.keypad.Left = true;
            }

            if (e.KeyCode == Keys.Right)
            {
                gameboy.keypad.Right = true;
            }

            if (e.KeyCode == Keys.Up)
            {
                gameboy.keypad.Up = true;
            }

            if (e.KeyCode == Keys.Down)
            {
                gameboy.keypad.Down = true;
            }

            if (e.KeyCode == Keys.Enter)
            {
                gameboy.keypad.Start = true;
            }
            if (e.KeyCode == Keys.Back)
            {
                gameboy.keypad.Select = true;
            }
            if (e.KeyCode == Keys.Z)
            {
                gameboy.keypad.A = true;
            }
            if (e.KeyCode == Keys.X)
            {
                gameboy.keypad.B = true;
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                gameboy.keypad.Left = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                gameboy.keypad.Right = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                gameboy.keypad.Up = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                gameboy.keypad.Down = false;
            }
            if (e.KeyCode == Keys.Enter)
            {
                gameboy.keypad.Start = false;
            }
            if (e.KeyCode == Keys.Back)
            {
                gameboy.keypad.Select = false;
            }
            if (e.KeyCode == Keys.Z)
            {
                gameboy.keypad.A = false;
            }
            if (e.KeyCode == Keys.X)
            {
                gameboy.keypad.B = false;
            }
        }
    }
}
