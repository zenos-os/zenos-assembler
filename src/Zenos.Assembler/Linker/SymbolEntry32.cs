using System.IO;

namespace Zenos.Linker
{
    /// <summary>
    /// SymbolEntry
    /// </summary>
    public class SymbolEntry32
    {
        public const ushort EntrySize32 = 0x10;

        /// <summary>
        /// This member holds an index into the object file's symbol string table, which holds
        /// the character representations of the symbol names.
        /// </summary>
        public uint Name;

        /// <summary>
        /// This member gives the value of the associated symbol. Depending on the context,
        /// this may be an absolute value, an virtualAddress, and so on; details appear below.
        /// </summary>
        public ulong Value;

        /// <summary>
        /// Many symbols have associated sizes. For example, a data object's size is the number
        /// of bytes contained in the object. This member holds 0 if the symbol has no size or
        /// an unknown size.
        /// </summary>
        public ulong Size;

        /// <summary>
        /// The symbol binding
        /// </summary>
        public SymbolBinding SymbolBinding;

        /// <summary>
        /// The symbol type
        /// </summary>
        public SymbolType SymbolType;

        /// <summary>
        /// The symbol visibility
        /// </summary>
        public SymbolVisibility SymbolVisibility;

        /// <summary>
        /// Every symbol table entry is "defined'' in relation to some section; this member holds
        /// the relevant section header table index.
        /// </summary>
        public int SectionHeaderTableIndex;

        /// <summary>
        /// Gets the Info value.
        /// </summary>
        public byte Info => (byte)((((byte)SymbolBinding) << 4) | (((byte)SymbolType) & 0xF));

        /// <summary>
        /// Gets the other value.
        /// </summary>
        public byte Other => (byte)(((byte)SymbolVisibility) & 0x3);

        public uint GetEntrySize() => EntrySize32;

        /// <summary>
        /// Writes the program header
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write((uint)Value);
            writer.Write((uint)Size);
            writer.Write(Info);
            writer.Write(Other);
            writer.Write((ushort)SectionHeaderTableIndex);
        }
    }
}