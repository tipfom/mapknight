namespace mapKnight.ToolKit {
    partial class Home {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose ();
            }
            base.Dispose (disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent () {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.tabcontrol_main = new mapKnight.ToolKit.BeatifulTabControl();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.menustrip_main = new mapKnight.ToolKit.BeautifulMenuStrip();
            this.fILEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maintoolstrip_load = new System.Windows.Forms.ToolStripMenuItem();
            this.maintoolstrip_save = new System.Windows.Forms.ToolStripMenuItem();
            this.maintoolstrip_saveas = new System.Windows.Forms.ToolStripMenuItem();
            this.maintoolstrip_export = new System.Windows.Forms.ToolStripMenuItem();
            this.iNFOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitcontainer_menustrip = new System.Windows.Forms.SplitContainer();
            this.menustrip_tab = new mapKnight.ToolKit.BeautifulMenuStrip();
            this.openfiledialog_workfile = new System.Windows.Forms.OpenFileDialog();
            this.savefiledialog_workfile = new System.Windows.Forms.SaveFileDialog();
            this.folderdialog = new System.Windows.Forms.FolderBrowserDialog();
            this.menustrip_main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainer_menustrip)).BeginInit();
            this.splitcontainer_menustrip.Panel1.SuspendLayout();
            this.splitcontainer_menustrip.Panel2.SuspendLayout();
            this.splitcontainer_menustrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabcontrol_main
            // 
            this.tabcontrol_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabcontrol_main.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabcontrol_main.Location = new System.Drawing.Point(0, 28);
            this.tabcontrol_main.Name = "tabcontrol_main";
            this.tabcontrol_main.SelectedIndex = 0;
            this.tabcontrol_main.Size = new System.Drawing.Size(633, 462);
            this.tabcontrol_main.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "test");
            // 
            // menustrip_main
            // 
            this.menustrip_main.BackColor = System.Drawing.Color.Gray;
            this.menustrip_main.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.menustrip_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fILEToolStripMenuItem,
            this.iNFOToolStripMenuItem});
            this.menustrip_main.Location = new System.Drawing.Point(0, 0);
            this.menustrip_main.Name = "menustrip_main";
            this.menustrip_main.Size = new System.Drawing.Size(211, 24);
            this.menustrip_main.TabIndex = 1;
            this.menustrip_main.Text = "beautifulMenuStrip1";
            // 
            // fILEToolStripMenuItem
            // 
            this.fILEToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maintoolstrip_load,
            this.maintoolstrip_save,
            this.maintoolstrip_saveas,
            this.maintoolstrip_export});
            this.fILEToolStripMenuItem.Name = "fILEToolStripMenuItem";
            this.fILEToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.fILEToolStripMenuItem.Text = "FILE";
            // 
            // maintoolstrip_load
            // 
            this.maintoolstrip_load.Name = "maintoolstrip_load";
            this.maintoolstrip_load.Size = new System.Drawing.Size(115, 22);
            this.maintoolstrip_load.Text = "LOAD";
            this.maintoolstrip_load.Click += new System.EventHandler(this.maintoolstrip_load_Click);
            // 
            // maintoolstrip_save
            // 
            this.maintoolstrip_save.Name = "maintoolstrip_save";
            this.maintoolstrip_save.Size = new System.Drawing.Size(115, 22);
            this.maintoolstrip_save.Text = "SAVE";
            this.maintoolstrip_save.Click += new System.EventHandler(this.maintoolstrip_save_Click);
            // 
            // maintoolstrip_saveas
            // 
            this.maintoolstrip_saveas.Name = "maintoolstrip_saveas";
            this.maintoolstrip_saveas.Size = new System.Drawing.Size(115, 22);
            this.maintoolstrip_saveas.Text = "SAVE AS";
            this.maintoolstrip_saveas.Click += new System.EventHandler(this.maintoolstrip_saveas_Click);
            // 
            // maintoolstrip_export
            // 
            this.maintoolstrip_export.Name = "maintoolstrip_export";
            this.maintoolstrip_export.Size = new System.Drawing.Size(115, 22);
            this.maintoolstrip_export.Text = "EXPORT";
            this.maintoolstrip_export.Click += new System.EventHandler(this.maintoolstrip_export_Click);
            // 
            // iNFOToolStripMenuItem
            // 
            this.iNFOToolStripMenuItem.Name = "iNFOToolStripMenuItem";
            this.iNFOToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.iNFOToolStripMenuItem.Text = "INFO";
            // 
            // splitcontainer_menustrip
            // 
            this.splitcontainer_menustrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitcontainer_menustrip.IsSplitterFixed = true;
            this.splitcontainer_menustrip.Location = new System.Drawing.Point(0, 0);
            this.splitcontainer_menustrip.Name = "splitcontainer_menustrip";
            // 
            // splitcontainer_menustrip.Panel1
            // 
            this.splitcontainer_menustrip.Panel1.Controls.Add(this.menustrip_main);
            // 
            // splitcontainer_menustrip.Panel2
            // 
            this.splitcontainer_menustrip.Panel2.Controls.Add(this.menustrip_tab);
            this.splitcontainer_menustrip.Size = new System.Drawing.Size(633, 28);
            this.splitcontainer_menustrip.SplitterDistance = 211;
            this.splitcontainer_menustrip.SplitterWidth = 1;
            this.splitcontainer_menustrip.TabIndex = 2;
            // 
            // menustrip_tab
            // 
            this.menustrip_tab.BackColor = System.Drawing.Color.Gray;
            this.menustrip_tab.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.menustrip_tab.Location = new System.Drawing.Point(0, 0);
            this.menustrip_tab.Name = "menustrip_tab";
            this.menustrip_tab.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.menustrip_tab.Size = new System.Drawing.Size(421, 24);
            this.menustrip_tab.TabIndex = 0;
            this.menustrip_tab.Text = "beautifulMenuStrip1";
            // 
            // openfiledialog_workfile
            // 
            this.openfiledialog_workfile.Filter = "TOOLKIT-Files|*.mktk";
            // 
            // savefiledialog_workfile
            // 
            this.savefiledialog_workfile.Filter = "TOOLKIT-Files|*.mktk";
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 490);
            this.Controls.Add(this.tabcontrol_main);
            this.Controls.Add(this.splitcontainer_menustrip);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menustrip_main;
            this.Name = "Home";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "mapKnight ToolKit";
            this.Load += new System.EventHandler(this.Home_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Home_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Home_KeyUp);
            this.menustrip_main.ResumeLayout(false);
            this.menustrip_main.PerformLayout();
            this.splitcontainer_menustrip.Panel1.ResumeLayout(false);
            this.splitcontainer_menustrip.Panel1.PerformLayout();
            this.splitcontainer_menustrip.Panel2.ResumeLayout(false);
            this.splitcontainer_menustrip.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainer_menustrip)).EndInit();
            this.splitcontainer_menustrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BeatifulTabControl tabcontrol_main;
        private BeautifulMenuStrip menustrip_main;
        private System.Windows.Forms.ToolStripMenuItem fILEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maintoolstrip_load;
        private System.Windows.Forms.ToolStripMenuItem maintoolstrip_save;
        private System.Windows.Forms.ToolStripMenuItem maintoolstrip_saveas;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem iNFOToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitcontainer_menustrip;
        private BeautifulMenuStrip menustrip_tab;
        private System.Windows.Forms.ToolStripMenuItem maintoolstrip_export;
        private System.Windows.Forms.OpenFileDialog openfiledialog_workfile;
        private System.Windows.Forms.SaveFileDialog savefiledialog_workfile;
        private System.Windows.Forms.FolderBrowserDialog folderdialog;
    }
}

