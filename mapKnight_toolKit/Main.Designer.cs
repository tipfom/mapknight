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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.tlstrp_main = new System.Windows.Forms.ToolStrip();
            this.tsddb_file = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmi_load = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_save = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_saveas = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmmi_exportraw = new System.Windows.Forms.ToolStripMenuItem();
            this.tsddb_edit = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmi_undo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsb_info = new System.Windows.Forms.ToolStripButton();
            this.tbctrl_main = new System.Windows.Forms.TabControl();
            this.tbpg_mapeditor = new System.Windows.Forms.TabPage();
            this.tlstrp_map = new System.Windows.Forms.ToolStrip();
            this.tscb_map_mapselect = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_addnew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_map_wand = new System.Windows.Forms.ToolStripButton();
            this.tsb_map_fill = new System.Windows.Forms.ToolStripButton();
            this.tsb_map_select = new System.Windows.Forms.ToolStripButton();
            this.tsb_map_eraser = new System.Windows.Forms.ToolStripButton();
            this.tsb_map_brush = new System.Windows.Forms.ToolStripButton();
            this.hscrlbar_map = new System.Windows.Forms.HScrollBar();
            this.vscrlbar_map = new System.Windows.Forms.VScrollBar();
            this.spltcntr_map = new System.Windows.Forms.SplitContainer();
            this.lvw_tiles = new System.Windows.Forms.ListView();
            this.imglst_tile = new System.Windows.Forms.ImageList(this.components);
            this.lvw_overlays = new System.Windows.Forms.ListView();
            this.imglst_overlay = new System.Windows.Forms.ImageList(this.components);
            this.tbpg_charcreation = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeview_character = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbutton_addset = new System.Windows.Forms.ToolStripButton();
            this.tsbutton_removeset = new System.Windows.Forms.ToolStripButton();
            this.tstextbox_setname = new System.Windows.Forms.ToolStripTextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.listview_slot = new System.Windows.Forms.ListView();
            this.textbox_itemdescription = new System.Windows.Forms.TextBox();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.button_itembitmap = new System.Windows.Forms.Button();
            this.textbox_itemname = new System.Windows.Forms.TextBox();
            this.tbpg_animeditor = new System.Windows.Forms.TabPage();
            this.panel_anim_editstep = new mapKnight.ToolKit.FlickerFreePanel();
            this.numericUpDown_anim_time = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDown_anim_rot = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown_anim_y = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown_anim_x = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.button_anim_mirrored = new System.Windows.Forms.Button();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel_anim_editanim = new System.Windows.Forms.Panel();
            this.button_anim_loopable = new System.Windows.Forms.Button();
            this.button_anim_abortable = new System.Windows.Forms.Button();
            this.trackBar_anim_progress = new System.Windows.Forms.TrackBar();
            this.button_anim_repeat = new System.Windows.Forms.Button();
            this.button_anim_pause = new System.Windows.Forms.Button();
            this.button_anim_play = new System.Windows.Forms.Button();
            this.textBox_anim_action = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitContainer_anim = new System.Windows.Forms.SplitContainer();
            this.treeView_anim = new mapKnight.ToolKit.AddRemoveTreeView();
            this.tbpg_entity = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tbpg_terminal = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listbox_commands = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textbox_commandinput = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listbox_log = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.textbox_ip = new System.Windows.Forms.ToolStripTextBox();
            this.button_connect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.label_connectionstate = new System.Windows.Forms.ToolStripLabel();
            this.ofd_mapfile = new System.Windows.Forms.OpenFileDialog();
            this.sfd_mapfile = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.ofd_imageedit = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tlstrp_main.SuspendLayout();
            this.tbctrl_main.SuspendLayout();
            this.tbpg_mapeditor.SuspendLayout();
            this.tlstrp_map.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltcntr_map)).BeginInit();
            this.spltcntr_map.Panel1.SuspendLayout();
            this.spltcntr_map.Panel2.SuspendLayout();
            this.spltcntr_map.SuspendLayout();
            this.tbpg_charcreation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.tbpg_animeditor.SuspendLayout();
            this.panel_anim_editstep.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_anim_time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_anim_rot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_anim_y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_anim_x)).BeginInit();
            this.panel_anim_editanim.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_anim_progress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_anim)).BeginInit();
            this.splitContainer_anim.Panel1.SuspendLayout();
            this.splitContainer_anim.SuspendLayout();
            this.tbpg_entity.SuspendLayout();
            this.tbpg_terminal.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlstrp_main
            // 
            this.tlstrp_main.BackColor = System.Drawing.SystemColors.Control;
            this.tlstrp_main.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tlstrp_main.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tlstrp_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddb_file,
            this.tsddb_edit,
            this.tsb_info});
            this.tlstrp_main.Location = new System.Drawing.Point(0, 0);
            this.tlstrp_main.Name = "tlstrp_main";
            this.tlstrp_main.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tlstrp_main.Size = new System.Drawing.Size(1362, 25);
            this.tlstrp_main.TabIndex = 0;
            // 
            // tsddb_file
            // 
            this.tsddb_file.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddb_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_load,
            this.tsmi_save,
            this.tsmi_saveas,
            this.toolStripSeparator3,
            this.tsmmi_exportraw});
            this.tsddb_file.Image = ((System.Drawing.Image)(resources.GetObject("tsddb_file.Image")));
            this.tsddb_file.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddb_file.Name = "tsddb_file";
            this.tsddb_file.Size = new System.Drawing.Size(38, 22);
            this.tsddb_file.Text = "File";
            // 
            // tsmi_load
            // 
            this.tsmi_load.Name = "tsmi_load";
            this.tsmi_load.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.tsmi_load.Size = new System.Drawing.Size(201, 22);
            this.tsmi_load.Text = "Load";
            this.tsmi_load.Click += new System.EventHandler(this.tsmi_load_Click);
            // 
            // tsmi_save
            // 
            this.tsmi_save.Name = "tsmi_save";
            this.tsmi_save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmi_save.Size = new System.Drawing.Size(201, 22);
            this.tsmi_save.Text = "Save";
            this.tsmi_save.Click += new System.EventHandler(this.tsmi_save_Click);
            // 
            // tsmi_saveas
            // 
            this.tsmi_saveas.Name = "tsmi_saveas";
            this.tsmi_saveas.Size = new System.Drawing.Size(201, 22);
            this.tsmi_saveas.Text = "Save As";
            this.tsmi_saveas.Click += new System.EventHandler(this.tsmi_saveas_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(198, 6);
            // 
            // tsmmi_exportraw
            // 
            this.tsmmi_exportraw.Name = "tsmmi_exportraw";
            this.tsmmi_exportraw.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.tsmmi_exportraw.Size = new System.Drawing.Size(201, 22);
            this.tsmmi_exportraw.Text = "Export Raw Data";
            this.tsmmi_exportraw.Click += new System.EventHandler(this.tsmmi_exportraw_Click);
            // 
            // tsddb_edit
            // 
            this.tsddb_edit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddb_edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_undo});
            this.tsddb_edit.Image = ((System.Drawing.Image)(resources.GetObject("tsddb_edit.Image")));
            this.tsddb_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddb_edit.Name = "tsddb_edit";
            this.tsddb_edit.Size = new System.Drawing.Size(40, 22);
            this.tsddb_edit.Text = "Edit";
            // 
            // tsmi_undo
            // 
            this.tsmi_undo.Name = "tsmi_undo";
            this.tsmi_undo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.tsmi_undo.Size = new System.Drawing.Size(146, 22);
            this.tsmi_undo.Text = "Undo";
            this.tsmi_undo.Click += new System.EventHandler(this.tsmi_undo_Click);
            // 
            // tsb_info
            // 
            this.tsb_info.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsb_info.Image = ((System.Drawing.Image)(resources.GetObject("tsb_info.Image")));
            this.tsb_info.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_info.Name = "tsb_info";
            this.tsb_info.Size = new System.Drawing.Size(32, 22);
            this.tsb_info.Text = "Info";
            this.tsb_info.Click += new System.EventHandler(this.tsb_info_Click);
            // 
            // tbctrl_main
            // 
            this.tbctrl_main.Controls.Add(this.tbpg_mapeditor);
            this.tbctrl_main.Controls.Add(this.tbpg_charcreation);
            this.tbctrl_main.Controls.Add(this.tbpg_animeditor);
            this.tbctrl_main.Controls.Add(this.tbpg_entity);
            this.tbctrl_main.Controls.Add(this.tbpg_terminal);
            this.tbctrl_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbctrl_main.Location = new System.Drawing.Point(0, 25);
            this.tbctrl_main.Name = "tbctrl_main";
            this.tbctrl_main.SelectedIndex = 0;
            this.tbctrl_main.Size = new System.Drawing.Size(1362, 716);
            this.tbctrl_main.TabIndex = 1;
            this.tbctrl_main.SelectedIndexChanged += new System.EventHandler(this.tbctrl_main_SelectedIndexChanged);
            // 
            // tbpg_mapeditor
            // 
            this.tbpg_mapeditor.BackColor = System.Drawing.Color.White;
            this.tbpg_mapeditor.Controls.Add(this.tlstrp_map);
            this.tbpg_mapeditor.Controls.Add(this.hscrlbar_map);
            this.tbpg_mapeditor.Controls.Add(this.vscrlbar_map);
            this.tbpg_mapeditor.Controls.Add(this.spltcntr_map);
            this.tbpg_mapeditor.Location = new System.Drawing.Point(4, 22);
            this.tbpg_mapeditor.Name = "tbpg_mapeditor";
            this.tbpg_mapeditor.Padding = new System.Windows.Forms.Padding(3);
            this.tbpg_mapeditor.Size = new System.Drawing.Size(1354, 690);
            this.tbpg_mapeditor.TabIndex = 0;
            this.tbpg_mapeditor.Text = "MapEditor";
            this.tbpg_mapeditor.Paint += new System.Windows.Forms.PaintEventHandler(this.tbpg_mapeditor_Paint);
            this.tbpg_mapeditor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbpg_mapeditor_MouseDown);
            this.tbpg_mapeditor.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tbpg_mapeditor_MouseMove);
            this.tbpg_mapeditor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbpg_mapeditor_MouseUp);
            this.tbpg_mapeditor.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.tbpg_mapeditor_MouseWheel);
            // 
            // tlstrp_map
            // 
            this.tlstrp_map.AutoSize = false;
            this.tlstrp_map.BackColor = System.Drawing.Color.LightGray;
            this.tlstrp_map.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tlstrp_map.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tlstrp_map.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tlstrp_map.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscb_map_mapselect,
            this.toolStripSeparator2,
            this.tsb_addnew,
            this.toolStripSeparator1,
            this.tsb_map_wand,
            this.tsb_map_fill,
            this.tsb_map_select,
            this.tsb_map_eraser,
            this.tsb_map_brush});
            this.tlstrp_map.Location = new System.Drawing.Point(263, 3);
            this.tlstrp_map.Name = "tlstrp_map";
            this.tlstrp_map.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tlstrp_map.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tlstrp_map.Size = new System.Drawing.Size(1071, 25);
            this.tlstrp_map.TabIndex = 5;
            this.tlstrp_map.Text = "toolStrip1";
            // 
            // tscb_map_mapselect
            // 
            this.tscb_map_mapselect.Name = "tscb_map_mapselect";
            this.tscb_map_mapselect.Size = new System.Drawing.Size(121, 25);
            this.tscb_map_mapselect.SelectedIndexChanged += new System.EventHandler(this.tscb_map_mapselect_SelectedIndexChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsb_addnew
            // 
            this.tsb_addnew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_addnew.Image = global::mapKnight.ToolKit.Properties.Resources.icon_plus;
            this.tsb_addnew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_addnew.Name = "tsb_addnew";
            this.tsb_addnew.Size = new System.Drawing.Size(24, 22);
            this.tsb_addnew.Text = "add new";
            this.tsb_addnew.Click += new System.EventHandler(this.tsb_addnew_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsb_map_wand
            // 
            this.tsb_map_wand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_map_wand.Image = global::mapKnight.ToolKit.Properties.Resources.icon_finger;
            this.tsb_map_wand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_map_wand.Name = "tsb_map_wand";
            this.tsb_map_wand.Size = new System.Drawing.Size(24, 22);
            this.tsb_map_wand.Text = "select spawnpoint";
            this.tsb_map_wand.Click += new System.EventHandler(this.tsb_map_wand_Click);
            // 
            // tsb_map_fill
            // 
            this.tsb_map_fill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_map_fill.Image = global::mapKnight.ToolKit.Properties.Resources.icon_bucket;
            this.tsb_map_fill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_map_fill.Name = "tsb_map_fill";
            this.tsb_map_fill.Size = new System.Drawing.Size(24, 22);
            this.tsb_map_fill.Text = "fill area";
            this.tsb_map_fill.Click += new System.EventHandler(this.tsb_map_fill_Click);
            // 
            // tsb_map_select
            // 
            this.tsb_map_select.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_map_select.Image = global::mapKnight.ToolKit.Properties.Resources.icon_pipette;
            this.tsb_map_select.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_map_select.Name = "tsb_map_select";
            this.tsb_map_select.Size = new System.Drawing.Size(24, 22);
            this.tsb_map_select.Text = "select tile";
            this.tsb_map_select.Click += new System.EventHandler(this.tsb_map_select_Click);
            // 
            // tsb_map_eraser
            // 
            this.tsb_map_eraser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_map_eraser.Image = global::mapKnight.ToolKit.Properties.Resources.icon_eraser;
            this.tsb_map_eraser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_map_eraser.Name = "tsb_map_eraser";
            this.tsb_map_eraser.Size = new System.Drawing.Size(24, 22);
            this.tsb_map_eraser.Text = "erase tile";
            this.tsb_map_eraser.Click += new System.EventHandler(this.tsb_map_eraser_Click);
            // 
            // tsb_map_brush
            // 
            this.tsb_map_brush.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_map_brush.Image = global::mapKnight.ToolKit.Properties.Resources.icon_brush;
            this.tsb_map_brush.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_map_brush.Name = "tsb_map_brush";
            this.tsb_map_brush.Size = new System.Drawing.Size(24, 22);
            this.tsb_map_brush.Text = "paint tile";
            this.tsb_map_brush.Click += new System.EventHandler(this.tsb_map_brush_Click);
            // 
            // hscrlbar_map
            // 
            this.hscrlbar_map.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hscrlbar_map.LargeChange = 1;
            this.hscrlbar_map.Location = new System.Drawing.Point(263, 670);
            this.hscrlbar_map.Maximum = 0;
            this.hscrlbar_map.Name = "hscrlbar_map";
            this.hscrlbar_map.Size = new System.Drawing.Size(1071, 17);
            this.hscrlbar_map.TabIndex = 4;
            this.hscrlbar_map.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hscrlbar_map_Scroll);
            // 
            // vscrlbar_map
            // 
            this.vscrlbar_map.Dock = System.Windows.Forms.DockStyle.Right;
            this.vscrlbar_map.LargeChange = 1;
            this.vscrlbar_map.Location = new System.Drawing.Point(1334, 3);
            this.vscrlbar_map.Maximum = 0;
            this.vscrlbar_map.Name = "vscrlbar_map";
            this.vscrlbar_map.Size = new System.Drawing.Size(17, 684);
            this.vscrlbar_map.TabIndex = 3;
            this.vscrlbar_map.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vscrlbr_map_Scroll);
            // 
            // spltcntr_map
            // 
            this.spltcntr_map.Dock = System.Windows.Forms.DockStyle.Left;
            this.spltcntr_map.Location = new System.Drawing.Point(3, 3);
            this.spltcntr_map.Name = "spltcntr_map";
            this.spltcntr_map.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spltcntr_map.Panel1
            // 
            this.spltcntr_map.Panel1.Controls.Add(this.lvw_tiles);
            // 
            // spltcntr_map.Panel2
            // 
            this.spltcntr_map.Panel2.Controls.Add(this.lvw_overlays);
            this.spltcntr_map.Size = new System.Drawing.Size(260, 684);
            this.spltcntr_map.SplitterDistance = 341;
            this.spltcntr_map.TabIndex = 2;
            // 
            // lvw_tiles
            // 
            this.lvw_tiles.BackColor = System.Drawing.Color.LightGray;
            this.lvw_tiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvw_tiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvw_tiles.LargeImageList = this.imglst_tile;
            this.lvw_tiles.Location = new System.Drawing.Point(0, 0);
            this.lvw_tiles.MultiSelect = false;
            this.lvw_tiles.Name = "lvw_tiles";
            this.lvw_tiles.Size = new System.Drawing.Size(260, 341);
            this.lvw_tiles.TabIndex = 0;
            this.lvw_tiles.UseCompatibleStateImageBehavior = false;
            this.lvw_tiles.SelectedIndexChanged += new System.EventHandler(this.lvw_tiles_SelectedIndexChanged);
            // 
            // imglst_tile
            // 
            this.imglst_tile.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imglst_tile.ImageSize = new System.Drawing.Size(20, 20);
            this.imglst_tile.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lvw_overlays
            // 
            this.lvw_overlays.BackColor = System.Drawing.Color.LightGray;
            this.lvw_overlays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvw_overlays.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvw_overlays.LargeImageList = this.imglst_overlay;
            this.lvw_overlays.Location = new System.Drawing.Point(0, 0);
            this.lvw_overlays.MultiSelect = false;
            this.lvw_overlays.Name = "lvw_overlays";
            this.lvw_overlays.Size = new System.Drawing.Size(260, 339);
            this.lvw_overlays.TabIndex = 0;
            this.lvw_overlays.UseCompatibleStateImageBehavior = false;
            this.lvw_overlays.SelectedIndexChanged += new System.EventHandler(this.lvw_overlays_SelectedIndexChanged);
            // 
            // imglst_overlay
            // 
            this.imglst_overlay.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imglst_overlay.ImageSize = new System.Drawing.Size(16, 16);
            this.imglst_overlay.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tbpg_charcreation
            // 
            this.tbpg_charcreation.Controls.Add(this.splitContainer1);
            this.tbpg_charcreation.Location = new System.Drawing.Point(4, 22);
            this.tbpg_charcreation.Name = "tbpg_charcreation";
            this.tbpg_charcreation.Size = new System.Drawing.Size(1354, 690);
            this.tbpg_charcreation.TabIndex = 1;
            this.tbpg_charcreation.Text = "Character Editor";
            this.tbpg_charcreation.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeview_character);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1354, 690);
            this.splitContainer1.SplitterDistance = 450;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeview_character
            // 
            this.treeview_character.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeview_character.Location = new System.Drawing.Point(0, 25);
            this.treeview_character.Name = "treeview_character";
            this.treeview_character.Size = new System.Drawing.Size(450, 665);
            this.treeview_character.TabIndex = 1;
            this.treeview_character.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeview_character_NodeMouseClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbutton_addset,
            this.tsbutton_removeset,
            this.tstextbox_setname});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(450, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbutton_addset
            // 
            this.tsbutton_addset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbutton_addset.Image = global::mapKnight.ToolKit.Properties.Resources.icon_plus;
            this.tsbutton_addset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbutton_addset.Name = "tsbutton_addset";
            this.tsbutton_addset.Size = new System.Drawing.Size(23, 22);
            this.tsbutton_addset.Text = "toolStripButton1";
            this.tsbutton_addset.Click += new System.EventHandler(this.tsbutton_addset_Click);
            // 
            // tsbutton_removeset
            // 
            this.tsbutton_removeset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbutton_removeset.Image = global::mapKnight.ToolKit.Properties.Resources.icon_minus;
            this.tsbutton_removeset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbutton_removeset.Name = "tsbutton_removeset";
            this.tsbutton_removeset.Size = new System.Drawing.Size(23, 22);
            this.tsbutton_removeset.Text = "toolStripButton2";
            this.tsbutton_removeset.Click += new System.EventHandler(this.tsbutton_removeset_Click);
            // 
            // tstextbox_setname
            // 
            this.tstextbox_setname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tstextbox_setname.Name = "tstextbox_setname";
            this.tstextbox_setname.Size = new System.Drawing.Size(200, 25);
            this.tstextbox_setname.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tstextbox_setname_KeyUp);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textbox_itemdescription);
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer2.Size = new System.Drawing.Size(900, 690);
            this.splitContainer2.SplitterDistance = 299;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.listview_slot);
            this.splitContainer3.Size = new System.Drawing.Size(900, 299);
            this.splitContainer3.SplitterDistance = 230;
            this.splitContainer3.TabIndex = 0;
            // 
            // listview_slot
            // 
            this.listview_slot.BackColor = System.Drawing.Color.White;
            this.listview_slot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listview_slot.Location = new System.Drawing.Point(0, 0);
            this.listview_slot.Name = "listview_slot";
            this.listview_slot.Size = new System.Drawing.Size(230, 299);
            this.listview_slot.TabIndex = 0;
            this.listview_slot.UseCompatibleStateImageBehavior = false;
            this.listview_slot.View = System.Windows.Forms.View.List;
            this.listview_slot.SelectedIndexChanged += new System.EventHandler(this.listview_slot_SelectedIndexChanged);
            this.listview_slot.EnabledChanged += new System.EventHandler(this.listview_slot_EnabledChanged);
            // 
            // textbox_itemdescription
            // 
            this.textbox_itemdescription.AcceptsReturn = true;
            this.textbox_itemdescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textbox_itemdescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textbox_itemdescription.Location = new System.Drawing.Point(0, 0);
            this.textbox_itemdescription.Multiline = true;
            this.textbox_itemdescription.Name = "textbox_itemdescription";
            this.textbox_itemdescription.Size = new System.Drawing.Size(900, 387);
            this.textbox_itemdescription.TabIndex = 0;
            this.textbox_itemdescription.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textbox_itemdescription_KeyUp);
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.button_itembitmap);
            this.splitContainer4.Panel1.Controls.Add(this.textbox_itemname);
            this.splitContainer4.Size = new System.Drawing.Size(900, 387);
            this.splitContainer4.SplitterDistance = 402;
            this.splitContainer4.TabIndex = 0;
            // 
            // button_itembitmap
            // 
            this.button_itembitmap.Location = new System.Drawing.Point(4, 4);
            this.button_itembitmap.Name = "button_itembitmap";
            this.button_itembitmap.Size = new System.Drawing.Size(25, 25);
            this.button_itembitmap.TabIndex = 2;
            this.button_itembitmap.Text = "...";
            this.button_itembitmap.UseVisualStyleBackColor = true;
            this.button_itembitmap.Click += new System.EventHandler(this.button_itembitmap_Click);
            // 
            // textbox_itemname
            // 
            this.textbox_itemname.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textbox_itemname.Location = new System.Drawing.Point(0, 367);
            this.textbox_itemname.Name = "textbox_itemname";
            this.textbox_itemname.Size = new System.Drawing.Size(402, 20);
            this.textbox_itemname.TabIndex = 0;
            this.textbox_itemname.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textbox_itemname_KeyUp);
            // 
            // tbpg_animeditor
            // 
            this.tbpg_animeditor.Controls.Add(this.panel_anim_editstep);
            this.tbpg_animeditor.Controls.Add(this.panel_anim_editanim);
            this.tbpg_animeditor.Controls.Add(this.splitContainer_anim);
            this.tbpg_animeditor.Location = new System.Drawing.Point(4, 22);
            this.tbpg_animeditor.Name = "tbpg_animeditor";
            this.tbpg_animeditor.Size = new System.Drawing.Size(1354, 690);
            this.tbpg_animeditor.TabIndex = 2;
            this.tbpg_animeditor.Text = "Animation Editor";
            this.tbpg_animeditor.UseVisualStyleBackColor = true;
            // 
            // panel_anim_editstep
            // 
            this.panel_anim_editstep.Controls.Add(this.numericUpDown_anim_time);
            this.panel_anim_editstep.Controls.Add(this.label4);
            this.panel_anim_editstep.Controls.Add(this.label8);
            this.panel_anim_editstep.Controls.Add(this.numericUpDown_anim_rot);
            this.panel_anim_editstep.Controls.Add(this.label7);
            this.panel_anim_editstep.Controls.Add(this.numericUpDown_anim_y);
            this.panel_anim_editstep.Controls.Add(this.label6);
            this.panel_anim_editstep.Controls.Add(this.numericUpDown_anim_x);
            this.panel_anim_editstep.Controls.Add(this.label5);
            this.panel_anim_editstep.Controls.Add(this.button_anim_mirrored);
            this.panel_anim_editstep.Controls.Add(this.splitter2);
            this.panel_anim_editstep.Location = new System.Drawing.Point(127, 167);
            this.panel_anim_editstep.Name = "panel_anim_editstep";
            this.panel_anim_editstep.Size = new System.Drawing.Size(968, 427);
            this.panel_anim_editstep.TabIndex = 0;
            this.panel_anim_editstep.Visible = false;
            this.panel_anim_editstep.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_anim_editstep_Paint);
            this.panel_anim_editstep.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_anim_editstep_MouseDown);
            this.panel_anim_editstep.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_anim_editstep_MouseMove);
            this.panel_anim_editstep.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_anim_editstep_MouseUp);
            // 
            // numericUpDown_anim_time
            // 
            this.numericUpDown_anim_time.Location = new System.Drawing.Point(756, 8);
            this.numericUpDown_anim_time.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_anim_time.Name = "numericUpDown_anim_time";
            this.numericUpDown_anim_time.Size = new System.Drawing.Size(132, 20);
            this.numericUpDown_anim_time.TabIndex = 16;
            this.numericUpDown_anim_time.ValueChanged += new System.EventHandler(this.numericUpDown_anim_time_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(675, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "AnimTime (ms)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(512, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Mirrored";
            // 
            // numericUpDown_anim_rot
            // 
            this.numericUpDown_anim_rot.Location = new System.Drawing.Point(374, 8);
            this.numericUpDown_anim_rot.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDown_anim_rot.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.numericUpDown_anim_rot.Name = "numericUpDown_anim_rot";
            this.numericUpDown_anim_rot.Size = new System.Drawing.Size(132, 20);
            this.numericUpDown_anim_rot.TabIndex = 13;
            this.numericUpDown_anim_rot.ValueChanged += new System.EventHandler(this.numericUpDown_anim_rot_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(321, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Rotation";
            // 
            // numericUpDown_anim_y
            // 
            this.numericUpDown_anim_y.Location = new System.Drawing.Point(183, 8);
            this.numericUpDown_anim_y.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown_anim_y.Name = "numericUpDown_anim_y";
            this.numericUpDown_anim_y.Size = new System.Drawing.Size(132, 20);
            this.numericUpDown_anim_y.TabIndex = 11;
            this.numericUpDown_anim_y.ValueChanged += new System.EventHandler(this.numericUpDown_anim_y_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(163, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Y";
            // 
            // numericUpDown_anim_x
            // 
            this.numericUpDown_anim_x.Location = new System.Drawing.Point(25, 8);
            this.numericUpDown_anim_x.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown_anim_x.Name = "numericUpDown_anim_x";
            this.numericUpDown_anim_x.Size = new System.Drawing.Size(132, 20);
            this.numericUpDown_anim_x.TabIndex = 9;
            this.numericUpDown_anim_x.ValueChanged += new System.EventHandler(this.numericUpDown_anim_x_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(5, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "X";
            // 
            // button_anim_mirrored
            // 
            this.button_anim_mirrored.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button_anim_mirrored.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_anim_mirrored.Location = new System.Drawing.Point(563, 5);
            this.button_anim_mirrored.Name = "button_anim_mirrored";
            this.button_anim_mirrored.Size = new System.Drawing.Size(106, 23);
            this.button_anim_mirrored.TabIndex = 6;
            this.button_anim_mirrored.Text = "false";
            this.button_anim_mirrored.UseVisualStyleBackColor = false;
            this.button_anim_mirrored.Click += new System.EventHandler(this.button_anim_mirrored_Click);
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.Color.White;
            this.splitter2.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(968, 37);
            this.splitter2.TabIndex = 0;
            this.splitter2.TabStop = false;
            // 
            // panel_anim_editanim
            // 
            this.panel_anim_editanim.Controls.Add(this.button_anim_loopable);
            this.panel_anim_editanim.Controls.Add(this.button_anim_abortable);
            this.panel_anim_editanim.Controls.Add(this.trackBar_anim_progress);
            this.panel_anim_editanim.Controls.Add(this.button_anim_repeat);
            this.panel_anim_editanim.Controls.Add(this.button_anim_pause);
            this.panel_anim_editanim.Controls.Add(this.button_anim_play);
            this.panel_anim_editanim.Controls.Add(this.textBox_anim_action);
            this.panel_anim_editanim.Controls.Add(this.label3);
            this.panel_anim_editanim.Controls.Add(this.label2);
            this.panel_anim_editanim.Controls.Add(this.label1);
            this.panel_anim_editanim.Controls.Add(this.splitter1);
            this.panel_anim_editanim.Location = new System.Drawing.Point(64, 100);
            this.panel_anim_editanim.Name = "panel_anim_editanim";
            this.panel_anim_editanim.Size = new System.Drawing.Size(519, 427);
            this.panel_anim_editanim.TabIndex = 1;
            this.panel_anim_editanim.Visible = false;
            // 
            // button_anim_loopable
            // 
            this.button_anim_loopable.Location = new System.Drawing.Point(119, 4);
            this.button_anim_loopable.Name = "button_anim_loopable";
            this.button_anim_loopable.Size = new System.Drawing.Size(173, 23);
            this.button_anim_loopable.TabIndex = 13;
            this.button_anim_loopable.Text = "false";
            this.button_anim_loopable.UseVisualStyleBackColor = true;
            this.button_anim_loopable.Click += new System.EventHandler(this.button_anim_loopable_Click);
            // 
            // button_anim_abortable
            // 
            this.button_anim_abortable.Location = new System.Drawing.Point(119, 33);
            this.button_anim_abortable.Name = "button_anim_abortable";
            this.button_anim_abortable.Size = new System.Drawing.Size(173, 23);
            this.button_anim_abortable.TabIndex = 12;
            this.button_anim_abortable.Text = "false";
            this.button_anim_abortable.UseVisualStyleBackColor = true;
            this.button_anim_abortable.Click += new System.EventHandler(this.button_anim_abortable_Click);
            // 
            // trackBar_anim_progress
            // 
            this.trackBar_anim_progress.AutoSize = false;
            this.trackBar_anim_progress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trackBar_anim_progress.LargeChange = 2;
            this.trackBar_anim_progress.Location = new System.Drawing.Point(0, 399);
            this.trackBar_anim_progress.Name = "trackBar_anim_progress";
            this.trackBar_anim_progress.Size = new System.Drawing.Size(519, 28);
            this.trackBar_anim_progress.TabIndex = 11;
            this.trackBar_anim_progress.ValueChanged += new System.EventHandler(this.trackBar_anim_progress_ValueChanged);
            // 
            // button_anim_repeat
            // 
            this.button_anim_repeat.BackgroundImage = global::mapKnight.ToolKit.Properties.Resources.icon_refresh;
            this.button_anim_repeat.Location = new System.Drawing.Point(127, 100);
            this.button_anim_repeat.Name = "button_anim_repeat";
            this.button_anim_repeat.Size = new System.Drawing.Size(50, 50);
            this.button_anim_repeat.TabIndex = 10;
            this.button_anim_repeat.UseVisualStyleBackColor = true;
            this.button_anim_repeat.Click += new System.EventHandler(this.button_anim_repeat_Click);
            // 
            // button_anim_pause
            // 
            this.button_anim_pause.BackgroundImage = global::mapKnight.ToolKit.Properties.Resources.icon_pause;
            this.button_anim_pause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_anim_pause.Location = new System.Drawing.Point(71, 100);
            this.button_anim_pause.Name = "button_anim_pause";
            this.button_anim_pause.Size = new System.Drawing.Size(50, 50);
            this.button_anim_pause.TabIndex = 8;
            this.button_anim_pause.UseVisualStyleBackColor = true;
            this.button_anim_pause.Click += new System.EventHandler(this.button_anim_pause_Click);
            // 
            // button_anim_play
            // 
            this.button_anim_play.BackgroundImage = global::mapKnight.ToolKit.Properties.Resources.icon_play;
            this.button_anim_play.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_anim_play.Location = new System.Drawing.Point(15, 100);
            this.button_anim_play.Name = "button_anim_play";
            this.button_anim_play.Size = new System.Drawing.Size(50, 50);
            this.button_anim_play.TabIndex = 7;
            this.button_anim_play.UseVisualStyleBackColor = true;
            this.button_anim_play.Click += new System.EventHandler(this.button_anim_play_Click);
            // 
            // textBox_anim_action
            // 
            this.textBox_anim_action.Location = new System.Drawing.Point(119, 62);
            this.textBox_anim_action.Name = "textBox_anim_action";
            this.textBox_anim_action.Size = new System.Drawing.Size(173, 20);
            this.textBox_anim_action.TabIndex = 6;
            this.textBox_anim_action.TextChanged += new System.EventHandler(this.textBox_anim_action_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Action";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Abortable";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Loopable";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(519, 93);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // splitContainer_anim
            // 
            this.splitContainer_anim.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_anim.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_anim.Name = "splitContainer_anim";
            // 
            // splitContainer_anim.Panel1
            // 
            this.splitContainer_anim.Panel1.Controls.Add(this.treeView_anim);
            // 
            // splitContainer_anim.Panel2
            // 
            this.splitContainer_anim.Panel2.SizeChanged += new System.EventHandler(this.splitContainer5_Panel2_SizeChanged);
            this.splitContainer_anim.Size = new System.Drawing.Size(1354, 690);
            this.splitContainer_anim.SplitterDistance = 370;
            this.splitContainer_anim.TabIndex = 0;
            // 
            // treeView_anim
            // 
            this.treeView_anim.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_anim.Location = new System.Drawing.Point(0, 0);
            this.treeView_anim.Name = "treeView_anim";
            this.treeView_anim.Size = new System.Drawing.Size(370, 690);
            this.treeView_anim.TabIndex = 0;
            this.treeView_anim.OnAddNormalButtonClicked += new System.EventHandler<System.Windows.Forms.TreeNode>(this.treeView_anim_OnAddNormalButtonClicked);
            this.treeView_anim.OnAddDefaultButtonClicked += new System.EventHandler<System.Windows.Forms.TreeNode>(this.treeView_anim_OnAddDefaultButtonClicked);
            this.treeView_anim.OnRemoveButtonClicked += new System.EventHandler<System.Windows.Forms.TreeNode>(this.treeView_anim_OnRemoveButtonClicked);
            this.treeView_anim.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_anim_NodeMouseClick);
            // 
            // tbpg_entity
            // 
            this.tbpg_entity.Controls.Add(this.listBox1);
            this.tbpg_entity.Location = new System.Drawing.Point(4, 22);
            this.tbpg_entity.Name = "tbpg_entity";
            this.tbpg_entity.Padding = new System.Windows.Forms.Padding(3);
            this.tbpg_entity.Size = new System.Drawing.Size(1354, 690);
            this.tbpg_entity.TabIndex = 3;
            this.tbpg_entity.Text = "Entity Editor";
            this.tbpg_entity.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 475);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1348, 212);
            this.listBox1.TabIndex = 0;
            // 
            // tbpg_terminal
            // 
            this.tbpg_terminal.Controls.Add(this.groupBox3);
            this.tbpg_terminal.Controls.Add(this.groupBox1);
            this.tbpg_terminal.Controls.Add(this.groupBox2);
            this.tbpg_terminal.Controls.Add(this.toolStrip2);
            this.tbpg_terminal.Location = new System.Drawing.Point(4, 22);
            this.tbpg_terminal.Name = "tbpg_terminal";
            this.tbpg_terminal.Padding = new System.Windows.Forms.Padding(3);
            this.tbpg_terminal.Size = new System.Drawing.Size(1354, 690);
            this.tbpg_terminal.TabIndex = 4;
            this.tbpg_terminal.Text = "Development Terminal";
            this.tbpg_terminal.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listbox_commands);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(427, 28);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(924, 621);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "supported commands";
            // 
            // listbox_commands
            // 
            this.listbox_commands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listbox_commands.Enabled = false;
            this.listbox_commands.FormattingEnabled = true;
            this.listbox_commands.Location = new System.Drawing.Point(3, 16);
            this.listbox_commands.Name = "listbox_commands";
            this.listbox_commands.Size = new System.Drawing.Size(918, 602);
            this.listbox_commands.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.textbox_commandinput);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(427, 649);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(924, 38);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "input";
            // 
            // textbox_commandinput
            // 
            this.textbox_commandinput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textbox_commandinput.Enabled = false;
            this.textbox_commandinput.Location = new System.Drawing.Point(3, 16);
            this.textbox_commandinput.Name = "textbox_commandinput";
            this.textbox_commandinput.Size = new System.Drawing.Size(918, 20);
            this.textbox_commandinput.TabIndex = 1;
            this.textbox_commandinput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_commandinput_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listbox_log);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(3, 28);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(424, 659);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "log";
            // 
            // listbox_log
            // 
            this.listbox_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listbox_log.Enabled = false;
            this.listbox_log.FormattingEnabled = true;
            this.listbox_log.Location = new System.Drawing.Point(3, 16);
            this.listbox_log.Name = "listbox_log";
            this.listbox_log.Size = new System.Drawing.Size(418, 640);
            this.listbox_log.TabIndex = 0;
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textbox_ip,
            this.button_connect,
            this.toolStripSeparator4,
            this.label_connectionstate});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(1348, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // textbox_ip
            // 
            this.textbox_ip.Name = "textbox_ip";
            this.textbox_ip.Size = new System.Drawing.Size(300, 25);
            // 
            // button_connect
            // 
            this.button_connect.Image = global::mapKnight.ToolKit.Properties.Resources.icon_connect;
            this.button_connect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(72, 22);
            this.button_connect.Text = "Connect";
            this.button_connect.ToolTipText = "connect to the ip in the textbox";
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // label_connectionstate
            // 
            this.label_connectionstate.Name = "label_connectionstate";
            this.label_connectionstate.Size = new System.Drawing.Size(125, 22);
            this.label_connectionstate.Text = "Status : not connected";
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 741);
            this.Controls.Add(this.tbctrl_main);
            this.Controls.Add(this.tlstrp_main);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Main";
            this.Text = "mapKnight ToolKit";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.tlstrp_main.ResumeLayout(false);
            this.tlstrp_main.PerformLayout();
            this.tbctrl_main.ResumeLayout(false);
            this.tbpg_mapeditor.ResumeLayout(false);
            this.tlstrp_map.ResumeLayout(false);
            this.tlstrp_map.PerformLayout();
            this.spltcntr_map.Panel1.ResumeLayout(false);
            this.spltcntr_map.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltcntr_map)).EndInit();
            this.spltcntr_map.ResumeLayout(false);
            this.tbpg_charcreation.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.tbpg_animeditor.ResumeLayout(false);
            this.panel_anim_editstep.ResumeLayout(false);
            this.panel_anim_editstep.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_anim_time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_anim_rot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_anim_y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_anim_x)).EndInit();
            this.panel_anim_editanim.ResumeLayout(false);
            this.panel_anim_editanim.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_anim_progress)).EndInit();
            this.splitContainer_anim.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_anim)).EndInit();
            this.splitContainer_anim.ResumeLayout(false);
            this.tbpg_entity.ResumeLayout(false);
            this.tbpg_terminal.ResumeLayout(false);
            this.tbpg_terminal.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.TabPage tbpg_entity;
        private System.Windows.Forms.TabPage tbpg_terminal;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox listbox_commands;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textbox_commandinput;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listbox_log;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton button_connect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel label_connectionstate;
        private System.Windows.Forms.ToolStripTextBox textbox_ip;
        private System.Windows.Forms.Panel panel_anim_editanim;
        private FlickerFreePanel panel_anim_editstep;
        private System.Windows.Forms.SplitContainer splitContainer_anim;
        private mapKnight.ToolKit.AddRemoveTreeView treeView_anim;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button_anim_repeat;
        private System.Windows.Forms.Button button_anim_pause;
        private System.Windows.Forms.Button button_anim_play;
        private System.Windows.Forms.TextBox textBox_anim_action;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button button_anim_mirrored;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.TrackBar trackBar_anim_progress;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDown_anim_rot;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown_anim_y;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown_anim_x;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_anim_loopable;
        private System.Windows.Forms.Button button_anim_abortable;
        private System.Windows.Forms.NumericUpDown numericUpDown_anim_time;
        private System.Windows.Forms.Label label4;
    }
}