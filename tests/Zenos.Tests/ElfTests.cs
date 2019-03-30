using System.IO;
using Xunit;
using Zenos.Linker;
using Version = Zenos.Linker.Version;

namespace Zenos.Assembler.Tests
{
    public class ElfTests
    {
        [Fact]
        public void CanParseBasic()
        {
            var bytes = TestHelpers.NasmBuildElf(TestHelpers.Resource("basic.asm"));

            var stream = new MemoryStream(bytes);
            using (var reader = new BinaryReader(stream))
            {
                var ident = ElfHeaderIdent.ReadFrom(reader);
                Assert.True(ident.IsValid);
                Assert.Equal(IdentClass.Class64, ident.IdentClass);
                Assert.Equal(IdentData.Data2LSB, ident.IdentData);

                using (var endianReader = new EndianAwareBinaryReader(stream, Endianness.Little))
                {
                    var header = ElfHeader64.ReadFrom64(endianReader);
                    Assert.Equal(FileType.Relocatable, header.Type);
                    Assert.Equal(MachineType.X86_64, header.Machine);
                    Assert.Equal(Version.Current, header.Version);
                    Assert.Equal(0ul, header.EntryAddress);
                    Assert.Equal(0ul, header.ProgramHeaderOffset);
                    Assert.Equal(0x40ul, header.SectionHeaderOffset);
                    Assert.Equal(0u, header.Flags);
                    Assert.Equal(0u, header.ProgramHeaderCount);
                    Assert.Equal(5u, header.SectionHeaderCount);
                    Assert.Equal(2, header.SectionHeaderStringIndex);

                    var section0 = SectionHeaderEntry64.ReadFrom(endianReader);
                    Assert.Equal(0u, section0.Name);
                    Assert.Equal(SectionType.Null, section0.Type);

                    var section1 = SectionHeaderEntry64.ReadFrom(endianReader);
                    Assert.Equal(SectionType.ProgBits, section1.Type);
                    Assert.Equal(SectionAttribute.AllocExecute, section1.Flags);
                    Assert.Equal(0x180ul, section1.Offset);
                    Assert.Equal(6u, section1.Size);

                    var section2 = SectionHeaderEntry64.ReadFrom(endianReader);
                    Assert.Equal(SectionType.StringTable, section2.Type);
                    Assert.Equal(0x190ul, section2.Offset);
                    Assert.Equal(33u, section2.Size);

                    var section3 = SectionHeaderEntry64.ReadFrom(endianReader);
                    Assert.Equal(SectionType.SymbolTable, section3.Type);
                    Assert.Equal(0x1C0ul, section3.Offset);
                    Assert.Equal(24u, section3.EntrySize);
                    Assert.Equal(96u, section3.Size);

                    var section4 = SectionHeaderEntry64.ReadFrom(endianReader);
                    Assert.Equal(SectionType.StringTable, section4.Type);
                    Assert.Equal(0x220ul, section4.Offset);
                    Assert.Equal(38u, section4.Size);

                    // section names string table
                    stream.Position = (long)section2.Offset;
                    var stringTable = StringTableSection.ReadFrom(endianReader);
                    Assert.Collection(stringTable.Strings, s => Assert.Equal((string)".text", (string)s),
                        s => Assert.Equal((string)".shstrtab", (string)s),
                        s => Assert.Equal((string)".symtab", (string)s),
                        s => Assert.Equal((string)".strtab", (string)s));

                    stream.Position = (long)section4.Offset;
                    var stringTable2 = StringTableSection.ReadFrom(endianReader);
                    Assert.Collection(stringTable2.Strings, s => Assert.Equal((string)"resources\\Assembler\\basic.asm", (string)s),
                        s => Assert.Equal((string)"_start", (string)s));

                    // TODO parse symbol table
                    // TODO parse all of this into a ElfFile and Sections
                }
            }
        }
    }
}