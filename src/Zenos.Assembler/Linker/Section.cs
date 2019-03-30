using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Zenos.Linker
{
    public class ObjectFile
    {
        public ObjectFile(Section[] sections)
        {
            Sections = sections;
        }

        public Section[] Sections { get; }
    }

    public class SymbolTableSection : Section
    {
    }

    public class StringTableSection : Section
    {
        public string[] Strings { get; }

        private StringTableSection(string[] strings)
        {
            Strings = strings;
        }

        public static StringTableSection ReadFrom(EndianAwareBinaryReader reader)
        {
            var list = new List<string>();
            reader.ReadByte();

            var sb = new StringBuilder();
            while (true)
            {
                var b = reader.ReadByte();
                if (b == 0)
                {
                    if (sb.Length == 0)
                    {
                        // no more strings
                        break;
                    }

                    list.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }

                sb.Append((char)b);
            }

            return new StringTableSection(list.ToArray());
        }
    }

    public abstract class Section
    {
    }
}