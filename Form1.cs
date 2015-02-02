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
		private TaskbarManager manager;
		private JumpList jumpList;
		private Settings settings;
		private char endreg;
		private char end64;
		private char end32;
		private char end16;
		private char end8;
		private Thread batchProcessThread;
		private Thread mipProcessThread;
		private Thread batchSaveFilesThread;
		private Thread datRebuildThread;		
		private bool batchProcessed;
		private bool mipsBuilt;
		private bool datRebuilt;
		private Random random;
		private Nullable<long> lowerInstRange;
		private Nullable<long> upperInstRange;
		private MipmapFormat mipFormat;

		internal BatchFshCollection batchFshList;
		internal string outputFolder;
		internal string groupPath;
		internal string rangePath;
		internal DatFile dat;
		internal bool displayProgress;

		private static class NativeMethods
		{
			internal const uint SSE = 6;

			[DllImport("kernel32.dll", ExactSpelling = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool IsProcessorFeaturePresent(uint ProcessorFeature);
		} 

		public Form1()
		{
			InitializeComponent();

			this.displayProgress = true;
			if (Type.GetType("Mono.Runtime") == null) // skip the Windows 7 code if we are on mono 
			{
				if (TaskbarManager.IsPlatformSupported)
				{
					this.manager = TaskbarManager.Instance;
					this.manager.ApplicationId = "PngtoFshBatch";
				}
			}
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
					Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.ProcessingMipsStatusTextFormat });
				}
				BatchFshContainer batchFsh = batchFshList[i];
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
		/// <param name="index">The index in the instArray to compare to.</param>
		private void SetInstanceEndChars(string instance)
		{
			if (instance.EndsWith("4", StringComparison.Ordinal) || instance.EndsWith("3", StringComparison.Ordinal)
						   || instance.EndsWith("2", StringComparison.Ordinal) || instance.EndsWith("1", StringComparison.Ordinal) || instance.EndsWith("0", StringComparison.Ordinal))
			{
				endreg = '4';
				end64 = '3';
				end32 = '2';
				end16 = '1';
				end8 = '0';
			}
			else if (instance.EndsWith("9", StringComparison.Ordinal) || instance.EndsWith("8", StringComparison.Ordinal)
				|| instance.EndsWith("7", StringComparison.Ordinal) || instance.EndsWith("6", StringComparison.Ordinal) || instance.EndsWith("5", StringComparison.Ordinal))
			{
				endreg = '9';
				end64 = '8';
				end32 = '7';
				end16 = '6';
				end8 = '5';
			}
			else if (instance.EndsWith("E", StringComparison.Ordinal) || instance.EndsWith("D", StringComparison.Ordinal)
				|| instance.EndsWith("C", StringComparison.Ordinal) || instance.EndsWith("B", StringComparison.Ordinal) || instance.EndsWith("A", StringComparison.Ordinal))
			{
				endreg = 'E';
				end64 = 'D';
				end32 = 'C';
				end16 = 'B';
				end8 = 'A';
			}
		}

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
					sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:X8}", batchFshList[index].GroupId + "\n"));

					string instance = batchFshList[index].InstanceId;
					SetInstanceEndChars(instance);

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

		private static string GetAlphaSourceString(string path)
		{
			string alname = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "_a" + Path.GetExtension(path));
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
			if (batchListView.SelectedItems.Count > 0)
			{				
				int index = batchListView.SelectedItems[0].Index;
				BatchFshContainer batch = batchFshList[index];

				string inst = batch.InstanceId;
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
		}

		internal void RebuildDat(DatFile inputdat)
		{
			if (mipsBuilt)
			{
				for (int i = 0; i < batchFshList.Count; i++)
				{
					if (displayProgress)
					{
						Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.BuildingDatStatusTextFormat });
					}
					BatchFshContainer batchFsh = batchFshList[i];
					uint group = uint.Parse(batchFsh.GroupId, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
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

					SetInstanceEndChars(batchFsh.InstanceId);

					string sub = batchFsh.InstanceId.Substring(0, 7);
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
				for (int i = 0; i < batchFshList.Count; i++)
				{
					if (displayProgress)
					{
						Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.BuildingDatStatusTextFormat });
					}
					BatchFshContainer batchFsh = batchFshList[i];
					uint group = uint.Parse(batchFsh.GroupId, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					string inst = batchFsh.InstanceId;
					SetInstanceEndChars(inst);
					uint instanceID = uint.Parse(inst.Substring(0, 7) + endreg, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

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

			if (displayProgress)
			{
				BeginInvoke(new MethodInvoker(delegate()
				{
					this.toolStripProgressStatus.Text = Resources.SavingDatStatusText;
				}));
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
						SetControlsEnabled(false);

						try
						{
							if (!batchProcessed)
							{
								SetProgressBarMaximum();
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
								dat.Save(saveDatDialog1.FileName);
							}
						}
						catch (Exception)
						{
							throw;
						}
						finally
						{                                    
							dat.Close();
							dat = null;
							SetControlsEnabled(true);

							ClearandReset();
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

		internal void ProcessBatch()
		{
			try
			{
				int count = batchFshList.Count;
				for (int i = 0; i < count; i++)
				{
					if (displayProgress)
					{
						Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.ProcessingStatusTextFormat });
					}
					BatchFshContainer batch = batchFshList[i];
					string fileName = batch.FileName;
					using (Bitmap temp = new Bitmap(fileName))
					{
						BitmapEntry item = new BitmapEntry();

						item.Bitmap = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), PixelFormat.Format24bppRgb);
						string alname = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + "_a" + Path.GetExtension(fileName));
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
						
						FSHImageWrapper fsh = new FSHImageWrapper();
						fsh.Bitmaps.Add(item);
							
						batch.MainImage = fsh;
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
			this.toolStripProgressBar1.PerformStep();
			this.toolStripProgressStatus.Text = string.Format(CultureInfo.CurrentCulture, statusTextFormat, (value + 1), this.batchListView.Items.Count);

			if (this.manager != null)
			{
				this.manager.SetProgressValue(this.toolStripProgressBar1.Value, this.toolStripProgressBar1.Maximum, this.Handle);
			}
		}

		internal void ProcessBatchSaveFiles()
		{
			for (int i = 0; i < batchFshList.Count; i++)
			{
				if (displayProgress)
				{
					Invoke(new Action<int, string>(SetProgressBarValue), new object[] { i, Resources.SavingFshProgressTextFormat });
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
					WriteTgi(filepath, 4, i);
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
						WriteTgi(filepath, 3, i);
					}
					if (batchFsh.Mip32Fsh != null)
					{
						filepath = GetFilePath(fileName, "_s2", outputFolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip32Fsh);
						}
						WriteTgi(filepath, 2, i);
					}
					if (batchFsh.Mip16Fsh != null)
					{
						filepath = GetFilePath(fileName, "_s1", outputFolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip16Fsh);
						}
						WriteTgi(filepath, 1, i);
					}
					if (batchFsh.Mip8Fsh != null)
					{
						filepath = GetFilePath(fileName, "_s0", outputFolder);
						using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
						{
							SaveFsh(fstream, batchFsh.Mip8Fsh);
						}
						WriteTgi(filepath, 0, i);
					}
				}
			}
		}

		private void processbatchbtn_Click(object sender, EventArgs e)
		{
			try
			{
				SetProgressBarMaximum();
				this.Cursor = Cursors.WaitCursor;
				SetControlsEnabled(false);

				if (!batchProcessed)
				{
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
			}
			catch (ArgumentOutOfRangeException ag)
			{
				MessageBox.Show(this, ag.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{ 
				if (this.Cursor != Cursors.Default)
				{
					this.Cursor = Cursors.Default;
				}
				SetControlsEnabled(true);
				ClearandReset();
			}
		}

		private void LoadSettings()
		{
			settings = new Settings(Path.Combine(Application.StartupPath, @"PngtoFshBatch.xml"));
			compDatCb.Checked = bool.Parse(settings.GetSetting("compDatcb_checked", bool.TrueString));
			mipFormatCbo.SelectedIndex = int.Parse(settings.GetSetting("MipFormat", "0"), CultureInfo.InvariantCulture);
			fshWriteCompCb.Checked = bool.Parse(settings.GetSetting("fshwritecompcb_checked", bool.FalseString));
		}

		private void CheckForSSE()
		{
			if (Type.GetType("Mono.Runtime") == null)
			{
				if (!NativeMethods.IsProcessorFeaturePresent(NativeMethods.SSE))
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

		private string RandomHexString(int length)
		{
			if (random == null)
			{
				random = new Random();
				ReadRangetxt(rangePath);
			}

			if (lowerInstRange.HasValue && upperInstRange.HasValue)
			{
				long lower = lowerInstRange.Value;
				long upper = upperInstRange.Value;

				double rn = (upper * 1.0 - lower * 1.0) * random.NextDouble() + lower * 1.0;

				return Convert.ToInt64(rn).ToString("X").Substring(0, 7);
			}

			byte[] buffer = new byte[length / 2];
			random.NextBytes(buffer);
			string result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
			if ((length % 2) == 0)
				return result;

			return result + random.Next(16).ToString("X");
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

		internal void ReadGrouptxt(string path)
		{
			int count = batchFshList.Count;

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
							for (int i = 0; i < count; i++)
							{
								batchFshList[i].GroupId = g;
							}
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					batchFshList[i].GroupId = "1ABE787D";
				}
			}
		}

		private void ReadRangetxt(string path)
		{
			if (File.Exists(path))
			{
				string[] instArray = null;
				using (StreamReader sr = new StreamReader(path))
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
						throw new FormatException(string.Format(Resources.InvalidInstanceIdFormat, inst0));
					}
					if (!ValidateHexString(inst1))
					{
						throw new FormatException(string.Format(Resources.InvalidInstanceIdFormat, inst1));
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
			if (!string.IsNullOrEmpty(str))
			{
				if (str.Length == 8 || str.Length == 10)
				{
					Regex r = new Regex(@"^(0x|0X)?[a-fA-F0-9]+$");
					
					return r.IsMatch(str);
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

			string instSub = batchFshList[index].InstanceId.Substring(0, 7);
			if (temp.Width >= 128 && temp.Height >= 128)
			{
				batchFshList[index].InstanceId = instSub + endreg;
			}
			else if (temp.Width == 64 && temp.Height == 64)
			{
				batchFshList[index].InstanceId = instSub + end64;
			}
			else if (temp.Width == 32 && temp.Height == 32)
			{
				batchFshList[index].InstanceId = instSub + end32;
			}
			else if (temp.Width == 16 && temp.Height == 16)
			{
				batchFshList[index].InstanceId = instSub + end16;
			}
			else if (temp.Width == 8 && temp.Height == 8)
			{
				batchFshList[index].InstanceId = instSub + end8;
			}
		}

		private void FormatRefresh(int index)
		{
			using (Bitmap temp = new Bitmap(batchFshList[index].FileName))
			{
				SetEndFormat(temp, index);
			}
			string instance = batchFshList[index].InstanceId;
			batchListView.SelectedItems[0].SubItems[3].Text = instance;
			tgiInstanceTxt.Text = instance;
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
			for (int i = 0; i < batchFshList.Count; i++)
			{
				BatchFshContainer batch = batchFshList[i];
				if (string.IsNullOrEmpty(batch.GroupId))
				{
					batch.GroupId = "1ABE787D";
				}
				
				if (string.IsNullOrEmpty(batch.InstanceId))
				{
					string fileName = Path.GetFileNameWithoutExtension(batch.FileName);

					if (fileName.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
					{
						if (!ValidateHexString(fileName))
						{
							throw new FormatException(string.Format(Resources.InvalidInstanceFileNameFormat, fileName));
						}

						batch.InstanceId = fileName.Substring(2, 8);
					}
					else
					{
						batch.InstanceId = RandomHexString(7);
					}
				}
			}
		}

		internal void AddFilesToListView()
		{
			AddFilesToListView(0);
		}

		private void AddFilesToListView(int startIndex)
		{
			try
			{
				int count = batchFshList.Count;

				if (tgiGroupTxt.Text.Length == 8)
				{
					for (int i = 0; i < count; i++)
					{
						batchFshList[i].GroupId = tgiGroupTxt.Text;
					}
				}
				else
				{
					ReadGrouptxt(groupPath);
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
							if (!ValidateHexString(fileName))
							{
								throw new FormatException(string.Format(Resources.InvalidInstanceFileNameFormat, fileName));
							}

							batch.InstanceId = fileName.Substring(2, 8);
						}
						else
						{
							batch.InstanceId = RandomHexString(7);
							SetEndFormat(temp, n);
						}

						string alphaName = Path.Combine(Path.GetDirectoryName(path), fileName + "_a" + Path.GetExtension(path));
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
				if (tgiGroupTxt.Text.Length == 8)
				{
					string group = tgiGroupTxt.Text;
					if (!group.Equals(batchFshList[index].GroupId, StringComparison.OrdinalIgnoreCase))
					{
						batchFshList[index].GroupId = group;
						batchListView.SelectedItems[0].SubItems[2].Text = group;
					}
				}
			}
		}

		private void tgiInstanceTxt_TextChanged(object sender, EventArgs e)
		{
			if (batchListView.SelectedItems.Count > 0)
			{
				int index = batchListView.SelectedItems[0].Index;
				if (tgiInstanceTxt.Text.Length == 8)
				{
					string instance = tgiInstanceTxt.Text;

					if (!instance.Equals(batchFshList[index].InstanceId, StringComparison.OrdinalIgnoreCase))
					{
						batchFshList[index].InstanceId = instance;
						FormatRefresh(index);
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
					
					int count = infos.Count;
					for (int i = 0; i < count; i++)
					{
						FileInfo fi = infos[i];
						
						if (!fi.Name.Contains("_a"))
						{
							fileList.Add(fi.FullName);
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
			
			if (batchFshList != null)
			{
				batchFshList.Dispose();
				batchFshList = null;
			}
			
			outputFolder = null;
			
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

		private void mipFormatCbo_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.mipFormat = (MipmapFormat)mipFormatCbo.SelectedIndex;

			if (settings != null)
			{
				settings.PutSetting("MipFormat", mipFormatCbo.SelectedIndex.ToString(CultureInfo.InvariantCulture));
			}
		}

		private void SetControlsEnabled(bool enabled)
		{
			this.mipFormatCbo.Enabled = enabled;
			this.compDatCb.Enabled = enabled;
			this.fshWriteCompCb.Enabled = enabled;
			this.tgiGroupTxt.Enabled = enabled;
			this.tgiInstanceTxt.Enabled = enabled;
			this.fshTypeBox.Enabled = enabled;
			this.Inst0_4rdo.Enabled = enabled;
			this.Inst5_9rdo.Enabled = enabled;
			this.InstA_Erdo.Enabled = enabled;
			this.newDatbtn.Enabled = enabled;
			this.saveDatBtn.Enabled = enabled;
			this.addBtn.Enabled = enabled;
			this.remBtn.Enabled = enabled;
			this.clearListBtn.Enabled = enabled;
			this.outFolderBtn.Enabled = enabled;
			this.processBatchBtn.Enabled = enabled;
		}
	}
}
