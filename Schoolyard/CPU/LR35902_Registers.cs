using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Schoolyard.CPU.Registers;
using Schoolyard.Utilities;
namespace Schoolyard.CPU
{
    public partial class LR35902
    {
        public byte A { get { return regs.A; } set { regs.A = value; } }
        public byte B { get { return regs.B; } set { regs.B = value; } }
        public byte C { get { return regs.C; } set { regs.C = value; } }
        public byte D { get { return regs.D; } set { regs.D = value; } }
        public byte E { get { return regs.E; } set { regs.E = value; } }
        public byte H { get { return regs.H; } set { regs.H = value; } }
        public byte L { get { return regs.L; } set { regs.L = value; } }
        public byte Flags { get { return regs.Flags; } set { regs.Flags = value; } }
        public ushort SP { get { return regs.SP; } set { regs.SP = value; } }
        public ushort PC { get { return regs.PC; } set { regs.PC = value; } }
        public ushort AF { get { return regs.AF; } set { regs.AF = value; } }
        public ushort BC { get { return regs.BC; } set { regs.BC = value; } }
        public ushort DE { get { return regs.DE; } set { regs.DE = value; } }
        public ushort HL { get { return regs.HL; } set { regs.HL = value; } }
        public bool IME { get { return regs.IME; } set { regs.IME = value; } }

        public void SetFlags(RegFlags flags)
        {
            Flags |= (byte)flags;
        }

        public void ClearFlags(RegFlags flags)
        {
            Flags &= (byte)~(byte)flags;
        }

        // Flags
        public bool FlagZero { get { return (Flags & (byte)RegFlags.Z) != 0; } }
        public bool FlagNegative { get { return (Flags & (byte)RegFlags.N) != 0; } }
        public bool FlagHalfCarry { get { return (Flags & (byte)RegFlags.H) != 0; } }
        public bool FlagCarry { get { return (Flags & (byte)RegFlags.C) != 0; } }

        public void DebugPrintRegisters()
        {
            Console.WriteLine("#--- LR35902 Status -----------------------");
            Console.Write("AF: " + ByteUtilities.HexString(AF, true));
            Console.WriteLine(" BC: " + ByteUtilities.HexString(BC, true));
            Console.Write("DE: " + ByteUtilities.HexString(DE, true));
            Console.WriteLine(" HL: " + ByteUtilities.HexString(HL, true));
            Console.Write("PC: " + ByteUtilities.HexString(PC, true));
            Console.WriteLine(" SP: " + ByteUtilities.HexString(SP, true));
            Console.Write("M: " + regs.M);
            Console.WriteLine(" T: " + regs.T);
            Console.WriteLine("Instructions Run: " + instructionsExecuted);
            //Console.WriteLine("IE: " + ByteUtilities.HexString(mem.Read8(0xFFFF)));

            Console.WriteLine("#------------------------------------------");
        }
    }
}
