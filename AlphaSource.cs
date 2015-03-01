
namespace PngtoFshBatchtxt
{
    internal enum AlphaSource
    {
        /// <summary>
        /// The alpha map is from a separate file.
        /// </summary>
        File,
        /// <summary>
        /// The alpha map is from the image transparency.
        /// </summary>
        Transparency,
        /// <summary>
        /// The alpha map is generated for an image without transparency.
        /// </summary>
        Generated
    }
}