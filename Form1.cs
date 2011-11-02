using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using FshDatIO;
using FSHLib;
using PngtoFshBatchtxt.Properties;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Reflection;

namespace PngtoFshBatchtxt
{
	internal partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
            if (Type.GetType("Mono.Runtime") == null) // skip the Windows 7 code if we are on mono 
            {
                if (TaskbarManager.IsPlatformSupported)
                {
                    this.manager = TaskbarManager.Instance;
                    this.manager.ApplicationId = "PngtoFshBatch";
                } 
            }
		}
		internal List<BatchFshContainer> batchFshList = null;
		internal bool mipsbtn_clicked = false;

		internal void ProcessMips()
		{             
			for (int n = 0; n < batchListView.Items.Count; n++)
			{
				BatchFshContainer batchFsh = batchFshList[n];
				if (batchFsh.Mip64Fsh != null && batchFsh.Mip32Fsh != null && batchFsh.Mip16Fsh != null && batchFsh.Mip8Fsh != null)
				{
					batchFsh.Mip64Fsh = null;
					batchFsh.Mip32Fsh = null;
					batchFsh.Mip16Fsh = null;
					batchFsh.Mip8Fsh = null;
				}
			}
			
			for (int n = 0; n < batchListView.Items.Count; n++)
			{
				this.Invoke(new SetProgressBarValueDelegate(SetProgressBarValue), new object[] { n, Resources.ProcessingMipsStatusTextFormat });
				Bitmap[] bmps = new Bitmap[4];
				Bitmap[] alphas = new Bitmap[4];

				BitmapItem item = new BitmapItem();

				BatchFshContainer batchFsh = batchFshList[n];

				item = (BitmapItem)batchFsh.MainImage.Bitmaps[0];

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
					//alpha

					using (Bitmap alpha = new Bitmap(item.Alpha))
					{
						alphas[0] = GetBitmapThumbnail(alpha, 8, 8);
						alphas[1] = GetBitmapThumbnail(alpha, 16, 16);
						alphas[2] = GetBitmapThumbnail(alpha, 32, 32);
						alphas[3] = GetBitmapThumbnail(alpha, 64, 64);
					}

					for (int i = 0; i < 4; i++)
					{
						if (bmps[i] != null && alphas[i] != null)
						{
							BitmapItem mipitm = new BitmapItem();
							mipitm.Bitmap = bmps[i];
							mipitm.Alpha = alphas[i];
							mipitm.SetDirName(Encoding.ASCII.GetString(item.DirName));

							if (item.BmpType == FSHBmpType.DXT3 || item.BmpType == FSHBmpType.ThirtyTwoBit)
							{
								mipitm.BmpType = FSHBmpType.DXT3;
							}
							else
							{
								mipitm.BmpType = FSHBmpType.DXT1;
							}
							if (mipitm.Bitmap.Width == 64 && mipitm.Bitmap.Height == 64)
							{
								batchFsh.Mip64Fsh = new FSHImage();
								batchFsh.Mip64Fsh.Bitmaps.Add(mipitm);
								batchFsh.Mip64Fsh.UpdateDirty();
								using (MemoryStream mstream = new MemoryStream())
								{
									SaveFsh(mstream, batchFsh.Mip64Fsh);
                                    if (fshWriteCompCb.Checked)
                                    {
                                        batchFsh.Mip64Fsh.SetRawData(mstream.ToArray());
                                    }
                                }
							}
							else if (mipitm.Bitmap.Width == 32 && mipitm.Bitmap.Height == 32)
							{
								batchFsh.Mip32Fsh = new FSHImage();
								batchFsh.Mip32Fsh.Bitmaps.Add(mipitm);
								batchFsh.Mip32Fsh.UpdateDirty();
								using (MemoryStream mstream = new MemoryStream())
								{
									SaveFsh(mstream, batchFsh.Mip32Fsh);
                                    if (fshWriteCompCb.Checked)
                                    {
                                        batchFsh.Mip32Fsh.SetRawData(mstream.ToArray());
                                    }
                                }
							}
							else if (mipitm.Bitmap.Width == 16 && mipitm.Bitmap.Height == 16)
							{
								batchFsh.Mip16Fsh = new FSHImage();
								batchFsh.Mip16Fsh.Bitmaps.Add(mipitm);
								batchFsh.Mip16Fsh.UpdateDirty();
								using (MemoryStream mstream = new MemoryStream())
								{
									SaveFsh(mstream, batchFsh.Mip16Fsh);
                                    if (fshWriteCompCb.Checked)
                                    {
                                        batchFsh.Mip16Fsh.SetRawData(mstream.ToArray());
                                    }
                                }
							}
							else if (mipitm.Bitmap.Width == 8 && mipitm.Bitmap.Height == 8)
							{
								batchFsh.Mip8Fsh = new FSHImage();
								batchFsh.Mip8Fsh.Bitmaps.Add(mipitm);
								batchFsh.Mip8Fsh.UpdateDirty();
								using (MemoryStream mstream = new MemoryStream())
								{
									SaveFsh(mstream, batchFsh.Mip8Fsh);
                                    if (fshWriteCompCb.Checked)
                                    {
                                        batchFsh.Mip8Fsh.SetRawData(mstream.ToArray());
                                    }
                                }
							}
						}
					}
				}

			}

			mipsbtn_clicked = true;
		}
		/// <summary>
		/// Creates the mip thumbnail using Graphics.DrawImage
		/// </summary>
		/// <param name="source">The Bitmap to draw</param>
		/// <param name="width">The width of the new bitmap</param>
		/// <param name="height">The height of the new bitmap</param>
		/// <returns>The new scaled Bitmap</returns>
		private static Bitmap GetBitmapThumbnail(Bitmap source, int width, int height)
		{
#if DEBUG			
            Bitmap image = SuperSample.SuperSample.GetBitmapThumbnail(source, width, height);

			//string path = Path.Combine(Application.StartupPath,"thumb" + width.ToString() + ".Png");
			//temp.Save(path);
            
            return image;
#else
            return SuperSample.SuperSample.GetBitmapThumbnail(source, width, height);
#endif

        }

        /// <summary>
        /// Sets the instance end chars.
        /// </summary>
        /// <param name="index">The index in the instArray to compare to.</param>
        private void SetInstanceEndChars(int index)
        {
            if (instarray[index].EndsWith("4", StringComparison.Ordinal) || instarray[index].EndsWith("3", StringComparison.Ordinal)
                           || instarray[index].EndsWith("2", StringComparison.Ordinal) || instarray[index].EndsWith("1", StringComparison.Ordinal) || instarray[index].EndsWith("0", StringComparison.Ordinal))
            {
                endreg = '4';
                end64 = '3';
                end32 = '2';
                end16 = '1';
                end8 = '0';
            }
            else if (instarray[index].EndsWith("9", StringComparison.Ordinal) || instarray[index].EndsWith("8", StringComparison.Ordinal)
                || instarray[index].EndsWith("7", StringComparison.Ordinal) || instarray[index].EndsWith("6", StringComparison.Ordinal) || instarray[index].EndsWith("5", StringComparison.Ordinal))
            {
                endreg = '9';
                end64 = '8';
                end32 = '7';
                end16 = '6';
                end8 = '5';
            }
            else if (instarray[index].EndsWith("E", StringComparison.Ordinal) || instarray[index].EndsWith("D", StringComparison.Ordinal)
                || instarray[index].EndsWith("C", StringComparison.Ordinal) || instarray[index].EndsWith("B", StringComparison.Ordinal) || instarray[index].EndsWith("A", StringComparison.Ordinal))
            {
                endreg = 'E';
                end64 = 'D';
                end32 = 'C';
                end16 = 'B';
                end8 = 'A';
            }
        }

		delegate void WriteTgiDelegate(string fileName, int zoom, int cnt);
		private char endreg;
		private char end64;
		private char end32;
		private char end16;
		private char end8;
		private void WriteTgi(string filename, int zoom, int index)
		{
            FileStream fs = null;

            try
            {
                fs = new FileStream(filename + ".TGI", FileMode.OpenOrCreate, FileAccess.Write);

                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("7ab50e44\t\n");
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", batchListView.Items[index].SubItems[2].Text + "\n"));

                    SetInstanceEndChars(index);

                    switch (zoom)
                    {
                        case 0:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", batchListView.Items[index].SubItems[3].Text.Substring(0, 7) + end8));
                            break;
                        case 1:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", batchListView.Items[index].SubItems[3].Text.Substring(0, 7) + end16));
                            break;
                        case 2:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", batchListView.Items[index].SubItems[3].Text.Substring(0, 7) + end32));
                            break;
                        case 3:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", batchListView.Items[index].SubItems[3].Text.Substring(0, 7) + end64));
                            break;
                        case 4:
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", batchListView.Items[index].SubItems[3].Text));
                            break;
                    }
                }

                fs = null;
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

#if false
        private int CountBatchlines(string file)
        {
            int linecnt = 0;
            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "")
                    {
                        if (line.Equals("%Fsh_batch%") || line.StartsWith("#"))
                        {
                            continue;
                        }
                        linecnt++;
                    }
                }
            }
            return linecnt;
        } 
#endif

		internal List<string> patharray = null;
		internal List<string> instarray = null;
		internal List<string> typearray = null;
		internal List<string> grouparray = null;
		private static void Alphasrc(ListViewItem item, string path)
		{ 
			string alname = Path.Combine(Path.GetDirectoryName(path),Path.GetFileNameWithoutExtension(path) +"_a"+ Path.GetExtension(path));
			if (File.Exists(alname))
			{
				item.SubItems.Add(Path.GetFileName(alname));
			}
			else
			{
				using (Bitmap bmp = new Bitmap(path))
				{
					if (Path.GetExtension(path).Equals(".png", StringComparison.OrdinalIgnoreCase) && bmp.PixelFormat == PixelFormat.Format32bppArgb)
					{
						item.SubItems.Add(Resources.AlphaTransString);
					}
					else
					{
						item.SubItems.Add(Resources.AlphaGenString);
					}
				}
			}
		}

		private int typeindex = 0; // store previously selected fsh type
		private void batchListView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (batchListView.SelectedItems.Count > 0)
			{ 
				string inst = batchListView.SelectedItems[0].SubItems[3].Text;
				if (inst.EndsWith("4", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("3", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("2", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("1", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("0", StringComparison.OrdinalIgnoreCase))
				{
					Inst0_4rdo.Checked = true;
				}
				else if (inst.EndsWith("9", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("8", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("7", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("6", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("5", StringComparison.OrdinalIgnoreCase))
				{
					Inst5_9rdo.Checked = true;
				}
				else if (inst.EndsWith("E", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("D", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("C", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("B", StringComparison.OrdinalIgnoreCase) || inst.EndsWith("A", StringComparison.OrdinalIgnoreCase))
				{
					InstA_Erdo.Checked = true;
				}
				tgiGroupTxt.Text = batchListView.SelectedItems[0].SubItems[2].Text;
				tgiInstanceTxt.Text = batchListView.SelectedItems[0].SubItems[3].Text;
				int index = batchListView.SelectedItems[0].Index;
				switch (typearray[index].ToUpperInvariant())
				{ 
					case "TWENTYFOURBIT":
						fshTypeBox.SelectedIndex = 0;
						typeindex = 0;
						break;
					case "THIRTYTWOBIT":
						fshTypeBox.SelectedIndex = 1;
						typeindex = 1;
						break;
					case "DXT1":
						fshTypeBox.SelectedIndex = 2;
						typeindex = 2;
						break;
					case "DXT3":
						fshTypeBox.SelectedIndex = 3;
						typeindex = 3;
						break;
				}
			}
		}

		delegate ListViewItem GetBatchListViewItemDelegate(int index);

		private ListViewItem GetBatchListViewItem(int index)
		{
			return batchListView.Items[index];
		}

		internal DatFile dat = null;
		internal bool compress_datmips = false;
		private Thread datRebuildThread = null;
		private bool datRebuilt = false;


		internal void RebuildDat(DatFile inputdat)
		{
			if (mipsbtn_clicked)
			{
				for (int c = 0; c < batchListView.Items.Count; c++)
				{
					this.Invoke(new SetProgressBarValueDelegate(SetProgressBarValue), new object[] { c, Resources.BuildingDatStatusTextFormat });

                    BatchFshContainer batchFsh = batchFshList[c];
					ListViewItem item = (ListViewItem)this.Invoke(new GetBatchListViewItemDelegate(GetBatchListViewItem), new object[] {c});
					uint group = uint.Parse(item.SubItems[2].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					uint[] instanceid = new uint[5];
					FshWrapper[] fshwrap = new FshWrapper[5];
					FSHImage[] fshimg = new FSHImage[5];
					if (batchFsh.Mip64Fsh != null && batchFsh.Mip32Fsh != null && batchFsh.Mip16Fsh != null && batchFsh.Mip8Fsh != null)
					{
						fshimg[0] = batchFsh.Mip8Fsh;
						fshimg[1] = batchFsh.Mip16Fsh;
						fshimg[2] = batchFsh.Mip32Fsh;
						fshimg[3] = batchFsh.Mip64Fsh; 
					}
					
					fshimg[4] = batchFsh.MainImage;

                    SetInstanceEndChars(c);

                    instanceid[0] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end8, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    instanceid[1] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end16, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    instanceid[2] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end32, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    instanceid[3] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end64, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[4] = uint.Parse(item.SubItems[3].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					if (inputdat == null)
					{
						dat = new DatFile();
					}

					for (int i = 4; i >= 0; i--)
					{
						if (fshimg[i] != null)
						{
							fshwrap[i] = new FshWrapper(fshimg[i]) { UseFshWrite = fshWriteCompCb.Checked };

							inputdat.Add(fshwrap[i], group, instanceid[i], compress_datmips);
							// Debug.WriteLine("Bmp: " + index.ToString() + " zoom: " + i.ToString());
						}
					}
				}
			}
			else if (!autoProcMipsCb.Checked && batchFshList != null)
			{
				for (int c = 0; c < batchListView.Items.Count; c++)
				{
                    this.Invoke(new SetProgressBarValueDelegate(SetProgressBarValue), new object[] { c, Resources.BuildingDatStatusTextFormat });

					ListViewItem item = (ListViewItem)this.Invoke(new GetBatchListViewItemDelegate(GetBatchListViewItem), new object[] { c });
					
					uint Group = uint.Parse(item.SubItems[2].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					uint instanceid = new uint();
					FshWrapper fshwrap = new FshWrapper();
                    SetInstanceEndChars(c);
                    instanceid = uint.Parse(item.SubItems[3].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					if (inputdat == null)
					{
						dat = new DatFile();
					}

					if (batchFshList[c].MainImage != null)
					{
						fshwrap = new FshWrapper(batchFshList[c].MainImage) { UseFshWrite = fshWriteCompCb.Checked };

						inputdat.Add(fshwrap, Group, instanceid, compress_datmips);
					}
				}
			}

            this.BeginInvoke(new MethodInvoker(delegate()
            {                
                this.toolStripProgressStatus.Text = Resources.SavingDatStatusText;
            }));

			datRebuilt = true;
		}
		private void saveDatbtn_Click(object sender, EventArgs e)
		{
            if (batchListView.Items.Count > 0)
            {
                if (dat == null)
                {
                    dat = new DatFile();
                }

                try
                {

                    if (saveDatDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        if (!batch_processed)
                        {
                            this.SetProgressBarMaximum();
                            this.Cursor = Cursors.WaitCursor;
                            Application.DoEvents();
                            this.batchProcessThread = new Thread(new ThreadStart(ProcessBatch)) { Priority = ThreadPriority.AboveNormal, IsBackground = true };
                            this.batchProcessThread.Start();
                            while (batchProcessThread.IsAlive)
                            {
                                Application.DoEvents();
                            }
                            this.batchProcessThread.Join();
                        }
                        if (compDatcb.Checked && !compress_datmips)
                        {
                            compress_datmips = true;
                        }

                        if (autoProcMipsCb.Checked)
                        {
                            Application.DoEvents();
                            this.mipProcessThread = new Thread(new ThreadStart(ProcessMips)) { Priority = ThreadPriority.AboveNormal, IsBackground = true };
                            this.mipProcessThread.Start();
                            while (mipProcessThread.IsAlive)
                            {
                                Application.DoEvents();
                            }
                            this.mipProcessThread.Join();
                        }


                        if (!datRebuilt)
                        {
                            Application.DoEvents();
                            this.datRebuildThread = new Thread(() => RebuildDat(dat)) { Priority = ThreadPriority.AboveNormal, IsBackground = true };
                            this.datRebuildThread.Start();
                            while (datRebuildThread.IsAlive)
                            {
                                Application.DoEvents();
                            }
                            this.datRebuildThread.Join();

                        }


                        this.Cursor = Cursors.Default;
                        if (dat.Indexes.Count > 0)
                        {
                            try
                            {
                                dat.Save(saveDatDialog1.FileName);
                                dat.Close();
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            finally
                            {
                                ClearandReset();
                                dat = null;
                            }

                        }
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            }
		}

		private void newDatbtn_Click(object sender, EventArgs e)
		{
			this.dat = new DatFile();
			Datnametxt.Text = Resources.DatInMemory;
		}
		/// <summary>
		/// Extracts the alpha channel bitmap from a transparent png
		/// </summary>
		/// <param name="source">The 32-bit input png</param>
		/// <returns>The extreacted alpha channel bitmap</returns>
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
						dstpxl[0] = srcpxl[3];
						dstpxl[1] = srcpxl[3];
						dstpxl[2] = srcpxl[3];

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

		internal bool batch_processed = false;
		internal void ProcessBatch()
		{
			try
			{
				for (int c = 0; c < batchListView.Items.Count; c++)
				{
					this.Invoke(new SetProgressBarValueDelegate(SetProgressBarValue), new object[] { c, Resources.ProcessingStatusTextFormat });
					using(Bitmap temp = new Bitmap(patharray[c]))
					{
						BitmapItem item = new BitmapItem();

						item.Bitmap = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), PixelFormat.Format24bppRgb);
						string alname = Path.Combine(Path.GetDirectoryName(patharray[c]), Path.GetFileNameWithoutExtension(patharray[c]) + "_a" + Path.GetExtension(patharray[c]));
						if (File.Exists(alname))
						{
							using (Bitmap alpha = new Bitmap(alname))
							{
								item.Alpha = alpha.Clone(new Rectangle(0, 0, alpha.Width, alpha.Height), PixelFormat.Format24bppRgb);
							}
						   
						}
						else if (Path.GetExtension(patharray[c]).Equals(".png",StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
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
						if (typearray[c].ToUpperInvariant() == FSHBmpType.ThirtyTwoBit.ToString().ToUpperInvariant())
						{
							item.BmpType = FSHBmpType.ThirtyTwoBit;
						}
						else if (typearray[c].ToUpperInvariant() == FSHBmpType.TwentyFourBit.ToString().ToUpperInvariant())
						{
							item.BmpType = FSHBmpType.TwentyFourBit;
						}
						else if (typearray[c].ToUpperInvariant() == FSHBmpType.DXT1.ToString().ToUpperInvariant())
						{
							item.BmpType = FSHBmpType.DXT1;
						}
						else if (typearray[c].ToUpperInvariant() == FSHBmpType.DXT3.ToString().ToUpperInvariant())
						{
							item.BmpType = FSHBmpType.DXT3;
						}

						if (c <= batchFshList.Capacity)
						{
							FSHImage fsh = new FSHImage();
								
							fsh.Bitmaps.Add(item);
							fsh.UpdateDirty();
							batchFshList.Insert(c, new BatchFshContainer(fsh));

							using (MemoryStream mstream = new MemoryStream())
							{
								SaveFsh(mstream, batchFshList[c].MainImage);

								if (IsDXTFsh(batchFshList[c].MainImage) && fshWriteCompCb.Checked)
								{
									batchFshList[c].MainImage.SetRawData(mstream.ToArray());
								}
							}
						}
					}
					
				}
				batch_processed = true;
			}
			catch (ArgumentOutOfRangeException ag)
			{
				MessageBox.Show(this, ag.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception)
			{
				batch_processed = false;
				throw;
			}
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
		private void SaveFsh(Stream fs, FSHImage image)
		{
			try
			{
				if (fshWriteCompCb.Checked && IsDXTFsh(image))
				{
					Fshwrite fw = new Fshwrite(image.Bitmaps.Count);
					foreach (BitmapItem bi in image.Bitmaps)
					{
						if (bi.Bitmap != null && bi.Alpha != null)
						{
							fw.bmp.Add(bi.Bitmap);
							fw.alpha.Add(bi.Alpha);
							fw.dir.Add(bi.DirName);
							fw.code.Add((int)bi.BmpType);
						}
					}
					fw.WriteFsh(fs);
				}
				else
				{
					image.Save(fs);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
		/// <summary>
		/// Test if the fsh only contains DXT1 or DXT3 items
		/// </summary>
		/// <param name="image">The FSHImage to test</param>
		/// <returns>True if successful otherwise false</returns>
		private static bool IsDXTFsh(FSHImage image)
		{
			bool result = true;
			foreach (BitmapItem bi in image.Bitmaps)
			{
				if (bi.BmpType != FSHBmpType.DXT3 && bi.BmpType != FSHBmpType.DXT1)
				{
					result = false;
				}
			}
			return result;
		}

		private void SetProgressBarMaximum()
		{
			if (this.autoProcMipsCb.Checked)
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

        private TaskbarManager manager;
        private static JumpList jumpList;

		delegate void SetProgressBarValueDelegate(int value, string statusTextFormat);
		private void SetProgressBarValue(int value, string statusTextFormat)
		{            
			toolStripProgressBar1.PerformStep();			
            toolStripProgressStatus.Text = string.Format(CultureInfo.CurrentCulture, statusTextFormat, (value + 1), batchListView.Items.Count);

            if (manager != null)
            {
                manager.SetProgressValue(toolStripProgressBar1.Value, toolStripProgressBar1.Maximum, this.Handle);
            }
		}
        delegate int GetBatchListViewItemsCountDelegate();
        private int GetBatchListViewItemsCount()
        {
            return batchListView.Items.Count;
        }

		private void ProcessBatchSaveFiles()
		{
            int itemCount = (int)this.Invoke(new GetBatchListViewItemsCountDelegate(GetBatchListViewItemsCount));
			for (int c = 0; c < itemCount; c++)
			{
				string filepath;
                this.Invoke(new SetProgressBarValueDelegate(SetProgressBarValue), new object[] { c, Resources.SavingFshProgressTextFormat });
				BatchFshContainer batchFsh = batchFshList[c];
				if (batchFsh.MainImage != null)
				{
					filepath = GetFilePath(patharray[c], string.Empty, outfolder);
					using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
					{
						SaveFsh(fstream, batchFsh.MainImage);
					}
					this.Invoke(new WriteTgiDelegate(WriteTgi), new object[] {filepath, 4, c});
				}
				if (autoProcMipsCb.Checked)
				{
					if (batchFsh.Mip64Fsh != null)
					{
						filepath = GetFilePath(patharray[c], "_s3", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip64Fsh);
						}
                        this.Invoke(new WriteTgiDelegate(WriteTgi), new object[] { filepath, 3, c });
                    }
					if (batchFsh.Mip32Fsh != null)
					{
						filepath = GetFilePath(patharray[c], "_s2", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip32Fsh);
						}
                        this.Invoke(new WriteTgiDelegate(WriteTgi), new object[] { filepath, 2, c });
                    }
					if (batchFsh.Mip16Fsh != null)
					{
						filepath = GetFilePath(patharray[c], "_s1", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip16Fsh);
						}
                        this.Invoke(new WriteTgiDelegate(WriteTgi), new object[] { filepath, 1, c });
                    }
					if (batchFsh.Mip8Fsh != null)
					{
						filepath = GetFilePath(patharray[c], "_s0", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip8Fsh);
						}
                        this.Invoke(new WriteTgiDelegate(WriteTgi), new object[] { filepath, 0, c });
                    }
				}
			}
		}
		private Thread batchProcessThread = null;
		private Thread mipProcessThread = null;
		private Thread batchSaveFilesThread = null;
		internal void processbatchbtn_Click(object sender, EventArgs e)
		{
			try
			{
			   
				if (!batch_processed)
				{
					this.SetProgressBarMaximum();

					this.Cursor = Cursors.WaitCursor;
					Application.DoEvents();
					this.batchProcessThread = new Thread(new ThreadStart(ProcessBatch)) {Priority = ThreadPriority.AboveNormal, IsBackground = true };
					this.batchProcessThread.Start();
					while (batchProcessThread.IsAlive)
					{
						Application.DoEvents();
					}
					this.batchProcessThread.Join();
				}

				if (autoProcMipsCb.Checked)
				{
					this.mipProcessThread = new Thread(new ThreadStart(ProcessMips)) { Priority = ThreadPriority.AboveNormal, IsBackground = true };
					this.mipProcessThread.Start();
					while (mipProcessThread.IsAlive)
					{
						Application.DoEvents();
					}
					this.mipProcessThread.Join();
				}
			    this.batchSaveFilesThread = new Thread(new ThreadStart(ProcessBatchSaveFiles)) { Priority = ThreadPriority.AboveNormal, IsBackground = true };
				this.batchSaveFilesThread.Start();
				while (batchSaveFilesThread.IsAlive)
				{
					Application.DoEvents();
				}
				this.batchSaveFilesThread.Join();
				
				if (this.Cursor != Cursors.Default)
				{ 
					this.Cursor = Cursors.Default; 
				}
				ClearandReset();
			}
			catch (ArgumentOutOfRangeException ag)
			{
				MessageBox.Show(this,ag.Message,this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error); 
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		Settings settings = null;
		internal string grppath = null;
		internal string rangepath = null;
		private void LoadSettings()
		{
			try
			{
				settings = new Settings(Path.Combine(Application.StartupPath, @"PngtoFshBatch.xml"));
				compDatcb.Checked = bool.Parse(settings.GetSetting("compDatcb_checked", bool.TrueString));
				autoProcMipsCb.Checked = bool.Parse(settings.GetSetting("AutoprocessMips", bool.FalseString));
				fshWriteCompCb.Checked = bool.Parse(settings.GetSetting("fshwritecompcb_checked", bool.FalseString));
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}  
		private const uint SSE = 2;

		[return: MarshalAs(UnmanagedType.Bool)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("kernel32.dll")]
		private static extern bool IsProcessorFeaturePresent(uint ProcessorFeature);

		private void CheckForSSE()
		{
			if (!IsProcessorFeaturePresent(SSE))
			{
				MessageBox.Show(Resources.FshWriteSSERequiredError, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				fshWriteCompCb.Enabled = fshWriteCompCb.Checked = false;
			}
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			LoadSettings();
			CheckForSSE();
			fshTypeBox.SelectedIndex = 2;
			grppath = Path.Combine(Application.StartupPath, @"Groupid.txt");
			rangepath = Path.Combine(Application.StartupPath, @"instRange.txt");
			CheckRangeFilesExist(rangepath);
			CheckRangeFilesExist(grppath);
		}
		
		private Random ra = new Random();
		private string lowerinst = null;
		private string upperinst = null;
		internal string RandomHexString(int length)
		{
			const string numbers = "0123456789";
			const string hexcode = "ABCDEF";
			char[] charArray = new char[length];
			string hexstring = string.Empty;

			hexstring += numbers;
			hexstring += hexcode;

			ReadRangetxt(rangepath, false);
			for (int c = 0; c < charArray.Length; c++)
			{
				int index;
				if (!string.IsNullOrEmpty(lowerinst) && !string.IsNullOrEmpty(upperinst))
				{
                    long lower = long.Parse(lowerinst, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    long upper = long.Parse(upperinst, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					double rn = (upper * 1.0 - lower * 1.0) * ra.NextDouble() + lower * 1.0;
					return Convert.ToInt64(rn).ToString("X8", CultureInfo.InvariantCulture);
				}
				else
				{
				   
					index = ra.Next(0, hexstring.Length);
					charArray[c] = hexstring[index];
				}

			}
			return new string(charArray);
		}
  
		private void compDatcb_CheckedChanged(object sender, EventArgs e)
		{
			if (settings != null)
			{
				settings.PutSetting("compDatcb_checked", compDatcb.Checked.ToString());
			}
		}
		private void autoprocMipscb_CheckedChanged(object sender, EventArgs e)
		{
			if (settings != null)
			{
				settings.PutSetting("AutoprocessMips", autoProcMipsCb.Checked.ToString());
			}
		}
		internal string outfolder = string.Empty;
		private void outfolderbtn_Click(object sender, EventArgs e)
		{
			if (OutputBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				outfolder = OutputBrowserDialog1.SelectedPath;
			}
		}
		
		internal void ReadRangetxt(string path, bool grouptxt)
		{
			try
			{
				if (grouptxt)
				{
					if (File.Exists(path))
					{
						using (StreamReader sr = new StreamReader(path))
						{
							string line;
							while ((line = sr.ReadLine()) != null)
							{
								if (!string.IsNullOrEmpty(line))
								{
									string g = null;
									if (ValidateHexString(line))
									{
										g = line;
									}
									else
									{
										g = "1ABE787D";
										MessageBox.Show(this, Resources.InvalidGroupID, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
									}
									for (int i = 0; i < patharray.Count; i++)
									{
										grouparray.Insert(i, g);
									}
								}
							}
						}
					}
					else
					{
						string errgrp = "1ABE787D";
						for (int i = 0; i < patharray.Count; i++)
						{
							grouparray.Insert(i, errgrp);
						}
                        string message = string.Format(CultureInfo.CurrentCulture, Resources.FileNotFoundFormat, Path.GetFileName(path), path);
						throw new FileNotFoundException(message);

					}
				}
				else
				{
					if (File.Exists(path))
					{ 
						string[] instarray = null;
						using (StreamReader sr = new StreamReader(path))
						{
							string line;
							char[] splitchar = new char[] { ',' };
							while ((line = sr.ReadLine()) != null)
							{
								if (!string.IsNullOrEmpty(line))
								{
									instarray = line.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
								}

							}
						}
						if (instarray != null)
						{
							string inst0 = instarray[0];
							string inst1 = instarray[1];
  
							if (inst0.Length == 10)
							{
								if (inst0.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
								{
									lowerinst = inst0.Substring(2, 8);
								}
							}
							else if (inst0.Length == 8)
							{
								lowerinst = inst0;
							}
							if (inst1.Length == 10)
							{
                                if (inst1.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
								{
									upperinst = inst1.Substring(2, 8);
								}
							}
							else if (inst1.Length == 8)
							{
								upperinst = inst1;
							}
							
						}

					}
					else
					{
						lowerinst = string.Empty;
						upperinst = string.Empty;
					}
				}
			}
			catch (FileNotFoundException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}
		}
		internal static bool ValidateHexString(string str)
		{
			string tmp = null;
			if (!string.IsNullOrEmpty(str))
			{ 
			   
				if (str.Length == 10)
				{
					if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
					{
						tmp = str.Substring(2, 8);
					}
				}

				if (str.Length == 8 || tmp != null)
				{
					Regex r = new Regex(@"^[A-Fa-f0-9]*$"); ;
					if (tmp != null)
					{
						return r.IsMatch(tmp);
					}
					else
					{
						return r.IsMatch(str);
					}
				}
				else
				{
					return false;
				}
			}
			return false;
		}
        internal const string ProgramName = "Png to Fsh Batch";

        private void AddRecentFolder(string path)
        {
            if (jumpList != null)
            {
                using (JumpListLink link = new JumpListLink(Assembly.GetExecutingAssembly().Location, Path.GetFileName(path)))
                {
                    link.Arguments = path;
                    link.IconReference = new Microsoft.WindowsAPICodePack.Shell.IconReference("shell32.dll", 3);

                    JumpListHelper.AddToRecent(link, manager.ApplicationId);
                }

                jumpList.Refresh();
            } 
        }

		internal static int Countpngs(string[] filenames)
		{                
			int fcnt = 0;
			try
			{
				for (int f = 0; f < filenames.Length; f++)
				{
					if (filenames[f].StartsWith("/proc", StringComparison.OrdinalIgnoreCase) || filenames[f].StartsWith("/mips", StringComparison.OrdinalIgnoreCase) || filenames[f].StartsWith("/dat:", StringComparison.OrdinalIgnoreCase) || filenames[f].StartsWith("/outDir:", StringComparison.OrdinalIgnoreCase) || filenames[f].StartsWith("/?", StringComparison.OrdinalIgnoreCase) || filenames[f].StartsWith("/group:", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					if ((File.GetAttributes(filenames[f]) & FileAttributes.Directory) == FileAttributes.Directory)
					{
						DirectoryInfo di = new DirectoryInfo(filenames[f]);
						ArrayList fa = new ArrayList();

						fa.AddRange(di.GetFiles("*.png", SearchOption.TopDirectoryOnly));
						FileInfo[] bmparr = di.GetFiles("*.bmp", SearchOption.TopDirectoryOnly);
						if (bmparr.Length > 0)
						{
							fa.AddRange(bmparr);
						}

						for (int i = 0; i < fa.Count; i++)
						{
							FileInfo fi = (FileInfo)fa[i];

							if (Path.GetFileName(fi.FullName).Contains("_a"))
							{
								continue;
							}
							else
							{
								fcnt++;
							}
						}                        
                      
					}
					else
					{
						FileInfo fi = new FileInfo(filenames[f]);
						if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
						{

							if (Path.GetFileName(filenames[f]).Contains("_a"))
							{
								continue;
							}
							else
							{
								fcnt++;
							}
						}
					}
				}
				return fcnt;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ProgramName);
				return 0;
			}

			
		}

		private void SetEndFormat(Bitmap temp, int index)
		{
            SetInstanceEndChars(index);

			if (temp.Width >= 128 && temp.Height >= 128)
			{
				instarray[index] = instarray[index].Substring(0, 7) + endreg;
			}
			else if (temp.Width == 64 && temp.Height == 64)
			{
				instarray[index] = instarray[index].Substring(0, 7) + end64;
			}
			else if (temp.Width == 32 && temp.Height == 32)
			{
				instarray[index] = instarray[index].Substring(0, 7) + end32;
			}
			else if (temp.Width == 16 && temp.Height == 16)
			{
				instarray[index] = instarray[index].Substring(0, 7) + end16;
			}
			else if (temp.Width == 8 && temp.Height == 8)
			{
				instarray[index] = instarray[index].Substring(0, 7) + end8;
			}
		}

		private void FormatRefresh(int index)
		{
			using (Bitmap temp = new Bitmap(patharray[index]))
			{
				SetEndFormat(temp, index);
			}
			
			batchListView.SelectedItems[0].SubItems[3].Text = instarray[index];
			tgiInstanceTxt.Text = instarray[index];
		}
		private void Format_radios_changed(object sender, EventArgs e)
		{
			if (batchListView.SelectedItems.Count > 0)
			{
				FormatRefresh(batchListView.SelectedItems[0].Index);
			}
			else
			{
				Inst5_9rdo.Checked = false;
				InstA_Erdo.Checked = false;
				Inst0_4rdo.Checked = true;
			}
		}
		internal void BuildPngList()
		{
			try
			{
				if (tgiGroupTxt.Text.Length > 0 && tgiGroupTxt.Text.Length == 8)
				{
					for (int j = 0; j < patharray.Count; j++)
					{
						grouparray.Insert(j, tgiGroupTxt.Text);
					}
				}
				else
				{
					ReadRangetxt(grppath, true);
				}


				for (int n = 0; n < patharray.Count; n++)
				{
					if (Path.GetFileName(patharray[n]).StartsWith("0x", StringComparison.OrdinalIgnoreCase))
					{
						string pathinst = Path.GetFileNameWithoutExtension(patharray[n]).Substring(2, 8);
						instarray.Insert(n, pathinst);
					}
					else
					{
						instarray.Insert(n, RandomHexString(7));
					}
					using (Bitmap temp = new Bitmap(patharray[n]))
					{
						if (!Path.GetFileName(patharray[n]).StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
                            SetEndFormat(temp, n);
						}


						string alname = Path.Combine(Path.GetDirectoryName(patharray[n]), Path.GetFileNameWithoutExtension(patharray[n]) + "_a" + Path.GetExtension(patharray[n]));
						string fn = Path.GetFileName(patharray[n]);
						if (File.Exists(alname))
						{
							typearray.Insert(n, "DXT3");
						}
						else if (temp.PixelFormat == PixelFormat.Format32bppArgb)
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typearray.Insert(n, "ThirtyTwoBit");
							}
							else
							{
								typearray.Insert(n, "DXT3");
							}
						}
						else
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typearray.Insert(n, "TwentyFourBit");
							}
							else
							{
								typearray.Insert(n, "DXT1");
							}
						}
					}

				}


				for (int i = 0; i < patharray.Count; i++)
				{
					if (File.Exists(patharray[i]))
					{
						ListViewItem item1 = new ListViewItem(Path.GetFileName(patharray[i]));
						Alphasrc(item1, patharray[i]);
						item1.SubItems.Add(grouparray[i]);
						item1.SubItems.Add(instarray[i]);
						batchListView.Items.Add(item1);
					}
				}
				/*Debug.Assert(patharray.Count == batchListView.Items.Count,"The number of items in the batch and patharray lists do not match"
					,string.Concat("patharray count: " + patharray.Count.ToString()," does not match ", "BatchlistView count: " +batchListView.Items.Count.ToString()));*/
			}
			catch (FileNotFoundException fx)
			{
				MessageBox.Show(fx.Message + fx.FileName, this.Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			
		}
	   
		private void remBtn_Click(object sender, EventArgs e)
		{
			if (batchListView.SelectedItems.Count > 0 && batchListView.Items.Count > 1)
			{
				try
				{
					int index = batchListView.SelectedItems[0].Index;
					grouparray.RemoveAt(index);
					instarray.RemoveAt(index);
					typearray.RemoveAt(index);
					patharray.RemoveAt(index);
					if (batchFshList.Count > 0)
					{
						if (batchFshList[index] != null)
						{
							batchFshList.RemoveAt(index);
							batchFshList.Capacity = batchFshList.Count;
						}
					}
				   
					batchListView.Items.RemoveAt(index);
					ListViewItem selitem = batchListView.Items[0];
					selitem.Selected = true;
					batchListView.Refresh();
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, ex.Message, this.Text);
				}
			}
			else if (batchListView.SelectedItems.Count > 0 && batchListView.Items.Count == 1)
			{
				ClearandReset();
			}

		}
	   
		private void BuildAddList(int dif)
		{
			try
			{
				if (tgiGroupTxt.Text.Length > 0 && tgiGroupTxt.Text.Length == 8)
				{
					for (int j = 0; j < patharray.Count; j++)
					{
						grouparray.Insert(j, tgiGroupTxt.Text);
					}
				}
				else
				{
					ReadRangetxt(grppath, true);
				}

				for (int n = dif; n < patharray.Count; n++)
				{
					if (Path.GetFileName(patharray[n]).StartsWith("0x", StringComparison.OrdinalIgnoreCase))
					{
						string pathinst = Path.GetFileNameWithoutExtension(patharray[n]).Substring(2, 8);
						instarray.Insert(n, pathinst);
					}
					else
					{
						instarray.Insert(n, RandomHexString(7));
					}

					using (Bitmap temp = new Bitmap(patharray[n]))
					{
						if (!Path.GetFileName(patharray[n]).StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							SetEndFormat(temp, n);
						}

						string alname = Path.Combine(Path.GetDirectoryName(patharray[n]), Path.GetFileNameWithoutExtension(patharray[n]) + "_a" + Path.GetExtension(patharray[n]));
						string fn = Path.GetFileName(patharray[n]);
						if (File.Exists(alname))
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typearray.Insert(n, "ThirtyTwoBit");
							}
							else
							{
								typearray.Insert(n, "DXT3");
							}
						}
						else if (Path.GetExtension(patharray[n]).Equals(".png",StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typearray.Insert(n, "ThirtyTwoBit");
							}
							else
							{
								typearray.Insert(n, "DXT3");
							}
						}
						else
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typearray.Insert(n, "TwentyFourBit");
							}
							else
							{
								typearray.Insert(n, "DXT1");
							}
						}
					}
				}

				for (int i = dif; i < patharray.Count; i++)
				{
					if (File.Exists(patharray[i]))
					{
						ListViewItem item1 = new ListViewItem(Path.GetFileName(patharray[i]));
						Alphasrc(item1, patharray[i]);
						item1.SubItems.Add(grouparray[i]);
						item1.SubItems.Add(instarray[i]);
						batchListView.Items.Insert(i, item1);
					}
				}
			}
			catch (FileNotFoundException fx)
			{
				MessageBox.Show(fx.Message + fx.FileName, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception)
			{
				throw;
			} 
		}
		private void addBtn_Click(object sender, EventArgs e)
		{
			if (PngopenDialog.ShowDialog() == DialogResult.OK)
			{
				int fcnt = Countpngs(PngopenDialog.FileNames);
				int cnt;
				int pathcnt;
				if (patharray == null && instarray == null && grouparray == null && typearray == null)
				{
					patharray = new List<string>(fcnt);
					instarray = new List<string>(fcnt);
					grouparray = new List<string>(fcnt);
					typearray = new List<string>(fcnt);
				}
				if (patharray.Count > 0)
				{
					pathcnt = patharray.Count;
					cnt = fcnt + pathcnt;
				}
				else
				{
					pathcnt = 0;
					cnt = fcnt;
				}
				if (batchFshList == null)
				{ 
					batchFshList = new List<BatchFshContainer>(fcnt);
				}
				batchFshList.SetCapacity(cnt);
				patharray.SetCapacity(cnt);
				grouparray.SetCapacity(cnt);
				instarray.SetCapacity(cnt);
                typearray.SetCapacity(cnt);
				try
				{
					for (int f = 0; f < PngopenDialog.FileNames.Length; f++)
					{
						if (Path.GetFileName(PngopenDialog.FileNames[f]).Contains("_a"))
						{
							continue;
						}
						else
						{
							patharray.Add(PngopenDialog.FileNames[f]);
						}
					}
					int dif;
					if (pathcnt != 0)
					{
						dif = cnt - fcnt;
					}
					else
					{
						dif = 0;
					}
					BuildAddList(dif);
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, ex.Message, this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
					ClearandReset();
				}
			}
			
		}
		private void tgiGroupTxt_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) && ModifierKeys != Keys.Shift)
			{
				e.Handled = true;
				e.SuppressKeyPress = false;
			}
			else if (e.KeyCode == Keys.A || e.KeyCode == Keys.B || e.KeyCode == Keys.C || e.KeyCode == Keys.D || e.KeyCode == Keys.E || e.KeyCode == Keys.F || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
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
			if (batchListView.SelectedItems.Count > 0)
			{
				int index = batchListView.SelectedItems[0].Index;
				if (tgiGroupTxt.Text.Length > 0 && tgiGroupTxt.Text.Length == 8)
				{
					if (!tgiGroupTxt.Text.Equals(grouparray[index], StringComparison.OrdinalIgnoreCase))
					{
						grouparray[index] = tgiGroupTxt.Text;
						batchListView.SelectedItems[0].SubItems[2].Text = grouparray[index];
					}
				}
			}
		}

		private void tgiInstanceTxt_TextChanged(object sender, EventArgs e)
		{
			if (batchListView.SelectedItems.Count > 0)
			{
				int index = batchListView.SelectedItems[0].Index;
				if (tgiInstanceTxt.Text.Length > 0 && tgiInstanceTxt.Text.Length == 8)
				{
					if (!tgiInstanceTxt.Text.Equals(instarray[index], StringComparison.OrdinalIgnoreCase))
					{
						instarray.Insert(index, tgiInstanceTxt.Text);
						FormatRefresh(index);
					}
				}
			}
		}

		private void fshTypeBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (batchListView.SelectedItems.Count > 0)
			{
				int index = batchListView.SelectedItems[0].Index;
				string seltype = null;
				using (Bitmap b = new Bitmap(patharray[index]))
				{
					switch (fshTypeBox.SelectedIndex)
					{
						case 0:
							seltype = FSHBmpType.TwentyFourBit.ToString();
							break;
						case 1:
							seltype = FSHBmpType.ThirtyTwoBit.ToString();
							break;
						case 2:
							seltype = FSHBmpType.DXT1.ToString();
							break;
						case 3:
							seltype = FSHBmpType.DXT3.ToString();
							break;
					}
				}
				if (!seltype.ToUpperInvariant().Equals(typearray[index], StringComparison.OrdinalIgnoreCase))
				{
					typearray[index] = seltype;
				}
			}
		}

		private void batchListView1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
			{
				e.Effect = DragDropEffects.Copy;
			}
		}
		
		private string[] GetFilesfromDirectory(string[] filenames)
		{
			ArrayList flist = new ArrayList();

			for (int f = 0; f < filenames.Length; f++)
			{
				if ((File.GetAttributes(filenames[f]) & FileAttributes.Directory) == FileAttributes.Directory)
				{
					DirectoryInfo di = new DirectoryInfo(filenames[f]);
					ArrayList fa = new ArrayList();

					fa.AddRange(di.GetFiles("*.png", SearchOption.TopDirectoryOnly));
					FileInfo[] bmparr = di.GetFiles("*.bmp", SearchOption.TopDirectoryOnly);
					if (bmparr.Length > 0)
					{
						fa.AddRange(bmparr);
					}

					for (int i = 0; i < fa.Count; i++)
					{
						FileInfo fi = (FileInfo)fa[i];
						if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
						{
							if (Path.GetFileName(fi.FullName).Contains("_a"))
							{
								continue;
							}
							else
							{
								flist.Add(fi.FullName);
							}
						}
					}

                    AddRecentFolder(di.FullName);
				}
				else
				{
						FileInfo fi = new FileInfo(filenames[f]);
						if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
						{

							if (Path.GetFileName(filenames[f]).Contains("_a"))
							{
								continue;
							}
							else
							{
								flist.Add(fi.FullName);
							}
						}
				}
			}
			return (string[])flist.ToArray(typeof(string));
		}

		private void batchListView1_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = GetFilesfromDirectory((string[])e.Data.GetData(DataFormats.FileDrop));
			if (batchListView.Items.Count > 0)
			{
				ClearandReset();
			}
			int fcnt = Countpngs(files);
			int pngcnt = -1;
			patharray = new List<string>(fcnt);
			instarray = new List<string>(fcnt);
			grouparray = new List<string>(fcnt);
			typearray = new List<string>(fcnt);
			batchFshList = new List<BatchFshContainer>(fcnt);

			for (int f = 0; f < files.Length; f++)
			{
				FileInfo fi = new FileInfo(files[f]);
				if (fi.Exists)
				{

					if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
					{
						if (Path.GetFileName(files[f]).Contains("_a"))
						{
							continue;
						}
						else
						{
							pngcnt++;
							patharray.Insert(pngcnt, files[f]);
						}
					}
				}
			}
			BuildPngList();
		}

		private void addBtn_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
			{
				e.Effect = DragDropEffects.Copy;
			}
		}
		private void ClearandReset()
		{
			batchListView.Items.Clear();
			if (patharray != null && instarray != null && grouparray != null && typearray != null)
			{
				patharray.Clear();
				patharray = null;
				instarray.Clear();
				instarray = null;
				grouparray.Clear();
				grouparray = null;
				typearray.Clear();
				typearray = null;
			}

			if (batchFshList != null)
			{
				batchFshList.Clear();
				batchFshList = null;
			}
			if (outfolder != null)
			{
				outfolder = null;
			}
			mipsbtn_clicked = false;
			batch_processed = false;
			datRebuilt = false;
			tgiGroupTxt.Text = null;
			tgiInstanceTxt.Text = null;
			fshTypeBox.SelectedIndex = 2;

			this.toolStripProgressBar1.Value = 0;

            this.toolStripProgressStatus.Text = Resources.StatusTextReset;   
            if (manager != null)
            {
                this.manager.SetProgressState(TaskbarProgressBarState.NoProgress, this.Handle);
            }
		}

		private void addBtn_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = GetFilesfromDirectory((string[])e.Data.GetData(DataFormats.FileDrop));
			int fcnt = Countpngs(files);
			int count;
			int pathcnt;
			if (patharray == null && instarray == null && grouparray == null && typearray == null)
			{
				patharray = new List<string>(fcnt);
				instarray = new List<string>(fcnt);
				grouparray = new List<string>(fcnt);
				typearray = new List<string>(fcnt);
			}
			if (patharray.Count > 0)
			{
				pathcnt = patharray.Count;
				count = fcnt + pathcnt;
			}
			else
			{
				pathcnt = 0;
				count = fcnt;
			}
			if (batchFshList == null)
			{
				batchFshList = new List<BatchFshContainer>(fcnt);
			}
			batchFshList.SetCapacity(count);
            patharray.SetCapacity(count);
			grouparray.SetCapacity(count);
			instarray.SetCapacity(count);
			typearray.SetCapacity(count);
			try 
			{
				for (int f = 0; f < files.Length; f++)
				{
					FileInfo fi = new FileInfo(files[f]);
					if (fi.Exists)
					{

						if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
						{
							if (Path.GetFileName(files[f]).Contains("_a"))
							{
								continue;
							}
							else
							{
								patharray.Add(files[f]);
							}
						}
					}
				}
				int dif;
				if (pathcnt != 0)
				{
					dif = count - fcnt;
				}
				else
				{
					dif = 0;
				}
				BuildAddList(dif);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				ClearandReset();
			}
		}

		private void clearlistbtn_Click(object sender, EventArgs e)
		{
			ClearandReset();
		}
	   
		private bool Checkhdimgsize(int index)
		{ 
			using (Bitmap b = new Bitmap(patharray[index]))
			{
				if (b.Width >= 256 && b.Height >= 256)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		private void fshTypeBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (batchListView.SelectedItems.Count > 0)
			{
				if (!Checkhdimgsize(batchListView.SelectedItems[0].Index))
				{
					if (e.Index == 0 || e.Index == 1)
					{
						// make the hd fsh items look disabled 
						string text = cb.Items[e.Index].ToString();
						e.DrawBackground();
						e.Graphics.DrawString(text, e.Font, SystemBrushes.GrayText, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
						e.DrawFocusRectangle();
					}
					else
					{
						//leave the other items alone
						string text = cb.Items[e.Index].ToString();
						e.DrawBackground();
						e.Graphics.DrawString(text, e.Font, SystemBrushes.WindowText, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
						e.DrawFocusRectangle();
					}
				}
				else
				{
					// draw it normally
					string text = cb.Items[e.Index].ToString();
					e.DrawBackground();
					e.Graphics.DrawString(text, e.Font, SystemBrushes.WindowText, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
					e.DrawFocusRectangle();
				}
			}
			else
			{
				// draw it normally
				string text = cb.Items[e.Index].ToString();
				e.DrawBackground();
				e.Graphics.DrawString(text, e.Font, SystemBrushes.WindowText, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
				e.DrawFocusRectangle();
			}
		}

		private void fshTypeBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (batchListView.SelectedItems.Count > 0)
			{ 
				if (!Checkhdimgsize(batchListView.SelectedItems[0].Index))
				{
					// if it is not 256 disable hd fsh
					if (fshTypeBox.SelectedIndex == 0 || fshTypeBox.SelectedIndex == 1)
					{
						fshTypeBox.SelectedIndex = typeindex;
					}
				}
			}
		}
		private void CheckRangeFilesExist(string path)
		{
            if (!File.Exists(path))
            {
                string fileName = Path.GetFileName(path);
                string message = string.Format(CultureInfo.CurrentCulture, Resources.FileNotFoundFormat, fileName, path);

				MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
		}

		private void fshwritecompcb_CheckedChanged(object sender, EventArgs e)
		{
			if (settings != null)
			{
				settings.PutSetting("fshwritecompcb_checked", fshWriteCompCb.Checked.ToString());
			}
		}

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (manager != null)
            {
                jumpList = JumpList.CreateJumpList();
            }

        }
	}
}
