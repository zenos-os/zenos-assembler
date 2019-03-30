using System.Collections.Generic;
using Xunit;
using Zenos.Assembly.AST;

namespace Zenos.Assembler.Tests
{
    public class InstructionEmitterTests
    {
        public static IEnumerable<object[]> EmitPushPopRegsTestCases()
        {
            var registers = new[]
            {
                "rax",
                "rbx",
                "rcx",
                "rdx",
                "rsi",
                "rdi",
                "rbp",
                "rsp",
                "r8",
                "r9",
                "r10",
                "r11",
                "r12",
                "r13",
                "r14",
                "r15",
            };

            // push reg
            foreach (var register in registers)
            {
                yield return new object[]
                {
                    new Instruction(64, "push", new Operand[] {new Register64Operand(register)}),
                    $"push {register}"
                };
            }

            // pop reg
            foreach (var register in registers)
            {
                yield return new object[]
                {
                    new Instruction(64, "pop", new Operand[] {new Register64Operand(register)}),
                    $"pop {register}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(EmitPushPopRegsTestCases))]
        public void TestEmitPushPopRegs(Instruction instruction, string assembler)
        {
            EmitTest(instruction, assembler);
        }

        public static IEnumerable<object[]> EmitPushImmTestCases()
        {
            var immediate8Values = new sbyte[]
            {
                -128,
                -127,
                -126,
                -1,
                0,
                1,
                126,
                127,
            };

            var imm16Values = new short[]
            {
                -255,
                -300,
                -1234,
                128,
                255,
                300,
                1234,
            };

            var imm32Values = new int[]
            {
                -56789,
                -101112,
                -1314151,
                -71819202,
                -122232425,
                56789,
                101112,
                1314151,
                71819202,
                122232425,
            };

            yield return new object[]
            {
                new Instruction(64, "push", new Operand[] {new Imm8Operand(0)}),
                $"push 0"
            };

            // push imm8
            foreach (var imm in immediate8Values)
            {
                yield return new object[]
                {
                    new Instruction(64, "push", new Operand[] {new Imm8Operand(imm)}),
                    $"push {imm}"
                };
            }

            // push imm16
            foreach (var imm in imm16Values)
            {
                yield return new object[]
                {
                    new Instruction(64, "push", new Operand[] {new Imm16Operand(imm)}),
                    $"push {imm}"
                };
            }

            // push imm32
            foreach (var imm in imm32Values)
            {
                yield return new object[]
                {
                    new Instruction(64, "push", new Operand[] {new Imm32Operand(imm)}),
                    $"push {imm}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(EmitPushImmTestCases))]
        public void TestEmitPushImm(Instruction instruction, string assembler)
        {
            EmitTest(instruction, assembler);
        }

        public static IEnumerable<object[]> EmitAddTestCases()
        {
            var immediate8Values = new sbyte[]
            {
                -128,
                -127,
                -126,
                -1,
                0,
                1,
                126,
                127,
            };

            var imm16Values = new short[]
            {
                -255,
                -300,
                -1234,
                128,
                255,
                300,
                1234,
            };

            var imm32Values = new int[]
            {
                -56789,
                -101112,
                -1314151,
                -71819202,
                -122232425,
                56789,
                101112,
                1314151,
                71819202,
                122232425,
            };

            yield return new object[]
            {
                new Instruction(64, "add", new Operand[] {
                    new Register32Operand("eax"),
                    new Imm8Operand(0)
                }),
                $"add eax, 0"
            };

            yield return new object[]
            {
                new Instruction(64, "add", new Operand[] {
                    new Register16Operand("ax"),
                    new Imm8Operand(0)
                }),
                $"add ax, 0"
            };

            yield return new object[]
            {
                new Instruction(64, "add", new Operand[] {
                    new Register64Operand("rax"),
                    new Imm8Operand(0)
                }),
                $"add rax, 0"
            };

            // add imm8
            foreach (var imm in immediate8Values)
            {
                yield return new object[]
                {
                    new Instruction(64, "add", new Operand[]
                    {
                        new Register8Operand("al"),
                        new Imm8Operand(imm)
                    }),
                    $"add al, {imm}"
                };
            }

            // add imm16
            foreach (var imm in imm16Values)
            {
                yield return new object[]
                {
                    new Instruction(64, "add", new Operand[]
                    {
                        new Register16Operand("ax"),
                        new Imm16Operand(imm)
                    }),
                    $"add ax, {imm}"
                };
            }

            // add imm32
            foreach (var imm in imm32Values)
            {
                yield return new object[]
                {
                    new Instruction(64, "add", new Operand[]
                    {
                        new Register32Operand("eax"),
                        new Imm32Operand(imm)
                    }),
                    $"add eax, {imm}"
                };
            }

            // add imm32
            foreach (var imm in imm32Values)
            {
                yield return new object[]
                {
                    new Instruction(64, "add", new Operand[]
                    {
                        new Register64Operand("rax"),
                        new Imm32Operand(imm)
                    }),
                    $"add rax, {imm}"
                };
            }

            // add rax, rcx
            yield return new object[]
            {
                new Instruction(64, "add", new Operand[]
                {
                    new Register64Operand("rax"),
                    new Register64Operand("rcx")
                }),
                $"add rax, rcx"
            };

            // add rsp, 1
            yield return new object[]
            {
                new Instruction(64, "add", new Operand[]
                {
                    new Register64Operand("rsp"),
                    new Imm8Operand(1),
                }),
                $"add rsp, 1"
            };
        }

        [Theory]
        [MemberData(nameof(EmitAddTestCases))]
        public void TestEmitAdd(Instruction instruction, string assembler)
        {
            EmitTest(instruction, assembler);
        }

        public static IEnumerable<object[]> EmitOffsetTestCases()
        {
            var registers = new[]
            {
                "rax",
                "rbx",
                "rcx",
                "rdx",
                "rsi",
                "rdi",
                "rbp",
                "rsp",
                "r8",
                "r9",
                "r10",
                "r11",
                "r12",
                "r13",
                "r14",
                "r15",
            };

            var values = new[]
            {
                -122232425,
                -71819202,
                -1314151,
                -101112,
                -56789,
                -1234,
                -300,
                -255,
                -128,
                -127,
                -126,
                -1,
                0,
                1,
                126,
                127,
                128,
                255,
                300,
                1234,
                56789,
                101112,
                1314151,
                71819202,
                122232425,
            };

            foreach (var value in values)
            {
                var imm = new Imm32Operand(value);
                var memoryOperand = new MemoryOperand(imm);

                foreach (var reg1 in registers)
                {
                    var register64Operand = new Register64Operand(reg1);

                    // mov [offset], reg2
                    yield return new object[]
                    {
                        new Instruction(64, "mov", new Operand[] {memoryOperand, register64Operand}),
                        $"mov [{value}], {reg1}"
                    };

                    // mov reg1, [offset]
                    yield return new object[]
                    {
                        new Instruction(64, "mov", new Operand[] {register64Operand, memoryOperand}),
                        $"mov {reg1}, [{value}]"
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EmitOffsetTestCases))]
        public void TestEmitOffsets(Instruction instruction, string assembler)
        {
            EmitTest(instruction, assembler);
        }

        public static IEnumerable<object[]> ParseDataTestCases()
        {
            var immediate8Values = new byte[]
            {
                0,
                1,
                126,
                127,
                128,
                192,
                224,
                240,
                248,
                252,
                254,
                255,
            };

            var imm16Values = new short[]
            {
                -255,
                -300,
                -1234,
                0,
                128,
                255,
                300,
                1234,
            };

            var imm32Values = new int[]
            {
                -56789,
                -101112,
                -1314151,
                -71819202,
                -122232425,
                0,
                56789,
                101112,
                1314151,
                71819202,
                122232425,
            };

            var imm64Values = new long[]
            {
                -56789,
                -101112,
                -1314151,
                -71819202,
                -122232425,
                0,
                56789,
                101112,
                1314151,
                71819202,
                122232425,
            };

            yield return new object[]
            {
                new Data(0),
                $"dd 0"
            };

            // db imm8
            foreach (var imm in immediate8Values)
            {
                yield return new object[]
                {
                    new Data(imm),
                    $"db {imm}"
                };
            }

            foreach (var imm in immediate8Values)
            {
                yield return new object[]
                {
                    new Data(imm),
                    $"db 0x{imm:X2}"
                };
            }

            // dw imm16
            foreach (var imm in imm16Values)
            {
                yield return new object[]
                {
                    new Data(imm),
                    $"dw {imm}"
                };
            }

            // dw imm16
            foreach (var imm in imm16Values)
            {
                yield return new object[]
                {
                    new Data(imm),
                    $"dw 0x{imm:X2}"
                };
            }

            // dd imm32
            foreach (var imm in imm32Values)
            {
                yield return new object[]
                {
                    new Data(imm),
                    $"dd {imm}"
                };
            }

            // dd imm32
            foreach (var imm in imm32Values)
            {
                yield return new object[]
                {
                    new Data(imm),
                    $"dd 0x{imm:X4}"
                };
            }

            // dq imm32
            foreach (var imm in imm64Values)
            {
                yield return new object[]
                {
                    new Data(imm),
                    $"dq {imm}"
                };
            }

            // dq heximm32
            foreach (var imm in imm64Values)
            {
                yield return new object[]
                {
                    new Data(imm),
                    $"dq 0x{imm:X8}"
                };
            }

            yield return new object[]
            {
                new Data("Hello, World"),
                $"db 'Hello, World'"
            };
        }

        [Theory]
        [MemberData(nameof(ParseDataTestCases))]
        public void TestEmitData(Data expected, string assembler)
        {
            EmitDataTest(expected, assembler);
        }

        public static IEnumerable<object[]> EmitMovRegRegTestCases()
        {
            var registers = new[]
            {
                "rax",
                "rbx",
                "rcx",
                "rdx",
                "rsi",
                "rdi",
                "rbp",
                "rsp",
                "r8",
                "r9",
                "r10",
                "r11",
                "r12",
                "r13",
                "r14",
                "r15",
            };

            foreach (var reg1 in registers)
            {
                foreach (var reg2 in registers)
                {
                    // mov reg1, reg2
                    yield return new object[]
                    {
                        new Instruction(64, "mov",
                            new Operand[] {new Register64Operand(reg1), new Register64Operand(reg2)}),
                        $"mov {reg1}, {reg2}"
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EmitMovRegRegTestCases))]
        public void TestEmitMoveRegReg(Instruction expected, string assembler)
        {
            EmitTest(expected, assembler);
        }

        public static IEnumerable<object[]> EmitMovMemRegTestCases()
        {
            var registers = new[]
            {
                "rax",
                "rbx",
                "rcx",
                "rdx",
                "rsi",
                "rdi",
                "rbp",
                "rsp",
                "r8",
                "r9",
                "r10",
                "r11",
                "r12",
                "r13",
                "r14",
                "r15",
            };

            foreach (var reg1 in registers)
            {
                foreach (var reg2 in registers)
                {
                    // mov [reg1], reg2
                    yield return new object[]
                    {
                        new Instruction(64, "mov",
                            new Operand[] {new MemoryOperand(new Register64Operand(reg1)), new Register64Operand(reg2)}),
                        $"mov [{reg1}], {reg2}"
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EmitMovMemRegTestCases))]
        public void TestEmitMoveMemReg(Instruction expected, string assembler)
        {
            EmitTest(expected, assembler);
        }

        public static IEnumerable<object[]> EmitMovRegMemTestCases()
        {
            var registers = new[]
            {
                "rax",
                "rbx",
                "rcx",
                "rdx",
                "rsi",
                "rdi",
                "rbp",
                "rsp",
                "r8",
                "r9",
                "r10",
                "r11",
                "r12",
                "r13",
                "r14",
                "r15",
            };

            foreach (var reg1 in registers)
            {
                foreach (var reg2 in registers)
                {
                    // mov reg1, [reg2]
                    yield return new object[]
                    {
                        new Instruction(64, "mov",
                            new Operand[] {new Register64Operand(reg1), new MemoryOperand(new Register64Operand(reg2))}),
                        $"mov {reg1}, [{reg2}]"
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EmitMovRegMemTestCases))]
        public void TestEmitMoveRegMem(Instruction expected, string assembler)
        {
            EmitTest(expected, assembler);
        }

        public static IEnumerable<object[]> EmitMovRegImmTestCases()
        {
            var registers = new[]
            {
                "rax",
                "rbx",
                "rcx",
                "rdx",
                "rsi",
                "rdi",
                "rbp",
                "rsp",
                "r8",
                "r9",
                "r10",
                "r11",
                "r12",
                "r13",
                "r14",
                "r15",
            };

            var immediate8Values = new sbyte[]
            {
                -128,
                -127,
                -126,
                -1,
                0,
                1,
                126,
                127,
            };

            foreach (var reg1 in registers)
            {
                foreach (var imm in immediate8Values)
                {
                    // mov reg1, imm
                    yield return new object[]
                    {
                        new Instruction(64, "mov",
                            new Operand[] { new Register64Operand(reg1), new Imm8Operand(imm) }),
                        $"mov {reg1}, {imm}"
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EmitMovRegImmTestCases))]
        public void TestEmitMoveRegImm(Instruction expected, string assembler)
        {
            EmitTest(expected, assembler);
        }

        private static void EmitDataTest(Data data, string assembler)
        {
            var actual = new List<byte>();
            actual.AddRange(data.Bytes);

            var expected = TestHelpers.NasmBuildBinFromString64(assembler);

            for (var i = 0; i < expected.Length; i++)
            {
                if (i >= actual.Count)
                {
                    Assert.True(false, $"Expected: '0x{expected[i]:X2}' at offset {i}.");
                }
                else
                {
                    Assert.True(expected[i] == actual[i],
                        $"Expected: '0x{expected[i]:X2}', Actual: '0x{actual[i]:X2}' at offset {i}."
                    );
                }
            }
        }

        private static void EmitTest(Instruction instruction, string assembler)
        {
            var actual = new List<byte>();

            Assert.Null(InstructionEmitter.Emit(instruction, actual));

            var expected = TestHelpers.NasmBuildBinFromString($"bits 64\r\n{assembler}");

            TestHelpers.AssertByteArrays(expected, actual, $"Assembling `{assembler}`:\r\n");
        }
    }
}