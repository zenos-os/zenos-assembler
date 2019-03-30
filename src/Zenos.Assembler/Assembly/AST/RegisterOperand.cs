using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public abstract class RegisterOperand : Operand, IEquatable<RegisterOperand>
    {
        public abstract string Name { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as RegisterOperand);
        }

        public bool Equals(RegisterOperand other)
        {
            return other != null &&
                   base.Equals(other) &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Name);
        }

        public static bool operator ==(RegisterOperand operand1, RegisterOperand operand2)
        {
            return EqualityComparer<RegisterOperand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(RegisterOperand operand1, RegisterOperand operand2)
        {
            return !(operand1 == operand2);
        }

        public override string ToString()
        {
            return $"reg{(int)Size}[{Name}]";
        }
    }
}