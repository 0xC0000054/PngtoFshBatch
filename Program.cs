using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using FshDatIO;
using PngtoFshBatchtxt.Properties;

namespace PngtoFshBatchtxt
{
	static class Program
	{
		private const string ProgramName = "Png to Fsh Batch";

		private static class CommandLineSwitches
		{
			internal const string OutputDirectory = "/outdir:";
			internal const string NormalMipmaps = "/mips";
			internal const string EmbeddedMipmaps = "/embed";
			internal const string GroupID = "/group:";
			internal const string FshwriteCompression = "/fshwrite";
			internal const string ProcessDat = "/dat:";
			internal const string ProcessFiles = "/proc";
		}

		private static void ValidateArgumentPaths(string[] args)
		{ 
			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];
				if (arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase) || arg.StartsWith(CommandLineSwitches.OutputDirectory, StringComparison.OrdinalIgnoreCase))
				{
					int strlen;
					// test for an empty path string
					if (arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase))
					{
						strlen = 5;
					}
					else
					{
						strlen = 8;
					}

					string teststr = arg.Substring(strlen, arg.Length - strlen);

					if (string.IsNullOrEmpty(teststr))
					{
						MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentPathEmpty, teststr), ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(1);
					}
					// test if the directory exists
					string path =  arg.Substring(strlen, arg.Length - strlen);
					
					string dir = Path.GetDirectoryName(path);
					if (!Directory.Exists(dir))
					{
						MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentDirectoryNotFound, dir), ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(1);
					}
				} 
			}
		}

		private static void ProcessCommandLineSwitches(string[] args, Form1 form1, int fileCount, bool cmdLineOnly)
		{
			bool pngListBuilt = false;

			if (args.Length == 1 && args[0] == "/?")
			{
				ShowHelp();
				return;
			}

			ValidateArgumentPaths(args);

			if (cmdLineOnly)
			{
				form1.mipFormatCbo.SelectedIndex = (int)MipmapFormat.None; 
			}

			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];

				if (arg.StartsWith(CommandLineSwitches.OutputDirectory, StringComparison.OrdinalIgnoreCase))
				{
					string dir = arg.Substring(8, arg.Length - 8);

					if (!string.IsNullOrEmpty(dir))
					{
						if (Directory.Exists(Path.GetDirectoryName(dir)))
						{
							form1.outputFolder = dir;
						}
					}
				}
				else if (arg.StartsWith(CommandLineSwitches.GroupID, StringComparison.OrdinalIgnoreCase))
				{
					string group = arg.Substring(7, arg.Length - 7);
					string groupid = null;

					if (!string.IsNullOrEmpty(group))
					{
						if (group.Length == 10 && group.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							groupid = group.ToUpperInvariant().Substring(2, 8);
						}
						else if (group.Length == 8)
						{
							groupid = group.ToUpperInvariant();
						}
					}

					if (groupid != null)
					{
						if (Form1.ValidateHexString(groupid))
						{
							if (fileCount > 0)
							{
								for (int c = 0; c < form1.batchListView.Items.Count; c++)
								{
									form1.groupArray.Insert(c, groupid);
									form1.batchListView.Items[c].SubItems[2].Text = form1.groupArray[c];
								}
							}
							else
							{
								form1.tgiGroupTxt.Text = groupid;
							}
						}
						else
						{
							if (fileCount > 0)
							{
								string defaultGroup = "1ABE787D";
								for (int c = 0; c < form1.batchListView.Items.Count; c++)
								{
									form1.groupArray.Insert(c, defaultGroup);
									form1.batchListView.Items[c].SubItems[2].Text = form1.groupArray[c];
								}
							}

							MessageBox.Show(Resources.InvalidGroupIDArgument, form1.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
				else if (arg.StartsWith(CommandLineSwitches.NormalMipmaps, StringComparison.OrdinalIgnoreCase))
				{
					form1.mipFormatCbo.SelectedIndex = (int)MipmapFormat.Normal;
				}
				else if (arg.StartsWith(CommandLineSwitches.EmbeddedMipmaps, StringComparison.OrdinalIgnoreCase))
				{
					form1.mipFormatCbo.SelectedIndex = (int)MipmapFormat.Embedded;
				}
				else if (arg.Equals(CommandLineSwitches.FshwriteCompression, StringComparison.OrdinalIgnoreCase))
				{
					form1.fshWriteCompCb.Checked = true;
				}
				else if (arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase) && fileCount > 0)
				{
					try
					{
						string datFileName = arg.Substring(5, arg.Length - 5).Trim();

						if (!string.IsNullOrEmpty(datFileName))
						{

							string path = Path.GetDirectoryName(datFileName);
							if (Directory.Exists(path))
							{
								if (!pngListBuilt)
								{
									form1.BuildPngList();
									pngListBuilt = true;
								}

								if (File.Exists(datFileName))
								{
									try
									{
										form1.dat = new DatFile(datFileName);
									}
									catch (DatHeaderException)
									{
										form1.dat.Dispose();
										form1.dat = new DatFile();
									}
								}
								else if (form1.dat == null)
								{
									form1.dat = new DatFile();
								}
								if (!form1.batchProcessed)
								{
									form1.ProcessBatchCmd();
								}

								if (form1.mipFormatCbo.SelectedIndex == (int)MipmapFormat.Normal)
								{
									if (!form1.mipsBuilt)
									{
										form1.ProcessMips();
										form1.RebuildDatCmd(form1.dat);
									}
									else
									{
										form1.RebuildDatCmd(form1.dat);
									}
								}
								else
								{
									form1.RebuildDatCmd(form1.dat);
								}
								form1.dat.Save(datFileName);
								form1.dat.Close();
								form1.dat = null;
							}
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, form1.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}

				}
				else if (arg.StartsWith(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase) && (fileCount > 0))
				{
					if (!pngListBuilt)
					{
						form1.BuildPngList();
						pngListBuilt = true;
					}

					form1.ProcessBatchCmd();
					form1.ProcessBatchSaveFilesCmd();
				}
			}
		}

		private static string[] GetImagePaths(string[] fileNames)
		{
			try
			{
				List<string> filePaths = new List<string>();
				foreach (var fileName in fileNames)
				{
					if (fileName.StartsWith(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith(CommandLineSwitches.NormalMipmaps, StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith(CommandLineSwitches.EmbeddedMipmaps, StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith(CommandLineSwitches.OutputDirectory, StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith(CommandLineSwitches.GroupID, StringComparison.OrdinalIgnoreCase) ||
						fileName.StartsWith("/?", StringComparison.Ordinal))
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
							if (!item.Name.Contains("_a"))
							{
								filePaths.Add(item.FullName);
							}
						}

					}
					else
					{
						string ext = Path.GetExtension(fileName);
						if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
						{
							if (!Path.GetFileName(fileName).Contains("_a"))
							{
								filePaths.Add(fileName);
							}
						}
					}
				}

				return filePaths.ToArray();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return new string[0];
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// <param name="args">The arguments.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			bool cmdLineOnly = false;
			if (args.Length > 0)
			{
				using (Form1 form1 = new Form1())
				{
					string loc = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
					form1.groupPath = Path.Combine(loc, @"Groupid.txt");
					form1.rangePath = Path.Combine(loc, @"instRange.txt");

					bool haveSwitches = false;
					foreach (string arg in args)
					{
						if (arg.StartsWith(CommandLineSwitches.OutputDirectory, StringComparison.OrdinalIgnoreCase) ||
							arg.StartsWith(CommandLineSwitches.GroupID, StringComparison.InvariantCultureIgnoreCase) ||
							arg.Equals(CommandLineSwitches.NormalMipmaps, StringComparison.OrdinalIgnoreCase) ||
							arg.Equals(CommandLineSwitches.EmbeddedMipmaps, StringComparison.OrdinalIgnoreCase) ||
							arg.Equals(CommandLineSwitches.FshwriteCompression, StringComparison.OrdinalIgnoreCase) ||
							arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase) ||
							arg.Equals(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase) ||
							arg.Equals("/?", StringComparison.Ordinal))
						{
							if (!haveSwitches)
							{
								haveSwitches = true;
							}

							if (arg.Equals(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase) ||
								arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase) ||
								arg.Equals("/?", StringComparison.Ordinal))
							{
								cmdLineOnly = true;
							}
						}

					}

					string[] files = GetImagePaths(args);
					int fileCount = files.Length;
					if (fileCount > 0)
					{
						form1.pathArray = new List<string>(files);
						form1.instArray = new List<string>(fileCount);
						form1.groupArray = new List<string>(fileCount);
						form1.typeArray = new List<FshImageFormat>(fileCount);
						form1.batchFshList = new List<BatchFshContainer>(fileCount);
					}

					if (haveSwitches)
					{
						ProcessCommandLineSwitches(args, form1, fileCount, cmdLineOnly);
					}

					if (form1.pathArray != null && form1.pathArray.Count > 0 && !cmdLineOnly)
					{
						form1.BuildPngList();
					}

					if (!cmdLineOnly)
					{
						Application.Run(form1);
					}
				}
			}
			else
			{
				Application.Run(new Form1());
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.MessageBox.Show(System.String,System.String)")]
		static void ShowHelp()
		{
			MessageBox.Show("Command line arguments:\n\n PngtoFshBatch images [/outdir:<directory>] [/group:<groupid>] [/mips] [/embed] [/fshwrite] [/dat:<filename>] [/proc] [/?] \n\n images a list or folder of images to process seperated by spaces \n /outdir:<directory> Output the fsh files from /proc into directory.\n /group:<groupid> Assign the <groupid> to the files.\n /mips Generate mipmaps and save in separate files.\n /embed Generate mipmaps and save after the main image (used by most automata).\n  /fshwrite Compress the DXT1 and DXT3 images with FshWrite compression.\n /dat:<filename> Process images and save them into a new or existing dat.\n /proc Process images and save.\n /? Show this help. \n\n Paths containing spaces must be encased in quotes.", ProgramName);
		}
	}
}
