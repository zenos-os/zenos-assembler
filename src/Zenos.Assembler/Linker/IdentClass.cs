﻿namespace Zenos.Linker
{
    /// <summary>
    /// Identifies the file's class, or capacity.
    /// </summary>
    public enum IdentClass : byte
    {
        /// <summary>
        /// Invalid class
        /// </summary>
        ClassNone = 0x00,

        /// <summary>
        /// 32-bit objects
        /// </summary>
        Class32 = 0x01,

        /// <summary>
        /// 64-bit objects
        /// </summary>
        Class64 = 0x02,
    }
}