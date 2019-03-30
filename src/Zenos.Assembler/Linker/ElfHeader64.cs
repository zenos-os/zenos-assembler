using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Zenos.Linker
{
    /// <summary>
    /// Elf Header
    /// </summary>
    public sealed class ElfHeader64 : IBinaryWritable
    {
        /// <summary>
        /// This member identifies the object file type
        /// </summary>
        public FileType Type { get; }

        /// <summary>
        /// This member's value specifies the required architecture for an individual file.
        /// </summary>
        public MachineType Machine { get; }

        /// <summary>
        /// This member identifies the object file version.
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// This member gives the virtual virtualAddress to which the system first transfers control,
        /// thus starting the process. If the file has no associated entry point, this member holds
        /// zero.
        /// </summary>
        public ulong EntryAddress { get; }

        /// <summary>
        /// This member holds the program header table's file offset in bytes. If the file has no
        /// program header table, this member holds zero.
        /// </summary>
        public ulong ProgramHeaderOffset { get; }

        /// <summary>
        /// This member holds the section header table's file offset in bytes. If the file has no
        /// section header table, this member holds zero.
        /// </summary>
        public ulong SectionHeaderOffset { get; }

        /// <summary>
        /// This member holds processor-specific flags associated with the file. Flag names
        /// take the form EF_machine_flag.
        /// </summary>
        public uint Flags { get; }

        /// <summary>
        /// This member holds the ELF header's size in bytes.
        /// </summary>
        internal const ushort ElfHeaderSize64 = 0x40;

        /// <summary>
        /// This member holds the number of entries in the program header table. Thus the
        /// product of ProgramHeaderEntrySize and ProgramHeaderNumber gives the table's size in bytes. If a file
        /// has no program header table,  ProgramHeaderNumber holds the value zero.
        /// </summary>
        public ushort ProgramHeaderCount { get; }

        /// <summary>
        /// This member holds the number of entries in the section header table. Thus the
        /// product of SectionHeaderEntrySize and SectionHeaderNumber gives the section header table's size in
        /// bytes. If a file has no section header table,  SectionHeaderNumber holds the value zero.
        /// </summary>
        public ushort SectionHeaderCount { get; }

        /// <summary>
        /// This member holds the section header table index of the entry associated with the
        /// section name string table. If the file has no section name string table, this member
        /// holds the value  SHN_UNDEF. See "Sections" and "String Table" below for more
        /// information.
        /// </summary>
        public int SectionHeaderStringIndex { get; }

        public ElfHeader64(FileType type, MachineType machine, Version version, ulong entryAddress, ulong programHeaderOffset, ushort programHeaderCount, ulong sectionHeaderOffset, ushort sectionHeaderCount, uint flags, int sectionHeaderStringIndex)
        {
            Type = type;
            Machine = machine;
            Version = version;
            EntryAddress = entryAddress;
            ProgramHeaderOffset = programHeaderOffset;
            ProgramHeaderCount = programHeaderCount;
            SectionHeaderOffset = sectionHeaderOffset;
            SectionHeaderCount = sectionHeaderCount;
            Flags = flags;
            SectionHeaderStringIndex = sectionHeaderStringIndex;
        }

        /// <summary>
        /// Writes the elf header
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((ushort)Type);             // type
            writer.Write((ushort)Machine);          // machine
            writer.Write((uint)Version);            // version

            writer.Write(EntryAddress);             // entry

            writer.Write((ulong)ProgramHeaderOffset);      // phoff
            writer.Write((ulong)SectionHeaderOffset);      // shoff
            writer.Write(Flags);                    // flags
            writer.Write(ElfHeaderSize64);            // ehsize
            writer.Write(ProgramHeader.EntrySize64);   // phentsize
            writer.Write(ProgramHeaderCount);      // phnum
            writer.Write(SectionHeaderEntry64.EntrySize64);   // shentsize
            writer.Write((ushort)SectionHeaderCount);      // shnum
            writer.Write((ushort)SectionHeaderStringIndex); // shstrndx
        }

        /// <summary>
        /// Reads elf header
        /// </summary>
        /// <param name="reader">The reader.</param>
        public static ElfHeader64 ReadFrom64(EndianAwareBinaryReader reader)
        {
            var Type = (FileType)reader.ReadUInt16();
            var Machine = (MachineType)reader.ReadUInt16();
            var Version = (Version)reader.ReadUInt32();
            var EntryAddress = reader.ReadUInt64();
            var ProgramHeaderOffset = reader.ReadUInt64();
            var SectionHeaderOffset = reader.ReadUInt64();
            var Flags = reader.ReadUInt32();

            var ElfHeaderSize = reader.ReadUInt16();
            var ProgramHeaderEntrySize = reader.ReadUInt16();
            var ProgramHeaderCount = reader.ReadUInt16();

            var SectionHeaderEntrySize = reader.ReadUInt16();
            var SectionHeaderCount = reader.ReadUInt16();
            var SectionHeaderStringIndex = reader.ReadUInt16();

            return new ElfHeader64(Type, Machine, Version, EntryAddress, ProgramHeaderOffset, ProgramHeaderCount,
                SectionHeaderOffset, SectionHeaderCount, Flags, SectionHeaderStringIndex);
        }

        /// <summary>
        /// Prints the info.
        /// </summary>
        public void PrintInfo()
        {
            Console.WriteLine("--------------");
            Console.WriteLine("| Elf Info:");
            Console.WriteLine("--------------");
            Console.WriteLine();
            Console.WriteLine("Magic number equals 0x7F454C46: Yes");
            Console.WriteLine("FileType:                       {0}", Type);
            Console.WriteLine("Machine:                        {0}", Machine);
            Console.WriteLine("Version:                        {0}", Version);
            Console.WriteLine("Entry VirtualAddress:           0x{0:x}", EntryAddress);
            Console.WriteLine("ProgramHeaderOffset:            0x{0:x}", ProgramHeaderOffset);
            Console.WriteLine("SectionHeaderOffset:            0x{0:x}", SectionHeaderOffset);
            Console.WriteLine("Flags:                          0x{0:x}", Flags);
            Console.WriteLine("ProgramHeaderNumber:            0x{0:x}", ProgramHeaderCount);
            Console.WriteLine("SectionHeaderNumber:            0x{0:x}", SectionHeaderCount);
            Console.WriteLine("SectionHeaderStringIndex:       0x{0:x}", SectionHeaderStringIndex);
        }
    }
}