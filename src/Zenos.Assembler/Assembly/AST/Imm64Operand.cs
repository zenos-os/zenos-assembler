using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public class Imm64Operand : ImmOperand, IEquatable<Imm64Operand>
    {
        public long Value { get; }
        public override OperandSize Size => OperandSize.Size64;

        public Imm64Operand(long value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Imm64Operand);
        }

        public bool Equals(Imm64Operand other)
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

        public static bool operator ==(Imm64Operand operand1, Imm64Operand operand2)
        {
            return EqualityComparer<Imm64Operand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(Imm64Operand operand1, Imm64Operand operand2)
        {
            return !(operand1 == operand2);
        }

        public override string ToString()
        {
            return $"imm{(int)Size}({Value})";
        }

        public override ImmOperand Widen() => this;
    }
}