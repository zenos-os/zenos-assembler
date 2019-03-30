namespace Zenos.Linker
{
    /// <summary>
    /// SymbolType
    /// </summary>
    public enum SymbolType
    {
        NotSpecified = 0,
        Object = 1,
        Function = 2,
        Section = 3,
        File = 4,
        Common = 5,
        TLS = 6,
    }
}