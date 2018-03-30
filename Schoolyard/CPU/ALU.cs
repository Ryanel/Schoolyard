using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Schoolyard.CPU.Registers;

namespace Schoolyard.CPU
{
    public partial class LR35902
    {
        public ALU alu;
        public class ALU
        {
            private LR35902 cpu;
            public ALU(LR35902 cpu)
            {
                this.cpu = cpu;
            }

            public byte Calculate(byte a, byte b, Func<byte, byte, int> operation,
                RegFlags flagsToUpdate,
                RegFlags setFlags = RegFlags.None,
                RegFlags resetFlags = RegFlags.None)
            {
                cpu.ClearFlags(resetFlags); // Clear flags we're told to clear
                cpu.SetFlags(setFlags); // Set flags we're told to set
                cpu.ClearFlags(flagsToUpdate);

                int operation_result = operation(a, b);
                byte result = (byte)(operation_result & 0xFF);
                int carryBits = (a ^ b ^ operation_result);

                RegFlags flags = RegFlags.None;

                // Set carry flag
                if ((carryBits & 0x100) == 0x100)
                {
                    flags |= RegFlags.C;
                }

                // Set half carry
                if ((carryBits & 0x10) == 0x10)
                {
                    flags |= RegFlags.H;
                }

                // Set zero flag
                if (result == 0)
                {
                    flags |= RegFlags.Z;
                }

                // Set the flags. The & flagsToUpdate ensures that we only set flags we want to update.
                cpu.SetFlags(flags & flagsToUpdate);
                return result;
            }

            public ushort Calculate(ushort a, ushort b, Func<ushort, ushort, int> operation,
                RegFlags flagsToUpdate,
                RegFlags setFlags = RegFlags.None,
                RegFlags resetFlags = RegFlags.None)
            {
                cpu.ClearFlags(resetFlags); // Clear flags we're told to clear
                cpu.SetFlags(setFlags); // Set flags we're told to set
                // Re-clear anything we're updating.
                // We do this after so we don't mess with our state
                cpu.ClearFlags(flagsToUpdate);

                int operation_result = operation(a, b);
                ushort result = (ushort)(operation_result & 0xFFFF);

                RegFlags flags = RegFlags.None;

                // Set carry flag
                if (((a ^ b ^ operation_result) & 0x10000) == 0x10000)
                {
                    flags |= RegFlags.C;
                }

                // Set half carry
                if (((a ^ b ^ result) & 0x1000) == 0x1000)
                {
                    flags |= RegFlags.H;
                }

                // Set zero flag
                if (result == 0)
                {
                    flags |= RegFlags.Z;
                }

                // Set the flags. The & flagsToUpdate ensures that we only set flags we want to update.
                cpu.SetFlags(flags & flagsToUpdate);
                return result;
            }

            public ushort Calculate(ushort a, sbyte b, Func<ushort, sbyte, int> operation,
                RegFlags flagsToUpdate,
                RegFlags setFlags = RegFlags.None,
                RegFlags resetFlags = RegFlags.None)
            {
                cpu.ClearFlags(resetFlags); // Clear flags we're told to clear
                cpu.SetFlags(setFlags); // Set flags we're told to set
                // Re-clear anything we're updating.
                // We do this after so we don't mess with our state
                cpu.ClearFlags(flagsToUpdate);

                int operation_result = operation(a, b);
                ushort result = (ushort)(operation_result & 0xFFFF);

                RegFlags flags = RegFlags.None;

                // Set carry flag
                if (((a ^ b ^ operation_result) & 0x100) == 0x100)
                {
                    flags |= RegFlags.C;
                }

                // Set half carry
                if (((a ^ b ^ result) & 0x10) == 0x10)
                {
                    flags |= RegFlags.H;
                }

                // Set zero flag
                if (result == 0)
                {
                    flags |= RegFlags.Z;
                }

                // Set the flags. The & flagsToUpdate ensures that we only set flags we want to update.
                cpu.SetFlags(flags & flagsToUpdate);
                return result;
            }

            public byte Add(byte a, byte b, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Calculate(a, b, (x, y) => { return x + y; }, updateFlags, setFlags, resetFlags);
            }

            public ushort Add(ushort a, ushort b, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Calculate(a, b, (x, y) => { return x + y; }, updateFlags, setFlags, resetFlags);
            }

            public ushort Add(ushort a, byte b, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Calculate(a, b, (x, y) => { return x + y; }, updateFlags, setFlags, resetFlags);
            }

            public ushort Add(ushort a, sbyte b, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Calculate(a, b, (x, y) => { return x + y; }, updateFlags, setFlags, resetFlags);
            }

            public byte Adc(byte a, byte b, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                int carrybefore = cpu.FlagCarry ? 1 : 0;
                return Calculate(a, b, (x, y) => {
                    return x + y + carrybefore;
                }, updateFlags, setFlags, resetFlags);
            }

            public byte Sub(byte a, byte b, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Calculate(a, b, (x, y) => { return x - y; }, updateFlags, setFlags, resetFlags);
            }

            public ushort Sub(ushort a, ushort b, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Calculate(a, b, (x, y) => { return x - y; }, updateFlags, setFlags, resetFlags);
            }

            public byte Sbc(byte a, byte b, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                int carrybefore = cpu.FlagCarry ? 1 : 0;
                return Calculate(a, b, (x, y) => {
                    return x - (y + carrybefore);
                }, updateFlags, setFlags, resetFlags);
            }

            public byte Increment(byte a, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Add(a, (byte)1, updateFlags, setFlags, resetFlags);
            }

            public ushort Increment(ushort a, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Add(a, (ushort)1, updateFlags, setFlags, resetFlags);
            }

            public byte Decrement(byte a, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Sub(a, 1, updateFlags, setFlags, resetFlags);
            }

            public ushort Decrement(ushort a, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                return Sub(a, (ushort)1, updateFlags, setFlags, resetFlags);
            }

            public byte Xor(byte a)
            {
                unchecked
                {
                    cpu.A = (byte)(cpu.A ^ a);
                    cpu.Flags = (byte)RegFlags.None;
                    if (cpu.A == 0)
                    {
                        cpu.Flags = (byte)RegFlags.Z;
                    }
                    return cpu.A;
                }
            }

            public byte And(byte a)
            {
                unchecked
                {
                    byte result = (byte)((cpu.A & a) & 0xFF);
                    cpu.Flags = (byte)(RegFlags.H | (result == 0 ? RegFlags.Z : RegFlags.None));
                    return result;
                }
            }

            public byte Or(byte a)
            {
                unchecked
                {
                    byte result = (byte)((cpu.A | a) & 0xFF);
                    cpu.Flags = (byte)(result == 0 ? RegFlags.Z : RegFlags.None);
                    return result;
                }
            }

            public void Compare(byte n, RegFlags updateFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                Sub(cpu.A, n, updateFlags, setFlags, resetFlags);
            }

            public byte Cpl(byte value)
            {
                cpu.SetFlags(RegFlags.N | RegFlags.H);
                return (byte)~value;
            }
            public void Ccf()
            {
                cpu.ClearFlags(RegFlags.N | RegFlags.H);
                if (cpu.FlagCarry)
                    cpu.ClearFlags(RegFlags.C);
                else
                    cpu.SetFlags(RegFlags.C);
            }

            public byte Swap(byte value)
            {
                byte upper = (byte)(value & 0x0F);
                byte lower = (byte)(value & 0xF0);
                upper = (byte)(upper << 4);
                lower = (byte)(lower >> 4);
                byte result = (byte)(upper | lower);
                cpu.Flags = (byte)((result == 0 ? RegFlags.Z : RegFlags.None));
                return result;
            }

            public byte Set(byte value, int position)
            {
                return (byte)(value | (1 << position));
            }

            public byte Res(byte value, int position)
            {
                return (byte)(value & ~(1 << position));
            }


            // TODO: Rewrite in cleaner style!

            public byte Rl(byte value, RegFlags affectedFlags = RegFlags.None,
            RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                byte newValue = (byte)((value << 1) | (cpu.FlagCarry ? 1 : 0));

                cpu.ClearFlags(affectedFlags | resetFlags);
                cpu.SetFlags(setFlags);

                var flags = RegFlags.None;
                if (newValue == 0)
                    flags |= RegFlags.Z;
                if ((value & (1 << 7)) == (1 << 7))
                    flags |= RegFlags.C;
                cpu.SetFlags(flags & affectedFlags);

                return newValue;
            }

            public byte Rlc(byte value, RegFlags affectedFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                byte newValue = (byte)((value << 1) | (value >> 7));

                cpu.ClearFlags(affectedFlags | resetFlags);
                cpu.SetFlags(setFlags);
                var flags = RegFlags.None;
                if (newValue == 0)
                    flags |= RegFlags.Z;
                if ((value & (1 << 7)) == (1 << 7))
                    flags |= RegFlags.C;
                cpu.SetFlags(flags & affectedFlags);

                return newValue;
            }

            public byte Rr(byte value, RegFlags affectedFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                byte newValue = (byte)((value >> 1) | (cpu.FlagCarry ? 1 << 7 : 0));

                cpu.ClearFlags(affectedFlags | resetFlags);
                cpu.SetFlags(setFlags);
                var flags = RegFlags.None;
                if (newValue == 0)
                    flags |= RegFlags.Z;
                if ((value & 1) == 1)
                    flags |= RegFlags.C;
                cpu.SetFlags(flags & affectedFlags);

                return newValue;
            }

            public byte Rrc(byte value, RegFlags affectedFlags = RegFlags.None,
                RegFlags setFlags = RegFlags.None, RegFlags resetFlags = RegFlags.None)
            {
                byte newValue = (byte)((value >> 1) | ((value & 1) << 7));

                cpu.ClearFlags(affectedFlags | resetFlags);
                cpu.SetFlags(setFlags);
                var flags = RegFlags.None;
                if (newValue == 0)
                    flags |= RegFlags.Z;
                if ((value & 1) == 1)
                    flags |= RegFlags.C;
                cpu.SetFlags(flags & affectedFlags);

                return newValue;
            }

            public byte Sla(byte value)
            {
                byte newValue = (byte)(value << 1);
                var flags = RegFlags.None;
                if (newValue == 0)
                    flags |= RegFlags.Z;
                if ((value & (1 << 7)) == (1 << 7))
                    flags |= RegFlags.C;
                cpu.Flags = (byte)flags;
                return newValue;
            }


            public void Bit(byte value, int position)
            {
                var c = cpu.FlagCarry ? RegFlags.C : RegFlags.None;
                var z = ((value >> position) & 1) == 0 ? RegFlags.Z : RegFlags.None;
                cpu.Flags = (byte)(z | RegFlags.H | c);
            }

            public void Daa()
            {
                var flags = RegFlags.None;
                int a = cpu.A;
                if(!cpu.FlagNegative)
                {
                    if (cpu.FlagHalfCarry || (a & 0xF) > 9)
                    {
                        a += 0x06;
                    }
                    if (cpu.FlagCarry || a > 0x9F)
                    {
                        a += 0x60;
                    }
                }
                else
                {
                    if (cpu.FlagHalfCarry)
                    {
                        a = (a - 6) & 0xFF;
                    }
                    if (cpu.FlagCarry)
                    {
                        a -= 0x60;
                    }
                }

                cpu.ClearFlags(RegFlags.H | RegFlags.Z);

                if ((a & 0x100) == 0x100)
                {
                    flags |= RegFlags.C;
                }

                a &= 0xFF;

                // Set zero flag
                if (a == 0)
                {
                    flags |= RegFlags.Z;
                }

                cpu.SetFlags(flags);

                cpu.A = (byte)a;

            }
            public byte Sr(byte value, bool resetMsb)
            {
                byte newValue = (byte)(value >> 1);
                if (!resetMsb)
                {
                    newValue |= (byte)(value & (1 << 7));
                }

                // Set flags
                RegFlags flags = RegFlags.None;
                if (newValue == 0) { flags |= RegFlags.Z; }
                if ((value & 1) == 1) { flags |= RegFlags.C; }
                cpu.Flags = (byte)flags;

                return newValue;
            }
            public void Scf()
            {
                cpu.ClearFlags(RegFlags.N | RegFlags.H);
                cpu.SetFlags(RegFlags.C);
            }
        }
    }
}
