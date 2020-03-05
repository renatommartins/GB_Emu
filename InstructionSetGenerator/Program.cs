using System;
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
            {
                "LD",
                (int index, string instruction , TextFormatter textFormatter) =>
                {
                    string[] parameters = instruction.Split(' ')[1].Split(',');

                    string disassembly = Regex.Replace(instruction,"n{1,2}", "0x{0:X}");
                    int cycles = 0;
                    int operandLength = 0;
                    string[] instructionCodeLines = new string[0];

                    WriteInstruction(textFormatter, instruction, index, disassembly, cycles, operandLength, instructionCodeLines);
                }
            },
            {
                "LDD",
                null
            },
            {
                "LDI",
                null
            },
            {
                "LDH",
                null
            },
            {
                "LDHL",
                null
            },
            {
                "PUSH",
                null
            },
            {
                "POP",
                null
            },
            #endregion

            #region Arithmetic
            {
                "ADD",
                null
            },
            {
                "ADC",
                null
            },
            {
                "SUB",
                null
            },
            {
                "SBC",
                null
            },
            {
                "AND",
                null
            },
            {
                "OR",
                null
            },
            {
                "XOR",
                null
            },
            {
                "CP",
                null
            },
            {
                "INC",
                null
            },
            {
                "DEC",
                null
            },
            #endregion

            #region Miscellaneous
            {
                "SWAP",
                null
            },
            {
                "DAA",
                null
            },
            {
                "CPL",
                null
            },
            {
                "CCF",
                null
            },
            {
                "SCF",
                null
            },
            {
                "NOP",
                null
            },
            {
                "HALT",
                null
            },
            {
                "STOP",
                null
            },
            {
                "DI",
                null
            },
            {
                "EI",
                null
            },
            #endregion

            #region Rotates and Shifts
            {
                "RLCA",
                null
            },
            {
                "RLA",
                null
            },
            {
                "RRCA",
                null
            },
            {
                "RRA",
                null
            },
            {
                "RLC",
                null
            },
            {
                "RL",
                null
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
            {
                "JP",
                null
            },
            {
                "JR",
                null
            },
            #endregion

            #region Calls
            {
                "CALL",
                null
            },
            #endregion

            #region Restarts
            {
                "RST",
                null
            },
            #endregion

            #region Returns
            {
                "RET",
                null
            },
            {
                "RETI",
                null
            },
            #endregion

            #region Removed
            {
                "XX",
                (int index, string instruction ,TextFormatter textFormatter) =>
                {
                    textFormatter.AppendLine("//Removed");
                    textFormatter.AppendLine("{");
                    textFormatter.AppendLine($"0x{index.ToString("X2")},", TextFormatter.IndentChange.Increase);
                    textFormatter.AppendLine("new Instruction()");
                    textFormatter.AppendLine("{");
                    textFormatter.AppendLine("disassembly = \"\",", TextFormatter.IndentChange.Increase);
                    textFormatter.AppendLine("cycles: 0,");
                    textFormatter.AppendLine("operandLength: 0,");
                    textFormatter.AppendLine("method = (gameboy, length) =>");
                    textFormatter.AppendLine("{");
                    textFormatter.AppendLine("throw new Exception(\"Instruction removed\");", TextFormatter.IndentChange.Increase);
                    textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
                    textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
                    textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);

                    /*
                    autoGeneratedCode =
                    $"\t\t\t//Removed\n" +
                    $"\t\t\t{{\n" +
                    $"\t\t\t\t0x{index.ToString("X2")},\n" +
                    $"\t\t\t\tnew Instruction()\n" +
                    $"\t\t\t\t{{\n" +
                    $"\t\t\t\t\tdisassembly = \"\"\n" +
                    $"\t\t\t\t\tcycles = 0\n" +
                    $"\t\t\t\t\toperandLength = 0\n" +
                    $"\t\t\t\t\tmethod = (gameboy, operands) =>\n" +
                    $"\t\t\t\t\t{{\n" +
                    $"\t\t\t\t\t\tthrow new Exception(\"Instruction removed\");\n" +
                    $"\t\t\t\t\t}}\n" +
                    $"\t\t\t\t}}\n" +
                    $"\t\t\t}},\n";
                    */
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
            textFormatter.AppendLine($"cycles: {cycles},");
            textFormatter.AppendLine($"operandLength: {operandLength},");
            textFormatter.AppendLine("method = (gameboy, length) =>");
            textFormatter.AppendLine("{");
            textFormatter.IncreaseIndentLevel();
            for (int i = 0; i < codeLines.Length; i++)
            {
                textFormatter.AppendLine(codeLines[i]);
            }
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
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

            #region Base File Start
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
            textFormatter.AppendLine("private static Dictionary<byte, Instruction> _instructionSet = new Dictionary<byte, Instruction>()");
            textFormatter.AppendLine("{");
            textFormatter.IncreaseIndentLevel();
            #endregion

            for (int i = 0; i < opCodes.Count && i <256; i++)
            {
                string opCodeKey = opCodes[i].Split(' ')[0];
                //Console.WriteLine(opCodeKey);
                if (instructionParsers.ContainsKey(opCodeKey))
                {
                    string generatedCode = null;
                    instructionParsers[opCodeKey]?.Invoke(i, opCodes[i], textFormatter);
                    if(generatedCode != null)
                        builder.Append(generatedCode);
                }
                else
                {
                    Console.WriteLine($"No parser found for {i.ToString("X2")} -> \"{opCodes[i]}\"!!!");
                }
            }

            #region Base File End
            textFormatter.AppendLine("};", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            textFormatter.AppendLine("}", TextFormatter.IndentChange.Decrease);
            #endregion

            StreamWriter writer = File.CreateText(instructionSetClassPath);
            writer.Write(textFormatter.ToString());
            writer.Close();
            writer.Dispose();

            isParsing = false;
        }
    }
}
