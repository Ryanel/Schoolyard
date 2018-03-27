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
        public long cycles;

        // Statistics
        public long instructionsExecuted = 0;

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
        }

        public long Step()
        {
            if(!StateRunning)
            {
                return 0;
            }

            // LR35902 design:
            // Check for interrupts before next instruction cycle
            // If there are any interrupts, do that this instruction cycle
            // Run an instruction if we're not halted
            long cyclesBefore = cycles;

            if (!HandleInterrupts())
            {
                if(!StateHalt)
                {
                    cycles += RunInstruction();
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

            //Console.WriteLine(i.ToString());

            regs.T += cycles;
            instructionsExecuted++;
            return cycles;
        }

        public void Halt()
        {
            StateHalt = true;
        }

        public void Stop()
        {
            StateRunning = false;
        }
    }
}
