using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public class Imm8Operand : ImmOperand, IEquatable<Imm8Operand>
    {
        public sbyte Value { get; }
        public override OperandSize Size => OperandSize.Size8;

        public Imm8Operand(sbyte value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Imm8Operand);
        }

        public bool Equals(Imm8Operand other)
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

        public static bool operator ==(Imm8Operand operand1, Imm8Operand operand2)
        {
            return EqualityComparer<Imm8Operand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(Imm8Operand operand1, Imm8Operand operand2)
        {
            return !(operand1 == operand2);
        }

        public override string ToString()
        {
            return $"imm{(int)Size}({Value})";
        }

        public override ImmOperand Widen() => new Imm16Operand(this.Value);
    }
}