using System;
using System.Collections.Generic;
using Xunit;
using Zenos.Assembly;
using Zenos.Assembly.AST;

namespace Zenos.Assembler.Tests
{
    public class AssemblerParsingTests
    {
        [Fact]
        public void ParsesComments()
        {
            var input = ParseFile("comments.asm");

            Assert.Empty(input.Directives);
        }

        [Fact]
        public void ParsesEmptyFile()
        {
            var input = ParseFile("empty-file.asm");

            Assert.Empty(input.Directives);
        }

        [Fact]
        public void ParsesBasicFile()
        {
            var input = ParseFile("basic.asm");

            Assert.Collection(input.Directives,
                AssertGlobal("_start"),
                AssertSection(".text",
                    AssertLabel("_start"),
                    AssertInstruction("push", AssertRegOperand("rax")),
                    AssertInstruction("mov", AssertRegOperand("rdi"), AssertIntOperand(1))
                ));
        }

        [Fact]
        public void ParsesMinimalFile()
        {
            var input = ParseFile("minimal.asm");

            Assert.Collection(input.Directives,
                AssertSection("",
                    AssertInstruction("push", AssertRegOperand("rax"))
                ));
        }

        public static IEnumerable<object[]> ParsePushPopRegsTestCases()
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
        [MemberData(nameof(ParsePushPopRegsTestCases))]
        public void TestParsePushPopRegs(Instruction expected, string assembler)
        {
            var listing = ParseInstructionText(assembler);

            InstructionEqual(expected, listing);
        }

        public static IEnumerable<object[]> ParseMovTestCases()
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

                    // mov [reg1], reg2
                    yield return new object[]
                    {
                        new Instruction(64, "mov",
                            new Operand[] {new MemoryOperand(new Register64Operand(reg1)), new Register64Operand(reg2)}),
                        $"mov [{reg1}], {reg2}"
                    };

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
        [MemberData(nameof(ParseMovTestCases))]
        public void TestParseMov(Instruction expected, string assembler)
        {
            var listing = ParseInstructionText(assembler);

            InstructionEqual(expected, listing);
        }

        public static IEnumerable<object[]> ParseMovFromMemoryOffsetTestCases()
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
        [MemberData(nameof(ParseMovFromMemoryOffsetTestCases))]
        public void TestParseMovFromMemoryOffset(Instruction expected, string assembler)
        {
            var listing = ParseInstructionText(assembler);

            InstructionEqual(expected, listing);
        }

        public static IEnumerable<object[]> ParsePushImmTestCases()
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
        [MemberData(nameof(ParsePushImmTestCases))]
        public void TestEmitPushImm(Instruction expected, string assembler)
        {
            var listing = ParseInstructionText(assembler);

            InstructionEqual(expected, listing);
        }

        public static IEnumerable<object[]> ParseAddTestCases()
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

            // push imm8
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

            // push imm16
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

            // push imm32
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

            // push imm32
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
        }

        [Theory]
        [MemberData(nameof(ParseAddTestCases))]
        public void TestParseAdd(Instruction expected, string assembler)
        {
            var listing = ParseInstructionText(assembler);

            InstructionEqual(expected, listing);
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
        public void TestParseData(Data expected, string assembler)
        {
            var actual = ParseDataText(assembler);

            Assert.Equal(expected.Bytes, actual.Bytes);
        }

        private static void InstructionEqual(Instruction expected, Instruction actual)
        {
            Assert.Equal(expected.Bits, actual.Bits);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Operands.Length, actual.Operands.Length);

            for (int i = 0; i < expected.Operands.Length; i++)
            {
                Assert.True(expected.Operands[i].Equals(actual.Operands[i]));
            }
        }

        private static Action<SectionEntry> AssertInstruction(string instName, params Action<Operand>[] actions) => sectionEntry =>
        {
            var inst = Assert.IsType<Instruction>(sectionEntry);
            Assert.Equal(instName, inst.Name);
            Assert.Collection(inst.Operands, actions);
        };

        private static Action<Operand> AssertRegOperand(string regName) => operand =>
        {
            var reg = Assert.IsType<Register64Operand>(operand);
            Assert.Equal(regName, reg.Name);
        };

        private static Action<Operand> AssertIntOperand(int value) => operand =>
        {
            var intOp = Assert.IsType<Imm8Operand>(operand);
            Assert.Equal(value, intOp.Value);
        };

        private static Action<SectionEntry> AssertLabel(string name) => section =>
        {
            var label = Assert.IsType<SectionLabel>(section);
            Assert.Equal(name, label.Name);
        };

        private static Action<AssemblyDirective> AssertSection(string name, params Action<SectionEntry>[] actions) => directive =>
        {
            var section = Assert.IsType<SectionDeclaration>(directive);
            Assert.Equal(name, section.Name);
            Assert.Collection(section.SectionEntries, actions);
        };

        private static Action<AssemblyDirective> AssertGlobal(string name) => directive =>
        {
            var global = Assert.IsType<GlobalDeclaration>(directive);
            Assert.Equal(name, global.Name);
        };

        private AssemblyListing ParseFile(string filename)
        {
            var parser = new AssemblerParser();
            var listing = parser.ParseFile(TestHelpers.Resource(filename));

            Assert.NotNull(listing);

            return listing;
        }

        private Data ParseDataText(string text)
        {
            var listing = ParseText("section .data\r\n" + text);

            var directive = Assert.Single(listing.Directives);
            var section = Assert.IsType<SectionDeclaration>(directive);

            Assert.Equal(".data", section.Name);

            var sectionEntry = Assert.Single(section.SectionEntries);
            return Assert.IsType<Data>(sectionEntry);
        }

        private Instruction ParseInstructionText(string text)
        {
            var listing = ParseText("section .text\r\n" + text);

            var directive = Assert.Single(listing.Directives);
            var section = Assert.IsType<SectionDeclaration>(directive);

            Assert.Equal(".text", section.Name);
            var sectionEntry = Assert.Single(section.SectionEntries);

            return Assert.IsType<Instruction>(sectionEntry);
        }

        private AssemblyListing ParseText(string text)
        {
            var parser = new AssemblerParser();
            var listing = parser.Parse(text);

            Assert.NotNull(listing);

            return listing;
        }
    }
}