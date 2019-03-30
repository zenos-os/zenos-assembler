using System;
using System.Text;

namespace Zenos.Assembly.AST
{
    public class Data : SectionEntry
    {
        public byte[] Bytes { get; }

        public Data(byte[] bytes)
        {
            Bytes = bytes;
        }

        public Data(byte value)
            : this(new[] { value })
        {
        }

        public Data(short value)
            : this(BitConverter.GetBytes(value))
        {
        }

        public Data(int value)
            : this(BitConverter.GetBytes(value))
        {
        }

        public Data(long value)
            : this(BitConverter.GetBytes(value))
        {
        }

        public Data(string value)
            : this(Encoding.ASCII.GetBytes(value))
        {
        }
    }
}