using System;
using System.Collections.Generic;
using System.Text;

namespace GBBase.CPU
{
    class CPU : ICPU
    {
        public IALU ALU { get; private set; }

        public IRegisters registers { get; private set; }
        public int BusyCycles { get; set; }
    }
}
