using System.IO;

namespace Zenos.Linker
{
    public struct NativeInt : IBinaryWritable
    {
        private bool _is64;
        private readonly uint _value32;
        private readonly ulong _value64;

        public NativeInt(uint value)
        {
            _value64 = 0;
            _value32 = value;
            _is64 = false;
        }

        public NativeInt(ulong value)
        {
            _value32 = 0;
            _value64 = value;
            _is64 = true;
        }

        public void WriteTo(BinaryWriter writer)
        {
            if (_is64)
            {
                writer.Write(_value64);
            }
            else
            {
                writer.Write(_value32);
            }
        }
    }
}