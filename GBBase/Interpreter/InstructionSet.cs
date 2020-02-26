using System;
using System.Collections.Generic;
using System.Text;

namespace GBBase
{
    public partial class Interpreter
    {
        public delegate void InstructionMethod(GameboyHardware gameboy, params byte[] operands);

        public struct Instruction
        {
            public string disassembly;
            public byte cycles;
            public byte operandLength;
            public InstructionMethod method;
        }

        private static void AssertOperands(byte length, byte[] operands)
        {
            if(
                (length > 0 && length > operands.Length) ||
                (length == 0 && (operands != null || operands.Length > 0))
                )
            {
                throw new ArgumentException("invalid operands");
            }
        }

        private static bool EvaluateHalfCarryAdd(byte value, byte add)
        {
            return ((((value & 0x0F) + (add & 0x0F)) & 0x10) == 0x10);
        }

        private static bool EvaluateHalfCarrySubtraction(byte value, byte sub)
        {
            return ((value & 0x0F) - (sub & 0x0F)) < 0;
        }

        private static Dictionary<byte, Instruction> _instructionSet = new Dictionary<byte, Instruction>()
        {
            //0x00 NOP
            {
                0x00,
                new Instruction()
                {
                    disassembly = "NOP",
                    cycles = 1,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        
                    }
                }
            },
            //0x01 LD BC, n16
            {
                0x01,
                new Instruction()
                {
                    disassembly = "LD BC, {0:D}",
                    cycles = 3,
                    operandLength = 2,
                    method = (gameboy, operands) =>
                    {
                        ushort value = BitConverter.ToUInt16(operands, 0);
                        gameboy.registers.bc = value;
                    }
                }
            },
            //0x02 LD (BC), A
            {
                0x02,
                new Instruction()
                {
                    disassembly = "LD (BC), A",
                    cycles = 2,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.ram[gameboy.registers.bc] = gameboy.registers.a;
                    }
                }
            },
            //0x03 INC BC
            {
                0x03,
                new Instruction()
                {
                    disassembly = "INC BC",
                    cycles = 2,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.registers.bc++;
                    }
                }
            },
            //0x04 INC B
            {
                0x04,
                new Instruction()
                {
                    disassembly = "INC B",
                    cycles = 1,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.registers.NegativeFlag = false;
                        gameboy.registers.HalfCarryFlag = EvaluateHalfCarryAdd(gameboy.registers.b, 1);
                        gameboy.registers.b++;
                        
                        if(gameboy.registers.b == 0)
                        {
                            gameboy.registers.ZeroFlag = true;
                        }
                        
                    }
                }
            },
            //0x18 JR n8
            {
                0x18,
                new Instruction()
                {
                    disassembly = "JR {0:D}",
                    cycles = 3,
                    operandLength = 1,
                    method = (gameboy, operands) =>
                    {
                        sbyte value = (sbyte)operands[0];
                        int newAddress = gameboy.registers.pc;
                        gameboy.registers.pc = (ushort)(newAddress + value);
                    }
                }
            },
            //0x28 JR Z, n8
            {
                0x28,
                new Instruction()
                {
                    disassembly = "JR Z, {0:D}",
                    cycles = 3,//2?
                    operandLength = 1,
                    method = (gameboy, operands) =>
                    {
                        if(gameboy.registers.ZeroFlag)
                        {
                            sbyte value = (sbyte)operands[0];
                            int newAddress = gameboy.registers.pc;
                            gameboy.registers.pc = (ushort)(newAddress + value);
                        }
                    }
                }
            },
            //0x3E LD A, n8
            {
                0x3E,
                new Instruction()
                {
                    disassembly = "LD A, {0:D}",
                    cycles = 2,
                    operandLength = 1,
                    method = (gameboy, operands) =>
                    {
                        gameboy.registers.a = operands[0];
                    }
                }
            },
            //0xAF XOR A
            {
                0xAF,
                new Instruction()
                {
                    disassembly = "XOR A",
                    cycles = 1,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.registers.a ^= gameboy.registers.a;
                        if(gameboy.registers.a == 0)
                        {
                            gameboy.registers.ZeroFlag = true;
                        }
                    }
                }
            },
            //0xEA LD n16, A
            {
                0xEA,
                new Instruction()
                {
                    disassembly = "LD {0:D}, A",
                    cycles = 4,
                    operandLength = 2,
                    method = (gameboy, operands) =>
                    {
                        ushort value = (ushort)((operands[0] << 8) | (operands[1] << 0));//BitConverter.ToUInt16(operands, 0);
                        gameboy.ram[value] = gameboy.registers.a;
                    }
                }
            },
            //0xFE CP n8
            {
                0xFE,
                new Instruction()
                {
                    disassembly = "CP {0:D}",
                    cycles = 2,
                    operandLength = 1,
                    method = (gameboy, operands) =>
                    {
                        gameboy.registers.NegativeFlag = true;
                        gameboy.registers.HalfCarryFlag = EvaluateHalfCarrySubtraction(gameboy.registers.a, operands[0]);
                        if(operands[0] > gameboy.registers.a)
                        {
                            gameboy.registers.FullCarryFlag = true;
                        }
                        gameboy.registers.a -= operands[0];
                        if(gameboy.registers.a == 0)
                        {
                            gameboy.registers.ZeroFlag = true;
                        }
                    }
                }
            },
        };
    }
}
