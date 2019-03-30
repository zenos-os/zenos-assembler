using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;

namespace Zenos.Assembly.AST
{
    public static class InstructionEmitter
    {
        public static string Emit(Instruction instruction, List<byte> bytes)
        {
            var operands = instruction.Operands;
            var def = InstructionDefinitions.Lookup(instruction);
            if (def == null)
                return $"instruction `{instruction}` not supported";

            /*
             * Instruction format is in Section 2.1
             *
             * 1. Instruction Prefix (0 or 1 byte)
             * 2. Op code (1, 2, or 3)
             * 3. ModR/M (0 or 1 byte)
             * 4. SIB (0 or 1 byte)
             * 5. Displacement (0, 1, 2, or 4 bytes)
             * 6. Immediate (0, 1, 2, or 4 bytes)
             *
             */

            var op1 = operands.FirstOrDefault();
            var op1Def = def.Operands.FirstOrDefault();
            var op2 = operands.Skip(1).FirstOrDefault();
            var op2Def = def.Operands.Skip(1).FirstOrDefault();

            if (op2?.Type == OperandType.Memory)
            {
                // swap operands as we want op1 to be the memory operand
                (op1, op2) = (op2, op1);
                (op1Def, op2Def) = (op2Def, op1Def);
            }

            // 1. REX prefix
            var rexPrefix = CalculateRexPrefix(def, op1, op2);
            if (rexPrefix != null)
            {
                bytes.Add(rexPrefix.Value);
            }

            // 2. Op code (1, 2, or 3)

            bytes.AddRange(def.OpCode);

            // opcode register adjustment
            if (def.OpcodeReg && op1 is RegisterOperand reg1)
            {
                bytes[bytes.Count - 1] = (byte)(bytes[bytes.Count - 1] + (byte)(RegisterByte(reg1.Name) & 0b111));
            }

            // 3. ModR/M (0 or 1 byte)
            var modRm = CalculateModRM(op1, op1Def, op2, op2Def);
            if (!def.OpcodeReg && modRm != null)
            {
                bytes.Add(modRm.Value);
            }

            // 4. SIB (0 or 1 byte)
            var sib = CalculateSibByte(op1);
            if (sib != null)
            {
                bytes.Add(sib.Value);
            }

            // 5. Displacement (0, 1, 2, or 4 bytes)
            if (!def.OpcodeReg && modRm != null)
            {
                var mod = modRm.Value >> 6;
                switch (mod)
                {
                    case 0b01: // disp8
                        // we don't support displacements yet so zeros for now
                        bytes.Add(0);
                        break;

                    case 0b10: // disp32
                        // we don't support displacements yet so zeros for now
                        bytes.Add(0);
                        bytes.Add(0);
                        bytes.Add(0);
                        bytes.Add(0);
                        break;

                    /* if rbp/r13 then disp32 */
                    case 0b00 when sib != null && (sib & 0b111) == 0b101:
                        var value = GetDisplacement32(op1);

                        bytes.Add((byte)value);
                        bytes.Add((byte)(value >> 8));
                        bytes.Add((byte)(value >> 16));
                        bytes.Add((byte)(value >> 24));
                        break;
                }
            }

            // 6. immediate
            var imm = GetImmOperand(operands);
            switch (imm)
            {
                case Imm8Operand imm8:
                    {
                        var value = imm8.Value;
                        bytes.Add((byte)value);
                        break;
                    }
                case Imm16Operand imm16:
                    {
                        var value = imm16.Value;
                        bytes.Add((byte)value);
                        bytes.Add((byte)(value >> 8));
                        bytes.Add((byte)(value >> 16));
                        bytes.Add((byte)(value >> 24));
                        break;
                    }
                case Imm32Operand imm32:
                    {
                        var value = imm32.Value;
                        bytes.Add((byte)value);
                        bytes.Add((byte)(value >> 8));
                        bytes.Add((byte)(value >> 16));
                        bytes.Add((byte)(value >> 24));
                        break;
                    }
            }

            return null;
        }

        private static int GetDisplacement32(Operand op)
        {
            if (op is MemoryOperand mem && mem.InnerOperand is Imm32Operand imm32)
            {
                return imm32.Value;
            }

            throw new InvalidOperationException("No disp32 value");
        }

        private static byte? CalculateSibByte(Operand op1)
        {
            if (op1 is MemoryOperand memOp)
            {
                switch (memOp.InnerOperand)
                {
                    case RegisterOperand reg:
                        {
                            var registerBits = RegisterByte(reg.Name) & 0b111;
                            if (registerBits == 4) // rsp/r12
                            {
                                var scale = 0;
                                var index = registerBits;
                                var baseReg = registerBits;

                                return (byte)(scale << 6 | index << 3 | baseReg);
                            }

                            break;
                        }
                    case ImmOperand _:
                        {
                            var scale = 0;
                            var index = 0b100;
                            var baseReg = 0b101;

                            return (byte)(scale << 6 | index << 3 | baseReg);
                        }
                }
            }

            return null;
        }

        private static byte? CalculateRexPrefix(InstructionDefinition def, Operand op1, Operand op2)
        {
            var rexB = GetRexBit(op1);
            var rexR = GetRexBit(op2);

            if (def.W || rexB || rexR)
            {
                var rex = 0b01000000;

                if (rexB)
                {
                    rex |= 0b00000001;
                }

                // SIB index
                //if (rexX)
                //{
                //    rex |= 0b00000010;
                //}

                if (rexR)
                {
                    rex |= 0b00000100;
                }

                if (def.W)
                {
                    rex |= 0b00001000;
                }

                return ((byte)rex);
            }

            return null;
        }

        private static bool GetRexBit(Operand op)
        {
            return op is RegisterOperand rmReg && RegisterByte(rmReg.Name) > 7 || op is MemoryOperand mem1 && mem1.InnerOperand is RegisterOperand reg2 && RegisterByte(reg2.Name) > 7;
        }

        private static byte? CalculateModRMRegBits(Operand op, OperandDefinition opDef)
        {
            if (op is RegisterOperand rmReg && opDef.Type != OperandType.Fixed)
                return (byte)(RegisterByte(rmReg.Name) & 0b111);

            if (op is MemoryOperand mem1)
            {
                if (mem1.InnerOperand is RegisterOperand reg)
                    return (byte)(RegisterByte(reg.Name) & 0b111);

                if (mem1.InnerOperand is ImmOperand)
                    return 0b100; // SP
            }

            return null;
        }

        ///  <summary>
        ///
        ///  </summary>
        /// <param name="operand1">Memory operand</param>
        ///  <param name="op1Def"></param>
        ///  <param name="operand2">reg operand</param>
        ///  <param name="op2Def"></param>
        ///  <returns></returns>
        private static byte? CalculateModRM(Operand operand1, OperandDefinition op1Def, Operand operand2, OperandDefinition op2Def)
        {
            var reg1Bits = CalculateModRMRegBits(operand1, op1Def);

            if (reg1Bits != null)
            {
                var reg1Byte = reg1Bits.Value;
                var reg2Bits = CalculateModRMRegBits(operand2, op2Def);

                var mod = CalculateModRmModBits(operand1, reg1Bits) << 6;

                if (reg2Bits != null)
                {
                    var reg2Byte = reg2Bits.Value;
                    return (byte)(mod | (reg2Byte << 3) | reg1Byte);
                }

                return (byte)(mod | reg1Byte);
            }

            return null;
        }

        private static byte CalculateModRmModBits(Operand operand1, byte? op1RegBits)
        {
            // good reference https://www-user.tu-chemnitz.de/~heha/viewchm.php/hs/x86.chm/x64.htm#Size_prefix
            if (operand1 is MemoryOperand mem)
            {
                if (op1RegBits != null && op1RegBits == 5) // sib extension [rbp/r13]
                {
                    return 0b01; // [SIB + disp8]
                }

                // [SIB + disp32] return 0b10;

                if (mem.InnerOperand is ImmOperand)
                {
                    return 0b00;  // [SIB] only which gets converted to [disp32]
                }

                return 0b00;
            }

            return 0b11;
        }

        private static ImmOperand GetImmOperand(IEnumerable<Operand> operands) => operands.OfType<ImmOperand>().FirstOrDefault();

        private static RegisterOperand GetRegisterOperand(Operand[] operands) => operands.OfType<RegisterOperand>().FirstOrDefault();

        private static MemoryOperand GetMemoryOperand(Operand[] operands) => operands.OfType<MemoryOperand>().FirstOrDefault();

        private static byte RegisterByte(string register)
        {
            switch (register)
            {
                case "al":
                case "ax":
                case "eax":
                case "rax":
                    return 0;

                case "cl":
                case "cx":
                case "ecx":
                case "rcx":
                    return 1;

                case "dl":
                case "dx":
                case "edx":
                case "rdx":
                    return 2;

                case "bl":
                case "bx":
                case "ebx":
                case "rbx":
                    return 3;

                case "ah":
                case "sp":
                case "esp":
                case "rsp":
                    return 4;

                case "ch":
                case "bp":
                case "ebp":
                case "rbp":
                    return 5;

                case "dh":
                case "si":
                case "esi":
                case "rsi":
                    return 6;

                case "bh":
                case "di":
                case "edi":
                case "rdi":
                    return 7;

                case "r8l":
                case "r8w":
                case "r8d":
                case "r8":
                    return 8;

                case "r9l":
                case "r9w":
                case "r9d":
                case "r9":
                    return 9;

                case "r10l":
                case "r10w":
                case "r10d":
                case "r10":
                    return 10;

                case "r11l":
                case "r11w":
                case "r11d":
                case "r11":
                    return 11;

                case "r12l":
                case "r12w":
                case "r12d":
                case "r12":
                    return 12;

                case "r13l":
                case "r13w":
                case "r13d":
                case "r13":
                    return 13;

                case "r14l":
                case "r14w":
                case "r14d":
                case "r14":
                    return 14;

                case "r15l":
                case "r15w":
                case "r15d":
                case "r15":
                    return 15;

                default:
                    throw new InvalidOperationException($"Invalid register name: {register}");
            }
        }
    }

    internal class InstructionEncoding
    {
        public InstructionEncoding(IReadOnlyList<byte> opCode, bool hasModRm, bool hasScaleIndexBase)
        {
            OpCode = opCode;
            HasModRM = hasModRm;
            HasScaleIndexBase = hasScaleIndexBase;
        }

        public IReadOnlyList<byte> OpCode { get; }
        public bool HasModRM { get; }
        public bool HasScaleIndexBase { get; }
    }
}