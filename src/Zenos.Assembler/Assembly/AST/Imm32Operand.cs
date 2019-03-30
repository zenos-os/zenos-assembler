using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public class Imm32Operand : ImmOperand, IEquatable<Imm32Operand>
    {
        public int Value { get; }
        public override OperandSize Size => OperandSize.Size32;

        public Imm32Operand(int value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Imm32Operand);
        }

        public bool Equals(Imm32Operand other)
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

        public static bool operator ==(Imm32Operand operand1, Imm32Operand operand2)
        {
            return EqualityComparer<Imm32Operand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(Imm32Operand operand1, Imm32Operand operand2)
        {
            return !(operand1 == operand2);
        }

        public override string ToString()
        {
            return $"imm{(int)Size}({Value})";
        }

        public override ImmOperand Widen() => new Imm64Operand(Value);
    }
}