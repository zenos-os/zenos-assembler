using System.Collections.Generic;
using System.Linq;

namespace Zenos.Assembly.AST
{
    internal static class InstructionDefinitions
    {
        private static readonly Dictionary<string, List<InstructionDefinition>> _instructionDefinitions = new Dictionary<string, List<InstructionDefinition>>
        {
            ["add"] = new List<InstructionDefinition>
            {
                new InstructionDefinition(new List<byte> { 0x66, 0x83 }, new OperandDefinition[] {OperandTypeDefinition.Reg16, OperandTypeDefinition.Imm8 }),
                new InstructionDefinition(new List<byte> { 0x83 }, new OperandDefinition[] {OperandTypeDefinition.Reg32, OperandTypeDefinition.Imm8 }),
                new InstructionDefinition(new List<byte> { 0x83 }, new OperandDefinition[] {OperandTypeDefinition.Reg64, OperandTypeDefinition.Imm8 }, w: true), // REX.W + 81 /0 id ADD r/m64, imm32 MI
                new InstructionDefinition(new List<byte> { 0x04 }, new OperandDefinition[] {FixedOperandDefinition.AL, OperandTypeDefinition.Imm8 }),
                new InstructionDefinition(new List<byte> { 0x66, 0x05 }, new OperandDefinition[] {FixedOperandDefinition.AX, OperandTypeDefinition.Imm16 }),
                new InstructionDefinition(new List<byte> { 0x05 }, new OperandDefinition[] {FixedOperandDefinition.EAX, OperandTypeDefinition.Imm32 }),
                new InstructionDefinition(new List<byte> { 0x05 }, new OperandDefinition[] {FixedOperandDefinition.RAX, OperandTypeDefinition.Imm32 }, w: true),
                new InstructionDefinition(new List<byte> { 0x01 }, new OperandDefinition[] {OperandTypeDefinition.Memory64, OperandTypeDefinition.Reg64 }, w: true),
                new InstructionDefinition(new List<byte> { 0x01 }, new OperandDefinition[] {OperandTypeDefinition.Memory32, OperandTypeDefinition.Reg32 }),
                new InstructionDefinition(new List<byte> { 0x01 }, new OperandDefinition[] {OperandTypeDefinition.Memory16, OperandTypeDefinition.Reg16 }),
                new InstructionDefinition(new List<byte> { 0x03 }, new OperandDefinition[] {OperandTypeDefinition.Reg64, OperandTypeDefinition.Memory64 }, w: true),
                new InstructionDefinition(new List<byte> { 0x03 }, new OperandDefinition[] {OperandTypeDefinition.Reg32, OperandTypeDefinition.Memory32 }),
                new InstructionDefinition(new List<byte> { 0x03 }, new OperandDefinition[] {OperandTypeDefinition.Reg16, OperandTypeDefinition.Memory16 }),
            },
            ["mov"] = new List<InstructionDefinition>
            {
//                new InstructionDefinition(new List<byte> { 0xB8 }, new OperandDefinition[] {OperandTypeDefinition.Reg32, OperandTypeDefinition.Imm32 }, opcodeReg: true), // B8+rd id MOV r32, imm32 OI
                new InstructionDefinition(new List<byte> { 0xB8 }, new OperandDefinition[] {OperandTypeDefinition.Reg64, OperandTypeDefinition.Imm32 }, opcodeReg: true), // B8+rd id MOV r32, imm32 OI
                new InstructionDefinition(new List<byte> { 0x89 }, new OperandDefinition[] {OperandTypeDefinition.Memory64, OperandTypeDefinition.Reg64 }, w: true),      // REX.W + 89 /r MOV r/m64,r64 MR
                new InstructionDefinition(new List<byte> { 0x8B }, new OperandDefinition[] {OperandTypeDefinition.Reg64, OperandTypeDefinition.Memory64 }, w: true),      // REX.W + 8B /r MOV r64,r/m64 RM
                new InstructionDefinition(new List<byte> { 0xC7 }, new OperandDefinition[] {OperandTypeDefinition.Reg64, OperandTypeDefinition.Imm32 }, w: true),         // REX.W + C7 /0 io MOV r/m64, imm32 MI
                new InstructionDefinition(new List<byte> { 0xC7 }, new OperandDefinition[] {OperandTypeDefinition.Reg32, OperandTypeDefinition.Imm32 }),                  // C7 /0 io MOV r/m32, imm32 MI
                new InstructionDefinition(new List<byte> { 0xC7 }, new OperandDefinition[] {OperandTypeDefinition.Reg16, OperandTypeDefinition.Imm16 }),                  // C7 /0 io MOV r/m32, imm32 MI
                new InstructionDefinition(new List<byte> { 0xC7 }, new OperandDefinition[] {OperandTypeDefinition.Reg64, OperandTypeDefinition.Imm32 }, w: true),         // REX.W + C7 /0 io MOV r/m64, imm32 MI
            },
            ["push"] = new List<InstructionDefinition>
            {
                new InstructionDefinition(new List<byte> { 0x50 }, new OperandDefinition[] { OperandTypeDefinition.Reg16 }, opcodeReg: true), // 50+rw PUSH r16 O
                new InstructionDefinition(new List<byte> { 0x50 }, new OperandDefinition[] { OperandTypeDefinition.Reg32 }, opcodeReg: true), // 50+rd PUSH r32 O
                new InstructionDefinition(new List<byte> { 0x50 }, new OperandDefinition[] { OperandTypeDefinition.Reg64 }, opcodeReg: true), // 50+rd PUSH r64 O
                new InstructionDefinition(new List<byte> { 0x6A }, new OperandDefinition[] { OperandTypeDefinition.Imm8 }),                   // 6A ib PUSH imm8 I
                new InstructionDefinition(new List<byte> { 0x68 }, new OperandDefinition[] { OperandTypeDefinition.Imm16 }),                  // 68 iw PUSH imm16 I
                new InstructionDefinition(new List<byte> { 0x68 }, new OperandDefinition[] { OperandTypeDefinition.Imm32 }),                  // 68 id PUSH imm32 I
            },
            ["pop"] = new List<InstructionDefinition>
            {
                new InstructionDefinition(new List<byte> { 0x58 }, new OperandDefinition[] { OperandTypeDefinition.Reg16 }, opcodeReg: true), // O
                new InstructionDefinition(new List<byte> { 0x58 }, new OperandDefinition[] { OperandTypeDefinition.Reg32 }, opcodeReg: true), // O
                new InstructionDefinition(new List<byte> { 0x58 }, new OperandDefinition[] { OperandTypeDefinition.Reg64 }, opcodeReg: true), // O
            }
        };

        public static InstructionDefinition Lookup(Instruction instruction)
        {
            if (_instructionDefinitions.TryGetValue(instruction.Name, out var defs))
            {
                while (true)
                {
                    foreach (var definition in defs)
                    {
                        if (Matches(definition, instruction))
                        {
                            return definition;
                        }
                    }

                    // try widening imm
                    if (!WidenInstructionOperandImm(instruction))
                        break;
                }
            }

            return null;
        }


        private static bool WidenInstructionOperandImm(Instruction instruction)
        {
            for (int i = 0; i < instruction.Operands.Length; i++)
            {
                if (instruction.Operands[i] is ImmOperand imm && imm.CanWiden)
                {
                    instruction.Operands[i] = imm.Widen();
                    return true;
                }
            }

            return false;
        }

        private static bool Matches(InstructionDefinition definition, Instruction instruction)
        {
            if (definition.Operands.Length != instruction.Operands.Length)
                return false;

            return instruction.Operands
                .Zip(definition.Operands, (op, def) => (op, def))
                .All(((Operand op, OperandDefinition opDef) tuple) => OperandMatches(tuple.opDef, tuple.op));
        }

        private static bool OperandMatches(OperandDefinition opDef, Operand op)
        {
            if (op.Size != opDef.Size)
                return false;

            switch (opDef)
            {
                case FixedOperandDefinition fixDef:
                    {
                        return op is RegisterOperand reg && reg.Name == fixDef.Name;
                    }

                default:
                    {
                        // if we have a register operand but the definition requires a memory operand that is a match still
                        if (op.Type == opDef.Type)
                            return true;

                        return op.Type == OperandType.Register && opDef.Type == OperandType.Memory;
                    }
            }
        }
    }
}