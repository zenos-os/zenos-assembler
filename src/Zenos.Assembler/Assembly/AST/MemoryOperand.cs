using System;

namespace Zenos.Assembly.AST
{
    public class MemoryOperand : Operand, IEquatable<MemoryOperand>
    {
        public bool Equals(MemoryOperand other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(InnerOperand, other.InnerOperand);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MemoryOperand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (InnerOperand != null ? InnerOperand.GetHashCode() : 0);
            }
        }

        public static bool operator ==(MemoryOperand left, MemoryOperand right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MemoryOperand left, MemoryOperand right)
        {
            return !Equals(left, right);
        }

        public Operand InnerOperand { get; }
        public OperandType InnerType => InnerOperand.Type;

        public override OperandType Type => OperandType.Memory;
        public override OperandSize Size => OperandSize.Size64; // FIXME: this should be dependent on the current BITS settings

        public MemoryOperand(Operand innerOperand)
        {
            InnerOperand = innerOperand;
        }
    }
}