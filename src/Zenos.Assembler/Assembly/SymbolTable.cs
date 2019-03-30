using System.Collections.Generic;

namespace Zenos.Assembly
{
    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> _store = new Dictionary<string, Symbol>();

        public SymbolTable()
        {
        }

        public Symbol Export(string name)
        {
            var symbol = Resolve(name);
            return !symbol.Exported ? Update(symbol.Export()) : symbol;
        }

        public Symbol Import(string name)
        {
            var symbol = Resolve(name);
            return !symbol.Imported ? Update(symbol.Import()) : symbol;
        }

        public Symbol Update(Symbol symbol) => _store[symbol.Name] = symbol;

        public Symbol Resolve(string name) =>
            _store.TryGetValue(name, out var sym)
                ? sym
                : Update(new Symbol(_store.Count, name));
    }
}