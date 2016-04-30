using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace mapKnight.ToolKit {
    public partial class Home : Form {
        public delegate void SetMenuStripDelegate (List<ToolStripItem> menustrip);
        public delegate string CreateTileSetDelegate (string name);
        public delegate Tileset GetTileSetDelegate (string name);
        public delegate Tileset[ ] GetAllTileSetDelegate ();

        private string lastSavePath;

        public Home () {
            InitializeComponent ( );
        }

        private void Home_Load (object sender, EventArgs e) {
            splitcontainer_menustrip.BackColor = Properties.Settings.Default.BackColor;
            splitcontainer_menustrip.SplitterDistance = menustrip_main.PreferredSize.Width;
            splitcontainer_menustrip.Height = menustrip_main.PreferredSize.Height;
            splitcontainer_menustrip.IsSplitterFixed = true;
            splitcontainer_menustrip.FixedPanel = FixedPanel.Panel1;

            initTabControl ( );
        }

        private void initTabControl () {
            tabcontrol_main.TabPages.Add (new MapTabPage (SetMenuStrip, GetTileSet, GetTileSets, CreateTileSet));
            tabcontrol_main.TabPages.Add (new TilesetTabPage (SetMenuStrip));
            tabcontrol_main.SelectedIndex = -1;
            tabcontrol_main.SelectedIndex = 0;
        }

        public void SetMenuStrip (List<ToolStripItem> items) {
            menustrip_tab.Items.Clear ( );
            foreach (ToolStripItem item in items) {
                menustrip_tab.Items.Add (item);
            }
        }

        public Tileset[ ] GetTileSets () {
            return ((TilesetTabPage)tabcontrol_main.TabPages[1]).GetAllTilesets ( );
        }

        public Tileset GetTileSet (string name) {
            return ((TilesetTabPage)tabcontrol_main.TabPages[1]).GetTileset (name);
        }

        private string CreateTileSet (string name) {
            return ((TilesetTabPage)tabcontrol_main.TabPages[1]).CreateTileSet (name);
        }

        private void maintoolstrip_export_Click (object sender, EventArgs e) {
            if (folderdialog.ShowDialog ( ) == DialogResult.OK) {
                string exportpath = folderdialog.SelectedPath;
                string mappath = Path.Combine (exportpath, "maps");
                string tilesetpath = Path.Combine (exportpath, "tilesets");

                if (!Directory.Exists (mappath))
                    Directory.CreateDirectory (mappath);
                if (!Directory.Exists (tilesetpath))
                    Directory.CreateDirectory (tilesetpath);

                ((MapTabPage)tabcontrol_main.TabPages[0]).Export (mappath);
                ((TilesetTabPage)tabcontrol_main.TabPages[1]).Export (tilesetpath);
            }
        }

        private void maintoolstrip_saveas_Click (object sender, EventArgs e) {
            if (savefiledialog_workfile.ShowDialog ( ) == DialogResult.OK) {
                lastSavePath = savefiledialog_workfile.FileName;
                save ( );
            }
        }

        private void maintoolstrip_save_Click (object sender, EventArgs e) {
            if (lastSavePath != null) {
                save ( );
            } else if (savefiledialog_workfile.ShowDialog ( ) == DialogResult.OK) {
                lastSavePath = savefiledialog_workfile.FileName;
                save ( );
            }
        }

        private void maintoolstrip_load_Click (object sender, EventArgs e) {
            if (openfiledialog_workfile.ShowDialog ( ) == DialogResult.OK) {
                lastSavePath = openfiledialog_workfile.FileName;
                load ( );
            }
        }

        private void save () {
            Basic.XMLElemental parsed = new Basic.XMLElemental ("workfile");

            parsed.AddChild (((MapTabPage)tabcontrol_main.TabPages[0]).Save ( ));
            parsed.AddChild (((TilesetTabPage)tabcontrol_main.TabPages[1]).Save ( ));

            using (FileStream stream = File.Open (lastSavePath, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter (stream)) {
                    writer.WriteLine (parsed.Flush ( ));
                }
            }
        }

        private void load () {
            Basic.XMLElemental loaded = Basic.XMLElemental.Load (File.OpenRead (lastSavePath));

            ((MapTabPage)tabcontrol_main.TabPages[0]).Load (loaded["map"]);
            ((TilesetTabPage)tabcontrol_main.TabPages[1]).Load (loaded["tileset"]);
        }

        private void Home_KeyDown (object sender, KeyEventArgs e) {
            if (e.KeyData == (Keys.Control | Keys.S)) {
                maintoolstrip_save_Click (this, EventArgs.Empty);
            }
            if (e.KeyData == (Keys.Control | Keys.L)) {
                maintoolstrip_load_Click (this, EventArgs.Empty);
            }
            if (e.KeyData == (Keys.Control | Keys.E)) {
                maintoolstrip_export_Click (this, EventArgs.Empty);
            }

            if (this.tabcontrol_main.SelectedTab.GetType ( ) == typeof (MapTabPage)) {
                ((MapTabPage)this.tabcontrol_main.SelectedTab).HandleKeyDown (e);
            }
        }

        private void Home_KeyUp (object sender, KeyEventArgs e) {
            if (this.tabcontrol_main.SelectedTab.GetType ( ) == typeof (MapTabPage)) {
                ((MapTabPage)this.tabcontrol_main.SelectedTab).HandleKeyUp (e);
            }
        }
    }
}
