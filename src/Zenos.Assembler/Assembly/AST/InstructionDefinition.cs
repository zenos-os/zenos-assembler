using System.Collections.Generic;

namespace Zenos.Assembly.AST
{
    internal class InstructionDefinition
    {
        public InstructionDefinition(List<byte> opCode, OperandDefinition[] operands, bool opcodeReg = false, bool w = false)
        {
            OpCode = opCode;
            Operands = operands;
            OpcodeReg = opcodeReg;
            W = w;
        }

        public List<byte> OpCode { get; }
        public OperandDefinition[] Operands { get; }
        public bool OpcodeReg { get; }
        public bool W { get; }
    }
}