using System;
using FshDatIO;

namespace PngtoFshBatchtxt
{
    sealed class BatchFshContainer
    {
        private FSHImageWrapper mainImage;
        private FSHImageWrapper mip64Fsh;
        private FSHImageWrapper mip32Fsh;
        private FSHImageWrapper mip16Fsh;
        private FSHImageWrapper mip8Fsh;

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
        public BatchFshContainer(FSHImageWrapper mainImage)
        {
            this.MainImage = mainImage;
        }


        public FSHImageWrapper MainImage
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

        public FSHImageWrapper Mip64Fsh
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

        public FSHImageWrapper Mip32Fsh
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

        public FSHImageWrapper Mip16Fsh
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

        public FSHImageWrapper Mip8Fsh
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
