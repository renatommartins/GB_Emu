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
                    disassembly = "LD BC, 0x{0:X}",
                    cycles = 3,
                    operandLength = 2,
                    method = (gameboy, operands) =>
                    {
                        ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));
                        gameboy.CPU.registers.BC = value;
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
                        gameboy.memory.WriteByte(gameboy.CPU.registers.BC, gameboy.CPU.registers.A);
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
                        gameboy.CPU.ALU.IncrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.BC);
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
                        gameboy.CPU.ALU.IncrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.B);
                    }
                }
            },
            //0x05 DEC B
            {
                0x05,
                new Instruction()
                {
                    disassembly = "DEC B",
                     cycles = 1,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.ALU.DecrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.B);
                    }
                }
            },
            //0x06 LD B, n8
            {
                0x06,
                new Instruction()
                {
                    disassembly = "LD B, {0:X}",
                    cycles = 2,
                    operandLength = 1,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.registers.B = operands[0];
                    }
                }
            },
            //0x07 RLC A
            {
                0x07,
                new Instruction()
                {
                    disassembly = "RLC A",
                    cycles = 1,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.ALU.RotateRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, CPU.IALU.RotateDirection.Left, true);
                    }
                }
            },
            //0x08 LD n16, SP
            {
                0x08,
                new Instruction()
                {
                    disassembly = "LD {0:X}, SP",
                    cycles = 5,
                    operandLength = 2,
                    method = (gameboy, operands) =>
                    {
                        ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));
                        gameboy.memory.WriteUshort(value, gameboy.CPU.registers.SP);
                    }
                }
            },
            //0x09 ADD HL, BC
            {
                0x09,
                new Instruction()
                {
                    disassembly = "ADD HL, BC",
                    cycles = 2,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.ALU.AddUshort(gameboy.CPU.registers, CPU.IALU.TargetRegister.HL, gameboy.CPU.registers.BC, false);
                    }
                }
            },
            //0x0A LD A, (BC)
            {
                0x0A,
                new Instruction()
                {
                    disassembly = "LD A, (BC)",
                    cycles = 2,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.registers.A = gameboy.memory.ReadByte(gameboy.CPU.registers.BC);
                    }
                }
            },
            //0x0B DEC BC
            {
                0x0B,
                new Instruction()
                {
                    disassembly = "DEC BC",
                    cycles = 2,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.ALU.DecrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.BC);
                    }
                }
            },
            //0x0C INC C
            {
                0x0C,
                new Instruction()
                {
                    disassembly = "INC C",
                    cycles = 1,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.ALU.IncrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.C);
                    }
                }
            },
            //0x0D DEC C
            {
                0x0D,
                new Instruction()
                {
                    disassembly = "DEC C",
                    cycles = 1,
                    operandLength = 0,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.ALU.DecrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.C);
                    }
                }
            },
            //0x18 JR n8
            {
                0x18,
                new Instruction()
                {
                    disassembly = "JR 0x{0:X}",
                    cycles = 3,
                    operandLength = 1,
                    method = (gameboy, operands) =>
                    {
                        sbyte value = (sbyte)operands[0];
                        int newAddress = gameboy.CPU.registers.PC;
                        gameboy.CPU.registers.PC = (ushort)(newAddress + value);
                    }
                }
            },
            //0x28 JR Z, n8
            {
                0x28,
                new Instruction()
                {
                    disassembly = "JR Z, 0x{0:X}",
                    cycles = 3,//2?
                    operandLength = 1,
                    method = (gameboy, operands) =>
                    {
                        if(gameboy.CPU.registers.ZeroFlag)
                        {
                            sbyte value = (sbyte)operands[0];
                            int newAddress = gameboy.CPU.registers.PC;
                            gameboy.CPU.registers.PC = (ushort)(newAddress + value);
                        }
                    }
                }
            },
            //0x3E LD A, n8
            {
                0x3E,
                new Instruction()
                {
                    disassembly = "LD A, 0x{0:X}",
                    cycles = 2,
                    operandLength = 1,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.registers.A = operands[0];
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
                        gameboy.CPU.ALU.XorBitwiseByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, gameboy.CPU.registers.A);
                    }
                }
            },
            //0xC3 JP n16
            {
                0xC3,
                new Instruction()
                {
                    disassembly = "JP 0x{0:X}",
                    cycles = 1,
                    operandLength = 2,
                    method = (gameboy, operands) =>
                    {
                        ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));
                        gameboy.CPU.registers.PC = value;
                    }
                }
            },
            //0xEA LD n16, A
            {
                0xEA,
                new Instruction()
                {
                    disassembly = "LD 0x{0:X}, A",
                    cycles = 4,
                    operandLength = 2,
                    method = (gameboy, operands) =>
                    {
                        ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));
                        gameboy.memory.WriteByte(value, gameboy.CPU.registers.A);
                    }
                }
            },
            //0xFE CP n8
            {
                0xFE,
                new Instruction()
                {
                    disassembly = "CP 0x{0:X}",
                    cycles = 2,
                    operandLength = 1,
                    method = (gameboy, operands) =>
                    {
                        gameboy.CPU.ALU.CompareByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, operands[0]);
                    }
                }
            },
        };
    }
}
