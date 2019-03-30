namespace Zenos.Assembly.AST
{
    public class SectionLabel : SectionEntry
    {
        public SectionLabel(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}