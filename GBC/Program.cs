using System;
using System.IO;

using GBBase;

namespace GBC
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryReader binaryReader = new BinaryReader(File.OpenRead("E:\\Projects\\Pokemon - Blue Version (UE) [S][!].gb"));
            binaryReader.BaseStream.Seek(GameboyHardware.programStartAddress, SeekOrigin.Begin);

            StreamWriter streamWriter = new StreamWriter(File.OpenWrite("E:\\dump.txt"));

            Interpreter interpreter = new Interpreter();
            for(int i=GameboyHardware.programStartAddress; i<0x7FFF; i++)
            {
                byte opcode = binaryReader.ReadByte();
                try
                {
                    Interpreter.Instruction instruction = interpreter.GetInstruction(opcode);
                    string disassembly = null;

                    switch (instruction.operandLength)
                    {
                        case 0:
                            {
                                disassembly = instruction.disassembly;
                            }
                            break;
                        case 1:
                            {
                                byte value = binaryReader.ReadByte();
                                disassembly = String.Format(instruction.disassembly, value);
                            }
                            break;
                        case 2:
                            {
                                ushort value = (ushort)((binaryReader.ReadByte() << 0) | (binaryReader.ReadByte() << 8));
                                disassembly = String.Format(instruction.disassembly, value);
                            }
                            break;
                    }

                    Console.WriteLine(disassembly);
                    streamWriter.WriteLine(disassembly);
                }
                catch(NotImplementedException exception)
                {
                    Console.WriteLine(exception.Message);

                    binaryReader.Close();
                    streamWriter.Close();

                    return;
                }
            }

            binaryReader.Close();
            streamWriter.Close();
        }
    }
}
