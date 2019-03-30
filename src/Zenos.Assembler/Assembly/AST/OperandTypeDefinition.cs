namespace Zenos.Assembly.AST
{
    internal class OperandTypeDefinition : OperandDefinition
    {
        public override OperandType Type { get; }
        public override OperandSize Size { get; }

        private OperandTypeDefinition(OperandType type, OperandSize size)
        {
            Type = type;
            Size = size;
        }

        public static readonly OperandTypeDefinition Memory8 = new OperandTypeDefinition(OperandType.Memory, OperandSize.Size8);
        public static readonly OperandTypeDefinition Memory16 = new OperandTypeDefinition(OperandType.Memory, OperandSize.Size16);
        public static readonly OperandTypeDefinition Memory32 = new OperandTypeDefinition(OperandType.Memory, OperandSize.Size32);
        public static readonly OperandTypeDefinition Memory64 = new OperandTypeDefinition(OperandType.Memory, OperandSize.Size64);

        public static readonly OperandTypeDefinition Reg8 = new OperandTypeDefinition(OperandType.Register, OperandSize.Size8);
        public static readonly OperandTypeDefinition Reg16 = new OperandTypeDefinition(OperandType.Register, OperandSize.Size16);
        public static readonly OperandTypeDefinition Reg32 = new OperandTypeDefinition(OperandType.Register, OperandSize.Size32);
        public static readonly OperandTypeDefinition Reg64 = new OperandTypeDefinition(OperandType.Register, OperandSize.Size64);

        public static readonly OperandTypeDefinition Imm8 = new OperandTypeDefinition(OperandType.Immediate, OperandSize.Size8);
        public static readonly OperandTypeDefinition Imm16 = new OperandTypeDefinition(OperandType.Immediate, OperandSize.Size16);
        public static readonly OperandTypeDefinition Imm32 = new OperandTypeDefinition(OperandType.Immediate, OperandSize.Size32);
    }
}