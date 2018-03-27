using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schoolyard.Utilities;

namespace Schoolyard.CPU
{
    public partial class LR35902
    {
        public delegate int CodeOP(LR35902 cpu, Instruction ins);

        public struct Instruction
        {
            public byte Opcode { get; }
            public ushort Offset { get; }
            public byte[] Operands;

            public Opcode code;
            public bool isPrefix;
            public Instruction(byte op, ushort pos, byte[] operands, Opcode code)
            {
                Opcode = op;
                Offset = pos;
                Operands = operands;
                this.code = code;
                isPrefix = false;
            }

            public byte Operand8
            {
                get
                {
                    return Operands[0];
                }
            }

            public ushort Operand16
            {
                get
                {
                    return ByteUtilities.Pack16(Operands[1], Operands[0]);
                }
            }

            public override string ToString()
            {
                switch (code.Length)
                {
                    default:
                    case 1:
                        return code.Disassembly;
                    case 2:
                        return String.Format(code.Disassembly, Operand8);
                    case 3:
                        return String.Format(code.Disassembly, Operand16);
                }
            }
        }

        public struct Opcode
        {
            /// Dissassembly format string
            public readonly string Disassembly;
            /// The actual opcode
            public readonly byte Code;
            /// Length in bytes
            public readonly int Length;
            /// The code to run
            public readonly CodeOP Operation;

            public Opcode(byte opcode, string disassembly, int length, CodeOP operation)
            {
                Disassembly = disassembly;
                Code = opcode;
                Length = length;
                Operation = operation;
            }
        }
    }
}
