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
        public abstract ushort Read16(ushort address);
        public abstract void Write8(ushort address, byte val);
        public abstract void Write16(ushort address, ushort val);
    }
}
