namespace Zenos.Assembly.AST
{
    internal abstract class OperandDefinition
    {
        public abstract OperandType Type { get; }
        public abstract OperandSize Size { get; }
    }
}