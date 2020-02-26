using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GBBase
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Registers
    {
        [FieldOffset(0)] public byte a;
        [FieldOffset(1)] public byte f;
        [FieldOffset(0)] public ushort af;

        [FieldOffset(2)] public byte c;
        [FieldOffset(3)] public byte b;
        [FieldOffset(2)] public ushort bc;

        [FieldOffset(4)] public byte e;
        [FieldOffset(5)] public byte d;
        [FieldOffset(4)] public ushort de;

        [FieldOffset(6)] public byte l;
        [FieldOffset(7)] public byte h;
        [FieldOffset(6)] public ushort hl;

        [FieldOffset(8)] private ushort _flags;
        public ushort flags
        {
            get
            {
                return _flags;
            }

            set
            {
                _flags = (ushort)(value & 0xF0);
            }
        }

        public bool ZeroFlag
        {
            get
            {
                return (_flags & 0x80) != 0;
            }
            set
            {
                _flags = (ushort)(value ? _flags | 0x80 : _flags & (~0x80));
            }
        }

        public bool NegativeFlag
        {
            get
            {
                return (_flags & 0x40) != 0;
            }
            set
            {
                _flags = (ushort)(value ? _flags | 0x40 : _flags & (~0x40));
            }
        }

        public bool HalfCarryFlag
        {
            get
            {
                return (_flags & 0x20) != 0;
            }
            set
            {
                _flags = (ushort)(value ? _flags | 0x20 : _flags & (~0x20));
            }
        }

        public bool FullCarryFlag
        {
            get
            {
                return (_flags & 0x10) != 0;
            }
            set
            {
                _flags = (ushort)(value ? _flags | 0x10 : _flags & (~0x10));
            }
        }

        [FieldOffset(9)] public ushort sp;
        [FieldOffset(10)] public ushort pc;
    }
}
