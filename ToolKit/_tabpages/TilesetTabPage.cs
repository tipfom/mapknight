using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using mapKnight.Basic;

namespace mapKnight.ToolKit {
    class TilesetTabPage : TabPage {
        private const int TILE_SIZE_IN_LISTVIEW = 30;

        private readonly static string[ ] supportedMasks = { "COLLISION" };

        private Home.SetMenuStripDelegate setMenuStrip;

        private SplitContainer splitContainer;
        private TableLayoutPanel tableLayoutPanel;
        private TextBox attributeTextBox;
        private TextBox nameTextBox;
        private CheckedListBox maskListBox;
        private Label[ ] infoLabel;
        private OpenFileDialog openTilesetDialog;
        private BeautifulDragAndDropListView listViewContainer;
        private List<ToolStripItem> menuItems;

        private List<Tileset> tilesets = new List<Tileset> ( );
        private int _selectedTileset_ = -1;
        private int selectedTile = -1;
        private int selectedTileset {
            get { return _selectedTileset_; }
            set {
                _selectedTileset_ = value;
                updateListView ( );
                ((ToolStripComboBox)menuItems[1]).SelectedIndex = value;
            }
        }

        public TilesetTabPage (Home.SetMenuStripDelegate smsdelegate) : base ( ) {
            InitializeComponent ( );

            this.Name = "tabpage_tileset";
            this.Text = " TILESET ";

            setMenuStrip = smsdelegate;
        }

        protected override void OnParentChanged (EventArgs e) {
            if (this.Parent != null)
                ((TabControl)this.Parent).SelectedIndexChanged += TileTabPage_SelectedIndexChanged;
        }

        private void TileTabPage_SelectedIndexChanged (object sender, EventArgs e) {
            if (((TabControl)this.Parent).SelectedTab == this) {
                setMenuStrip (this.menuItems);
            }
        }

        private void InitializeComponent () {
            splitContainer = new SplitContainer ( );
            tableLayoutPanel = new TableLayoutPanel ( );
            attributeTextBox = new TextBox ( );
            nameTextBox = new TextBox ( );
            maskListBox = new CheckedListBox ( );
            infoLabel = new Label[ ] { new Label ( ), new Label ( ), new Label ( ) };
            openTilesetDialog = new OpenFileDialog ( );
            listViewContainer = new BeautifulDragAndDropListView ( );
            menuItems = new List<ToolStripItem> ( );

            this.openTilesetDialog.Filter = "TILESET-Files|*.tileset";

            this.listViewContainer.Dock = DockStyle.Fill;
            this.listViewContainer.LargeImageList = new ImageList ( );
            this.listViewContainer.LargeImageList.ImageSize = new System.Drawing.Size (TILE_SIZE_IN_LISTVIEW, TILE_SIZE_IN_LISTVIEW);
            this.listViewContainer.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            this.listViewContainer.BitmapDroped += ListViewContainer_BitmapDroped;
            this.listViewContainer.SelectedIndexChanged += ListViewContainer_SelectedIndexChanged;

            this.infoLabel[0].Text = "Name";
            this.infoLabel[0].ForeColor = Properties.Settings.Default.ListViewForeColor;
            this.infoLabel[0].Font = Properties.Settings.Default.ListViewFont;

            this.nameTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            this.infoLabel[1].Text = "Attributes (name value)";
            this.infoLabel[1].Padding = new Padding (0, 4, 0, 0);
            this.infoLabel[1].ForeColor = Properties.Settings.Default.ListViewForeColor;
            this.infoLabel[1].Font = Properties.Settings.Default.ListViewFont;

            this.attributeTextBox.Multiline = true;
            this.attributeTextBox.Dock = DockStyle.Fill;

            this.infoLabel[2].Text = "Mask";
            this.infoLabel[2].Dock = DockStyle.Top;
            this.infoLabel[2].ForeColor = Properties.Settings.Default.ListViewForeColor;
            this.infoLabel[2].Font = Properties.Settings.Default.ListViewFont;

            this.maskListBox.Dock = DockStyle.Fill;
            this.maskListBox.IntegralHeight = false;
            this.maskListBox.Items.AddRange (supportedMasks);

            this.tableLayoutPanel.BackColor = Properties.Settings.Default.ListViewBackColor;
            this.tableLayoutPanel.Dock = DockStyle.Fill;
            this.tableLayoutPanel.Padding = new Padding (0, 3, 0, 3);
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add (new RowStyle (SizeType.Absolute, this.infoLabel[0].PreferredHeight));
            this.tableLayoutPanel.RowStyles.Add (new RowStyle (SizeType.Absolute, this.nameTextBox.PreferredHeight));
            this.tableLayoutPanel.RowStyles.Add (new RowStyle (SizeType.Absolute, this.infoLabel[1].PreferredHeight));
            this.tableLayoutPanel.RowStyles.Add (new RowStyle (SizeType.Percent, 2));
            this.tableLayoutPanel.RowStyles.Add (new RowStyle (SizeType.Absolute, this.infoLabel[2].PreferredHeight));
            this.tableLayoutPanel.RowStyles.Add (new RowStyle (SizeType.Percent, 3));
            this.tableLayoutPanel.Controls.Add (this.infoLabel[0], 0, 0);
            this.tableLayoutPanel.Controls.Add (this.nameTextBox, 0, 1);
            this.tableLayoutPanel.Controls.Add (this.infoLabel[1], 0, 2);
            this.tableLayoutPanel.Controls.Add (this.attributeTextBox, 0, 3);
            this.tableLayoutPanel.Controls.Add (this.infoLabel[2], 0, 4);
            this.tableLayoutPanel.Controls.Add (this.maskListBox, 0, 5);

            this.splitContainer.BackColor = Properties.Settings.Default.SplitterColor;
            this.splitContainer.Dock = DockStyle.Fill;
            this.splitContainer.SplitterDistance = (int)(this.splitContainer.Width * 0.7f);
            this.splitContainer.Panel1.Controls.Add (this.listViewContainer);
            this.splitContainer.Panel2.Controls.Add (this.tableLayoutPanel);
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.Purple;

            this.Controls.Add (this.splitContainer);

            this.menuItems.Add (new ToolStripMenuItem ("TILESET"));
            ((ToolStripMenuItem)this.menuItems[0]).DropDownItems.Add ("NEW");
            ((ToolStripMenuItem)this.menuItems[0]).DropDownItems[0].Click += menuItems_NEW_clicked;
            ((ToolStripMenuItem)this.menuItems[0]).DropDownItems.Add ("LOAD");
            ((ToolStripMenuItem)this.menuItems[0]).DropDownItems[1].Click += menuItems_LOAD_clicked;
            this.menuItems.Add (new ToolStripComboBox ("combobox_tileset") { Width = 300 });
            ((ToolStripComboBox)menuItems[1]).SelectedIndexChanged += menuItems_SELECT_changed;
        }

        private void ListViewContainer_SelectedIndexChanged (object sender, EventArgs e) {
            if (this.listViewContainer.SelectedIndices.Count > 0) {
                updateTile ( );

                this.selectedTile = this.listViewContainer.SelectedIndices[0];

                this.nameTextBox.Text = this.tilesets[selectedTileset].Tiles[selectedTile].Name;
                this.attributeTextBox.Lines = this.tilesets[selectedTileset].Tiles[selectedTile].Attributes.Select (str => str.Key.ToString ( ) + " " + str.Value.ToString ( )).ToArray ( );
                for (int i = 0; i < this.maskListBox.Items.Count; i++) {
                    this.maskListBox.SetItemChecked (i, false);
                    // uncheck everything
                }
                foreach (string entry in this.tilesets[selectedTileset].Tiles[selectedTile].MaskFlag) {
                    if (this.maskListBox.Items.Contains (entry))
                        this.maskListBox.SetItemChecked (this.maskListBox.Items.IndexOf (entry), true);
                }
            }
        }

        private void menuItems_SELECT_changed (object sender, EventArgs e) {
            if (this.selectedTile > -1)
                updateTile ( );
            this.selectedTileset = ((ToolStripComboBox)menuItems[1]).SelectedIndex;
        }

        private void menuItems_NEW_clicked (object sender, EventArgs e) {
            NameForm namedialog = new NameForm ("tileset");
            if (namedialog.ShowDialog ( ) == DialogResult.OK) {
                updateTile ( );

                this.tilesets.Add (new Tileset (namedialog.Result));
                updateMenuItems ( );
                this.selectedTileset = this.tilesets.Count - 1;
            }
        }

        private void menuItems_LOAD_clicked (object sender, EventArgs e) {
            if (this.openTilesetDialog.ShowDialog ( ) == DialogResult.OK) {
                updateTile ( );

                this.tilesets.Add (new Tileset (XMLElemental.Load (File.OpenRead (this.openTilesetDialog.FileName))));
                updateMenuItems ( );
                this.selectedTileset = this.tilesets.Count - 1;
            }
        }

        private void updateMenuItems () {
            ((ToolStripComboBox)menuItems[1]).Items.Clear ( );
            ((ToolStripComboBox)menuItems[1]).Items.AddRange (this.tilesets.ToArray ( ));
            ((ToolStripComboBox)menuItems[1]).Control.Refresh ( );
        }

        private void updateListView () {
            this.listViewContainer.LargeImageList.Images.Clear ( );
            this.listViewContainer.Items.Clear ( );

            foreach (Tileset.Tile tile in this.tilesets[selectedTileset].Tiles) {
                this.listViewContainer.LargeImageList.Images.Add (tile.Texture);
                this.listViewContainer.Items.Add (new ListViewItem (tile.Name, this.tilesets[selectedTileset].Tiles.IndexOf (tile)));
            }
        }

        private void updateTile () {
            if (this.selectedTileset != -1 && this.tilesets.Count > 0 && this.tilesets[selectedTileset].Tiles.Count > 0 && this.selectedTile > -1) {
                this.tilesets[selectedTileset].Tiles[selectedTile].Name = this.nameTextBox.Text;

                this.tilesets[selectedTileset].Tiles[selectedTile].MaskFlag = new string[this.maskListBox.CheckedItems.Count];
                for (int i = 0; i < this.maskListBox.CheckedItems.Count; i++) {
                    this.tilesets[selectedTileset].Tiles[selectedTile].MaskFlag[i] = this.maskListBox.CheckedItems[i].ToString ( );
                }
                this.tilesets[selectedTileset].Tiles[selectedTile].Attributes.Clear ( );

                foreach (string line in this.attributeTextBox.Lines) {
                    string[ ] arguments = line.Split (new char[ ] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arguments.Length == 2 && !this.tilesets[selectedTileset].Tiles[selectedTile].Attributes.ContainsKey (arguments[0])) {
                        this.tilesets[selectedTileset].Tiles[selectedTile].Attributes.Add (arguments[0], arguments[1]);
                    }
                }
            } else {
                this.nameTextBox.Text = "";
                this.attributeTextBox.Text = "";
            }
        }

        private void ListViewContainer_BitmapDroped (object sender, System.Drawing.Bitmap e) {
            updateTile ( );

            AddTileForm addtiledialog = new AddTileForm (e);
            if (addtiledialog.ShowDialog ( ) == DialogResult.OK) {
                this.tilesets[selectedTileset].Tiles.Add (new Tileset.Tile (addtiledialog.textbox_name.Text, e));
            }

            updateListView ( );
            updateMenuItems ( );
            this.listViewContainer.SelectedIndices.Add (this.tilesets[selectedTileset].Tiles.Count - 1);
        }

        public Tileset GetTileset (string name) {
            return tilesets.Find ((Tileset tileset) => tileset.Name == name);
        }

        public Tileset[ ] GetAllTilesets () {
            return tilesets.ToArray ( );
        }

        public string CreateTileSet (string name) {
            this.tilesets.Add (new Tileset (name));
            updateMenuItems ( );
            return this.tilesets[this.tilesets.Count - 1].Name;
        }

        public mapKnight.Basic.XMLElemental Save () {
            updateTile ( );

            Basic.XMLElemental container = new Basic.XMLElemental ("tileset");
            foreach (Tileset tileset in this.tilesets) {
                container.AddChild (tileset.Save ( ));
            }
            return container;
        }

        public void Export (string homepath) {
            updateTile ( );

            foreach (Tileset tileset in this.tilesets) {
                tileset.Export (Path.ChangeExtension (Path.Combine (homepath, tileset.Name), "tileset"));
            }
        }

        public void Load (XMLElemental config) {
            this.tilesets.Clear ( );
            foreach (XMLElemental tileset in config.GetAll ( )) {
                this.tilesets.Add (new Tileset (tileset));
            }
            updateMenuItems ( );
        }
    }
}
