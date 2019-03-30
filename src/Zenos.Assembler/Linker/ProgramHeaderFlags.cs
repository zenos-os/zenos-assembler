using System;

namespace Zenos.Linker
{
    /// <summary>
    /// Program Header Flags
    /// </summary>
    [Flags]
    public enum ProgramHeaderFlags : uint
    {
        Execute = 0x1,
        Write = 0x2,
        Read = 0x4,
        MaskProc = 0xF0000000,
    }
}