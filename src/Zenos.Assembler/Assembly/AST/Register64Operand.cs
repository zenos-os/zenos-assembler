using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public class Register64Operand : RegisterOperand, IEquatable<Register64Operand>
    {
        public Register64Operand(string name)
        {
            Name = name;
        }

        public override string Name { get; }
        public override OperandType Type => OperandType.Register;
        public override OperandSize Size => OperandSize.Size64;

        public override bool Equals(object obj)
        {
            return Equals(obj as Register64Operand);
        }

        public bool Equals(Register64Operand other)
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

        public static bool operator ==(Register64Operand operand1, Register64Operand operand2)
        {
            return EqualityComparer<Register64Operand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(Register64Operand operand1, Register64Operand operand2)
        {
            return !(operand1 == operand2);
        }
    }
}