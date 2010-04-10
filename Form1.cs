﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using FSHLib;
using SynapticEffect.SimCity;
using SynapticEffect.SimCity.DatNamespace;
using SynapticEffect.SimCity.IO;
using SynapticEffect.SimCity.IO.FileTypes;
using SynapticEffect;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace PngtoFshBatchtxt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        internal List<FSHImage> curimage = null;
        internal BitmapItem bmpitem = null;
        internal bool mipsbtn_clicked = false;
        internal List<FSHImage> mip64fsh = null;
        internal List<FSHImage> mip32fsh = null;
        internal List<FSHImage> mip16fsh = null;
        internal List<FSHImage> mip8fsh = null;

        private void GenerateMips()
        {
            for (int n = 0; n < BatchlistView1.Items.Count; n++)
            {
                int i = 0;

                Bitmap[] bmps = new Bitmap[4];
                Bitmap[] alphas = new Bitmap[4];

                BitmapItem item = new BitmapItem();
                item = (BitmapItem)curimage[n].Bitmaps[0];
                // 0 = 8, 1 = 16, 2 = 32, 3 = 64
                Image.GetThumbnailImageAbort abort = new Image.GetThumbnailImageAbort(thabort);
                Bitmap bmp = new Bitmap(item.Bitmap);
                bmps[0] = (Bitmap)bmp.GetThumbnailImage(8, 8, abort, IntPtr.Zero);
                bmps[1] = (Bitmap)bmp.GetThumbnailImage(16, 16, abort, IntPtr.Zero);
                bmps[2] = (Bitmap)bmp.GetThumbnailImage(32, 32, abort, IntPtr.Zero);
                bmps[3] = (Bitmap)bmp.GetThumbnailImage(64, 64, abort, IntPtr.Zero);
               //alpha
                Bitmap alpha = new Bitmap(item.Alpha);
                alphas[0] = (Bitmap)alpha.GetThumbnailImage(8, 8, abort, IntPtr.Zero);
                alphas[1] = (Bitmap)alpha.GetThumbnailImage(16, 16, abort, IntPtr.Zero);
                alphas[2] = (Bitmap)alpha.GetThumbnailImage(32, 32, abort, IntPtr.Zero);
                alphas[3] = (Bitmap)alpha.GetThumbnailImage(64, 64, abort, IntPtr.Zero);

                

                for (i = 0; i < 4; i++)
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
                            
                            mip64fsh.Insert(n,new FSHImage());
                            mip64fsh[n].Bitmaps.Add(mipitm);
                            mip64fsh[n].UpdateDirty();
                            using (MemoryStream mstream = new MemoryStream())
                            {
                               SaveFsh(mstream, mip64fsh[n]);
                               mip64fsh[n] = new FSHImage(mstream);
                            }
                        }
                        else if (mipitm.Bitmap.Width == 32 && mipitm.Bitmap.Height == 32)
                        {
                           
                            mip32fsh.Insert(n, new FSHImage());
                            mip32fsh[n].Bitmaps.Add(mipitm);
                            mip32fsh[n].UpdateDirty();
                            using (MemoryStream mstream = new MemoryStream())
                            {
                                SaveFsh(mstream, mip32fsh[n]);
                                mip32fsh[n] = new FSHImage(mstream);
                            }
                        }
                        else if (mipitm.Bitmap.Width == 16 && mipitm.Bitmap.Height == 16)
                        {

                            mip16fsh.Insert(n, new FSHImage());
                            mip16fsh[n].Bitmaps.Add(mipitm);
                            mip16fsh[n].UpdateDirty();
                            using (MemoryStream mstream = new MemoryStream())
                            {
                                SaveFsh(mstream, mip16fsh[n]);
                                mip16fsh[n] = new FSHImage(mstream);
                            }
                        }
                        else if (mipitm.Bitmap.Width == 8 && mipitm.Bitmap.Height == 8)
                        {
                            mip8fsh.Insert(n, new FSHImage());
                            mip8fsh[n].Bitmaps.Add(mipitm);
                            mip8fsh[n].UpdateDirty();
                            using (MemoryStream mstream = new MemoryStream())
                            {
                                SaveFsh(mstream, mip8fsh[n]);
                                mip8fsh[n] = new FSHImage(mstream);
                            }
                        }
                    }
                }

            }
        }
        internal void mipbtn_Click(object sender, EventArgs e)
        {
            if (mip64fsh != null && mip32fsh != null && mip16fsh != null && mip8fsh != null)
            {
                if (mip64fsh.Count > 0 && mip32fsh.Count > 0 && mip16fsh.Count > 0 && mip8fsh.Count > 0)
                {
                    for (int n = 0; n < BatchlistView1.Items.Count; n++)
                    {
                        mip64fsh[n] = null;
                        mip32fsh[n] = null;
                        mip16fsh[n] = null;
                        mip8fsh[n] = null;
                    }
                }
            }
            if (!batch_processed)
            {
                ProcessBatch();
            }
            GenerateMips();
            mipsbtn_clicked = true;
        }
        private bool thabort()
        {
            return false;
        }
       
        private char endreg;
        private char end64;
        private char end32;
        private char end16;
        private char end8;
        private void WriteTgi(string filename, int zoom,int cnt)
        {
            FileStream fs = new FileStream(filename + ".TGI", FileMode.OpenOrCreate, FileAccess.Write);
            
            try
            {
                using (StreamWriter sw = new StreamWriter(fs)) 
                {
                    sw.WriteLine("7ab50e44\t\n");
                    sw.WriteLine(string.Format("{0:X8}", BatchlistView1.Items[cnt].SubItems[2].Text + "\n"));
                    if (instarray[cnt].EndsWith("4") || instarray[cnt].EndsWith("3") || instarray[cnt].EndsWith("2") || instarray[cnt].EndsWith("1") || instarray[cnt].EndsWith("0"))
                    {
                        endreg = Convert.ToChar("4");
                        end64 = Convert.ToChar("3");
                        end32 = Convert.ToChar("2");
                        end16 = Convert.ToChar("1");
                        end8 = Convert.ToChar("0");
                    }
                    else if (instarray[cnt].EndsWith("9") || instarray[cnt].EndsWith("8") || instarray[cnt].EndsWith("7") || instarray[cnt].EndsWith("6") || instarray[cnt].EndsWith("5"))
                    {
                        endreg = Convert.ToChar("9");
                        end64 = Convert.ToChar("8");
                        end32 = Convert.ToChar("7");
                        end16 = Convert.ToChar("6");
                        end8 = Convert.ToChar("5");
                    }
                    else if (instarray[cnt].EndsWith("E") || instarray[cnt].EndsWith("D") || instarray[cnt].EndsWith("C") || instarray[cnt].EndsWith("B") || instarray[cnt].EndsWith("A"))
                    {
                        endreg = Convert.ToChar("E");
                        end64 = Convert.ToChar("D");
                        end32 = Convert.ToChar("C");
                        end16 = Convert.ToChar("B");
                        end8 = Convert.ToChar("A");
                    }
                    switch (zoom)
                    {
                        case 0:
                            sw.WriteLine(string.Format("{0:X8}", BatchlistView1.Items[cnt].SubItems[3].Text.Substring(0, 7) + end8));
                            break;
                        case 1:
                            sw.WriteLine(string.Format("{0:X8}", BatchlistView1.Items[cnt].SubItems[3].Text.Substring(0, 7) + end16));
                            break;
                        case 2:
                            sw.WriteLine(string.Format("{0:X8}", BatchlistView1.Items[cnt].SubItems[3].Text.Substring(0, 7) + end32));
                            break;
                        case 3:
                            sw.WriteLine(string.Format("{0:X8}", BatchlistView1.Items[cnt].SubItems[3].Text.Substring(0, 7) + end64));
                            break;
                        case 4:
                            sw.WriteLine(string.Format("{0:X8}", BatchlistView1.Items[cnt].SubItems[3].Text));
                            break;
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text);
            }
        }

        /*private int CountBatchlines(string file)
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
        }*/

        internal List<string> patharray = null;
        internal List<string> instarray = null;
        internal List<string> typearray = null;
        internal List<string> grouparray = null;
        private void Alphasrc(ListViewItem item, string path)
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
                    if (Path.GetExtension(path).ToLowerInvariant().Equals(".png") && bmp.PixelFormat == PixelFormat.Format32bppArgb)
                    {
                        item.SubItems.Add("From Transparency");
                    }
                    else
                    {
                        item.SubItems.Add("Generated Alpha");
                    }
                }
            }
        }
        /*private bool isBatchtxt(string batchpath)
        {

            if (File.Exists(batchpath))
            {
                using (StreamReader sr = new StreamReader(batchpath))
                {
                    string filetxt = sr.ReadToEnd();
                    string comp = filetxt.Substring(0, 11);
                    if (comp.Equals("%Fsh_batch%"))
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }
        internal void loadBatchtxt(string batchpath)
        {
            if (BatchlistView1.Items.Count > 0)
            {
                ClearandReset();
            }
            if (File.Exists(batchpath))
            {
                int batchline = CountBatchlines(batchpath);
                if (isBatchtxt(batchpath) == true)
                {
                    using (StreamReader sr = new StreamReader(batchpath))
                    {
                        string line = null;
                        char[] pathsplit = new char[] { '%' };
                        char[] instsplit = new char[] { '^' };
                        char[] groupsplit = new char[] { ',' };
                        patharray = new List<string>(batchline);
                        instarray = new List<string>(batchline);
                        grouparray = new List<string>(batchline);
                        typearray = new List<string>(batchline);
                        curimage = new List<FSHImage>(batchline);
                        mip64fsh = new List<FSHImage>(batchline);
                        mip32fsh = new List<FSHImage>(batchline);
                        mip16fsh = new List<FSHImage>(batchline);
                        mip8fsh = new List<FSHImage>(batchline);
                        int linecnt = -1;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line != "")
                            {
                                if (line.Equals("%Fsh_batch%") || line.StartsWith("#"))
                                {
                                    continue;
                                }
                                linecnt++;
                                string[] pathtemp = line.Split(pathsplit, StringSplitOptions.RemoveEmptyEntries);
                                patharray.Insert(linecnt, pathtemp[0].Trim());
                                string[] grouptemp = pathtemp[1].Split(instsplit, StringSplitOptions.RemoveEmptyEntries);
                                grouparray.Insert(linecnt, grouptemp[0].Trim());
                                string[] insttemp = grouptemp[1].Split(groupsplit, StringSplitOptions.RemoveEmptyEntries);
                                instarray.Insert(linecnt, insttemp[0].Trim());
                                typearray.Insert(linecnt, insttemp[1].Trim());
                            }
                        }
                    }
                    ArrayList errorarray = new ArrayList();
                    for (int i = 0; i < batchline; i++)
                    {
                        if (File.Exists(patharray[i]))
                        {
                            if (Path.GetFileName(patharray[i]).StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                            {
                               string pathinst = Path.GetFileNameWithoutExtension(patharray[i]).Substring(2,8);
                               instarray[i] = pathinst;
                            }
                            ListViewItem item1 = new ListViewItem(Path.GetFileName(patharray[i]));
                            Alphasrc(item1, patharray[i]);
                            item1.SubItems.Add(grouparray[i]);
                            item1.SubItems.Add(instarray[i]);
                            BatchlistView1.Items.Add(item1);
                        }
                        else
                        {
                            errorarray.Add(patharray[i]);
                        }
                    }
                    if (errorarray.Count > 0)
                    {
                        MessageBox.Show(this, "Some of paths do not exist", this.Text);
                    }

                }
                else
                {
                    MessageBox.Show(this, "Not a valid Batch file", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void loadbatchbtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadBatchtxt(openFileDialog1.FileName);
            }
        }*/
        private int typeindex = 0; // store previously selected fsh type
        private void BatchlistView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BatchlistView1.SelectedItems.Count > 0)
            { 
                string inst = BatchlistView1.SelectedItems[0].SubItems[3].Text;
                if (inst.EndsWith("4", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("3", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("2", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("1", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("0", StringComparison.InvariantCultureIgnoreCase))
                {
                    Inst0_4rdo.Checked = true;
                }
                else if (inst.EndsWith("9", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("8", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("7", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("6", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("5", StringComparison.InvariantCultureIgnoreCase))
                {
                    Inst5_9rdo.Checked = true;
                }
                else if (inst.EndsWith("E", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("D", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("C", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("B", StringComparison.InvariantCultureIgnoreCase) || inst.EndsWith("A", StringComparison.InvariantCultureIgnoreCase))
                {
                    InstA_Erdo.Checked = true;
                }
                tgiGrouptxt.Text = BatchlistView1.SelectedItems[0].SubItems[2].Text;
                tgiInstancetxt.Text = BatchlistView1.SelectedItems[0].SubItems[3].Text;
                int index = BatchlistView1.SelectedItems[0].Index;
                switch (typearray[index].ToUpper())
                { 
                    case "TWENTYFOURBIT":
                        FshtypeBox.SelectedIndex = 0;
                        typeindex = 0;
                        break;
                    case "THIRTYTWOBIT":
                        FshtypeBox.SelectedIndex = 1;
                        typeindex = 1;
                        break;
                    case "DXT1":
                        FshtypeBox.SelectedIndex = 2;
                        typeindex = 2;
                        break;
                    case "DXT3":
                        FshtypeBox.SelectedIndex = 3;
                        typeindex = 3;
                        break;
                }
            }
        }
        internal DatFile4 dat = null;
        internal bool compress_datmips = false;
        
        internal void RebuildDat(DatFile4 inputdat)
        {
            if (mipsbtn_clicked && mip64fsh != null && mip32fsh != null && mip16fsh != null && mip8fsh != null && curimage != null)
            {
                for (int c = 0; c < BatchlistView1.Items.Count; c++)
                {

                    TGIEntry tgi = new TGIEntry();
                    tgi.TypeID = uint.Parse("7ab50e44", NumberStyles.HexNumber);
                    tgi.GroupID = uint.Parse(BatchlistView1.Items[c].SubItems[2].Text, NumberStyles.HexNumber);
                    uint[] instanceid = new uint[5];
                    FileItem[] fi = new FileItem[5];
                    FSHWrapper[] fshwrap = new FSHWrapper[5];
                    FSHImage[] fshimg = new FSHImage[5];
                    fshimg[0] = mip8fsh[c]; fshimg[1] = mip16fsh[c]; fshimg[2] = mip32fsh[c];
                    fshimg[3] = mip64fsh[c]; fshimg[4] = curimage[c];
                    if (instarray[c].EndsWith("4") || instarray[c].EndsWith("3") || instarray[c].EndsWith("2") || instarray[c].EndsWith("1") || instarray[c].EndsWith("0"))
                    {
                        endreg = Convert.ToChar("4");
                        end64 = Convert.ToChar("3");
                        end32 = Convert.ToChar("2");
                        end16 = Convert.ToChar("1");
                        end8 = Convert.ToChar("0");
                    }
                    else if (instarray[c].EndsWith("9") || instarray[c].EndsWith("8") || instarray[c].EndsWith("7") || instarray[c].EndsWith("6") || instarray[c].EndsWith("5"))
                    {
                        endreg = Convert.ToChar("9");
                        end64 = Convert.ToChar("8");
                        end32 = Convert.ToChar("7");
                        end16 = Convert.ToChar("6");
                        end8 = Convert.ToChar("5");
                    }
                    else if (instarray[c].EndsWith("E") || instarray[c].EndsWith("D") || instarray[c].EndsWith("C") || instarray[c].EndsWith("B") || instarray[c].EndsWith("A"))
                    {
                        endreg = Convert.ToChar("E");
                        end64 = Convert.ToChar("D");
                        end32 = Convert.ToChar("C");
                        end16 = Convert.ToChar("B");
                        end8 = Convert.ToChar("A");
                    }
                    instanceid[0] = uint.Parse(BatchlistView1.Items[c].SubItems[3].Text.Substring(0, 7) + end8, NumberStyles.HexNumber);
                    instanceid[1] = uint.Parse(BatchlistView1.Items[c].SubItems[3].Text.Substring(0, 7) + end16, NumberStyles.HexNumber);
                    instanceid[2] = uint.Parse(BatchlistView1.Items[c].SubItems[3].Text.Substring(0, 7) + end32, NumberStyles.HexNumber);
                    instanceid[3] = uint.Parse(BatchlistView1.Items[c].SubItems[3].Text.Substring(0, 7) + end64, NumberStyles.HexNumber);
                    instanceid[4] = uint.Parse(BatchlistView1.Items[c].SubItems[3].Text, NumberStyles.HexNumber);

                    if (inputdat == null)
                    {
                        dat = new DatFile4();
                    }

                    for (int i = 4; i >= 0; i--)
                    {
                        if (fi[i] == null)
                        {
                            fi[i] = new FileItem();
                        }
                        fi[i].FileObject = fshwrap[i] = new FSHWrapper(fshimg[i]) { RawData = fshimg[i].RawData };
                        tgi.InstanceID = instanceid[i];
                        // CheckInstance(inputdat, tgi.GroupID, tgi.InstanceID);
                        fi[i].Size = ((IRawData)fi[i].FileObject).RawData.Length;

                        inputdat.Insert(fi[i], tgi, false, compress_datmips);
                       // Debug.WriteLine("Bmp: " + c.ToString() + " zoom: " + i.ToString());
                    }
                }
            }
            else if (!autoprocMipscb.Checked && curimage != null)
            {
                for (int c = 0; c < BatchlistView1.Items.Count; c++)
                {

                    TGIEntry tgi = new TGIEntry();
                    tgi.TypeID = uint.Parse("7ab50e44", NumberStyles.HexNumber);
                    tgi.GroupID = uint.Parse(BatchlistView1.Items[c].SubItems[2].Text, NumberStyles.HexNumber);
                    uint instanceid = new uint();
                    FileItem fi = new FileItem();
                    FSHWrapper fshwrap = new FSHWrapper();
                    if (instarray[c].EndsWith("4") || instarray[c].EndsWith("3") || instarray[c].EndsWith("2") || instarray[c].EndsWith("1") || instarray[c].EndsWith("0"))
                    {
                        endreg = Convert.ToChar("4");
                        end64 = Convert.ToChar("3");
                        end32 = Convert.ToChar("2");
                        end16 = Convert.ToChar("1");
                        end8 = Convert.ToChar("0");
                    }
                    else if (instarray[c].EndsWith("9") || instarray[c].EndsWith("8") || instarray[c].EndsWith("7") || instarray[c].EndsWith("6") || instarray[c].EndsWith("5"))
                    {
                        endreg = Convert.ToChar("9");
                        end64 = Convert.ToChar("8");
                        end32 = Convert.ToChar("7");
                        end16 = Convert.ToChar("6");
                        end8 = Convert.ToChar("5");
                    }
                    else if (instarray[c].EndsWith("E") || instarray[c].EndsWith("D") || instarray[c].EndsWith("C") || instarray[c].EndsWith("B") || instarray[c].EndsWith("A"))
                    {
                        endreg = Convert.ToChar("E");
                        end64 = Convert.ToChar("D");
                        end32 = Convert.ToChar("C");
                        end16 = Convert.ToChar("B");
                        end8 = Convert.ToChar("A");
                    }
                    instanceid = uint.Parse(BatchlistView1.Items[c].SubItems[3].Text, NumberStyles.HexNumber);

                    if (inputdat == null)
                    {
                        dat = new DatFile4();
                    }

                    if (fi == null)
                    {
                        fi = new FileItem();
                    }
                    fi.FileObject = fshwrap = new FSHWrapper(curimage[c]) { RawData = curimage[c].RawData };
                    tgi.InstanceID = instanceid;
                    // CheckInstance(inputdat, tgi.GroupID, tgi.InstanceID);
                    fi.Size = ((IRawData)fi.FileObject).RawData.Length;

                    inputdat.Insert(fi, tgi, false, compress_datmips);
                }
            }
        }
        private void CheckInstance(DatFile4 checkdat, uint group, uint instance)
        {
            for (int n = 0; n < checkdat.Files.Count; n++)
            {
                FileItem chkitem = (FileItem)checkdat.Files[n];
                DatIndex4 chkindex = checkdat.Indexes[chkitem.IndexNumber];
                if (chkindex.TypeID == uint.Parse("7ab50e44", NumberStyles.HexNumber) && chkindex.GroupID == group)
                {
                    if (chkindex.InstanceID == instance)
                    {
                        if (chkindex.Flags != IndexFlags.New)
                        {
                            TGIEntry remtgi = new TGIEntry();
                            remtgi.TypeID = chkindex.TypeID;
                            remtgi.GroupID = chkindex.GroupID;
                            remtgi.InstanceID = chkindex.InstanceID;
                            checkdat.Remove(remtgi);
                        }
                        else 
                        {
                            TGIEntry remtgi = new TGIEntry();
                            remtgi.TypeID = chkindex.TypeID;
                            remtgi.GroupID = chkindex.GroupID;
                            remtgi.InstanceID = chkindex.InstanceID;
                            checkdat.Indexes.Remove(checkdat.Indexes.Find(remtgi.TypeID, remtgi.GroupID, remtgi.InstanceID));
                        }
                    }
                }
            }
        }

        private void saveDatbtn_Click(object sender, EventArgs e)
        {
            if (dat == null)
            {
                dat = new DatFile4();
            } 
            
            try
            {
                if (!batch_processed)
                {
                    ProcessBatch();
                }
                if (compDatcb.Checked && !compress_datmips)
                {
                    compress_datmips = true;
                }

                if (autoprocMipscb.Checked)
                {
                    if (!mipsbtn_clicked)
                    {
                        mipbtn_Click(sender, e);
                        RebuildDat(dat);
                    }
                    else
                    {
                        RebuildDat(dat);
                    }
                }
                else
                {
                    RebuildDat(dat);
                }

                if (dat.Files.Count > 0)
                {
                    if (saveDatDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(dat.FileName) || dat.FileName != saveDatDialog1.FileName)
                            {
                                dat.FileName = saveDatDialog1.FileName;
                                Datnametxt.Text = Path.GetFileName(dat.FileName);
                            }

                            dat.Save();
                            dat.Close();
                            ClearandReset();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this,ex.Message,this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                        finally
                        {
                            dat = null;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\n" + ex.StackTrace,this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void newDatbtn_Click(object sender, EventArgs e)
        {
            this.dat = new DatFile4();
            Datnametxt.Text = "Dat in Memory";
        }

        internal bool batch_processed = false;
        internal void ProcessBatch()
        {
            try
            {
                for (int c = 0; c < BatchlistView1.Items.Count; c++)
                {
                    Bitmap temp = new Bitmap(patharray[c]);
                    bmpitem = new BitmapItem();

                    bmpitem.Bitmap = new Bitmap(patharray[c]);
                    string alname = Path.Combine(Path.GetDirectoryName(patharray[c]), Path.GetFileNameWithoutExtension(patharray[c]) + "_a" + Path.GetExtension(patharray[c]));
                    if (File.Exists(alname))
                    {
                        Bitmap alpha = new Bitmap(alname);
                        bmpitem.Alpha = alpha;
                    }
                    else if (Path.GetExtension(patharray[c]).Equals(".png",StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
                    {
                        Bitmap testbmp = new Bitmap(temp.Width, temp.Height, PixelFormat.Format32bppArgb);

                        for (int y = 0; y < testbmp.Height; y++)
                        {
                            for (int x = 0; x < testbmp.Width; x++)
                            {
                                Color srcpxl = temp.GetPixel(x, y);
                                testbmp.SetPixel(x, y, Color.FromArgb(srcpxl.A,srcpxl.A,srcpxl.A));
                            }
                        }
                        bmpitem.Alpha = testbmp;
                    }
                    else
                    {
                        Bitmap alpha = new Bitmap(temp.Width, temp.Height);
                        for (int y = 0; y < alpha.Height; y++)
                        {
                            for (int x = 0; x < alpha.Width; x++)
                            {
                                alpha.SetPixel(x, y, Color.White);
                            }
                        }
                        bmpitem.Alpha = alpha;
                    }
                    if (typearray[c].ToUpper() == FSHBmpType.ThirtyTwoBit.ToString().ToUpper())
                    {
                        bmpitem.BmpType = FSHBmpType.ThirtyTwoBit;
                    }
                    else if (typearray[c].ToUpper() == FSHBmpType.TwentyFourBit.ToString().ToUpper())
                    {
                        bmpitem.BmpType = FSHBmpType.TwentyFourBit;
                    }
                    else if (typearray[c].ToUpper() == FSHBmpType.DXT1.ToString().ToUpper())
                    {
                        bmpitem.BmpType = FSHBmpType.DXT1;
                    }
                    else if (typearray[c].ToUpper() == FSHBmpType.DXT3.ToString().ToUpper())
                    {
                        bmpitem.BmpType = FSHBmpType.DXT3;
                    }
                    if (temp.Width >= 128 && temp.Height >= 128)
                    {
                        if (c <= curimage.Capacity)
                        {
                            curimage.Insert(c, new FSHImage());
                            curimage[c].Bitmaps.Add(bmpitem);
                            curimage[c].UpdateDirty();

                            using (MemoryStream mstream = new MemoryStream())
                            {
                                SaveFsh(mstream, curimage[c]);
                                if (IsDXTFsh(curimage[c]))
                                {
                                    curimage[c] = new FSHImage(mstream);
                                }
                            }
                        }
                    }
                    
                }
                batch_processed = true;
            }
            catch (ArgumentOutOfRangeException ag)
            {
                MessageBox.Show(this, ag.Message + "\n" + "Param = " + ag.ParamName + " Actual value: " + ag.ActualValue.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                batch_processed = false;
                throw ex;
            }
        }
        internal string Getfilepath(string filepath, string addtopath, string outdir)
        {
            if (outdir != null)
            {
                return Path.Combine(outdir, Path.GetFileNameWithoutExtension(filepath) + addtopath + ".fsh");
            }
            else
            {
                return Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath) + addtopath + ".fsh");
            }
        }
        /// <summary>
        /// Saves a fsh using either FshWrite or FSHLib
        /// </summary>
        /// <param name="fs">The stream to save to</param>
        /// <param name="image">The image to save</param>
        private void SaveFsh(Stream fs, FSHImage image)
        {
            try
            {
                if (UseFshWriteDxt && IsDXTFsh(image))
                {
                    Fshwrite fw = new Fshwrite();
                    foreach (BitmapItem bi in image.Bitmaps)
                    {
                        if ((bi.Bitmap != null && bi.Alpha != null) && bi.BmpType == FSHBmpType.DXT1 || bi.BmpType == FSHBmpType.DXT3)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Test if the fsh only contains DXT1 or DXT3 items
        /// </summary>
        /// <param name="image">The image to test</param>
        /// <returns>True if successful otherwise false</returns>
        private bool IsDXTFsh(FSHImage image)
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
        internal void processbatchbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (batch_processed == false)
                {
                    ProcessBatch();
                }
                if (autoprocMipscb.Checked)
                {
                    mipbtn_Click(sender, e);
                }
                for (int c = 0; c < BatchlistView1.Items.Count; c++)
                {
                    string filepath;

                    if (curimage[c] != null)
                    {
                        filepath = Getfilepath(patharray[c], "", outfolder);
                        using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            SaveFsh(fstream, curimage[c]);
                        }
                        WriteTgi(filepath, 4, c);
                    }
                    if (autoprocMipscb.Checked)
                    {
                        if (mip64fsh[c] != null)
                        {
                            filepath = Getfilepath(patharray[c], "_s3", outfolder);
                            using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                               SaveFsh(fstream, mip64fsh[c]);
                            }
                            WriteTgi(filepath, 3, c);
                        }
                        if (mip32fsh[c] != null)
                        {
                            filepath = Getfilepath(patharray[c], "_s2", outfolder);
                            using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                               SaveFsh(fstream, mip32fsh[c]);
                            }
                            WriteTgi(filepath, 2, c);
                        }
                        if (mip16fsh[c] != null)
                        {
                            filepath = Getfilepath(patharray[c], "_s1", outfolder);
                            using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                               SaveFsh(fstream, mip16fsh[c]);
                            }
                            WriteTgi(filepath, 1, c);
                        }
                        if (mip8fsh[c] != null)
                        {
                            filepath = Getfilepath(patharray[c], "_s0", outfolder);
                            using (FileStream fstream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                               SaveFsh(fstream, mip8fsh[c]);
                            }
                            WriteTgi(filepath, 0, c);
                        }
                    }
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
        private bool UseFshWriteDxt = true;
        private void LoadSettings()
        {
            try
            {
                settings = new Settings(Path.Combine(Application.StartupPath, @"PngtoFshBatch.xml"));
                compDatcb.Checked = bool.Parse(settings.GetSetting("compDatcb_checked", bool.TrueString));
                autoprocMipscb.Checked = bool.Parse(settings.GetSetting("AutoprocessMips", bool.FalseString));
                UseFshWriteDxt = bool.Parse(settings.GetSetting("FshWriteDXTComp", bool.TrueString));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  
        private const uint SSE = 2;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool IsProcessorFeaturePresent(uint ProcessorFeature);

        private void CheckForSSE()
        {
            if (!IsProcessorFeaturePresent(SSE))
            {
                MessageBox.Show("A processor that supports SSE is required to for FshWrite save DXT1 and DXT3 fsh images", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UseFshWriteDxt = false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSettings();
            CheckForSSE();
            FshtypeBox.SelectedIndex = 2;
            grppath = Path.Combine(Application.StartupPath, @"Groupid.txt");
            rangepath = Path.Combine(Application.StartupPath, @"instRange.txt");
            CheckRangeFilesExist(rangepath, false);
            CheckRangeFilesExist(grppath, true);
           
        }
        
        private Random ra = new Random();
        private string lowerinst = null;
        private string upperinst = null;
        internal virtual string RandomHexString(int length)
        {
            const string numbers = "0123456789";
            const string hexcode = "ABCDEF";
            char[] charArray = new char[length];
            string hexstring = string.Empty;

            hexstring += numbers;
            hexstring += hexcode;

            ReadRangetxt(rangepath, false);
            bool copied = false;
            for (int c = 0; c < charArray.Length; c++)
            {
                int index;
                if (!string.IsNullOrEmpty(lowerinst) && !string.IsNullOrEmpty(upperinst))
                {
                    long lower = long.Parse(lowerinst, NumberStyles.HexNumber);
                    long upper = long.Parse(upperinst, NumberStyles.HexNumber);
                    double rn = (upper * 1.0 - lower * 1.0) * ra.NextDouble() + lower * 1.0;
                    return Convert.ToInt64(rn).ToString("X8");
                }
                else
                {
                    if (!copied)
                    {
                        char[] id = DatFile4.GenerateGUID().ToString("X").ToCharArray();
                        Array.Copy(id, charArray, 5);
                        c = 5;
                        copied = true;
                    }
                    index = ra.Next(5, hexstring.Length);
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
                settings.PutSetting("AutoprocessMips", autoprocMipscb.Checked.ToString());
            }
            if (autoprocMipscb.Checked)
            {
                mipbtn.Enabled = false;
            }
            else
            {
                mipbtn.Enabled = true;
            }
        }
        internal string outfolder = null;
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
                                        MessageBox.Show(this, "The group id in Groupid.txt contains invalid characters.\n It must only contain numbers 0-9 and letters A-F.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        throw new FileNotFoundException("Groupid.txt not found at ", path);

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
                                if (inst0.ToUpper().StartsWith("0X"))
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
                                if (inst1.ToUpper().StartsWith("0X"))
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
                        throw new FileNotFoundException("instRange.txt not found at ", path);
                    }
                }
            }
            catch (FileNotFoundException fx)
            {
                throw fx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal bool ValidateHexString(string str)
        {
            string tmp = null;
            if (!string.IsNullOrEmpty(str))
            { 
               
                if (str.Length == 10)
                {
                    if (str.ToLower().StartsWith("0x", StringComparison.OrdinalIgnoreCase))
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
        internal int Countpngs(string[] filenames)
        {                
            int fcnt = 0;
            try
            {
                for (int f = 0; f < filenames.Length; f++)
                {
                    if (filenames[f].StartsWith("/proc", StringComparison.InvariantCultureIgnoreCase) || filenames[f].StartsWith("/mips", StringComparison.InvariantCultureIgnoreCase) || filenames[f].StartsWith("/dat:", StringComparison.InvariantCultureIgnoreCase) || filenames[f].StartsWith("/outdir:", StringComparison.InvariantCultureIgnoreCase) || filenames[f].StartsWith("/?", StringComparison.InvariantCultureIgnoreCase) || filenames[f].StartsWith("/group:", StringComparison.InvariantCultureIgnoreCase))
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
                        if (fi.Extension.Equals(".png") || fi.Extension.Equals(".bmp"))
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
                MessageBox.Show(ex.Message, this.Text);
                return 0;
            }

            
        }
        private void FormatRefresh(int index)
        {
            if (Inst0_4rdo.Checked)
            {
                endreg = Convert.ToChar("4");
                end64 = Convert.ToChar("3");
                end32 = Convert.ToChar("2");
                end16 = Convert.ToChar("1");
                end8 = Convert.ToChar("0");
            }
            else if (Inst5_9rdo.Checked)
            {
                endreg = Convert.ToChar("9");
                end64 = Convert.ToChar("8");
                end32 = Convert.ToChar("7");
                end16 = Convert.ToChar("6");
                end8 = Convert.ToChar("5");
            }
            else if (InstA_Erdo.Checked)
            {
                endreg = Convert.ToChar("E");
                end64 = Convert.ToChar("D");
                end32 = Convert.ToChar("C");
                end16 = Convert.ToChar("B");
                end8 = Convert.ToChar("A");
            }
            Bitmap temp = new Bitmap(patharray[index]);
            if (temp.Width >= 128 && temp.Height >= 128)
            {
                if (Inst0_4rdo.Checked)
                {
                    instarray[index] = instarray[index].Substring(0, 7) + endreg;
                }
                else if (Inst5_9rdo.Checked)
                {
                    instarray[index] = instarray[index].Substring(0, 7) + endreg;
                }
                else if (InstA_Erdo.Checked)
                {
                    instarray[index] = instarray[index].Substring(0, 7) + endreg;
                }
            }
            
            BatchlistView1.SelectedItems[0].SubItems[3].Text = instarray[index];
            tgiInstancetxt.Text = instarray[index];
        }
        private void Format_radios_changed(object sender, EventArgs e)
        {
            if (BatchlistView1.SelectedItems.Count > 0)
            {
                FormatRefresh(BatchlistView1.SelectedItems[0].Index);
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
                CheckSize(0);
                if (tgiGrouptxt.Text.Length > 0 && tgiGrouptxt.Text.Length == 8)
                {
                    for (int j = 0; j < patharray.Count; j++)
                    {
                        grouparray.Insert(j, tgiGrouptxt.Text);
                    }
                }
                else
                {
                    ReadRangetxt(grppath, true);
                }


                for (int n = 0; n < patharray.Count; n++)
                {
                    if (Path.GetFileName(patharray[n]).StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
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
                        if (!Path.GetFileName(patharray[n]).StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (temp.Width >= 128 && temp.Height >= 128)
                            {
                                if (Inst0_4rdo.Checked)
                                {
                                    instarray[n] = instarray[n].Substring(0, 7) + "4";
                                }
                                else if (Inst5_9rdo.Checked)
                                {
                                    instarray[n] = instarray[n].Substring(0, 7) + "9";
                                }
                                else if (InstA_Erdo.Checked)
                                {
                                    instarray[n] = instarray[n].Substring(0, 7) + "E";
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }


                        string alname = Path.Combine(Path.GetDirectoryName(patharray[n]), Path.GetFileNameWithoutExtension(patharray[n]) + "_a" + Path.GetExtension(patharray[n]));
                        string fn = Path.GetFileName(patharray[n]);
                        if (File.Exists(alname))
                        {
                            typearray.Insert(n, "DXT3");
                        }
                        else if (temp.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.InvariantCultureIgnoreCase))
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
                            if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.InvariantCultureIgnoreCase))
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
                        BatchlistView1.Items.Add(item1);
                    }
                }
                /*Debug.Assert(patharray.Count == BatchlistView1.Items.Count,"The number of items in the batch and patharray lists do not match"
                    ,string.Concat("patharray count: " + patharray.Count.ToString()," does not match ", "BatchlistView count: " +BatchlistView1.Items.Count.ToString()));*/
            }
            catch (FileNotFoundException fx)
            {
                MessageBox.Show(fx.Message + fx.FileName, this.Text,MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }
       
        private void rembtn_Click(object sender, EventArgs e)
        {
            if (BatchlistView1.SelectedItems.Count > 0 && BatchlistView1.Items.Count > 1)
            {
                try
                {
                    int index = BatchlistView1.SelectedItems[0].Index;
                    grouparray.RemoveAt(index);
                    instarray.RemoveAt(index);
                    typearray.RemoveAt(index);
                    patharray.RemoveAt(index);
                    if (curimage.Count > 0)
                    {
                        if (curimage[index] != null)
                        {
                            curimage.RemoveAt(index);
                            curimage.Capacity = curimage.Count;
                        }
                    }
                    if (mipsbtn_clicked)
                    {
                        if (mip64fsh[index] != null && mip32fsh[index] != null && mip16fsh[index] != null && mip8fsh[index] != null)
                        {
                            mip64fsh.RemoveAt(index);
                            mip32fsh.RemoveAt(index);
                            mip16fsh.RemoveAt(index);
                            mip8fsh.RemoveAt(index);
                        }
                    }
                    BatchlistView1.Items.RemoveAt(index);
                    ListViewItem selitem = BatchlistView1.Items[0];
                    selitem.Selected = true;
                    BatchlistView1.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text);
                }
            }
            else if (BatchlistView1.SelectedItems.Count > 0 && BatchlistView1.Items.Count == 1)
            {
                ClearandReset();
            }

        }
        /// <summary>
        /// Checks the list for images 64 x 64 or smaller
        /// </summary>
        /// <param name="dif">The index to start checking the list at</param>
        private void CheckSize(int dif)
        {
            try
            {
                ArrayList remlist = new ArrayList();  // list of file indexes to remove.              

                for (int n = dif; n < patharray.Count; n++)
                {
                    using (Bitmap bmp = new Bitmap(patharray[n]))
                    {
                        if (bmp.Width <= 64 && bmp.Height <= 64)
                        {
                            remlist.Add(n);
                        }
                    }
                }                    
                if (remlist.Count > 0)
                {
                    int filesrem = 0; // offset the index by  the number of files removed.  
                    foreach (int rl in remlist)
                    {
                        int rem = (rl - filesrem);
                        patharray.RemoveAt(rem);
                        patharray.Capacity = (patharray.Capacity - 1);
                        grouparray.Capacity = (grouparray.Capacity - 1);
                        instarray.Capacity = (instarray.Capacity - 1);
                        typearray.Capacity = (typearray.Capacity - 1);
                        filesrem++; 
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void BuildAddList(int fcnt,int dif)
        {
            try
            {
                
                CheckSize(dif);
                if (tgiGrouptxt.Text.Length > 0 && tgiGrouptxt.Text.Length == 8)
                {
                    for (int j = 0; j < patharray.Count; j++)
                    {
                        grouparray.Insert(j, tgiGrouptxt.Text);
                    }
                }
                else
                {
                    ReadRangetxt(grppath, true);
                }

                for (int n = dif; n < patharray.Count; n++)
                {
                    if (Path.GetFileName(patharray[n]).StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
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
                        if (!Path.GetFileName(patharray[n]).StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (temp.Width >= 128 && temp.Height >= 128)
                            {
                                if (Inst0_4rdo.Checked)
                                {
                                    instarray[n] = instarray[n].Substring(0, 7) + "4";
                                }
                                else if (Inst5_9rdo.Checked)
                                {
                                    instarray[n] = instarray[n].Substring(0, 7) + "9";
                                }
                                else if (InstA_Erdo.Checked)
                                {
                                    instarray[n] = instarray[n].Substring(0, 7) + "E";
                                }
                            }
                        }

                        string alname = Path.Combine(Path.GetDirectoryName(patharray[n]), Path.GetFileNameWithoutExtension(patharray[n]) + "_a" + Path.GetExtension(patharray[n]));
                        string fn = Path.GetFileName(patharray[n]);
                        if (File.Exists(alname))
                        {
                            typearray.Insert(n, "DXT3");
                        }
                        else if (Path.GetExtension(patharray[n]).Equals(".png",StringComparison.OrdinalIgnoreCase) && temp.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.InvariantCultureIgnoreCase))
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
                            if (temp.Width >= 256 && temp.Height >= 256 && fn.StartsWith("hd", StringComparison.InvariantCultureIgnoreCase))
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
                        BatchlistView1.Items.Insert(i, item1);
                    }
                }
            }
            catch (FileNotFoundException fx)
            {
                MessageBox.Show(fx.Message + fx.FileName, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }
        private void addbtn_Click(object sender, EventArgs e)
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
                if (curimage == null && mip64fsh == null && mip32fsh == null && mip16fsh == null && mip8fsh == null)
                { 
                    curimage = new List<FSHImage>(fcnt);
                    mip64fsh = new List<FSHImage>(fcnt);
                    mip32fsh = new List<FSHImage>(fcnt);
                    mip16fsh = new List<FSHImage>(fcnt);
                    mip8fsh = new List<FSHImage>(fcnt);
                }
                curimage.Capacity = cnt;
                mip64fsh.Capacity = cnt;
                mip32fsh.Capacity = cnt;
                mip16fsh.Capacity = cnt;
                mip8fsh.Capacity = cnt;
                patharray.Capacity = cnt;
                grouparray.Capacity = cnt;
                instarray.Capacity = cnt;
                typearray.Capacity = cnt;
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
                    BuildAddList(fcnt,dif);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.Text,MessageBoxButtons.OK,MessageBoxIcon.Error);
                    ClearandReset();
                }
            }
            
        }
        private void TgiGrouptxt_KeyDown(object sender, KeyEventArgs e)
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

        private void tgiGrouptxt_TextChanged(object sender, EventArgs e)
        {                
            if (BatchlistView1.SelectedItems.Count > 0)
            {
                int index = BatchlistView1.SelectedItems[0].Index;
                if (tgiGrouptxt.Text.Length > 0 && tgiGrouptxt.Text.Length == 8)
                {
                    if (!tgiGrouptxt.Text.Equals(grouparray[index], StringComparison.InvariantCultureIgnoreCase))
                    {
                        grouparray[index] = tgiGrouptxt.Text;
                        BatchlistView1.SelectedItems[0].SubItems[2].Text = grouparray[index];
                    }
                }
            }
        }

        private void tgiInstancetxt_TextChanged(object sender, EventArgs e)
        {
            if (BatchlistView1.SelectedItems.Count > 0)
            {
                int index = BatchlistView1.SelectedItems[0].Index;
                if (tgiInstancetxt.Text.Length > 0 && tgiInstancetxt.Text.Length == 8)
                {
                    if (!tgiInstancetxt.Text.Equals(instarray[index], StringComparison.InvariantCultureIgnoreCase))
                    {
                        instarray.Insert(index, tgiInstancetxt.Text);
                        FormatRefresh(index);
                    }
                }
            }
        }

        private void FshtypeBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (BatchlistView1.SelectedItems.Count > 0)
            {
                int index = BatchlistView1.SelectedItems[0].Index;
                string seltype = null;
                using (Bitmap b = new Bitmap(patharray[index]))
                {
                    switch (FshtypeBox.SelectedIndex)
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
                if (!seltype.ToUpper().Equals(typearray[index], StringComparison.OrdinalIgnoreCase))
                {
                    typearray[index] = seltype;
                }
            }
        }

        private void BatchlistView1_DragEnter(object sender, DragEventArgs e)
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
                        if (fi.Extension.Equals(".png") || fi.Extension.Equals(".bmp"))
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
                }
                else
                {
                        FileInfo fi = new FileInfo(filenames[f]);
                        if (fi.Extension.Equals(".png") || fi.Extension.Equals(".bmp"))
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

        private void BatchlistView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = GetFilesfromDirectory((string[])e.Data.GetData(DataFormats.FileDrop));
            if (BatchlistView1.Items.Count > 0)
            {
                ClearandReset();
            }
            int fcnt = Countpngs(files);
            int pngcnt = -1;
            patharray = new List<string>(fcnt);
            instarray = new List<string>(fcnt);
            grouparray = new List<string>(fcnt);
            typearray = new List<string>(fcnt);
            curimage = new List<FSHImage>(fcnt);
            mip64fsh = new List<FSHImage>(fcnt);
            mip32fsh = new List<FSHImage>(fcnt);
            mip16fsh = new List<FSHImage>(fcnt);
            mip8fsh = new List<FSHImage>(fcnt);
            for (int f = 0; f < files.Length; f++)
            {
                FileInfo fi = new FileInfo(files[f]);
                if (fi.Exists)
                {

                    if (fi.Extension.Equals(".png") || fi.Extension.Equals(".bmp"))
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

        private void addbtn_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void ClearandReset()
        {
            BatchlistView1.Items.Clear();
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
            if (mip64fsh != null && mip32fsh != null && mip16fsh != null && mip8fsh != null)
            {
                mip64fsh.Clear();
                mip64fsh = null;
                mip32fsh.Clear();
                mip32fsh = null;
                mip16fsh.Clear();
                mip16fsh = null;
                mip8fsh.Clear();
                mip8fsh = null;
            }
            if (curimage != null)
            {
                curimage.Clear();
                curimage = null;
            }
            if (outfolder != null)
            {
                outfolder = null;
            }
            mipsbtn_clicked = false;
            batch_processed = false;
            tgiGrouptxt.Text = null;
            tgiInstancetxt.Text = null;
            FshtypeBox.SelectedIndex = 2;
        }
        private void addbtn_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = GetFilesfromDirectory((string[])e.Data.GetData(DataFormats.FileDrop));
            int fcnt = Countpngs(files);
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
            if (curimage == null && mip64fsh == null && mip32fsh == null && mip16fsh == null && mip8fsh == null)
            {
                curimage = new List<FSHImage>(fcnt);
                mip64fsh = new List<FSHImage>(fcnt);
                mip32fsh = new List<FSHImage>(fcnt);
                mip16fsh = new List<FSHImage>(fcnt);
                mip8fsh = new List<FSHImage>(fcnt);
            }
            curimage.Capacity = cnt;
            mip64fsh.Capacity = cnt;
            mip32fsh.Capacity = cnt;
            mip16fsh.Capacity = cnt;
            mip8fsh.Capacity = cnt;
            patharray.Capacity = cnt;
            grouparray.Capacity = cnt;
            instarray.Capacity = cnt;
            typearray.Capacity = cnt;
            try 
	        {
                for (int f = 0; f < files.Length; f++)
                {
                    FileInfo fi = new FileInfo(files[f]);
                    if (fi.Exists)
                    {

                        if (fi.Extension.Equals(".png") || fi.Extension.Equals(".bmp"))
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
                    dif = cnt - fcnt;
                }
                else
                {
                    dif = 0;
                }
                BuildAddList(fcnt,dif);
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
        private void FshtypeBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (BatchlistView1.SelectedItems.Count > 0)
            {
                if (!Checkhdimgsize(BatchlistView1.SelectedItems[0].Index))
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

        private void FshtypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BatchlistView1.SelectedItems.Count > 0)
            { 
                if (!Checkhdimgsize(BatchlistView1.SelectedItems[0].Index))
                {
                    // if it is not 256 disable hd fsh
                    if (FshtypeBox.SelectedIndex == 0 || FshtypeBox.SelectedIndex == 1)
                    {
                        FshtypeBox.SelectedIndex = typeindex;
                    }
                }
            }
        }
        private void CheckRangeFilesExist(string path,bool group)
        {
            try
            {                    
                FileInfo fi = new FileInfo(path);
                if (group)
                {
                    if (!fi.Exists)
                    {
                        throw new FileNotFoundException("Groupid.txt not found at ", fi.FullName);
                    }
                }
                else
                {
                    if (!fi.Exists)
                    {
                        throw new FileNotFoundException("instRange.txt not found at ", fi.FullName);
                    }
                }                
            }
            catch (FileNotFoundException fx)
            {
                string message = string.Concat(fx.Message, fx.FileName);
                MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
