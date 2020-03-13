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
        /// <summary>
        /// Interrupt Master Enable Flag
        /// </summary>
        bool IME { get; set; }
    }
}
