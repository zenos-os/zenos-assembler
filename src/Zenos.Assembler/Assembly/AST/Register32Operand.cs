using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public class Register32Operand : RegisterOperand, IEquatable<Register32Operand>
    {
        public Register32Operand(string name)
        {
            Name = name;
        }

        public override string Name { get; }
        public override OperandType Type => OperandType.Register;
        public override OperandSize Size => OperandSize.Size32;

        public override bool Equals(object obj)
        {
            return Equals(obj as Register32Operand);
        }

        public bool Equals(Register32Operand other)
        {
            return other != null &&
                   base.Equals(other) &&
                   Name == other.Name &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Name, Type);
        }

        public static bool operator ==(Register32Operand operand1, Register32Operand operand2)
        {
            return EqualityComparer<Register32Operand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(Register32Operand operand1, Register32Operand operand2)
        {
            return !(operand1 == operand2);
        }
    }
}