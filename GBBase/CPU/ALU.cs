using System;
using System.Collections.Generic;
using System.Text;

using GBBase.Memory;

namespace GBBase.CPU
{
    public class ALU : IALU
    {
        private bool EvaluateHalfCarrySubtraction(byte value, byte sub)
        {
            return ((value & 0x0F) - (sub & 0x0F)) < 0;
        }

        public void AddByte(IRegisters registers, IALU.TargetRegister targetRegister, byte value, bool useCarry)
        {
            int result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                default:
                    throw new ArgumentException();
            }
            int originalValue = result;
            result += value + (useCarry && registers.FullCarryFlag? 1:0);

            registers.NegativeFlag = false;
            if ((result & 0xFF) == 0)
                registers.ZeroFlag = true;
            if ((((originalValue & 0x0F) + (value & 0x0F)) & 0x10) == 0x10)
                registers.HalfCarryFlag = true;
            if(result > byte.MaxValue)
                registers.FullCarryFlag = true;

            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    registers.A = (byte)result;
                    break;
                case IALU.TargetRegister.F:
                    registers.F = (byte)result;
                    break;
                case IALU.TargetRegister.B:
                    registers.B = (byte)result;
                    break;
                case IALU.TargetRegister.C:
                    registers.C = (byte)result;
                    break;
                case IALU.TargetRegister.D:
                    registers.D = (byte)result;
                    break;
                case IALU.TargetRegister.E:
                    registers.E = (byte)result;
                    break;
                case IALU.TargetRegister.H:
                    registers.H = (byte)result;
                    break;
                case IALU.TargetRegister.L:
                    registers.L = (byte)result;
                    break;
            }
        }

        public void AddSbyte(IRegisters registers, IALU.TargetRegister targetRegister, sbyte value, bool useCarry)
        {
            int result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                default:
                    throw new ArgumentException();
            }
            int originalValue = result;
            result += value + (useCarry && registers.FullCarryFlag ? 1 : 0);

            registers.NegativeFlag = false;
            registers.ZeroFlag = false;
            if ((((originalValue & 0x0F) + (value & 0x0F)) & 0x10) == 0x10)
                registers.HalfCarryFlag = true;
            if (result > byte.MaxValue)
                registers.FullCarryFlag = true;

            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    registers.A = (byte)result;
                    break;
                case IALU.TargetRegister.F:
                    registers.F = (byte)result;
                    break;
                case IALU.TargetRegister.B:
                    registers.B = (byte)result;
                    break;
                case IALU.TargetRegister.C:
                    registers.C = (byte)result;
                    break;
                case IALU.TargetRegister.D:
                    registers.D = (byte)result;
                    break;
                case IALU.TargetRegister.E:
                    registers.E = (byte)result;
                    break;
                case IALU.TargetRegister.H:
                    registers.H = (byte)result;
                    break;
                case IALU.TargetRegister.L:
                    registers.L = (byte)result;
                    break;
            }
        }

        public void AddUshort(IRegisters registers, IALU.TargetRegister targetRegister, ushort value, bool useCarry)
        {
            int result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.AF:
                    result = registers.AF;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.BC:
                    result = registers.BC;
                    break;
                case IALU.TargetRegister.DE:
                    result = registers.DE;
                    break;
                case IALU.TargetRegister.HL:
                    result = registers.HL;
                    break;
                default:
                    throw new ArgumentException();
            }
            int originalValue = result;
            result += value + (useCarry && registers.FullCarryFlag ? 1 : 0);

            //Zero flag is ignored
            registers.NegativeFlag = false;
            if ((((originalValue & 0x0FFF) + (value & 0x0FFF)) & 0x1000) == 0x1000)
                registers.HalfCarryFlag = true;
            if (result > ushort.MaxValue)
                registers.FullCarryFlag = true;

            switch (targetRegister)
            {
                case IALU.TargetRegister.AF:
                    registers.AF = (ushort)result;
                    break;
                case IALU.TargetRegister.BC:
                    registers.BC = (ushort)result;
                    break;
                case IALU.TargetRegister.DE:
                    registers.DE = (ushort)result;
                    break;
                case IALU.TargetRegister.HL:
                    registers.HL = (ushort)result;
                    break;
            }
        }

        public void SubtractByte(IRegisters registers, IALU.TargetRegister targetRegister, byte value, bool useCarry)
        {
            int result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                default:
                    throw new ArgumentException();
            }
            int originalValue = result;
            result -= value + (useCarry && registers.FullCarryFlag ? 1 : 0);

            registers.NegativeFlag = true;
            if ((result & 0xFF) == 0)
                registers.ZeroFlag = true;
            if (!(((originalValue & 0x0F) - (value & 0x0F)) < 0))
                registers.HalfCarryFlag = true;
            if (result < 0)
                registers.FullCarryFlag = true;

            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    registers.A = (byte)result;
                    break;
                case IALU.TargetRegister.F:
                    registers.F = (byte)result;
                    break;
                case IALU.TargetRegister.B:
                    registers.B = (byte)result;
                    break;
                case IALU.TargetRegister.C:
                    registers.C = (byte)result;
                    break;
                case IALU.TargetRegister.D:
                    registers.D = (byte)result;
                    break;
                case IALU.TargetRegister.E:
                    registers.E = (byte)result;
                    break;
                case IALU.TargetRegister.H:
                    registers.H = (byte)result;
                    break;
                case IALU.TargetRegister.L:
                    registers.L = (byte)result;
                    break;
            }
        }

        public void SubtractSbyte(IRegisters registers, IALU.TargetRegister targetRegister, sbyte value, bool useCarry)
        {
            int result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                default:
                    throw new ArgumentException();
            }
            int originalValue = result;
            result -= value + (useCarry && registers.FullCarryFlag ? 1 : 0);

            registers.NegativeFlag = true;
            registers.ZeroFlag = false;
            if (!(((originalValue & 0x0F) - (value & 0x0F)) < 0))
                registers.HalfCarryFlag = true;
            if (result < 0)
                registers.FullCarryFlag = true;

            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    registers.A = (byte)result;
                    break;
                case IALU.TargetRegister.F:
                    registers.F = (byte)result;
                    break;
                case IALU.TargetRegister.B:
                    registers.B = (byte)result;
                    break;
                case IALU.TargetRegister.C:
                    registers.C = (byte)result;
                    break;
                case IALU.TargetRegister.D:
                    registers.D = (byte)result;
                    break;
                case IALU.TargetRegister.E:
                    registers.E = (byte)result;
                    break;
                case IALU.TargetRegister.H:
                    registers.H = (byte)result;
                    break;
                case IALU.TargetRegister.L:
                    registers.L = (byte)result;
                    break;
            }
        }

        public void SubtractUshort(IRegisters registers, IALU.TargetRegister targetRegister, ushort value, bool useCarry)
        {
            int result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.AF:
                    result = registers.AF;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.BC:
                    result = registers.BC;
                    break;
                case IALU.TargetRegister.DE:
                    result = registers.DE;
                    break;
                case IALU.TargetRegister.HL:
                    result = registers.HL;
                    break;
                default:
                    throw new ArgumentException();
            }
            int originalValue = result;
            result -= value + (useCarry && registers.FullCarryFlag ? 1 : 0);

            //Zero flag is ignored
            registers.NegativeFlag = true;
            if (!(((originalValue & 0x0FFF) - (value & 0x0FFF)) < 0))
                registers.HalfCarryFlag = true;
            if (result < 0)
                registers.FullCarryFlag = true;

            switch (targetRegister)
            {
                case IALU.TargetRegister.AF:
                    registers.AF = (ushort)result;
                    break;
                case IALU.TargetRegister.BC:
                    registers.BC = (ushort)result;
                    break;
                case IALU.TargetRegister.DE:
                    registers.DE = (ushort)result;
                    break;
                case IALU.TargetRegister.HL:
                    registers.HL = (ushort)result;
                    break;
            }
        }

        public void IncrementRegister(IRegisters registers, IALU.TargetRegister targetRegister)
        {
            int result;
            bool is16Bit = false;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.AF:
                    is16Bit = true;
                    result = registers.AF;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.BC:
                    is16Bit = true;
                    result = registers.BC;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.DE:
                    is16Bit = true;
                    result = registers.DE;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                case IALU.TargetRegister.HL:
                    is16Bit = true;
                    result = registers.HL;
                    break;
                default:
                    throw new ArgumentException();
            }
            int originalValue = result;
            result += 1;

            if(!is16Bit)
            {
                registers.NegativeFlag = false;
                if ((result & 0xFF) == 0)
                    registers.ZeroFlag = true;
                if ((((originalValue & 0x0F) + (1 & 0x0F)) & 0x10) == 0x10)
                    registers.HalfCarryFlag = true;
                //ignore carry flag
            }

            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    registers.A = (byte)result;
                    break;
                case IALU.TargetRegister.F:
                    registers.F = (byte)result;
                    break;
                case IALU.TargetRegister.AF:
                    registers.AF = (ushort)result;
                    break;
                case IALU.TargetRegister.B:
                    registers.B = (byte)result;
                    break;
                case IALU.TargetRegister.C:
                    registers.C = (byte)result;
                    break;
                case IALU.TargetRegister.BC:
                    registers.BC = (ushort)result;
                    break;
                case IALU.TargetRegister.D:
                    registers.D = (byte)result;
                    break;
                case IALU.TargetRegister.E:
                    registers.E = (byte)result;
                    break;
                case IALU.TargetRegister.DE:
                    registers.AF = (ushort)result;
                    break;
                case IALU.TargetRegister.H:
                    registers.H = (byte)result;
                    break;
                case IALU.TargetRegister.L:
                    registers.L = (byte)result;
                    break;
                case IALU.TargetRegister.HL:
                    registers.AF = (ushort)result;
                    break;
            }
        }

        public void IncrementMemory(IRegisters registers, IMemory memory, ushort address)
        {
            int result = memory.ReadByte(address);
            int originalValue = result;
            result += 1;

            registers.NegativeFlag = false;
            if ((result & 0xFF) == 0)
                registers.ZeroFlag = true;
            if ((((originalValue & 0x0F) + (1 & 0x0F)) & 0x10) == 0x10)
                registers.HalfCarryFlag = true;
            //ignore carry flag

            memory.WriteByte(address, (byte)result);
        }

        public void DecrementRegister(IRegisters registers, IALU.TargetRegister targetRegister)
        {
            int result;
            bool is16Bit = false;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.AF:
                    is16Bit = true;
                    result = registers.AF;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.BC:
                    is16Bit = true;
                    result = registers.BC;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.DE:
                    is16Bit = true;
                    result = registers.DE;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                case IALU.TargetRegister.HL:
                    is16Bit = true;
                    result = registers.HL;
                    break;
                default:
                    throw new ArgumentException();
            }
            int originalValue = result;
            result -= 1;

            if(!is16Bit)
            {
                registers.NegativeFlag = true;
                if ((result & 0xFF) == 0)
                    registers.ZeroFlag = true;
                if (!(((originalValue & 0x0F) - (1 & 0x0F)) < 0))
                    registers.HalfCarryFlag = true;
                //ignore carry flag
            }

            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    registers.A = (byte)result;
                    break;
                case IALU.TargetRegister.F:
                    registers.F = (byte)result;
                    break;
                case IALU.TargetRegister.B:
                    registers.B = (byte)result;
                    break;
                case IALU.TargetRegister.C:
                    registers.C = (byte)result;
                    break;
                case IALU.TargetRegister.D:
                    registers.D = (byte)result;
                    break;
                case IALU.TargetRegister.E:
                    registers.E = (byte)result;
                    break;
                case IALU.TargetRegister.H:
                    registers.H = (byte)result;
                    break;
                case IALU.TargetRegister.L:
                    registers.L = (byte)result;
                    break;
            }
        }

        public void DecrementMemory(IRegisters registers, IMemory memory, ushort address)
        {
            int result = memory.ReadByte(address);
            int originalValue = result;
            result -= 1;

            registers.NegativeFlag = false;
            if ((result & 0xFF) == 0)
                registers.ZeroFlag = true;
            if (!(((originalValue & 0x0F) - (1 & 0x0F)) < 0))
                registers.HalfCarryFlag = true;
            //ignore carry flag

            memory.WriteByte(address, (byte)result);
        }

        public void AndBitwiseByte(IRegisters registers, IALU.TargetRegister targetRegister, byte value)
        {
            byte result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                default:
                    throw new ArgumentException();
            }
            result &= value;

            if (result == 0)
                registers.ZeroFlag = true;
            registers.NegativeFlag = false;
            registers.HalfCarryFlag = true;
            registers.FullCarryFlag = false;

            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    registers.A = result;
                    break;
                case IALU.TargetRegister.F:
                    registers.F = result;
                    break;
                case IALU.TargetRegister.B:
                    registers.B = result;
                    break;
                case IALU.TargetRegister.C:
                    registers.C = result;
                    break;
                case IALU.TargetRegister.D:
                    registers.D = result;
                    break;
                case IALU.TargetRegister.E:
                    registers.E = result;
                    break;
                case IALU.TargetRegister.H:
                    registers.H = result;
                    break;
                case IALU.TargetRegister.L:
                    registers.L = result;
                    break;
            }
        }

        public void OrBitwiseByte(IRegisters registers, IALU.TargetRegister targetRegister, byte value)
        {
            byte result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                default:
                    throw new ArgumentException();
            }
            result |= value;

            if (result == 0)
                registers.ZeroFlag = true;
            registers.NegativeFlag = false;
            registers.HalfCarryFlag = false;
            registers.FullCarryFlag = false;

            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    registers.A = result;
                    break;
                case IALU.TargetRegister.F:
                    registers.F = result;
                    break;
                case IALU.TargetRegister.B:
                    registers.B = result;
                    break;
                case IALU.TargetRegister.C:
                    registers.C = result;
                    break;
                case IALU.TargetRegister.D:
                    registers.D = result;
                    break;
                case IALU.TargetRegister.E:
                    registers.E = result;
                    break;
                case IALU.TargetRegister.H:
                    registers.H = result;
                    break;
                case IALU.TargetRegister.L:
                    registers.L = result;
                    break;
            }
        }

        public void XorBitwiseByte(IRegisters registers, IALU.TargetRegister targetRegister, byte value)
        {
            byte result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                default:
                    throw new ArgumentException();
            }
            result ^= value;

            if (result == 0)
                registers.ZeroFlag = true;
            registers.NegativeFlag = false;
            registers.HalfCarryFlag = false;
            registers.FullCarryFlag = false;

            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    registers.A = result;
                    break;
                case IALU.TargetRegister.F:
                    registers.F = result;
                    break;
                case IALU.TargetRegister.B:
                    registers.B = result;
                    break;
                case IALU.TargetRegister.C:
                    registers.C = result;
                    break;
                case IALU.TargetRegister.D:
                    registers.D = result;
                    break;
                case IALU.TargetRegister.E:
                    registers.E = result;
                    break;
                case IALU.TargetRegister.H:
                    registers.H = result;
                    break;
                case IALU.TargetRegister.L:
                    registers.L = result;
                    break;
            }
        }

        public void TestBitByte(IRegisters registers, int bit, byte value)
        {
            if ((value & (1 << bit)) > 0)
            {
                registers.ZeroFlag = true;
            }
            registers.NegativeFlag = false;
            registers.HalfCarryFlag = true;
        }

        public void CompareByte(IRegisters registers, IALU.TargetRegister targetRegister, byte value)
        {
            int result;
            switch (targetRegister)
            {
                case IALU.TargetRegister.A:
                    result = registers.A;
                    break;
                case IALU.TargetRegister.F:
                    result = registers.F;
                    break;
                case IALU.TargetRegister.B:
                    result = registers.B;
                    break;
                case IALU.TargetRegister.C:
                    result = registers.C;
                    break;
                case IALU.TargetRegister.D:
                    result = registers.D;
                    break;
                case IALU.TargetRegister.E:
                    result = registers.E;
                    break;
                case IALU.TargetRegister.H:
                    result = registers.H;
                    break;
                case IALU.TargetRegister.L:
                    result = registers.L;
                    break;
                default:
                    throw new ArgumentException();
            }
            int originalValue = result;
            result -= value;

            registers.NegativeFlag = true;
            if ((result & 0xFF) == 0)
                registers.ZeroFlag = true;
            if (!(((originalValue & 0x0F) - (value & 0x0F)) < 0))
                registers.HalfCarryFlag = true;
            if (result < 0)
                registers.FullCarryFlag = true;

            //don't register
        }
    }
}
