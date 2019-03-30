using System.IO;

namespace Zenos.Linker
{
    public sealed class ElfHeaderIdent : IBinaryWritable
    {
        private readonly byte[] _ident;

        /// <summary>
        /// The magic number
        /// </summary>
        public static readonly byte[] MagicNumber = { 0x7F, (byte)'E', (byte)'L', (byte)'F' };

        public ElfHeaderIdent(byte[] bytes)
        {
            this._ident = bytes;
        }

        public bool IsValid => _ident[0] == MagicNumber[0] && _ident[1] == MagicNumber[1] &&
                               _ident[2] == MagicNumber[2] && _ident[3] == MagicNumber[3];

        public IdentClass IdentClass => (IdentClass)_ident[4];
        public IdentData IdentData => (IdentData)_ident[5];

        public ElfHeaderIdent(IdentClass identClass, IdentData data)
        {
            _ident = new byte[16];

            // Store magic number
            _ident[0] = MagicNumber[0];
            _ident[1] = MagicNumber[1];
            _ident[2] = MagicNumber[2];
            _ident[3] = MagicNumber[3];

            // Store class
            _ident[4] = (byte)identClass;

            // Store data flags
            _ident[5] = (byte)data;

            // Version has to be current, otherwise the file won't load
            _ident[6] = (byte)Version.Current;

            _ident[7] = 0x00;

            for (int i = 8; i < 16; ++i)
                _ident[i] = 0x00;
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(_ident);
        }

        public static ElfHeaderIdent ReadFrom(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(16);
            return new ElfHeaderIdent(bytes);
        }
    }
}