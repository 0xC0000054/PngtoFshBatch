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
            this.bitmapHeader = new System.Windows.Forms.ColumnHeader();
            this.alphaHeader = new System.Windows.Forms.ColumnHeader();
            this.groupHeader = new System.Windows.Forms.ColumnHeader();
            this.instanceHeader = new System.Windows.Forms.ColumnHeader();
            this.compDatcb = new System.Windows.Forms.CheckBox();
            this.datFuncBox = new System.Windows.Forms.GroupBox();
            this.Datnametxt = new System.Windows.Forms.Label();
            this.datlbl = new System.Windows.Forms.Label();
            this.newDatbtn = new System.Windows.Forms.Button();
            this.saveDatbtn = new System.Windows.Forms.Button();
            this.mipbtn = new System.Windows.Forms.Button();
            this.processbatchbtn = new System.Windows.Forms.Button();
            this.saveDatDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.autoprocMipscb = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.outfolderbtn = new System.Windows.Forms.Button();
            this.OutputBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.PngopenDialog = new System.Windows.Forms.OpenFileDialog();
            this.InstendBox1 = new System.Windows.Forms.GroupBox();
            this.InstA_Erdo = new System.Windows.Forms.RadioButton();
            this.Inst5_9rdo = new System.Windows.Forms.RadioButton();
            this.Inst0_4rdo = new System.Windows.Forms.RadioButton();
            this.rembtn = new System.Windows.Forms.Button();
            this.addbtn = new System.Windows.Forms.Button();
            this.FshtypeBox = new System.Windows.Forms.ComboBox();
            this.tgiInstlbl = new System.Windows.Forms.Label();
            this.tgiGrouplbl = new System.Windows.Forms.Label();
            this.tgiInstancetxt = new System.Windows.Forms.TextBox();
            this.tgiGrouptxt = new System.Windows.Forms.TextBox();
            this.clearlistbtn = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.fshwritecompcb = new System.Windows.Forms.CheckBox();
            this.datFuncBox.SuspendLayout();
            this.InstendBox1.SuspendLayout();
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
            this.batchListView.Size = new System.Drawing.Size(467, 128);
            this.batchListView.TabIndex = 0;
            this.toolTip1.SetToolTip(this.batchListView, Properties.Resources.batchListView_Tooltip);
            this.batchListView.UseCompatibleStateImageBehavior = false;
            this.batchListView.View = System.Windows.Forms.View.Details;
            this.batchListView.SelectedIndexChanged += new System.EventHandler(this.batchListView1_SelectedIndexChanged);
            this.batchListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.batchListView1_DragDrop);
            this.batchListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.batchListView1_DragEnter);
            // 
            // bitmapHeader
            // 
            this.bitmapHeader.Text = "Bitmap";
            this.bitmapHeader.Width = 96;
            // 
            // alphaHeader
            // 
            this.alphaHeader.Text = "Alpha";
            this.alphaHeader.Width = 115;
            // 
            // groupHeader
            // 
            this.groupHeader.Text = "Group";
            this.groupHeader.Width = 96;
            // 
            // instanceHeader
            // 
            this.instanceHeader.Text = "Instance";
            this.instanceHeader.Width = 96;
            // 
            // compDatcb
            // 
            this.compDatcb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.compDatcb.AutoSize = true;
            this.compDatcb.Checked = true;
            this.compDatcb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.compDatcb.Location = new System.Drawing.Point(12, 164);
            this.compDatcb.Name = "compDatcb";
            this.compDatcb.Size = new System.Drawing.Size(93, 17);
            this.compDatcb.TabIndex = 3;
            this.compDatcb.Text = Properties.Resources.compDatcb_Text;
            this.toolTip1.SetToolTip(this.compDatcb, Properties.Resources.compDatcb_Tooltip);
            this.compDatcb.UseVisualStyleBackColor = true;
            this.compDatcb.CheckedChanged += new System.EventHandler(this.compDatcb_CheckedChanged);
            // 
            // datFuncBox
            // 
            this.datFuncBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.datFuncBox.Controls.Add(this.Datnametxt);
            this.datFuncBox.Controls.Add(this.datlbl);
            this.datFuncBox.Controls.Add(this.newDatbtn);
            this.datFuncBox.Controls.Add(this.saveDatbtn);
            this.datFuncBox.Location = new System.Drawing.Point(312, 146);
            this.datFuncBox.Name = "datFuncBox";
            this.datFuncBox.Size = new System.Drawing.Size(167, 64);
            this.datFuncBox.TabIndex = 12;
            this.datFuncBox.TabStop = false;
            this.datFuncBox.Text = "Dat Functions";
            // 
            // Datnametxt
            // 
            this.Datnametxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Datnametxt.AutoSize = true;
            this.Datnametxt.Location = new System.Drawing.Point(48, 18);
            this.Datnametxt.Name = "Datnametxt";
            this.Datnametxt.Size = new System.Drawing.Size(0, 13);
            this.Datnametxt.TabIndex = 83;
            // 
            // datlbl
            // 
            this.datlbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.datlbl.AutoSize = true;
            this.datlbl.Location = new System.Drawing.Point(6, 18);
            this.datlbl.Name = "datlbl";
            this.datlbl.Size = new System.Drawing.Size(36, 13);
            this.datlbl.TabIndex = 82;
            this.datlbl.Text = "Dat = ";
            // 
            // newDatbtn
            // 
            this.newDatbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newDatbtn.Location = new System.Drawing.Point(6, 34);
            this.newDatbtn.Name = "newDatbtn";
            this.newDatbtn.Size = new System.Drawing.Size(75, 23);
            this.newDatbtn.TabIndex = 12;
            this.newDatbtn.Text = "New dat";
            this.newDatbtn.UseVisualStyleBackColor = true;
            this.newDatbtn.Click += new System.EventHandler(this.newDatbtn_Click);
            // 
            // saveDatbtn
            // 
            this.saveDatbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveDatbtn.Location = new System.Drawing.Point(87, 34);
            this.saveDatbtn.Name = "saveDatbtn";
            this.saveDatbtn.Size = new System.Drawing.Size(75, 23);
            this.saveDatbtn.TabIndex = 15;
            this.saveDatbtn.Text = "Save dat";
            this.toolTip1.SetToolTip(this.saveDatbtn, "Process the list and save into a new dat");
            this.saveDatbtn.UseVisualStyleBackColor = true;
            this.saveDatbtn.Click += new System.EventHandler(this.saveDatbtn_Click);
            // 
            // mipbtn
            // 
            this.mipbtn.Location = new System.Drawing.Point(188, 279);
            this.mipbtn.Name = "mipbtn";
            this.mipbtn.Size = new System.Drawing.Size(106, 23);
            this.mipbtn.TabIndex = 85;
            this.mipbtn.Text = "Generate Mipmaps";
            this.mipbtn.UseVisualStyleBackColor = true;
            this.mipbtn.Visible = false;
            this.mipbtn.Click += new System.EventHandler(this.mipbtn_Click);
            // 
            // processbatchbtn
            // 
            this.processbatchbtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.processbatchbtn.Location = new System.Drawing.Point(399, 279);
            this.processbatchbtn.Name = "processbatchbtn";
            this.processbatchbtn.Size = new System.Drawing.Size(75, 23);
            this.processbatchbtn.TabIndex = 14;
            this.processbatchbtn.Text = "Process";
            this.toolTip1.SetToolTip(this.processbatchbtn, "Process the items in the list");
            this.processbatchbtn.UseVisualStyleBackColor = true;
            this.processbatchbtn.Click += new System.EventHandler(this.processbatchbtn_Click);
            // 
            // saveDatDialog1
            // 
            this.saveDatDialog1.DefaultExt = "dat";
            this.saveDatDialog1.Filter = "Dat files (*.dat)|*.dat|All files (*.*)|*.*";
            this.saveDatDialog1.RestoreDirectory = true;
            // 
            // autoprocMipscb
            // 
            this.autoprocMipscb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.autoprocMipscb.AutoSize = true;
            this.autoprocMipscb.Location = new System.Drawing.Point(12, 146);
            this.autoprocMipscb.Name = "autoprocMipscb";
            this.autoprocMipscb.Size = new System.Drawing.Size(153, 17);
            this.autoprocMipscb.TabIndex = 2;
            this.autoprocMipscb.Text = "Automatically process Mips";
            this.toolTip1.SetToolTip(this.autoprocMipscb, "Generate mipmaps for the zoom levels");
            this.autoprocMipscb.UseVisualStyleBackColor = true;
            this.autoprocMipscb.CheckedChanged += new System.EventHandler(this.autoprocMipscb_CheckedChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "txt";
            this.openFileDialog1.Filter = "Batch files (*.txt)|*.txt";
            // 
            // outfolderbtn
            // 
            this.outfolderbtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.outfolderbtn.Location = new System.Drawing.Point(300, 279);
            this.outfolderbtn.Name = "outfolderbtn";
            this.outfolderbtn.Size = new System.Drawing.Size(93, 23);
            this.outfolderbtn.TabIndex = 11;
            this.outfolderbtn.Text = "Output Folder";
            this.toolTip1.SetToolTip(this.outfolderbtn, "Change the output folder for the processed files");
            this.outfolderbtn.UseVisualStyleBackColor = true;
            this.outfolderbtn.Click += new System.EventHandler(this.outfolderbtn_Click);
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
            this.InstendBox1.Location = new System.Drawing.Point(171, 146);
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
            this.toolTip1.SetToolTip(this.InstA_Erdo, "Change the end diget of the instance id");
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
            this.toolTip1.SetToolTip(this.Inst5_9rdo, "Change the end diget of the instance id");
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
            this.toolTip1.SetToolTip(this.Inst0_4rdo, "Change the end diget of the instance id");
            this.Inst0_4rdo.UseVisualStyleBackColor = true;
            this.Inst0_4rdo.CheckedChanged += new System.EventHandler(this.Format_radios_changed);
            // 
            // rembtn
            // 
            this.rembtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rembtn.Location = new System.Drawing.Point(399, 216);
            this.rembtn.Name = "rembtn";
            this.rembtn.Size = new System.Drawing.Size(75, 23);
            this.rembtn.TabIndex = 16;
            this.rembtn.Text = "Remove";
            this.toolTip1.SetToolTip(this.rembtn, "Removes the selected item from the list");
            this.rembtn.UseVisualStyleBackColor = true;
            this.rembtn.Click += new System.EventHandler(this.rembtn_Click);
            // 
            // addbtn
            // 
            this.addbtn.AllowDrop = true;
            this.addbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addbtn.Location = new System.Drawing.Point(318, 216);
            this.addbtn.Name = "addbtn";
            this.addbtn.Size = new System.Drawing.Size(75, 23);
            this.addbtn.TabIndex = 13;
            this.addbtn.Text = "Add";
            this.toolTip1.SetToolTip(this.addbtn, "Add files to the existing list");
            this.addbtn.UseVisualStyleBackColor = true;
            this.addbtn.Click += new System.EventHandler(this.addbtn_Click);
            this.addbtn.DragDrop += new System.Windows.Forms.DragEventHandler(this.addbtn_DragDrop);
            this.addbtn.DragEnter += new System.Windows.Forms.DragEventHandler(this.addbtn_DragEnter);
            // 
            // FshtypeBox
            // 
            this.FshtypeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FshtypeBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.FshtypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FshtypeBox.FormattingEnabled = true;
            this.FshtypeBox.Items.AddRange(new object[] {
            "24 Bit RGB (0:8:8:8)",
            "32 Bit ARGB (8:8:8:8)",
            "DXT1 Compressed, no Alpha",
            "DXT3 Compressed, with Alpha "});
            this.FshtypeBox.Location = new System.Drawing.Point(12, 258);
            this.FshtypeBox.Name = "FshtypeBox";
            this.FshtypeBox.Size = new System.Drawing.Size(172, 21);
            this.FshtypeBox.TabIndex = 6;
            this.toolTip1.SetToolTip(this.FshtypeBox, "Change the fsh type of the selected item");
            this.FshtypeBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.FshtypeBox_DrawItem);
            this.FshtypeBox.SelectionChangeCommitted += new System.EventHandler(this.FshtypeBox_SelectionChangeCommitted);
            this.FshtypeBox.SelectedIndexChanged += new System.EventHandler(this.FshtypeBox_SelectedIndexChanged);
            // 
            // tgiInstlbl
            // 
            this.tgiInstlbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiInstlbl.AutoSize = true;
            this.tgiInstlbl.BackColor = System.Drawing.SystemColors.Control;
            this.tgiInstlbl.Location = new System.Drawing.Point(13, 230);
            this.tgiInstlbl.Name = "tgiInstlbl";
            this.tgiInstlbl.Size = new System.Drawing.Size(48, 13);
            this.tgiInstlbl.TabIndex = 105;
            this.tgiInstlbl.Text = "Instance";
            // 
            // tgiGrouplbl
            // 
            this.tgiGrouplbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiGrouplbl.AutoSize = true;
            this.tgiGrouplbl.Location = new System.Drawing.Point(25, 207);
            this.tgiGrouplbl.Name = "tgiGrouplbl";
            this.tgiGrouplbl.Size = new System.Drawing.Size(36, 13);
            this.tgiGrouplbl.TabIndex = 104;
            this.tgiGrouplbl.Text = "Group";
            // 
            // tgiInstancetxt
            // 
            this.tgiInstancetxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiInstancetxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tgiInstancetxt.Location = new System.Drawing.Point(67, 227);
            this.tgiInstancetxt.MaxLength = 8;
            this.tgiInstancetxt.Name = "tgiInstancetxt";
            this.tgiInstancetxt.Size = new System.Drawing.Size(82, 20);
            this.tgiInstancetxt.TabIndex = 5;
            this.toolTip1.SetToolTip(this.tgiInstancetxt, "Change the instance id of the selected item");
            this.tgiInstancetxt.TextChanged += new System.EventHandler(this.tgiInstancetxt_TextChanged);
            this.tgiInstancetxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TgiGrouptxt_KeyDown);
            // 
            // tgiGrouptxt
            // 
            this.tgiGrouptxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tgiGrouptxt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tgiGrouptxt.Location = new System.Drawing.Point(67, 201);
            this.tgiGrouptxt.MaxLength = 8;
            this.tgiGrouptxt.Name = "tgiGrouptxt";
            this.tgiGrouptxt.Size = new System.Drawing.Size(82, 20);
            this.tgiGrouptxt.TabIndex = 4;
            this.toolTip1.SetToolTip(this.tgiGrouptxt, "Change the group id of the selected item");
            this.tgiGrouptxt.TextChanged += new System.EventHandler(this.tgiGrouptxt_TextChanged);
            this.tgiGrouptxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TgiGrouptxt_KeyDown);
            // 
            // clearlistbtn
            // 
            this.clearlistbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearlistbtn.Location = new System.Drawing.Point(399, 245);
            this.clearlistbtn.Name = "clearlistbtn";
            this.clearlistbtn.Size = new System.Drawing.Size(75, 23);
            this.clearlistbtn.TabIndex = 106;
            this.clearlistbtn.Text = "Clear List";
            this.clearlistbtn.UseVisualStyleBackColor = true;
            this.clearlistbtn.Click += new System.EventHandler(this.clearlistbtn_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 1000;
            this.toolTip1.ReshowDelay = 500;
            // 
            // fshwritecompcb
            // 
            this.fshwritecompcb.AutoSize = true;
            this.fshwritecompcb.Location = new System.Drawing.Point(12, 183);
            this.fshwritecompcb.Name = "fshwritecompcb";
            this.fshwritecompcb.Size = new System.Drawing.Size(128, 17);
            this.fshwritecompcb.TabIndex = 107;
            this.fshwritecompcb.Text = "Fshwrite Compression";
            this.toolTip1.SetToolTip(this.fshwritecompcb, "Use Fshwrite compression when saving DXT1 and DXT3 fsh images");
            this.fshwritecompcb.UseVisualStyleBackColor = true;
            this.fshwritecompcb.CheckedChanged += new System.EventHandler(this.fshwritecompcb_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 310);
            this.Controls.Add(this.fshwritecompcb);
            this.Controls.Add(this.clearlistbtn);
            this.Controls.Add(this.tgiInstlbl);
            this.Controls.Add(this.tgiGrouplbl);
            this.Controls.Add(this.tgiInstancetxt);
            this.Controls.Add(this.tgiGrouptxt);
            this.Controls.Add(this.FshtypeBox);
            this.Controls.Add(this.addbtn);
            this.Controls.Add(this.rembtn);
            this.Controls.Add(this.InstendBox1);
            this.Controls.Add(this.outfolderbtn);
            this.Controls.Add(this.processbatchbtn);
            this.Controls.Add(this.compDatcb);
            this.Controls.Add(this.datFuncBox);
            this.Controls.Add(this.mipbtn);
            this.Controls.Add(this.batchListView);
            this.Controls.Add(this.autoprocMipscb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimumSize = new System.Drawing.Size(499, 342);
            this.Name = "Form1";
            this.Text = "Png to Fsh Batch";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.datFuncBox.ResumeLayout(false);
            this.datFuncBox.PerformLayout();
            this.InstendBox1.ResumeLayout(false);
            this.InstendBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader bitmapHeader;
        private System.Windows.Forms.ColumnHeader alphaHeader;
        private System.Windows.Forms.CheckBox compDatcb;
        private System.Windows.Forms.GroupBox datFuncBox;
        private System.Windows.Forms.Label Datnametxt;
        private System.Windows.Forms.Label datlbl;
        private System.Windows.Forms.Button newDatbtn;
        private System.Windows.Forms.Button saveDatbtn;
        internal System.Windows.Forms.Button mipbtn;
        internal System.Windows.Forms.Button processbatchbtn;
        private System.Windows.Forms.SaveFileDialog saveDatDialog1;
        internal System.Windows.Forms.CheckBox autoprocMipscb;
        private System.Windows.Forms.ColumnHeader groupHeader;
        private System.Windows.Forms.ColumnHeader instanceHeader;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button outfolderbtn;
        private System.Windows.Forms.FolderBrowserDialog OutputBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog PngopenDialog;
        private System.Windows.Forms.GroupBox InstendBox1;
        private System.Windows.Forms.RadioButton InstA_Erdo;
        private System.Windows.Forms.RadioButton Inst5_9rdo;
        private System.Windows.Forms.RadioButton Inst0_4rdo;
        private System.Windows.Forms.Button rembtn;
        private System.Windows.Forms.Button addbtn;
        private System.Windows.Forms.ComboBox FshtypeBox;
        private System.Windows.Forms.Label tgiInstlbl;
        private System.Windows.Forms.Label tgiGrouplbl;
        private System.Windows.Forms.TextBox tgiInstancetxt;
        internal System.Windows.Forms.TextBox tgiGrouptxt;
        private System.Windows.Forms.Button clearlistbtn;
        internal System.Windows.Forms.ListView batchListView;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox fshwritecompcb;
    }
}

