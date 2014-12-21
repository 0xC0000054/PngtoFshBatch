using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using FshDatIO;
using Microsoft.WindowsAPICodePack.Taskbar;
using PngtoFshBatchtxt.Properties;

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
		internal bool mipsBuilt = false;

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

							if (item.BmpType == FshImageFormat.DXT3 || item.BmpType == FshImageFormat.ThirtyTwoBit)
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

			mipsBuilt = true;
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
		internal List<FshImageFormat> typeArray = null;
		internal List<string> groupArray = null;
		private static void Alphasrc(ListViewItem item, string path)
		{
			string alname = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "_a" + Path.GetExtension(path));
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
				FshImageFormat type = typeArray[index];
				switch (type)
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
		}

		private ListViewItem[] GetBatchListViewItems()
		{
			ListViewItem[] items = new ListViewItem[batchListView.Items.Count];
			batchListView.Items.CopyTo(items, 0);
			return items;
		}

		internal DatFile dat = null;
		private Thread datRebuildThread = null;
		private bool datRebuilt = false;

		private void RebuildDat(DatFile inputdat)
		{
			ListViewItem[] items = (ListViewItem[])base.Invoke(new Func<ListViewItem[]>(GetBatchListViewItems));
			if (mipsBuilt)
			{
				for (int i = 0; i < items.Length; i++)
				{
					this.Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.BuildingDatStatusTextFormat });

					BatchFshContainer batchFsh = batchFshList[i];
					ListViewItem item = items[i];
					uint group = uint.Parse(item.SubItems[2].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					uint[] instanceid = new uint[5];
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
					instanceid[4] = uint.Parse(sub + endreg, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					bool useFshWrite = this.fshWriteCompCb.Checked;
					bool compress = this.compDatCb.Checked;
					for (int j = 4; j >= 0; j--)
					{
						if (fshimg[j] != null)
						{
							CheckInstance(inputdat, group, instanceid[j]);

							inputdat.Add(new FshFileItem(fshimg[j], useFshWrite), group, instanceid[j], compress);
						}
					}


				}
			}
			else if (batchFshList != null)
			{
				for (int i = 0; i < items.Length; i++)
				{
					this.Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.BuildingDatStatusTextFormat });

					ListViewItem item = items[i];

					uint group = uint.Parse(item.SubItems[2].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					SetInstanceEndChars(i);
					string inst = item.SubItems[3].Text.Substring(0, 7) + endreg;
					uint instanceID = uint.Parse(inst, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					FSHImageWrapper mainImage = batchFshList[i].MainImage;
					if (mainImage != null)
					{
						if (mipFormat == MipmapFormat.Embedded)
						{
							mainImage.Bitmaps[0].CalculateMipmapCount();
						}

						CheckInstance(inputdat, group, instanceID);

						inputdat.Add(new FshFileItem(mainImage, this.fshWriteCompCb.Checked), group, instanceID, this.compDatCb.Checked);
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
			var items = this.batchListView.Items;
			if (mipsBuilt)
			{
				uint[] instanceid = new uint[5];
				FSHImageWrapper[] fshimg = new FSHImageWrapper[5];
				for (int i = 0; i < items.Count; i++)
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

					fshimg[4] = batchFsh.MainImage;

					SetInstanceEndChars(i);

					instanceid[0] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end8, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[1] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end16, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[2] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end32, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[3] = uint.Parse(item.SubItems[3].Text.Substring(0, 7) + end64, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					instanceid[4] = uint.Parse(item.SubItems[3].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					bool useFshWrite = this.fshWriteCompCb.Checked;

					for (int j = 4; j >= 0; j--)
					{
						if (fshimg[j] != null)
						{
							CheckInstance(inputdat, group, instanceid[j]);

							inputdat.Add(new FshFileItem(fshimg[j], useFshWrite), group, instanceid[j], true);
						}
					}


				}
			}
			else if (batchFshList != null)
			{
				bool useFshWrite = this.fshWriteCompCb.Checked;

				for (int i = 0; i < items.Count; i++)
				{
					ListViewItem item = items[i];

					uint group = uint.Parse(item.SubItems[2].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					uint instanceid = uint.Parse(item.SubItems[3].Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

					FSHImageWrapper mainImage = this.batchFshList[i].MainImage;
					if (mainImage != null)
					{
						if (this.mipFormat == MipmapFormat.Embedded)
						{
							mainImage.Bitmaps[0].CalculateMipmapCount();
						}
						CheckInstance(inputdat, group, instanceid);

						inputdat.Add(new FshFileItem(mainImage, useFshWrite), group, instanceid, true);
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
			for (int i = 0; i < count; i++)
			{
				DatIndex index = checkdat.Indexes[i];
				if (index.Type == 0x7ab50e44U && index.Group == group && index.IndexState == DatIndexState.None)
				{
					if (index.Instance == instance)
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

						if (!batchProcessed)
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

		internal bool batchProcessed = false;
		private void ProcessBatch()
		{
			try
			{
				int count = batchListView.Items.Count;
				for (int i = 0; i < count; i++)
				{
					this.Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.ProcessingStatusTextFormat });
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
						item.BmpType = typeArray[i];
						item.DirName = "FiSH";

						if (i <= batchFshList.Capacity)
						{
							FSHImageWrapper fsh = new FSHImageWrapper();

							fsh.Bitmaps.Add(item);
							batchFshList.Insert(i, new BatchFshContainer(fsh));

							using (MemoryStream mstream = new MemoryStream())
							{
								SaveFsh(mstream, batchFshList[i].MainImage);
							}
						}
					}

				}
				batchProcessed = true;
			}
			catch (ArgumentOutOfRangeException ag)
			{
				MessageBox.Show(this, ag.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception)
			{
				batchProcessed = false;
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
						item.BmpType = typeArray[i];
						item.DirName = "FiSH";

						if (i <= batchFshList.Capacity)
						{
							FSHImageWrapper fsh = new FSHImageWrapper();

							fsh.Bitmaps.Add(item);
							batchFshList.Insert(i, new BatchFshContainer(fsh));

							using (MemoryStream mstream = new MemoryStream())
							{
								SaveFsh(mstream, batchFshList[i].MainImage);
							}
						}
					}

				}
				batchProcessed = true;

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
			this.toolStripProgressBar1.PerformStep();
			this.toolStripProgressStatus.Text = string.Format(CultureInfo.CurrentCulture, statusTextFormat, (value + 1), this.batchListView.Items.Count);

			if (this.manager != null)
			{
				this.manager.SetProgressValue(this.toolStripProgressBar1.Value, this.toolStripProgressBar1.Maximum, this.Handle);
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
					if (this.mipFormat == MipmapFormat.Embedded)
					{
						batchFsh.MainImage.Bitmaps[0].CalculateMipmapCount();
					}

					filepath = GetFilePath(pathArray[c], string.Empty, outputFolder);
					using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
					{
						SaveFsh(fstream, batchFsh.MainImage);
					}
					this.Invoke(new Action<string, int, int>(WriteTgi), new object[] { filepath, 4, c });
				}
				if (this.mipFormat == MipmapFormat.Normal)
				{
					if (batchFsh.Mip64Fsh != null)
					{
						filepath = GetFilePath(pathArray[c], "_s3", outputFolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip64Fsh);
						}
						this.Invoke(new Action<string, int, int>(WriteTgi), new object[] { filepath, 3, c });
					}
					if (batchFsh.Mip32Fsh != null)
					{
						filepath = GetFilePath(pathArray[c], "_s2", outputFolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip32Fsh);
						}
						this.Invoke(new Action<string, int, int>(WriteTgi), new object[] { filepath, 2, c });
					}
					if (batchFsh.Mip16Fsh != null)
					{
						filepath = GetFilePath(pathArray[c], "_s1", outputFolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip16Fsh);
						}
						this.Invoke(new Action<string, int, int>(WriteTgi), new object[] { filepath, 1, c });
					}
					if (batchFsh.Mip8Fsh != null)
					{
						filepath = GetFilePath(pathArray[c], "_s0", outputFolder);
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
					filepath = GetFilePath(pathArray[i], string.Empty, outputFolder);
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
						filepath = GetFilePath(pathArray[i], "_s3", outputFolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip64Fsh);
						}
						this.WriteTgi(filepath, 3, i);
					}
					if (batchFsh.Mip32Fsh != null)
					{
						filepath = GetFilePath(pathArray[i], "_s2", outputFolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip32Fsh);
						}
						this.WriteTgi(filepath, 2, i);
					}
					if (batchFsh.Mip16Fsh != null)
					{
						filepath = GetFilePath(pathArray[i], "_s1", outputFolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip16Fsh);
						}
						this.WriteTgi(filepath, 1, i);
					}
					if (batchFsh.Mip8Fsh != null)
					{
						filepath = GetFilePath(pathArray[i], "_s0", outputFolder);
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

				if (!batchProcessed)
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
				MessageBox.Show(this, ag.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private Settings settings = null;
		internal string groupPath = null;
		internal string rangePath = null;
		private void LoadSettings()
		{
			settings = new Settings(Path.Combine(Application.StartupPath, @"PngtoFshBatch.xml"));
			compDatCb.Checked = bool.Parse(settings.GetSetting("compDatcb_checked", bool.TrueString));
			mipFormatCbo.SelectedIndex = int.Parse(settings.GetSetting("MipFormat", "0"), CultureInfo.InvariantCulture);
			fshWriteCompCb.Checked = bool.Parse(settings.GetSetting("fshwritecompcb_checked", bool.FalseString));
		}
		private const uint SSE = 6;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsProcessorFeaturePresent(uint ProcessorFeature);

		private void CheckForSSE()
		{
			if (Type.GetType("Mono.Runtime") == null)
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
			groupPath = Path.Combine(Application.StartupPath, @"Groupid.txt");
			rangePath = Path.Combine(Application.StartupPath, @"instRange.txt");
			CheckRangeFilesExist(rangePath);
			CheckRangeFilesExist(groupPath);
		}

		private Random ra = new Random();
		private string lowerInstRange = null;
		private string upperInstRange = null;
		private string RandomHexString(int length)
		{
			if (string.IsNullOrEmpty(lowerInstRange) && string.IsNullOrEmpty(upperInstRange))
			{
				ReadRangetxt(rangePath);
			}

			if (!string.IsNullOrEmpty(lowerInstRange) && !string.IsNullOrEmpty(upperInstRange))
			{
				long lower, upper;

				if (long.TryParse(lowerInstRange, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out lower) &&
					long.TryParse(upperInstRange, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out upper))
				{
					double rn = (upper * 1.0 - lower * 1.0) * ra.NextDouble() + lower * 1.0;

					return Convert.ToInt64(rn).ToString("X").Substring(0, 7);
				}
			}

			byte[] buffer = new byte[length / 2];
			ra.NextBytes(buffer);
			string result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
			if ((length % 2) == 0)
				return result;

			return result + ra.Next(16).ToString("X");
		}

		private void compDatcb_CheckedChanged(object sender, EventArgs e)
		{
			if (settings != null)
			{
				settings.PutSetting("compDatcb_checked", compDatCb.Checked.ToString());
			}
		}


		internal string outputFolder = string.Empty;
		private void outfolderbtn_Click(object sender, EventArgs e)
		{
			if (OutputBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				outputFolder = OutputBrowserDialog1.SelectedPath;
			}
		}

		private void ReadGrouptxt(string path)
		{
			int pathArrayCount = pathArray.Count;

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
				for (int i = 0; i < pathArrayCount; i++)
				{
					groupArray.Insert(i, "1ABE787D");
				}
				string message = string.Format(CultureInfo.CurrentCulture, Resources.FileNotFoundFormat, Path.GetFileName(path), path);
				throw new FileNotFoundException(message);
			}
		}

		private void ReadRangetxt(string path)
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
				if ((instTemp != null) && instTemp.Length == 2)
				{
					string inst0 = instTemp[0];
					string inst1 = instTemp[1];

					if (inst0.Length == 10 && inst0.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
					{
						lowerInstRange = inst0.Substring(2, 8);
					}
					else if (inst0.Length == 8)
					{
						lowerInstRange = inst0;
					}

					if (inst1.Length == 10 && inst1.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
					{
						upperInstRange = inst1.Substring(2, 8);
					}
					else if (inst1.Length == 8)
					{
						upperInstRange = inst1;
					}
				}

			}
			else
			{
				lowerInstRange = string.Empty;
				upperInstRange = string.Empty;
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

		private void SetEndFormat(Bitmap temp, int index)
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
				if (tgiGroupTxt.Text.Length == 8)
				{
					for (int j = 0; j < pathArrayCount; j++)
					{
						groupArray.Insert(j, tgiGroupTxt.Text);
					}
				}
				else
				{
					ReadGrouptxt(groupPath);
				}

				for (int n = 0; n < pathArrayCount; n++)
				{
					string fileName = Path.GetFileNameWithoutExtension(pathArray[n]);

					using (Bitmap temp = new Bitmap(pathArray[n]))
					{
						if (fileName.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							if (!ValidateHexString(fileName))
							{
								throw new FormatException(string.Format(Resources.InvalidInstanceIDFormat, fileName));
							}

							string pathinst = fileName.Substring(2, 8);
							instArray.Insert(n, pathinst);
						}
						else
						{
							instArray.Insert(n, RandomHexString(7));
							SetEndFormat(temp, n);
						}


						string alphaName = Path.Combine(Path.GetDirectoryName(pathArray[n]), fileName + "_a" + Path.GetExtension(pathArray[n]));
						if (File.Exists(alphaName))
						{
							typeArray.Insert(n, FshImageFormat.DXT3);
						}
						else if (temp.PixelFormat == PixelFormat.Format32bppArgb)
						{
							if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.ThirtyTwoBit);
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT3);
							}
						}
						else
						{
							if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.TwentyFourBit);
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT1);
							}
						}
					}

				}

				for (int i = 0; i < pathArrayCount; i++)
				{
					ListViewItem item1 = new ListViewItem(Path.GetFileName(pathArray[i]));
					Alphasrc(item1, pathArray[i]);
					item1.SubItems.Add(groupArray[i]);
					item1.SubItems.Add(instArray[i]);
					batchListView.Items.Add(item1);
				}
			}
			catch (FileNotFoundException fx)
			{
				MessageBox.Show(fx.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (FormatException ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (IOException ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		private void remBtn_Click(object sender, EventArgs e)
		{
			if (batchListView.SelectedItems.Count > 0 && batchListView.Items.Count > 1)
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
			else if (batchListView.SelectedItems.Count > 0 && batchListView.Items.Count == 1)
			{
				ClearandReset();
			}

		}

		private void BuildAddList(int startIndex)
		{
			try
			{
				int pathArrayCount = pathArray.Count;

				if (tgiGroupTxt.Text.Length == 8)
				{
					for (int j = 0; j < pathArrayCount; j++)
					{
						groupArray.Insert(j, tgiGroupTxt.Text);
					}
				}
				else
				{
					ReadGrouptxt(groupPath);
				}

				for (int n = startIndex; n < pathArrayCount; n++)
				{
					using (Bitmap temp = new Bitmap(pathArray[n]))
					{
						string fileName = Path.GetFileNameWithoutExtension(pathArray[n]);

						if (fileName.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							if (!ValidateHexString(fileName))
							{
								throw new FormatException(string.Format(Resources.InvalidInstanceIDFormat, fileName));
							}

							string pathinst = fileName.Substring(2, 8);
							instArray.Insert(n, pathinst);
						}
						else
						{
							instArray.Insert(n, RandomHexString(7));
							SetEndFormat(temp, n);
						}

						string alphaName = Path.Combine(Path.GetDirectoryName(pathArray[n]), fileName + "_a" + Path.GetExtension(pathArray[n]));
						if (File.Exists(alphaName))
						{
							if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.ThirtyTwoBit);
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT3);
							}
						}
						else if (Path.GetExtension(pathArray[n]).Equals(".png", StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
						{
							if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.ThirtyTwoBit);
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT3);
							}
						}
						else
						{
							if (fileName.StartsWith("hd", StringComparison.OrdinalIgnoreCase))
							{
								typeArray.Insert(n, FshImageFormat.TwentyFourBit);
							}
							else
							{
								typeArray.Insert(n, FshImageFormat.DXT1);
							}
						}
					}
				}

				for (int i = startIndex; i < pathArrayCount; i++)
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
				MessageBox.Show(fx.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (FormatException ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (IndexOutOfRangeException ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (IOException ex)
			{
				MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}
		private void addBtn_Click(object sender, EventArgs e)
		{
			if (PngopenDialog.ShowDialog() == DialogResult.OK)
			{
				// remove the alpha mask images from the file
				string[] files = PngopenDialog.FileNames.Where(f => !Path.GetFileName(f).Contains("_a")).ToArray();

				int fileCount = files.Length;
				if (fileCount > 0)
				{
					int totalCount, existingFileCount;

					if (pathArray == null && instArray == null && groupArray == null && typeArray == null)
					{
						pathArray = new List<string>(fileCount);
						instArray = new List<string>(fileCount);
						groupArray = new List<string>(fileCount);
						typeArray = new List<FshImageFormat>(fileCount);
					}

					if (pathArray.Count > 0)
					{
						existingFileCount = pathArray.Count;
						totalCount = fileCount + existingFileCount;
					}
					else
					{
						existingFileCount = 0;
						totalCount = fileCount;
					}

					if (batchFshList == null)
					{
						batchFshList = new List<BatchFshContainer>(fileCount);
					}
					batchFshList.SetCapacity(totalCount);
					pathArray.SetCapacity(totalCount);
					groupArray.SetCapacity(totalCount);
					instArray.SetCapacity(totalCount);
					typeArray.SetCapacity(totalCount);

					pathArray.AddRange(files);

					int startIndex = 0;
					if (existingFileCount != 0)
					{
						startIndex = totalCount - fileCount;
					}

					BuildAddList(startIndex);

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
				if (selectedFormat != typeArray[index])
				{
					typeArray[index] = selectedFormat;
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
					FileInfo[] bmparr = di.GetFiles("*.bmp", SearchOption.TopDirectoryOnly);
					if (bmparr.Length > 0)
					{
						infos.AddRange(bmparr);
					}
					int count = infos.Count;
					for (int i = 0; i < count; i++)
					{
						FileInfo fi = infos[i];
						string ext = fi.Extension;
						if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
						{
							if (!fi.Name.Contains("_a"))
							{
								fileList.Add(fi.FullName);
							}
							else
							{
								continue;
							}
						}
					}

					AddRecentFolder(di.FullName);
				}
				else
				{
					string ext = Path.GetExtension(file);
					if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
					{
						if (!Path.GetFileName(file).Contains("_a"))
						{
							fileList.Add(file);
						}
						else
						{
							continue;
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
			int pngcnt = -1;
			pathArray = new List<string>(fileCount);
			instArray = new List<string>(fileCount);
			groupArray = new List<string>(fileCount);
			typeArray = new List<FshImageFormat>(fileCount);
			batchFshList = new List<BatchFshContainer>(fileCount);

			foreach (var file in files)
			{
				if (File.Exists(file))
				{
					pngcnt++;
					pathArray.Insert(pngcnt, file);
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
			if (outputFolder != null)
			{
				outputFolder = null;
			}
			mipsBuilt = false;
			batchProcessed = false;
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
			string[] files = GetFilesfromDirectory(e.Data.GetData(DataFormats.FileDrop) as string[]);
			int fileCount = files.Length;
			int totalCount, existingFileCount;

			if (pathArray == null && instArray == null && groupArray == null && typeArray == null)
			{
				pathArray = new List<string>(fileCount);
				instArray = new List<string>(fileCount);
				groupArray = new List<string>(fileCount);
				typeArray = new List<FshImageFormat>(fileCount);
			}

			if (pathArray.Count > 0)
			{
				existingFileCount = pathArray.Count;
				totalCount = fileCount + existingFileCount;
			}
			else
			{
				existingFileCount = 0;
				totalCount = fileCount;
			}

			if (batchFshList == null)
			{
				batchFshList = new List<BatchFshContainer>(fileCount);
			}
			batchFshList.SetCapacity(totalCount);
			pathArray.SetCapacity(totalCount);
			groupArray.SetCapacity(totalCount);
			instArray.SetCapacity(totalCount);
			typeArray.SetCapacity(totalCount);

			foreach (var file in files)
			{
				if (File.Exists(file))
				{
					pathArray.Add(file);
				}
			}

			int startIndex = 0;
			if (existingFileCount != 0)
			{
				startIndex = totalCount - fileCount;
			}

			BuildAddList(startIndex);

		}

		private void clearlistbtn_Click(object sender, EventArgs e)
		{
			ClearandReset();
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
