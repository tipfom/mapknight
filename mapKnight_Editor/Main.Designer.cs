namespace mapKnight.ToolKit
{
	partial class Main
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.components = new System.ComponentModel.Container ();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager (typeof(Main));
			this.tlstrp_main = new System.Windows.Forms.ToolStrip ();
			this.tsddb_file = new System.Windows.Forms.ToolStripDropDownButton ();
			this.tsmi_load = new System.Windows.Forms.ToolStripMenuItem ();
			this.tsmi_save = new System.Windows.Forms.ToolStripMenuItem ();
			this.tsmi_saveas = new System.Windows.Forms.ToolStripMenuItem ();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator ();
			this.tsmmi_exportraw = new System.Windows.Forms.ToolStripMenuItem ();
			this.tsddb_edit = new System.Windows.Forms.ToolStripDropDownButton ();
			this.tsmi_undo = new System.Windows.Forms.ToolStripMenuItem ();
			this.tsb_info = new System.Windows.Forms.ToolStripButton ();
			this.tbctrl_main = new System.Windows.Forms.TabControl ();
			this.tbpg_mapeditor = new System.Windows.Forms.TabPage ();
			this.tlstrp_map = new System.Windows.Forms.ToolStrip ();
			this.tscb_map_mapselect = new System.Windows.Forms.ToolStripComboBox ();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator ();
			this.tsb_addnew = new System.Windows.Forms.ToolStripButton ();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator ();
			this.tsb_map_wand = new System.Windows.Forms.ToolStripButton ();
			this.tsb_map_fill = new System.Windows.Forms.ToolStripButton ();
			this.tsb_map_select = new System.Windows.Forms.ToolStripButton ();
			this.tsb_map_eraser = new System.Windows.Forms.ToolStripButton ();
			this.tsb_map_brush = new System.Windows.Forms.ToolStripButton ();
			this.hscrlbar_map = new System.Windows.Forms.HScrollBar ();
			this.vscrlbar_map = new System.Windows.Forms.VScrollBar ();
			this.spltcntr_map = new System.Windows.Forms.SplitContainer ();
			this.lvw_tiles = new System.Windows.Forms.ListView ();
			this.imglst_tile = new System.Windows.Forms.ImageList (this.components);
			this.lvw_overlays = new System.Windows.Forms.ListView ();
			this.imglst_overlay = new System.Windows.Forms.ImageList (this.components);
			this.tbpg_charcreation = new System.Windows.Forms.TabPage ();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer ();
			this.treeview_character = new System.Windows.Forms.TreeView ();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip ();
			this.tsbutton_addset = new System.Windows.Forms.ToolStripButton ();
			this.tsbutton_removeset = new System.Windows.Forms.ToolStripButton ();
			this.tstextbox_setname = new System.Windows.Forms.ToolStripTextBox ();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer ();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer ();
			this.listview_slot = new System.Windows.Forms.ListView ();
			this.textbox_itemdescription = new System.Windows.Forms.TextBox ();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer ();
			this.button_itembitmap = new System.Windows.Forms.Button ();
			this.textbox_itemname = new System.Windows.Forms.TextBox ();
			this.tbpg_animeditor = new System.Windows.Forms.TabPage ();
			this.ofd_mapfile = new System.Windows.Forms.OpenFileDialog ();
			this.sfd_mapfile = new System.Windows.Forms.SaveFileDialog ();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog ();
			this.ofd_imageedit = new System.Windows.Forms.OpenFileDialog ();
			this.tlstrp_main.SuspendLayout ();
			this.tbctrl_main.SuspendLayout ();
			this.tbpg_mapeditor.SuspendLayout ();
			this.tlstrp_map.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this.spltcntr_map)).BeginInit ();
			this.spltcntr_map.Panel1.SuspendLayout ();
			this.spltcntr_map.Panel2.SuspendLayout ();
			this.spltcntr_map.SuspendLayout ();
			this.tbpg_charcreation.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit ();
			this.splitContainer1.Panel1.SuspendLayout ();
			this.splitContainer1.Panel2.SuspendLayout ();
			this.splitContainer1.SuspendLayout ();
			this.toolStrip1.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit ();
			this.splitContainer2.Panel1.SuspendLayout ();
			this.splitContainer2.Panel2.SuspendLayout ();
			this.splitContainer2.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit ();
			this.splitContainer3.Panel1.SuspendLayout ();
			this.splitContainer3.Panel2.SuspendLayout ();
			this.splitContainer3.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit ();
			this.splitContainer4.Panel1.SuspendLayout ();
			this.splitContainer4.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// tlstrp_main
			// 
			this.tlstrp_main.BackColor = System.Drawing.SystemColors.Control;
			this.tlstrp_main.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.tlstrp_main.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tlstrp_main.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.tsddb_file,
				this.tsddb_edit,
				this.tsb_info
			});
			this.tlstrp_main.Location = new System.Drawing.Point (0, 0);
			this.tlstrp_main.Name = "tlstrp_main";
			this.tlstrp_main.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tlstrp_main.Size = new System.Drawing.Size (1362, 25);
			this.tlstrp_main.TabIndex = 0;
			// 
			// tsddb_file
			// 
			this.tsddb_file.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_file.DropDownItems.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.tsmi_load,
				this.tsmi_save,
				this.tsmi_saveas,
				this.toolStripSeparator3,
				this.tsmmi_exportraw
			});
			this.tsddb_file.Image = ((System.Drawing.Image)(resources.GetObject ("tsddb_file.Image")));
			this.tsddb_file.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddb_file.Name = "tsddb_file";
			this.tsddb_file.Size = new System.Drawing.Size (38, 22);
			this.tsddb_file.Text = "File";
			// 
			// tsmi_load
			// 
			this.tsmi_load.Name = "tsmi_load";
			this.tsmi_load.Size = new System.Drawing.Size (209, 22);
			this.tsmi_load.Text = "Load (Crtl + L)";
			this.tsmi_load.Click += new System.EventHandler (this.tsmi_load_Click);
			// 
			// tsmi_save
			// 
			this.tsmi_save.Name = "tsmi_save";
			this.tsmi_save.Size = new System.Drawing.Size (209, 22);
			this.tsmi_save.Text = "Save (Ctrl + S)";
			this.tsmi_save.Click += new System.EventHandler (this.tsmi_save_Click);
			// 
			// tsmi_saveas
			// 
			this.tsmi_saveas.Name = "tsmi_saveas";
			this.tsmi_saveas.Size = new System.Drawing.Size (209, 22);
			this.tsmi_saveas.Text = "Save As";
			this.tsmi_saveas.Click += new System.EventHandler (this.tsmi_saveas_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size (206, 6);
			// 
			// tsmmi_exportraw
			// 
			this.tsmmi_exportraw.Name = "tsmmi_exportraw";
			this.tsmmi_exportraw.Size = new System.Drawing.Size (209, 22);
			this.tsmmi_exportraw.Text = "Export Raw Data (Ctrl + E)";
			this.tsmmi_exportraw.Click += new System.EventHandler (this.tsmmi_exportraw_Click);
			// 
			// tsddb_edit
			// 
			this.tsddb_edit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsddb_edit.DropDownItems.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.tsmi_undo
			});
			this.tsddb_edit.Image = ((System.Drawing.Image)(resources.GetObject ("tsddb_edit.Image")));
			this.tsddb_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddb_edit.Name = "tsddb_edit";
			this.tsddb_edit.Size = new System.Drawing.Size (40, 22);
			this.tsddb_edit.Text = "Edit";
			// 
			// tsmi_undo
			// 
			this.tsmi_undo.Name = "tsmi_undo";
			this.tsmi_undo.Size = new System.Drawing.Size (154, 22);
			this.tsmi_undo.Text = "Undo (Crtl + Z)";
			this.tsmi_undo.Click += new System.EventHandler (this.tsmi_undo_Click);
			// 
			// tsb_info
			// 
			this.tsb_info.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsb_info.Image = ((System.Drawing.Image)(resources.GetObject ("tsb_info.Image")));
			this.tsb_info.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_info.Name = "tsb_info";
			this.tsb_info.Size = new System.Drawing.Size (32, 22);
			this.tsb_info.Text = "Info";
			this.tsb_info.Click += new System.EventHandler (this.tsb_info_Click);
			// 
			// tbctrl_main
			// 
			this.tbctrl_main.Controls.Add (this.tbpg_mapeditor);
			this.tbctrl_main.Controls.Add (this.tbpg_charcreation);
			this.tbctrl_main.Controls.Add (this.tbpg_animeditor);
			this.tbctrl_main.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbctrl_main.Location = new System.Drawing.Point (0, 25);
			this.tbctrl_main.Name = "tbctrl_main";
			this.tbctrl_main.SelectedIndex = 0;
			this.tbctrl_main.Size = new System.Drawing.Size (1362, 716);
			this.tbctrl_main.TabIndex = 1;
			this.tbctrl_main.SelectedIndexChanged += new System.EventHandler (this.tbctrl_main_SelectedIndexChanged);
			// 
			// tbpg_mapeditor
			// 
			this.tbpg_mapeditor.BackColor = System.Drawing.Color.White;
			this.tbpg_mapeditor.Controls.Add (this.tlstrp_map);
			this.tbpg_mapeditor.Controls.Add (this.hscrlbar_map);
			this.tbpg_mapeditor.Controls.Add (this.vscrlbar_map);
			this.tbpg_mapeditor.Controls.Add (this.spltcntr_map);
			this.tbpg_mapeditor.Location = new System.Drawing.Point (4, 22);
			this.tbpg_mapeditor.Name = "tbpg_mapeditor";
			this.tbpg_mapeditor.Padding = new System.Windows.Forms.Padding (3);
			this.tbpg_mapeditor.Size = new System.Drawing.Size (1354, 690);
			this.tbpg_mapeditor.TabIndex = 0;
			this.tbpg_mapeditor.Text = "MapEditor";
			this.tbpg_mapeditor.Paint += new System.Windows.Forms.PaintEventHandler (this.tbpg_mapeditor_Paint);
			this.tbpg_mapeditor.MouseDown += new System.Windows.Forms.MouseEventHandler (this.tbpg_mapeditor_MouseDown);
			this.tbpg_mapeditor.MouseMove += new System.Windows.Forms.MouseEventHandler (this.tbpg_mapeditor_MouseMove);
			this.tbpg_mapeditor.MouseUp += new System.Windows.Forms.MouseEventHandler (this.tbpg_mapeditor_MouseUp);
			this.tbpg_mapeditor.MouseWheel += new System.Windows.Forms.MouseEventHandler (this.tbpg_mapeditor_MouseWheel);
			// 
			// tlstrp_map
			// 
			this.tlstrp_map.AutoSize = false;
			this.tlstrp_map.BackColor = System.Drawing.Color.LightGray;
			this.tlstrp_map.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.tlstrp_map.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tlstrp_map.ImageScalingSize = new System.Drawing.Size (20, 20);
			this.tlstrp_map.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.tscb_map_mapselect,
				this.toolStripSeparator2,
				this.tsb_addnew,
				this.toolStripSeparator1,
				this.tsb_map_wand,
				this.tsb_map_fill,
				this.tsb_map_select,
				this.tsb_map_eraser,
				this.tsb_map_brush
			});
			this.tlstrp_map.Location = new System.Drawing.Point (263, 3);
			this.tlstrp_map.Name = "tlstrp_map";
			this.tlstrp_map.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.tlstrp_map.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tlstrp_map.Size = new System.Drawing.Size (1071, 25);
			this.tlstrp_map.TabIndex = 5;
			this.tlstrp_map.Text = "toolStrip1";
			// 
			// tscb_map_mapselect
			// 
			this.tscb_map_mapselect.Name = "tscb_map_mapselect";
			this.tscb_map_mapselect.Size = new System.Drawing.Size (121, 25);
			this.tscb_map_mapselect.SelectedIndexChanged += new System.EventHandler (this.tscb_map_mapselect_SelectedIndexChanged);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size (6, 25);
			// 
			// tsb_addnew
			// 
			this.tsb_addnew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_addnew.Image = global::mapKnight.ToolKit.Properties.Resources.icon_plus;
			this.tsb_addnew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_addnew.Name = "tsb_addnew";
			this.tsb_addnew.Size = new System.Drawing.Size (24, 22);
			this.tsb_addnew.Text = "add new";
			this.tsb_addnew.Click += new System.EventHandler (this.tsb_addnew_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size (6, 25);
			// 
			// tsb_map_wand
			// 
			this.tsb_map_wand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_map_wand.Image = global::mapKnight.ToolKit.Properties.Resources.icon_finger;
			this.tsb_map_wand.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_map_wand.Name = "tsb_map_wand";
			this.tsb_map_wand.Size = new System.Drawing.Size (24, 22);
			this.tsb_map_wand.Text = "select spawnpoint";
			this.tsb_map_wand.Click += new System.EventHandler (this.tsb_map_wand_Click);
			// 
			// tsb_map_fill
			// 
			this.tsb_map_fill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_map_fill.Image = global::mapKnight.ToolKit.Properties.Resources.icon_bucket;
			this.tsb_map_fill.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_map_fill.Name = "tsb_map_fill";
			this.tsb_map_fill.Size = new System.Drawing.Size (24, 22);
			this.tsb_map_fill.Text = "fill area";
			this.tsb_map_fill.Click += new System.EventHandler (this.tsb_map_fill_Click);
			// 
			// tsb_map_select
			// 
			this.tsb_map_select.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_map_select.Image = global::mapKnight.ToolKit.Properties.Resources.icon_pipette;
			this.tsb_map_select.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_map_select.Name = "tsb_map_select";
			this.tsb_map_select.Size = new System.Drawing.Size (24, 22);
			this.tsb_map_select.Text = "select tile";
			this.tsb_map_select.Click += new System.EventHandler (this.tsb_map_select_Click);
			// 
			// tsb_map_eraser
			// 
			this.tsb_map_eraser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_map_eraser.Image = global::mapKnight.ToolKit.Properties.Resources.icon_eraser;
			this.tsb_map_eraser.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_map_eraser.Name = "tsb_map_eraser";
			this.tsb_map_eraser.Size = new System.Drawing.Size (24, 22);
			this.tsb_map_eraser.Text = "erase tile";
			this.tsb_map_eraser.Click += new System.EventHandler (this.tsb_map_eraser_Click);
			// 
			// tsb_map_brush
			// 
			this.tsb_map_brush.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsb_map_brush.Image = global::mapKnight.ToolKit.Properties.Resources.icon_brush;
			this.tsb_map_brush.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsb_map_brush.Name = "tsb_map_brush";
			this.tsb_map_brush.Size = new System.Drawing.Size (24, 22);
			this.tsb_map_brush.Text = "paint tile";
			this.tsb_map_brush.Click += new System.EventHandler (this.tsb_map_brush_Click);
			// 
			// hscrlbar_map
			// 
			this.hscrlbar_map.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.hscrlbar_map.LargeChange = 1;
			this.hscrlbar_map.Location = new System.Drawing.Point (263, 670);
			this.hscrlbar_map.Maximum = 0;
			this.hscrlbar_map.Name = "hscrlbar_map";
			this.hscrlbar_map.Size = new System.Drawing.Size (1071, 17);
			this.hscrlbar_map.TabIndex = 4;
			this.hscrlbar_map.Scroll += new System.Windows.Forms.ScrollEventHandler (this.hscrlbar_map_Scroll);
			// 
			// vscrlbar_map
			// 
			this.vscrlbar_map.Dock = System.Windows.Forms.DockStyle.Right;
			this.vscrlbar_map.LargeChange = 1;
			this.vscrlbar_map.Location = new System.Drawing.Point (1334, 3);
			this.vscrlbar_map.Maximum = 0;
			this.vscrlbar_map.Name = "vscrlbar_map";
			this.vscrlbar_map.Size = new System.Drawing.Size (17, 684);
			this.vscrlbar_map.TabIndex = 3;
			this.vscrlbar_map.Scroll += new System.Windows.Forms.ScrollEventHandler (this.vscrlbr_map_Scroll);
			// 
			// spltcntr_map
			// 
			this.spltcntr_map.Dock = System.Windows.Forms.DockStyle.Left;
			this.spltcntr_map.Location = new System.Drawing.Point (3, 3);
			this.spltcntr_map.Name = "spltcntr_map";
			this.spltcntr_map.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// spltcntr_map.Panel1
			// 
			this.spltcntr_map.Panel1.Controls.Add (this.lvw_tiles);
			// 
			// spltcntr_map.Panel2
			// 
			this.spltcntr_map.Panel2.Controls.Add (this.lvw_overlays);
			this.spltcntr_map.Size = new System.Drawing.Size (260, 684);
			this.spltcntr_map.SplitterDistance = 341;
			this.spltcntr_map.TabIndex = 2;
			// 
			// lvw_tiles
			// 
			this.lvw_tiles.BackColor = System.Drawing.Color.LightGray;
			this.lvw_tiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvw_tiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvw_tiles.LargeImageList = this.imglst_tile;
			this.lvw_tiles.Location = new System.Drawing.Point (0, 0);
			this.lvw_tiles.MultiSelect = false;
			this.lvw_tiles.Name = "lvw_tiles";
			this.lvw_tiles.Size = new System.Drawing.Size (260, 341);
			this.lvw_tiles.TabIndex = 0;
			this.lvw_tiles.UseCompatibleStateImageBehavior = false;
			this.lvw_tiles.SelectedIndexChanged += new System.EventHandler (this.lvw_tiles_SelectedIndexChanged);
			// 
			// imglst_tile
			// 
			this.imglst_tile.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imglst_tile.ImageSize = new System.Drawing.Size (20, 20);
			this.imglst_tile.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// lvw_overlays
			// 
			this.lvw_overlays.BackColor = System.Drawing.Color.LightGray;
			this.lvw_overlays.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvw_overlays.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvw_overlays.LargeImageList = this.imglst_overlay;
			this.lvw_overlays.Location = new System.Drawing.Point (0, 0);
			this.lvw_overlays.MultiSelect = false;
			this.lvw_overlays.Name = "lvw_overlays";
			this.lvw_overlays.Size = new System.Drawing.Size (260, 339);
			this.lvw_overlays.TabIndex = 0;
			this.lvw_overlays.UseCompatibleStateImageBehavior = false;
			this.lvw_overlays.SelectedIndexChanged += new System.EventHandler (this.lvw_overlays_SelectedIndexChanged);
			// 
			// imglst_overlay
			// 
			this.imglst_overlay.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imglst_overlay.ImageSize = new System.Drawing.Size (16, 16);
			this.imglst_overlay.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tbpg_charcreation
			// 
			this.tbpg_charcreation.Controls.Add (this.splitContainer1);
			this.tbpg_charcreation.Location = new System.Drawing.Point (4, 22);
			this.tbpg_charcreation.Name = "tbpg_charcreation";
			this.tbpg_charcreation.Size = new System.Drawing.Size (1354, 690);
			this.tbpg_charcreation.TabIndex = 1;
			this.tbpg_charcreation.Text = "Character Editor";
			this.tbpg_charcreation.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point (0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add (this.treeview_character);
			this.splitContainer1.Panel1.Controls.Add (this.toolStrip1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add (this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size (1354, 690);
			this.splitContainer1.SplitterDistance = 450;
			this.splitContainer1.TabIndex = 0;
			// 
			// treeview_character
			// 
			this.treeview_character.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeview_character.Location = new System.Drawing.Point (0, 25);
			this.treeview_character.Name = "treeview_character";
			this.treeview_character.Size = new System.Drawing.Size (450, 665);
			this.treeview_character.TabIndex = 1;
			this.treeview_character.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler (this.treeview_character_NodeMouseClick);
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.tsbutton_addset,
				this.tsbutton_removeset,
				this.tstextbox_setname
			});
			this.toolStrip1.Location = new System.Drawing.Point (0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size (450, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tsbutton_addset
			// 
			this.tsbutton_addset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbutton_addset.Image = global::mapKnight.ToolKit.Properties.Resources.icon_plus;
			this.tsbutton_addset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbutton_addset.Name = "tsbutton_addset";
			this.tsbutton_addset.Size = new System.Drawing.Size (23, 22);
			this.tsbutton_addset.Text = "toolStripButton1";
			this.tsbutton_addset.Click += new System.EventHandler (this.tsbutton_addset_Click);
			// 
			// tsbutton_removeset
			// 
			this.tsbutton_removeset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbutton_removeset.Image = global::mapKnight.ToolKit.Properties.Resources.icon_minus;
			this.tsbutton_removeset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbutton_removeset.Name = "tsbutton_removeset";
			this.tsbutton_removeset.Size = new System.Drawing.Size (23, 22);
			this.tsbutton_removeset.Text = "toolStripButton2";
			this.tsbutton_removeset.Click += new System.EventHandler (this.tsbutton_removeset_Click);
			// 
			// tstextbox_setname
			// 
			this.tstextbox_setname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tstextbox_setname.Name = "tstextbox_setname";
			this.tstextbox_setname.Size = new System.Drawing.Size (200, 25);
			this.tstextbox_setname.KeyUp += new System.Windows.Forms.KeyEventHandler (this.tstextbox_setname_KeyUp);
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point (0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add (this.splitContainer3);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add (this.splitContainer4);
			this.splitContainer2.Size = new System.Drawing.Size (900, 690);
			this.splitContainer2.SplitterDistance = 299;
			this.splitContainer2.TabIndex = 0;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point (0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add (this.listview_slot);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add (this.textbox_itemdescription);
			this.splitContainer3.Size = new System.Drawing.Size (900, 299);
			this.splitContainer3.SplitterDistance = 230;
			this.splitContainer3.TabIndex = 0;
			// 
			// listview_slot
			// 
			this.listview_slot.BackColor = System.Drawing.Color.White;
			this.listview_slot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listview_slot.Location = new System.Drawing.Point (0, 0);
			this.listview_slot.Name = "listview_slot";
			this.listview_slot.Size = new System.Drawing.Size (230, 299);
			this.listview_slot.TabIndex = 0;
			this.listview_slot.UseCompatibleStateImageBehavior = false;
			this.listview_slot.View = System.Windows.Forms.View.List;
			this.listview_slot.SelectedIndexChanged += new System.EventHandler (this.listview_slot_SelectedIndexChanged);
			this.listview_slot.EnabledChanged += new System.EventHandler (this.listview_slot_EnabledChanged);
			// 
			// textbox_itemdescription
			// 
			this.textbox_itemdescription.AcceptsReturn = true;
			this.textbox_itemdescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textbox_itemdescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textbox_itemdescription.Location = new System.Drawing.Point (0, 0);
			this.textbox_itemdescription.Multiline = true;
			this.textbox_itemdescription.Name = "textbox_itemdescription";
			this.textbox_itemdescription.Size = new System.Drawing.Size (666, 299);
			this.textbox_itemdescription.TabIndex = 0;
			this.textbox_itemdescription.KeyUp += new System.Windows.Forms.KeyEventHandler (this.textbox_itemdescription_KeyUp);
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.Location = new System.Drawing.Point (0, 0);
			this.splitContainer4.Name = "splitContainer4";
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add (this.button_itembitmap);
			this.splitContainer4.Panel1.Controls.Add (this.textbox_itemname);
			this.splitContainer4.Size = new System.Drawing.Size (900, 387);
			this.splitContainer4.SplitterDistance = 402;
			this.splitContainer4.TabIndex = 0;
			// 
			// button_itembitmap
			// 
			this.button_itembitmap.Location = new System.Drawing.Point (4, 4);
			this.button_itembitmap.Name = "button_itembitmap";
			this.button_itembitmap.Size = new System.Drawing.Size (25, 25);
			this.button_itembitmap.TabIndex = 2;
			this.button_itembitmap.Text = "...";
			this.button_itembitmap.UseVisualStyleBackColor = true;
			this.button_itembitmap.Click += new System.EventHandler (this.button_itembitmap_Click);
			// 
			// textbox_itemname
			// 
			this.textbox_itemname.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textbox_itemname.Location = new System.Drawing.Point (0, 367);
			this.textbox_itemname.Name = "textbox_itemname";
			this.textbox_itemname.Size = new System.Drawing.Size (402, 20);
			this.textbox_itemname.TabIndex = 0;
			this.textbox_itemname.KeyUp += new System.Windows.Forms.KeyEventHandler (this.textbox_itemname_KeyUp);
			// 
			// tbpg_animeditor
			// 
			this.tbpg_animeditor.Location = new System.Drawing.Point (4, 22);
			this.tbpg_animeditor.Name = "tbpg_animeditor";
			this.tbpg_animeditor.Size = new System.Drawing.Size (1354, 690);
			this.tbpg_animeditor.TabIndex = 2;
			this.tbpg_animeditor.Text = "Animation Editor";
			this.tbpg_animeditor.UseVisualStyleBackColor = true;
			// 
			// ofd_mapfile
			// 
			this.ofd_mapfile.Filter = "WorkFile-Dateien|*.workfile";
			this.ofd_mapfile.InitialDirectory = "Personal";
			this.ofd_mapfile.RestoreDirectory = true;
			// 
			// sfd_mapfile
			// 
			this.sfd_mapfile.Filter = "WorkFile-Dateien|*.workfile";
			// 
			// folderBrowserDialog
			// 
			this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
			// 
			// ofd_imageedit
			// 
			this.ofd_imageedit.FileName = "openFileDialog1";
			this.ofd_imageedit.Filter = "PNG-Dateien|*.png|JPEG-Dateien|*.jpeg|Alle Dateien|*.*";
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (1362, 741);
			this.Controls.Add (this.tbctrl_main);
			this.Controls.Add (this.tlstrp_main);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject ("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "Main";
			this.Text = "mapKnight ToolKit";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler (this.Main_FormClosing);
			this.Load += new System.EventHandler (this.Main_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler (this.Main_KeyDown);
			this.Resize += new System.EventHandler (this.Main_Resize);
			this.tlstrp_main.ResumeLayout (false);
			this.tlstrp_main.PerformLayout ();
			this.tbctrl_main.ResumeLayout (false);
			this.tbpg_mapeditor.ResumeLayout (false);
			this.tlstrp_map.ResumeLayout (false);
			this.tlstrp_map.PerformLayout ();
			this.spltcntr_map.Panel1.ResumeLayout (false);
			this.spltcntr_map.Panel2.ResumeLayout (false);
			((System.ComponentModel.ISupportInitialize)(this.spltcntr_map)).EndInit ();
			this.spltcntr_map.ResumeLayout (false);
			this.tbpg_charcreation.ResumeLayout (false);
			this.splitContainer1.Panel1.ResumeLayout (false);
			this.splitContainer1.Panel1.PerformLayout ();
			this.splitContainer1.Panel2.ResumeLayout (false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit ();
			this.splitContainer1.ResumeLayout (false);
			this.toolStrip1.ResumeLayout (false);
			this.toolStrip1.PerformLayout ();
			this.splitContainer2.Panel1.ResumeLayout (false);
			this.splitContainer2.Panel2.ResumeLayout (false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit ();
			this.splitContainer2.ResumeLayout (false);
			this.splitContainer3.Panel1.ResumeLayout (false);
			this.splitContainer3.Panel2.ResumeLayout (false);
			this.splitContainer3.Panel2.PerformLayout ();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit ();
			this.splitContainer3.ResumeLayout (false);
			this.splitContainer4.Panel1.ResumeLayout (false);
			this.splitContainer4.Panel1.PerformLayout ();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit ();
			this.splitContainer4.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.ToolStrip tlstrp_main;
		private System.Windows.Forms.TabControl tbctrl_main;
		private System.Windows.Forms.TabPage tbpg_mapeditor;
		private System.Windows.Forms.ToolStripDropDownButton tsddb_file;
		private System.Windows.Forms.SplitContainer spltcntr_map;
		private System.Windows.Forms.ListView lvw_tiles;
		private System.Windows.Forms.ListView lvw_overlays;
		private System.Windows.Forms.ToolStrip tlstrp_map;
		private System.Windows.Forms.HScrollBar hscrlbar_map;
		private System.Windows.Forms.VScrollBar vscrlbar_map;
		private System.Windows.Forms.ToolStripButton tsb_map_wand;
		private System.Windows.Forms.ToolStripButton tsb_map_fill;
		private System.Windows.Forms.TabPage tbpg_charcreation;
		private System.Windows.Forms.TabPage tbpg_animeditor;
		private System.Windows.Forms.ToolStripButton tsb_map_select;
		private System.Windows.Forms.ToolStripButton tsb_map_eraser;
		private System.Windows.Forms.ToolStripButton tsb_map_brush;
		private System.Windows.Forms.ToolStripDropDownButton tsddb_edit;
		private System.Windows.Forms.ToolStripButton tsb_info;
		private System.Windows.Forms.ImageList imglst_tile;
		private System.Windows.Forms.ImageList imglst_overlay;
		private System.Windows.Forms.ToolStripMenuItem tsmi_load;
		private System.Windows.Forms.ToolStripMenuItem tsmi_save;
		private System.Windows.Forms.ToolStripMenuItem tsmi_saveas;
		private System.Windows.Forms.ToolStripMenuItem tsmi_undo;
		private System.Windows.Forms.OpenFileDialog ofd_mapfile;
		private System.Windows.Forms.SaveFileDialog sfd_mapfile;
		private System.Windows.Forms.ToolStripComboBox tscb_map_mapselect;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton tsb_addnew;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem tsmmi_exportraw;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.OpenFileDialog ofd_imageedit;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView treeview_character;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private System.Windows.Forms.Button button_itembitmap;
		private System.Windows.Forms.TextBox textbox_itemname;
		private System.Windows.Forms.ToolStripButton tsbutton_addset;
		private System.Windows.Forms.ToolStripButton tsbutton_removeset;
		private System.Windows.Forms.ToolStripTextBox tstextbox_setname;
		private System.Windows.Forms.TextBox textbox_itemdescription;
		private System.Windows.Forms.ListView listview_slot;
	}
}