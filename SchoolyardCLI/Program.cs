using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Schoolyard;

namespace SchoolyardCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Gameboy gameboy = new Gameboy();
            gameboy.Reset();

            // Load rom
            if(!gameboy.loader.LoadROM("drmario.gb"))
            {
                Console.WriteLine("Failed to load rom");
                Console.ReadKey();
                return;
            }

            gameboy.Start();

            while (gameboy.cpu.StateRunning)
            {
                gameboy.Step();
                //gameboy.cpu.DebugPrintRegisters();
            }

            Console.ReadKey();
        }
    }
}
