namespace Zenos.Assembly.AST
{
    public class ExternDeclaration : AssemblyDirective
    {
        public ExternDeclaration(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}