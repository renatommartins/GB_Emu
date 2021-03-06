﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace InstructionSetGenerator
{
    class Program
    {
        delegate void InstructionParser(int index, string instruction, TextFormatter textFormatter);

        static List<string> opCodes = null;

        static bool isFetching = true;
        static HttpClient httpClient = new HttpClient();
        static string dirtyDumpPath = "opCodeDumpDirty.txt";
        static string cleanDumpPath = "opCodeDumpClean.txt";

        static bool isParsing = true;
        static string instructionSetClassPath = "InstructionSet.cs";
        static Dictionary<string, InstructionParser> instructionParsers = new System.Collections.Generic.Dictionary<string, InstructionParser>
        {
            #region Loads
            //LD
            {
                "LD",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        if(Regex.IsMatch(parameters[1], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                        {
                            cycles = 1;
                            operandLength = 0;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.registers.{parameters[0]} = gameboy.CPU.registers.{parameters[1]};"
                            };
                        }
                        else if(Regex.IsMatch(parameters[1], "n{1}"))
                        {
                            cycles = 2;
                            operandLength = 1;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.registers.{parameters[0]} = operands[0];"
                            };
                        }
                        if(Regex.IsMatch(parameters[1], "(^\\(AF\\)$)|(^\\(BC\\)$)|(^\\(DE\\)$)|(^\\(HL\\)$)"))
                        {
                            cycles = 2;
                            operandLength = 0;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.registers.{parameters[0]} = gameboy.memory.ReadByte(gameboy.CPU.registers.{Regex.Replace(parameters[1],"\\(|\\)","")});"
                            };
                        }
                    }
                    else if(Regex.IsMatch(parameters[0], "(^AF$)|(^BC$)|(^DE$)|(^HL$)"))
                    {
                        cycles = 3;
                        operandLength = 2;
                        instructionCodeLines = new string[]
                        {
                            $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                            $"gameboy.CPU.registers.{parameters[0]} = value;"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "SP"))
                    {
                        if(Regex.IsMatch(parameters[1], "HL"))
                        {
                            cycles = 2;
                            operandLength = 0;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.registers.{parameters[0]} = gameboy.CPU.registers.{parameters[1]};"
                            };
                        }
                        else
                        {
                            cycles = 3;
                            operandLength = 2;
                            instructionCodeLines = new string[]
                            {
                                $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                                $"gameboy.CPU.registers.{parameters[0]} = value;"
                            };
                        }
                    }
                    else if(Regex.IsMatch(parameters[0], "\\(nn\\)"))
                    {
                        if(Regex.IsMatch(parameters[1], "A"))
                        {
                            cycles = 4;
                            operandLength = 2;
                            instructionCodeLines = new string[]
                            {
                                $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                                $"gameboy.memory.WriteByte(value, gameboy.CPU.registers.{parameters[1]});"
                            };
                        }
                        else
                        {
                            cycles = 5;
                            operandLength = 2;
                            instructionCodeLines = new string[]
                            {
                                $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                                $"gameboy.memory.WriteUshort(value, gameboy.CPU.registers.{parameters[1]});"
                            };
                        } 
                    }
                    else if(Regex.IsMatch(parameters[0], "(^\\(AF\\)$)|(^\\(BC\\)$)|(^\\(DE\\)$)|(^\\(HL\\)$)"))
                    {
                        if(Regex.IsMatch(parameters[1], "n{1}"))
                        {
                            cycles = 3;
                            operandLength = 1;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.memory.WriteByte(gameboy.CPU.registers.{Regex.Replace(parameters[0],"\\(|\\)","")}, operands[0]);"
                            };
                        }
                        else if(Regex.IsMatch(parameters[1], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                        {
                            cycles = 2;
                            operandLength = 0;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.memory.WriteByte(gameboy.CPU.registers.{Regex.Replace(parameters[0],"\\(|\\)","")}, gameboy.CPU.registers.{parameters[1]});"
                            };
                        }  
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //LDD
            {
                "LDD",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 2;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.memory.WriteByte(gameboy.CPU.registers.HL, gameboy.CPU.registers.A);",
                            $"gameboy.CPU.registers.HL--;"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^A$"))
                    {
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.registers.A = gameboy.memory.ReadByte(gameboy.CPU.registers.HL);",
                            $"gameboy.CPU.registers.HL--;"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //LDI
            {
                "LDI",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 2;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.memory.WriteByte(gameboy.CPU.registers.HL, gameboy.CPU.registers.A);",
                            $"gameboy.CPU.registers.HL++;"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^A$"))
                    {
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.registers.A = gameboy.memory.ReadByte(gameboy.CPU.registers.HL);",
                            $"gameboy.CPU.registers.HL++;"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //LDH
            {
                "LDH",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "^\\(n\\)$"))
                    {
                        cycles = 3;
                        operandLength = 1;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.memory.WriteByte((ushort)(0xFF00 + operands[0]), gameboy.CPU.registers.A);"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(C\\)$"))
                    {
                        cycles = 2;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.memory.WriteByte((ushort)(0xFF00 + gameboy.CPU.registers.C), gameboy.CPU.registers.A);"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^A$"))
                    {
                        cycles = 3;
                        operandLength = 1;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.registers.A = gameboy.memory.ReadByte((ushort)(0xFF00 + operands[0]));"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //LDHL
            {
                "LDHL",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 3;
                    int operandLength = 1;
                    string[] instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.registers.HL = (ushort)(gameboy.CPU.registers.SP + (sbyte)operands[0]);"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //PUSH
            {
                "PUSH",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 4;
                    int operandLength = 1;
                    string[] instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.registers.SP -= 2;",
                        $"gameboy.memory.WriteUshort(gameboy.CPU.registers.SP, gameboy.CPU.registers.{parameters[0]});"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //POP
            {
                "POP",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 3;
                    int operandLength = 1;
                    string[] instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.registers.{parameters[0]} = gameboy.memory.ReadUshort(gameboy.CPU.registers.SP);",
                        $"gameboy.CPU.registers.SP += 2;",
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            #endregion

            #region Arithmetic
            //ADD
            {
                "ADD",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];
                    
                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        if(Regex.IsMatch(parameters[1], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                        {
                            cycles = 1;
                            operandLength = 0;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.AddByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, gameboy.CPU.registers.{parameters[1]}, false);"
                            };
                        }
                        else if(Regex.IsMatch(parameters[1], "^\\(HL\\)$"))
                        {
                            cycles = 2;
                            operandLength = 1;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.AddByte(" +
                                $"gameboy.CPU.registers, " +
                                $"CPU.IALU.TargetRegister.{parameters[0]}, " +
                                $"gameboy.memory.ReadByte(gameboy.CPU.registers.HL), " +
                                $"false" +
                                $");"
                            };
                        }
                        else
                        {
                            cycles = 2;
                            operandLength = 2;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.AddByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, operands[0], false);"
                            };
                        }
                    }
                    else if(Regex.IsMatch(parameters[0], "^HL$"))
                    {
                        cycles = 2;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.AddUshort(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, gameboy.CPU.registers.{parameters[1]}, false);"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^SP$"))
                    {
                        instruction += " *NOT USING ALU IMPLEMENTATION!!!";
                        cycles = 4;
                        operandLength = 1;
                        instructionCodeLines = new string[]
                        {
                            "int originalValue = gameboy.CPU.registers.SP;",
                            "int result = gameboy.CPU.registers.SP + operands[0];",
                            "gameboy.CPU.registers.SP = (ushort)result;",
                            "gameboy.CPU.registers.NegativeFlag = false;",
                            "gameboy.CPU.registers.ZeroFlag = false;",
                            "if ((((originalValue & 0x0F) + (originalValue & 0x0F)) & 0x10) == 0x10)",
                            "\tgameboy.CPU.registers.HalfCarryFlag = true;",
                            "if (result > byte.MaxValue)",
                            "\tgameboy.CPU.registers.FullCarryFlag = true;"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //ADC
            {
                "ADC",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        if(Regex.IsMatch(parameters[1], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                        {
                            cycles = 1;
                            operandLength = 0;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.AddByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, gameboy.CPU.registers.{parameters[1]}, true);"
                            };
                        }
                        else if(Regex.IsMatch(parameters[1], "^\\(HL\\)$"))
                        {
                            cycles = 2;
                            operandLength = 1;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.AddByte(" +
                                $"gameboy.CPU.registers, " +
                                $"CPU.IALU.TargetRegister.{parameters[0]}, " +
                                $"gameboy.memory.ReadByte(gameboy.CPU.registers.HL), " +
                                $"true" +
                                $");"
                            };
                        }
                        else
                        {
                            cycles = 2;
                            operandLength = 2;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.AddByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, operands[0], true);"
                            };
                        }
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //SUB
            {
                "SUB",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        if(Regex.IsMatch(parameters[1], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                        {
                            cycles = 1;
                            operandLength = 0;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.SubtractByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, gameboy.CPU.registers.{parameters[1]}, false);"
                            };
                        }
                        else if(Regex.IsMatch(parameters[1], "^\\(HL\\)$"))
                        {
                            cycles = 2;
                            operandLength = 1;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.SubtractByte(" +
                                $"gameboy.CPU.registers, " +
                                $"CPU.IALU.TargetRegister.{parameters[0]}, " +
                                $"gameboy.memory.ReadByte(gameboy.CPU.registers.HL), " +
                                $"false" +
                                $");"
                            };
                        }
                        else
                        {
                            cycles = 2;
                            operandLength = 2;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.SubtractByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, operands[0], false);"
                            };
                        }
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //SBC
            {
                "SBC",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        if(Regex.IsMatch(parameters[1], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                        {
                            cycles = 1;
                            operandLength = 0;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.SubtractByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, gameboy.CPU.registers.{parameters[1]}, true);"
                            };
                        }
                        else if(Regex.IsMatch(parameters[1], "^\\(HL\\)$"))
                        {
                            cycles = 2;
                            operandLength = 1;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.SubtractByte(" +
                                $"gameboy.CPU.registers, " +
                                $"CPU.IALU.TargetRegister.{parameters[0]}, " +
                                $"gameboy.memory.ReadByte(gameboy.CPU.registers.HL), " +
                                $"true" +
                                $");"
                            };
                        }
                        else
                        {
                            cycles = 2;
                            operandLength = 2;
                            instructionCodeLines = new string[]
                            {
                                $"gameboy.CPU.ALU.SubtractByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, operands[0], true);"
                            };
                        }
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //AND
            {
                "AND",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        cycles = 1;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.AndBitwiseByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, gameboy.CPU.registers.{parameters[0]});"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 2;
                        operandLength = 1;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.AndBitwiseByte(" +
                            $"gameboy.CPU.registers, " +
                            $"CPU.IALU.TargetRegister.A, " +
                            $"gameboy.memory.ReadByte(gameboy.CPU.registers.HL)" +
                            $");"
                        };
                    }
                    else
                    {
                        cycles = 2;
                        operandLength = 2;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.AndBitwiseByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, operands[0]);"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //OR
            {
                "OR",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        cycles = 1;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.OrBitwiseByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, gameboy.CPU.registers.{parameters[0]});"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 2;
                        operandLength = 1;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.OrBitwiseByte(" +
                            $"gameboy.CPU.registers, " +
                            $"CPU.IALU.TargetRegister.A, " +
                            $"gameboy.memory.ReadByte(gameboy.CPU.registers.HL)" +
                            $");"
                        };
                    }
                    else
                    {
                        cycles = 2;
                        operandLength = 2;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.OrBitwiseByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, operands[0]);"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //XOR
            {
                "XOR",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        cycles = 1;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.XorBitwiseByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, gameboy.CPU.registers.{parameters[0]});"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 2;
                        operandLength = 1;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.XorBitwiseByte(" +
                            $"gameboy.CPU.registers, " +
                            $"CPU.IALU.TargetRegister.A, " +
                            $"gameboy.memory.ReadByte(gameboy.CPU.registers.HL)" +
                            $");"
                        };
                    }
                    else
                    {
                        cycles = 2;
                        operandLength = 2;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.XorBitwiseByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, operands[0]);"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //CP
            {
                "CP",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        cycles = 1;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.CompareByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, gameboy.CPU.registers.{parameters[0]});"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 2;
                        operandLength = 1;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.CompareByte(" +
                            $"gameboy.CPU.registers, " +
                            $"CPU.IALU.TargetRegister.A, " +
                            $"gameboy.memory.ReadByte(gameboy.CPU.registers.HL)" +
                            $");"
                        };
                    }
                    else
                    {
                        cycles = 2;
                        operandLength = 2;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.CompareByte(gameboy.CPU.registers, CPU.IALU.TargetRegister.A, operands[0]);"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //INC
            {
                "INC",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        cycles = 1;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.IncrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]});"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 3;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.IncrementMemory(gameboy.CPU.registers, gameboy.memory, gameboy.CPU.registers.HL);"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "(^AF$)|(^BC$)|(^DE$)|(^HL$)|(^SP$)"))
                    {
                        cycles = 2;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.IncrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]});"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //DEC
            {
                "DEC",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        cycles = 1;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.DecrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]});"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 3;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.DecrementMemory(gameboy.CPU.registers, gameboy.memory, gameboy.CPU.registers.HL);"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "(^AF$)|(^BC$)|(^DE$)|(^HL$)|(^SP$)"))
                    {
                        cycles = 2;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.DecrementRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]});"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            #endregion

            #region Miscellaneous
            //SWAP
            {
                "SWAP",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        cycles = 2;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.registers.{parameters[0]} = (byte)(((gameboy.CPU.registers.{parameters[0]} & 0x0F) << 4) | ((gameboy.CPU.registers.{parameters[0]} & 0xF0) >> 4));"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 4;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"byte value = 0;"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //DAA
            {
                "DAA",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 1;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[]
                    {
                        $"byte value = gameboy.CPU.registers.A;",
                        $"byte result = (byte)(((value / 10) << 4)|((value % 10) << 0));",
                        $"gameboy.CPU.registers.ZeroFlag = (result == 0? true : false);"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //CPL
            {
                "CPL",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 1;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.registers.A = (byte)~gameboy.CPU.registers.A;",
                        $"gameboy.CPU.registers.NegativeFlag = true;",
                        $"gameboy.CPU.registers.HalfCarryFlag = true;"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //CCF
            {
                "CCF",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 1;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.registers.FullCarryFlag = !gameboy.CPU.registers.FullCarryFlag;",
                        $"gameboy.CPU.registers.NegativeFlag = false;",
                        $"gameboy.CPU.registers.HalfCarryFlag = false;"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //SCF
            {
                "SCF",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 1;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.registers.FullCarryFlag = true;",
                        $"gameboy.CPU.registers.NegativeFlag = false;",
                        $"gameboy.CPU.registers.HalfCarryFlag = false;"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //NOP
            {
                "NOP",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 1;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[0];

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //HALT
            {
                "HALT",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 0;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[0];

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //STOP
            {
                "STOP",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 0;
                    int operandLength = 1;
                    string[] instructionCodeLines = new string[0];

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //DI
            {
                "DI",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 1;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.IME = false;"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //EI
            {
                "EI",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 1;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.IME = true;"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //Ext Ops
            {
                "Ext",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 0;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[]
                    {
                        
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            #endregion

            #region Rotates and Shifts
            //RLC
            {
                "RLC",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)"))
                    {
                        cycles = 1;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.RotateRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, CPU.IALU.RotateDirection.Left, true);"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        cycles = 2;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.RotateRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, CPU.IALU.RotateDirection.Left, true);"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 4;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"byte value = gameboy.memory.ReadByte(gameboy.CPU.registers.HL);",
                            $"gameboy.CPU.registers.FullCarryFlag = (value & 0x80) != 0? true : false);",
                            $"value = (byte)((value << 1) | ((value & 0x80) != 0? 1 : 0));",
                            $"gameboy.memory.WriteByte(value);"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //RL
            {
                "RL",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "(^A$)|(^B$)|(^C$)|(^D$)|(^E$)|(^F$)|(^H$)|(^L$)"))
                    {
                        cycles = 2;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.ALU.RotateRegister(gameboy.CPU.registers, CPU.IALU.TargetRegister.{parameters[0]}, CPU.IALU.RotateDirection.Left, true);"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 4;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"byte value = gameboy.memory.ReadByte(gameboy.CPU.registers.HL);",
                            $"gameboy.CPU.registers.FullCarryFlag = (value & 0x80) != 0? true : false);",
                            $"value = (byte)((value << 1) | ((value & 0x80) != 0? 1 : 0));",
                            $"gameboy.memory.WriteByte(value);"
                        };
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            {
                "RRC",
                null
            },
            {
                "RR",
                null
            },
            {
                "SLA",
                null
            },
            {
                "SRA",
                null
            },
            {
                "SRL",
                null
            },
            #endregion

            #region Bit
            {
                "BIT",
                null
            },
            {
                "SET",
                null
            },
            {
                "RES",
                null
            },
            #endregion

            #region Jumps
            //JP
            {
                "JP",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "^nn$"))
                    {
                        cycles = 4;
                        operandLength = 2;
                        instructionCodeLines = new string[]
                        {
                            $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                            $"gameboy.CPU.registers.PC = value;"
                        };
                    }
                    else if(Regex.IsMatch(parameters[0], "^\\(HL\\)$"))
                    {
                        cycles = 1;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.registers.PC = gameboy.CPU.registers.HL;"
                        };
                    }
                    else
                    {
                        cycles = 3;
                        operandLength = 2;
                        if(Regex.IsMatch(parameters[0], "Z"))
                        {
                            instructionCodeLines = new string[]
                            {
                                $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                                $"if({(Regex.IsMatch(parameters[0], "N")? "!":"")}gameboy.CPU.registers.ZeroFlag)",
                                $"{{",
                                $"\tgameboy.CPU.registers.PC = value;",
                                $"\tgameboy.CPU.BusyCycles++;",
                                $"}}"
                            };
                        }
                        else if(Regex.IsMatch(parameters[0], "C"))
                        {
                            instructionCodeLines = new string[]
                            {
                                $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                                $"if({(Regex.IsMatch(parameters[0], "N")? "!":"")}gameboy.CPU.registers.FullCarryFlag)",
                                $"{{",
                                $"\tgameboy.CPU.registers.PC = value;",
                                $"\tgameboy.CPU.BusyCycles++;",
                                $"}}"
                            };
                        }
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //JR
            {
                "JR",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "^nn$"))
                    {
                        cycles = 3;
                        operandLength = 1;
                        instructionCodeLines = new string[]
                        {
                            $"sbyte value = (sbyte)operands[0];",
                            $"gameboy.CPU.registers.PC = (ushort)(gameboy.CPU.registers.PC + value);"
                        };
                    }
                    else
                    {
                        cycles = 2;
                        operandLength = 1;
                        if(Regex.IsMatch(parameters[0], "Z"))
                        {
                            instructionCodeLines = new string[]
                            {
                                $"sbyte value = (sbyte)operands[0];",
                                $"if({(Regex.IsMatch(parameters[0], "N")? "!":"")}gameboy.CPU.registers.ZeroFlag)",
                                $"{{",
                                $"\tgameboy.CPU.registers.PC = (ushort)(gameboy.CPU.registers.PC + value);",
                                $"\tgameboy.CPU.BusyCycles++;",
                                $"}}"
                            };
                        }
                        else if(Regex.IsMatch(parameters[0], "C"))
                        {
                            instructionCodeLines = new string[]
                            {
                                $"sbyte value = (sbyte)operands[0];",
                                $"if({(Regex.IsMatch(parameters[0], "N")? "!":"")}gameboy.CPU.registers.FullCarryFlag)",
                                $"{{",
                                $"\tgameboy.CPU.registers.PC = (ushort)(gameboy.CPU.registers.PC + value);",
                                $"\tgameboy.CPU.BusyCycles++;",
                                $"}}"
                            };
                        }
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            #endregion

            #region Calls
            //CALL
            {
                "CALL",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(Regex.IsMatch(parameters[0], "^nn$"))
                    {
                        cycles = 6;
                        operandLength = 2;
                        instructionCodeLines = new string[]
                        {
                            $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                            $"gameboy.CPU.registers.SP -= 2;",
                            $"gameboy.memory.WriteUshort(gameboy.CPU.registers.SP, gameboy.CPU.registers.PC);",
                            $"gameboy.CPU.registers.PC = value;"
                        };
                    }
                    else
                    {
                        cycles = 3;
                        operandLength = 2;
                        if(Regex.IsMatch(parameters[0], "Z"))
                        {
                            instructionCodeLines = new string[]
                            {
                                $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                                $"if({(Regex.IsMatch(parameters[0], "N")? "!":"")}gameboy.CPU.registers.ZeroFlag)",
                                $"{{",
                                $"\tgameboy.CPU.registers.SP -= 2;",
                                $"\tgameboy.memory.WriteUshort(gameboy.CPU.registers.SP, gameboy.CPU.registers.PC);",
                                $"\tgameboy.CPU.registers.PC = value;",
                                $"\tgameboy.CPU.BusyCycles += 3;",
                                $"}}"
                            };
                        }
                        else if(Regex.IsMatch(parameters[0], "C"))
                        {
                            instructionCodeLines = new string[]
                            {
                                $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                                $"if({(Regex.IsMatch(parameters[0], "N")? "!":"")}gameboy.CPU.registers.FullCarryFlag)",
                                $"{{",
                                $"\tgameboy.CPU.registers.SP -= 2;",
                                $"\tgameboy.memory.WriteUshort(gameboy.CPU.registers.SP, gameboy.CPU.registers.PC);",
                                $"\tgameboy.CPU.registers.PC = value;",
                                $"\tgameboy.CPU.BusyCycles += 3;",
                                $"}}"
                            };
                        }
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            #endregion

            #region Restarts
            //RST
            {
                "RST",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 4;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[0];

                    instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.registers.SP -= 2;",
                        $"gameboy.memory.WriteUshort(gameboy.CPU.registers.SP, gameboy.CPU.registers.PC);",
                        $"gameboy.CPU.registers.PC = (ushort) 0x{parameters[0]};"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            #endregion

            #region Returns
            //RET
            {
                "RET",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = null;
                    try
                    {
                        parameters = instruction.Split(' ')[1].Split(',');
                    }
                    catch
                    {
                        parameters = new string[0];
                    }

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 9999;
                    int operandLength = 8888;
                    string[] instructionCodeLines = new string[0];

                    if(parameters.Length == 0)
                    {
                        cycles = 4;
                        operandLength = 0;
                        instructionCodeLines = new string[]
                        {
                            $"gameboy.CPU.registers.PC = gameboy.memory.ReadUshort(gameboy.CPU.registers.SP);",
                            $"gameboy.CPU.registers.SP += 2;"
                        };
                    }
                    else
                    {
                        cycles = 2;
                        operandLength = 0;
                        if(Regex.IsMatch(parameters[0], "Z"))
                        {
                            instructionCodeLines = new string[]
                            {
                                $"if({(Regex.IsMatch(parameters[0], "N")? "!":"")}gameboy.CPU.registers.ZeroFlag)",
                                $"{{",
                                $"\tgameboy.CPU.registers.PC = gameboy.memory.ReadUshort(gameboy.CPU.registers.SP);",
                                $"\tgameboy.CPU.registers.SP += 2;",
                                $"\tgameboy.CPU.BusyCycles += 3;",
                                $"}}"
                            };
                        }
                        else if(Regex.IsMatch(parameters[0], "C"))
                        {
                            instructionCodeLines = new string[]
                            {
                                $"ushort value = (ushort)((operands[0] << 0) | (operands[1] << 8));",
                                $"if({(Regex.IsMatch(parameters[0], "N")? "!":"")}gameboy.CPU.registers.FullCarryFlag)",
                                $"{{",
                                $"\tgameboy.CPU.registers.PC = gameboy.memory.ReadUshort(gameboy.CPU.registers.SP);",
                                $"\tgameboy.CPU.registers.SP += 2;",
                                $"\tgameboy.CPU.BusyCycles += 3;",
                                $"}}"
                            };
                        }
                    }

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            //RETI
            {
                "RETI",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");

                    int cycles = 4;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[]
                    {
                        $"gameboy.CPU.IME = true;",
                        $"gameboy.CPU.registers.PC = gameboy.memory.ReadUshort(gameboy.CPU.registers.SP);",
                        $"gameboy.CPU.registers.SP += 2;"
                    };

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            #endregion

            #region Removed
            {
                "XX",
                (int index, string instruction ,TextFormatter textFormatter) =>
                {
                    WriteInstruction(textFormatter, "Removed", index, "", 0, 0, null);
                }
            },
            #endregion
        };

        static void WriteInstruction(TextFormatter textFormatter, string comment, int opCode, string disassembly, int cycles, int operandLength, string[] codeLines)
        {
            textFormatter.AppendLine($"//{comment}");
            textFormatter.AppendLine("{");
            textFormatter.AppendLine($"0x{opCode.ToString("X2")},", TextFormatter.IndentChange.Increase);
            textFormatter.AppendLine("new Instruction()");
            textFormatter.AppendLine("{");
            textFormatter.AppendLine($"disassembly = \"{disassembly}\",", TextFormatter.IndentChange.Increase);
            textFormatter.AppendLine($"cycles = {cycles},");
            textFormatter.AppendLine($"operandLength = {operandLength},");
            textFormatter.AppendLine("method = (gameboy, operands) =>");
            textFormatter.AppendLine("{");
            textFormatter.IncreaseIndentLevel();
            for (int i = 0; codeLines != null && i < codeLines.Length; i++)
            {
                textFormatter.AppendLine(codeLines[i]);
            }
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("},", TextFormatter.IndentChange.Decrease);
        }

        static void Main(string[] args)
        {
            if(!File.Exists(cleanDumpPath))
            {
                Fetching();

                while (isFetching)
                {
                    Thread.Sleep(1000);
                }
            }
            else
            {
                opCodes = new List<string>();
                StreamReader reader = new StreamReader(File.OpenRead(cleanDumpPath));
                while(!reader.EndOfStream)
                {
                    opCodes.Add(reader.ReadLine());
                }
            }

            if(opCodes.Count > 0)
            {
                Parsing();
                while(isParsing)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        static async void Fetching()
        {
            HttpResponseMessage message = await httpClient.GetAsync("http://imrannazar.com/Gameboy-Z80-Opcode-Map");
            MatchCollection matches = Regex.Matches(
                await message.Content.ReadAsStringAsync(),
                "<td><abbr title=[a-z,A-Z,\",', ,\\-,+,0-9,\\(,\\)]*>[A-Z,a-z,0-9, ,\\,,\\(,\\)]*<\\/abbr><\\/td>\n");

            List<string> opCodeList = matches.Cast<Match>().Select(match => match.Value).ToList();
            List<string> cleanOpCodeList = new List<string>();

            Console.WriteLine(opCodeList.Count);
            for(int i=0; i<opCodeList.Count; i++)
            {
                Console.Write(opCodeList[i]);
            }
            Console.Write("\n\n\n");

            StreamWriter writer = File.CreateText(dirtyDumpPath);
            for(int i=0; i<opCodeList.Count; i++)
            {
                cleanOpCodeList.Add(Regex.Replace(opCodeList[i], "<td><abbr title=[a-z,A-Z,\", ', ,\\-,+,0-9,\\(,\\)]*>|<\\/abbr><\\/td>\\n", ""));
                writer.Write(opCodeList[i]);
            }
            writer.Close();
            writer.Dispose();

            writer = File.CreateText(cleanDumpPath);
            for(int i=0; i<cleanOpCodeList.Count; i++)
            {
                Console.WriteLine(cleanOpCodeList[i]);
                writer.WriteLine(cleanOpCodeList[i]);
            }
            writer.Close();
            writer.Dispose();

            opCodes = cleanOpCodeList;

            isFetching = false;
        }

        static async void Parsing()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            TextFormatter textFormatter = new TextFormatter("\t");

            List<string> unparsedInstructions = new List<string>();
            int unparsedCount = 0;

            #region Base File Start
            textFormatter.AppendLine("// --- AUTO GENERATED CODE --- //");
            textFormatter.AppendLine("using System;");
            textFormatter.AppendLine("using System.Collections.Generic;");
            textFormatter.AppendLine("using System.Text;");
            textFormatter.AppendLine("");
            textFormatter.AppendLine("namespace GBBase");
            textFormatter.AppendLine("{");
            textFormatter.AppendLine("public partial class Interpreter", TextFormatter.IndentChange.Increase);
            textFormatter.AppendLine("{");
            textFormatter.AppendLine("public delegate void InstructionMethod(GameboyHardware gameboy, params byte[] operands);", TextFormatter.IndentChange.Increase);
            textFormatter.AppendLine("");
            textFormatter.AppendLine("public struct Instruction");
            textFormatter.AppendLine("{");
            textFormatter.AppendLine("public string disassembly;", TextFormatter.IndentChange.Increase);
            textFormatter.AppendLine("public byte cycles;");
            textFormatter.AppendLine("public byte operandLength;");
            textFormatter.AppendLine("public InstructionMethod method;");
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("");
            textFormatter.AppendLine("private static void AssertOperands(byte length, byte[] operands)");
            textFormatter.AppendLine("{");
            textFormatter.AppendLine("if(", TextFormatter.IndentChange.Increase);
            textFormatter.AppendLine("(length > 0 && length > operands.Length) ||", TextFormatter.IndentChange.Increase);
            textFormatter.AppendLine("(length == 0 && (operands != null || operands.Length > 0))");
            textFormatter.AppendLine(")");
            textFormatter.AppendLine("{", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("throw new ArgumentException(\"invalid operands\");", TextFormatter.IndentChange.Increase);
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("");
            #endregion

            #region Instruction Set
            textFormatter.AppendLine("private static Dictionary<byte, Instruction> _instructionSet = new Dictionary<byte, Instruction>()");
            textFormatter.AppendLine("{");
            textFormatter.IncreaseIndentLevel();

            for (int i = 0; i < opCodes.Count && i < 256; i++)
            {
                string opCodeKey = opCodes[i].Split(' ')[0];
                //Console.WriteLine(opCodeKey);
                if (instructionParsers.ContainsKey(opCodeKey))
                {
                    string generatedCode = null;
                    if (instructionParsers[opCodeKey] != null)
                        instructionParsers[opCodeKey].Invoke(i, opCodes[i], textFormatter);
                    else
                    {
                        if(!unparsedInstructions.Contains(opCodeKey))
                        {
                            unparsedInstructions.Add(opCodeKey);
                        }
                        unparsedCount++;
                    }
                    

                    if (generatedCode != null)
                        builder.Append(generatedCode);
                }
                else
                {
                    Console.WriteLine($"No parser found for {i.ToString("X2")} -> \"{opCodes[i]}\"!!!");
                    unparsedInstructions.Add(opCodeKey);
                    unparsedCount++;
                }
            }

            textFormatter.AppendLine("};", TextFormatter.IndentChange.Decrease);
            #endregion

            textFormatter.AppendLine("");

            #region Extended Instruction Set
            textFormatter.AppendLine("private static Dictionary<byte, Instruction> _extendedInstructionSet = new Dictionary<byte, Instruction>()");
            textFormatter.AppendLine("{");
            textFormatter.IncreaseIndentLevel();

            for (int i = 256; i < opCodes.Count; i++)
            {
                string opCodeKey = opCodes[i].Split(' ')[0];
                //Console.WriteLine(opCodeKey);
                if (instructionParsers.ContainsKey(opCodeKey))
                {
                    string generatedCode = null;
                    if (instructionParsers[opCodeKey] != null)
                        instructionParsers[opCodeKey].Invoke(i-256, opCodes[i], textFormatter);
                    else
                    {
                        if (!unparsedInstructions.Contains(opCodeKey))
                        {
                            unparsedInstructions.Add(opCodeKey);
                        }
                        unparsedCount++;
                    }


                    if (generatedCode != null)
                        builder.Append(generatedCode);
                }
                else
                {
                    Console.WriteLine($"No parser found for {i.ToString("X2")} -> \"{opCodes[i]}\"!!!");
                    unparsedInstructions.Add(opCodeKey);
                    unparsedCount++;
                }
            }

            textFormatter.AppendLine("};", TextFormatter.IndentChange.Decrease);
            #endregion

            #region Base File End
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            #endregion

            StreamWriter writer = File.CreateText(instructionSetClassPath);
            writer.Write(textFormatter.ToString());
            writer.Close();
            writer.Dispose();

            Console.WriteLine($"Couldn't parse {unparsedCount} opCodes");
            Console.WriteLine("Unparsed List:");
            foreach (string opCodeKey in unparsedInstructions)
                Console.WriteLine($"\t{opCodeKey}");

            isParsing = false;
        }
    }
}
