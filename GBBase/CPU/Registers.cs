using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GBBase.CPU
{
    public class Registers : IRegisters
    {
        public byte A { get; set; }
        public byte F { get; set; }
        public ushort AF
        {
            get
            {
                return (ushort)((A << 0) + (F << 8));
            }
            set
            {
                A = (byte)((value & 0x00FF) >> 0);
                F = (byte)((value & 0xFF00) >> 8);
            }
        }

        public byte C { get; set; }
        public byte B { get; set; }
        public ushort BC
        {
            get
            {
                return (ushort)((B << 0) + (C << 8));
            }
            set
            {
                B = (byte)((value & 0x00FF) >> 0);
                C = (byte)((value & 0xFF00) >> 8);
            }
        }

        public byte E { get; set; }
        public byte D { get; set; }
        public ushort DE
        {
            get
            {
                return (ushort)((D << 0) + (E << 8));
            }
            set
            {
                D = (byte)((value & 0x00FF) >> 0);
                E = (byte)((value & 0xFF00) >> 8);
            }
        }

        public byte L { get; set; }
        public byte H { get; set; }
        public ushort HL
        {
            get
            {
                return (ushort)((H << 0) + (L << 8));
            }
            set
            {
                H = (byte)((value & 0x00FF) >> 0);
                L = (byte)((value & 0xFF00) >> 8);
            }
        }

        public byte flags
        {
            get
            {
                return F;
            }

            set
            {
                F = (byte)(value & 0xF0);
            }
        }

        public bool ZeroFlag
        {
            get
            {
                return (F & 0x80) != 0;
            }
            set
            {
                F = (byte)(value ? F | 0x80 : F & (~0x80));
            }
        }

        public bool NegativeFlag
        {
            get
            {
                return (F & 0x40) != 0;
            }
            set
            {
                F = (byte)(value ? F | 0x40 : F & (~0x40));
            }
        }

        public bool HalfCarryFlag
        {
            get
            {
                return (F & 0x20) != 0;
            }
            set
            {
                F = (byte)(value ? F | 0x20 : F & (~0x20));
            }
        }

        public bool FullCarryFlag
        {
            get
            {
                return (F & 0x10) != 0;
            }
            set
            {
                F = (byte)(value ? F | 0x10 : F & (~0x10));
            }
        }

        public ushort SP { get; set; }
        public ushort PC { get; set; }
    }
}
