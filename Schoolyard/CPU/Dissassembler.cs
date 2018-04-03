using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.CPU
{
    public static class Dissassembler
    {
        public static LR35902.Instruction ReadInstruction(LR35902 cpu,ushort address)
        {
            LR35902.Opcode opcode;

            // Get opcode
            byte op = cpu.mem.Read8(address);
            bool isPrefix = op == 0xCB;

            // Decode instruction
            if (!isPrefix)
            {
                opcode = LR35902.OpCodes[op];
            }
            else
            {
                op = cpu.mem.Read8((ushort)(address + 1));
                opcode = LR35902.OpCodesPrefix[op];
            }

            // Populate operands
            byte[] operands = new byte[opcode.Length - 1];
            for (int i = 0; i < opcode.Length - 1; i++)
            {
                operands[i] = cpu.mem.Read8((ushort)(address + i + 1));
            }

            // Generate instruction
            var instruction = new LR35902.Instruction(op, address, operands, opcode)
            {
                isPrefix = isPrefix
            };

            return instruction;
        }
    }
}
