using System.IO;

namespace Zenos.Linker
{
    public abstract class SectionHeaderEntry : IBinaryWritable
    {
        /// <summary>
        /// This member specifies the name of the section. Its value is an index into
        /// the section header string table section , giving
        /// the location of a null-terminated string.
        /// </summary>
        public uint Name { get; }

        /// <summary>
        /// This member categorizes the section's contents and semantics. Section
        /// types and their descriptions appear below.
        /// </summary>
        public SectionType Type { get; }

        /// <summary>
        /// Sections support 1-bit flags that describe miscellaneous attributes.
        /// </summary>
        public SectionAttribute Flags { get; }

        /// <summary>
        /// This member holds a section header table index link, whose interpretation
        /// depends on the section type.
        /// </summary>
        public int Link { get; }

        /// <summary>
        /// This member holds extra information, whose interpretation depends on the
        /// section type.
        /// </summary>
        public int Info { get; }

        public SectionHeaderEntry(uint name, SectionType type, SectionAttribute flags, int link, int info)
        {
            Name = name;
            Type = type;
            Flags = flags;
            Link = link;
            Info = info;
        }

        public abstract void WriteTo(BinaryWriter writer);
    }
}