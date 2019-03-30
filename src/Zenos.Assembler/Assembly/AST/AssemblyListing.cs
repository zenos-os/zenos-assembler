namespace Zenos.Assembly.AST
{
    public class AssemblyListing : AstNode
    {
        public AssemblyListing(AssemblyDirective[] directives)
        {
            Directives = directives;
        }

        public AssemblyDirective[] Directives { get; }
    }

    public class AstNode
    {
    }
}