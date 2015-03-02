using FshDatIO;
using System.Drawing;

namespace PngtoFshBatchtxt
{
    internal sealed class BatchListItemTag
    {
        private readonly Size mainImageSize;
        private FshImageFormat format;


        public Size MainImageSize
        {
            get
            {
                return this.mainImageSize;
            }
        }

        public FshImageFormat Format
        {
            get
            {
                return this.format;
            }
            set
            {
                this.format = value;
            }
        }

        public BatchListItemTag(Size mainImageSize, FshImageFormat format)
        {
            this.mainImageSize = mainImageSize;
            this.format = format;
        }
    }
}
