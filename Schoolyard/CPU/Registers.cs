using Schoolyard.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.CPU
{
    public class Registers
    {
        public byte a;
        public byte f;
        public byte b;
        public byte c;
        public byte d;
        public byte e;
        public byte h;
        public byte l;

        public ushort sp;
        public ushort pc;
        public long t;
        public bool interruptsMasterEnable;

        [Flags]
        public enum RegFlags : byte
        {
            None = 0,
            Z = 1 << 7,
            N = 1 << 6,
            H = 1 << 5,
            C = 1 << 4,
            All = Z | N | H | C
        }

        public byte A { get { return a; } set { a = value; } }
        public byte B { get { return b; } set { b = value; } }
        public byte C { get { return c; } set { c = value; } }
        public byte D { get { return d; } set { d = value; } }
        public byte E { get { return e; } set { e = value; } }
        public byte H { get { return h; } set { h = value; } }
        public byte L { get { return l; } set { l = value; } }
        public byte Flags { get { return f; } set { f = value; } }

        public ushort SP { get { return sp; } set { sp = value; } }
        public ushort PC { get { return pc; } set { pc = value; } }

        // Packed registers. Transparently combine / split registers
        public ushort AF
        {
            get { return ByteUtilities.Pack16(a, f); }
            set { ByteUtilities.Unpack16(value, out a, out f); }
        }
        public ushort BC
        {
            get { return ByteUtilities.Pack16(b, c); }
            set { ByteUtilities.Unpack16(value, out b, out c); }
        }
        public ushort DE
        {
            get { return ByteUtilities.Pack16(d, e); }
            set { ByteUtilities.Unpack16(value, out d, out e); }
        }
        public ushort HL
        {
            get { return ByteUtilities.Pack16(h, l); }
            set { ByteUtilities.Unpack16(value, out h, out l); }
        }

        public bool IME { get { return interruptsMasterEnable; } set { interruptsMasterEnable = value; } }

        public long M { get { return t / 4; } }
        public long T { get { return t; } set { t = value; } }

        public void Reset()
        {
            PC = 0x100;
            AF = 0x01B0;
            BC = 0x0013;
            DE = 0x00D8;
            HL = 0x014D;
            SP = 0xFFFE;
            interruptsMasterEnable = false;
        }
    }
}
