namespace Zenos.Linker
{
    /// <summary>
    /// Machine Type
    /// </summary>
    public enum MachineType : ushort
    {
        /// <summary>
        /// No machine
        /// </summary>
        NoMachine = 0x0,

        /// <summary>
        /// Intel Architecture
        /// </summary>
        Intel386 = 0x3,

        /// <summary>
        /// Advanced RISC Machines ARM
        /// </summary>
        ARM = 40,

        /// <summary>
        /// Intel IA-64 processor architecture
        /// </summary>
        IA_64 = 0x32,

        /// <summary>
        /// x86_64 processor architecture
        /// </summary>
        X86_64 = 0x3E
    }
}