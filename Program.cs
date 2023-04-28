/*
* This file is part of PngtoFshBatch, a tool for batch converting images
* to FSH.
*
* Copyright (C) 2009-2017, 2023 Nicholas Hayes
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*
*/

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

        private static bool ValidateArgumentPath(string path, string argumentName)
        {
            if (!string.IsNullOrEmpty(path))
            {
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
                string message = string.Format(CultureInfo.CurrentCulture, Resources.ArgumentPathEmpty, argumentName);
                if (ShowErrorMessage(message) == DialogResult.OK)
                {
                    return false;
                }
            }

            return true;
        }
      
        private static bool ValidateArguments(string[] args, int firstCommandLineSwitch, int fileCount)
        {
            for (int i = firstCommandLineSwitch; i < args.Length; i++)
            {
                string arg = args[i];

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
                else if (arg.StartsWith(CommandLineSwitches.OutputDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    string path = arg.Substring(8, arg.Length - 8).Trim();

                    if (!ValidateArgumentPath(path, CommandLineSwitches.OutputDirectory))
                    {
                        return false;
                    }
                }
                else if (arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase))
                {
                    if (fileCount > 0)
                    {
                        string path = arg.Substring(5, arg.Length - 5).Trim();

                        if (!ValidateArgumentPath(Path.GetDirectoryName(path), CommandLineSwitches.ProcessDat))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (ShowErrorMessage(string.Format(CultureInfo.InvariantCulture, Resources.ProcessCommandNoFiles, CommandLineSwitches.ProcessDat)) == DialogResult.OK)
                        {
                            return false;
                        }
                    }
                }
                else if (arg.StartsWith(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase) && fileCount == 0)
                {
                    if (ShowErrorMessage(string.Format(CultureInfo.InvariantCulture, Resources.ProcessCommandNoFiles, CommandLineSwitches.ProcessFiles)) == DialogResult.OK)
                    {
                        return false;
                    }
                }
                
            }

            return true;
        }

        private static void ProcessCommandLineSwitches(string[] args, int firstCommandLineSwitch, Form1 form1, int fileCount, bool cmdLineOnly)
        {
            if (ValidateArguments(args, firstCommandLineSwitch, fileCount))
            {
                if (cmdLineOnly)
                {
                    form1.mipFormatCbo.SelectedIndex = (int)MipmapFormat.None;
                }

                try
                {
                    for (int i = firstCommandLineSwitch; i < args.Length; i++)
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

                            form1.groupId = groupid;
                            if (fileCount > 0)
                            {
                                for (int c = 0; c < form1.batchFshList.Count; c++)
                                {
                                    form1.batchFshList[c].GroupId = groupid;
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
                        else if (arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase))
                        {
                            string datFileName = arg.Substring(5, arg.Length - 5).Trim();

                            form1.displayProgress = false;

                            if (File.Exists(datFileName))
                            {
                                form1.dat = new DatFile(datFileName);
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

                                form1.RebuildDat();

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
                        else if (arg.StartsWith(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase))
                        {
                            form1.displayProgress = false;

                            form1.SetGroupAndInstanceIds();
                            form1.ProcessBatch();
                            form1.ProcessBatchSaveFiles();
                        }
                    }
                }
                catch (DatFileException ex)
                {
                    ShowErrorMessage(ex.Message);
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

        private static string[] GetImagePaths(string[] args, int firstCommandLineSwitch)
        {
            if (firstCommandLineSwitch > 0)
            {
                try
                {
                    List<string> filePaths = new List<string>();
                    for (int i = 0; i < firstCommandLineSwitch; i++)
                    {
                        string fileName = args[i];
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
                if (args.Length == 1 && args[0] == "/?")
                {
                    ShowHelp();
                    return;
                }

                using (Form1 form1 = new Form1())
                {
                    bool haveSwitches = false;
                    bool cmdLineOnly = false;
                    int firstCommandLineSwitch = args.Length;

                    for (int i = 0; i < args.Length; i++)
                    {
                        string arg = args[i];

                        if (arg.StartsWith(CommandLineSwitches.OutputDirectory, StringComparison.OrdinalIgnoreCase) ||
                            arg.StartsWith(CommandLineSwitches.GroupID, StringComparison.OrdinalIgnoreCase) ||
                            arg.Equals(CommandLineSwitches.NormalMipmaps, StringComparison.OrdinalIgnoreCase) ||
                            arg.Equals(CommandLineSwitches.EmbeddedMipmaps, StringComparison.OrdinalIgnoreCase) ||
                            arg.Equals(CommandLineSwitches.FshwriteCompression, StringComparison.OrdinalIgnoreCase) ||
                            arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase) ||
                            arg.Equals(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!haveSwitches)
                            {
                                haveSwitches = true;
                                firstCommandLineSwitch = i;
                            }

                            if (arg.Equals(CommandLineSwitches.ProcessFiles, StringComparison.OrdinalIgnoreCase) ||
                                arg.StartsWith(CommandLineSwitches.ProcessDat, StringComparison.OrdinalIgnoreCase))
                            {
                                cmdLineOnly = true;
                            }
                        }

                    }

                    string[] files = GetImagePaths(args, firstCommandLineSwitch);
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
                        ProcessCommandLineSwitches(args, firstCommandLineSwitch, form1, fileCount, cmdLineOnly);
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
