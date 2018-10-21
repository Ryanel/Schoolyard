using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Schoolyard;
using SFML;
using SFML.Window;
namespace SchoolyardUI
{
    public partial class MainWindow : Form
    {
        const double widthRatio = 160;
        const double heightRatio = 144 + 16;
        private Debugger debugger;
       // private Gameboy gameboy;

        private Emulator emulator;

        // Graphics Surface
        private SFML.Graphics.RenderWindow sfmlWindow;
        private DrawingSurface renderSurface;

        // Emulation state
        private string loadedRom = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // On load...
        private void MainWindow_Load(object sender, EventArgs e)
        {
            InitGraphics();
            emulator = new Emulator(sfmlWindow);

            sfmlThread.RunWorkerAsync();
            emulator.OnWindowResize();
        }

        public void InitGraphics()
        {
            // Create render surface
            renderSurface = new DrawingSurface();
            renderSurface.Dock = DockStyle.Fill;
            Controls.Add(renderSurface);
            renderSurface.BringToFront();
            renderSurface.BeingDrawnTo = true;

            // Create SFML window, launch thread
            sfmlWindow = new SFML.Graphics.RenderWindow(renderSurface.Handle);
            sfmlWindow.SetActive(false);
            
        }

        private void TryLoadFile(string path)
        {
            emulator.Stop();
            bool loaded = emulator.LoadROM(path);

            if (loaded)
            {
                loadedRom = path;
                Text = "Schoolyard - " + loadedRom;
                Start();
            }
            else
            {
                MessageBox.Show("Failed to Load ROM: Rom is unsupported.", "Load ROM", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void Start()
        {
            emulator.Start();
            if (!emulationThread.IsBusy)
            {
                emulationThread.RunWorkerAsync();
            }
        }

        private void Stop()
        {
            emulator.Stop();
        }

        /* ================================
         * Emulation thread 
         =================================*/
        private void EmulationThread_DoWork(object sender, DoWorkEventArgs e)
        {
            emulator.EmulationThread();
        }

        /* ================================
         * Display / Input / Audio thread 
         =================================*/
        private void SMFLDisplayThread_DoWork(object sender, DoWorkEventArgs e)
        {
            emulator.DisplayThread();
        }

        /* ================================
         * Winforms plumbing
         =================================*/

        const int WM_SIZING = 0x214;
        const int WMSZ_LEFT = 1;
        const int WMSZ_RIGHT = 2;
        const int WMSZ_TOP = 3;
        const int WMSZ_BOTTOM = 6;

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            emulator.Stop();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SIZING) // Resize window while keeping aspect ratio
            {
                RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
                int res = m.WParam.ToInt32();
                if (res == WMSZ_LEFT || res == WMSZ_RIGHT)
                {
                    rc.Bottom = rc.Top + (int)(heightRatio * Width / widthRatio);
                }
                else if (res == WMSZ_TOP || res == WMSZ_BOTTOM)
                {
                    rc.Right = rc.Left + (int)(widthRatio * Height / heightRatio);
                }
                else if (res == WMSZ_RIGHT + WMSZ_BOTTOM)
                {
                    rc.Bottom = rc.Top + (int)(heightRatio * Width / widthRatio);
                }
                else if (res == WMSZ_LEFT + WMSZ_TOP)
                {
                    rc.Left = rc.Right - (int)(widthRatio * Height / heightRatio);
                }
                Marshal.StructureToPtr(rc, m.LParam, true);

                emulator.OnWindowResize();
            }

            base.WndProc(ref m);
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            emulator.paused = !emulator.paused;
            pauseToolStripMenuItem.Checked = emulator.paused;
        }

        private void rebootGameboyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryLoadFile(loadedRom);
        }

        private void debuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (debugger == null)
            {
                debugger = new Debugger(emulator.GetGameboy());
            }
            debugger.Show();
        }

        private void OpenFile()
        {
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                TryLoadFile(openFileDialog.FileName);
            }
        }
    }
}
