using System;
using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    public abstract class Operand : AstNode, IEquatable<Operand>
    {
        public abstract OperandType Type { get; }
        public abstract OperandSize Size { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Operand);
        }

        public bool Equals(Operand other)
        {
            return other != null && Type == other.Type && Size == other.Size;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type);
        }

        public static bool operator ==(Operand operand1, Operand operand2)
        {
            return EqualityComparer<Operand>.Default.Equals(operand1, operand2);
        }

        public static bool operator !=(Operand operand1, Operand operand2)
        {
            return !(operand1 == operand2);
        }
    }
}