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
			int count = batchListView.Items.Count;
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
			
			for (int n = 0; n < count; n++)
			{
				this.Invoke(new Action<int, string>(SetProgressBarValue), new object[] { n, Resources.ProcessingMipsStatusTextFormat });
				Bitmap[] bmps = new Bitmap[4];
				Bitmap[] alphas = new Bitmap[4];

				

				BatchFshContainer batchFsh = batchFshList[n];

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

					bool fshWriteCompression = fshWriteCompCb.Checked;
					for (int i = 3; i >= 0; i--)
					{
						if (bmps[i] != null && alphas[i] != null)
						{
							BitmapEntry mipitm = new BitmapEntry();
							mipitm.Bitmap = bmps[i];
							mipitm.Alpha = alphas[i];
							mipitm.DirName = item.DirName;

							if (item.BmpType == FshImageFormat.DXT3 || item.BmpType ==  FshImageFormat.ThirtyTwoBit)
							{
								mipitm.BmpType = FshImageFormat.DXT3;
							}
							else
							{
								mipitm.BmpType = FshImageFormat.DXT1;
							}

							switch (i)
							{
								case 3:
									batchFsh.Mip64Fsh = new FSHImageWrapper();
									batchFsh.Mip64Fsh.Bitmaps.Add(mipitm);
									using (MemoryStream mstream = new MemoryStream())
									{
										batchFsh.Mip64Fsh.Save(mstream, fshWriteCompression);
									}
									break;
								case 2: 
									batchFsh.Mip32Fsh = new FSHImageWrapper();
									batchFsh.Mip32Fsh.Bitmaps.Add(mipitm);
									using (MemoryStream mstream = new MemoryStream())
									{
										batchFsh.Mip32Fsh.Save(mstream, fshWriteCompression);
									}
									break;
								case 1:
									batchFsh.Mip16Fsh = new FSHImageWrapper();
									batchFsh.Mip16Fsh.Bitmaps.Add(mipitm);
									using (MemoryStream mstream = new MemoryStream())
									{
										batchFsh.Mip16Fsh.Save(mstream, fshWriteCompression);
									}
									break;
								case 0:
									batchFsh.Mip8Fsh = new FSHImageWrapper();
									batchFsh.Mip8Fsh.Bitmaps.Add(mipitm);
									using (MemoryStream mstream = new MemoryStream())
									{
										batchFsh.Mip8Fsh.Save(mstream, fshWriteCompression);
									}
									break;
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
			using (Bitmap temp = SuperSample.GetBitmapThumbnail(source, width, height))
			{
				return temp.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format24bppRgb);
			}
		}

		/// <summary>
		/// Sets the instance end chars.
		/// </summary>
		/// <param name="index">The index in the instArray to compare to.</param>
		private void SetInstanceEndChars(int index)
		{
			if (instArray[index].EndsWith("4", StringComparison.Ordinal) || instArray[index].EndsWith("3", StringComparison.Ordinal)
						   || instArray[index].EndsWith("2", StringComparison.Ordinal) || instArray[index].EndsWith("1", StringComparison.Ordinal) || instArray[index].EndsWith("0", StringComparison.Ordinal))
			{
				endreg = '4';
				end64 = '3';
				end32 = '2';
				end16 = '1';
				end8 = '0';
			}
			else if (instArray[index].EndsWith("9", StringComparison.Ordinal) || instArray[index].EndsWith("8", StringComparison.Ordinal)
				|| instArray[index].EndsWith("7", StringComparison.Ordinal) || instArray[index].EndsWith("6", StringComparison.Ordinal) || instArray[index].EndsWith("5", StringComparison.Ordinal))
			{
				endreg = '9';
				end64 = '8';
				end32 = '7';
				end16 = '6';
				end8 = '5';
			}
			else if (instArray[index].EndsWith("E", StringComparison.Ordinal) || instArray[index].EndsWith("D", StringComparison.Ordinal)
				|| instArray[index].EndsWith("C", StringComparison.Ordinal) || instArray[index].EndsWith("B", StringComparison.Ordinal) || instArray[index].EndsWith("A", StringComparison.Ordinal))
			{
				endreg = 'E';
				end64 = 'D';
				end32 = 'C';
				end16 = 'B';
				end8 = 'A';
			}
		}

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
					fs = null;
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

		internal List<string> pathArray = null;
		internal List<string> instArray = null;
		internal List<string> typeArray = null;
		internal List<string> groupArray = null;
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
				FshImageFormat type = (FshImageFormat)Enum.Parse(typeof(FshImageFormat), typeArray[index]);
				switch (type)
				{ 
					case FshImageFormat.TwentyFourBit:
						fshTypeBox.SelectedIndex = 0;
						typeindex = 0;
						break;
					case FshImageFormat.ThirtyTwoBit:
						fshTypeBox.SelectedIndex = 1;
						typeindex = 1;
						break;
					case FshImageFormat.DXT1:
						fshTypeBox.SelectedIndex = 2;
						typeindex = 2;
						break;
					case FshImageFormat.DXT3:
						fshTypeBox.SelectedIndex = 3;
						typeindex = 3;
						break;
				}
			}
		}

		delegate ListViewItem[] GetBatchListViewItemsDelegate();

		private ListViewItem[] GetBatchListViewItems()
		{
			ListViewItem[] items = new ListViewItem[batchListView.Items.Count];
			batchListView.Items.CopyTo(items, 0);
			return items;
		}

		internal DatFile dat = null;
		internal bool compress_datmips = false;
		private Thread datRebuildThread = null;
		private bool datRebuilt = false;

		private void RebuildDat(DatFile inputdat)
		{
			ListViewItem[] items = (ListViewItem[])base.Invoke(new GetBatchListViewItemsDelegate(GetBatchListViewItems));
			int itemCount = items.Length;

			if (mipsbtn_clicked)
			{
				for (int i = 0; i < itemCount; i++)
				{
					this.Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.BuildingDatStatusTextFormat });

					BatchFshContainer batchFsh = batchFshList[i];
					ListViewItem item = items[i];
					uint group = uint.Parse(item.SubItems[2].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					uint[] instanceid = new uint[5];
					FshWrapper[] fshwrap = new FshWrapper[5];
					FSHImageWrapper[] fshimg = new FSHImageWrapper[5];
					if (batchFsh.Mip64Fsh != null && batchFsh.Mip32Fsh != null && batchFsh.Mip16Fsh != null && batchFsh.Mip8Fsh != null)
					{
						fshimg[0] = batchFsh.Mip8Fsh;
						fshimg[1] = batchFsh.Mip16Fsh;
						fshimg[2] = batchFsh.Mip32Fsh;
						fshimg[3] = batchFsh.Mip64Fsh;
					}

					fshimg[4] = batchFsh.MainImage;

					SetInstanceEndChars(i);

					string sub = item.SubItems[3].Text.Substring(0, 7);
					instanceid[0] = uint.Parse(sub + end8, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[1] = uint.Parse(sub + end16, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[2] = uint.Parse(sub + end32, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[3] = uint.Parse(sub + end64, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[4] = uint.Parse(item.SubItems[3].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					for (int j = 4; j >= 0; j--)
					{
						if (fshimg[j] != null)
						{
							fshwrap[j] = new FshWrapper(fshimg[j]) { UseFshWrite = fshWriteCompCb.Checked };
							CheckInstance(inputdat, group, instanceid[j]);

							
							inputdat.Add(fshwrap[j], group, instanceid[j], compress_datmips);
						}
					}


				}
			}
			else if (batchFshList != null)
			{
				bool embeddedMipmaps = mipFormat == MipmapFormat.Embedded;
				for (int i = 0; i < itemCount; i++)
				{
					this.Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.BuildingDatStatusTextFormat });

					ListViewItem item = items[i];

					uint group = uint.Parse(item.SubItems[2].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					uint instanceid = new uint();
					FshWrapper fshwrap = new FshWrapper();
					SetInstanceEndChars(i);
					instanceid = uint.Parse(item.SubItems[3].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					FSHImageWrapper mainImage = batchFshList[i].MainImage;
					if (mainImage != null)
					{
						if (embeddedMipmaps)
						{
							mainImage.Bitmaps[0].CalculateMipmapCount();
						}

						fshwrap = new FshWrapper(mainImage) { UseFshWrite = fshWriteCompCb.Checked };

						CheckInstance(inputdat, group, instanceid);
					   
						inputdat.Add(fshwrap, group, instanceid, compress_datmips);
					}
				}
			}

			this.BeginInvoke(new MethodInvoker(delegate()
			{                
				this.toolStripProgressStatus.Text = Resources.SavingDatStatusText;
			}));

			datRebuilt = true;
		}

		/// <summary>
		/// Rebuilds the dat for the command line processing.
		/// </summary>
		/// <param name="inputdat">The inputdat.</param>
		internal void RebuildDatCmd(DatFile inputdat)
		{
			ListViewItem[] items = GetBatchListViewItems();
			int itemCount = items.Length;
			if (mipsbtn_clicked)
			{                
				uint[] instanceid = new uint[5];
				FshWrapper[] fshwrap = new FshWrapper[5];
				FSHImageWrapper[] fshimg = new FSHImageWrapper[5];
				for (int i = 0; i < itemCount; i++)
				{
					BatchFshContainer batchFsh = batchFshList[i];
					ListViewItem item = items[i];
					uint group = uint.Parse(item.SubItems[2].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					if (batchFsh.Mip64Fsh != null && batchFsh.Mip32Fsh != null && batchFsh.Mip16Fsh != null && batchFsh.Mip8Fsh != null)
					{
						fshimg[0] = batchFsh.Mip8Fsh;
						fshimg[1] = batchFsh.Mip16Fsh;
						fshimg[2] = batchFsh.Mip32Fsh;
						fshimg[3] = batchFsh.Mip64Fsh;
					}
					else
					{

					}

					fshimg[4] = batchFsh.MainImage;

					SetInstanceEndChars(i);

					instanceid[0] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end8, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[1] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end16, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[2] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end32, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[3] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end64, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[4] = uint.Parse(item.SubItems[3].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					for (int j = 4; j >= 0; j--)
					{
						if (fshimg[j] != null)
						{
							fshwrap[j] = new FshWrapper(fshimg[j]) { UseFshWrite = fshWriteCompCb.Checked };
							CheckInstance(inputdat, group, instanceid[j]);

							inputdat.Add(fshwrap[j], group, instanceid[j], compress_datmips);
						}
					}


				}
			}
			else if (batchFshList != null)
			{
				bool embeddedMipmaps = mipFormat == MipmapFormat.Embedded;
				for (int i = 0; i < itemCount; i++)
				{
					ListViewItem item = items[i];

					uint group = uint.Parse(item.SubItems[2].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					uint instanceid = new uint();
					FshWrapper fshwrap = new FshWrapper();
					SetInstanceEndChars(i);
					instanceid = uint.Parse(item.SubItems[3].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					FSHImageWrapper mainImage = batchFshList[i].MainImage;
					if (mainImage != null)
					{
						if (embeddedMipmaps)
						{
							foreach (var entry in mainImage.Bitmaps)
							{
								entry.CalculateMipmapCount();
							}
						}
						fshwrap = new FshWrapper(mainImage) { UseFshWrite = fshWriteCompCb.Checked };

						CheckInstance(inputdat, group, instanceid);

						inputdat.Add(fshwrap, group, instanceid, compress_datmips);
					}
				}
			}

			datRebuilt = true;
		}

		/// <summary>
		/// Checks the dat for files with the same TGI id
		/// </summary>
		/// <param name="checkdat">The Dat to check</param>
		/// <param name="group">The group id to check</param>
		/// <param name="instance">The instance id to check</param>
		private static void CheckInstance(DatFile checkdat, uint group, uint instance)
		{
			int count = checkdat.Indexes.Count;
			for (int n = 0; n < count; n++)
			{
				DatIndex chkindex = checkdat.Indexes[n];
				if (chkindex.Type == 0x7ab50e44U && chkindex.Group == group && chkindex.IndexState != DatIndexState.New)
				{
					if (chkindex.Instance == instance)
					{
						checkdat.Remove(group, instance);
					}
				}
			}
		}



		private void saveDatbtn_Click(object sender, EventArgs e)
		{
			if (batchListView.Items.Count > 0)
			{
				try
				{

					if (saveDatDialog1.ShowDialog(this) == DialogResult.OK)
					{
						if (File.Exists(saveDatDialog1.FileName))
						{
							try
							{
								dat = new DatFile(saveDatDialog1.FileName);
							}
							catch (DatHeaderException)
							{
								dat.Dispose();
								dat = new DatFile();
							}
						}
						else if (dat == null)
						{
							dat = new DatFile();
						}

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

						if (this.mipFormat == MipmapFormat.Normal)
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
					MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
		private void ProcessBatch()
		{
			try
			{
				int count = batchListView.Items.Count;
				for (int i = 0; i < count; i++)
				{
					this.Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.ProcessingStatusTextFormat });
					using(Bitmap temp = new Bitmap(pathArray[i]))
					{
						BitmapEntry item = new BitmapEntry();

						item.Bitmap = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), PixelFormat.Format24bppRgb);
						string alname = Path.Combine(Path.GetDirectoryName(pathArray[i]), Path.GetFileNameWithoutExtension(pathArray[i]) + "_a" + Path.GetExtension(pathArray[i]));
						if (File.Exists(alname))
						{
							using (Bitmap alpha = new Bitmap(alname))
							{
								item.Alpha = alpha.Clone(new Rectangle(0, 0, alpha.Width, alpha.Height), PixelFormat.Format24bppRgb);
							}
						   
						}
						else if (Path.GetExtension(pathArray[i]).Equals(".png",StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
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
						item.BmpType = (FshImageFormat)Enum.Parse(typeof(FshImageFormat), typeArray[i]);
						item.DirName = "FiSH";

						if (i <= batchFshList.Capacity)
						{
							FSHImageWrapper fsh = new FSHImageWrapper();
								
							fsh.Bitmaps.Add(item);
							batchFshList.Insert(i, new BatchFshContainer(fsh));

							using (MemoryStream mstream = new MemoryStream())
							{
								SaveFsh(mstream, batchFshList[i].MainImage);

								if (!fshWriteCompCb.Checked)
								{
									batchFshList[i].MainImage.SetRawData(mstream.ToArray());
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

		/// <summary>
		/// Processes the batch list for the command line.
		/// </summary>
		internal void ProcessBatchCmd()
		{ 
			int count = batchListView.Items.Count;
			try
			{
				for (int i = 0; i < count; i++)
				{
					using (Bitmap temp = new Bitmap(pathArray[i]))
					{
						BitmapEntry item = new BitmapEntry();

						item.Bitmap = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), PixelFormat.Format24bppRgb);
						string alname = Path.Combine(Path.GetDirectoryName(pathArray[i]), Path.GetFileNameWithoutExtension(pathArray[i]) + "_a" + Path.GetExtension(pathArray[i]));
						if (File.Exists(alname))
						{
							using (Bitmap alpha = new Bitmap(alname))
							{
								item.Alpha = alpha.Clone(new Rectangle(0, 0, alpha.Width, alpha.Height), PixelFormat.Format24bppRgb);
							}

						}
						else if (Path.GetExtension(pathArray[i]).Equals(".png", StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
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
						item.BmpType = (FshImageFormat)Enum.Parse(typeof(FshImageFormat), typeArray[i]);
						item.DirName = "FiSH";

						if (i <= batchFshList.Capacity)
						{
							FSHImageWrapper fsh = new FSHImageWrapper();

							fsh.Bitmaps.Add(item);
							batchFshList.Insert(i, new BatchFshContainer(fsh));

							using (MemoryStream mstream = new MemoryStream())
							{
								SaveFsh(mstream, batchFshList[i].MainImage);

								if (!fshWriteCompCb.Checked)
								{
									batchFshList[i].MainImage.SetRawData(mstream.ToArray());
								}
							}
						}
					}

				}	
				batch_processed = true;

			}
			catch (Exception)
			{  
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

		private TaskbarManager manager;
		private static JumpList jumpList;

		private void SetProgressBarValue(int value, string statusTextFormat)
		{            
			toolStripProgressBar1.PerformStep();			
			toolStripProgressStatus.Text = string.Format(CultureInfo.CurrentCulture, statusTextFormat, (value + 1), batchListView.Items.Count);

			if (manager != null)
			{
				manager.SetProgressValue(toolStripProgressBar1.Value, toolStripProgressBar1.Maximum, this.Handle);
			}
		}
	   
		private void ProcessBatchSaveFiles()
		{
			int itemCount = (int)this.Invoke(new Func<Int32>(delegate()
				{
					return batchListView.Items.Count;
				}));
			for (int c = 0; c < itemCount; c++)
			{
				string filepath;
				this.Invoke(new Action<int, string>(SetProgressBarValue), new object[] { c, Resources.SavingFshProgressTextFormat });
				BatchFshContainer batchFsh = batchFshList[c];
				if (batchFsh.MainImage != null)
				{
					filepath = GetFilePath(pathArray[c], string.Empty, outfolder);
					using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
					{
						SaveFsh(fstream, batchFsh.MainImage);
					}
					this.Invoke(new Action<string, int, int>(WriteTgi), new object[] { filepath, 4, c });
				}
				if (mipFormat == MipmapFormat.Normal)
				{
					if (batchFsh.Mip64Fsh != null)
					{
						filepath = GetFilePath(pathArray[c], "_s3", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip64Fsh);
						}
						this.Invoke(new Action<string, int, int>(WriteTgi), new object[] { filepath, 3, c });
					}
					if (batchFsh.Mip32Fsh != null)
					{
						filepath = GetFilePath(pathArray[c], "_s2", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip32Fsh);
						}
						this.Invoke(new Action<string, int, int>(WriteTgi), new object[] { filepath, 2, c });
					}
					if (batchFsh.Mip16Fsh != null)
					{
						filepath = GetFilePath(pathArray[c], "_s1", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip16Fsh);
						}
						this.Invoke(new Action<string, int, int>(WriteTgi), new object[] { filepath, 1, c });
					}
					if (batchFsh.Mip8Fsh != null)
					{
						filepath = GetFilePath(pathArray[c], "_s0", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip8Fsh);
						}
						this.Invoke(new Action<string, int, int>(WriteTgi), new object[] { filepath, 0, c });
					}
				}
			}
		}

		/// <summary>
		///  Saves the batch files from the command line.
		/// </summary>
		internal void ProcessBatchSaveFilesCmd()
		{
			int itemCount = batchListView.Items.Count;
			for (int i = 0; i < itemCount; i++)
			{
				string filepath;
				BatchFshContainer batchFsh = batchFshList[i];
				if (batchFsh.MainImage != null)
				{
					filepath = GetFilePath(pathArray[i], string.Empty, outfolder);
					using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
					{
						SaveFsh(fstream, batchFsh.MainImage);
					}
					this.WriteTgi(filepath, 4, i);
				}
				if (mipFormat == MipmapFormat.Normal)
				{
					if (batchFsh.Mip64Fsh != null)
					{
						filepath = GetFilePath(pathArray[i], "_s3", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip64Fsh);
						}
						this.WriteTgi(filepath, 3, i);
					}
					if (batchFsh.Mip32Fsh != null)
					{
						filepath = GetFilePath(pathArray[i], "_s2", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip32Fsh);
						}
						this.WriteTgi(filepath, 2, i);
					}
					if (batchFsh.Mip16Fsh != null)
					{
						filepath = GetFilePath(pathArray[i], "_s1", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip16Fsh);
						}
						this.WriteTgi(filepath, 1, i);
					}
					if (batchFsh.Mip8Fsh != null)
					{
						filepath = GetFilePath(pathArray[i], "_s0", outfolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip8Fsh);
						}
						this.WriteTgi(filepath, 0, i);
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

				if (mipFormat == MipmapFormat.Normal)
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
				MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				mipFormatCbo.SelectedIndex = int.Parse(settings.GetSetting("MipFormat", "0"), CultureInfo.InvariantCulture);
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
			Type t = Type.GetType("Mono.Runtime");
			if (t == null)
			{
				if (!IsProcessorFeaturePresent(SSE))
				{
					MessageBox.Show(Resources.FshWriteSSERequiredError, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					fshWriteCompCb.Enabled = fshWriteCompCb.Checked = false;
				} 
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
		private string RandomHexString(int length)
		{
			const string numbers = "0123456789";
			const string hexcode = "ABCDEF";
			char[] charArray = new char[length];
			string hexstring = string.Empty;

			hexstring += numbers;
			hexstring += hexcode;

			ReadRangetxt(rangepath, false);

			long lower = -1;
			long upper = -1;
			if (!string.IsNullOrEmpty(lowerinst) && !string.IsNullOrEmpty(upperinst))
			{
				lower = long.Parse(lowerinst, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				upper = long.Parse(upperinst, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}

			for (int c = 0; c < charArray.Length; c++)
			{
				int index;
				if (lower > 0 && upper > 0)
				{
					double id = (upper * 1.0 - lower * 1.0) * ra.NextDouble() + lower * 1.0;
					return Convert.ToInt64(id).ToString("X8", CultureInfo.InvariantCulture);
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
				int pathArrayCount = pathArray.Count;
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
									for (int i = 0; i < pathArrayCount; i++)
									{
										groupArray.Insert(i, g);
									}
								}
							}
						}
					}
					else
					{
						string errgrp = "1ABE787D";
						for (int i = 0; i < pathArrayCount; i++)
						{
							groupArray.Insert(i, errgrp);
						}
						string message = string.Format(CultureInfo.CurrentCulture, Resources.FileNotFoundFormat, Path.GetFileName(path), path);
						throw new FileNotFoundException(message);

					}
				}
				else
				{
					if (File.Exists(path))
					{ 
						string[] instTemp = null;
						using (StreamReader sr = new StreamReader(path))
						{
							string line;
							char[] splitchar = new char[] { ',' };
							while ((line = sr.ReadLine()) != null)
							{
								if (!string.IsNullOrEmpty(line))
								{
									instTemp = line.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
								}

							}
						}
						if (instTemp != null)
						{
							string inst0 = instTemp[0];
							string inst1 = instTemp[1];
  
							if (inst0.Length == 10 && inst0.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
							{
								lowerinst = inst0.Substring(2, 8);
							}
							else if (inst0.Length == 8)
							{
								lowerinst = inst0;
							}
							
							if (inst1.Length == 10 && inst1.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
							{
								upperinst = inst1.Substring(2, 8);
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

				if (str.Length == 10 && str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				{
					tmp = str.Substring(2, 8);
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
					link.Arguments = "\"" + path + "\"";
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
				foreach (var fileName in filenames)
				{
					if (fileName.StartsWith("/proc", StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith("/mips", StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith("/dat:", StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith("/outDir:", StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith("/?", StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith("/group:", StringComparison.OrdinalIgnoreCase))
					{
						continue; // skip the command line switches
					}
					if ((File.GetAttributes(fileName) & FileAttributes.Directory) == FileAttributes.Directory)
					{
						DirectoryInfo di = new DirectoryInfo(fileName);
						List<FileInfo> files = new List<FileInfo>();

						files.AddRange(di.GetFiles("*.png", SearchOption.TopDirectoryOnly));
						FileInfo[] bmparr = di.GetFiles("*.bmp", SearchOption.TopDirectoryOnly);
						if (bmparr.Length > 0)
						{
							files.AddRange(bmparr);
						}

						foreach (FileInfo item in files)
						{
							if (item.Name.Contains("_a"))
							{
								continue;
							}
								
							fcnt++;
						}                        
					  
					}
					else
					{
						FileInfo fi = new FileInfo(fileName);
						string ext = fi.Extension;
						if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
						{
							if (fi.Name.Contains("_a"))
							{
								continue;
							}

							fcnt++;
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
				instArray[index] = instArray[index].Substring(0, 7) + endreg;
			}
			else if (temp.Width == 64 && temp.Height == 64)
			{
				instArray[index] = instArray[index].Substring(0, 7) + end64;
			}
			else if (temp.Width == 32 && temp.Height == 32)
			{
				instArray[index] = instArray[index].Substring(0, 7) + end32;
			}
			else if (temp.Width == 16 && temp.Height == 16)
			{
				instArray[index] = instArray[index].Substring(0, 7) + end16;
			}
			else if (temp.Width == 8 && temp.Height == 8)
			{
				instArray[index] = instArray[index].Substring(0, 7) + end8;
			}
		}

		private void FormatRefresh(int index)
		{
			using (Bitmap temp = new Bitmap(pathArray[index]))
			{
				SetEndFormat(temp, index);
			}
			
			batchListView.SelectedItems[0].SubItems[3].Text = instArray[index];
			tgiInstanceTxt.Text = instArray[index];
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
				int pathArrayCount = pathArray.Count;
				if (tgiGroupTxt.Text.Length > 0 && tgiGroupTxt.Text.Length == 8)
				{
					for (int j = 0; j < pathArrayCount; j++)
					{
						groupArray.Insert(j, tgiGroupTxt.Text);
					}
				}
				else
				{
					ReadRangetxt(grppath, true);
				}


				for (int n = 0; n < pathArrayCount; n++)
				{
					if (Path.GetFileName(pathArray[n]).StartsWith("0x", StringComparison.OrdinalIgnoreCase))
					{
						string pathinst = Path.GetFileNameWithoutExtension(pathArray[n]).Substring(2, 8);
						instArray.Insert(n, pathinst);
					}
					else
					{
						instArray.Insert(n, RandomHexString(7));
					}
					using (Bitmap temp = new Bitmap(pathArray[n]))
					{
						if (!Path.GetFileName(pathArray[n]).StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							SetEndFormat(temp, n);
						}


						string alname = Path.Combine(Path.GetDirectoryName(pathArray[n]), Path.GetFileNameWithoutExtension(pathArray[n]) + "_a" + Path.GetExtension(pathArray[n]));
						string fn = Path.GetFileName(pathArray[n]);
						if (File.Exists(alname))
						{
							typeArray.Insert(n, FshImageFormat.DXT3.ToString());
						}
						else if (temp.PixelFormat == PixelFormat.Format32bppArgb)
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.ThirtyTwoBit.ToString());
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT3.ToString());
							}
						}
						else
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.TwentyFourBit.ToString());
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT1.ToString());
							}
						}
					}

				}

				for (int i = 0; i < pathArrayCount; i++)
				{
					if (File.Exists(pathArray[i]))
					{
						ListViewItem item1 = new ListViewItem(Path.GetFileName(pathArray[i]));
						Alphasrc(item1, pathArray[i]);
						item1.SubItems.Add(groupArray[i]);
						item1.SubItems.Add(instArray[i]);
						batchListView.Items.Add(item1);
					}
				}
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
					groupArray.RemoveAt(index);
					instArray.RemoveAt(index);
					typeArray.RemoveAt(index);
					pathArray.RemoveAt(index);
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
				int pathArrayCount = pathArray.Count;

				if (tgiGroupTxt.Text.Length > 0 && tgiGroupTxt.Text.Length == 8)
				{
					for (int j = 0; j < pathArrayCount; j++)
					{
						groupArray.Insert(j, tgiGroupTxt.Text);
					}
				}
				else
				{
					ReadRangetxt(grppath, true);
				}

				for (int n = dif; n < pathArrayCount; n++)
				{
					if (Path.GetFileName(pathArray[n]).StartsWith("0x", StringComparison.OrdinalIgnoreCase))
					{
						string pathinst = Path.GetFileNameWithoutExtension(pathArray[n]).Substring(2, 8);
						instArray.Insert(n, pathinst);
					}
					else
					{
						instArray.Insert(n, RandomHexString(7));
					}

					using (Bitmap temp = new Bitmap(pathArray[n]))
					{
						if (!Path.GetFileName(pathArray[n]).StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							SetEndFormat(temp, n);
						}

						string alname = Path.Combine(Path.GetDirectoryName(pathArray[n]), Path.GetFileNameWithoutExtension(pathArray[n]) + "_a" + Path.GetExtension(pathArray[n]));
						string fn = Path.GetFileName(pathArray[n]);
						if (File.Exists(alname))
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.ThirtyTwoBit.ToString());
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT3.ToString());
							}
						}
						else if (Path.GetExtension(pathArray[n]).Equals(".png",StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.ThirtyTwoBit.ToString());
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT3.ToString());
							}
						}
						else
						{
							if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.TwentyFourBit.ToString());
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT1.ToString());
							}
						}
					}
				}

				for (int i = dif; i < pathArrayCount; i++)
				{
					if (File.Exists(pathArray[i]))
					{
						ListViewItem item1 = new ListViewItem(Path.GetFileName(pathArray[i]));
						Alphasrc(item1, pathArray[i]);
						item1.SubItems.Add(groupArray[i]);
						item1.SubItems.Add(instArray[i]);
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
				if (pathArray == null && instArray == null && groupArray == null && typeArray == null)
				{
					pathArray = new List<string>(fcnt);
					instArray = new List<string>(fcnt);
					groupArray = new List<string>(fcnt);
					typeArray = new List<string>(fcnt);
				}
				if (pathArray.Count > 0)
				{
					pathcnt = pathArray.Count;
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
				pathArray.SetCapacity(cnt);
				groupArray.SetCapacity(cnt);
				instArray.SetCapacity(cnt);
				typeArray.SetCapacity(cnt);
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
							pathArray.Add(PngopenDialog.FileNames[f]);
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
					if (!tgiGroupTxt.Text.Equals(groupArray[index], StringComparison.OrdinalIgnoreCase))
					{
						groupArray[index] = tgiGroupTxt.Text;
						batchListView.SelectedItems[0].SubItems[2].Text = groupArray[index];
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
					if (!tgiInstanceTxt.Text.Equals(instArray[index], StringComparison.OrdinalIgnoreCase))
					{
						instArray.Insert(index, tgiInstanceTxt.Text);
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
				switch (fshTypeBox.SelectedIndex)
				{
					case 0:
						seltype = FshImageFormat.TwentyFourBit.ToString();
						break;
					case 1:
						seltype = FshImageFormat.ThirtyTwoBit.ToString();
						break;
					case 2:
						seltype = FshImageFormat.DXT1.ToString();
						break;
					case 3:
						seltype = FshImageFormat.DXT3.ToString();
						break;
				}
				if (!seltype.Equals(typeArray[index], StringComparison.Ordinal))
				{
					typeArray[index] = seltype;
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
			List<string> flist = new List<string>();

			for (int f = 0; f < filenames.Length; f++)
			{
				if ((File.GetAttributes(filenames[f]) & FileAttributes.Directory) == FileAttributes.Directory)
				{
					DirectoryInfo di = new DirectoryInfo(filenames[f]);
					List<FileInfo> fa = new List<FileInfo>();

					fa.AddRange(di.GetFiles("*.png", SearchOption.TopDirectoryOnly));
					FileInfo[] bmparr = di.GetFiles("*.bmp", SearchOption.TopDirectoryOnly);
					if (bmparr.Length > 0)
					{
						fa.AddRange(bmparr);
					}
					int count = fa.Count;
					for (int i = 0; i < count; i++)
					{
						FileInfo fi = fa[i];
						string ext = fi.Extension;
						if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
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
					string ext = fi.Extension;
					if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
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
			return flist.ToArray();
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
			pathArray = new List<string>(fcnt);
			instArray = new List<string>(fcnt);
			groupArray = new List<string>(fcnt);
			typeArray = new List<string>(fcnt);
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
							pathArray.Insert(pngcnt, files[f]);
						}
					}
				}
			}
			BuildPngList();
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
			batchListView.Items.Clear();
			if (pathArray != null && instArray != null && groupArray != null && typeArray != null)
			{
				pathArray.Clear();
				pathArray = null;
				instArray.Clear();
				instArray = null;
				groupArray.Clear();
				groupArray = null;
				typeArray.Clear();
				typeArray = null;
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
			if (pathArray == null && instArray == null && groupArray == null && typeArray == null)
			{
				pathArray = new List<string>(fcnt);
				instArray = new List<string>(fcnt);
				groupArray = new List<string>(fcnt);
				typeArray = new List<string>(fcnt);
			}
			if (pathArray.Count > 0)
			{
				pathcnt = pathArray.Count;
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
			pathArray.SetCapacity(count);
			groupArray.SetCapacity(count);
			instArray.SetCapacity(count);
			typeArray.SetCapacity(count);
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
								pathArray.Add(files[f]);
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
			using (Bitmap b = new Bitmap(pathArray[index]))
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

		private MipmapFormat mipFormat;
		private void mipFormatCbo_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.mipFormat = (MipmapFormat)mipFormatCbo.SelectedIndex;

			if (settings != null)
			{
				settings.PutSetting("MipFormat", mipFormatCbo.SelectedIndex.ToString(CultureInfo.InvariantCulture));
			}
		}
	}
}
