using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace Schoolyard.Memory
{
    public class MemoryController
    {
        private List<MemoryDevice> devices = new List<MemoryDevice>();

        public void Map(MemoryDevice device) { devices.Add(device); }
        public void UnMap(MemoryDevice device) { devices.Remove(device); }

        public string serialOut = ""; // Hack to allow easy serial output

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryDevice GetMappedDevice(ushort address)
        {
            MemoryDevice result = null;
            int devCount = devices.Count;
            for (int i = 0; i < devCount; i++) // We don't foreach for performance, as this is called on every memory read
            {
                MemoryDevice item = devices[i];
                int devBaseAddress = item.addressBase;
                int devEndAddress = devBaseAddress + item.size;
                if (address >= devBaseAddress && address < devEndAddress)
                {
                    result = item;
                    break;
                }
            }

            return result;
        }
        
        public byte Read8(ushort address)
        {
            MemoryDevice dev = GetMappedDevice(address);
            if (dev != null) {
                return dev.Read8(address);
            }

            throw new ArgumentNullException(nameof(dev),String.Format("Could not find mapped device for address ${0:X4}", address));
        }
        
        public void Write8(ushort address, byte val)
        {
            // Catch serial out
            if (address == 0xff01)
            {
                // Serial output
                Console.Write((Char)val);
                if (val == 10)
                {
                    serialOut += "\r\n";
                }
                else
                {
                    serialOut += (Char)val;
                }
            }

            MemoryDevice dev = GetMappedDevice(address);
            if (dev != null) {
                dev.Write8(address, val);
                return;
            }

            throw new ArgumentNullException(nameof(dev), String.Format("Could not find mapped device for address ${0:X4}",address));
        }

        public ushort Read16(ushort address)
        {
            byte lsb = Read8(address);
            byte msb = Read8((ushort)(address + 1));
            ushort result = (ushort)((ushort)(msb << 8) + lsb);
            return result;
        }

        public void Write16(ushort address, ushort val)
        {
            Write8(address, (byte)(val & 0x00FF));
            Write8((ushort)(address + 1), (byte)((val & 0xFF00) >> 8));
        }

        public void Reset()
        {
            devices.Clear();
        }
    }
}
