using System;
using System.Collections.Generic;
using System.Text;

namespace GBBase.CPU
{
    public interface IALU
    {
        public enum TargetRegister
        { 
            A,
            F,
            AF,
            B,
            C,
            BC,
            D,
            E,
            DE,
            H,
            L,
            HL
        }

        public enum RotateDirection
        {
            Left,
            Right
        }

        public void AddByte(IRegisters registers, TargetRegister targetRegister, byte value, bool useCarry);
        public void AddSbyte(IRegisters registers, TargetRegister targetRegister, sbyte value, bool useCarry);
        public void AddUshort(IRegisters registers, TargetRegister targetRegister, ushort value, bool useCarry);
        public void SubtractByte(IRegisters registers, TargetRegister targetRegister, byte value, bool useCarry);
        public void SubtractSbyte(IRegisters registers, TargetRegister targetRegister, sbyte value, bool useCarry);
        public void SubtractUshort(IRegisters registers, TargetRegister targetRegister, ushort value, bool useCarry);
        public void RotateRegister(IRegisters registers, TargetRegister targetRegister, RotateDirection direction, bool useCarry);
        public void IncrementRegister(IRegisters registers, TargetRegister targetRegister);
        public void IncrementMemory(IRegisters registers, Memory.IMemory memory, ushort address);
        public void DecrementRegister(IRegisters registers, TargetRegister targetRegister);
        public void DecrementMemory(IRegisters registers, Memory.IMemory memory, ushort address);
        public void AndBitwiseByte(IRegisters registers, TargetRegister targetRegister, byte value);
        public void OrBitwiseByte(IRegisters registers, TargetRegister targetRegister, byte value);
        public void XorBitwiseByte(IRegisters registers, TargetRegister targetRegister, byte value);
        public void TestBitByte(IRegisters registers, int bit, byte value);
        public void CompareByte(IRegisters registers, TargetRegister targetRegister, byte value);
    }
}
