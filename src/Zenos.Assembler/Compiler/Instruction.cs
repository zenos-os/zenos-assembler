using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Collections.Generic;
using CecilInstruction = Mono.Cecil.Cil.Instruction;

namespace Zenos.Compiler
{
    //internal class Instructions
    //{
    //    public List<byte> Bytes { get; }

    //    protected Instructions()
    //        : this(new List<byte>())
    //    {
    //    }

    //    public Instructions(List<byte> bytes)
    //    {
    //        Bytes = bytes;
    //    }

    //    public static Instructions FromInstructions(IEnumerable<CecilInstruction> instructions)
    //    {
    //        return new Instructions(instructions.SelectMany(x => x.Bytes).ToList());
    //    }

    //    public string ToDisassembly()
    //    {
    //        var sb = new StringBuilder();
    //        var offset = 0;
    //        var bytes = Bytes.ToArray();
    //        while (offset < bytes.Length)
    //        {
    //            var insOffset = offset;
    //            if (!InstructionDefinition.Lookup((OpCode)(int)bytes[offset], out var def))
    //            {
    //                sb.AppendLine($"ERROR: failed to find instruction {bytes[offset]}");
    //                offset++;
    //                continue;
    //            }
    //            // for opcode
    //            offset++;

    //            var (operands, read) = def.ReadOperands(bytes, offset);
    //            offset += read;

    //            sb.AppendLine($"{insOffset:X4} {FormatInstruction(def, operands)}");
    //        }

    //        return sb.ToString();
    //    }

    //    private static string FormatInstruction(InstructionDefinition def, int[] operands)
    //    {
    //        var operandCount = def.OperandWidths.Count;
    //        if (operands.Length != operandCount)
    //            return $"ERROR: operand len {operands.Length} does not match defined {operandCount}";

    //        switch (operandCount)
    //        {
    //            case 0:
    //                return def.Name;

    //            case 1:
    //                return $"{def.Name} {operands[0]}";

    //            default:
    //                return $"ERROR: unhandled operandCount for {def.Name}";
    //        }
    //    }

    //    public override string ToString() => ToDisassembly();
    //}

    //internal class Instruction
    //{
    //    private Instruction()
    //        : this(new List<byte>())
    //    {
    //    }

    //    private Instruction(List<byte> bytes)
    //    {
    //        Bytes = bytes;
    //    }

    //    public IReadOnlyList<byte> Bytes { get; }

    //    public OpCode OpCode => (OpCode)Bytes[0];

    //    public static Instruction Make(OpCode opCode, params int[] operands)
    //    {
    //        if (!InstructionDefinition.Lookup(opCode, out var definition))
    //            return new Instruction();

    //        var bytes = new List<byte> { (byte)opCode };
    //        for (int i = 0; i < operands.Length; i++)
    //        {
    //            var width = definition.OperandWidths[i];
    //            switch (width)
    //            {
    //                case 2:
    //                    bytes.AddRange(BitConverter.GetBytes((ushort)operands[i]).Reverse());
    //                    break;

    //                default:
    //                    throw new NotSupportedException("Operand width: " + width);
    //            }
    //        }

    //        return new Instruction(bytes);
    //    }
    //}

    //internal enum OpCode
    //{
    //}

    //internal class InstructionDefinition
    //{
    //    public InstructionDefinition(string name, List<int> operandWidths = null)
    //    {
    //        Name = name;
    //        OperandWidths = operandWidths ?? new List<int>();
    //    }

    //    public string Name { get; }
    //    public List<int> OperandWidths { get; }

    //    //public (int[], int) ReadOperands(byte[] bytes, int offset)
    //    //{
    //    //    var operands = new int[OperandWidths.Count];
    //    //    var bytesRead = 0;
    //    //    for (var i = 0; i < OperandWidths.Count; i++)
    //    //    {
    //    //        var width = OperandWidths[i];
    //    //        switch (width)
    //    //        {
    //    //            case 2:
    //    //                operands[i] = Code.ReadUint16(bytes, offset);
    //    //                break;

    //    //            default:
    //    //                throw new NotSupportedException();
    //    //        }

    //    //        bytesRead += width;
    //    //    }

    //    //    return (operands, bytesRead);
    //    //}

    //    private static readonly Dictionary<OpCode, InstructionDefinition> definitions = new Dictionary<OpCode, InstructionDefinition>
    //    {
    //        //[OpCode.Constant] = new InstructionDefinition("Constant", new List<int> { 2 }),
    //        //[OpCode.Null] = new InstructionDefinition("Null"),
    //        //[OpCode.Add] = new InstructionDefinition("Add"),
    //        //[OpCode.Pop] = new InstructionDefinition("Pop"),
    //        //[OpCode.Sub] = new InstructionDefinition("Sub"),
    //        //[OpCode.Mul] = new InstructionDefinition("Mul"),
    //        //[OpCode.Div] = new InstructionDefinition("Div"),
    //        //[OpCode.True] = new InstructionDefinition("True"),
    //        //[OpCode.False] = new InstructionDefinition("False"),
    //        //[OpCode.Equ] = new InstructionDefinition("Equ"),
    //        //[OpCode.NotEqu] = new InstructionDefinition("NotEqu"),
    //        //[OpCode.GreaterThan] = new InstructionDefinition("GreaterThan"),
    //        //[OpCode.Minus] = new InstructionDefinition("Minus"),
    //        //[OpCode.Bang] = new InstructionDefinition("Bang"),
    //        //[OpCode.JumpFalse] = new InstructionDefinition("JumpFalse", new List<int> { 2 }),
    //        //[OpCode.Jump] = new InstructionDefinition("Jump", new List<int> { 2 }),
    //        //[OpCode.LoadGlobal] = new InstructionDefinition("LoadGlobal", new List<int> { 2 }),
    //        //[OpCode.StoreGlobal] = new InstructionDefinition("StoreGlobal", new List<int> { 2 }),
    //        //[OpCode.Array] = new InstructionDefinition("Array", new List<int> { 2 }),
    //        //[OpCode.Hash] = new InstructionDefinition("Hash", new List<int> { 2 }),
    //        //[OpCode.Index] = new InstructionDefinition("Hash"),
    //    };

    //    public static bool Lookup(OpCode code, out InstructionDefinition value) => definitions.TryGetValue(code, out value);
    //}

    //public static class Code
    //{
    //    public static int ReadUint16(byte[] bytes, int offset) => (bytes[offset] << 8) + bytes[offset + 1];
    //}
}