using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Zenos.Assembly.AST;
using CInstruction = Mono.Cecil.Cil.Instruction;
using ZInstruction = Zenos.Assembly.AST.Instruction;

namespace Zenos.Compiler
{
    public class ZenosMethodCompiler
    {
        private delegate void CompileInstructionAction(CInstruction instruction, List<string> imports,
            List<SectionEntry> code);

        private delegate void CompileInstructionWithDataAction<in T>(CInstruction instruction, List<string> imports,
            List<SectionEntry> code, T data);

        private static readonly Dictionary<Code, CompileInstructionAction> _compileInstructionActions = new Dictionary<Code, CompileInstructionAction>
        {
            [Code.Nop] = CompileNop,
            [Code.Ldarg_0] = WithData(0, CompileLoadArgN),
            [Code.Ldarg_1] = WithData(1, CompileLoadArgN),
            [Code.Ldarg_2] = WithData(2, CompileLoadArgN),
            [Code.Ldarg_3] = WithData(3, CompileLoadArgN),

            [Code.Ldloc_0] = WithData(0, CompileLoadLocalN),
            [Code.Ldloc_1] = WithData(1, CompileLoadLocalN),
            [Code.Ldloc_2] = WithData(2, CompileLoadLocalN),
            [Code.Ldloc_3] = WithData(3, CompileLoadLocalN),

            [Code.Stloc_0] = WithData(0, CompileStoreLocalN),
            [Code.Stloc_1] = WithData(1, CompileStoreLocalN),
            [Code.Stloc_2] = WithData(2, CompileStoreLocalN),
            [Code.Stloc_3] = WithData(3, CompileStoreLocalN),

            [Code.Ldarg_S] = IntDataFromOperand(CompileLoadArgN),
            [Code.Starg_S] = IntDataFromOperand(CompileStoreArgN),
            [Code.Ldloc_S] = IntDataFromOperand(CompileLoadLocalN),
            [Code.Stloc_S] = IntDataFromOperand(CompileStoreLocalN),

            //[Code.Ldnull] = ???
            [Code.Ldc_I4_M1] = WithData(-1, CompileLoadConstantI4),
            [Code.Ldc_I4_0] = WithData(0, CompileLoadConstantI4),
            [Code.Ldc_I4_1] = WithData(1, CompileLoadConstantI4),
            [Code.Ldc_I4_2] = WithData(2, CompileLoadConstantI4),
            [Code.Ldc_I4_3] = WithData(3, CompileLoadConstantI4),
            [Code.Ldc_I4_4] = WithData(4, CompileLoadConstantI4),
            [Code.Ldc_I4_5] = WithData(5, CompileLoadConstantI4),
            [Code.Ldc_I4_6] = WithData(6, CompileLoadConstantI4),
            [Code.Ldc_I4_7] = WithData(7, CompileLoadConstantI4),
            [Code.Ldc_I4_8] = WithData(8, CompileLoadConstantI4),
            [Code.Ldc_I4_S] = IntDataFromOperand(CompileLoadConstantI4),
            [Code.Ldc_I4] = IntDataFromOperand(CompileLoadConstantI4),

            //[Code.Ldarg] = CompileLoadArg,
        };

        private static void CompileStoreLocalN(CInstruction instruction, List<string> imports, List<SectionEntry> code, int n)
        {
            // pop local N
            // Pop a value from stack into local variable n

            // addr=LCL+2
            // SP--
            // *addr=*SP
            throw new NotImplementedException();
        }

        private static void CompileLoadLocalN(CInstruction instruction, List<string> imports, List<SectionEntry> code, int n)
        {
            // push local N
            // Load local variable of index n onto stack
            // PUSH constant 17
            // @17 // D=17
            // D=A
            // @SP // *SP=D
            // A=M
            // M=D
            // @SP
            // M=M+1

            // mov rax, SP
            // add rsp, 1
            throw new NotImplementedException();
        }

        private static void CompileStoreArgN(CInstruction instruction, List<string> imports, List<SectionEntry> code, int argNum)
        {
            throw new NotImplementedException();
        }

        private static void CompileLoadArgN(CInstruction instruction, List<string> imports, List<SectionEntry> code, int argNum)
        {
            throw new NotImplementedException();
        }

        private static CompileInstructionAction WithData<T>(T data, CompileInstructionWithDataAction<T> action) => (instruction, imports, code) => action(instruction, imports, code, data);

        private static CompileInstructionAction IntDataFromOperand(CompileInstructionWithDataAction<int> action) => (instruction, imports, code) =>
        {
            action(instruction, imports, code, IntFromObject(instruction.Operand));
        };

        private static void CompileNop(CInstruction instruction, List<string> imports, List<SectionEntry> code)
        {
        }

        private static int IntFromObject(object operand)
        {
            switch (operand)
            {
                case int i:
                    return i;

                case short i:
                    return i;

                case sbyte b:
                    return b;

                default:
                    throw new NotSupportedException("Unexpected integer operand type: " + operand.GetType());
            }
        }

        private static void CompileLoadConstantI4(CInstruction instruction, List<string> imports, List<SectionEntry> code, int data)
        {
            code.Add(new ZInstruction("push", new Imm32Operand(data)));
        }

        public AssemblyListing Compile(MethodDefinition method)
        {
            var imports = new List<string>();
            var exports = new List<string>(); // public methods

            var textSectionInstructions = new List<SectionEntry>();

            if (method.IsPublic)
            {
                exports.Add(method.FullName);
            }

            foreach (var instruction in method.Body.Instructions)
            {
                Compile(instruction, imports, textSectionInstructions);
            }

            var code = new SectionDeclaration(".text", textSectionInstructions.ToArray());
            var directives = new List<AssemblyDirective>();
            directives.AddRange(imports.Select(import => new ExternDeclaration(import)));
            directives.AddRange(exports.Select(export => new GlobalDeclaration(export)));
            directives.Add(code);

            return new AssemblyListing(directives.ToArray());
        }

        private static void Compile(CInstruction instruction, List<string> imports, List<SectionEntry> textSectionInstructions)
        {
            if (_compileInstructionActions.TryGetValue(instruction.OpCode.Code, out var compileAction))
            {
                textSectionInstructions.Add(new SectionLabel($"CIL_{instruction.Offset:X4}"));

                compileAction(instruction, imports, textSectionInstructions);
            }

            throw new NotSupportedException($"Unsupported op code {instruction.OpCode.Code} while compiling instruction: {instruction}");
        }
    }
}