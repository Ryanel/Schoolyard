﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace Schoolyard.Memory
{
    public class MemoryController
    {
        public Gameboy gameboy;
        private List<MemoryDevice> devices = new List<MemoryDevice>();
        private MemoryDevice rom;

        public const bool debugLog = false;
        public string serialOut = ""; // Hack to allow easy serial output
        private bool cacheUpToDate = false;
        private MemoryDevice[] translationCache = new MemoryDevice[0x10000];

        public MemoryController(Gameboy gameboy)
        {
            this.gameboy = gameboy;
        }

        public void Map(MemoryDevice device) {
            devices.Add(device);
            cacheUpToDate = false;
        }
        public void Map(MemoryDevice device, bool rom) {
            devices.Add(device);
            this.rom = device;
            cacheUpToDate = false;
            RebuildCache();
        }

        public void UnMap(MemoryDevice device) {
            devices.Remove(device);
            cacheUpToDate = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryDevice GetMappedDevice(ushort address)
        {
            if(cacheUpToDate)
            {
                return translationCache[address];
            }
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


        public void RebuildCache()
        {
            cacheUpToDate = false;
            for (int i = 0; i <= 0xFFFF; i++)
            {
                translationCache[i] = GetMappedDevice((ushort)i);
            }
            cacheUpToDate = true;
        }
        
        public byte Read8(ushort address)
        {
            MemoryDevice dev = GetMappedDevice(address);
            if (dev != null) {
                if (debugLog)
                {
                    Console.WriteLine(String.Format("Read from {2} at ${0:X4} -> {1:X2}", address, dev.Read8(address), dev.name));
                }
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

            if (address == 0xFF46)
            {
                gameboy.dma.Write8(0xFF46, val);
                return;
            }

            MemoryDevice dev = GetMappedDevice(address);
            if (dev != null) {
                if(debugLog)
                {
                    Console.WriteLine(String.Format("Write to {2} at ${0:X4} <- {1:X2}", address, val, dev.name));
                }
                
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
            rom = null;
        }
    }
}
