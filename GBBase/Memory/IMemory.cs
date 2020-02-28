using System;
using System.Collections.Generic;
using System.Text;

namespace GBBase.Memory
{
    public interface IMemory
    {
        void WriteByte(ushort address, byte value);
        byte ReadByte(ushort address);
        void WriteUshort(ushort address, ushort value);
        ushort ReadUshort(ushort address);
    }
}
