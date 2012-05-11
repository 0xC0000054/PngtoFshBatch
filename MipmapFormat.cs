namespace PngtoFshBatchtxt
{
    /// <summary>
    /// The format of the mipmaps
    /// </summary>
    internal enum MipmapFormat
    {
        /// <summary>
        /// The mipmaps are in seperate files. 
        /// </summary>
        Normal,
        /// <summary>
        /// The mipmaps are after the main image (used by most automata).
        /// </summary>
        Embedded,
        /// <summary>
        /// No mipmaps are generated
        /// </summary>
        None
    }
}