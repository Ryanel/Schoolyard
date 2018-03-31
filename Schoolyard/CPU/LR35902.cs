using Schoolyard.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.CPU
{
    public partial class LR35902
    {
        // State
        public bool StateHalt { get; private set; }
        public bool StateRunning;
        public ulong cycles;
        public bool haltBug = false;

        // Statistics
        public ulong instructionsExecuted = 0;

        // Components
        public Registers regs = new Registers();

        // References
        private Gameboy gameboy;
        public Memory.MemoryController mem;

        public LR35902(Gameboy gameboy)
        {
            this.gameboy = gameboy;
            mem = gameboy.memory;
            alu = new ALU(this);
        }

        public void Reset()
        {
            regs.Reset();
            cycles = 0;
            instructionsExecuted = 0;
            StateHalt = false;
            StateRunning = true;
            haltBug = false;
        }

        public ulong Step()
        {
            if(!StateRunning)
            {
                return 0;
            }

            // LR35902 design:
            // Check for interrupts before next instruction cycle
            // If there are any interrupts, do that this instruction cycle
            // Run an instruction if we're not halted
            ulong cyclesBefore = cycles;

            if (!HandleInterrupts())
            {
                if(!StateHalt)
                {
                    cycles += (ulong)RunInstruction();
                }
                else
                {
                    cycles += 4;
                }
            }
            return cycles - cyclesBefore;
        }

        /// <summary>
        /// Checks for and runs interrupts
        /// </summary>
        /// <returns>True if interrupt was fired</returns>
        public bool HandleInterrupts()
        {
            byte flags = regs.interruptFlag;
            byte enable = regs.interruptEnableFlag;
            if ((regs.IME == true) && (enable != 0) && (flags != 0))
            {
                byte fired = (byte)(enable & flags);
                if (haltBug)
                {
                    StateHalt = false;
                    cycles += 4;
                    return true;
                }
                if ((fired & (byte)Registers.InterruptFlags.VBlank) != 0) // VBlank
                {
                    mem.Write8(0xFF85, 0); // Set to 0 on vblank
                    DoInterrupt(Registers.InterruptFlags.VBlank, 0x40);
                }
                else if ((fired & (byte)Registers.InterruptFlags.LCDStat) != 0) // LCD Status interrupt
                {
                    DoInterrupt(Registers.InterruptFlags.LCDStat, 0x48);
                }
                else if ((fired & (byte)Registers.InterruptFlags.Timer) != 0) // Timer
                {
                    DoInterrupt(Registers.InterruptFlags.Timer, 0x50);
                }
            }

            return false;
        }

        private void DoInterrupt(Registers.InterruptFlags interrupt, ushort address)
        {
            // Set flag
            byte flags = regs.interruptFlag;
            regs.interruptsMasterEnable = false;
            unchecked {
                flags &= (byte)~(byte)interrupt;
            }
            mem.Write8(0xFF0F, flags);

            // Jump to address
            Push16(PC);
            regs.pc = address;
            cycles += 12;
            StateHalt = false;
        }

        public void IssueInterrupt(Registers.InterruptFlags interrupt)
        {
            byte flags = regs.interruptFlag;
            byte enable = regs.interruptEnableFlag;
            byte intbyte = (byte)interrupt;
            if ((enable & (byte)intbyte) != 0)
            {
                flags |= (byte)intbyte;
                mem.Write8(0xFF0F, flags);
            }
        }

        /// <summary>
        /// Run the next instruction
        /// </summary>
        public int RunInstruction()
        {
            Instruction i = Dissassembler.ReadInstruction(this, regs.pc);
            regs.pc += (ushort)i.code.Length;
            int cycles =  i.code.Operation(this, i);

            regs.T += cycles;
            instructionsExecuted++;
            return cycles;
        }

        public void Halt()
        {
            StateHalt = true;

            if (regs.interruptsMasterEnable != true)
            {
                haltBug = true;
            }
        }

        public void Stop()
        {
            StateRunning = false;
        }
    }
}
