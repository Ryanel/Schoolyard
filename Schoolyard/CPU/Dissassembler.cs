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
            byte op = cpu.mem.Read8(address);
            LR35902.Opcode opcode = new LR35902.Opcode(op, "Unknown Opcode", 1, LR35902.OpUnimplemented);
            bool isPrefix = op == 0xCB;
            if (op != 0xCB)
            {
                opcode = LR35902.OpCodes[op];
            }
            else
            {
                opcode = new LR35902.Opcode(op, "Unknown Opcode (Prefix CB)", 2, LR35902.OpUnimplemented);
                op = cpu.mem.Read8((ushort)(address + 1));

                //TODO: Change over to lookup once this is done
                foreach (var item in LR35902.OpCodesPrefix)
                {
                    if (item.Code == op)
                    {
                        opcode = item;
                        break;
                    }
                }
            }

            byte[] operands = new byte[opcode.Length - 1];
            for (int i = 0; i < opcode.Length - 1; i++)
            {
                operands[i] = cpu.mem.Read8((ushort)(address + i + 1));
            }

            var instruction = new LR35902.Instruction(op, address, operands, opcode)
            {
                isPrefix = isPrefix
            };

            return instruction;
        }
    }
}
