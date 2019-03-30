using System.IO;
using System.Text;

namespace Zenos.Linker
{
    public class EndianAwareBinaryWriter : BinaryWriter
    {
        private readonly bool swap = false;

        public EndianAwareBinaryWriter(Stream input, Endianness endianness)
            : base(input)
        {
            bool isLittleEndian = endianness == Endianness.Little;
            swap = (isLittleEndian != Endian.NativeIsLittleEndian);
        }

        public EndianAwareBinaryWriter(Stream input, Encoding encoding, Endianness endianness)
            : base(input, encoding)
        {
            bool isLittleEndian = endianness == Endianness.Little;
            swap = (isLittleEndian != Endian.NativeIsLittleEndian);
        }

        public override void Write(ushort value)
        {
            value = swap ? Endian.Swap(value) : value;
            base.Write(value);
        }

        public override void Write(uint value)
        {
            value = swap ? Endian.Swap(value) : value;
            base.Write(value);
        }

        public void WriteByte(byte value) => Write(value);

        public void WriteZeroBytes(int size)
        {
            for (int i = 0; i < size; i++)
                Write((byte)0);
        }

        public void WriteZeroBytes(uint size)
        {
            for (uint i = 0; i < size; i++)
                Write((byte)0);
        }

        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }
    }
}