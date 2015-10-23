namespace mapKnight_Editor
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.overwriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.revertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.mapKnightMapEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.version01AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createdByTipfomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_spawn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.lv_tile = new System.Windows.Forms.ListView();
            this.lv_overlay = new System.Windows.Forms.ListView();
            this.sfd_mapsave = new System.Windows.Forms.SaveFileDialog();
            this.ofd_mapfile = new System.Windows.Forms.OpenFileDialog();
            this.trb_zoom = new System.Windows.Forms.TrackBar();
            this.trb_imagesize = new System.Windows.Forms.TrackBar();
            this.lbl_zoom = new System.Windows.Forms.Label();
            this.lbl_imgsize = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trb_zoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trb_imagesize)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.PowderBlue;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripDropDownButton1,
            this.toolStripLabel3,
            this.toolStripSeparator1,
            this.toolStripLabel4,
            this.toolStripLabel5,
            this.toolStripButton1,
            this.toolStripLabel6,
            this.toolStripSeparator2,
            this.tsb_spawn,
            this.toolStripSeparator3,
            this.toolStripButton2});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(734, 35);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToToolStripMenuItem,
            this.overwriteToolStripMenuItem});
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(38, 19);
            this.toolStripLabel1.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToToolStripMenuItem
            // 
            this.saveToToolStripMenuItem.Enabled = false;
            this.saveToToolStripMenuItem.Name = "saveToToolStripMenuItem";
            this.saveToToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveToToolStripMenuItem.Text = "Save To";
            this.saveToToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // overwriteToolStripMenuItem
            // 
            this.overwriteToolStripMenuItem.Enabled = false;
            this.overwriteToolStripMenuItem.Name = "overwriteToolStripMenuItem";
            this.overwriteToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.overwriteToolStripMenuItem.Text = "Save";
            this.overwriteToolStripMenuItem.Click += new System.EventHandler(this.overwriteToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.revertToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(40, 19);
            this.toolStripDropDownButton1.Text = "Edit";
            // 
            // revertToolStripMenuItem
            // 
            this.revertToolStripMenuItem.Name = "revertToolStripMenuItem";
            this.revertToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.revertToolStripMenuItem.Text = "Undo";
            this.revertToolStripMenuItem.Click += new System.EventHandler(this.revertToolStripMenuItem_Click);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapKnightMapEditorToolStripMenuItem,
            this.version01AToolStripMenuItem,
            this.createdByTipfomToolStripMenuItem});
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(53, 19);
            this.toolStripLabel3.Text = "About";
            // 
            // mapKnightMapEditorToolStripMenuItem
            // 
            this.mapKnightMapEditorToolStripMenuItem.Name = "mapKnightMapEditorToolStripMenuItem";
            this.mapKnightMapEditorToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.mapKnightMapEditorToolStripMenuItem.Text = "mapKnight Map Editor";
            // 
            // version01AToolStripMenuItem
            // 
            this.version01AToolStripMenuItem.Name = "version01AToolStripMenuItem";
            this.version01AToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.version01AToolStripMenuItem.Text = "Version 1.05 i";
            // 
            // createdByTipfomToolStripMenuItem
            // 
            this.createdByTipfomToolStripMenuItem.Name = "createdByTipfomToolStripMenuItem";
            this.createdByTipfomToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.createdByTipfomToolStripMenuItem.Text = "Created by tipfom";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Checked = true;
            this.toolStripLabel4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripLabel4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel4.Image = global::mapKnight_Editor.Properties.Resources.Brush_tool_icon;
            this.toolStripLabel4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(23, 20);
            this.toolStripLabel4.Text = "Brush";
            this.toolStripLabel4.Click += new System.EventHandler(this.toolStripLabel4_Click);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel5.Image = global::mapKnight_Editor.Properties.Resources.Paint_bucket_tool_icon;
            this.toolStripLabel5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(23, 20);
            this.toolStripLabel5.Text = "Paint Bucket";
            this.toolStripLabel5.Click += new System.EventHandler(this.toolStripLabel5_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::mapKnight_Editor.Properties.Resources.Eyedropper_icon;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 20);
            this.toolStripButton1.Text = "Select Tile";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel6.Image = global::mapKnight_Editor.Properties.Resources.Eraser_icon;
            this.toolStripLabel6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(23, 20);
            this.toolStripLabel6.Text = "Eraser";
            this.toolStripLabel6.Click += new System.EventHandler(this.toolStripLabel6_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // tsb_spawn
            // 
            this.tsb_spawn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_spawn.Image = global::mapKnight_Editor.Properties.Resources.Magic_wand_icon;
            this.tsb_spawn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_spawn.Name = "tsb_spawn";
            this.tsb_spawn.Size = new System.Drawing.Size(23, 20);
            this.tsb_spawn.Text = "Set Spawn";
            this.tsb_spawn.Click += new System.EventHandler(this.tsb_spawn_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.CheckOnClick = true;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(113, 19);
            this.toolStripButton2.Text = "Use Custom Cursor";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 35);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(214, 367);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar1.Location = new System.Drawing.Point(214, 385);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(520, 17);
            this.hScrollBar1.TabIndex = 3;
            this.hScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar1.Location = new System.Drawing.Point(717, 35);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 350);
            this.vScrollBar1.TabIndex = 4;
            this.vScrollBar1.ValueChanged += new System.EventHandler(this.vScrollBar1_ValueChanged);
            // 
            // lv_tile
            // 
            this.lv_tile.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lv_tile.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv_tile.Location = new System.Drawing.Point(0, 25);
            this.lv_tile.MultiSelect = false;
            this.lv_tile.Name = "lv_tile";
            this.lv_tile.Size = new System.Drawing.Size(214, 97);
            this.lv_tile.TabIndex = 9;
            this.lv_tile.UseCompatibleStateImageBehavior = false;
            this.lv_tile.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.lv_tile_ColumnWidthChanging);
            this.lv_tile.SelectedIndexChanged += new System.EventHandler(this.lv_tile_SelectedIndexChanged);
            // 
            // lv_overlay
            // 
            this.lv_overlay.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv_overlay.HideSelection = false;
            this.lv_overlay.Location = new System.Drawing.Point(0, 156);
            this.lv_overlay.Name = "lv_overlay";
            this.lv_overlay.Size = new System.Drawing.Size(214, 97);
            this.lv_overlay.TabIndex = 10;
            this.lv_overlay.UseCompatibleStateImageBehavior = false;
            this.lv_overlay.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.lv_overlay_ColumnWidthChanging);
            this.lv_overlay.SelectedIndexChanged += new System.EventHandler(this.lv_overlay_SelectedIndexChanged);
            // 
            // sfd_mapsave
            // 
            this.sfd_mapsave.Filter = "TMSL2-Dateien|*.tmsl2";
            // 
            // ofd_mapfile
            // 
            this.ofd_mapfile.Filter = "Map-Dateien|*.map|XML-Dateien|*.xml|TMSL2-Dateien|*.tmsl2|Text-Dateien|*.txt";
            // 
            // trb_zoom
            // 
            this.trb_zoom.AutoSize = false;
            this.trb_zoom.BackColor = System.Drawing.Color.PowderBlue;
            this.trb_zoom.Location = new System.Drawing.Point(608, 0);
            this.trb_zoom.Maximum = 20;
            this.trb_zoom.Name = "trb_zoom";
            this.trb_zoom.Size = new System.Drawing.Size(126, 27);
            this.trb_zoom.TabIndex = 11;
            this.trb_zoom.ValueChanged += new System.EventHandler(this.trb_zoom_ValueChanged);
            // 
            // trb_imagesize
            // 
            this.trb_imagesize.AutoSize = false;
            this.trb_imagesize.BackColor = System.Drawing.Color.PowderBlue;
            this.trb_imagesize.Location = new System.Drawing.Point(449, 0);
            this.trb_imagesize.Maximum = 20;
            this.trb_imagesize.Name = "trb_imagesize";
            this.trb_imagesize.Size = new System.Drawing.Size(126, 27);
            this.trb_imagesize.TabIndex = 12;
            this.trb_imagesize.ValueChanged += new System.EventHandler(this.trb_imagesize_ValueChanged);
            // 
            // lbl_zoom
            // 
            this.lbl_zoom.AutoSize = true;
            this.lbl_zoom.BackColor = System.Drawing.Color.PowderBlue;
            this.lbl_zoom.Location = new System.Drawing.Point(581, 9);
            this.lbl_zoom.Name = "lbl_zoom";
            this.lbl_zoom.Size = new System.Drawing.Size(34, 13);
            this.lbl_zoom.TabIndex = 13;
            this.lbl_zoom.Text = "Zoom";
            // 
            // lbl_imgsize
            // 
            this.lbl_imgsize.AutoSize = true;
            this.lbl_imgsize.BackColor = System.Drawing.Color.PowderBlue;
            this.lbl_imgsize.Location = new System.Drawing.Point(400, 9);
            this.lbl_imgsize.Name = "lbl_imgsize";
            this.lbl_imgsize.Size = new System.Drawing.Size(56, 13);
            this.lbl_imgsize.TabIndex = 14;
            this.lbl_imgsize.Text = "ImageSize";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(734, 402);
            this.Controls.Add(this.lbl_imgsize);
            this.Controls.Add(this.lbl_zoom);
            this.Controls.Add(this.trb_imagesize);
            this.Controls.Add(this.trb_zoom);
            this.Controls.Add(this.lv_overlay);
            this.Controls.Add(this.lv_tile);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.toolStrip1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(750, 441);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "mapKnight Map Editor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trb_zoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trb_imagesize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.ListView lv_tile;
        private System.Windows.Forms.ListView lv_overlay;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripLabel4;
        private System.Windows.Forms.ToolStripButton toolStripLabel5;
        private System.Windows.Forms.ToolStripButton toolStripLabel6;
        private System.Windows.Forms.ToolStripDropDownButton toolStripLabel1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripLabel3;
        private System.Windows.Forms.ToolStripMenuItem mapKnightMapEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem version01AToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createdByTipfomToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog sfd_mapsave;
        private System.Windows.Forms.OpenFileDialog ofd_mapfile;
        private System.Windows.Forms.ToolStripMenuItem overwriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsb_spawn;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.TrackBar trb_zoom;
        private System.Windows.Forms.TrackBar trb_imagesize;
        private System.Windows.Forms.Label lbl_zoom;
        private System.Windows.Forms.Label lbl_imgsize;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem revertToolStripMenuItem;
    }
}

