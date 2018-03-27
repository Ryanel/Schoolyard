using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schoolyard.CPU;
using Schoolyard.Memory;
using Schoolyard.LCD;

namespace Schoolyard
{
    public class Gameboy
    {
        public ROMLoader loader;
        public LR35902 cpu;
        public PPU ppu;
        public MemoryController memory;

        public Gameboy()
        {
            loader = new ROMLoader(this);
            memory = new MemoryController();
            cpu = new LR35902(this);
            ppu = new PPU();
        }

        public void SetupMemoryMap()
        {
            // 0x0000 - 0xA000 ROM

            RAM bgData = new RAM("bg", 0x9800, 0x800);              // BG Map 1 - 2
            RAM wram   = new RAM("wram", 0xC000, 0x2000);           // Work Ram
            RAM hwio   = new RAM("hwio - unmapped", 0xFF00, 0x80);  // Unmapped HWIO (so it doesn't crash)
            RAM hiram  = new RAM("hiram", 0xFF80, 0x7F);            // Zero page ram
            RAM ie     = new RAM("ie", 0xFFFF, 0x01);               // Interrupt enable bte

            // Memory Mapping rules
            // 1. Order determines priorities
            // 2. Multiple devices can be mapped to the same address

            memory.Map(ppu.cram); // 0x8000 - 0x97FF
            memory.Map(bgData);   // 0x9800 - 0x9FFF
            memory.Map(wram);     // 0xC000 - 0xDFFF
            memory.Map(ppu.regs); // 0xFF40
            memory.Map(hwio);     // 0xFF00 - 0xFF80
            memory.Map(hiram);    // 0xFF80 - 0xFFFE
            memory.Map(ie);       // 0xFFFF
        }

        public void Reset()
        {
            memory.Reset();
            cpu.Reset();
            cpu.StateRunning = false;
            ppu.Reset();
            SetupMemoryMap();
        }

        public void Start()
        {
            cpu.StateRunning = true;
        }

        public void Step()
        {
            ulong cyclesDelta = cpu.Step();
            ppu.Step(cyclesDelta);
        }
    }
}
