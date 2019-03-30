using System;

namespace Zenos.Assembly
{
    public class Symbol : IEquatable<Symbol>
    {
        public string Name { get; }
        public int Index { get; }
        public SymbolType SymbolType { get; }
        public bool Exported => SymbolType == SymbolType.Export;
        public bool Imported => SymbolType == SymbolType.Import;

        public Symbol(int index, string name, SymbolType symbolType = SymbolType.Normal)
        {
            Index = index;
            Name = name;
            SymbolType = symbolType;
        }

        public Symbol Export() => new Symbol(Index, Name, SymbolType.Export);

        public Symbol Import() => new Symbol(Index, Name, SymbolType.Import);

        public bool Equals(Symbol other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Index == other.Index && SymbolType == other.SymbolType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Symbol)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Index;
                hashCode = (hashCode * 397) ^ SymbolType.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Symbol left, Symbol right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Symbol left, Symbol right)
        {
            return !Equals(left, right);
        }
    }
}