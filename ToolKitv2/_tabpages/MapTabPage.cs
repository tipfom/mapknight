using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace mapKnight.ToolKit {
    class MapTabPage : TabPage {
        private enum Tool {
            Brush,
            Bucket,
            Eraser,
            Select,
            SpawnPointer
        }

        private const int MAX_ZOOM_LEVEL = 20;
        private const int MIN_TILE_SIZE_DRAW = 20;
        private const int MAX_TILE_SIZE_DRAW = 100;
        private const int TILE_SIZE_IN_LISTVIEW = 30;

        private Home.SetMenuStripDelegate setMenuStrip;
        private Home.GetTileSetDelegate getTileSet;
        private Home.GetAllTileSetDelegate getAllTileSets;
        private Home.CreateTileSetDelegate createTileSet;

        private SplitContainer splitContainer;
        private BeautifulDragAndDropListView tileListView;
        private HScrollBar xScrollBar;
        private VScrollBar yScrollBar;
        private OpenFileDialog openMapFileDialog;
        private CheckBox gridCheckBox;

        private List<ToolStripItem> menuItems;
        private List<Map> maps;
        private Bitmap[] tileImages;

        private int currentZoomLevel;
        private int selectedMap = -1;
        private int selectedLayer = -1;
        private int[] selectedTile = new int[3];
        private Tool selectedTool;
        private int tileSize = Tileset.Tile.TILE_SIZE;
        private bool clicked;
        private bool altPressed;
        private Size renderSize;

        public MapTabPage (Home.SetMenuStripDelegate smsdelegate, Home.GetTileSetDelegate gtsdelegate, Home.GetAllTileSetDelegate gatsdelegate, Home.CreateTileSetDelegate ctsdelegate) : base () {
            InitializeComponent ();

            this.Name = "tabpage_map";
            this.Text = " MAP ";
            this.maps = new List<Map> ();

            setMenuStrip = smsdelegate;
            getTileSet = gtsdelegate;
            getAllTileSets = gatsdelegate;
            createTileSet = ctsdelegate;
        }

        protected override void OnParentChanged (EventArgs e) {
            if (this.Parent != null)
                ((TabControl)this.Parent).SelectedIndexChanged += MapTabPage_SelectedIndexChanged;
        }

        private void MapTabPage_SelectedIndexChanged (object sender, EventArgs e) {
            if (((TabControl)this.Parent).SelectedTab == this) {
                setMenuStrip (this.menuItems);

                if (this.maps.Count > 0 && this.selectedMap != -1)
                    updateListView ();
            }
        }

        protected override void OnSizeChanged (EventArgs e) {
            base.OnSizeChanged (e);
            renderSize = new Size ((int)((this.splitContainer.Panel2.Width - this.yScrollBar.Width) / tileSize) + 1, (int)((this.splitContainer.Panel2.Height - this.xScrollBar.Height) / tileSize) + 1);
            updateScrollBar ();
        }

        private void InitializeComponent () {
            menuItems = new List<ToolStripItem> ();
            splitContainer = new SplitContainer ();
            tileListView = new BeautifulDragAndDropListView ();
            xScrollBar = new HScrollBar ();
            yScrollBar = new VScrollBar ();
            openMapFileDialog = new OpenFileDialog ();
            gridCheckBox = new CheckBox ();

            this.openMapFileDialog.Filter = "TMSL4-Files|*.tmsl4";

            this.splitContainer.Dock = DockStyle.Fill;
            this.splitContainer.Panel2.Paint += (object sender, PaintEventArgs e) => { drawMap (); };
            this.splitContainer.Panel2.MouseWheel += Panel2_MouseWheel;
            this.splitContainer.Panel2.MouseDown += Panel2_MouseDown;
            this.splitContainer.Panel2.MouseMove += Panel2_MouseMove;
            this.splitContainer.Panel2.MouseUp += Panel2_MouseUp;
            this.splitContainer.Panel2.MouseLeave += Panel2_MouseLeave;
            this.Controls.Add (this.splitContainer);

            this.tileListView.Dock = DockStyle.Fill;
            this.tileListView.LargeImageList = new ImageList () { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size (TILE_SIZE_IN_LISTVIEW, TILE_SIZE_IN_LISTVIEW) };
            this.tileListView.SelectedIndexChanged += TileListView_SelectedIndexChanged;
            this.tileListView.BitmapDroped += TileListView_BitmapDroped;
            this.splitContainer.Panel1.Controls.Add (this.tileListView);

            this.xScrollBar.Dock = DockStyle.Bottom;
            this.xScrollBar.Maximum = 0;
            this.xScrollBar.Value = 0;
            this.xScrollBar.ValueChanged += (object sender, EventArgs e) => { drawMap (); };
            this.splitContainer.Panel2.Controls.Add (this.xScrollBar);

            this.yScrollBar.Dock = DockStyle.Right;
            this.yScrollBar.Maximum = 0;
            this.yScrollBar.Value = 0;
            this.yScrollBar.ValueChanged += (object sender, EventArgs e) => { drawMap (); };
            this.splitContainer.Panel2.Controls.Add (this.yScrollBar);

            this.gridCheckBox.Text = "Show Grid";
            this.gridCheckBox.Padding = new Padding (10, 0, 10, 0);
            this.gridCheckBox.ForeColor = Properties.Settings.Default.MenuStripInActiveForeColor;
            this.gridCheckBox.BackColor = Properties.Settings.Default.BackColor;
            this.gridCheckBox.MouseEnter += (object sender, EventArgs e) => {
                this.gridCheckBox.ForeColor = Properties.Settings.Default.MenuStripActiveForeColor;
                this.gridCheckBox.BackColor = Properties.Settings.Default.MenuStripActiveBackColor;
            };
            this.gridCheckBox.MouseLeave += (object sender, EventArgs e) => {
                this.gridCheckBox.ForeColor = Properties.Settings.Default.MenuStripInActiveForeColor;
                this.gridCheckBox.BackColor = Properties.Settings.Default.MenuStripInActiveBackColor;
            };
            this.gridCheckBox.CheckedChanged += (object sender, EventArgs e) => { drawMap (); };

            this.menuItems.Add (new ToolStripMenuItem ("MAP"));
            ((ToolStripMenuItem)this.menuItems[0]).DropDownItems.Add ("NEW");
            ((ToolStripMenuItem)this.menuItems[0]).DropDownItems[0].Click += newMap_Click;
            ((ToolStripMenuItem)this.menuItems[0]).DropDownItems.Add ("LOAD");
            ((ToolStripMenuItem)this.menuItems[0]).DropDownItems[1].Click += loadMap_Click;
            this.menuItems.Add (new ToolStripMenuItem ("UNDO"));
            ((ToolStripMenuItem)this.menuItems[1]).Click += (object sender, EventArgs e) => { if (selectedMap != -1) { maps[selectedMap].Undo (); drawMap (); } };
            this.menuItems.Add (new ToolStripComboBox ("map_select") { Enabled = false, Width = 200 });
            ((ToolStripComboBox)this.menuItems[2]).SelectedIndexChanged += map_select_SelectedIndexChanged;

            this.menuItems.Add (new ToolStripMenuItem ("L1") { DisplayStyle = ToolStripItemDisplayStyle.ImageAndText });
            ((ToolStripMenuItem)this.menuItems[3]).Click += brush_layer1_clicked;
            this.menuItems.Add (new ToolStripMenuItem ("L2") { DisplayStyle = ToolStripItemDisplayStyle.ImageAndText });
            ((ToolStripMenuItem)this.menuItems[4]).Click += brush_layer2_clicked;
            this.menuItems.Add (new ToolStripMenuItem ("L3") { DisplayStyle = ToolStripItemDisplayStyle.ImageAndText });
            ((ToolStripMenuItem)this.menuItems[5]).Click += brush_layer3_clicked;
            this.menuItems.Add (new ToolStripMenuItem ("ERASE") { DisplayStyle = ToolStripItemDisplayStyle.Image });
            ((ToolStripMenuItem)this.menuItems[6]).Click += erase_clicked;
            this.menuItems.Add (new ToolStripMenuItem ("FILL") { DisplayStyle = ToolStripItemDisplayStyle.Image });
            ((ToolStripMenuItem)this.menuItems[7]).Click += fill_clicked;
            this.menuItems.Add (new ToolStripMenuItem ("SELECT") { DisplayStyle = ToolStripItemDisplayStyle.Image });
            ((ToolStripMenuItem)this.menuItems[8]).Click += select_clicked;
            this.menuItems.Add (new ToolStripMenuItem ("SPAWN") { DisplayStyle = ToolStripItemDisplayStyle.Image });
            ((ToolStripMenuItem)this.menuItems[9]).Click += spawn_clicked;
            this.menuItems.Add (new ToolStripControlHost (gridCheckBox));
        }

        private void TileListView_BitmapDroped (object sender, Bitmap e) {
            // add tile to the maps tileset
            AddTileForm addtiledialog = new AddTileForm (e);
            if (addtiledialog.ShowDialog () == DialogResult.OK) {
                getTileSet (maps[selectedMap].TileSet).Tiles.Add (new Tileset.Tile (addtiledialog.textbox_name.Text, e));
            }
            updateListView ();
        }

        private void erase_clicked (object sender, EventArgs e) {
            selectedTool = Tool.Eraser;
            updateToolInterface ();
        }

        private void spawn_clicked (object sender, EventArgs e) {
            selectedTool = Tool.SpawnPointer;
            updateToolInterface ();
        }

        private void select_clicked (object sender, EventArgs e) {
            selectedTool = Tool.Select;
            updateToolInterface ();
        }

        private void fill_clicked (object sender, EventArgs e) {
            selectedTool = Tool.Bucket;
            updateToolInterface ();
        }

        private void brush_layer3_clicked (object sender, EventArgs e) {
            updateBrushTool (2);
        }

        private void brush_layer2_clicked (object sender, EventArgs e) {
            updateBrushTool (1);
        }

        private void brush_layer1_clicked (object sender, EventArgs e) {
            updateBrushTool (0);
        }

        private void updateBrushTool (int newSelectedLayer) {
            selectedTool = Tool.Brush;
            selectedLayer = newSelectedLayer;
            if (altPressed) {
                if (this.tileListView.SelectedItems.Count > 0) {
                    selectedTile[selectedLayer] = this.tileListView.SelectedIndices[0];
                } else {
                    selectedTile[selectedLayer] = 0;
                    this.tileListView.SelectedIndices.Clear ();
                    if (selectedTile[selectedLayer] != -1)
                        this.tileListView.SelectedIndices.Add (selectedTile[selectedLayer]);
                }
            } else {
                this.tileListView.SelectedIndices.Clear ();
                if (selectedTile[selectedLayer] != -1)
                    this.tileListView.SelectedIndices.Add (selectedTile[selectedLayer]);
            }
            updateToolInterface ();
        }

        public void HandleKeyUp (KeyEventArgs e) {
            if (e.KeyCode == Keys.Menu) {
                altPressed = false;
            }
        }

        public void HandleKeyDown (KeyEventArgs e) {
            if (e.KeyCode == Keys.Menu) {
                altPressed = true;
            }

            if (e.KeyData == (Keys.Control | Keys.Z)) {
                maps[selectedMap].Undo ();
                drawMap ();
            }
        }

        private void updateToolInterface () {
            ((ToolStripMenuItem)this.menuItems[3]).Checked = false;
            ((ToolStripMenuItem)this.menuItems[4]).Checked = false;
            ((ToolStripMenuItem)this.menuItems[5]).Checked = false;
            ((ToolStripMenuItem)this.menuItems[6]).Checked = false;
            ((ToolStripMenuItem)this.menuItems[7]).Checked = false;
            ((ToolStripMenuItem)this.menuItems[8]).Checked = false;
            ((ToolStripMenuItem)this.menuItems[9]).Checked = false;
            switch (selectedTool) {
            case Tool.Brush:
                switch (selectedLayer) {
                case 0:
                    ((ToolStripMenuItem)this.menuItems[3]).Checked = true;
                    break;
                case 1:
                    ((ToolStripMenuItem)this.menuItems[4]).Checked = true;
                    break;
                case 2:
                    ((ToolStripMenuItem)this.menuItems[5]).Checked = true;
                    break;
                }
                break;
            case Tool.Bucket:
                ((ToolStripMenuItem)this.menuItems[7]).Checked = true;
                break;
            case Tool.Eraser:
                ((ToolStripMenuItem)this.menuItems[6]).Checked = true;
                break;
            case Tool.Select:
                ((ToolStripMenuItem)this.menuItems[8]).Checked = true;
                break;
            case Tool.SpawnPointer:
                ((ToolStripMenuItem)this.menuItems[9]).Checked = true;
                break;
            }
        }

        private void TileListView_SelectedIndexChanged (object sender, EventArgs e) {
            if (tileListView.SelectedItems.Count > 0 && selectedLayer > -1) {
                selectedTile[selectedLayer] = tileListView.SelectedIndices[0];
            }
        }

        private void Panel2_MouseWheel (object sender, MouseEventArgs e) {
            if (e.Delta > 0) {
                // zoom in
                currentZoomLevel = Math.Min (currentZoomLevel + 1, MAX_ZOOM_LEVEL);
                tileSize = (int)(MIN_TILE_SIZE_DRAW + (MAX_TILE_SIZE_DRAW - MIN_TILE_SIZE_DRAW) * (float)(currentZoomLevel) / MAX_ZOOM_LEVEL);
                renderSize = new Size ((int)((this.splitContainer.Panel2.Width - this.yScrollBar.Width) / tileSize) + 1, (int)((this.splitContainer.Panel2.Height - this.xScrollBar.Height) / tileSize) + 1);
                updateScrollBar ();
                drawMap ();
            } else {
                // zoom out
                currentZoomLevel = Math.Max (currentZoomLevel - 1, 0);
                tileSize = (int)(MIN_TILE_SIZE_DRAW + (MAX_TILE_SIZE_DRAW - MIN_TILE_SIZE_DRAW) * (float)(currentZoomLevel) / MAX_ZOOM_LEVEL);
                renderSize = new Size ((int)((this.splitContainer.Panel2.Width - this.yScrollBar.Width) / tileSize) + 1, (int)((this.splitContainer.Panel2.Height - this.xScrollBar.Height) / tileSize) + 1);
                updateScrollBar ();
                drawMap ();
            }
        }

        private void Panel2_MouseDown (object sender, MouseEventArgs e) {
            if (selectedMap > -1) {
                Point clickedTile = new Point ((int)e.X / tileSize + xScrollBar.Value, (int)e.Y / tileSize + yScrollBar.Value);

                if (e.Button == MouseButtons.Left && clickedTile.X >= 0 && clickedTile.Y >= 0 && clickedTile.X < maps[selectedMap].Width && clickedTile.Y < maps[selectedMap].Height) {
                    maps[selectedMap].PrepareUndo ();

                    switch (selectedTool) {
                    case Tool.Brush:
                        if (selectedLayer > -1)
                            maps[selectedMap].Data[clickedTile.X, clickedTile.Y, selectedLayer] = selectedTile[selectedLayer];
                        clicked = true;
                        break;
                    case Tool.Eraser:
                        maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 0] = 0;
                        maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 1] = 0;
                        maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 2] = 0;
                        clicked = true;
                        break;
                    case Tool.Bucket:
                        maps[selectedMap].Fill (clickedTile.X, clickedTile.Y, selectedTile);
                        break;
                    case Tool.SpawnPointer:
                        maps[selectedMap].Spawn = new Point (clickedTile.X, maps[selectedMap].Height - clickedTile.Y);
                        break;
                    case Tool.Select:
                        selectedLayer = 0;
                        tileListView.Items[(int)maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 0]].Selected = true;
                        tileListView.Items[(int)maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 0]].Focused = true;
                        selectedLayer = 1;
                        tileListView.Items[(int)maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 1]].Selected = true;
                        tileListView.Items[(int)maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 1]].Focused = true;
                        selectedLayer = 2;
                        tileListView.Items[(int)maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 2]].Selected = true;
                        tileListView.Items[(int)maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 2]].Focused = true;

                        break;
                    }
                } else if (e.Button == MouseButtons.Right) {
                    maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 0] = 0;
                    maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 1] = 0;
                    maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 2] = 0;
                    clicked = true;
                }

                drawMap ();
            }
        }

        private void Panel2_MouseMove (object sender, MouseEventArgs e) {
            if (clicked) {
                Point clickedTile = new Point ((int)e.X / tileSize + xScrollBar.Value, (int)e.Y / tileSize + yScrollBar.Value);

                if (e.Button == MouseButtons.Left && clickedTile.X < maps[selectedMap].Width && clickedTile.Y < maps[selectedMap].Height && clickedTile.X >= 0 && clickedTile.Y >= 0) {
                    switch (selectedTool) {
                    case Tool.Brush:
                        if (selectedLayer > -1)
                            maps[selectedMap].Data[clickedTile.X, clickedTile.Y, selectedLayer] = selectedTile[selectedLayer];
                        break;
                    case Tool.Eraser:
                        maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 0] = 0;
                        maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 1] = 0;
                        maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 2] = 0;
                        break;
                    }
                } else if (e.Button == MouseButtons.Right) {
                    maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 0] = 0;
                    maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 1] = 0;
                    maps[selectedMap].Data[clickedTile.X, clickedTile.Y, 2] = 0;
                }

                drawMap ();
            }
        }

        private void Panel2_MouseUp (object sender, MouseEventArgs e) {
            clicked = false;
        }

        private void Panel2_MouseLeave (object sender, EventArgs e) {
            clicked = false;
        }

        private void loadMap_Click (object sender, EventArgs e) {
            if (openMapFileDialog.ShowDialog () == DialogResult.OK) {
                maps.Add (new Map (File.ReadAllBytes (openMapFileDialog.FileName)));

                updateMenuItems ();
                selectLastMap ();
            }
        }

        private void selectLastMap () {
            ((ToolStripComboBox)this.menuItems[2]).SelectedIndex = ((ToolStripComboBox)this.menuItems[2]).Items.Count - 1;
        }

        private void newMap_Click (object sender, EventArgs e) {
            Tileset[] tilesets = getAllTileSets ();
            NewMapForm dialogform = new NewMapForm (getAllTileSets ());
            if (dialogform.ShowDialog () == DialogResult.OK) {
                if (dialogform.combobox_tileset.SelectedIndex == 0) {
                    // add new tileset selected
                    /**/
                    NameForm namedialog = new NameForm ("tileset");
                    if (namedialog.ShowDialog () == DialogResult.OK) {
                        maps.Add (new Map ((int)dialogform.numericupdown_width.Value,
                            (int)dialogform.numericupdown_height.Value,
                            dialogform.textbox_creator.Text,
                            dialogform.textbox_name.Text,
                            createTileSet (namedialog.Result),
                            new Point (0, 0)));
                    }
                } else {
                    maps.Add (new Map ((int)dialogform.numericupdown_width.Value,
                        (int)dialogform.numericupdown_height.Value,
                        dialogform.textbox_creator.Text,
                        dialogform.textbox_name.Text,
                        tilesets[dialogform.combobox_tileset.SelectedIndex - 1].RefID,
                        new Point (0, 0)));
                }

                updateMenuItems ();
                selectLastMap ();
            }
        }

        private void map_select_SelectedIndexChanged (object sender, EventArgs e) {
            selectedMap = ((ToolStripComboBox)this.menuItems[2]).SelectedIndex;
            updateListView ();
            updateScrollBar ();
            this.Refresh ();
        }

        private void updateListView () {
            tileListView.Items.Clear ();
            tileListView.LargeImageList.Images.Clear ();
            Tileset tileset = getTileSet (maps[selectedMap].TileSet);
            for (int i = 0; i < tileset.Tiles.Count; i++) {
                tileListView.LargeImageList.Images.Add (tileset.Tiles[i].Texture);
                tileListView.Items.Add (new ListViewItem (tileset.Tiles[i].Name, i));
            }
            tileImages = tileListView.LargeImageList.Images.Cast<Bitmap> ().ToArray ();
        }

        private void updateScrollBar () {
            if (selectedMap > -1) {
                xScrollBar.Maximum = Math.Max (maps[selectedMap].Width - renderSize.Width / 2 + 10, 0);
                xScrollBar.Minimum = 0;
                xScrollBar.Value = Math.Min (xScrollBar.Value, xScrollBar.Maximum);

                yScrollBar.Maximum = Math.Max (maps[selectedMap].Height - renderSize.Height / 2 + 10, 0);
                yScrollBar.Minimum = 0;
                yScrollBar.Value = Math.Min (yScrollBar.Value, yScrollBar.Maximum);
            }
        }

        private void updateMenuItems () {
            ((ToolStripComboBox)menuItems[2]).Enabled = true;
            ((ToolStripComboBox)menuItems[2]).Items.Clear ();
            ((ToolStripComboBox)menuItems[2]).Items.AddRange (this.maps.ToArray ());
        }

        protected override void OnPaintBackground (PaintEventArgs e) {

        }

        public mapKnight.Basic.XMLElemental Save () {
            Basic.XMLElemental container = new Basic.XMLElemental ("map");
            foreach (Map map in maps) {
                container.AddChild (map.Save ());
            }
            return container;
        }

        public void Export (string homepath) {
            foreach (Map map in maps) {
                map.Export (Path.ChangeExtension (Path.Combine (homepath, map.Name), "tmsl4"));
            }
        }

        public void Load (Basic.XMLElemental config) {
            this.maps.Clear ();
            foreach (Basic.XMLElemental map in config.GetAll ()) {
                this.maps.Add (new Map (map));
            }
            updateMenuItems ();
        }

        private void drawMap () {
            if (selectedMap > -1) {
                using (BufferedGraphics g = BufferedGraphicsManager.Current.Allocate (splitContainer.Panel2.CreateGraphics (), splitContainer.Bounds)) {
                    g.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    g.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
                    g.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                    g.Graphics.Clear (this.BackColor);

                    for (int x = 0; x < renderSize.Width; x++) {
                        for (int y = 0; y < renderSize.Height; y++) {
                            if (x + xScrollBar.Value < maps[selectedMap].Width && y + yScrollBar.Value < maps[selectedMap].Height) {
                                g.Graphics.DrawImage (tileImages[maps[selectedMap].Data[x + xScrollBar.Value, y + yScrollBar.Value, 0]], x * tileSize, y * tileSize, tileSize, tileSize);
                                g.Graphics.DrawImage (tileImages[maps[selectedMap].Data[x + xScrollBar.Value, y + yScrollBar.Value, 1]], x * tileSize, y * tileSize, tileSize, tileSize);
                                g.Graphics.DrawImage (tileImages[maps[selectedMap].Data[x + xScrollBar.Value, y + yScrollBar.Value, 2]], x * tileSize, y * tileSize, tileSize, tileSize);

                                if (this.gridCheckBox.Checked)
                                    g.Graphics.DrawRectangle (Pens.Black, new Rectangle (x * tileSize, y * tileSize, tileSize, tileSize));
                            }
                        }
                    }

                    // draw spawnpoint
                    g.Graphics.FillRectangle (Brushes.Red, new RectangleF ((maps[selectedMap].Spawn.X + 0.25f - xScrollBar.Value) * tileSize,
                        (maps[selectedMap].Height - maps[selectedMap].Spawn.Y + 0.25f - yScrollBar.Value) * tileSize,
                        tileSize / 2, tileSize / 2));

                    g.Render ();
                }
            }
        }
    }
}
