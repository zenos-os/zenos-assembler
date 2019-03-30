using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public class Register16Operand : RegisterOperand, IEquatable<Register16Operand>
    {
        public Register16Operand(string name)
        {
            Name = name;
        }

        public override string Name { get; }
        public override OperandType Type => OperandType.Register;
        public override OperandSize Size => OperandSize.Size16;

        public override bool Equals(object obj)
        {
            return Equals(obj as Register16Operand);
        }

        public bool Equals(Register16Operand other)
        {
            return other != null &&
                   Name == other.Name &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }

        public static bool operator ==(Register16Operand operand1, Register16Operand operand2)
        {
            return EqualityComparer<Register16Operand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(Register16Operand operand1, Register16Operand operand2)
        {
            return !(operand1 == operand2);
        }
    }
}