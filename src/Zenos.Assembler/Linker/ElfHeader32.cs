using System;
using System.IO;

namespace Zenos.Linker
{
    /// <summary>
    /// Elf Header
    /// </summary>
    public sealed class ElfHeader32 : IBinaryWritable
    {
        /// <summary>
        /// The initial bytes mark the file as an object file and provide machine-independent
        /// data with which to decode and interpret the file's contents.
        /// </summary>
        public ElfHeaderIdent Ident { get; }

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
        public IntPtr EntryAddress { get; }

        /// <summary>
        /// This member holds the program header table's file offset in bytes. If the file has no
        /// program header table, this member holds zero.
        /// </summary>
        public IntPtr ProgramHeaderOffset { get; }

        /// <summary>
        /// This member holds the section header table's file offset in bytes. If the file has no
        /// section header table, this member holds zero.
        /// </summary>
        public IntPtr SectionHeaderOffset { get; }

        /// <summary>
        /// This member holds processor-specific flags associated with the file. Flag names
        /// take the form EF_machine_flag.
        /// </summary>
        public uint Flags { get; }

        /// <summary>
        /// This member holds the ELF header's size in bytes.
        /// </summary>
        internal const ushort ElfHeaderSize32 = 0x34;

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

        public ElfHeader32(FileType type, MachineType machine, Version version, IntPtr entryAddress, IntPtr programHeaderOffset, ushort programHeaderCount, IntPtr sectionHeaderOffset, ushort sectionHeaderCount, uint flags, int sectionHeaderStringIndex)
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
            writer.Write((uint)EntryAddress);             // entry

            writer.Write((uint)ProgramHeaderOffset);      // phoff
            writer.Write((uint)SectionHeaderOffset);      // shoff
            writer.Write(Flags);                    // flags
            writer.Write(ElfHeaderSize32);            // ehsize
            writer.Write(ProgramHeader.EntrySize32);   // phentsize
            writer.Write(ProgramHeaderCount);      // phnum
            writer.Write(SectionHeaderEntry32.EntrySize32);   // shentsize
            writer.Write((ushort)SectionHeaderCount);      // shnum
            writer.Write((ushort)SectionHeaderStringIndex); // shstrndx
        }

        ///// <summary>
        ///// Reads elf header
        ///// </summary>
        ///// <param name="reader">The reader.</param>
        //public static ElfHeader64 ReadFrom32(EndianAwareBinaryReader reader)
        //{
        //    Type = (FileType)reader.ReadUInt16();
        //    Machine = (MachineType)reader.ReadUInt16();
        //    Version = (Version)reader.ReadUInt32();
        //    EntryAddress = reader.ReadUInt32();
        //    ProgramHeaderOffset = reader.ReadUInt32();
        //    SectionHeaderOffset = reader.ReadUInt32();
        //    Flags = reader.ReadUInt32();

        //    //ElfHeaderSize = reader.ReadUInt16();
        //    //ProgramHeaderEntrySize = reader.ReadUInt16();
        //    ProgramHeaderNumber = reader.ReadUInt16();

        //    //SectionHeaderEntrySize = reader.ReadUInt16();
        //    SectionHeaderNumber = reader.ReadUInt16();
        //    SectionHeaderStringIndex = reader.ReadUInt16();
        //}

        ///// <summary>
        ///// Prints the info.
        ///// </summary>
        //public void PrintInfo()
        //{
        //    Console.WriteLine("--------------");
        //    Console.WriteLine("| Elf Info:");
        //    Console.WriteLine("--------------");
        //    Console.WriteLine();
        //    Console.WriteLine("Magic number equals 0x7F454C46: Yes");
        //    Console.WriteLine("Ident class:                    {0} ({1:x})", (IdentClass)Ident[4], ((IdentClass)Ident[4]));
        //    Console.WriteLine("Ident data:                     {0} ({1:x})", (IdentData)Ident[4], ((IdentData)Ident[4]));
        //    Console.WriteLine("FileType:                       {0}", Type);
        //    Console.WriteLine("Machine:                        {0}", Machine);
        //    Console.WriteLine("Version:                        {0}", Version);
        //    Console.WriteLine("Entry VirtualAddress:           0x{0:x}", EntryAddress);
        //    Console.WriteLine("ProgramHeaderOffset:            0x{0:x}", ProgramHeaderOffset);
        //    Console.WriteLine("SectionHeaderOffset:            0x{0:x}", SectionHeaderOffset);
        //    Console.WriteLine("Flags:                          0x{0:x}", Flags);
        //    Console.WriteLine("ProgramHeaderNumber:            0x{0:x}", ProgramHeaderNumber);
        //    Console.WriteLine("SectionHeaderNumber:            0x{0:x}", SectionHeaderNumber);
        //    Console.WriteLine("SectionHeaderStringIndex:       0x{0:x}", SectionHeaderStringIndex);
        //}
    }
}