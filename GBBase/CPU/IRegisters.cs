using System;
using System.Collections.Generic;
using System.Text;

namespace GBBase.CPU
{
    public interface IRegisters
    {
        public byte A { get; set; }
        public byte F { get; set; }
        public ushort AF { get; set; }

        public byte C { get; set; }
        public byte B { get; set; }
        public ushort BC { get; set; }

        public byte E { get; set; }
        public byte D { get; set; }
        public ushort DE { get; set; }

        public byte L { get; set; }
        public byte H { get; set; }
        public ushort HL { get;set; }

        public byte flags { get; set; }

        public bool ZeroFlag { get; set; }

        public bool NegativeFlag { get; set; }

        public bool HalfCarryFlag { get; set; }

        public bool FullCarryFlag { get; set; }

        public ushort SP { get; set; }
        public ushort PC { get; set; }
    }
}
