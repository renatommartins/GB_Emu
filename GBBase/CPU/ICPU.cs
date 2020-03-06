using System;
using System.Collections.Generic;
using System.Text;

namespace GBBase.CPU
{
    public interface ICPU
    {
        IALU ALU { get; }
        IRegisters registers { get; }
        int BusyCycles { get; set; }
    }
}
