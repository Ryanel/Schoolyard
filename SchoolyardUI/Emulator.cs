using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Schoolyard;
using SFML;
using SFML.Window;

namespace SchoolyardUI
{
    public class Emulator
    {
        private Gameboy gameboy;

        public bool paused = false;


        private bool hasRenderedFrame = false;
        private bool sfmlHasDrawnFrame = false;

        private bool shouldRender = true;
        private bool exit = false;

        static AutoResetEvent ppuRenderAutoReset = new AutoResetEvent(false);

        // Display
        SFML.Graphics.RenderWindow sfmlWindow;

        public Emulator(SFML.Graphics.RenderWindow sfmlWindow)
        {
            this.sfmlWindow = sfmlWindow;
            CreateGameboy();
        }

        private void CreateGameboy()
        {
            gameboy = new Gameboy();
            gameboy.Reset();
            gameboy.ppu.OnDisplayRendered += OnRenderFrame;
        }

        public void Start()
        {
            gameboy.Start();
        }

        public void Stop()
        {
            gameboy.Reset();
            gameboy.cpu.Stop();
            gameboy.cpu.StateRunning = false;
        }

        public bool LoadROM(string path)
        {
            return gameboy.loader.LoadROM(path);
        }

        public void EmulationThread()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            
            while(gameboy.cpu.StateRunning)
            {
                if(paused)
                {
                    Thread.Sleep(0); // Yield
                    continue;
                }

                sw.Restart();
                ulong cycles = 0;

                GetInput(); // Update input before emulation cycle

                while (!hasRenderedFrame)
                {
                    cycles += gameboy.Step();
                }

                hasRenderedFrame = false;

                ThreadUtilities.SleepUntilTargetTime(sw, 16.66f);
            }
        }
        


        public void DisplayThread()
        {
            SFML.Graphics.Image gameboyScreen = new SFML.Graphics.Image(Schoolyard.LCD.PPU.width, Schoolyard.LCD.PPU.height);
            SFML.Graphics.Texture gameboyTexture = new SFML.Graphics.Texture(gameboyScreen);
            SFML.Graphics.Sprite gameboySprite = new SFML.Graphics.Sprite(gameboyTexture);
            SFML.Graphics.View gameboyView = new SFML.Graphics.View(new SFML.System.Vector2f(Schoolyard.LCD.PPU.width / 2, Schoolyard.LCD.PPU.height / 2), (new SFML.System.Vector2f(Schoolyard.LCD.PPU.width, Schoolyard.LCD.PPU.height)));
            gameboyTexture.Smooth = false;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            while (!exit)
            {
                if (sfmlWindow == null || shouldRender == false)
                {
                    continue;
                }

                ppuRenderAutoReset.WaitOne();
                ppuRenderAutoReset.Reset();

                sw.Restart();
  
                // Draw
                sfmlWindow.Clear(SFML.Graphics.Color.Black); // Clear
                sfmlWindow.SetView(gameboyView);

                if (!sfmlHasDrawnFrame)
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
                }

                sfmlWindow.Draw(gameboySprite);
                sfmlWindow.Display(); // display what SFML has drawn to the screen
            }
        }

        public void GetInput()
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

        public void OnWindowResize()
        {
            sfmlWindow.DispatchEvents();
            ppuRenderAutoReset.Set();
        }

        private void OnRenderFrame(object sender, EventArgs e)
        {
            hasRenderedFrame = true;
            ppuRenderAutoReset.Set();
        }

        public Gameboy GetGameboy()
        {
            return gameboy;
        }
    }
}
