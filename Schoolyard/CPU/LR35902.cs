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
            byte flags = mem.Read8(0xFF0F);
            byte enable = mem.Read8(0xFFFF);
            if ((regs.IME == true) && (enable != 0) && (flags != 0))
            {
                byte fired = (byte)(enable & flags);
                if (haltBug)
                {
                    StateHalt = false;
                    cycles += 0;
                }
                if ((fired & (byte)Registers.InterruptFlags.VBlank) != 0) // VBlank
                {
                    StateHalt = false;
                    mem.Write8(0xFF85, 0); // Set to 0 on vblank
                    //Console.WriteLine("[CPU] Interrupt VBLANK @ " + ByteUtilities.HexString(PC, true));
                    // Clear flag
                    // Call interrupt
                    regs.interruptsMasterEnable = false;
                    unchecked { flags &= (byte)~(byte)0x1;}

                    Push16(PC);
                    PC = 0x0040;
                    cycles += 12;
                    return true;
                }
                if ((fired & (byte)Registers.InterruptFlags.LCDStat) != 0) // LCD Status interrupt
                {
                    StateHalt = false;
                    //mem.Write8(0xFF85, 0); // Set to 0 on vblank
                    regs.interruptsMasterEnable = false;
                    unchecked { flags &= (byte)~(byte)Registers.InterruptFlags.VBlank; }
                    //Console.WriteLine("[CPU] Interrupt STATUS @ " + ByteUtilities.HexString(PC, true));
                    Push16(PC);
                    PC = 0x0048;
                    cycles += 12;
                    return true;
                }
                if ((fired & (byte)Registers.InterruptFlags.Timer) != 0) // LCD Status interrupt
                {
                    StateHalt = false;
                    //mem.Write8(0xFF85, 0); // Set to 0 on vblank
                    regs.interruptsMasterEnable = false;
                    unchecked { flags &= (byte)~(byte)Registers.InterruptFlags.Timer; }
                    //Console.WriteLine("[CPU] Interrupt TIMER @ " + ByteUtilities.HexString(PC, true));
                    Push16(PC);
                    PC = 0x0050;
                    cycles += 12;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Run the next instruction
        /// </summary>
        public int RunInstruction()
        {
            Instruction i = Dissassembler.ReadInstruction(this, regs.pc);
            regs.pc += (ushort)i.code.Length;
            int cycles =  i.code.Operation(this, i);

            //Console.WriteLine(String.Format("${0:X4} : {1}",regs.pc, i.ToString()));

            regs.T += cycles;
            instructionsExecuted++;
            return cycles;
        }

        public void Halt()
        {
            StateHalt = true;
            // TODO: Set haltbug flag here
        }

        public void Stop()
        {
            StateRunning = false;
        }
    }
}
