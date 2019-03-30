using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public class Register8Operand : RegisterOperand, IEquatable<Register8Operand>
    {
        public Register8Operand(string name)
        {
            Name = name;
        }

        public override string Name { get; }
        public override OperandType Type => OperandType.Register;
        public override OperandSize Size => OperandSize.Size8;

        public override bool Equals(object obj)
        {
            return Equals(obj as Register8Operand);
        }

        public bool Equals(Register8Operand other)
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

        public static bool operator ==(Register8Operand operand1, Register8Operand operand2)
        {
            return EqualityComparer<Register8Operand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(Register8Operand operand1, Register8Operand operand2)
        {
            return !(operand1 == operand2);
        }
    }
}