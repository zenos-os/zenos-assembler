using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using GosuParser;
using Zenos.Assembly.AST;
using static GosuParser.CharacterParsers;

namespace Zenos.Assembly
{
    public class AssemblerParser
    {
        private readonly SymbolTable _symbolTable = new SymbolTable();

        public AssemblyListing ParseFile(string filename) => Parse(File.ReadAllText(filename));

        public AssemblyListing Parse(string input)
        {
            string sectionName = "";
            var bits = 64;
            var directives = new List<AssemblyDirective>();
            var sectionEntries = new List<SectionEntry>();

            var lines = input.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var cleanLine = RemoveComment(line);
                if (cleanLine == string.Empty)
                    continue;

                // labels
                var labelIndex = cleanLine.IndexOf(':');
                string inst;
                if (labelIndex > 0)
                {
                    var labelAndInstruction = cleanLine.Split(':', 2).Select(x => x.Trim()).ToArray();
                    sectionEntries.Add(new SectionLabel(labelAndInstruction[0]));
                    inst = labelAndInstruction[1];
                }
                else
                {
                    inst = cleanLine;
                }

                if (string.Empty == inst)
                    continue;

                var instAndOps = inst.Split(new[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).ToArray();
                if (instAndOps.Length > 1)
                {
                    switch (instAndOps[0])
                    {
                        case "global":
                            directives.Add(new GlobalDeclaration(instAndOps[1]));
                            break;

                        case "extern":
                            directives.Add(new ExternDeclaration(instAndOps[1]));
                            break;

                        case "bits":
                            if (!int.TryParse(instAndOps[1], out bits))
                                throw new NotSupportedException("bits directive without integer operand");

                            break;

                        case "section":
                            if (sectionName != "")
                            {
                                directives.Add(new SectionDeclaration(sectionName, sectionEntries.ToArray()));
                                sectionEntries.Clear();
                            }

                            sectionName = instAndOps[1];

                            break;

                        case "db":
                            {
                                ParseData(sectionEntries, OperandSize.Size8, instAndOps[1]);
                                break;
                            }
                        case "dw":
                            {
                                ParseData(sectionEntries, OperandSize.Size16, instAndOps[1]);
                                break;
                            }
                        case "dd":
                            {
                                ParseData(sectionEntries, OperandSize.Size32, instAndOps[1]);
                                break;
                            }
                        case "dq":
                            {
                                ParseData(sectionEntries, OperandSize.Size64, instAndOps[1]);
                                break;
                            }
                        default:
                            var operands = ParseInstructionOperands(instAndOps[1]);

                            sectionEntries.Add(new Instruction(bits, instAndOps[0], operands));
                            break;
                    }
                }
                else
                {
                    sectionEntries.Add(new Instruction(bits, instAndOps[0], new Operand[0]));
                }
            }

            if (sectionEntries.Count > 0)
            {
                directives.Add(new SectionDeclaration(sectionName, sectionEntries.ToArray()));
            }

            return new AssemblyListing(directives.ToArray());
        }

        private void ParseData(List<SectionEntry> sectionEntries, OperandSize size, string operandsString)
        {
            // TODO: handle strings with commas
            var result = DatasParser(size).Run(operandsString);
            switch (result)
            {
                case Parser<char, IEnumerable<Data>>.Success success:
                    foreach (var value in success.Value)
                    {
                        sectionEntries.Add(value);
                    }

                    break;

                case Parser<char, IEnumerable<Data>>.Failure failure:
                    throw new InvalidOperationException(failure.FailureText);
            }

            //var operands = operandsString.Split(',').Select(x => x.Trim());

            //foreach (var operand in operands)
            //{
            //    sectionEntries.Add(ParseDataOperand(size, operand));
            //}
        }

        private Parser<char, IEnumerable<Data>> DatasParser(OperandSize size)
        {
            var doubleQuotedString = Satisfy(c => c != '"')
                .Many()
                .ToStringParser()
                .Between(Char('"'), Char('"'))
                .Select(str => new Data(str));

            var singleQuotedString = Satisfy(c => c != '\'')
                .Many()
                .ToStringParser()
                .Between(Char('\''), Char('\''))
                .Select(str => new Data(str));

            var hexChars =
                from _ in String("0x")
                from hex in AnyOf("0123456789abcdefABCDEF").OneOrMore().ToStringParser()
                select ParseHexAsData(size, hex);

            var intChars =
                from intValue in IntParser
                select ParseIntAsData(size, intValue.ToString()); // HACK: this could be improved

            var dataParser = doubleQuotedString.OrElse(singleQuotedString).OrElse(hexChars).OrElse(intChars);

            var sep = Char(',').TakeLeft(SkipWhitespaceChar.ZeroOrMore());

            return dataParser.SepBy1(sep);
        }

        //private static Data ParseDataOperand(OperandSize size, string operand)
        //{
        //    if ((operand.StartsWith("'") && operand.EndsWith("'")) ||
        //        (operand.StartsWith("\"") && operand.EndsWith("\""))) // string
        //    {
        //        return new Data(operand.Substring(1, operand.Length - 2));
        //    }

        //    if (operand.StartsWith("0x")) //hex
        //    {
        //        var hexValue = operand.Substring(2);

        //        return ParseHexAsData(size, hexValue);
        //    }

        //    // assume int
        //    return ParseIntAsData(size, operand);
        //}

        private static Data ParseIntAsData(OperandSize size, string operand)
        {
            switch (size)
            {
                case OperandSize.Size8 when byte.TryParse(operand, out var result):
                    return (new Data(result));

                case OperandSize.Size16 when short.TryParse(operand, out var result):
                    return (new Data(result));

                case OperandSize.Size32 when int.TryParse(operand, out var result):
                    return (new Data(result));

                case OperandSize.Size64 when long.TryParse(operand, out var result):
                    return (new Data(result));

                case OperandSize.None:
                default:
                    throw new InvalidOperationException("Unknown operand data value: " + operand);
            }
        }

        private static Data ParseHexAsData(OperandSize size, string hexValue)
        {
            switch (size)
            {
                case OperandSize.Size8 when byte.TryParse(hexValue, NumberStyles.HexNumber, null, out var result):
                    return new Data(result);

                case OperandSize.Size16 when short.TryParse(hexValue, NumberStyles.HexNumber, null, out var result):
                    return (new Data(result));

                case OperandSize.Size32 when int.TryParse(hexValue, NumberStyles.HexNumber, null, out var result):
                    return (new Data(result));

                case OperandSize.Size64 when long.TryParse(hexValue, NumberStyles.HexNumber, null, out var result):
                    return (new Data(result));

                case OperandSize.None:
                default:
                    throw new InvalidOperationException("Unknown operand data value: " + hexValue);
            }
        }

        private static string RemoveComment(string line)
        {
            return line.Split(';').First().Trim();
        }

        private Operand[] ParseInstructionOperands(string operandsString)
        {
            var cleanOpsString = operandsString.Trim();
            if (cleanOpsString == string.Empty)
                return new Operand[0];

            var stringOps = cleanOpsString.Split(',').Select(x => x.Trim());

            return stringOps.Select(opText => ParseOperand(opText, false)).ToArray();
        }

        private Operand ParseOperand(string operand, bool isInMemoryOperand)
        {
            if (!isInMemoryOperand)
            {
                if (operand.StartsWith("[") && operand.EndsWith("]"))
                {
                    return new MemoryOperand(ParseOperand(operand.Substring(1, operand.Length - 2), true));
                }

                if (sbyte.TryParse(operand, out var value))
                {
                    return new Imm8Operand(value);
                }

                if (short.TryParse(operand, out var svalue))
                {
                    return new Imm16Operand(svalue);
                }

                if (int.TryParse(operand, out var ivalue))
                {
                    return new Imm32Operand(ivalue);
                }

                if (long.TryParse(operand, out var lvalue))
                {
                    return new Imm64Operand(lvalue);
                }
            }
            else
            {
                if (int.TryParse(operand, out var ivalue))
                {
                    return new Imm32Operand(ivalue);
                }
            }

            if (_registers64.Contains(operand))
            {
                return new Register64Operand(operand);
            }

            if (_registers32.Contains(operand))
            {
                return new Register32Operand(operand);
            }

            if (_registers16.Contains(operand))
            {
                return new Register16Operand(operand);
            }

            if (_registers8.Contains(operand))
            {
                return new Register8Operand(operand);
            }

            return new SymbolOperand(_symbolTable.Resolve(operand));
        }

        private static readonly HashSet<string> _registers64 = new HashSet<string>()
        {
            "rax", "rbx", "rcx", "rdx",
            "rsi", "rdi", "rbp", "rsp",
            "r8", "r9", "r10", "r11",
            "r12", "r13", "r14", "r15"
        };

        private static readonly HashSet<string> _registers32 = new HashSet<string>
        {
            "eax", "ebx", "ecx", "edx",
            "esi", "edi", "ebp", "esp",
            "r8d", "r9d", "r10d ", "r11d ",
            "r12d ", "r13d ", "r14d", "r15d",
        };

        private static readonly HashSet<string> _registers16 = new HashSet<string>
        {
            "ax", "bx", "cx", "dx",
            "si", "di", "bp", "sp",
            "r8w", "r9w", "r10w", "r11w",
            "r12w", "r13w", "r14w", "r15w",
        };

        private static readonly HashSet<string> _registers8 = new HashSet<string>
        {
            "al", "bl", "cl", "dl",
            "sil", "dil", "bpl", "spl",
            "r8b", "r9b", "r10b", "r11b",
            "r12b", "r13b", "r14b", "r15b",
        };
    }
}