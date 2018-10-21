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
using SFML;
using SFML.Window;
namespace SchoolyardUI
{
    public partial class MainWindow : Form
    {
        private const float Ratio = 160 / 144;

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
            emulator.ForceRedraw();
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

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            emulator.Stop();
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            Height = (int)(Ratio * Width); // Maintain aspect ratio
            emulator.ForceRedraw();
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
