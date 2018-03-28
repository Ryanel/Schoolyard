using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schoolyard.Memory
{
    public abstract class MemoryDevice
    {
        public ushort size; // Size in address space
        public ushort addressBase; // Base address
        public ushort addressEnd; // Calculated from addressBase + size
        public string name; // Name of the device

        public abstract byte Read8(ushort address);
        public abstract void Write8(ushort address, byte val);

        public virtual ushort Read16(ushort address)
        {
            byte lsb = Read8(address);
            byte msb = Read8((ushort)(address + 1));
            ushort result = (ushort)((ushort)(msb << 8) + lsb);
            return result;
        }
        public virtual void Write16(ushort address, ushort val)
        {
            Write8(address, (byte)(val & 0x00FF));
            Write8((ushort)(address + 1), (byte)((val & 0xFF00) >> 8));
        }
    }
}
