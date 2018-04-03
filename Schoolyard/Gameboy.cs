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
        public Timer timer;
        public DMAController dma;
        public Input.Keypad keypad;
        public Gameboy()
        {
            loader = new ROMLoader(this);
            memory = new MemoryController(this);
            cpu = new LR35902(this);
            ppu = new PPU(this);
            timer = new Timer(this);
            dma = new DMAController(this);
            keypad = new Input.Keypad();
        }

        public void SetupMemoryMap()
        {
            RAM wram   = new RAM("wram", 0xC000, 0x2000);
            EchoRAM ech = new EchoRAM("echo", 0xE000, 0x1E00, 0xC000, memory);
            Reserved r = new Reserved("resv", 0xFEA0, 0x60);
            RAM hwio   = new RAM("hwio - unmapped", 0xFF00, 0x80);
            RAM hiram  = new RAM("hiram", 0xFF80, 0x7F);
            InterruptEnableFlag inte = new InterruptEnableFlag("ie", 0xFFFF, cpu.regs);
            InterruptFlag intf = new InterruptFlag("if", 0xFF0F, cpu.regs);

            // Memory Mapping rules
            // 1. Order determines priorities
            // 2. Multiple devices can be mapped to the same address
            //                        0x0000 - 0xA000 ROM
            memory.Map(dma);
            memory.Map(hiram);     // 0xFF80 - 0xFFFE Zero Page
            memory.Map(intf);
            memory.Map(ppu.cram);  // 0x8000 - 0x97FF CRAM
            memory.Map(ppu.bgram); // 0x9800 - 0x9FFF BG 1 and 2
            memory.Map(wram);      // 0xC000 - 0xDFFF Work RAM
            memory.Map(ech);       // 0xE000 - 0xFDFF Echo of Work RAM
            memory.Map(ppu.oam);   // 0xFE00 - 0xFE9F OAM
            memory.Map(r);         // 0xFEA0 - 0xFEFF Unmapped
            memory.Map(timer);
            memory.Map(ppu.regs);  // 0xFF40 - 0xFF47 PPU Registers
            memory.Map(keypad);    // 0xFF00
            memory.Map(hwio);      // 0xFF00 - 0xFF80 Unmapped HWIO
            memory.Map(inte);        // 0xFFFF          Interrupt enable byte

            memory.RebuildCache();

            memory.Write8(0xFF05, 0x00);
            memory.Write8(0xFF06, 0x00);
            memory.Write8(0xFF07, 0x00);
            memory.Write8(0xFF10, 0x80);
            memory.Write8(0xFF11, 0xBF);
            memory.Write8(0xFF12, 0xF3);
            memory.Write8(0xFF14, 0xBF);
            memory.Write8(0xFF16, 0x3F);
            memory.Write8(0xFF17, 0x00);
            memory.Write8(0xFF19, 0xBF);
            memory.Write8(0xFF1A, 0x7F);
            memory.Write8(0xFF1B, 0xFF);
            memory.Write8(0xFF1C, 0x9F);
            memory.Write8(0xFF1E, 0xBF);
            memory.Write8(0xFF20, 0xFF);
            memory.Write8(0xFF21, 0x00);
            memory.Write8(0xFF22, 0x00);
            memory.Write8(0xFF23, 0xBF);
            memory.Write8(0xFF24, 0x77);
            memory.Write8(0xFF25, 0xF3);
            memory.Write8(0xFF26, 0xF1);
            memory.Write8(0xFF40, 0x91);
            memory.Write8(0xFF42, 0x00);
            memory.Write8(0xFF43, 0x00);
            memory.Write8(0xFF45, 0x00);
            memory.Write8(0xFF47, 0xFC);
            memory.Write8(0xFF48, 0xFF);
            memory.Write8(0xFF49, 0xFF);
            memory.Write8(0xFF4A, 0x00);
            memory.Write8(0xFF4B, 0x00);
            memory.Write8(0xFFFF, 0x00);
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
            dma.Step(cyclesDelta);
            return cyclesDelta;
        }
    }
}
