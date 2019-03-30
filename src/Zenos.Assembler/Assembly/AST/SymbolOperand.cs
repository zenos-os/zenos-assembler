namespace Zenos.Assembly.AST
{
    public class SymbolOperand : ImmOperand
    {
        public Symbol Symbol { get; }
        public override OperandSize Size => OperandSize.Size64; // FIXME: this should be dependent on the current BITS settings

        public SymbolOperand(Symbol symbol)
        {
            Symbol = symbol;
        }

        public override ImmOperand Widen() => this;
    }
}