using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using FSHLib;
using System.Collections;
using FshDatIO;
using PngtoFshBatchtxt.Properties;

namespace PngtoFshBatchtxt
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool cmdlineonly = false;
            if (args.Length > 0)
            {
                Form1 runform = null;
                Form1 form1 = new Form1();
                try
                {
                    form1.autoprocMipscb.Checked = false;
                    form1.compress_datmips = true;
                    string loc = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    form1.grppath = Path.Combine(loc, @"Groupid.txt");
                    form1.rangepath = Path.Combine(loc, @"instRange.txt");
                    cmdlineonly = false;

                    int fcnt = Form1.Countpngs(args);
                    bool pnglistbuilt = false;
                    int imgarg = -1 + fcnt;
                    int imgcnt = -1;
                    if (fcnt > 0)
                    {
                        form1.patharray = new List<string>(fcnt);
                        form1.instarray = new List<string>(fcnt);
                        form1.grouparray = new List<string>(fcnt);
                        form1.typearray = new List<string>(fcnt);
                        form1.batchFshList = new List<BatchFshContainer>(fcnt);
                    }
                    for (int a = 0; a < args.Length; a++)
                    {
                        FileInfo fi = new FileInfo("null");
                        FileAttributes attr = new FileAttributes();
                        if (!args[a].StartsWith("/proc", StringComparison.OrdinalIgnoreCase) && !args[a].StartsWith("/mips", StringComparison.OrdinalIgnoreCase) && !args[a].StartsWith("/d", StringComparison.OrdinalIgnoreCase) && !args[a].StartsWith("/o", StringComparison.OrdinalIgnoreCase) && !args[a].StartsWith("/?", StringComparison.OrdinalIgnoreCase) && !args[a].StartsWith("/group:", StringComparison.OrdinalIgnoreCase))
                        {
                            attr = File.GetAttributes(args[a]);
                            fi = new FileInfo(args[a]);
                        }

                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            try
                            {
                                DirectoryInfo di = new DirectoryInfo(args[a]);
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
                                        imgcnt++;
                                        form1.patharray.Insert(imgcnt, info.FullName);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, form1.Text);
                            }

                        }
                        else if (fi.Extension.Equals(".png") || fi.Extension.Equals(".bmp"))
                        {
                            if (fi.Exists)
                            {
                                if (fi.Name.Contains("_a"))
                                {
                                    continue;
                                }
                                else
                                {
                                    imgcnt++;
                                    form1.patharray.Insert(imgcnt, fi.FullName);
                                }
                            }
                        }
                        else if ((args[a].StartsWith("/proc", StringComparison.OrdinalIgnoreCase)|| args[a].StartsWith("/dat:", StringComparison.OrdinalIgnoreCase) || args[a].StartsWith("/outdir:", StringComparison.OrdinalIgnoreCase) || args[a].StartsWith("/mips", StringComparison.OrdinalIgnoreCase) || args[a].StartsWith("/group:", StringComparison.OrdinalIgnoreCase)))
                        {
                            char[] splitchar = new char[] { ':' };
                            if (args[a].StartsWith("/dat:", StringComparison.OrdinalIgnoreCase) || args[a].StartsWith("/o", StringComparison.OrdinalIgnoreCase))
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
                                    MessageBox.Show(args[a] + "path must not be empty", form1.Text);
                                }
                                // test if the directory exists
                                string[] path = new string[2];
                                path[0] = args[a].Substring(0, strlen);
                                path[1] = args[a].Substring(strlen, args[a].Length - strlen);
                                string dir = Path.GetDirectoryName(path[1]);
                                if (!Directory.Exists(dir))
                                {
                                    MessageBox.Show(dir + "does not exist", form1.Text);
                                }

                            }

                            if (args[a].StartsWith("/proc", StringComparison.OrdinalIgnoreCase))
                            {
                                if (fcnt > 0)
                                {
                                    cmdlineonly = true;
                                    form1.processbatchbtn_Click(null, null);
                                }
                            }
                            else if (args[a].StartsWith("/mips", StringComparison.OrdinalIgnoreCase))
                            {
                                if (fcnt > 0)
                                {
                                    cmdlineonly = true;
                                    form1.autoprocMipscb.Checked = true;
                                }
                            }
                            else if (args[a].StartsWith("/dat:", StringComparison.OrdinalIgnoreCase))
                            {
                                if (fcnt > 0)
                                {
                                    try
                                    {
                                        cmdlineonly = true;
                                        string[] dat = new string[2];
                                        dat[0] = args[a].Substring(0, 5);
                                        dat[1] = args[a].Substring(5, args[a].Length - 5);
                                        if (!string.IsNullOrEmpty(dat[1]))
                                        {

                                            string path = Path.GetDirectoryName(dat[1]);
                                            if (Directory.Exists(path))
                                            {
                                                if (form1.dat == null)
                                                {
                                                    form1.dat = new DatFile();
                                                }
                                                
                                                if (form1.autoprocMipscb.Checked)
                                                {
                                                    if (form1.mipsbtn_clicked == false)
                                                    {
                                                        form1.mipbtn_Click(null, null);
                                                        form1.RebuildDat(form1.dat);
                                                    }
                                                    else
                                                    {
                                                        form1.RebuildDat(form1.dat);
                                                    }
                                                }
                                                else
                                                {
                                                    if (form1.batch_processed == false)
                                                    {
                                                        form1.ProcessBatch();
                                                    }
                                                    form1.RebuildDat(form1.dat);
                                                }
                                                form1.dat.Save(dat[1].Trim());
                                                form1.dat.Close();
                                                form1.dat = null;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, Resources.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    if (group[1].Length == 10)
                                    {
                                        if (group[1].ToUpperInvariant().StartsWith("0X"))
                                        {
                                            groupid = group[1].ToUpper().Substring(2, 8);
                                        }
                                    }
                                    else if (group[1].Length == 8)
                                    {
                                        groupid = group[1].ToUpper();
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
                                                form1.grouparray.Insert(c, groupid);
                                                form1.batchListView.Items[c].SubItems[2].Text = form1.grouparray[c];
                                            }
                                        }
                                        else
                                        {
                                            form1.tgiGrouptxt.Text = groupid;
                                        }
                                    }
                                    else
                                    {
                                        
                                        if (fcnt > 0)
                                        {
                                            string errgrp = "1ABE787D";
                                            for (int c = 0; c < form1.batchListView.Items.Count; c++)
                                            {
                                                form1.grouparray.Insert(c, errgrp);
                                                form1.batchListView.Items[c].SubItems[2].Text = form1.grouparray[c];
                                            }
                                        }

                                        MessageBox.Show("The group id after /group: contains invalid characters.\n It must only contain numbers 0-9 and letters A-F.", form1.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else if (args[a].StartsWith("/outdir:", StringComparison.OrdinalIgnoreCase))
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

                        }
                        else if (args[a].Equals("/?"))
                        {
                            cmdlineonly = true;
                            Program.showhelp();
                        }

                        if (form1.patharray != null && form1.patharray.Count > 0 && form1.patharray.Count == fcnt && !cmdlineonly && !pnglistbuilt)
                        {
                            form1.BuildPngList();
                            pnglistbuilt = true;
                        }
                        
                    }
                    runform = form1; 
                    
                }
                finally
                {
                    form1.Dispose();
                    form1 = null;
                }

                if (!cmdlineonly)
                {
                    Application.Run(form1);
                }
                else
                {
                    runform.Dispose();
                    runform = null;
                }
            }
            else
            {
                Application.Run(new Form1());
            }
        }
        static void showhelp()
        {
            MessageBox.Show("Command line arguments:\n\n PngtoFshBatch images [/outdir:<directory>] [/mips] [/group: <groupid>] [/dat:<filename>] [/proc] [/?] \n\n images a list or folder of images to process seperated by spaces \n /? show this help\n /proc process images and save\n /mips generate mipmaps for the zoom levels\n /dat:<filename> process images and save them as a dat\n /outdir:<directory> output the fsh files from /proc into directory.\n /group: <groupid> Assign the <groupid> to the files \n\n Paths containing spaces must be encased in quotes", Resources.ProgramName);
        }
    }
}
