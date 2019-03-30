namespace Zenos.Assembly.AST
{
    public abstract class ImmOperand : Operand
    {
        public override OperandType Type => OperandType.Immediate;

        public bool CanWiden => this.Size < OperandSize.Size64;

        public abstract ImmOperand Widen();
    }
}