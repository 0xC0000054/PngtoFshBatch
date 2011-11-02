using System;
using System.Collections.Generic;
using System.Text;
using FSHLib;

namespace PngtoFshBatchtxt
{
    class BatchFshContainer
    {
        private FSHImage mainImage;
        private FSHImage mip64Fsh;
        private FSHImage mip32Fsh;
        private FSHImage mip16Fsh;
        private FSHImage mip8Fsh;

        /// <summary>
        /// Creates a new BatchFshContainer
        /// </summary>
        public BatchFshContainer()
        {

        }
        /// <summary>
        /// Creates a new BatchFshContainer with the specified main image.
        /// </summary>
        /// <param name="mainImage">The image to add, this may be smaller than 128x128</param>
        public BatchFshContainer(FSHImage mainImage)
        {
            this.MainImage = mainImage;
        }


        public FSHImage MainImage
        {
            get 
            {
                return mainImage;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "value is null.");

                mainImage = value;
            }
        }

        public FSHImage Mip64Fsh
        {
            get
            {
                return mip64Fsh;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "value is null.");

                mip64Fsh = value;
            }
        }

        public FSHImage Mip32Fsh
        {
            get
            {
                return mip32Fsh;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "value is null.");

                mip32Fsh = value;
            }
        }

        public FSHImage Mip16Fsh
        {
            get
            {
                return mip16Fsh;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "value is null.");

                mip16Fsh = value;
            }
        }

        public FSHImage Mip8Fsh
        {
            get
            {
                return mip8Fsh;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "value is null.");

                mip8Fsh = value;
            }
        }

    }
}
