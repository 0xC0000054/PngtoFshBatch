using FshDatIO;
using PngtoFshBatchtxt.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Windows.Forms;

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

        private static DialogResult ShowErrorMessage(string message)
        {
            return MessageBox.Show(message, ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
        }

        private static bool ValidateArgumentPaths(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase) || arg.StartsWith(CommandLineSwitches.OutputDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    bool processDat = arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase);
                    int strlen = processDat ? 5 : 8;

                    string path = arg.Substring(strlen, arg.Length - strlen).Trim();

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (processDat)
                        {
                            path = Path.GetDirectoryName(path);
                        }

                        if (!Directory.Exists(path))
                        {
                            string message = string.Format(CultureInfo.CurrentCulture, Resources.ArgumentDirectoryNotFound, path);
                            if (ShowErrorMessage(message) == DialogResult.OK)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        string name = processDat ? CommandLineSwitches.ProcessDat : CommandLineSwitches.OutputDirectory;
                        string message = string.Format(CultureInfo.CurrentCulture, Resources.ArgumentPathEmpty, name);
                        if (ShowErrorMessage(message) == DialogResult.OK)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool ValidateArguments(string[] args, int fileCount)
        {
            if (!ValidateArgumentPaths(args))
            {
                return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if ((arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase) ||
                     arg.StartsWith(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase)) && fileCount == 0)
                {
                    return false;
                }

                if (arg.StartsWith(CommandLineSwitches.GroupID, StringComparison.OrdinalIgnoreCase))
                {
                    string group = arg.Substring(7, arg.Length - 7).Trim();

                    if (!Form1.ValidateHexString(group))
                    {
                        if (ShowErrorMessage(Resources.InvalidGroupIDArgument) == DialogResult.OK)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static void ProcessCommandLineSwitches(string[] args, Form1 form1, int fileCount, bool cmdLineOnly)
        {
            if (args.Length == 1 && args[0] == "/?")
            {
                ShowHelp();
                return;
            }

            if (ValidateArguments(args, fileCount))
            {
                if (cmdLineOnly)
                {
                    form1.mipFormatCbo.SelectedIndex = (int)MipmapFormat.None;
                }

                try
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        string arg = args[i];

                        if (arg.StartsWith(CommandLineSwitches.OutputDirectory, StringComparison.OrdinalIgnoreCase))
                        {
                            form1.outputFolder = arg.Substring(8, arg.Length - 8).Trim();
                        }
                        else if (arg.StartsWith(CommandLineSwitches.GroupID, StringComparison.OrdinalIgnoreCase))
                        {
                            string group = arg.Substring(7, arg.Length - 7).Trim();
                            string groupid;

                            if (group.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                            {
                                groupid = group.Substring(2, 8).ToUpperInvariant();
                            }
                            else
                            {
                                groupid = group.ToUpperInvariant();
                            }

                            if (fileCount > 0)
                            {
                                for (int c = 0; c < form1.batchFshList.Count; c++)
                                {
                                    form1.batchFshList[c].GroupId = groupid;
                                }
                            }
                            else
                            {
                                form1.groupId = groupid;
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
                            string datFileName = arg.Substring(5, arg.Length - 5).Trim();

                            form1.displayProgress = false;

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
                            else
                            {
                                form1.dat = new DatFile();
                            }

                            try
                            {
                                form1.SetGroupAndInstanceIds();
                                form1.ProcessBatch();

                                if (form1.mipFormatCbo.SelectedIndex == (int)MipmapFormat.Normal)
                                {
                                    form1.ProcessMips();
                                }

                                form1.RebuildDat(form1.dat);

                                form1.dat.Save(datFileName);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            finally
                            {
                                form1.dat.Close();
                                form1.dat = null;
                            }
                        }
                        else if (arg.StartsWith(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase) && fileCount > 0)
                        {
                            form1.displayProgress = false;

                            form1.SetGroupAndInstanceIds();
                            form1.ProcessBatch();
                            form1.ProcessBatchSaveFiles();
                        }
                    }
                }
                catch (DirectoryNotFoundException ex)
                {
                    ShowErrorMessage(ex.Message);
                }
                catch (FormatException ex)
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
                        files.AddRange(di.GetFiles("*.bmp", SearchOption.TopDirectoryOnly));


                        foreach (FileInfo item in files)
                        {
                            if (!item.Name.Contains(Form1.AlphaMapSuffix, StringComparison.OrdinalIgnoreCase))
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
                            if (!Path.GetFileName(fileName).Contains(Form1.AlphaMapSuffix, StringComparison.OrdinalIgnoreCase))
                            {
                                filePaths.Add(fileName);
                            }
                        }
                    }
                }

                return filePaths.ToArray();
            }
            catch (ArgumentException ex)
            {
                ShowErrorMessage(ex.Message);
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

            return new string[0];
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                bool cmdLineOnly = false;
                using (Form1 form1 = new Form1())
                {

                    bool haveSwitches = false;
                    foreach (string arg in args)
                    {
                        if (arg.StartsWith(CommandLineSwitches.OutputDirectory, StringComparison.OrdinalIgnoreCase) ||
                            arg.StartsWith(CommandLineSwitches.GroupID, StringComparison.OrdinalIgnoreCase) ||
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
                        form1.batchFshList = new BatchFshCollection(fileCount);
                        for (int i = 0; i < fileCount; i++)
                        {
                            form1.batchFshList.Add(new BatchFshContainer(files[i]));
                        }
                    }

                    if (haveSwitches)
                    {
                        ProcessCommandLineSwitches(args, form1, fileCount, cmdLineOnly);
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

        static void ShowHelp()
        {
            MessageBox.Show("Command line arguments:\n\n PngtoFshBatch images [/outdir:<directory>] [/group:<groupid>] [/mips] [/embed] [/fshwrite] [/dat:<filename>] [/proc] [/?] \n\n images a list or folder of images to process separated by spaces \n /outdir:<directory> Output the fsh files from /proc into directory.\n /group:<groupid> Assign the <groupid> to the files.\n /mips Generate mipmaps and save in separate files.\n /embed Generate mipmaps and save after the main image (used by most automata).\n  /fshwrite Compress the DXT1 and DXT3 images with FshWrite compression.\n /dat:<filename> Process images and save them into a new or existing dat.\n /proc Process images and save.\n /? Show this help. \n\n Paths containing spaces must be encased in quotes.", ProgramName, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0);
        }
    }
}
