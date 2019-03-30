using System.IO;

namespace Zenos.Linker
{
    /// <summary>
    /// Section Header Entry
    /// </summary>
    public class SectionHeaderEntry32 : SectionHeaderEntry
    {
        /// <summary>
        /// This member holds a section header's size in bytes. A section header is one entry
        /// in the section header table; all entries are the same size.
        /// </summary>
        public const ushort EntrySize32 = 0x28;

        /// <summary>
        /// If the section will appear in the memory image of a process, this member
        /// gives the virtualAddress at which the section's first byte should reside. Otherwise,
        /// the member contains 0.
        /// </summary>
        public uint Address { get; }

        /// <summary>
        /// This member's value gives the byte offset from the beginning of the file to
        /// the first byte in the section. One section type, NoBits,occupies no
        /// space in the file, and its Offset member locates
        /// the conceptual placement in the file.
        /// </summary>
        public uint Offset { get; }

        /// <summary>
        ///
        /// </summary>
        public uint Size { get; }

        /// <summary>
        /// Some sections have alignment constraints. For example, if a section
        /// holds a doubleword, the system must ensure doubleword alignment for the
        /// entire section.  That is, the value of sh_addr must be congruent to 0,
        /// modulo the value of sh_addralign. Currently, only 0 and positive
        /// integral powers of two are allowed. Values 0 and 1 mean the section has no
        /// alignment constraints.
        /// </summary>
        public uint AddressAlignment { get; }

        /// <summary>
        /// Some sections hold a table of fixed-size entries, such as a symbol table. For
        /// such a section, this member gives the size in bytes of each entry. The
        /// member contains 0 if the section does not hold a table of fixed-size entries.
        /// </summary>
        public uint EntrySize { get; }

        public SectionHeaderEntry32(uint name, SectionType type, SectionAttribute flags, uint address, uint offset, uint size, int link, int info, uint addressAlignment, uint entrySize)
            : base(name, type, flags, link, info)
        {
            Address = address;
            Offset = offset;
            Size = size;
            AddressAlignment = addressAlignment;
            EntrySize = entrySize;
        }

        /// <summary>
        /// Writes the section header
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write((uint)Type);
            writer.Write((uint)Flags);
            writer.Write(Address);
            writer.Write((int)Offset);
            writer.Write(Size);
            writer.Write(Link);
            writer.Write(Info);
            writer.Write(AddressAlignment);
            writer.Write(EntrySize);
        }
    }
}