namespace VisualBasic6X.Converter.Console.VisualBasic6
{
    /// <summary>
    /// VB6 project compilation types.
    /// </summary>
    public enum VB6ProjectType
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Standard Windows exe.
        /// </summary>
        Exe,

        /// <summary>
        /// ActiveX dll.
        /// </summary>
        OleDll,

        /// <summary>
        /// ActiveX exe.
        /// </summary>
        OleExe
    }
}