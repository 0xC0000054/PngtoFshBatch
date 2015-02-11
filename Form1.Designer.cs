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
            this.OutputBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.PngopenDialog = new System.Windows.Forms.OpenFileDialog();
            this.InstendBox1 = new System.Windows.Forms.GroupBox();
            this.InstA_Erdo = new System.Windows.Forms.RadioButton();
            this.Inst5_9rdo = new System.Windows.Forms.RadioButton();
            this.Inst0_4rdo = new System.Windows.Forms.RadioButton();
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
            this.batchListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.batchListView.HideSelection = false;
            this.batchListView.Location = new System.Drawing.Point(12, 12);
            this.batchListView.MultiSelect = false;
            this.batchListView.Name = "batchListView";
            this.batchListView.Size = new System.Drawing.Size(467, 132);
            this.batchListView.TabIndex = 0;
            this.toolTip1.SetToolTip(this.batchListView, global::PngtoFshBatchtxt.Properties.Resources.batchListView_ToolTip);
            this.batchListView.UseCompatibleStateImageBehavior = false;
            this.batchListView.View = System.Windows.Forms.View.Details;
            this.batchListView.SelectedIndexChanged += new System.EventHandler(this.batchListView1_SelectedIndexChanged);
            this.batchListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.batchListView1_DragDrop);
            this.batchListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.batchListView1_DragEnter);
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
            // PngopenDialog
            // 
            this.PngopenDialog.Filter = "Bitmap files (*.png;*.bmp)|*.png;*.bmp";
            this.PngopenDialog.Multiselect = true;
            // 
            // InstendBox1
            // 
            this.InstendBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.InstendBox1.Controls.Add(this.InstA_Erdo);
            this.InstendBox1.Controls.Add(this.Inst5_9rdo);
            this.InstendBox1.Controls.Add(this.Inst0_4rdo);
            this.InstendBox1.Location = new System.Drawing.Point(174, 150);
            this.InstendBox1.Name = "InstendBox1";
            this.InstendBox1.Size = new System.Drawing.Size(80, 78);
            this.InstendBox1.TabIndex = 7;
            this.InstendBox1.TabStop = false;
            this.InstendBox1.Text = "End format";
            // 
            // InstA_Erdo
            // 
            this.InstA_Erdo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.InstA_Erdo.AutoSize = true;
            this.InstA_Erdo.Location = new System.Drawing.Point(6, 55);
            this.InstA_Erdo.Name = "InstA_Erdo";
            this.InstA_Erdo.Size = new System.Drawing.Size(42, 17);
            this.InstA_Erdo.TabIndex = 9;
            this.InstA_Erdo.TabStop = true;
            this.InstA_Erdo.Text = "A-E";
            this.toolTip1.SetToolTip(this.InstA_Erdo, global::PngtoFshBatchtxt.Properties.Resources.InstanceIDRadioButtons_ToolTip);
            this.InstA_Erdo.UseVisualStyleBackColor = true;
            this.InstA_Erdo.CheckedChanged += new System.EventHandler(this.Format_radios_changed);
            // 
            // Inst5_9rdo
            // 
            this.Inst5_9rdo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Inst5_9rdo.AutoSize = true;
            this.Inst5_9rdo.Location = new System.Drawing.Point(6, 36);
            this.Inst5_9rdo.Name = "Inst5_9rdo";
            this.Inst5_9rdo.Size = new System.Drawing.Size(40, 17);
            this.Inst5_9rdo.TabIndex = 8;
            this.Inst5_9rdo.TabStop = true;
            this.Inst5_9rdo.Text = "5-9";
            this.toolTip1.SetToolTip(this.Inst5_9rdo, global::PngtoFshBatchtxt.Properties.Resources.InstanceIDRadioButtons_ToolTip);
            this.Inst5_9rdo.UseVisualStyleBackColor = true;
            this.Inst5_9rdo.CheckedChanged += new System.EventHandler(this.Format_radios_changed);
            // 
            // Inst0_4rdo
            // 
            this.Inst0_4rdo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Inst0_4rdo.AutoSize = true;
            this.Inst0_4rdo.Checked = true;
            this.Inst0_4rdo.Location = new System.Drawing.Point(6, 19);
            this.Inst0_4rdo.Name = "Inst0_4rdo";
            this.Inst0_4rdo.Size = new System.Drawing.Size(40, 17);
            this.Inst0_4rdo.TabIndex = 7;
            this.Inst0_4rdo.TabStop = true;
            this.Inst0_4rdo.Text = "0-4";
            this.toolTip1.SetToolTip(this.Inst0_4rdo, global::PngtoFshBatchtxt.Properties.Resources.InstanceIDRadioButtons_ToolTip);
            this.Inst0_4rdo.UseVisualStyleBackColor = true;
            this.Inst0_4rdo.CheckedChanged += new System.EventHandler(this.Format_radios_changed);
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
            this.addBtn.Text = "Add";
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
            this.clearListBtn.Click += new System.EventHandler(this.clearlistbtn_Click);
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
            this.fshWriteCompCb.CheckedChanged += new System.EventHandler(this.fshwritecompcb_CheckedChanged);
            // 
            // mipFormatCbo
            // 
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
        private System.Windows.Forms.FolderBrowserDialog OutputBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog PngopenDialog;
        private System.Windows.Forms.GroupBox InstendBox1;
        private System.Windows.Forms.RadioButton InstA_Erdo;
        private System.Windows.Forms.RadioButton Inst5_9rdo;
        private System.Windows.Forms.RadioButton Inst0_4rdo;
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
    }
}

