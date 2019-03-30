using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public class Imm16Operand : ImmOperand, IEquatable<Imm16Operand>
    {
        public short Value { get; }

        public Imm16Operand(short value)
        {
            Value = value;
        }

        public override OperandSize Size => OperandSize.Size16;

        public override bool Equals(object obj)
        {
            return Equals(obj as Imm16Operand);
        }

        public bool Equals(Imm16Operand other)
        {
            return other != null &&
                   base.Equals(other) &&
                   Value == other.Value &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Value, Type);
        }

        public static bool operator ==(Imm16Operand operand1, Imm16Operand operand2)
        {
            return EqualityComparer<Imm16Operand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(Imm16Operand operand1, Imm16Operand operand2)
        {
            return !(operand1 == operand2);
        }

        public override string ToString()
        {
            return $"imm{(int)Size}({Value})";
        }

        public override ImmOperand Widen() => new Imm32Operand(Value);
    }
}