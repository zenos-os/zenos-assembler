namespace Zenos.Assembly.AST
{
    public class SectionDeclaration : AssemblyDirective
    {
        public SectionDeclaration(string name, SectionEntry[] sectionEntries)
        {
            Name = name;
            SectionEntries = sectionEntries;
        }

        public string Name { get; }
        public SectionEntry[] SectionEntries { get; }
    }
}