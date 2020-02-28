using System;


namespace GBBase
{
    public class GameboyHardware
    {
        public struct Pixel
        {
            public byte alpha;
            public byte red;
            public byte green;
            public byte blue;

            public uint this[int index]
            {
                get
                {
                    return
                        (uint)(alpha << 24) +
                        (uint)(red << 16) +
                        (uint)(green << 8) +
                        (uint)(blue << 0)
                        ;
                }
                set
                {
                    alpha = (byte)((value >> 24) & 0xFF);
                    red = (byte)((value >> 16) & 0xFF);
                    green = (byte)((value >> 8) & 0xFF);
                    blue = (byte)((value >> 0) & 0xFF);
                }
            }
        }
        public const int programStartAddress = 0x150;

        public CPU.ICPU CPU = new CPU.CPU();
        public Memory.IMemory memory = new Memory.MemoryController();
        public Pixel[,] screenOutput = new Pixel[160,144];
    }
}
