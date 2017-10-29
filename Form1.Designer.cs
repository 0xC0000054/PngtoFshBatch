using PngtoFshBatchtxt.Properties;
namespace PngtoFshBatchtxt
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                if (dat != null)
                { 
                    dat.Dispose();
                    dat = null;
                }

                if (batchFshList != null)
                {
                    batchFshList.Dispose();
                    batchFshList = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.batchListView = new System.Windows.Forms.ListView();
            this.bitmapHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.alphaHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.instanceHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.compDatCb = new System.Windows.Forms.CheckBox();
            this.datFuncBox = new System.Windows.Forms.GroupBox();
            this.newDatbtn = new System.Windows.Forms.Button();
            this.saveDatBtn = new System.Windows.Forms.Button();
            this.processBatchBtn = new System.Windows.Forms.Button();
            this.saveDatDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.outFolderBtn = new System.Windows.Forms.Button();
            this.outputBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.addFilesDialog = new System.Windows.Forms.OpenFileDialog();
            this.InstendBox1 = new System.Windows.Forms.GroupBox();
            this.instA_ERdo = new System.Windows.Forms.RadioButton();
            this.inst5_9Rdo = new System.Windows.Forms.RadioButton();
            this.inst0_4Rdo = new System.Windows.Forms.RadioButton();
            this.remBtn = new System.Windows.Forms.Button();
            this.addBtn = new System.Windows.Forms.Button();
            this.fshTypeBox = new System.Windows.Forms.ComboBox();
            this.tgiInstlbl = new System.Windows.Forms.Label();
            this.tgiGrouplbl = new System.Windows.Forms.Label();
            this.tgiInstanceTxt = new System.Windows.Forms.TextBox();
            this.tgiGroupTxt = new System.Windows.Forms.TextBox();
            this.clearListBtn = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.fshWriteCompCb = new System.Windows.Forms.CheckBox();
            this.mipFormatCbo = new System.Windows.Forms.ComboBox();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.listIndexChangedTimer = new System.Windows.Forms.Timer(this.components);
            this.datFuncBox.SuspendLayout();
            this.InstendBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // batchListView
            // 
            this.batchListView.AllowDrop = true;
            this.batchListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.batchListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.bitmapHeader,
            this.alphaHeader,
            this.groupHeader,
            this.instanceHeader});
            this.batchListView.FullRowSelect = true;
            this.batchListView.HideSelection = false;
            this.batchListView.Location = new System.Drawing.Point(12, 12);
            this.batchListView.MultiSelect = false;
            this.batchListView.Name = "batchListView";
            this.batchListView.Size = new System.Drawing.Size(467, 132);
            this.batchListView.TabIndex = 0;
            this.toolTip1.SetToolTip(this.batchListView, global::PngtoFshBatchtxt.Properties.Resources.batchListView_ToolTip);
            this.batchListView.UseCompatibleStateImageBehavior = false;
            this.batchListView.View = System.Windows.Forms.View.Details;
            this.batchListView.VirtualMode = true;
            this.batchListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.batchListView_ColumnClick);
            this.batchListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.batchListView_RetrieveVirtualItem);
            this.batchListView.SelectedIndexChanged += new System.EventHandler(this.batchListView_SelectedIndexChanged);
            this.batchListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.batchListView_DragDrop);
            this.batchListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.batchListView_DragEnter);
            // 
            // bitmapHeader
            // 
            this.bitmapHeader.Text = global::PngtoFshBatchtxt.Properties.Resources.bitmapHeader_Text;
            this.bitmapHeader.Width = 96;
            // 
            // alphaHeader
            // 
            this.alphaHeader.Text = global::PngtoFshBatchtxt.Properties.Resources.alphaHeader_Text;
            this.alphaHeader.Width = 115;
            // 
            // groupHeader
            // 
            this.groupHeader.Text = global::PngtoFshBatchtxt.Properties.Resources.groupHeader_Text;
            this.groupHeader.Width = 96;
            // 
            // instanceHeader
            // 
            this.instanceHeader.Text = global::PngtoFshBatchtxt.Properties.Resources.instanceHeader_Text;
            this.instanceHeader.Width = 96;
            // 
            // compDatCb
            // 
            this.compDatCb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.compDatCb.AutoSize = true;
            this.compDatCb.Checked = true;
            this.compDatCb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.compDatCb.Location = new System.Drawing.Point(12, 173);
            this.compDatCb.Name = "compDatCb";
            this.compDatCb.Size = new System.Drawing.Size(90, 17);
            this.compDatCb.TabIndex = 3;
            this.compDatCb.Text = "Compress dat";
            this.toolTip1.SetToolTip(this.compDatCb, global::PngtoFshBatchtxt.Properties.Resources.compDatcb_ToolTip);
            this.compDatCb.UseVisualStyleBackColor = true;
            this.compDatCb.CheckedChanged += new System.EventHandler(this.compDatcb_CheckedChanged);
            // 
            // datFuncBox
            // 
            this.datFuncBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.datFuncBox.Controls.Add(this.newDatbtn);
            this.datFuncBox.Controls.Add(this.saveDatBtn);
            this.datFuncBox.Location = new System.Drawing.Point(318, 150);
            this.datFuncBox.Name = "datFuncBox";
            this.datFuncBox.Size = new System.Drawing.Size(167, 45);
            this.datFuncBox.TabIndex = 12;
            this.datFuncBox.TabStop = false;
            this.datFuncBox.Text = "Dat Functions";
            // 
            // newDatbtn
            // 
            this.newDatbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newDatbtn.Enabled = false;
            this.newDatbtn.Location = new System.Drawing.Point(6, 15);
            this.newDatbtn.Name = "newDatbtn";
            this.newDatbtn.Size = new System.Drawing.Size(75, 23);
            this.newDatbtn.TabIndex = 12;
            this.newDatbtn.Text = "New";
            this.newDatbtn.UseVisualStyleBackColor = true;
            this.newDatbtn.Click += new System.EventHandler(this.newDatbtn_Click);
            // 
            // saveDatBtn
            // 
            this.saveDatBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveDatBtn.Enabled = false;
            this.saveDatBtn.Location = new System.Drawing.Point(87, 15);
            this.saveDatBtn.Name = "saveDatBtn";
            this.saveDatBtn.Size = new System.Drawing.Size(75, 23);
            this.saveDatBtn.TabIndex = 15;
            this.saveDatBtn.Text = "Save...";
            this.toolTip1.SetToolTip(this.saveDatBtn, global::PngtoFshBatchtxt.Properties.Resources.saveDatBtn_ToolTip);
            this.saveDatBtn.UseVisualStyleBackColor = true;
            this.saveDatBtn.Click += new System.EventHandler(this.saveDatbtn_Click);
            // 
            // processBatchBtn
            // 
            this.processBatchBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.processBatchBtn.Enabled = false;
            this.processBatchBtn.Location = new System.Drawing.Point(399, 281);
            this.processBatchBtn.Name = "processBatchBtn";
            this.processBatchBtn.Size = new System.Drawing.Size(75, 23);
            this.processBatchBtn.TabIndex = 14;
            this.processBatchBtn.Text = "Process";
            this.toolTip1.SetToolTip(this.processBatchBtn, global::PngtoFshBatchtxt.Properties.Resources.processBatchBtn_ToolTip);
            this.processBatchBtn.UseVisualStyleBackColor = true;
            this.processBatchBtn.Click += new System.EventHandler(this.processbatchbtn_Click);
            // 
            // saveDatDialog1
            // 
            this.saveDatDialog1.DefaultExt = "dat";
            this.saveDatDialog1.Filter = "Dat files (*.dat)|*.dat|All files (*.*)|*.*";
            this.saveDatDialog1.RestoreDirectory = true;
            // 
            // outFolderBtn
            // 
            this.outFolderBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.outFolderBtn.Location = new System.Drawing.Point(300, 281);
            this.outFolderBtn.Name = "outFolderBtn";
            this.outFolderBtn.Size = new System.Drawing.Size(93, 23);
            this.outFolderBtn.TabIndex = 11;
            this.outFolderBtn.Text = "Output Folder...";
            this.toolTip1.SetToolTip(this.outFolderBtn, global::PngtoFshBatchtxt.Properties.Resources.outFolderBtn_ToolTip);
            this.outFolderBtn.UseVisualStyleBackColor = true;
            this.outFolderBtn.Click += new System.EventHandler(this.outfolderbtn_Click);
            // 
            // addFilesDialog
            // 
            this.addFilesDialog.Filter = "Bitmap files (*.png;*.bmp)|*.png;*.bmp";
            this.addFilesDialog.Multiselect = true;
            // 
            // InstendBox1
            // 
            this.InstendBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.InstendBox1.Controls.Add(this.instA_ERdo);
            this.InstendBox1.Controls.Add(this.inst5_9Rdo);
            this.InstendBox1.Controls.Add(this.inst0_4Rdo);
            this.InstendBox1.Location = new System.Drawing.Point(174, 150);
            this.InstendBox1.Name = "InstendBox1";
            this.InstendBox1.Size = new System.Drawing.Size(80, 78);
            this.InstendBox1.TabIndex = 7;
            this.InstendBox1.TabStop = false;
            this.InstendBox1.Text = "End format";
            // 
            // instA_ERdo
            // 
            this.instA_ERdo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.instA_ERdo.AutoSize = true;
            this.instA_ERdo.Enabled = false;
            this.instA_ERdo.Location = new System.Drawing.Point(6, 55);
            this.instA_ERdo.Name = "instA_ERdo";
            this.instA_ERdo.Size = new System.Drawing.Size(42, 17);
            this.instA_ERdo.TabIndex = 9;
            this.instA_ERdo.Text = "A-E";
            this.toolTip1.SetToolTip(this.instA_ERdo, global::PngtoFshBatchtxt.Properties.Resources.InstanceIDRadioButtons_ToolTip);
            this.instA_ERdo.UseVisualStyleBackColor = true;
            this.instA_ERdo.CheckedChanged += new System.EventHandler(this.Format_radios_changed);
            // 
            // inst5_9Rdo
            // 
            this.inst5_9Rdo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.inst5_9Rdo.AutoSize = true;
            this.inst5_9Rdo.Enabled = false;
            this.inst5_9Rdo.Location = new System.Drawing.Point(6, 36);
            this.inst5_9Rdo.Name = "inst5_9Rdo";
            this.inst5_9Rdo.Size = new System.Drawing.Size(40, 17);
            this.inst5_9Rdo.TabIndex = 8;
            this.inst5_9Rdo.Text = "5-9";
            this.toolTip1.SetToolTip(this.inst5_9Rdo, global::PngtoFshBatchtxt.Properties.Resources.InstanceIDRadioButtons_ToolTip);
            this.inst5_9Rdo.UseVisualStyleBackColor = true;
            this.inst5_9Rdo.CheckedChanged += new System.EventHandler(this.Format_radios_changed);
            // 
            // inst0_4Rdo
            // 
            this.inst0_4Rdo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.inst0_4Rdo.AutoSize = true;
            this.inst0_4Rdo.Enabled = false;
            this.inst0_4Rdo.Location = new System.Drawing.Point(6, 19);
            this.inst0_4Rdo.Name = "inst0_4Rdo";
            this.inst0_4Rdo.Size = new System.Drawing.Size(40, 17);
            this.inst0_4Rdo.TabIndex = 7;
            this.inst0_4Rdo.Text = "0-4";
            this.toolTip1.SetToolTip(this.inst0_4Rdo, global::PngtoFshBatchtxt.Properties.Resources.InstanceIDRadioButtons_ToolTip);
            this.inst0_4Rdo.UseVisualStyleBackColor = true;
            this.inst0_4Rdo.CheckedChanged += new System.EventHandler(this.Format_radios_changed);
            // 
            // remBtn
            // 
            this.remBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.remBtn.Enabled = false;
            this.remBtn.Location = new System.Drawing.Point(399, 226);
            this.remBtn.Name = "remBtn";
            this.remBtn.Size = new System.Drawing.Size(75, 23);
            this.remBtn.TabIndex = 16;
            this.remBtn.Text = "Remove";
            this.toolTip1.SetToolTip(this.remBtn, global::PngtoFshBatchtxt.Properties.Resources.remBtn_ToolTip);
            this.remBtn.UseVisualStyleBackColor = true;
            this.remBtn.Click += new System.EventHandler(this.remBtn_Click);
            // 
            // addBtn
            // 
            this.addBtn.AllowDrop = true;
            this.addBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addBtn.Location = new System.Drawing.Point(318, 226);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(75, 23);
            this.addBtn.TabIndex = 13;
            this.addBtn.Text = "Add...";
            this.toolTip1.SetToolTip(this.addBtn, global::PngtoFshBatchtxt.Properties.Resources.addBtn_ToolTip);
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            this.addBtn.DragDrop += new System.Windows.Forms.DragEventHandler(this.addBtn_DragDrop);
            this.addBtn.DragEnter += new System.Windows.Forms.DragEventHandler(this.addBtn_DragEnter);
            // 
            // fshTypeBox
            // 
            this.fshTypeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fshTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fshTypeBox.Enabled = false;
            this.fshTypeBox.FormattingEnabled = true;
            this.fshTypeBox.Items.AddRange(new object[] {
            "24 Bit RGB (0:8:8:8)",
            "32 Bit ARGB (8:8:8:8)",
            "DXT1 Compressed, no Alpha",
            "DXT3 Compressed, with Alpha "});
            this.fshTypeBox.Location = new System.Drawing.Point(12, 283);
            this.fshTypeBox.Name = "fshTypeBox";
            this.fshTypeBox.Size = new System.Drawing.Size(172, 21);
            this.fshTypeBox.TabIndex = 6;
            this.toolTip1.SetToolTip(this.fshTypeBox, global::PngtoFshBatchtxt.Properties.Resources.fshTypeBox_ToolTip);
            this.fshTypeBox.SelectionChangeCommitted += new System.EventHandler(this.fshTypeBox_SelectionChangeCommitted);
            // 
            // tgiInstlbl
            // 
            this.tgiInstlbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiInstlbl.AutoSize = true;
            this.tgiInstlbl.BackColor = System.Drawing.SystemColors.Control;
            this.tgiInstlbl.Location = new System.Drawing.Point(13, 255);
            this.tgiInstlbl.Name = "tgiInstlbl";
            this.tgiInstlbl.Size = new System.Drawing.Size(48, 13);
            this.tgiInstlbl.TabIndex = 105;
            this.tgiInstlbl.Text = "Instance";
            // 
            // tgiGrouplbl
            // 
            this.tgiGrouplbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiGrouplbl.AutoSize = true;
            this.tgiGrouplbl.Location = new System.Drawing.Point(25, 229);
            this.tgiGrouplbl.Name = "tgiGrouplbl";
            this.tgiGrouplbl.Size = new System.Drawing.Size(36, 13);
            this.tgiGrouplbl.TabIndex = 104;
            this.tgiGrouplbl.Text = "Group";
            // 
            // tgiInstanceTxt
            // 
            this.tgiInstanceTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiInstanceTxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tgiInstanceTxt.Enabled = false;
            this.tgiInstanceTxt.Location = new System.Drawing.Point(67, 252);
            this.tgiInstanceTxt.MaxLength = 8;
            this.tgiInstanceTxt.Name = "tgiInstanceTxt";
            this.tgiInstanceTxt.Size = new System.Drawing.Size(82, 20);
            this.tgiInstanceTxt.TabIndex = 5;
            this.toolTip1.SetToolTip(this.tgiInstanceTxt, global::PngtoFshBatchtxt.Properties.Resources.tgiInstanceTxt_ToolTip);
            this.tgiInstanceTxt.TextChanged += new System.EventHandler(this.tgiInstanceTxt_TextChanged);
            this.tgiInstanceTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tgiInstanceTxt_KeyDown);
            // 
            // tgiGroupTxt
            // 
            this.tgiGroupTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiGroupTxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tgiGroupTxt.Location = new System.Drawing.Point(67, 226);
            this.tgiGroupTxt.MaxLength = 8;
            this.tgiGroupTxt.Name = "tgiGroupTxt";
            this.tgiGroupTxt.Size = new System.Drawing.Size(82, 20);
            this.tgiGroupTxt.TabIndex = 4;
            this.toolTip1.SetToolTip(this.tgiGroupTxt, global::PngtoFshBatchtxt.Properties.Resources.tgiGroupTxt_ToolTip);
            this.tgiGroupTxt.TextChanged += new System.EventHandler(this.tgiGroupTxt_TextChanged);
            this.tgiGroupTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tgiGroupTxt_KeyDown);
            // 
            // clearListBtn
            // 
            this.clearListBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearListBtn.Enabled = false;
            this.clearListBtn.Location = new System.Drawing.Point(399, 252);
            this.clearListBtn.Name = "clearListBtn";
            this.clearListBtn.Size = new System.Drawing.Size(75, 23);
            this.clearListBtn.TabIndex = 106;
            this.clearListBtn.Text = "Clear List";
            this.clearListBtn.UseVisualStyleBackColor = true;
            this.clearListBtn.Click += new System.EventHandler(this.clearListBtn_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 1000;
            this.toolTip1.ReshowDelay = 500;
            // 
            // fshWriteCompCb
            // 
            this.fshWriteCompCb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fshWriteCompCb.AutoSize = true;
            this.fshWriteCompCb.Location = new System.Drawing.Point(12, 190);
            this.fshWriteCompCb.Name = "fshWriteCompCb";
            this.fshWriteCompCb.Size = new System.Drawing.Size(128, 17);
            this.fshWriteCompCb.TabIndex = 107;
            this.fshWriteCompCb.Text = "Fshwrite Compression";
            this.toolTip1.SetToolTip(this.fshWriteCompCb, global::PngtoFshBatchtxt.Properties.Resources.fshWriteCompCb_ToolTip);
            this.fshWriteCompCb.UseVisualStyleBackColor = true;
            this.fshWriteCompCb.CheckedChanged += new System.EventHandler(this.fshWriteCompCb_CheckedChanged);
            // 
            // mipFormatCbo
            // 
            this.mipFormatCbo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mipFormatCbo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mipFormatCbo.FormattingEnabled = true;
            this.mipFormatCbo.Items.AddRange(new object[] {
            "Normal Mipmaps",
            "Embedded Mipmaps",
            "No Mipmaps"});
            this.mipFormatCbo.Location = new System.Drawing.Point(12, 150);
            this.mipFormatCbo.Name = "mipFormatCbo";
            this.mipFormatCbo.Size = new System.Drawing.Size(127, 21);
            this.mipFormatCbo.TabIndex = 110;
            this.toolTip1.SetToolTip(this.mipFormatCbo, global::PngtoFshBatchtxt.Properties.Resources.mipFormatCbo_Tooltip);
            this.mipFormatCbo.SelectedIndexChanged += new System.EventHandler(this.mipFormatCbo_SelectedIndexChanged);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Enabled = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(300, 16);
            this.toolStripProgressBar1.Step = 1;
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripProgressStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 317);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(493, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 108;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressStatus
            // 
            this.toolStripProgressStatus.Name = "toolStripProgressStatus";
            this.toolStripProgressStatus.Size = new System.Drawing.Size(39, 17);
            this.toolStripProgressStatus.Text = global::PngtoFshBatchtxt.Properties.Resources.StatusReadyText;
            // 
            // listIndexChangedTimer
            // 
            this.listIndexChangedTimer.Interval = 50;
            this.listIndexChangedTimer.Tick += new System.EventHandler(this.listIndexChangedTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 339);
            this.Controls.Add(this.mipFormatCbo);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.fshWriteCompCb);
            this.Controls.Add(this.clearListBtn);
            this.Controls.Add(this.tgiInstlbl);
            this.Controls.Add(this.tgiGrouplbl);
            this.Controls.Add(this.tgiInstanceTxt);
            this.Controls.Add(this.tgiGroupTxt);
            this.Controls.Add(this.fshTypeBox);
            this.Controls.Add(this.addBtn);
            this.Controls.Add(this.remBtn);
            this.Controls.Add(this.InstendBox1);
            this.Controls.Add(this.outFolderBtn);
            this.Controls.Add(this.processBatchBtn);
            this.Controls.Add(this.compDatCb);
            this.Controls.Add(this.datFuncBox);
            this.Controls.Add(this.batchListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimumSize = new System.Drawing.Size(499, 342);
            this.Name = "Form1";
            this.Text = "Png to Fsh Batch";
            this.datFuncBox.ResumeLayout(false);
            this.InstendBox1.ResumeLayout(false);
            this.InstendBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader bitmapHeader;
        private System.Windows.Forms.ColumnHeader alphaHeader;
        private System.Windows.Forms.CheckBox compDatCb;
        private System.Windows.Forms.GroupBox datFuncBox;
        private System.Windows.Forms.Button newDatbtn;
        private System.Windows.Forms.Button saveDatBtn;
        internal System.Windows.Forms.Button processBatchBtn;
        private System.Windows.Forms.SaveFileDialog saveDatDialog1;
        private System.Windows.Forms.ColumnHeader groupHeader;
        private System.Windows.Forms.ColumnHeader instanceHeader;
        private System.Windows.Forms.Button outFolderBtn;
        private System.Windows.Forms.FolderBrowserDialog outputBrowserDialog;
        private System.Windows.Forms.OpenFileDialog addFilesDialog;
        private System.Windows.Forms.GroupBox InstendBox1;
        private System.Windows.Forms.RadioButton instA_ERdo;
        private System.Windows.Forms.RadioButton inst5_9Rdo;
        private System.Windows.Forms.RadioButton inst0_4Rdo;
        private System.Windows.Forms.Button remBtn;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.ComboBox fshTypeBox;
        private System.Windows.Forms.Label tgiInstlbl;
        private System.Windows.Forms.Label tgiGrouplbl;
        private System.Windows.Forms.TextBox tgiInstanceTxt;
        private System.Windows.Forms.Button clearListBtn;
        internal System.Windows.Forms.ListView batchListView;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripProgressStatus;
        internal System.Windows.Forms.CheckBox fshWriteCompCb;
        internal System.Windows.Forms.ComboBox mipFormatCbo;
        private System.Windows.Forms.TextBox tgiGroupTxt;
        private System.Windows.Forms.Timer listIndexChangedTimer;
    }
}

