using System.IO;

namespace Zenos.Linker
{
    public class EndianAwareBinaryReader : BinaryReader
    {
        private bool swap = false;

        public EndianAwareBinaryReader(Stream input, Endianness endianness)
            : base(input)
        {
            bool isLittleEndian = endianness == Endianness.Little;
            swap = (isLittleEndian != Endian.NativeIsLittleEndian);
        }

        public override ushort ReadUInt16()
        {
            ushort value = base.ReadUInt16();
            return swap ? Endian.Swap(value) : value;
        }

        public override uint ReadUInt32()
        {
            uint value = base.ReadUInt32();
            return swap ? Endian.Swap(value) : value;
        }
    }
}