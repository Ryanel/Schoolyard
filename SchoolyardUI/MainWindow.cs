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
        private Gameboy gameboy;

        private bool hasRenderedFrame = false;
        private bool sfmlHasDrawnFrame = false;
        private bool hasExited = false;

        private SFML.Graphics.RenderWindow sfmlWindow;
        private DrawingSurface renderSurface;

        // Emulation state
        private bool isPaused = false;
        private string loadedRom = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // On load...
        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Init gameboy
            InitGameboy();
            InitGraphics();
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
            sfmlThread.RunWorkerAsync();
        }

        public void InitGameboy()
        {
            gameboy = new Gameboy();
            gameboy.Reset();
            gameboy.ppu.OnDisplayRendered += OnRenderFrame;
        }

        private void TryLoadFile(string path)
        {
            gameboy.Reset();
            bool loaded = gameboy.loader.LoadROM(path);

            if (loaded)
            {
                gameboy.Start();
                loadedRom = path;
                if(!emulationThread.IsBusy)
                {
                    emulationThread.RunWorkerAsync();
                }
                this.Text = "Schoolyard - " + loadedRom;
            }
        }

        /* ================================
         * Emulation thread 
         =================================*/
        private void EmulationThread_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            while (gameboy.cpu.StateRunning)
            {
                if (isPaused || !gameboy.cpu.StateRunning)
                {
                    System.Threading.Thread.Sleep(0);
                    continue;
                }
                sw.Restart();
                ulong cycles = 0;

                while (!hasRenderedFrame)
                {
                    cycles += gameboy.Step();
                }
                hasRenderedFrame = false;
                sw.Stop();

                // Sleep 16 - elapsed, to run every 16ms
                int timeToSleep = 16 - (int)sw.ElapsedMilliseconds;
                if (timeToSleep <= 0) { timeToSleep = 0; }
                System.Threading.Thread.Sleep(timeToSleep);
            }
        }

        /* ================================
         * Display / Input / Audio thread 
         =================================*/
        private void smflDisplayThread_DoWork(object sender, DoWorkEventArgs e)
        {
            SFML.Graphics.Image gameboyScreen = new SFML.Graphics.Image(Schoolyard.LCD.PPU.width, Schoolyard.LCD.PPU.height);
            SFML.Graphics.Texture gameboyTexture = new SFML.Graphics.Texture(gameboyScreen);
            SFML.Graphics.Sprite gameboySprite = new SFML.Graphics.Sprite(gameboyTexture);
            SFML.Graphics.View gameboyView = new SFML.Graphics.View(new SFML.System.Vector2f(Schoolyard.LCD.PPU.width / 2, Schoolyard.LCD.PPU.height / 2), (new SFML.System.Vector2f(Schoolyard.LCD.PPU.width, Schoolyard.LCD.PPU.height)));
            gameboyTexture.Smooth = false;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            while (!hasExited)
            {
                if(sfmlWindow == null) {
                    continue;
                }

                bool shouldSleep = false;

                sw.Restart();

                sfmlWindow.DispatchEvents();
                CheckForInput();

                // Draw
                sfmlWindow.Clear(SFML.Graphics.Color.Black); // Clear
                sfmlWindow.SetView(gameboyView);

                if(!sfmlHasDrawnFrame)
                {
                    // Draw gameboy screen
                    SFML.Graphics.Color white = new SFML.Graphics.Color(224, 248, 208);
                    SFML.Graphics.Color lightGray = new SFML.Graphics.Color(136, 192, 112);
                    SFML.Graphics.Color darkGray = new SFML.Graphics.Color(52, 104, 86);
                    SFML.Graphics.Color black = new SFML.Graphics.Color(8, 24, 32);

                    for (uint y = 0; y < 144; y++)
                    {
                        for (uint x = 0; x < 160; x++)
                        {
                            byte raw = gameboy.ppu.framebuffer[x, y];

                            switch (raw)
                            {
                                case 0x0:
                                    gameboyScreen.SetPixel(x, y, white);
                                    break;
                                case 0x1:
                                    gameboyScreen.SetPixel(x, y, lightGray);
                                    break;
                                case 0x2:
                                    gameboyScreen.SetPixel(x, y, darkGray);
                                    break;
                                case 0x3:
                                default:
                                    gameboyScreen.SetPixel(x, y, black);
                                    break;
                            }
                        }
                    }
                    gameboyTexture.Update(gameboyScreen);
                    sfmlHasDrawnFrame = true;
                    shouldSleep = true;
                }

                sfmlWindow.Draw(gameboySprite);
                sfmlWindow.Display(); // display what SFML has drawn to the screen

                sw.Stop();

                if(shouldSleep) { // Sleep if we just drew. Sleep a little less time, so that we can be ready once a frame has been drawn.
                    int timeToSleep = 15 - (int)sw.ElapsedMilliseconds;
                    if (timeToSleep <= 0) { timeToSleep = 0; }
                    System.Threading.Thread.Sleep(timeToSleep);
                }
                
            }
        }

        private void CheckForInput()
        {
            lock (gameboy.keypad)
            {
                gameboy.keypad.Left = Keyboard.IsKeyPressed(Keyboard.Key.Left);
                gameboy.keypad.Right = Keyboard.IsKeyPressed(Keyboard.Key.Right);
                gameboy.keypad.Down = Keyboard.IsKeyPressed(Keyboard.Key.Down);
                gameboy.keypad.Up = Keyboard.IsKeyPressed(Keyboard.Key.Up);
                gameboy.keypad.Start = Keyboard.IsKeyPressed(Keyboard.Key.Return);
                gameboy.keypad.Select = Keyboard.IsKeyPressed(Keyboard.Key.BackSpace);
                gameboy.keypad.A = Keyboard.IsKeyPressed(Keyboard.Key.A);
                gameboy.keypad.B = Keyboard.IsKeyPressed(Keyboard.Key.S);
            }
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
            hasExited = true;
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            Height = (int)(Ratio * Width); // Maintain aspect ratio
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isPaused = !isPaused;
            pauseToolStripMenuItem.Checked = isPaused;
        }

        private void rebootGameboyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryLoadFile(loadedRom);
        }

        private void debuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (debugger == null)
            {
                debugger = new Debugger(gameboy);
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

        /* ================================
         * Normal plumbing
         =================================*/
        private void OnRenderFrame(object sender, EventArgs e)
        {
            hasRenderedFrame = true;
            sfmlHasDrawnFrame = false;
        }
    }
}
