using System;
using System.Collections.Generic;
using System.Text;

namespace GBBase.Memory
{
    public class MemoryController : IMemory
    {
        private byte[] workRam = new byte[1024 * 8];
        private byte[] expansionRam = new byte[1024 * 8];
        private byte[] lcdRam = new byte[1024 * 8];
        private byte[] cpuRam = new byte[512];
        private byte[] rom = new byte[1024 * 32];
        private byte[] romBank = new byte[1024 * 32];

        private void MapAddress(ushort address, out byte[] target, out int offset)
        {
            target = null;
            offset = 0;

            if(address >= 0x0000 && address < 0x8000)
            {
                target = rom;
                offset = address - 0x0000;
            }
            else if(address >= 0x8000 && address < 0x9800)
            {
                target = romBank;
                offset = address - 0x8000;
            }
            else if(address >= 0xA000 && address < 0xC000)
            {
                target = expansionRam;
                offset = address - 0xA000;
            }
            else if(address >= 0xC000 && address < 0xE000)
            {
                target = workRam;
                offset = address - 0xC000;
            }
            else if(address >= 0xE000 && address < 0xFE00)
            {
                target = workRam;
                offset = address - 0xE000;
            }
            else if(address >= 0xFE00 && address <= 0xFFFF)
            {
                target = cpuRam;
                offset = address - 0xFE00;
            }
        }

        public void WriteByte(ushort address, byte value)
        {
            byte[] memory;
            int offset;
            MapAddress(address, out memory, out offset);

            memory[offset] = value;
        }

        public byte ReadByte(ushort address)
        {
            byte[] memory;
            int offset;
            MapAddress(address, out memory, out offset);

            return memory[offset];
        }

        public void WriteUshort(ushort address, ushort value)
        {
            byte[] memory;
            int offset;
            MapAddress(address, out memory, out offset);

            memory[offset] = (byte)((value & 0x00FF) >> 0);
            memory[offset + 1] = (byte)((value & 0xFF00) >> 8);
        }

        public ushort ReadUshort(ushort address)
        {
            byte[] memory;
            int offset;
            MapAddress(address, out memory, out offset);

            return (ushort)((memory[offset] << 0) +(memory[offset + 1] << 8));
        }
    }
}
