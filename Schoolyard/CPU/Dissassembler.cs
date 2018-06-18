namespace Schoolyard.CPU
{
    public static class Dissassembler
    {
        public static LR35902.Instruction ReadInstruction(LR35902 cpu,ushort address)
        {
            LR35902.Opcode opcode;
            var instruction = new LR35902.Instruction();

            // Get opcode
            byte op = cpu.mem.Read8(address);

            // Decode instruction
            if (op != 0xCB) {
                opcode = LR35902.OpCodes[op];
            }
            else {
                op = cpu.mem.Read8((ushort)(address + 1));
                opcode = LR35902.OpCodesPrefix[op];
                instruction.isPrefix = true;
            }

            instruction.Opcode = op;
            instruction.code = opcode;

            // Populate operands
            byte[] operands = new byte[opcode.Length - 1];
            for (int i = 0; i < opcode.Length - 1; i++) {
                operands[i] = cpu.mem.Read8((ushort)(address + i + 1));
            }

            instruction.Operands = operands;
            
            return instruction;
        }
    }
}
