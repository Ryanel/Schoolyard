﻿using System;
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
        public Timer timer;
        public Gameboy()
        {
            loader = new ROMLoader(this);
            memory = new MemoryController();
            cpu = new LR35902(this);
            ppu = new PPU(memory);
            timer = new Timer(memory);
        }

        public void SetupMemoryMap()
        {
            RAM wram   = new RAM("wram", 0xC000, 0x2000);
            RAM oam    = new RAM("oam", 0xFE00, 0xA0);
            Reserved r = new Reserved("resv", 0xFEA0, 0x60);
            RAM hwio   = new RAM("hwio - unmapped", 0xFF00, 0x80);
            RAM hiram  = new RAM("hiram", 0xFF80, 0x7F);
            RAM ie     = new RAM("ie", 0xFFFF, 0x01);

            // Memory Mapping rules
            // 1. Order determines priorities
            // 2. Multiple devices can be mapped to the same address
            //                        0x0000 - 0xA000 ROM
            memory.Map(ppu.cram);  // 0x8000 - 0x97FF CRAM
            memory.Map(ppu.bgram); // 0x9800 - 0x9FFF BG 1 and 2
            memory.Map(wram);      // 0xC000 - 0xDFFF Work RAM
            memory.Map(oam);       // 0xFE00 - 0xFE9F OAM
            memory.Map(r);         // 0xFEA0 - 0xFEFF Unmapped
            memory.Map(timer);
            memory.Map(ppu.regs);  // 0xFF40 - 0xFF47 PPU Registers
            memory.Map(hwio);      // 0xFF00 - 0xFF80 Unmapped HWIO
            memory.Map(hiram);     // 0xFF80 - 0xFFFE Zero Page
            memory.Map(ie);        // 0xFFFF          Interrupt enable byte
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

        public ulong Step()
        {
            ulong cyclesDelta = cpu.Step();
            ppu.Step(cyclesDelta);
            timer.Step(cyclesDelta);
            return cyclesDelta;
        }
    }
}
