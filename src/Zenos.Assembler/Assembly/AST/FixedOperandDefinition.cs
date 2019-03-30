namespace Zenos.Assembly.AST
{
    internal class FixedOperandDefinition : OperandDefinition
    {
        public string Name { get; }

        private FixedOperandDefinition(string name, OperandSize size)
        {
            Name = name;
            Size = size;
        }

        public override OperandType Type => OperandType.Fixed;
        public override OperandSize Size { get; }

        public static readonly FixedOperandDefinition AL = new FixedOperandDefinition("al", OperandSize.Size8);
        public static readonly FixedOperandDefinition AX = new FixedOperandDefinition("ax", OperandSize.Size16);
        public static readonly FixedOperandDefinition EAX = new FixedOperandDefinition("eax", OperandSize.Size32);
        public static readonly FixedOperandDefinition RAX = new FixedOperandDefinition("rax", OperandSize.Size64);
    }
}