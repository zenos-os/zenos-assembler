namespace Zenos.Assembly.AST
{
    public class GlobalDeclaration : AssemblyDirective
    {
        public GlobalDeclaration(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}