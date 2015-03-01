using FshDatIO;
using System;
using System.ComponentModel;
using System.Drawing;

namespace PngtoFshBatchtxt
{
    internal sealed class BatchFshContainer : IDisposable
    {
        private readonly string fileName;
        private string groupId;
        private string instanceId;
        private FshImageFormat format;
        private FSHImageWrapper mainImage;
        private FSHImageWrapper mip64Fsh;
        private FSHImageWrapper mip32Fsh;
        private FSHImageWrapper mip16Fsh;
        private FSHImageWrapper mip8Fsh;
        private bool disposed;
        private Size mainImageSize;
        private AlphaSource alphaSource;

        /// <summary>
        /// Creates a new BatchFshContainer
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        public BatchFshContainer(string fileName)
        {
            this.fileName = fileName;
            this.groupId = null;
            this.instanceId = null;
            this.format = FshImageFormat.DXT1;
            this.mainImage = null;
            this.mip64Fsh = null;
            this.mip32Fsh = null;
            this.mip16Fsh = null;
            this.mip8Fsh = null;
            this.disposed = false;
            this.mainImageSize = Size.Empty;
            this.alphaSource = AlphaSource.File;
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        public string GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "value is null.");

                groupId = value;
            }
        }

        public string InstanceId
        {
            get
            {
                return instanceId;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "value is null.");

                instanceId = value;
            }
        }

        public FshImageFormat Format
        {
            get
            {
                return format;
            }
            set
            {
                format = value;
            }
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

        public Size MainImageSize
        {
            get
            {
                return mainImageSize;
            }
            set
            {
                mainImageSize = value;
            }
        }

        public AlphaSource AlphaSource
        {
            get
            {
                return alphaSource;
            }
            set
            {
                if (value < AlphaSource.File || value > AlphaSource.Generated)
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(AlphaSource));
                }

                alphaSource = value;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.disposed = true;

                if (disposing)
                {
                    if (mainImage != null)
                    {
                        mainImage.Dispose();
                        mainImage = null;
                    }

                    if (mip64Fsh != null)
                    {
                        mip64Fsh.Dispose();
                        mip64Fsh = null;
                    }

                    if (mip32Fsh != null)
                    {      
                        mip32Fsh.Dispose();
                        mip32Fsh = null;
                    }

                    if (mip16Fsh != null)
                    {      
                        mip16Fsh.Dispose();
                        mip16Fsh = null;
                    }

                    if (mip8Fsh != null)
                    {      
                        mip8Fsh.Dispose();
                        mip8Fsh = null;
                    }
                }
            }
        }
    }
}
