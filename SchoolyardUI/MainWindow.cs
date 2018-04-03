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
        private Debugger debugger;
        private Gameboy gameboy;

        private bool hasRenderedFrame = false;
        private bool hasExited = false;

        private SFML.Graphics.RenderWindow sfmlWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void InitGraphics()
        {
            renderSurface.BeingDrawnTo = true;
            sfmlWindow = new SFML.Graphics.RenderWindow(renderSurface.Handle);
            sfmlWindow.SetActive(false);
            sfmlThread.RunWorkerAsync();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            gameboy = new Gameboy();
            gameboy.Reset();
            gameboy.ppu.OnDisplayRendered += OnRenderFrame;
            InitGraphics();
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

        private void OnRenderFrame(object sender, EventArgs e)
        {
            hasRenderedFrame = true;
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

        private void CheckForInput()
        {
            lock(gameboy.keypad)
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

        private void smflDisplayThread_DoWork(object sender, DoWorkEventArgs e)
        {
            SFML.Graphics.Image gameboyScreen = new SFML.Graphics.Image(Schoolyard.LCD.PPU.width, Schoolyard.LCD.PPU.height);
            SFML.Graphics.Texture gameboyTexture = new SFML.Graphics.Texture(gameboyScreen);
            SFML.Graphics.Sprite gameboySprite = new SFML.Graphics.Sprite(gameboyTexture);
            SFML.Graphics.View gameboyView = new SFML.Graphics.View(new SFML.System.Vector2f(Schoolyard.LCD.PPU.width / 2, Schoolyard.LCD.PPU.height / 2), (new SFML.System.Vector2f(Schoolyard.LCD.PPU.width, Schoolyard.LCD.PPU.height)));
            gameboyTexture.Smooth = false;

            while(!hasExited)
            {
                if(sfmlWindow == null) {
                    continue;
                }
                sfmlWindow.DispatchEvents();
                CheckForInput();
                sfmlWindow.Clear(SFML.Graphics.Color.Black); // Clear
                sfmlWindow.SetView(gameboyView);
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
                sfmlWindow.Draw(gameboySprite);
                sfmlWindow.Display(); // display what SFML has drawn to the screen
                System.Threading.Thread.Sleep(15);
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            hasExited = true;
        }
    }
}
