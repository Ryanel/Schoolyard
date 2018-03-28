﻿using System;
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
        Gameboy gameboy;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            gameboy = new Gameboy();
            gameboy.Reset();

            gameboy.ppu.OnDisplayRendered += Ppu_RenderedFrame;
            gameboy.ppu.OnTileUpdate += Ppu_UpdatedTile;

            gameboy.loader.LoadROM("drmario.gb");
            gameboy.Start();
        }

        private void Ppu_RenderedFrame(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void Ppu_UpdatedTile(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 1000; i++)
            {
                gameboy.Step();
            }
        }

        Bitmap displayImage;

        void UpdateDisplay()
        {
            Schoolyard.LCD.PPU ppu = gameboy.ppu;
            if(displayImage == null)
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

        private void displayPicture_Paint(object sender, PaintEventArgs e)
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                gameboy.Reset();
                gameboy.loader.LoadROM(openFileDialog.FileName);
                gameboy.Start();
            }
        }
    }
}
