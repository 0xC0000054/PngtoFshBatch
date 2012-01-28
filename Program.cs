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
        private static bool pngListBuilt = false;
        private static void ProcessCommandLineSwitches(string[] args, Form1 form1, int fcnt)
        {

            char[] splitchar = new char[] { ':' };
            int length = args.Length;
            for (int a = 0; a < length; a++)
            {
                if (args[a].StartsWith("/dat:", StringComparison.OrdinalIgnoreCase) || args[a].StartsWith("/outdir:", StringComparison.OrdinalIgnoreCase))
                {
                    int strlen;
                    string teststr;
                    // test for an empty path string
                    if (args[a].StartsWith("/dat:", StringComparison.OrdinalIgnoreCase))
                    {
                        strlen = 5;
                    }
                    else
                    {
                        strlen = 8;
                    }
                    teststr = args[a].Substring(strlen, args[a].Length - strlen);
                    if (string.IsNullOrEmpty(teststr))
                    {
                        MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentPathEmpty, teststr), Form1.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                    // test if the directory exists
                    string[] path = new string[2];
                    path[0] = args[a].Substring(0, strlen);
                    path[1] = args[a].Substring(strlen, args[a].Length - strlen);
                    string dir = Path.GetDirectoryName(path[1]);
                    if (!Directory.Exists(dir))
                    {
                        MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Resources.ArgumentDirectoryNotFound, dir), Form1.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }

                }

  
                if (args[a].StartsWith("/outdir:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] dir = new string[2];
                    dir[0] = args[a].Substring(0, 8);
                    dir[1] = args[a].Substring(8, args[a].Length - 8);
                    if (!string.IsNullOrEmpty(dir[1]))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(dir[1])))
                        {
                            form1.outfolder = dir[1];
                        }
                    }
                }
                else if (args[a].StartsWith("/group:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] group = new string[2];
                    string groupid = null;
                    group = args[a].Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
                    if (!string.IsNullOrEmpty(group[1]))
                    {
                        if (group[1].Length == 10 && group[1].StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            groupid = group[1].ToUpperInvariant().Substring(2, 8);
                        }
                        else if (group[1].Length == 8)
                        {
                            groupid = group[1].ToUpperInvariant();
                        }
                    }
                    if (groupid != null)
                    {
                        if (Form1.ValidateHexString(groupid))
                        {
                            if (fcnt > 0)
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

                            if (fcnt > 0)
                            {
                                string errgrp = "1ABE787D";
                                for (int c = 0; c < form1.batchListView.Items.Count; c++)
                                {
                                    form1.groupArray.Insert(c, errgrp);
                                    form1.batchListView.Items[c].SubItems[2].Text = form1.groupArray[c];
                                }
                            }

                            MessageBox.Show(Resources.InvalidGroupIDArgument, form1.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (args[a].StartsWith("/mips", StringComparison.OrdinalIgnoreCase))
                {
                    form1.autoProcMipsCb.Checked = true;
                }
                else if (args[a].Equals("/fshwrite", StringComparison.OrdinalIgnoreCase))
                {
                    form1.fshWriteCompCb.Checked = true;
                }
                else if (args[a].StartsWith("/dat:", StringComparison.OrdinalIgnoreCase) && (fcnt > 0))
                {
                    try
                    {
                        string[] datArgs = new string[2];
                        datArgs[0] = args[a].Substring(0, 5);
                        datArgs[1] = args[a].Substring(5, args[a].Length - 5);
                        if (!string.IsNullOrEmpty(datArgs[1]))
                        {

                            string path = Path.GetDirectoryName(datArgs[1]);
                            if (Directory.Exists(path))
                            {
                                if (!pngListBuilt)
                                {
                                    form1.BuildPngList();
                                    pngListBuilt = true;
                                }

                                if (File.Exists(datArgs[1]))
                                {
                                    try
                                    {
                                        form1.dat = new DatFile(datArgs[1]);
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
                                if (!form1.batch_processed)
                                {
                                    form1.ProcessBatchCmd();
                                }

                                if (form1.autoProcMipsCb.Checked)
                                {
                                    if (!form1.mipsbtn_clicked)
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
                                form1.dat.Save(datArgs[1].Trim());
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
                else if (args[a].StartsWith("/proc", StringComparison.OrdinalIgnoreCase) && (fcnt > 0))
                {
                    if (!pngListBuilt)
                    {
                        form1.BuildPngList();
                        pngListBuilt = true;
                    }

                    form1.ProcessBatchCmd();
                    form1.ProcessBatchSaveFilesCmd();
                }
                else if (args[a].Equals("/?", StringComparison.Ordinal))
                {
                    Program.showhelp();
                }

            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
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

                    form1.autoProcMipsCb.Checked = false;
                    form1.compress_datmips = true;
                    string loc = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    form1.grppath = Path.Combine(loc, @"Groupid.txt");
                    form1.rangepath = Path.Combine(loc, @"instRange.txt");
                    cmdLineOnly = false;

                    int fcnt = Form1.Countpngs(args);
                    if (fcnt > 0)
                    {
                        form1.pathArray = new List<string>(fcnt);
                        form1.instArray = new List<string>(fcnt);
                        form1.groupArray = new List<string>(fcnt);
                        form1.typeArray = new List<string>(fcnt);
                        form1.batchFshList = new List<BatchFshContainer>(fcnt);
                    }
                    bool haveSwitches = false;
                    foreach (string arg in args)
                    {
                        if (arg.StartsWith("/outdir:", StringComparison.OrdinalIgnoreCase) ||
                            arg.StartsWith("/group:", StringComparison.InvariantCultureIgnoreCase) ||
                            arg.Equals("/mips", StringComparison.OrdinalIgnoreCase) ||
                            arg.Equals("/fshwrite", StringComparison.OrdinalIgnoreCase) ||
                            arg.StartsWith("/dat:", StringComparison.OrdinalIgnoreCase) ||
                            arg.Equals("/proc", StringComparison.OrdinalIgnoreCase) ||
                            arg.Equals("/?", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!haveSwitches)
                            {
                                haveSwitches = true;
                            }

                            if (arg.Equals("/proc", StringComparison.OrdinalIgnoreCase) ||
                                arg.StartsWith("/dat:", StringComparison.OrdinalIgnoreCase) ||
                                arg.Equals("/?", StringComparison.OrdinalIgnoreCase))
                            {
                                cmdLineOnly = true;
                            }


                            continue;
                        }

                        FileInfo fi = new FileInfo(arg);

                        if ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            try
                            {
                                DirectoryInfo di = new DirectoryInfo(arg);
                                ArrayList filearray = new ArrayList();

                                filearray.AddRange(di.GetFiles("*.png", SearchOption.TopDirectoryOnly));
                                FileInfo[] bmparr = di.GetFiles("*.bmp", SearchOption.TopDirectoryOnly);
                                if (bmparr.Length > 0)
                                {
                                    filearray.AddRange(bmparr);
                                }

                                foreach (FileInfo info in filearray)
                                {
                                    if (Path.GetFileName(info.FullName).Contains("_a"))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        form1.pathArray.Add(info.FullName);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, form1.Text);
                            }

                        }
                        else if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                        {
                            if (fi.Exists)
                            {
                                if (fi.Name.Contains("_a"))
                                {
                                    continue;
                                }
                                else
                                {
                                    form1.pathArray.Add(fi.FullName);
                                }
                            }
                        }
                    }

                    if (form1.pathArray != null && form1.pathArray.Count > 0 && !cmdLineOnly)
                    {
                        form1.BuildPngList();
                        pngListBuilt = true;
                    }

                    if (haveSwitches)
                    {
                        ProcessCommandLineSwitches(args, form1, fcnt);
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
        static void showhelp()
        {
            MessageBox.Show("Command line arguments:\n\n PngtoFshBatch images [/outdir:<directory>] [/group:<groupid>] [/mips] [/fshwrite] [/dat:<filename>] [/proc] [/?] \n\n images a list or folder of images to process seperated by spaces \n /outdir:<directory> Output the fsh files from /proc into directory.\n /group:<groupid> Assign the <groupid> to the files.\n /mips Generate mipmaps for the zoom levels.\n /fshwrite Compress the DXT1 and DXT3 images with FshWrite compression.\n /dat:<filename> Process images and save them into a new or existing dat.\n /proc Process images and save.\n /? Show this help. \n\n Paths containing spaces must be encased in quotes.", Form1.ProgramName);
        }
    }
}
