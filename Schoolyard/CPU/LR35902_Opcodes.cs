using Schoolyard.Utilities;
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
        public static readonly Opcode[] OpCodes =
{
            //        Opcode    Dissassembly      Length     Instruction Body
            // 0x00
            new Opcode(0x00, "nop"                 , 1, (c,i) => {return 4;}),
            new Opcode(0x01, "ld bc, {0:X4}"       , 3, (c,i) => {c.BC = i.Operand16; return 12;}),
            new Opcode(0x02, "ld (bc), a"          , 1, (c,i) => {c.mem.Write8(c.BC, c.A); return 8;}),
            new Opcode(0x03, "inc bc"              , 1, (c,i) => {ushort val = c.BC; int r = OpIncrement16(c,i,ref val); c.BC = val; return r;}),
            new Opcode(0x04, "inc b"               , 1, (c,i) => {return OpIncrement8(c,i,ref c.regs.b);}),
            new Opcode(0x05, "dec b"               , 1, (c,i) => {return OpDecrement8(c,i,ref c.regs.b);}),
            new Opcode(0x06, "ld b, {0:X2}"        , 2, (c,i) => {c.B = i.Operand8; return 8;}),
            new Opcode(0x07, "rrca"                , 1, (c,i) => {c.A = c.alu.Rlc(c.A, RegFlags.C, RegFlags.None, RegFlags.Z | RegFlags.H | RegFlags.N); return 4;}),
            new Opcode(0x08, "ld {0:X4}, sp"       , 3, (c,i) => {c.mem.Write16(i.Operand16, c.SP); return 20;}),
            new Opcode(0x09, "add hl, bc"          , 1, (c,i) => {c.HL = c.alu.Add(c.HL, c.BC, RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.Z); return 8;}),
            new Opcode(0x0A, "ld a, (bc)"          , 1, (c,i) => {c.A = c.mem.Read8(c.BC); return 8;}),
            new Opcode(0x0B, "dec bc"              , 1, (c,i) => {ushort val = c.BC; int r = OpDecrement16(c,i,ref val); c.BC = val; return r;}),
            new Opcode(0x0C, "inc c"               , 1, (c,i) => {return OpIncrement8(c,i,ref c.regs.c);}),
            new Opcode(0x0D, "dec c"               , 1, (c,i) => {return OpDecrement8(c,i,ref c.regs.c);}),
            new Opcode(0x0E, "ld c, {0:X2}"        , 2, (c,i) => {c.C = i.Operand8; return 8;}),
            new Opcode(0x0F, "rrca"                , 1, (c,i) => {c.A = c.alu.Rrc(c.A, RegFlags.C, RegFlags.None, RegFlags.Z | RegFlags.H | RegFlags.N); return 4;}),
           
            // 0x10  
            new Opcode(0x10, "stop"                , 2, (c,i) => {c.Stop(); return 4;}),
            new Opcode(0x11, "ld de, {0:X4}"       , 3, (c,i) => {c.DE = i.Operand16; return 12;}),
            new Opcode(0x12, "ld (de), a"          , 1, (c,i) => {c.mem.Write8(c.DE, c.A); return 8;}),
            new Opcode(0x13, "inc de"              , 1, (c,i) => {ushort val = c.DE; int r = OpIncrement16(c,i,ref val); c.DE = val; return r;}),
            new Opcode(0x14, "inc d"               , 1, (c,i) => {return OpIncrement8(c,i,ref c.regs.d);}),
            new Opcode(0x15, "dec d"               , 1, (c,i) => {return OpDecrement8(c,i,ref c.regs.d);}),
            new Opcode(0x16, "ld d, {0:X2}"        , 2, (c,i) => {c.D = i.Operand8; return 8;}),
            new Opcode(0x17, "rlca"                , 1, (c,i) => {c.A = c.alu.Rl(c.A, RegFlags.C, RegFlags.None, RegFlags.Z | RegFlags.H | RegFlags.N); return 4;}),
            new Opcode(0x18, "jr {0:X2}"           , 2, (c,i) => {return OpJumpRelative(c,i, true); }),
            new Opcode(0x19, "add hl, de"          , 1, (c,i) => {c.HL = c.alu.Add(c.HL, c.DE, RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.Z); return 8;}),
            new Opcode(0x1A, "ld A, (DE)"          , 1, (c,i) => {c.A = c.mem.Read8(c.DE); return 8;}),
            new Opcode(0x1B, "dec de"              , 1, (c,i) => {ushort val = c.DE; int r = OpDecrement16(c,i,ref val); c.DE = val; return r;}),
            new Opcode(0x1C, "inc e"               , 1, (c,i) => {return OpIncrement8(c,i,ref c.regs.e);}),
            new Opcode(0x1D, "dec e"               , 1, (c,i) => {return OpDecrement8(c,i,ref c.regs.e);}),
            new Opcode(0x1E, "ld e, {0:X2}"        , 2, (c,i) => {c.E = i.Operand8; return 8;}),
            new Opcode(0x1F, "rra"                 , 1, (c,i) => {c.A = c.alu.Rr(c.A, RegFlags.C, RegFlags.None, RegFlags.Z | RegFlags.H | RegFlags.N); return 4;}),
            // 0x20                                 
            new Opcode(0x20, "jr nz {0:X2}"        , 2, (c,i) => {return OpJumpRelative(c,i, c.FlagZero == false); }),
            new Opcode(0x21, "ld hl, {0:X4}"       , 3, (c,i) => {c.HL = i.Operand16; return 12;}),
            new Opcode(0x22, "ld (HL+), a"         , 1, (c,i) => {c.mem.Write8(c.HL,c.A); c.HL++; return 8;}),
            new Opcode(0x23, "inc hl"              , 1, (c,i) => {ushort val = c.HL; int r = OpIncrement16(c,i,ref val); c.HL = val; return r;}),
            new Opcode(0x24, "inc h"               , 1, (c,i) => {return OpIncrement8(c,i,ref c.regs.h);}),
            new Opcode(0x25, "dec h"               , 1, (c,i) => {return OpDecrement8(c,i,ref c.regs.h);}),
            new Opcode(0x26, "ld h, {0:X2}"        , 2, (c,i) => {c.H = i.Operand8; return 8;}),
            new Opcode(0x27, "daa"                 , 1, (c,i) => {c.alu.Daa(); return 4;}),
            new Opcode(0x28, "jr z {0:X2}"         , 2, (c,i) => {return OpJumpRelative(c,i, c.FlagZero == true); }),
            new Opcode(0x29, "add hl, hl"          , 1, (c,i) => {c.HL = c.alu.Add(c.HL, c.HL, RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.Z); return 8;}),
            new Opcode(0x2A, "ld A, (HL+)"         , 1, (c,i) => {c.A = c.mem.Read8(c.HL); c.HL++; return 8;}),
            new Opcode(0x2B, "dec hl"              , 1, (c,i) => {ushort val = c.HL; int r = OpDecrement16(c,i,ref val); c.HL = val; return r;}),
            new Opcode(0x2C, "inc l"               , 1, (c,i) => {return OpIncrement8(c,i,ref c.regs.l);}),
            new Opcode(0x2D, "dec l"               , 1, (c,i) => {return OpDecrement8(c,i,ref c.regs.l);}),
            new Opcode(0x2E, "ld l, {0:X2}"        , 2, (c,i) => {c.L = i.Operand8; return 8;}),
            new Opcode(0x2F, "cpl"                 , 1, (c,i) => {c.A = c.alu.Cpl(c.A); return 4;}),
            // 0x30                      
            new Opcode(0x30, "jr nc {0:X2}"        , 2, (c,i) => {return OpJumpRelative(c,i, c.FlagCarry == false); }),
            new Opcode(0x31, "ld sp, {0:X4}"       , 3, (c,i) => {c.SP = i.Operand16; return 12;}),
            new Opcode(0x32, "ld (HL-), a"         , 1, (c,i) => {c.mem.Write8(c.HL,c.A); c.HL--; return 8;}),
            new Opcode(0x33, "inc sp"              , 1, (c,i) => {ushort val = c.SP; int r = OpIncrement16(c,i,ref val); c.SP = val; return r;}),
            new Opcode(0x34, "inc (HL)"            , 1, (c,i) => {byte v = c.mem.Read8(c.HL); OpIncrement8(c,i,ref v); c.mem.Write8(c.HL,v); return 12;}),
            new Opcode(0x35, "dec (HL)"            , 1, (c,i) => {byte v = c.mem.Read8(c.HL); OpDecrement8(c,i,ref v); c.mem.Write8(c.HL,v); return 12;}),
            new Opcode(0x36, "ld (HL), {0:X2}"     , 2, (c,i) => {c.mem.Write8(c.HL,i.Operand8); return 12;}),
            new Opcode(0x37, "scf"                 , 1, (c,i) => {c.alu.Scf() ; return 4;}),
            new Opcode(0x38, "jr c {0:X2}"         , 2, (c,i) => {return OpJumpRelative(c,i, c.FlagCarry == true); }),
            new Opcode(0x39, "add hl, sp"          , 1, (c,i) => {c.HL = c.alu.Add(c.HL, c.SP, RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 8;}),
            new Opcode(0x3A, "ld A, (HL+)"         , 1, (c,i) => {c.A = c.mem.Read8(c.HL); c.HL--; return 8;}),
            new Opcode(0x3B, "dec sp"              , 1, (c,i) => {ushort val = c.SP; int r = OpDecrement16(c,i,ref val); c.SP = val; return r;}),
            new Opcode(0x3C, "inc a"               , 1, (c,i) => {return OpIncrement8(c,i,ref c.regs.a);}),
            new Opcode(0x3D, "dec a"               , 1, (c,i) => {return OpDecrement8(c,i,ref c.regs.a);}),
            new Opcode(0x3E, "ld a, {0:X2}"        , 2, (c,i) => {c.A = i.Operand8; return 8;}),
            new Opcode(0x3F, "ccf"                 , 1, (c,i) => {c.alu.Ccf(); return 4;}),
            // 0x40
            new Opcode(0x40, "ld b, b"             , 1, (c,i) => {c.B = c.B; return 4;}),
            new Opcode(0x41, "ld b, c"             , 1, (c,i) => {c.B = c.C; return 4;}),
            new Opcode(0x42, "ld b, d"             , 1, (c,i) => {c.B = c.D; return 4;}),
            new Opcode(0x43, "ld b, e"             , 1, (c,i) => {c.B = c.E; return 4;}),
            new Opcode(0x44, "ld b, h"             , 1, (c,i) => {c.B = c.H; return 4;}),
            new Opcode(0x45, "ld b, l"             , 1, (c,i) => {c.B = c.L; return 4;}),
            new Opcode(0x46, "ld b, (HL)"          , 1, (c,i) => {c.B = c.mem.Read8(c.HL); return 8;}),
            new Opcode(0x47, "ld b, a"             , 1, (c,i) => {c.B = c.A; return 4;}),
            new Opcode(0x48, "ld c, b"             , 1, (c,i) => {c.C = c.B; return 4;}),
            new Opcode(0x49, "ld c, c"             , 1, (c,i) => {c.C = c.C; return 4;}),
            new Opcode(0x4A, "ld c, d"             , 1, (c,i) => {c.C = c.D; return 4;}),
            new Opcode(0x4B, "ld c, e"             , 1, (c,i) => {c.C = c.E; return 4;}),
            new Opcode(0x4C, "ld c, h"             , 1, (c,i) => {c.C = c.H; return 4;}),
            new Opcode(0x4D, "ld c, l"             , 1, (c,i) => {c.C = c.L; return 4;}),
            new Opcode(0x4E, "ld c, (HL)"          , 1, (c,i) => {c.C = c.mem.Read8(c.HL); return 8;}),
            new Opcode(0x4F, "ld c, a"             , 1, (c,i) => {c.C = c.A; return 4;}),
            // 0x50
            new Opcode(0x50, "ld d, b"             , 1, (c,i) => {c.D = c.B; return 4;}),
            new Opcode(0x51, "ld d, c"             , 1, (c,i) => {c.D = c.C; return 4;}),
            new Opcode(0x52, "ld d, d"             , 1, (c,i) => {c.D = c.D; return 4;}),
            new Opcode(0x53, "ld d, e"             , 1, (c,i) => {c.D = c.E; return 4;}),
            new Opcode(0x54, "ld d, h"             , 1, (c,i) => {c.D = c.H; return 4;}),
            new Opcode(0x55, "ld d, l"             , 1, (c,i) => {c.D = c.L; return 4;}),
            new Opcode(0x56, "ld d, (HL)"          , 1, (c,i) => {c.D = c.mem.Read8(c.HL); return 8;}),
            new Opcode(0x57, "ld d, a"             , 1, (c,i) => {c.D = c.A; return 4;}),
            new Opcode(0x58, "ld e, b"             , 1, (c,i) => {c.E = c.B; return 4;}),
            new Opcode(0x59, "ld e, c"             , 1, (c,i) => {c.E = c.C; return 4;}),
            new Opcode(0x5A, "ld e, d"             , 1, (c,i) => {c.E = c.D; return 4;}),
            new Opcode(0x5B, "ld e, e"             , 1, (c,i) => {c.E = c.E; return 4;}),
            new Opcode(0x5C, "ld e, h"             , 1, (c,i) => {c.E = c.H; return 4;}),
            new Opcode(0x5D, "ld e, l"             , 1, (c,i) => {c.E = c.L; return 4;}),
            new Opcode(0x5E, "ld e, (HL)"          , 1, (c,i) => {c.E = c.mem.Read8(c.HL); return 8;}),
            new Opcode(0x5F, "ld e, a"             , 1, (c,i) => {c.E = c.A; return 4;}),
            // 0x60
            new Opcode(0x60, "ld h, b"             , 1, (c,i) => {c.H = c.B; return 4;}),
            new Opcode(0x61, "ld h, c"             , 1, (c,i) => {c.H = c.C; return 4;}),
            new Opcode(0x62, "ld h, d"             , 1, (c,i) => {c.H = c.D; return 4;}),
            new Opcode(0x63, "ld h, e"             , 1, (c,i) => {c.H = c.E; return 4;}),
            new Opcode(0x64, "ld h, h"             , 1, (c,i) => {c.H = c.H; return 4;}),
            new Opcode(0x65, "ld h, l"             , 1, (c,i) => {c.H = c.L; return 4;}),
            new Opcode(0x66, "ld h, (HL)"          , 1, (c,i) => {c.H = c.mem.Read8(c.HL); return 8;}),
            new Opcode(0x67, "ld h, a"             , 1, (c,i) => {c.H = c.A; return 4;}),
            new Opcode(0x68, "ld l, b"             , 1, (c,i) => {c.L = c.B; return 4;}),
            new Opcode(0x69, "ld l, c"             , 1, (c,i) => {c.L = c.C; return 4;}),
            new Opcode(0x6A, "ld l, d"             , 1, (c,i) => {c.L = c.D; return 4;}),
            new Opcode(0x6B, "ld l, e"             , 1, (c,i) => {c.L = c.E; return 4;}),
            new Opcode(0x6C, "ld l, h"             , 1, (c,i) => {c.L = c.H; return 4;}),
            new Opcode(0x6D, "ld l, l"             , 1, (c,i) => {c.L = c.L; return 4;}),
            new Opcode(0x6E, "ld l, (HL)"          , 1, (c,i) => {c.L = c.mem.Read8(c.HL); return 8;}),
            new Opcode(0x6F, "ld l, a"             , 1, (c,i) => {c.L = c.A; return 4;}),
            // 0x70
            new Opcode(0x70, "ld (HL), b"          , 1, (c,i) => {c.mem.Write8(c.HL, c.B); return 8;}),
            new Opcode(0x71, "ld (HL), c"          , 1, (c,i) => {c.mem.Write8(c.HL, c.C); return 8;}),
            new Opcode(0x72, "ld (HL), d"          , 1, (c,i) => {c.mem.Write8(c.HL, c.D); return 8;}),
            new Opcode(0x73, "ld (HL), e"          , 1, (c,i) => {c.mem.Write8(c.HL, c.E); return 8;}),
            new Opcode(0x74, "ld (HL), h"          , 1, (c,i) => {c.mem.Write8(c.HL, c.H); return 8;}),
            new Opcode(0x75, "ld (HL), l"          , 1, (c,i) => {c.mem.Write8(c.HL, c.L); return 8;}),
            new Opcode(0x76, "halt"                , 1, (c,i) => {c.Halt(); return 4;}),
            new Opcode(0x77, "ld (HL), a"          , 1, (c,i) => {c.mem.Write8(c.HL, c.A); return 8;}),
            new Opcode(0x78, "ld a, b"             , 1, (c,i) => {c.A = c.B; return 4;}),
            new Opcode(0x79, "ld a, c"             , 1, (c,i) => {c.A = c.C; return 4;}),
            new Opcode(0x7A, "ld a, d"             , 1, (c,i) => {c.A = c.D; return 4;}),
            new Opcode(0x7B, "ld a, e"             , 1, (c,i) => {c.A = c.E; return 4;}),
            new Opcode(0x7C, "ld a, h"             , 1, (c,i) => {c.A = c.H; return 4;}),
            new Opcode(0x7D, "ld a, l"             , 1, (c,i) => {c.A = c.L; return 4;}),
            new Opcode(0x7E, "ld a, (HL)"          , 1, (c,i) => {c.A = c.mem.Read8(c.HL); return 8;}),
            new Opcode(0x7F, "ld a, a"             , 1, (c,i) => {c.A = c.A; return 4;}),
            // 0x80
            new Opcode(0x80, "add a, b"            , 1, (c,i) => {c.A = c.alu.Add(c.A, c.B, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x81, "add a, c"            , 1, (c,i) => {c.A = c.alu.Add(c.A, c.C, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x82, "add a, d"            , 1, (c,i) => {c.A = c.alu.Add(c.A, c.D, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x83, "add a, e"            , 1, (c,i) => {c.A = c.alu.Add(c.A, c.E, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x84, "add a, h"            , 1, (c,i) => {c.A = c.alu.Add(c.A, c.H, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x85, "add a, l"            , 1, (c,i) => {c.A = c.alu.Add(c.A, c.L, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x86, "add a, (HL)"         , 1, (c,i) => {c.A = c.alu.Add(c.A, c.mem.Read8(c.HL),RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 8;}),
            new Opcode(0x87, "add a, a"            , 1, (c,i) => {c.A = c.alu.Add(c.A, c.A, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x88, "adc a, b"            , 1, (c,i) => {c.A = c.alu.Adc(c.A, c.B, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x89, "adc a, c"            , 1, (c,i) => {c.A = c.alu.Adc(c.A, c.C, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x8A, "adc a, d"            , 1, (c,i) => {c.A = c.alu.Adc(c.A, c.D, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x8B, "adc a, e"            , 1, (c,i) => {c.A = c.alu.Adc(c.A, c.E, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x8C, "adc a, h"            , 1, (c,i) => {c.A = c.alu.Adc(c.A, c.H, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x8D, "adc a, l"            , 1, (c,i) => {c.A = c.alu.Adc(c.A, c.L, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x8E, "adc a, (HL)"         , 1, (c,i) => {c.A = c.alu.Adc(c.A, c.mem.Read8(c.HL),RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 8;}),
            new Opcode(0x8F, "adc a, a"            , 1, (c,i) => {c.A = c.alu.Adc(c.A, c.A,RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            // 0x90
            new Opcode(0x90, "sub a, b"            , 1, (c,i) => {c.A = c.alu.Sub(c.A, c.B, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x91, "sub a, c"            , 1, (c,i) => {c.A = c.alu.Sub(c.A, c.C, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x92, "sub a, d"            , 1, (c,i) => {c.A = c.alu.Sub(c.A, c.D, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x93, "sub a, e"            , 1, (c,i) => {c.A = c.alu.Sub(c.A, c.E, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x94, "sub a, h"            , 1, (c,i) => {c.A = c.alu.Sub(c.A, c.H, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x95, "sub a, l"            , 1, (c,i) => {c.A = c.alu.Sub(c.A, c.L, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x96, "sub a, (HL)"         , 1, (c,i) => {c.A = c.alu.Sub(c.A, c.mem.Read8(c.HL),RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 8;}),
            new Opcode(0x97, "sub a, a"            , 1, (c,i) => {c.A = c.alu.Sub(c.A, c.A, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),
            new Opcode(0x98, "sbc a, b"            , 1, (c,i) => {c.A = c.alu.Sbc(c.A, c.B, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x99, "sbc a, c"            , 1, (c,i) => {c.A = c.alu.Sbc(c.A, c.C, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x9A, "sbc a, d"            , 1, (c,i) => {c.A = c.alu.Sbc(c.A, c.D, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x9B, "sbc a, e"            , 1, (c,i) => {c.A = c.alu.Sbc(c.A, c.E, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x9C, "sbc a, h"            , 1, (c,i) => {c.A = c.alu.Sbc(c.A, c.H, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x9D, "sbc a, l"            , 1, (c,i) => {c.A = c.alu.Sbc(c.A, c.L, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4;}),
            new Opcode(0x9E, "sbc a, (HL)"         , 1, (c,i) => {c.A = c.alu.Sbc(c.A, c.mem.Read8(c.HL),RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 8;}),
            new Opcode(0x9F, "sbc a, a"            , 1, (c,i) => {c.A = c.alu.Sbc(c.A, c.A, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 4;}),     
            // 0xA0
            new Opcode(0xA0, "and b"               , 1, (c,i) => {c.A = c.alu.And(c.B); return 4;}),
            new Opcode(0xA1, "and c"               , 1, (c,i) => {c.A = c.alu.And(c.C); return 4;}),
            new Opcode(0xA2, "and d"               , 1, (c,i) => {c.A = c.alu.And(c.D); return 4;}),
            new Opcode(0xA3, "and e"               , 1, (c,i) => {c.A = c.alu.And(c.E); return 4;}),
            new Opcode(0xA4, "and h"               , 1, (c,i) => {c.A = c.alu.And(c.H); return 4;}),
            new Opcode(0xA5, "and l"               , 1, (c,i) => {c.A = c.alu.And(c.L); return 4;}),
            new Opcode(0xA6, "and (HL)"            , 1, (c,i) => {c.A = c.alu.And(c.mem.Read8(c.HL)); return 8;}),
            new Opcode(0xA7, "and a"               , 1, (c,i) => {c.A = c.alu.And(c.A); return 4;}),
            new Opcode(0xA8, "xor b"               , 1, (c,i) => {c.A = c.alu.Xor(c.B); return 4;}),
            new Opcode(0xA9, "xor c"               , 1, (c,i) => {c.A = c.alu.Xor(c.C); return 4;}),
            new Opcode(0xAA, "xor d"               , 1, (c,i) => {c.A = c.alu.Xor(c.D); return 4;}),
            new Opcode(0xAB, "xor e"               , 1, (c,i) => {c.A = c.alu.Xor(c.E); return 4;}),
            new Opcode(0xAC, "xor h"               , 1, (c,i) => {c.A = c.alu.Xor(c.H); return 4;}),
            new Opcode(0xAD, "xor l"               , 1, (c,i) => {c.A = c.alu.Xor(c.L); return 4;}),
            new Opcode(0xAE, "xor (HL)"            , 1, (c,i) => {c.A = c.alu.Xor(c.mem.Read8(c.HL)); return 8;}),
            new Opcode(0xAF, "xor a"               , 1, (c,i) => {c.A = c.alu.Xor(c.A); return 4;}),
            // 0xB0
            new Opcode(0xB0, "or b"                , 1, (c,i) => {c.A = c.alu.Or(c.B); return 4;}),
            new Opcode(0xB1, "or c"                , 1, (c,i) => {c.A = c.alu.Or(c.C); return 4;}),
            new Opcode(0xB2, "or d"                , 1, (c,i) => {c.A = c.alu.Or(c.D); return 4;}),
            new Opcode(0xB3, "or e"                , 1, (c,i) => {c.A = c.alu.Or(c.E); return 4;}),
            new Opcode(0xB4, "or h"                , 1, (c,i) => {c.A = c.alu.Or(c.H); return 4;}),
            new Opcode(0xB5, "or l"                , 1, (c,i) => {c.A = c.alu.Or(c.L); return 4;}),
            new Opcode(0xB6, "or (HL)"             , 1, (c,i) => {c.A = c.alu.Or(c.mem.Read8(c.HL)); return 8;}),
            new Opcode(0xB7, "or a"                , 1, (c,i) => {c.A = c.alu.Or(c.A); return 4;}),
            new Opcode(0xB8, "cp b"                , 1, (c,i) => {c.alu.Compare(c.B, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4; }),
            new Opcode(0xB9, "cp c"                , 1, (c,i) => {c.alu.Compare(c.C, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4; }),
            new Opcode(0xBA, "cp d"                , 1, (c,i) => {c.alu.Compare(c.D, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4; }),
            new Opcode(0xBB, "cp e"                , 1, (c,i) => {c.alu.Compare(c.E, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4; }),
            new Opcode(0xBC, "cp h"                , 1, (c,i) => {c.alu.Compare(c.H, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4; }),
            new Opcode(0xBD, "cp l"                , 1, (c,i) => {c.alu.Compare(c.L, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4; }),
            new Opcode(0xBE, "cp (HL)"             , 1, (c,i) => {c.alu.Compare(c.mem.Read8(c.HL), RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 8; }),
            new Opcode(0xBF, "cp A"                , 1, (c,i) => {c.alu.Compare(c.A, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 4; }),
            // 0xC0
            new Opcode(0xC0, "ret nz"              , 1, (c,i) => {return c.RetCondition(c.FlagZero == false); }),
            new Opcode(0xC1, "pop bc"              , 1, (c,i) => {c.BC = c.Pop16(); return 12; }),
            new Opcode(0xC2, "jp nz {0:X4}"        , 3, (c,i) => {return c.OpJumpConditional(i, c.FlagZero == false); }),
            new Opcode(0xC3, "jp {0:X4}"           , 3, (c,i) => {c.PC = i.Operand16; return 16; }),
            new Opcode(0xC4, "call nz {0:X4}"      , 3, (c,i) => {return c.OpCallConditional(i.Operand16, c.FlagZero == false); }),
            new Opcode(0xC5, "push bc"             , 1, (c,i) => {c.Push16(c.BC); return 16; }),
            new Opcode(0xC6, "add a, {0:X2}"       , 2, (c,i) => {c.A = c.alu.Add(c.A, i.Operand8, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 8;}),
            new Opcode(0xC7, "rst $00"             , 1, (c,i) => {c.Call(0x00); return 16; }),
            new Opcode(0xC8, "ret z"               , 1, (c,i) => {return c.RetCondition(c.FlagZero == true); }),
            new Opcode(0xC9, "ret"                 , 1, (c,i) => {c.Ret(); return 16; }),
            new Opcode(0xCA, "jp z {0:X4}"         , 3, (c,i) => {return c.OpJumpConditional(i, c.FlagZero == true); }),
            new Opcode(0xCB, "CB prefix"           , 2, OpUnimplemented),
            new Opcode(0xCC, "call z {0:X4}"       , 3, (c,i) => {return c.OpCallConditional(i.Operand16, c.FlagZero == true); }),
            new Opcode(0xCD, "call {0:X4}"         , 3, (c,i) => {c.Call(i.Operand16); return 24; }),
            new Opcode(0xCE, "adc a, {0:X2}"       , 2, (c,i) => {c.A = c.alu.Adc((byte)c.A, (byte)i.Operand8, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.N); return 8;}),
            new Opcode(0xCF, "rst $08"             , 1, (c,i) => {c.Call(0x08); return 16; }),
            // 0xD0
            new Opcode(0xD0, "ret nc"              , 1, (c,i) => {return c.RetCondition(c.FlagCarry == false); }),
            new Opcode(0xD1, "pop de"              , 1, (c,i) => {c.DE = c.Pop16(); return 12; }),
            new Opcode(0xD2, "jp nc {0:X4}"        , 3, (c,i) => {return c.OpJumpConditional(i, c.FlagCarry == false); }),
            new Opcode(0xD3, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xD4, "call nc {0:X4}"      , 3, (c,i) => {return c.OpCallConditional(i.Operand16, c.FlagCarry == false); }),
            new Opcode(0xD5, "push de"             , 1, (c,i) => {c.Push16(c.DE); return 16; }),
            new Opcode(0xD6, "sub a, {0:X2}"       , 2, (c,i) => {c.A = c.alu.Sub(c.A, i.Operand8, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 8;}),
            new Opcode(0xD7, "rst $10"             , 1, (c,i) => {c.Call(0x10); return 16; }),
            new Opcode(0xD8, "ret c"               , 1, (c,i) => {return c.RetCondition(c.FlagCarry == true); }),
            new Opcode(0xD9, "reti"                , 1, (c,i) => {c.Ret(); c.regs.IME = true; return 16;  }),
            new Opcode(0xDA, "jp c {0:X4}"         , 3, (c,i) => {return c.OpJumpConditional(i, c.FlagCarry == true); }),
            new Opcode(0xDB, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xDC, "call c {0:X4}"       , 3, (c,i) => {return c.OpCallConditional(i.Operand16, c.FlagCarry == true); }),
            new Opcode(0xDD, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xDE, "sbc a, {0:X2}"       , 2, (c,i) => {c.A = c.alu.Adc(c.A, i.Operand8, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 8;}),
            new Opcode(0xDF, "rst $18"             , 1, (c,i) => {c.Call(0x18); return 16; }),
            // 0xE0
            new Opcode(0xE0, "ldh $FF00+{0:X2}, a" , 2, (c,i) => {c.mem.Write8((ushort)(0xFF00 + i.Operand8), c.A); return 12; }),
            new Opcode(0xE1, "pop hl"              , 1, (c,i) => {c.HL = c.Pop16(); return 12; }),
            new Opcode(0xE2, "ldh (c), a"          , 1, (c,i) => {c.mem.Write8((ushort)(0xFF00 + c.C), c.A); return 8; }),
            new Opcode(0xE3, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xE4, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xE5, "push hl"             , 1, (c,i) => {c.Push16(c.HL); return 16; }),
            new Opcode(0xE6, "and {0:X2}"          , 2, (c,i) => {c.A = c.alu.And(i.Operand8); return 4;}),
            new Opcode(0xE7, "rst $20"             , 1, (c,i) => {c.Call(0x20); return 16; }),
            new Opcode(0xE8, "add sp, {0:X2}"      , 2, (c,i) => {c.SP = c.alu.Add(c.SP, (sbyte)i.Operand8, RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.Z | RegFlags.N); return 16;}),
            new Opcode(0xE9, "jp HL"               , 1, (c,i) => {c.PC = c.HL; return 4; }),
            new Opcode(0xEA, "ld ({0:x4}), a"      , 3, (c,i) => {c.mem.Write8(i.Operand16, c.A); return 16; }),
            new Opcode(0xEB, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xEC, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xED, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xEE, "xor {0:X2}"          , 2, (c,i) => {c.A = c.alu.Xor(i.Operand8); return 8;}),
            new Opcode(0xEF, "rst $28"             , 1, (c,i) => {c.Call(0x28); return 16; }),
            // 0xF0
            new Opcode(0xF0, "ldh a, $FF00+{0:X2}" , 2, (c,i) => {c.A = c.mem.Read8((ushort)(0xFF00 + i.Operand8)); return 12; }),
            new Opcode(0xF1, "pop af"              , 1, (c,i) => {c.AF = c.Pop16(); c.Flags &= 0xF0; return 12; }),
            new Opcode(0xF2, "ldh a, (c)"          , 1, (c,i) => {c.A = c.mem.Read8((ushort)(0xFF00 + c.C)); return 8; }),
            new Opcode(0xF3, "di"                  , 1, (c,i) => {c.regs.IME = false; return 4; }),
            new Opcode(0xF4, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xF5, "push af"             , 1, (c,i) => {c.Push16(c.AF); return 16; }),
            new Opcode(0xF6, "or {0:X2}"           , 2, (c,i) => {c.A = c.alu.Or(i.Operand8); return 8;}),
            new Opcode(0xF7, "rst $30"             , 1, (c,i) => {c.Call(0x30); return 16; }),
            new Opcode(0xF8, "ld hl, sp+r8"        , 2, (c,i) => {c.HL = c.alu.Add(c.SP,(sbyte)i.Operand8, RegFlags.H | RegFlags.C, RegFlags.None, RegFlags.Z | RegFlags.N); return 12; }),
            new Opcode(0xF9, "ld sp, hl"           , 1, (c,i) => {c.SP = c.HL; return 8; }),
            new Opcode(0xFA, "ld a, ({0:X4})"      , 3, (c,i) => {c.A = c.mem.Read8(i.Operand16); return 16; }),
            new Opcode(0xFB, "ei"                  , 1, (c,i) => {c.regs.IME = true; return 4; }),
            new Opcode(0xFC, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xFD, "unimplemented"       , 1, OpUnimplemented),
            new Opcode(0xFE, "cp {0:X2}"           , 2, (c,i) => {c.alu.Compare(i.Operand8, RegFlags.Z | RegFlags.H | RegFlags.C, RegFlags.N); return 8; }),
            new Opcode(0xFF, "rst $38"             , 1, (c,i) => {c.Call(0x38); return 16; }),
        };

        public static readonly Opcode[] OpCodesPrefix =
        {
            //        Opcode    Dissassembly      Length     Instruction Body
            // 0x00
            new Opcode(0x00, "rlc b"               , 2, (c,i) => {c.B = c.alu.Rlc(c.B, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x01, "rlc c"               , 2, (c,i) => {c.C = c.alu.Rlc(c.C, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x02, "rlc d"               , 2, (c,i) => {c.D = c.alu.Rlc(c.D, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x03, "rlc e"               , 2, (c,i) => {c.E = c.alu.Rlc(c.E, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x04, "rlc h"               , 2, (c,i) => {c.H = c.alu.Rlc(c.H, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x05, "rlc l"               , 2, (c,i) => {c.L = c.alu.Rlc(c.L, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x06, "rlc (hl)"            , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Rlc(c.mem.Read8(c.HL), RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H)); return 16; }),
            new Opcode(0x07, "rlc a"               , 2, (c,i) => {c.A = c.alu.Rlc(c.A, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x08, "rrc b"               , 2, (c,i) => {c.B = c.alu.Rrc(c.B, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x09, "rrc c"               , 2, (c,i) => {c.C = c.alu.Rrc(c.C, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x0A, "rrc d"               , 2, (c,i) => {c.D = c.alu.Rrc(c.D, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x0B, "rrc e"               , 2, (c,i) => {c.E = c.alu.Rrc(c.E, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x0C, "rrc h"               , 2, (c,i) => {c.H = c.alu.Rrc(c.H, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x0D, "rrc l"               , 2, (c,i) => {c.L = c.alu.Rrc(c.L, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x0E, "rrc (hl)"            , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Rrc(c.mem.Read8(c.HL), RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H)); return 16; }),
            new Opcode(0x0F, "rrc a"               , 2, (c,i) => {c.A = c.alu.Rrc(c.A, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            // 0x10
            new Opcode(0x10, "rl b"                , 2, (c,i) => {c.B = c.alu.Rl(c.B, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x11, "rl c"                , 2, (c,i) => {c.C = c.alu.Rl(c.C, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x12, "rl d"                , 2, (c,i) => {c.D = c.alu.Rl(c.D, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x13, "rl e"                , 2, (c,i) => {c.E = c.alu.Rl(c.E, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x14, "rl h"                , 2, (c,i) => {c.H = c.alu.Rl(c.H, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x15, "rl l"                , 2, (c,i) => {c.L = c.alu.Rl(c.L, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x16, "rl (hl)"             , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Rl(c.mem.Read8(c.HL), RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H)); return 16; }),
            new Opcode(0x17, "rl a"                , 2, (c,i) => {c.A = c.alu.Rl(c.A, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x18, "rr b"                , 2, (c,i) => {c.B = c.alu.Rr(c.B, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x19, "rr c"                , 2, (c,i) => {c.C = c.alu.Rr(c.C, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x1A, "rr d"                , 2, (c,i) => {c.D = c.alu.Rr(c.D, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x1B, "rr e"                , 2, (c,i) => {c.E = c.alu.Rr(c.E, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x1C, "rr h"                , 2, (c,i) => {c.H = c.alu.Rr(c.H, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x1D, "rr l"                , 2, (c,i) => {c.L = c.alu.Rr(c.L, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            new Opcode(0x1E, "rr (hl)"             , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Rr(c.mem.Read8(c.HL), RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H)); return 16; }),
            new Opcode(0x1F, "rr a"                , 2, (c,i) => {c.A = c.alu.Rr(c.A, RegFlags.Z | RegFlags.C, RegFlags.None, RegFlags.N | RegFlags.H); return 8; }),
            // 0x20
            new Opcode(0x20, "sla b"               , 2, (c,i) => {c.B = c.alu.Sla(c.B); return 8; }),
            new Opcode(0x21, "sla c"               , 2, (c,i) => {c.C = c.alu.Sla(c.C); return 8; }),
            new Opcode(0x22, "sla d"               , 2, (c,i) => {c.D = c.alu.Sla(c.D); return 8; }),
            new Opcode(0x23, "sla e"               , 2, (c,i) => {c.E = c.alu.Sla(c.E); return 8; }),
            new Opcode(0x24, "sla h"               , 2, (c,i) => {c.H = c.alu.Sla(c.H); return 8; }),
            new Opcode(0x25, "sla l"               , 2, (c,i) => {c.L = c.alu.Sla(c.L); return 8; }),
            new Opcode(0x26, "sla (hl)"            , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Sla(c.mem.Read8(c.HL))); return 16; }),
            new Opcode(0x27, "sla a"               , 2, (c,i) => {c.A = c.alu.Sla(c.A); return 8; }),
            new Opcode(0x28, "sra b"               , 2, (c,i) => {c.B = c.alu.Sr(c.B,false); return 8; }),
            new Opcode(0x29, "sra c"               , 2, (c,i) => {c.C = c.alu.Sr(c.C,false); return 8; }),
            new Opcode(0x2A, "sra d"               , 2, (c,i) => {c.D = c.alu.Sr(c.D,false); return 8; }),
            new Opcode(0x2B, "sra e"               , 2, (c,i) => {c.E = c.alu.Sr(c.E,false); return 8; }),
            new Opcode(0x2C, "sra h"               , 2, (c,i) => {c.H = c.alu.Sr(c.H,false); return 8; }),
            new Opcode(0x2D, "sra l"               , 2, (c,i) => {c.L = c.alu.Sr(c.L,false); return 8; }),
            new Opcode(0x2E, "sra (hl)"            , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Sr(c.mem.Read8(c.HL), false)); return 16; }),
            new Opcode(0x2F, "sra a"               , 2, (c,i) => {c.A = c.alu.Sr(c.A, false); return 8; }),
            // 0x30
            new Opcode(0x30, "swap b"              , 2, (c,i) => {c.B = c.alu.Swap(c.B); return 8; }),
            new Opcode(0x31, "swap c"              , 2, (c,i) => {c.C = c.alu.Swap(c.C); return 8; }),
            new Opcode(0x32, "swap d"              , 2, (c,i) => {c.D = c.alu.Swap(c.D); return 8; }),
            new Opcode(0x33, "swap e"              , 2, (c,i) => {c.E = c.alu.Swap(c.E); return 8; }),
            new Opcode(0x34, "swap h"              , 2, (c,i) => {c.H = c.alu.Swap(c.H); return 8; }),
            new Opcode(0x35, "swap l"              , 2, (c,i) => {c.L = c.alu.Swap(c.L); return 8; }),
            new Opcode(0x36, "swap (hl)"           , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Swap(c.mem.Read8(c.HL))); return 16; }),
            new Opcode(0x37, "swap a"              , 2, (c,i) => {c.A = c.alu.Swap(c.A); return 8; }),
            new Opcode(0x38, "srl b"               , 2, (c,i) => {c.B = c.alu.Sr(c.B,true); return 8; }),
            new Opcode(0x39, "srl c"               , 2, (c,i) => {c.C = c.alu.Sr(c.C,true); return 8; }),
            new Opcode(0x3A, "srl d"               , 2, (c,i) => {c.D = c.alu.Sr(c.D,true); return 8; }),
            new Opcode(0x3B, "srl e"               , 2, (c,i) => {c.E = c.alu.Sr(c.E,true); return 8; }),
            new Opcode(0x3C, "srl h"               , 2, (c,i) => {c.H = c.alu.Sr(c.H,true); return 8; }),
            new Opcode(0x3D, "srl l"               , 2, (c,i) => {c.L = c.alu.Sr(c.L,true); return 8; }),
            new Opcode(0x3E, "srl (hl)"            , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Sr(c.mem.Read8(c.HL), true)); return 16; }),
            new Opcode(0x3F, "srl a"               , 2, (c,i) => {c.A = c.alu.Sr(c.A, true); return 8; }),
            // 0x40
            new Opcode(0x40, "bit 0, b"            , 2, (c,i) => {c.alu.Bit(c.B, 0); return 8; }),
            new Opcode(0x41, "bit 0, c"            , 2, (c,i) => {c.alu.Bit(c.C, 0); return 8; }),
            new Opcode(0x42, "bit 0, d"            , 2, (c,i) => {c.alu.Bit(c.D, 0); return 8; }),
            new Opcode(0x43, "bit 0, e"            , 2, (c,i) => {c.alu.Bit(c.E, 0); return 8; }),
            new Opcode(0x44, "bit 0, h"            , 2, (c,i) => {c.alu.Bit(c.H, 0); return 8; }),
            new Opcode(0x45, "bit 0, l"            , 2, (c,i) => {c.alu.Bit(c.L, 0); return 8; }),
            new Opcode(0x46, "bit 0, (hl)"         , 2, (c,i) => {c.alu.Bit(c.mem.Read8(c.HL), 0); return 16; }),
            new Opcode(0x47, "bit 0, a"            , 2, (c,i) => {c.alu.Bit(c.A, 0);  return 8; }),
            new Opcode(0x48, "bit 1, b"            , 2, (c,i) => {c.alu.Bit(c.B, 1); return 8; }),
            new Opcode(0x49, "bit 1, c"            , 2, (c,i) => {c.alu.Bit(c.C, 1); return 8; }),
            new Opcode(0x4A, "bit 1, d"            , 2, (c,i) => {c.alu.Bit(c.D, 1); return 8; }),
            new Opcode(0x4B, "bit 1, e"            , 2, (c,i) => {c.alu.Bit(c.E, 1); return 8; }),
            new Opcode(0x4C, "bit 1, h"            , 2, (c,i) => {c.alu.Bit(c.H, 1); return 8; }),
            new Opcode(0x4D, "bit 1, l"            , 2, (c,i) => {c.alu.Bit(c.L, 1); return 8; }),
            new Opcode(0x4E, "bit 1, (hl)"         , 2, (c,i) => {c.alu.Bit(c.mem.Read8(c.HL), 1); return 16; }),
            new Opcode(0x4F, "bit 1, a"            , 2, (c,i) => {c.alu.Bit(c.A, 1);  return 8; }),
            // 0x90
            new Opcode(0x50, "bit 2, b"            , 2, (c,i) => {c.alu.Bit(c.B, 2); return 8; }),
            new Opcode(0x51, "bit 2, c"            , 2, (c,i) => {c.alu.Bit(c.C, 2); return 8; }),
            new Opcode(0x52, "bit 2, d"            , 2, (c,i) => {c.alu.Bit(c.D, 2); return 8; }),
            new Opcode(0x53, "bit 2, e"            , 2, (c,i) => {c.alu.Bit(c.E, 2); return 8; }),
            new Opcode(0x54, "bit 2, h"            , 2, (c,i) => {c.alu.Bit(c.H, 2); return 8; }),
            new Opcode(0x55, "bit 2, l"            , 2, (c,i) => {c.alu.Bit(c.L, 2); return 8; }),
            new Opcode(0x56, "bit 2, (hl)"         , 2, (c,i) => {c.alu.Bit(c.mem.Read8(c.HL), 2); return 16; }),
            new Opcode(0x57, "bit 2, a"            , 2, (c,i) => {c.alu.Bit(c.A, 2);  return 8; }),
            new Opcode(0x58, "bit 3, b"            , 2, (c,i) => {c.alu.Bit(c.B, 3); return 8; }),
            new Opcode(0x59, "bit 3, c"            , 2, (c,i) => {c.alu.Bit(c.C, 3); return 8; }),
            new Opcode(0x5A, "bit 3, d"            , 2, (c,i) => {c.alu.Bit(c.D, 3); return 8; }),
            new Opcode(0x5B, "bit 3, e"            , 2, (c,i) => {c.alu.Bit(c.E, 3); return 8; }),
            new Opcode(0x5C, "bit 3, h"            , 2, (c,i) => {c.alu.Bit(c.H, 3); return 8; }),
            new Opcode(0x5D, "bit 3, l"            , 2, (c,i) => {c.alu.Bit(c.L, 3); return 8; }),
            new Opcode(0x5E, "bit 3, (hl)"         , 2, (c,i) => {c.alu.Bit(c.mem.Read8(c.HL), 3); return 16; }),
            new Opcode(0x5F, "bit 3, a"            , 2, (c,i) => {c.alu.Bit(c.A, 3);  return 8; }),
            // 0xA0      
            new Opcode(0x60, "bit 4, b"            , 2, (c,i) => {c.alu.Bit(c.B, 4); return 8; }),
            new Opcode(0x61, "bit 4, c"            , 2, (c,i) => {c.alu.Bit(c.C, 4); return 8; }),
            new Opcode(0x62, "bit 4, d"            , 2, (c,i) => {c.alu.Bit(c.D, 4); return 8; }),
            new Opcode(0x63, "bit 4, e"            , 2, (c,i) => {c.alu.Bit(c.E, 4); return 8; }),
            new Opcode(0x64, "bit 4, h"            , 2, (c,i) => {c.alu.Bit(c.H, 4); return 8; }),
            new Opcode(0x65, "bit 4, l"            , 2, (c,i) => {c.alu.Bit(c.L, 4); return 8; }),
            new Opcode(0x66, "bit 4, (hl)"         , 2, (c,i) => {c.alu.Bit(c.mem.Read8(c.HL), 4); return 16; }),
            new Opcode(0x67, "bit 4, a"            , 2, (c,i) => {c.alu.Bit(c.A, 4);  return 8; }),
            new Opcode(0x68, "bit 5, b"            , 2, (c,i) => {c.alu.Bit(c.B, 5); return 8; }),
            new Opcode(0x69, "bit 5, c"            , 2, (c,i) => {c.alu.Bit(c.C, 5); return 8; }),
            new Opcode(0x6A, "bit 5, d"            , 2, (c,i) => {c.alu.Bit(c.D, 5); return 8; }),
            new Opcode(0x6B, "bit 5, e"            , 2, (c,i) => {c.alu.Bit(c.E, 5); return 8; }),
            new Opcode(0x6C, "bit 5, h"            , 2, (c,i) => {c.alu.Bit(c.H, 5); return 8; }),
            new Opcode(0x6D, "bit 5, l"            , 2, (c,i) => {c.alu.Bit(c.L, 5); return 8; }),
            new Opcode(0x6E, "bit 5, (hl)"         , 2, (c,i) => {c.alu.Bit(c.mem.Read8(c.HL), 5); return 16; }),
            new Opcode(0x6F, "bit 5, a"            , 2, (c,i) => {c.alu.Bit(c.A, 5);  return 8; }),
            // 0xB0
            new Opcode(0x70, "bit 6, b"            , 2, (c,i) => {c.alu.Bit(c.B, 6); return 8; }),
            new Opcode(0x71, "bit 6, c"            , 2, (c,i) => {c.alu.Bit(c.C, 6); return 8; }),
            new Opcode(0x72, "bit 6, d"            , 2, (c,i) => {c.alu.Bit(c.D, 6); return 8; }),
            new Opcode(0x73, "bit 6, e"            , 2, (c,i) => {c.alu.Bit(c.E, 6); return 8; }),
            new Opcode(0x74, "bit 6, h"            , 2, (c,i) => {c.alu.Bit(c.H, 6); return 8; }),
            new Opcode(0x75, "bit 6, l"            , 2, (c,i) => {c.alu.Bit(c.L, 6); return 8; }),
            new Opcode(0x76, "bit 6, (hl)"         , 2, (c,i) => {c.alu.Bit(c.mem.Read8(c.HL), 6); return 16; }),
            new Opcode(0x77, "bit 6, a"            , 2, (c,i) => {c.alu.Bit(c.A, 6);  return 8; }),
            new Opcode(0x78, "bit 7, b"            , 2, (c,i) => {c.alu.Bit(c.B, 7); return 8; }),
            new Opcode(0x79, "bit 7, c"            , 2, (c,i) => {c.alu.Bit(c.C, 7); return 8; }),
            new Opcode(0x7A, "bit 7, d"            , 2, (c,i) => {c.alu.Bit(c.D, 7); return 8; }),
            new Opcode(0x7B, "bit 7, e"            , 2, (c,i) => {c.alu.Bit(c.E, 7); return 8; }),
            new Opcode(0x7C, "bit 7, h"            , 2, (c,i) => {c.alu.Bit(c.H, 7); return 8; }),
            new Opcode(0x7D, "bit 7, l"            , 2, (c,i) => {c.alu.Bit(c.L, 7); return 8; }),
            new Opcode(0x7E, "bit 7, (hl)"         , 2, (c,i) => {c.alu.Bit(c.mem.Read8(c.HL), 7); return 16; }),
            new Opcode(0x7F, "bit 7, a"            , 2, (c,i) => {c.alu.Bit(c.A, 7);  return 8; }),
            // 0x80
            new Opcode(0x80, "res 0, b"            , 2, (c,i) => {c.B = c.alu.Res(c.B, 0); return 8; }),
            new Opcode(0x81, "res 0, c"            , 2, (c,i) => {c.C = c.alu.Res(c.C, 0); return 8; }),
            new Opcode(0x82, "res 0, d"            , 2, (c,i) => {c.D = c.alu.Res(c.D, 0); return 8; }),
            new Opcode(0x83, "res 0, e"            , 2, (c,i) => {c.E = c.alu.Res(c.E, 0); return 8; }),
            new Opcode(0x84, "res 0, h"            , 2, (c,i) => {c.H = c.alu.Res(c.H, 0); return 8; }),
            new Opcode(0x85, "res 0, l"            , 2, (c,i) => {c.L = c.alu.Res(c.L, 0); return 8; }),
            new Opcode(0x86, "res 0, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Res(c.mem.Read8(c.HL), 0)); return 16; }),
            new Opcode(0x87, "res 0, a"            , 2, (c,i) => {c.A = c.alu.Res(c.A, 0);  return 8; }),
            new Opcode(0x88, "res 1, b"            , 2, (c,i) => {c.B = c.alu.Res(c.B, 1); return 8; }),
            new Opcode(0x89, "res 1, c"            , 2, (c,i) => {c.C = c.alu.Res(c.C, 1); return 8; }),
            new Opcode(0x8A, "res 1, d"            , 2, (c,i) => {c.D = c.alu.Res(c.D, 1); return 8; }),
            new Opcode(0x8B, "res 1, e"            , 2, (c,i) => {c.E = c.alu.Res(c.E, 1); return 8; }),
            new Opcode(0x8C, "res 1, h"            , 2, (c,i) => {c.H = c.alu.Res(c.H, 1); return 8; }),
            new Opcode(0x8D, "res 1, l"            , 2, (c,i) => {c.L = c.alu.Res(c.L, 1); return 8; }),
            new Opcode(0x8E, "res 1, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Res(c.mem.Read8(c.HL), 1)); return 16; }),
            new Opcode(0x8F, "res 1, a"            , 2, (c,i) => {c.A = c.alu.Res(c.A, 1);  return 8; }),
            // 0x90
            new Opcode(0x90, "res 2, b"            , 2, (c,i) => {c.B = c.alu.Res(c.B, 2); return 8; }),
            new Opcode(0x91, "res 2, c"            , 2, (c,i) => {c.C = c.alu.Res(c.C, 2); return 8; }),
            new Opcode(0x92, "res 2, d"            , 2, (c,i) => {c.D = c.alu.Res(c.D, 2); return 8; }),
            new Opcode(0x93, "res 2, e"            , 2, (c,i) => {c.E = c.alu.Res(c.E, 2); return 8; }),
            new Opcode(0x94, "res 2, h"            , 2, (c,i) => {c.H = c.alu.Res(c.H, 2); return 8; }),
            new Opcode(0x95, "res 2, l"            , 2, (c,i) => {c.L = c.alu.Res(c.L, 2); return 8; }),
            new Opcode(0x96, "res 2, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Res(c.mem.Read8(c.HL), 2)); return 16; }),
            new Opcode(0x97, "res 2, a"            , 2, (c,i) => {c.A = c.alu.Res(c.A, 2);  return 8; }),
            new Opcode(0x98, "res 3, b"            , 2, (c,i) => {c.B = c.alu.Res(c.B, 3); return 8; }),
            new Opcode(0x99, "res 3, c"            , 2, (c,i) => {c.C = c.alu.Res(c.C, 3); return 8; }),
            new Opcode(0x9A, "res 3, d"            , 2, (c,i) => {c.D = c.alu.Res(c.D, 3); return 8; }),
            new Opcode(0x9B, "res 3, e"            , 2, (c,i) => {c.E = c.alu.Res(c.E, 3); return 8; }),
            new Opcode(0x9C, "res 3, h"            , 2, (c,i) => {c.H = c.alu.Res(c.H, 3); return 8; }),
            new Opcode(0x9D, "res 3, l"            , 2, (c,i) => {c.L = c.alu.Res(c.L, 3); return 8; }),
            new Opcode(0x9E, "res 3, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Res(c.mem.Read8(c.HL), 3)); return 16; }),
            new Opcode(0x9F, "res 3, a"            , 2, (c,i) => {c.A = c.alu.Res(c.A, 3);  return 8; }),
            // 0xA0
            new Opcode(0xA0, "res 4, b"            , 2, (c,i) => {c.B = c.alu.Res(c.B, 4); return 8; }),
            new Opcode(0xA1, "res 4, c"            , 2, (c,i) => {c.C = c.alu.Res(c.C, 4); return 8; }),
            new Opcode(0xA2, "res 4, d"            , 2, (c,i) => {c.D = c.alu.Res(c.D, 4); return 8; }),
            new Opcode(0xA3, "res 4, e"            , 2, (c,i) => {c.E = c.alu.Res(c.E, 4); return 8; }),
            new Opcode(0xA4, "res 4, h"            , 2, (c,i) => {c.H = c.alu.Res(c.H, 4); return 8; }),
            new Opcode(0xA5, "res 4, l"            , 2, (c,i) => {c.L = c.alu.Res(c.L, 4); return 8; }),
            new Opcode(0xA6, "res 4, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Res(c.mem.Read8(c.HL), 4)); return 16; }),
            new Opcode(0xA7, "res 4, a"            , 2, (c,i) => {c.A = c.alu.Res(c.A, 4);  return 8; }),
            new Opcode(0xA8, "res 5, b"            , 2, (c,i) => {c.B = c.alu.Res(c.B, 5); return 8; }),
            new Opcode(0xA9, "res 5, c"            , 2, (c,i) => {c.C = c.alu.Res(c.C, 5); return 8; }),
            new Opcode(0xAA, "res 5, d"            , 2, (c,i) => {c.D = c.alu.Res(c.D, 5); return 8; }),
            new Opcode(0xAB, "res 5, e"            , 2, (c,i) => {c.E = c.alu.Res(c.E, 5); return 8; }),
            new Opcode(0xAC, "res 5, h"            , 2, (c,i) => {c.H = c.alu.Res(c.H, 5); return 8; }),
            new Opcode(0xAD, "res 5, l"            , 2, (c,i) => {c.L = c.alu.Res(c.L, 5); return 8; }),
            new Opcode(0xAE, "res 5, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Res(c.mem.Read8(c.HL), 5)); return 16; }),
            new Opcode(0xAF, "res 5, a"            , 2, (c,i) => {c.A = c.alu.Res(c.A, 5);  return 8; }),
            // 0xB0
            new Opcode(0xB0, "res 6, b"            , 2, (c,i) => {c.B = c.alu.Res(c.B, 6); return 8; }),
            new Opcode(0xB1, "res 6, c"            , 2, (c,i) => {c.C = c.alu.Res(c.C, 6); return 8; }),
            new Opcode(0xB2, "res 6, d"            , 2, (c,i) => {c.D = c.alu.Res(c.D, 6); return 8; }),
            new Opcode(0xB3, "res 6, e"            , 2, (c,i) => {c.E = c.alu.Res(c.E, 6); return 8; }),
            new Opcode(0xB4, "res 6, h"            , 2, (c,i) => {c.H = c.alu.Res(c.H, 6); return 8; }),
            new Opcode(0xB5, "res 6, l"            , 2, (c,i) => {c.L = c.alu.Res(c.L, 6); return 8; }),
            new Opcode(0xB6, "res 6, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Res(c.mem.Read8(c.HL), 6)); return 16; }),
            new Opcode(0xB7, "res 6, a"            , 2, (c,i) => {c.A = c.alu.Res(c.A, 6);  return 8; }),
            new Opcode(0xB8, "res 7, b"            , 2, (c,i) => {c.B = c.alu.Res(c.B, 7); return 8; }),
            new Opcode(0xB9, "res 7, c"            , 2, (c,i) => {c.C = c.alu.Res(c.C, 7); return 8; }),
            new Opcode(0xBA, "res 7, d"            , 2, (c,i) => {c.D = c.alu.Res(c.D, 7); return 8; }),
            new Opcode(0xBB, "res 7, e"            , 2, (c,i) => {c.E = c.alu.Res(c.E, 7); return 8; }),
            new Opcode(0xBC, "res 7, h"            , 2, (c,i) => {c.H = c.alu.Res(c.H, 7); return 8; }),
            new Opcode(0xBD, "res 7, l"            , 2, (c,i) => {c.L = c.alu.Res(c.L, 7); return 8; }),
            new Opcode(0xBE, "res 7, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Res(c.mem.Read8(c.HL), 7)); return 16; }),
            new Opcode(0xBF, "res 7, a"            , 2, (c,i) => {c.A = c.alu.Res(c.A, 7);  return 8; }),
            // 0xC0
            new Opcode(0xC0, "set 0, b"            , 2, (c,i) => {c.B = c.alu.Set(c.B, 0); return 8; }),
            new Opcode(0xC1, "set 0, c"            , 2, (c,i) => {c.C = c.alu.Set(c.C, 0); return 8; }),
            new Opcode(0xC2, "set 0, d"            , 2, (c,i) => {c.D = c.alu.Set(c.D, 0); return 8; }),
            new Opcode(0xC3, "set 0, e"            , 2, (c,i) => {c.E = c.alu.Set(c.E, 0); return 8; }),
            new Opcode(0xC4, "set 0, h"            , 2, (c,i) => {c.H = c.alu.Set(c.H, 0); return 8; }),
            new Opcode(0xC5, "set 0, l"            , 2, (c,i) => {c.L = c.alu.Set(c.L, 0); return 8; }),
            new Opcode(0xC6, "set 0, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Set(c.mem.Read8(c.HL), 0)); return 16; }),
            new Opcode(0xC7, "set 0, a"            , 2, (c,i) => {c.A = c.alu.Set(c.A, 0);  return 8; }),
            new Opcode(0xC8, "set 1, b"            , 2, (c,i) => {c.B = c.alu.Set(c.B, 1); return 8; }),
            new Opcode(0xC9, "set 1, c"            , 2, (c,i) => {c.C = c.alu.Set(c.C, 1); return 8; }),
            new Opcode(0xCA, "set 1, d"            , 2, (c,i) => {c.D = c.alu.Set(c.D, 1); return 8; }),
            new Opcode(0xCB, "set 1, e"            , 2, (c,i) => {c.E = c.alu.Set(c.E, 1); return 8; }),
            new Opcode(0xCC, "set 1, h"            , 2, (c,i) => {c.H = c.alu.Set(c.H, 1); return 8; }),
            new Opcode(0xCD, "set 1, l"            , 2, (c,i) => {c.L = c.alu.Set(c.L, 1); return 8; }),
            new Opcode(0xCE, "set 1, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Set(c.mem.Read8(c.HL), 1)); return 16; }),
            new Opcode(0xCF, "set 1, a"            , 2, (c,i) => {c.A = c.alu.Set(c.A, 1);  return 8; }),
            // 0xD0
            new Opcode(0xD0, "set 2, b"            , 2, (c,i) => {c.B = c.alu.Set(c.B, 2); return 8; }),
            new Opcode(0xD1, "set 2, c"            , 2, (c,i) => {c.C = c.alu.Set(c.C, 2); return 8; }),
            new Opcode(0xD2, "set 2, d"            , 2, (c,i) => {c.D = c.alu.Set(c.D, 2); return 8; }),
            new Opcode(0xD3, "set 2, e"            , 2, (c,i) => {c.E = c.alu.Set(c.E, 2); return 8; }),
            new Opcode(0xD4, "set 2, h"            , 2, (c,i) => {c.H = c.alu.Set(c.H, 2); return 8; }),
            new Opcode(0xD5, "set 2, l"            , 2, (c,i) => {c.L = c.alu.Set(c.L, 2); return 8; }),
            new Opcode(0xD6, "set 2, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Set(c.mem.Read8(c.HL), 2)); return 16; }),
            new Opcode(0xD7, "set 2, a"            , 2, (c,i) => {c.A = c.alu.Set(c.A, 2);  return 8; }),
            new Opcode(0xD8, "set 3, b"            , 2, (c,i) => {c.B = c.alu.Set(c.B, 3); return 8; }),
            new Opcode(0xD9, "set 3, c"            , 2, (c,i) => {c.C = c.alu.Set(c.C, 3); return 8; }),
            new Opcode(0xDA, "set 3, d"            , 2, (c,i) => {c.D = c.alu.Set(c.D, 3); return 8; }),
            new Opcode(0xDB, "set 3, e"            , 2, (c,i) => {c.E = c.alu.Set(c.E, 3); return 8; }),
            new Opcode(0xDC, "set 3, h"            , 2, (c,i) => {c.H = c.alu.Set(c.H, 3); return 8; }),
            new Opcode(0xDD, "set 3, l"            , 2, (c,i) => {c.L = c.alu.Set(c.L, 3); return 8; }),
            new Opcode(0xDE, "set 3, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Set(c.mem.Read8(c.HL), 3)); return 16; }),
            new Opcode(0xDF, "set 3, a"            , 2, (c,i) => {c.A = c.alu.Set(c.A, 3);  return 8; }),
            // 0xE0
            new Opcode(0xE0, "set 4, b"            , 2, (c,i) => {c.B = c.alu.Set(c.B, 4); return 8; }),
            new Opcode(0xE1, "set 4, c"            , 2, (c,i) => {c.C = c.alu.Set(c.C, 4); return 8; }),
            new Opcode(0xE2, "set 4, d"            , 2, (c,i) => {c.D = c.alu.Set(c.D, 4); return 8; }),
            new Opcode(0xE3, "set 4, e"            , 2, (c,i) => {c.E = c.alu.Set(c.E, 4); return 8; }),
            new Opcode(0xE4, "set 4, h"            , 2, (c,i) => {c.H = c.alu.Set(c.H, 4); return 8; }),
            new Opcode(0xE5, "set 4, l"            , 2, (c,i) => {c.L = c.alu.Set(c.L, 4); return 8; }),
            new Opcode(0xE6, "set 4, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Set(c.mem.Read8(c.HL), 4)); return 16; }),
            new Opcode(0xE7, "set 4, a"            , 2, (c,i) => {c.A = c.alu.Set(c.A, 4);  return 8; }),
            new Opcode(0xE8, "set 5, b"            , 2, (c,i) => {c.B = c.alu.Set(c.B, 5); return 8; }),
            new Opcode(0xE9, "set 5, c"            , 2, (c,i) => {c.C = c.alu.Set(c.C, 5); return 8; }),
            new Opcode(0xEA, "set 5, d"            , 2, (c,i) => {c.D = c.alu.Set(c.D, 5); return 8; }),
            new Opcode(0xEB, "set 5, e"            , 2, (c,i) => {c.E = c.alu.Set(c.E, 5); return 8; }),
            new Opcode(0xEC, "set 5, h"            , 2, (c,i) => {c.H = c.alu.Set(c.H, 5); return 8; }),
            new Opcode(0xED, "set 5, l"            , 2, (c,i) => {c.L = c.alu.Set(c.L, 5); return 8; }),
            new Opcode(0xEE, "set 5, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Set(c.mem.Read8(c.HL), 5)); return 16; }),
            new Opcode(0xEF, "set 5, a"            , 2, (c,i) => {c.A = c.alu.Set(c.A, 5);  return 8; }),
            // 0xF0
            new Opcode(0xF0, "set 6, b"            , 2, (c,i) => {c.B = c.alu.Set(c.B, 6); return 8; }),
            new Opcode(0xF1, "set 6, c"            , 2, (c,i) => {c.C = c.alu.Set(c.C, 6); return 8; }),
            new Opcode(0xF2, "set 6, d"            , 2, (c,i) => {c.D = c.alu.Set(c.D, 6); return 8; }),
            new Opcode(0xF3, "set 6, e"            , 2, (c,i) => {c.E = c.alu.Set(c.E, 6); return 8; }),
            new Opcode(0xF4, "set 6, h"            , 2, (c,i) => {c.H = c.alu.Set(c.H, 6); return 8; }),
            new Opcode(0xF5, "set 6, l"            , 2, (c,i) => {c.L = c.alu.Set(c.L, 6); return 8; }),
            new Opcode(0xF6, "set 6, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Set(c.mem.Read8(c.HL), 6)); return 16; }),
            new Opcode(0xF7, "set 6, a"            , 2, (c,i) => {c.A = c.alu.Set(c.A, 6);  return 8; }),
            new Opcode(0xF8, "set 7, b"            , 2, (c,i) => {c.B = c.alu.Set(c.B, 7); return 8; }),
            new Opcode(0xF9, "set 7, c"            , 2, (c,i) => {c.C = c.alu.Set(c.C, 7); return 8; }),
            new Opcode(0xFA, "set 7, d"            , 2, (c,i) => {c.D = c.alu.Set(c.D, 7); return 8; }),
            new Opcode(0xFB, "set 7, e"            , 2, (c,i) => {c.E = c.alu.Set(c.E, 7); return 8; }),
            new Opcode(0xFC, "set 7, h"            , 2, (c,i) => {c.H = c.alu.Set(c.H, 7); return 8; }),
            new Opcode(0xFD, "set 7, l"            , 2, (c,i) => {c.L = c.alu.Set(c.L, 7); return 8; }),
            new Opcode(0xFE, "set 7, (hl)"         , 2, (c,i) => {c.mem.Write8(c.HL, c.alu.Set(c.mem.Read8(c.HL), 7)); return 16; }),
            new Opcode(0xFF, "set 7, a"            , 2, (c,i) => {c.A = c.alu.Set(c.A, 7);  return 8; }),
        };

        public static int OpUnimplemented(LR35902 c, Instruction i)
        {
            string op = "0x" + ByteUtilities.HexString(i.Opcode);
            string offset = "$" + ByteUtilities.HexString(i.Offset, true);


            if (!i.isPrefix)
            {
                Console.WriteLine("Invalid instruction: " + op + " @ " + offset);
            }
            else
            {
                op = "0x" + ByteUtilities.HexString(i.Operand8);
                Console.WriteLine("Invalid prefix instruction: " + op + " @ " + offset);
            }

            c.PC -= (ushort)i.code.Length; // Rewind a bit, as we auto increment
            // Print neighborhood:
            Console.WriteLine("Memory:");
            int memoryRange = 2;
            for (int j = -memoryRange; j <= memoryRange; j++)
            {
                int address = i.Offset + j;
                string pointer = "";
                if (j == 0) { pointer = "<- PC"; }
                Console.WriteLine(String.Format("${0:X4}: {1:X2} {2}", (ushort)address, c.mem.Read8((ushort)address), pointer));
            }

            c.Stop(); // Halt CPU
            c.DebugPrintRegisters();
            return 4;
        }

        public static int OpInvalidOpcode(LR35902 c, Instruction i)
        {
            string op = "0x" + ByteUtilities.HexString(i.Opcode);
            string offset = "$" + ByteUtilities.HexString(i.Offset, true);


            if (!i.isPrefix)
            {
                Console.WriteLine("Invalid instruction: " + op + " @ " + offset);
            }
            else
            {
                op = "0x" + ByteUtilities.HexString(i.Operand8);
                Console.WriteLine("Invalid prefix instruction: " + op + " @ " + offset);
            }

            c.PC -= (ushort)i.code.Length; // Rewind a bit, as we auto increment
            // Print neighborhood:
            Console.WriteLine("Memory:");
            int memoryRange = 2;
            for (int j = -memoryRange; j <= memoryRange; j++)
            {
                int address = i.Offset + j;
                string pointer = "";
                if (j == 0) { pointer = "<- PC"; }
                Console.WriteLine(String.Format("${0:X4}: {1:X2} {2}", (ushort)address, c.mem.Read8((ushort)address), pointer));
            }

            c.Stop(); // Halt CPU
            c.DebugPrintRegisters();
            return 4;
        }

        private static int OpIncrement8(LR35902 c, Instruction i, ref byte val)
        {
            val = c.alu.Increment(val, RegFlags.Z | RegFlags.H, RegFlags.None, RegFlags.N);
            return 4;
        }
        private static int OpDecrement8(LR35902 c, Instruction i, ref byte val)
        {
            val = c.alu.Decrement(val, RegFlags.Z | RegFlags.H, RegFlags.N);
            return 4;
        }

        private static int OpIncrement16(LR35902 c, Instruction i, ref ushort val)
        {
            //val = c.alu.Increment(val, RegFlags.Z | RegFlags.H, RegFlags.None, RegFlags.N);
            val = c.alu.Increment(val);
            return 8;
        }
        // Decrement 16 bit number
        // Note that since 16 bit numbers are accessed via properties, that we must store the value temporarily
        private static int OpDecrement16(LR35902 c, Instruction i, ref ushort val)
        {
            val = c.alu.Decrement(val);
            return 8;
        }

        private static int OpJumpRelative(LR35902 c, Instruction i, bool condition)
        {
            if (condition)
            {
                c.PC = (ushort)unchecked(c.PC + (sbyte)i.Operand8);
                return 12;
            }
            else
            {
                return 8;
            }
        }

        private int OpJumpConditional(Instruction i, bool condition)
        {
            if (condition)
            {
                PC = i.Operand16;
                return 16;
            }
            else
            {
                return 12;
            }
        }

        private void Call(ushort address)
        {
            Push16(PC);
            PC = address;
        }

        private int OpCallConditional(ushort address, bool condition)
        {
            if (condition)
            {
                Call(address);
                return 24;
            }
            else
            {
                return 12;
            }
        }

        private void Ret()
        {
            PC = Pop16();
        }

        private int RetCondition(bool condition)
        {
            if (condition)
            {
                Ret();
                return 20;
            }
            else
            {
                return 8;
            }
        }

        private void Push16(ushort value)
        {
            SP -= 2;
            mem.Write16(SP, value);
        }

        private ushort Pop16()
        {
            ushort res = mem.Read16(SP);
            SP += 2;
            return res;
        }

        private static byte OpBit(byte bit, byte val, bool set)
        {
            byte mask = (byte)(1 << bit);
            byte res = val;
            if (set)
            {
                res = (byte)(val | mask);
            }
            else
            {
                res = (byte)(val & (~mask));
            }
            return res;
        }
    }
}
