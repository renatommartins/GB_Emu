using System;
using System.Collections.Generic;
using System.Text;

namespace GBBase
{
    public partial class Interpreter
    {
        public void Execute(GameboyHardware gameboy, byte opcode, params byte[] operands)
        {
            AssertOperands(_instructionSet[opcode].operandLength, operands);
            gameboy.CPU.BusyCycles += _instructionSet[opcode].cycles;
            _instructionSet[opcode].method(gameboy, operands);
        }

        public Instruction GetInstruction(byte opcode)
        {
            if(!_instructionSet.ContainsKey(opcode))
            {
                throw new NotImplementedException($"opcode {opcode:X2} not implemented");
            }
            return _instructionSet[opcode];
        }

        public string Disassembly(byte opcode)
        {
            return _instructionSet[opcode].disassembly;
        }
    }
}
