using System.IO;

namespace Zenos.Linker
{
    internal interface IBinaryWritable
    {
        void WriteTo(BinaryWriter writer);
    }
}