using FshDatIO;
using Microsoft.WindowsAPICodePack.Taskbar;
using PngtoFshBatchtxt.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PngtoFshBatchtxt
{
    internal partial class Form1 : Form
    {
        private TaskbarManager manager;
        private JumpList jumpList;
        private Settings settings;
        private char endreg;
        private char end64;
        private char end32;
        private char end16;
        private char end8;
        private bool batchProcessed;
        private bool mipsBuilt;
        private bool datRebuilt;
        private Random random;
        private Nullable<long> lowerInstRange;
        private Nullable<long> upperInstRange;
        private MipmapFormat mipFormat;
        private bool listControlsEnabled;
        private bool existingDat;

        private readonly string groupPath;
        private readonly string rangePath;
        private const string DefaultGroupId = "1ABE787D";
        private static Regex hexadecimalRegex;

        internal BatchFshCollection batchFshList;
        internal string outputFolder;
        internal DatFile dat;
        internal bool displayProgress;
        internal string groupId;

        internal const string AlphaMapSuffix = "_a";

        public Form1()
        {
            InitializeComponent();

            this.displayProgress = true;
            this.batchFshList = null;
            this.groupId = null;

            this.groupPath = Path.Combine(Application.StartupPath, @"Groupid.txt");
            this.rangePath = Path.Combine(Application.StartupPath, @"instRange.txt");

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (TaskbarManager.IsPlatformSupported)
                {
                    this.manager = TaskbarManager.Instance;
                    this.manager.ApplicationId = "PngtoFshBatch";
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            UIUtil.ShowErrorMessage(this, message, this.Text);
        }

        internal void ProcessMips()
        {
            int count = batchFshList.Count;
            for (int n = 0; n < count; n++)
            {
                BatchFshContainer batchFsh = batchFshList[n];
                if (batchFsh.Mip64Fsh != null && batchFsh.Mip32Fsh != null && batchFsh.Mip16Fsh != null && batchFsh.Mip8Fsh != null)
                {
                    batchFsh.Mip64Fsh.Dispose();
                    batchFsh.Mip32Fsh.Dispose();
                    batchFsh.Mip16Fsh.Dispose();
                    batchFsh.Mip8Fsh.Dispose();

                    batchFsh.Mip64Fsh = null;
                    batchFsh.Mip32Fsh = null;
                    batchFsh.Mip16Fsh = null;
                    batchFsh.Mip8Fsh = null;
                }
            }

            Bitmap[] bmps = new Bitmap[4];
            Bitmap[] alphas = new Bitmap[4];

            for (int i = 0; i < count; i++)
            {
                if (displayProgress)
                {
                    SetProgressBarValue(i, Resources.ProcessingMipsStatusTextFormat);
                }
                BatchFshContainer batchFsh = batchFshList[i];

                char endChar = batchFsh.InstanceId[7];

                // Only items that end with 4, 9 or E can have mipmaps in separate files. 
                if (endChar == '4' || endChar == '9' || endChar == 'E')
                {                
                    BitmapEntry item = batchFsh.MainImage.Bitmaps[0];

                    if (item.Bitmap.Width >= 128 && item.Bitmap.Height >= 128)
                    {
                        // 0 = 8, 1 = 16, 2 = 32, 3 = 64
                        using (Bitmap bmp = new Bitmap(item.Bitmap))
                        {
                            bmps[0] = GetBitmapThumbnail(bmp, 8, 8);
                            bmps[1] = GetBitmapThumbnail(bmp, 16, 16);
                            bmps[2] = GetBitmapThumbnail(bmp, 32, 32);
                            bmps[3] = GetBitmapThumbnail(bmp, 64, 64);
                        }

                        using (Bitmap alpha = new Bitmap(item.Alpha))
                        {
                            alphas[0] = GetBitmapThumbnail(alpha, 8, 8);
                            alphas[1] = GetBitmapThumbnail(alpha, 16, 16);
                            alphas[2] = GetBitmapThumbnail(alpha, 32, 32);
                            alphas[3] = GetBitmapThumbnail(alpha, 64, 64);
                        }

                        string dirName = item.DirName;
                        FshImageFormat bmpType;
                        if (item.BmpType == FshImageFormat.DXT3 || item.BmpType == FshImageFormat.ThirtyTwoBit)
                        {
                            bmpType = FshImageFormat.DXT3;
                        }
                        else
                        {
                            bmpType = FshImageFormat.DXT1;
                        }

                        for (int j = 3; j >= 0; j--)
                        {
                            switch (j)
                            {
                                case 3:
                                    batchFsh.Mip64Fsh = new FSHImageWrapper();
                                    batchFsh.Mip64Fsh.Bitmaps.Add(new BitmapEntry(bmps[j], alphas[j], bmpType, dirName));
                                    break;
                                case 2:
                                    batchFsh.Mip32Fsh = new FSHImageWrapper();
                                    batchFsh.Mip32Fsh.Bitmaps.Add(new BitmapEntry(bmps[j], alphas[j], bmpType, dirName));
                                    break;
                                case 1:
                                    batchFsh.Mip16Fsh = new FSHImageWrapper();
                                    batchFsh.Mip16Fsh.Bitmaps.Add(new BitmapEntry(bmps[j], alphas[j], bmpType, dirName));
                                    break;
                                case 0:
                                    batchFsh.Mip8Fsh = new FSHImageWrapper();
                                    batchFsh.Mip8Fsh.Bitmaps.Add(new BitmapEntry(bmps[j], alphas[j], bmpType, dirName));
                                    break;
                            }

                            bmps[j].Dispose();
                            alphas[j].Dispose();
                            bmps[j] = null;
                            alphas[j] = null;
                        }

                    }
                }
            }

            mipsBuilt = true;
        }

        /// <summary>
        /// Creates the mipmap thumbnail
        /// </summary>
        /// <param name="source">The Bitmap to draw</param>
        /// <param name="width">The width of the new bitmap</param>
        /// <param name="height">The height of the new bitmap</param>
        /// <returns>The new scaled Bitmap</returns>
        private static Bitmap GetBitmapThumbnail(Bitmap source, int width, int height)
        {
            using (Bitmap temp = SuperSample.GetBitmapThumbnail(source, width, height))
            {
                return temp.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format24bppRgb);
            }
        }

        /// <summary>
        /// Sets the instance end chars.
        /// </summary>
        private void SetInstanceEndChars(string instance)
        {
            char endChar = instance[7];

            if (endChar == '4' || endChar == '3' || endChar == '2' || endChar == '1' || endChar == '0')
            {
                endreg = '4';
                end64 = '3';
                end32 = '2';
                end16 = '1';
                end8 = '0';
            }
            else if (endChar == '9' || endChar == '8' || endChar == '7' || endChar == '6' || endChar == '5')
            {
                endreg = '9';
                end64 = '8';
                end32 = '7';
                end16 = '6';
                end8 = '5';
            }
            else if (endChar == 'E' || endChar == 'D' || endChar == 'C' || endChar == 'B' || endChar == 'A')
            {
                endreg = 'E';
                end64 = 'D';
                end32 = 'C';
                end16 = 'B';
                end8 = 'A';
            }
        }

        private void WriteTgi(string filename, int zoom, BatchFshContainer batch)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(filename + ".TGI", FileMode.OpenOrCreate, FileAccess.Write);

                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs = null;
                    sw.WriteLine("7AB50E44\n");
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}\n", batch.GroupId));

                    string instance = batch.InstanceId;

                    if (zoom != 4)
                    {
                        SetInstanceEndChars(instance); 
                    }

                    switch (zoom)
                    {
                        case 0:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", instance.Substring(0, 7) + end8));
                            break;
                        case 1:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", instance.Substring(0, 7) + end16));
                            break;
                        case 2:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", instance.Substring(0, 7) + end32));
                            break;
                        case 3:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", instance.Substring(0, 7) + end64));
                            break;
                        case 4:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", instance));
                            break;
                    }
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }


        }

        private static string GetAlphaSourceString(string path)
        {
            string alname = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + AlphaMapSuffix + Path.GetExtension(path));
            if (File.Exists(alname))
            {
                return Path.GetFileName(alname);
            }
            else
            {
                using (Bitmap bmp = new Bitmap(path))
                {
                    if (Path.GetExtension(path).Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
                    {
                        return Resources.AlphaTransString;
                    }
                    else
                    {
                        return Resources.AlphaGenString;
                    }
                }
            }
        }

        private void batchListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // As current item in the ListView is deselected before selecting the new item, we use a timer to ensure that the only deselection event we receive is from the user.
            this.listIndexChangedTimer.Start();
        }

        private void listIndexChangedTimer_Tick(object sender, EventArgs e)
        {
            this.listIndexChangedTimer.Stop();
            BatchListSelectedIndexChanged();
        }

        private void BatchListSelectedIndexChanged()
        {
            if (batchListView.SelectedItems.Count > 0)
            {
                if (!listControlsEnabled)
                {
                    EnableListControls();
                }

                int index = batchListView.SelectedItems[0].Index;
                BatchFshContainer batch = batchFshList[index];

                string inst = batch.InstanceId;
                char endChar = inst[7];

                Size mainImageSize = batch.MainImageSize;
                if ((mainImageSize.Width >= 128 || mainImageSize.Height >= 128) && endChar == '0' || endChar == '5' || endChar == 'A')
                {
                    // If the main image is at least 128 pixels in width or height and the instance ends with 0, 5 or A do not change it by setting the radio buttons.
                    this.Inst0_4rdo.Checked = false;
                    this.Inst5_9rdo.Checked = false;
                    this.InstA_Erdo.Checked = false;
                }
                else if (endChar == '4' || endChar == '3' || endChar == '2' || endChar == '1' || endChar == '0')
                {
                    Inst0_4rdo.Checked = true;
                }
                else if (endChar == '9' || endChar == '8' || endChar == '7' || endChar == '6' || endChar == '5')
                {
                    Inst5_9rdo.Checked = true;
                }
                else if (endChar == 'E' || endChar == 'D' || endChar == 'C' || endChar == 'B' || endChar == 'A')
                {
                    InstA_Erdo.Checked = true;
                }

                tgiGroupTxt.Text = batch.GroupId;
                tgiInstanceTxt.Text = inst;

                switch (batch.Format)
                {
                    case FshImageFormat.TwentyFourBit:
                        fshTypeBox.SelectedIndex = 0;
                        break;
                    case FshImageFormat.ThirtyTwoBit:
                        fshTypeBox.SelectedIndex = 1;
                        break;
                    case FshImageFormat.DXT1:
                        fshTypeBox.SelectedIndex = 2;
                        break;
                    case FshImageFormat.DXT3:
                        fshTypeBox.SelectedIndex = 3;
                        break;
                }
            }
            else
            {
                if (listControlsEnabled)
                {
                    DisableListControls();
                }
            }
        }

        internal void RebuildDat()
        {
            if (mipsBuilt)
            {
                for (int i = 0; i < batchFshList.Count; i++)
                {
                    if (displayProgress)
                    {
                        SetProgressBarValue(i, Resources.BuildingDatStatusTextFormat);
                    }
                    BatchFshContainer batchFsh = batchFshList[i];
                    uint group = uint.Parse(batchFsh.GroupId, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                    uint[] instanceid = new uint[5];
                    FSHImageWrapper[] fshimg = new FSHImageWrapper[5];

                    if (batchFsh.Mip64Fsh != null && batchFsh.Mip32Fsh != null && batchFsh.Mip16Fsh != null && batchFsh.Mip8Fsh != null)
                    {
                        fshimg[0] = batchFsh.Mip8Fsh;
                        fshimg[1] = batchFsh.Mip16Fsh;
                        fshimg[2] = batchFsh.Mip32Fsh;
                        fshimg[3] = batchFsh.Mip64Fsh;

                        SetInstanceEndChars(batchFsh.InstanceId);

                        string sub = batchFsh.InstanceId.Substring(0, 7);
                        instanceid[0] = uint.Parse(sub + end8, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                        instanceid[1] = uint.Parse(sub + end16, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                        instanceid[2] = uint.Parse(sub + end32, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                        instanceid[3] = uint.Parse(sub + end64, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                        instanceid[4] = uint.Parse(sub + endreg, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                    }
                    else
                    {
                        instanceid[4] = uint.Parse(batchFsh.InstanceId, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                    }

                    fshimg[4] = batchFsh.MainImage;

 
                    bool useFshWrite = this.fshWriteCompCb.Checked;
                    bool compress = this.compDatCb.Checked;
                    for (int j = 4; j >= 0; j--)
                    {
                        if (fshimg[j] != null)
                        {
                            if (this.existingDat)
                            {
                                this.dat.RemoveExistingFile(group, instanceid[j]);
                            }

                            this.dat.Add(new FshFileItem(fshimg[j], useFshWrite), group, instanceid[j], compress);
                        }
                    }
                }
            }
            else if (batchFshList != null)
            {
                for (int i = 0; i < batchFshList.Count; i++)
                {
                    if (displayProgress)
                    {
                        SetProgressBarValue(i, Resources.BuildingDatStatusTextFormat);
                    }
                    BatchFshContainer batchFsh = batchFshList[i];
                    uint group = uint.Parse(batchFsh.GroupId, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                    uint instanceID = uint.Parse(batchFsh.InstanceId, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);

                    FSHImageWrapper mainImage = batchFshList[i].MainImage;
                    if (mainImage != null)
                    {
                        if (mipFormat == MipmapFormat.Embedded)
                        {
                            mainImage.Bitmaps[0].CalculateMipmapCount();
                        }

                        if (this.existingDat)
                        {
                            this.dat.RemoveExistingFile(group, instanceID);
                        }

                        this.dat.Add(new FshFileItem(mainImage, this.fshWriteCompCb.Checked), group, instanceID, this.compDatCb.Checked);
                    }
                }
            }

            datRebuilt = true;
        }

        private void saveDatbtn_Click(object sender, EventArgs e)
        {
            if (batchListView.Items.Count > 0)
            {
                try
                {
                    if (saveDatDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        this.existingDat = false;
                        if (File.Exists(saveDatDialog1.FileName))
                        {
                            if (this.dat != null)
                            {
                                this.dat.Dispose();
                                this.dat = null;
                            }

                            this.dat = new DatFile(this.saveDatDialog1.FileName);
                            this.existingDat = true;
                        }
                        else if (this.dat == null)
                        {
                            this.dat = new DatFile();
                        }
                        DisableControlsForProcessing();

                        using (BackgroundWorker worker = new BackgroundWorker())
                        {
                            worker.DoWork += delegate(object s, DoWorkEventArgs args)
                            {
                                if (!this.batchProcessed)
                                {
                                    ProcessBatch();
                                }

                                if (this.mipFormat == MipmapFormat.Normal)
                                {
                                    ProcessMips();
                                }

                                if (!this.datRebuilt)
                                {
                                    RebuildDat();
                                }

                                Invoke(new Action(delegate()
                                {
                                    this.toolStripProgressStatus.Text = Resources.SavingDatStatusText;
                                    this.statusStrip1.Refresh();
                                }));

                                if (dat.Indexes.Count > 0)
                                {
                                    dat.Save(saveDatDialog1.FileName);
                                }
                            };

                            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                            {
                                if (args.Error != null)
                                {
                                    ShowErrorMessage(args.Error.Message);
                                }

                                dat.Close();
                                dat = null;
                                ClearandReset();
                                this.Cursor = Cursors.Default;
                            };


                            worker.RunWorkerAsync();
                        }

                    }
                }
                catch (DatFileException dfex)
                {
                    ShowErrorMessage(dfex.Message);
                }
                catch (DirectoryNotFoundException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (IOException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (SecurityException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
        }

        private void newDatbtn_Click(object sender, EventArgs e)
        {
            this.dat = new DatFile();
            this.toolStripProgressStatus.Text = Resources.DatInMemory;
        }
        /// <summary>
        /// Extracts the alpha channel bitmap from a transparent png
        /// </summary>
        /// <param name="source">The 32-bit input png</param>
        /// <returns>The extracted alpha channel bitmap</returns>
        private unsafe static Bitmap GetAlphaFromTransparency(Bitmap source)
        {
            Bitmap dest = null;
            Bitmap temp = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);

            try
            {
                Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

                BitmapData src = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData dst = temp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                byte* srcpxl = (byte*)src.Scan0.ToPointer();
                byte* dstpxl = (byte*)dst.Scan0.ToPointer();

                int srcofs = src.Stride - source.Width * 4;
                int dstofs = dst.Stride - temp.Width * 3;

                for (int y = 0; y < temp.Height; y++)
                {
                    for (int x = 0; x < temp.Width; x++)
                    {
                        dstpxl[0] = dstpxl[1] = dstpxl[2] = srcpxl[3];

                        srcpxl += 4;
                        dstpxl += 3;
                    }
                    srcpxl += srcofs;
                    dstpxl += dstofs;
                }

                temp.UnlockBits(dst);
                source.UnlockBits(src);

                dest = temp.Clone(rect, temp.PixelFormat);
            }
            finally
            {
                if (temp != null)
                {
                    temp.Dispose();
                    temp = null;
                }
            }

            return dest;
        }

        internal void ProcessBatch()
        {

            int count = batchFshList.Count;
            for (int i = 0; i < count; i++)
            {
                if (displayProgress)
                {
                    SetProgressBarValue(i, Resources.ProcessingStatusTextFormat);
                }
                BatchFshContainer batch = batchFshList[i];
                string fileName = batch.FileName;
                using (Bitmap temp = new Bitmap(fileName))
                {
                    using (BitmapEntry item = new BitmapEntry())
                    {
                        item.Bitmap = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), PixelFormat.Format24bppRgb);
                        string alname = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + AlphaMapSuffix + Path.GetExtension(fileName));
                        if (File.Exists(alname))
                        {
                            using (Bitmap alpha = new Bitmap(alname))
                            {
                                item.Alpha = alpha.Clone(new Rectangle(0, 0, alpha.Width, alpha.Height), PixelFormat.Format24bppRgb);
                            }
                        }
                        else if (Path.GetExtension(fileName).Equals(".png", StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            item.Alpha = GetAlphaFromTransparency(temp);
                        }
                        else
                        {
                            Bitmap alpha = new Bitmap(temp.Width, temp.Height, PixelFormat.Format24bppRgb);
                            try
                            {
                                BitmapData data = alpha.LockBits(new Rectangle(0, 0, alpha.Width, alpha.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                                try
                                {
                                    unsafe
                                    {
                                        byte* scan0 = (byte*)data.Scan0.ToPointer();
                                        int stride = data.Stride;
                                        for (int y = 0; y < data.Height; y++)
                                        {
                                            byte* p = scan0 + (y * stride);
                                            for (int x = 0; x < data.Width; x++)
                                            {
                                                p[0] = p[1] = p[2] = 255;
                                                p += 3;
                                            }
                                        }
                                    }

                                }
                                finally
                                {
                                    alpha.UnlockBits(data);
                                }

                                item.Alpha = alpha.Clone(new Rectangle(0, 0, alpha.Width, alpha.Height), alpha.PixelFormat);
                            }
                            finally
                            {
                                if (alpha != null)
                                {
                                    alpha.Dispose();
                                    alpha = null;
                                }
                            }
                        }
                        item.BmpType = batch.Format;
                        item.DirName = "FiSH";

                        batch.MainImage = new FSHImageWrapper();
                        batch.MainImage.Bitmaps.Add(item.Clone());
                    }
                }

            }
            batchProcessed = true;
        }

        private static string GetFilePath(string filePath, string addToPath, string outDir)
        {
            if (!string.IsNullOrEmpty(outDir))
            {
                return Path.Combine(outDir, Path.GetFileNameWithoutExtension(filePath) + addToPath + ".fsh");
            }
            else
            {
                return Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + addToPath + ".fsh");
            }
        }

        /// <summary>
        /// Saves a fsh using either FshWrite or FSHLib
        /// </summary>
        /// <param name="fs">The stream to save to</param>
        /// <param name="temp">The temp to save</param>
        private void SaveFsh(Stream fs, FSHImageWrapper image)
        {
            try
            {
                image.Save(fs, fshWriteCompCb.Checked);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetProgressBarMaximum()
        {
            if (this.mipFormat == MipmapFormat.Normal)
            {
                toolStripProgressBar1.Maximum = (this.batchListView.Items.Count * 3);
            }
            else
            {
                toolStripProgressBar1.Maximum = (this.batchListView.Items.Count * 2);
            }

            if (manager != null)
            {
                manager.SetProgressState(TaskbarProgressBarState.Normal);
            }
        }

        private void SetProgressBarValue(int value, string statusTextFormat)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(delegate()
                    {
                        this.toolStripProgressBar1.PerformStep();
                        this.toolStripProgressStatus.Text = string.Format(CultureInfo.CurrentCulture, statusTextFormat, (value + 1), this.batchListView.Items.Count);

                        if (this.manager != null)
                        {
                            this.manager.SetProgressValue(this.toolStripProgressBar1.Value, this.toolStripProgressBar1.Maximum, this.Handle);
                        }
                    }));

            }
            else
            {
                this.toolStripProgressBar1.PerformStep();
                this.toolStripProgressStatus.Text = string.Format(CultureInfo.CurrentCulture, statusTextFormat, (value + 1), this.batchListView.Items.Count);

                if (this.manager != null)
                {
                    this.manager.SetProgressValue(this.toolStripProgressBar1.Value, this.toolStripProgressBar1.Maximum, this.Handle);
                }
            }
        }

        internal void ProcessBatchSaveFiles()
        {
            for (int i = 0; i < batchFshList.Count; i++)
            {
                if (displayProgress)
                {
                    SetProgressBarValue(i, Resources.SavingFshProgressTextFormat);
                }
                BatchFshContainer batchFsh = batchFshList[i];
                string fileName = batchFsh.FileName;
                string filepath;

                if (batchFsh.MainImage != null)
                {
                    if (this.mipFormat == MipmapFormat.Embedded)
                    {
                        batchFsh.MainImage.Bitmaps[0].CalculateMipmapCount();
                    }

                    filepath = GetFilePath(fileName, string.Empty, outputFolder);
                    using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        SaveFsh(fstream, batchFsh.MainImage);
                    }
                    WriteTgi(filepath, 4, batchFsh);
                }
                if (this.mipFormat == MipmapFormat.Normal)
                {
                    if (batchFsh.Mip64Fsh != null)
                    {
                        filepath = GetFilePath(fileName, "_s3", outputFolder);
                        using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            SaveFsh(fstream, batchFsh.Mip64Fsh);
                        }
                        WriteTgi(filepath, 3, batchFsh);
                    }
                    if (batchFsh.Mip32Fsh != null)
                    {
                        filepath = GetFilePath(fileName, "_s2", outputFolder);
                        using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            SaveFsh(fstream, batchFsh.Mip32Fsh);
                        }
                        WriteTgi(filepath, 2, batchFsh);
                    }
                    if (batchFsh.Mip16Fsh != null)
                    {
                        filepath = GetFilePath(fileName, "_s1", outputFolder);
                        using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            SaveFsh(fstream, batchFsh.Mip16Fsh);
                        }
                        WriteTgi(filepath, 1, batchFsh);
                    }
                    if (batchFsh.Mip8Fsh != null)
                    {
                        filepath = GetFilePath(fileName, "_s0", outputFolder);
                        using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            SaveFsh(fstream, batchFsh.Mip8Fsh);
                        }
                        WriteTgi(filepath, 0, batchFsh);
                    }
                }
            }
        }

        private void processbatchbtn_Click(object sender, EventArgs e)
        {
            if (this.batchListView.Items.Count == 0)
            {
                return;
            }

            SetProgressBarMaximum();
            this.Cursor = Cursors.WaitCursor;
            DisableControlsForProcessing();

            using (BackgroundWorker worker = new BackgroundWorker())
            {
                worker.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    if (!this.batchProcessed)
                    {
                        ProcessBatch();
                    }

                    if (this.mipFormat == MipmapFormat.Normal)
                    {
                        ProcessMips();
                    }

                    ProcessBatchSaveFiles();
                };

                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    if (args.Error != null)
                    {
                        ShowErrorMessage(args.Error.Message);
                    }

                    ClearandReset();
                    this.Cursor = Cursors.Default;
                };

                worker.RunWorkerAsync();
            }
        }

        private void LoadSettings()
        {
            this.settings = new Settings(Path.Combine(Application.StartupPath, @"PngtoFshBatch.xml"));

            bool value;
            if (bool.TryParse(settings.GetSetting("compDatcb_checked", bool.TrueString), out value))
            {
                this.compDatCb.Checked = value;
            }

            if (bool.TryParse(settings.GetSetting("fshwritecompcb_checked", bool.FalseString), out value))
            {
                this.fshWriteCompCb.Checked = value;
            }
            this.mipFormatCbo.SelectedIndex = settings.GetSetting("MipFormat", 0);
        }

        private void CheckForSSE()
        {
            if (OS.IsMicrosoftWindows && !OS.HaveSSE)
            {
                UIUtil.ShowWarningMessage(this, Resources.FshWriteSSERequiredError, this.Text);
                fshWriteCompCb.Enabled = fshWriteCompCb.Checked = false;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            try
            {
                LoadSettings();
            }
            catch (FileNotFoundException fnfex)
            {
                ShowErrorMessage(fnfex.Message);
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (SecurityException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowErrorMessage(ex.Message);
            }

            CheckForSSE();
            
            if (this.groupId == null)
            {
                try
                { 
                    ReadGroupTxt();
                }
                catch (FileNotFoundException fnfex)
                {
                    ShowErrorMessage(fnfex.Message);
                }
                catch (IOException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (SecurityException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
            
            this.tgiGroupTxt.Text = this.groupId ?? DefaultGroupId;

            if (manager != null)
            {
                jumpList = JumpList.CreateJumpList();
            }

            if (batchFshList != null)
            {
                AddFilesToListView();
            }
        }

        private string RandomHexString(int length)
        {
            if (random == null)
            {
                random = new Random();
                ReadRangeTxt();
            }

            if (lowerInstRange.HasValue && upperInstRange.HasValue)
            {
                long lower = lowerInstRange.Value;
                long upper = upperInstRange.Value;

                double rn = (upper * 1.0 - lower * 1.0) * random.NextDouble() + lower * 1.0;

                return Convert.ToInt64(rn).ToString("X", CultureInfo.InvariantCulture).Substring(0, 7);
            }

            byte[] buffer = new byte[length / 2];
            random.NextBytes(buffer);
            string result = string.Concat(buffer.Select(x => x.ToString("X2", CultureInfo.InvariantCulture)).ToArray());
            if ((length % 2) == 0)
                return result;

            return result + random.Next(16).ToString("X", CultureInfo.InvariantCulture);
        }

        private void compDatcb_CheckedChanged(object sender, EventArgs e)
        {
            if (settings != null)
            {
                settings.PutSetting("compDatcb_checked", compDatCb.Checked.ToString());
            }
        }

        private void outfolderbtn_Click(object sender, EventArgs e)
        {
            if (OutputBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                outputFolder = OutputBrowserDialog1.SelectedPath;
            }
        }

        private void ReadGroupTxt()
        {
            using (StreamReader sr = new StreamReader(this.groupPath))
            {
                string line = sr.ReadLine();

                if (line != null)
                {
                    if (ValidateHexString(line))
                    {
                        this.groupId = line.ToUpperInvariant();
                    }
                    else
                    {
                        this.groupId = DefaultGroupId;
                        ShowErrorMessage(Resources.InvalidGroupID);
                    }
                }
                else
                {
                    this.groupId = DefaultGroupId;
                }
            }
        }

        private void ReadRangeTxt()
        {
            if (File.Exists(this.rangePath))
            {
                string[] instArray = null;
                using (StreamReader sr = new StreamReader(this.rangePath))
                {
                    string line;
                    char[] splitchar = new char[] { ',' };
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            instArray = line.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
                        }

                    }
                }

                if (instArray != null)
                {
                    if (instArray.Length != 2)
                    {
                        throw new FormatException(Resources.InvalidInstanceRange);
                    }

                    string inst0 = instArray[0].Trim();
                    string inst1 = instArray[1].Trim();

                    if (!ValidateHexString(inst0))
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidInstanceIdFormat, inst0));
                    }
                    if (!ValidateHexString(inst1))
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidInstanceIdFormat, inst1));
                    }

                    string lowerRange;
                    string upperRange;
                    if (inst0.Length == 10 && inst0.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        lowerRange = inst0.Substring(2, 8);
                    }
                    else
                    {
                        lowerRange = inst0;
                    }

                    if (inst1.Length == 10 && inst1.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        upperRange = inst1.Substring(2, 8);
                    }
                    else
                    {
                        upperRange = inst1;
                    }

                    long lower, upper;

                    if (long.TryParse(lowerRange, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lower) &&
                        long.TryParse(upperRange, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out upper))
                    {
                        if (lower >= upper)
                        {
                            throw new FormatException(Resources.InvalidInstanceRange);
                        }

                        lowerInstRange = lower;
                        upperInstRange = upper;
                    }
                }

            }
            else
            {
                lowerInstRange = null;
                upperInstRange = null;
            }
        }

        internal static bool ValidateHexString(string str)
        {
            if (str != null)
            {
                if (str.Length == 8 || str.Length == 10)
                {
                    if (hexadecimalRegex == null)
                    {
                        hexadecimalRegex = new Regex("^(0x|0X)?[a-fA-F0-9]+$", RegexOptions.CultureInvariant);
                    }

                    return hexadecimalRegex.IsMatch(str);
                }
            }

            return false;
        }

        private void AddRecentFolder(string path)
        {
            if (jumpList != null)
            {
                using (JumpListLink link = new JumpListLink(Assembly.GetExecutingAssembly().Location, Path.GetFileName(path)))
                {
                    link.Arguments = "\"" + path + "\"";
                    link.IconReference = OS.FolderIconReference;

                    JumpListHelper.AddToRecent(link);
                }

                jumpList.Refresh();
            }
        }

        private void SetEndFormat(Size mainImageSize, int index)
        {
            if (Inst0_4rdo.Checked)
            {
                endreg = '4';
                end64 = '3';
                end32 = '2';
                end16 = '1';
                end8 = '0';
            }
            else if (Inst5_9rdo.Checked)
            {
                endreg = '9';
                end64 = '8';
                end32 = '7';
                end16 = '6';
                end8 = '5';
            }
            else
            {
                endreg = 'E';
                end64 = 'D';
                end32 = 'C';
                end16 = 'B';
                end8 = 'A';
            }

            string instSub = batchFshList[index].InstanceId.Substring(0, 7);
            if (mainImageSize.Width >= 128 && mainImageSize.Height >= 128)
            {
                batchFshList[index].InstanceId = instSub + endreg;
            }
            else if (mainImageSize.Width == 64 && mainImageSize.Height == 64)
            {
                batchFshList[index].InstanceId = instSub + end64;
            }
            else if (mainImageSize.Width == 32 && mainImageSize.Height == 32)
            {
                batchFshList[index].InstanceId = instSub + end32;
            }
            else if (mainImageSize.Width == 16 && mainImageSize.Height == 16)
            {
                batchFshList[index].InstanceId = instSub + end16;
            }
            else if (mainImageSize.Width == 8 && mainImageSize.Height == 8)
            {
                batchFshList[index].InstanceId = instSub + end8;
            }
        }

        private void FormatRefresh(int index, bool formatRadiosChanged)
        {            
            BatchFshContainer batchFsh = batchFshList[index];
            string instance = batchFsh.InstanceId;
            char endChar = instance[7];

            Size mainImageSize = batchFsh.MainImageSize;
            if (((mainImageSize.Width >= 128 && mainImageSize.Height >= 128) && endChar == '4' || endChar == '9' || endChar == 'E') || formatRadiosChanged)
            {
                SetEndFormat(mainImageSize, index);
            }
           
            batchListView.SelectedItems[0].SubItems[3].Text = instance;
            tgiInstanceTxt.Text = instance;
        }

        private void Format_radios_changed(object sender, EventArgs e)
        {
            if (batchListView.SelectedItems.Count > 0)
            {
                FormatRefresh(batchListView.SelectedItems[0].Index, true);
            }
        }

        private void remBtn_Click(object sender, EventArgs e)
        {
            if (batchListView.SelectedItems.Count > 0 && batchListView.Items.Count > 1)
            {
                int index = batchListView.SelectedItems[0].Index;

                batchFshList.RemoveAt(index);

                batchListView.Items.RemoveAt(index);
                batchListView.Items[0].Selected = true;

                batchListView.Refresh();
            }
            else if (batchListView.SelectedItems.Count > 0 && batchListView.Items.Count == 1)
            {
                ClearandReset();
            }
        }

        /// <summary>
        /// Sets the group and instance ids when using command line processing.
        /// </summary>
        /// <exception cref="System.FormatException">The filename is not a valid instance id.</exception>
        internal void SetGroupAndInstanceIds()
        {
            ReadGroupTxt();

            for (int i = 0; i < batchFshList.Count; i++)
            {
                BatchFshContainer batch = batchFshList[i];
                if (string.IsNullOrEmpty(batch.GroupId))
                {
                    batch.GroupId = this.groupId;
                }

                if (string.IsNullOrEmpty(batch.InstanceId))
                {
                    string fileName = Path.GetFileNameWithoutExtension(batch.FileName);

                    if (fileName.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        string trimmed = null;

                        // If the filename is longer than a normal instance file name we only check the first 10 characters, as there may be a suffix (e.g. _blend). 
                        if (fileName.Length > 10)
                        {
                            trimmed = fileName.Substring(0, 10);
                        }

                        if (!ValidateHexString(trimmed ?? fileName))
                        {
                            throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidInstanceFileNameFormat, fileName));
                        }

                        batch.InstanceId = fileName.Substring(2, 8).ToUpperInvariant();
                    }
                    else
                    {
                        batch.InstanceId = RandomHexString(7);
                    }
                }
            }
        }

        private void AddFilesToListView()
        {
            AddFilesToListView(0);
        }

        private void AddFilesToListView(int startIndex)
        {
            try
            {
                int count = batchFshList.Count;

                for (int i = 0; i < count; i++)
                {
                    batchFshList[i].GroupId = this.groupId;
                }

                for (int n = startIndex; n < count; n++)
                {
                    BatchFshContainer batch = batchFshList[n];
                    string path = batch.FileName;
                    using (Bitmap temp = new Bitmap(path))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(path);

                        if (fileName.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            string trimmed = null;

                            // If the filename is longer than a normal instance file name we only check the first 10 characters, as there may be a suffix (e.g. _blend). 
                            if (fileName.Length > 10)
                            {
                                trimmed = fileName.Substring(0, 10);
                            }

                            if (!ValidateHexString(trimmed ?? fileName))
                            {
                                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidInstanceFileNameFormat, fileName));
                            }

                            batch.InstanceId = fileName.Substring(2, 8).ToUpperInvariant();
                        }
                        else
                        {
                            batch.InstanceId = RandomHexString(7);
                            SetEndFormat(temp.Size, n);
                        }
                        batch.MainImageSize = temp.Size;

                        string alphaName = Path.Combine(Path.GetDirectoryName(path), fileName + AlphaMapSuffix + Path.GetExtension(path));
                        if (File.Exists(alphaName) ||
                            Path.GetExtension(path).Equals(".png", StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                            {
                                batch.Format = FshImageFormat.ThirtyTwoBit;
                            }
                            else
                            {
                                batch.Format = FshImageFormat.DXT3;
                            }
                        }
                        else
                        {
                            if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
                            {
                                batch.Format = FshImageFormat.TwentyFourBit;
                            }
                            else
                            {
                                batch.Format = FshImageFormat.DXT1;
                            }
                        }
                    }
                }

                for (int i = startIndex; i < count; i++)
                {
                    BatchFshContainer batch = batchFshList[i];
                    string path = batch.FileName;

                    ListViewItem item = new ListViewItem(Path.GetFileName(path));
                    item.SubItems.Add(GetAlphaSourceString(path));
                    item.SubItems.Add(batch.GroupId);
                    item.SubItems.Add(batch.InstanceId);
                    batchListView.Items.Insert(i, item);
                }
                SetProcessingControlsEnabled(true);
            }
            catch (FileNotFoundException fx)
            {
                ShowErrorMessage(fx.Message);
            }
            catch (FormatException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (IndexOutOfRangeException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (SecurityException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            if (addFilesDialog.ShowDialog() == DialogResult.OK)
            {
                // remove the alpha mask images from the file
                string[] files = addFilesDialog.FileNames.Where(f => !Path.GetFileName(f).Contains(AlphaMapSuffix, StringComparison.OrdinalIgnoreCase)).ToArray();

                int fileCount = files.Length;
                if (fileCount > 0)
                {
                    int totalCount, existingFileCount;

                    if (batchFshList == null)
                    {
                        batchFshList = new BatchFshCollection(fileCount);
                    }

                    if (batchFshList.Count > 0)
                    {
                        existingFileCount = batchFshList.Count;
                        totalCount = fileCount + existingFileCount;
                    }
                    else
                    {
                        existingFileCount = 0;
                        totalCount = fileCount;
                    }

                    batchFshList.SetCapacity(totalCount);

                    foreach (var item in files)
                    {
                        batchFshList.Add(new BatchFshContainer(item));
                    }


                    int startIndex = 0;
                    if (existingFileCount != 0)
                    {
                        startIndex = totalCount - fileCount;
                    }

                    AddFilesToListView(startIndex);
                }
            }

        }

        /// <summary>
        /// Determines whether the specified key is a hexadecimal character.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="shiftPressed">Set to <c>true</c> when the shift key is pressed.</param>
        /// <returns><c>true</c> if the specified key is a valid hexadecimal character; otherwise <c>false</c></returns>
        private static bool IsHexadecimalChar(Keys key, bool shiftPressed)
        {
            bool result = false;
            if (!shiftPressed && (key >= Keys.D0 && key <= Keys.D9 || key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                result = true;
            }
            else
            {
                switch (key)
                {
                    case Keys.A:
                    case Keys.B:
                    case Keys.C:
                    case Keys.D:
                    case Keys.E:
                    case Keys.F:
                        result = true;
                        break;
                }
            }

            return result;
        }

        private void tgiGroupTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsHexadecimalChar(e.KeyCode, e.Shift) || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.Handled = true;
                e.SuppressKeyPress = false;
            }
            else
            {
                e.Handled = false;
                e.SuppressKeyPress = true;
            }
        }

        private void tgiGroupTxt_TextChanged(object sender, EventArgs e)
        {
            if (tgiGroupTxt.Text.Length == 8)
            {
                if (batchListView.SelectedItems.Count > 0)
                {
                    int index = batchListView.SelectedItems[0].Index;

                    string group = tgiGroupTxt.Text;
                    if (!group.Equals(batchFshList[index].GroupId, StringComparison.Ordinal))
                    {
                        batchFshList[index].GroupId = group;
                        batchListView.SelectedItems[0].SubItems[2].Text = group;
                    }
                }
                else
                {
                    this.groupId = tgiGroupTxt.Text;
                }
            }
        }

        private void tgiInstanceTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsHexadecimalChar(e.KeyCode, e.Shift) || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.Handled = true;
                e.SuppressKeyPress = false;
            }
            else
            {
                e.Handled = false;
                e.SuppressKeyPress = true;
            }
        }

        private void tgiInstanceTxt_TextChanged(object sender, EventArgs e)
        {
            if (batchListView.SelectedItems.Count > 0)
            {
                if (tgiInstanceTxt.Text.Length == 8)
                {
                    int index = batchListView.SelectedItems[0].Index;

                    string instance = tgiInstanceTxt.Text;
                    if (!instance.Equals(batchFshList[index].InstanceId, StringComparison.Ordinal))
                    {
                        batchFshList[index].InstanceId = instance;
                        FormatRefresh(index, false);
                    }
                }
            }
        }

        private void fshTypeBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (batchListView.SelectedItems.Count > 0)
            {
                FshImageFormat selectedFormat = FshImageFormat.DXT1;
                switch (fshTypeBox.SelectedIndex)
                {
                    case 0:
                        selectedFormat = FshImageFormat.TwentyFourBit;
                        break;
                    case 1:
                        selectedFormat = FshImageFormat.ThirtyTwoBit;
                        break;
                    case 2:
                        selectedFormat = FshImageFormat.DXT1;
                        break;
                    case 3:
                        selectedFormat = FshImageFormat.DXT3;
                        break;
                }

                int index = batchListView.SelectedItems[0].Index;

                if (batchFshList[index].Format != selectedFormat)
                {
                    batchFshList[index].Format = selectedFormat;
                }
            }
        }

        private void batchListView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private string[] GetFilesfromDirectory(string[] filenames)
        {
            List<string> fileList = new List<string>();

            foreach (var file in filenames)
            {
                if ((File.GetAttributes(file) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DirectoryInfo di = new DirectoryInfo(file);
                    List<FileInfo> infos = new List<FileInfo>();

                    infos.AddRange(di.GetFiles("*.png", SearchOption.TopDirectoryOnly));
                    infos.AddRange(di.GetFiles("*.bmp", SearchOption.TopDirectoryOnly));

                    for (int i = 0; i < infos.Count; i++)
                    {
                        FileInfo fi = infos[i];

                        if (!fi.Name.Contains(AlphaMapSuffix, StringComparison.OrdinalIgnoreCase))
                        {
                            fileList.Add(fi.FullName);
                        }
                    }

                    if (fileList.Count > 0)
                    {
                        AddRecentFolder(di.FullName);
                    }
                }
                else
                {
                    string ext = Path.GetExtension(file);
                    if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!Path.GetFileName(file).Contains(AlphaMapSuffix, StringComparison.OrdinalIgnoreCase))
                        {
                            fileList.Add(file);
                        }
                    }
                }
            }

            return fileList.ToArray();
        }

        private void batchListView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = GetFilesfromDirectory(e.Data.GetData(DataFormats.FileDrop) as string[]);

            if (batchListView.Items.Count > 0)
            {
                ClearandReset();
            }
            int fileCount = files.Length;

            if (fileCount > 0)
            {
                batchFshList = new BatchFshCollection(fileCount);

                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        batchFshList.Add(new BatchFshContainer(file));
                    }
                }
                AddFilesToListView();
            }
        }

        private void addBtn_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void ClearandReset()
        {
            this.batchListView.Items.Clear();
            SetProcessingControlsEnabled(false);
            DisableListControls();

            if (batchFshList != null)
            {
                this.batchFshList.Dispose();
                this.batchFshList = null;
            }

            this.outputFolder = null;
            this.mipsBuilt = false;
            this.batchProcessed = false;
            this.datRebuilt = false;
            this.mipFormatCbo.Enabled = true;
            this.compDatCb.Enabled = true;
            this.fshWriteCompCb.Enabled = true;
            this.tgiGroupTxt.Enabled = true;
            this.addBtn.Enabled = true;

            this.toolStripProgressBar1.Value = 0;
            this.toolStripProgressStatus.Text = Resources.StatusReadyText;
            if (manager != null)
            {
                this.manager.SetProgressState(TaskbarProgressBarState.NoProgress, this.Handle);
            }
        }

        private void addBtn_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = GetFilesfromDirectory(e.Data.GetData(DataFormats.FileDrop) as string[]);
            int fileCount = files.Length;

            if (fileCount > 0)
            {
                int totalCount, existingFileCount;

                if (batchFshList == null)
                {
                    batchFshList = new BatchFshCollection(fileCount);
                }

                if (batchFshList.Count > 0)
                {
                    existingFileCount = batchFshList.Count;
                    totalCount = fileCount + existingFileCount;
                }
                else
                {
                    existingFileCount = 0;
                    totalCount = fileCount;
                }

                batchFshList.SetCapacity(totalCount);

                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        batchFshList.Add(new BatchFshContainer(file));
                    }
                }

                int startIndex = 0;
                if (existingFileCount != 0)
                {
                    startIndex = totalCount - fileCount;
                }

                AddFilesToListView(startIndex);
            }
        }

        private void clearlistbtn_Click(object sender, EventArgs e)
        {
            ClearandReset();
        }

        private void fshwritecompcb_CheckedChanged(object sender, EventArgs e)
        {
            if (settings != null)
            {
                settings.PutSetting("fshwritecompcb_checked", fshWriteCompCb.Checked.ToString());
            }
        }

        private void mipFormatCbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.mipFormat = (MipmapFormat)mipFormatCbo.SelectedIndex;

            if (settings != null)
            {
                settings.PutSetting("MipFormat", mipFormatCbo.SelectedIndex);
            }
        }

        private void DisableControlsForProcessing()
        {
            this.mipFormatCbo.Enabled = false;
            this.compDatCb.Enabled = false;
            this.fshWriteCompCb.Enabled = false;
            this.tgiGroupTxt.Enabled = false;
            this.tgiInstanceTxt.Enabled = false;
            this.fshTypeBox.Enabled = false;
            this.Inst0_4rdo.Enabled = false;
            this.Inst5_9rdo.Enabled = false;
            this.InstA_Erdo.Enabled = false;
            this.newDatbtn.Enabled = false;
            this.saveDatBtn.Enabled = false;
            this.addBtn.Enabled = false;
            this.remBtn.Enabled = false;
            this.clearListBtn.Enabled = false;
            this.outFolderBtn.Enabled = false;
            this.processBatchBtn.Enabled = false;
        }

        private void SetProcessingControlsEnabled(bool enabled)
        {
            this.newDatbtn.Enabled = enabled;
            this.saveDatBtn.Enabled = enabled;
            this.clearListBtn.Enabled = enabled;
            this.processBatchBtn.Enabled = enabled;
        }

        private void EnableListControls()
        {
            this.tgiInstanceTxt.Enabled = true;
            this.fshTypeBox.Enabled = true;
            this.Inst0_4rdo.Enabled = true;
            this.Inst5_9rdo.Enabled = true;
            this.InstA_Erdo.Enabled = true;
            this.remBtn.Enabled = true;

            this.listControlsEnabled = true;
        }

        private void DisableListControls()
        {
            this.tgiGroupTxt.Text = this.groupId;
            this.tgiInstanceTxt.Text = string.Empty;
            this.fshTypeBox.SelectedIndex = -1;
            this.Inst0_4rdo.Checked = false;
            this.Inst5_9rdo.Checked = false;
            this.InstA_Erdo.Checked = false;

            this.tgiInstanceTxt.Enabled = false;
            this.fshTypeBox.Enabled = false;
            this.Inst0_4rdo.Enabled = false;
            this.Inst5_9rdo.Enabled = false;
            this.InstA_Erdo.Enabled = false;
            this.remBtn.Enabled = false;

            this.listControlsEnabled = false;
        }
    }
}
