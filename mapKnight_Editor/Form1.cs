using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Reflection;

namespace mapKnight_Editor
{
	public partial class Form1 : Form
	{
		private static int minTileSize = 20;
		private static int maxTileSize = 100;

		private static int minImageSize = 20;
		private static int maxImageSize = 60;

		private static int maxUndoCacheEntrys = 10;

		private enum Tool
		{
			Brush,
			Bucket,
			Eraser,
			Selector,
			SpawnPointer
		}

		public Dictionary<string, int> tileNameIndex = new Dictionary<string, int> () {
			{ "Air", 0 },
			{ "0", 0 }
		};
		public Dictionary<int, int> tileReadIndex = new Dictionary<int, int> () {
			{ 0, 0 }
		};

		public Dictionary<string, int> overlayNameIndex = new Dictionary<string, int> () {
			{ "None",0 },
			{ "0",0 }
		};
		public Dictionary<int, int> overlayReadIndex = new Dictionary<int, int> () {
			{ 0, 0 }
		};

		private readonly Cursor cursor_brush;
		private readonly Cursor cursor_bucket;
		private readonly Cursor cursor_eraser;
		private readonly Cursor cursor_selector;
		private readonly Cursor cursor_spawn;
        
		ImageList TileImageList;
		ImageList OverlayImageList;

		Bitmap[] TileImages;
		Bitmap[] OverlayImages;

		private int mapWidth;
		private int mapHeight;

		private string mapAuthor;
		private string mapName;
		private Point Spawn;

		private ushort[,,] Map;
		private bool initmap = false;

		private int tileSize = minTileSize;
		private int tileRenderWidth;
		private int tileRenderHeight;
        
		private int selectedTile = 0;
		private int selectedOverlay = 0;
		private Tool selectedTool = Tool.Brush;

		private string currentMapPath;

		private Stack<ushort[,,]> UndoCache;
		private ushort[,,] UndoLast;

		[System.Runtime.InteropServices.DllImport ("user32.dll")]
		static extern IntPtr LoadCursorFromFile (string lpFileName);

		public Form1 ()
		{
			InitializeComponent ();

			UndoCache = new Stack<ushort[,,]> ();

			cursor_brush = new Cursor (LoadCursorFromFile ("content/cursor_brush.cur"));
			cursor_bucket = new Cursor (LoadCursorFromFile ("content/cursor_bucket.cur"));
			cursor_eraser = new Cursor (LoadCursorFromFile ("content/cursor_eraser.cur"));
			cursor_selector = new Cursor (LoadCursorFromFile ("content/cursor_selector.cur"));
			cursor_spawn = new Cursor (LoadCursorFromFile ("content/cursor_spawn.cur"));

			Console.WriteLine (typeof(Form1).Assembly.GetName ().Version.ToString ());
			Console.WriteLine ("test");
		}

		private void UpdateVar ()
		{
			tileRenderWidth = (int)((hScrollBar1.ClientSize.Width) / tileSize) + 1;
			tileRenderHeight = (int)((vScrollBar1.ClientSize.Height) / tileSize) + 1;

			hScrollBar1.Maximum = Math.Max (mapWidth - tileRenderWidth + 10, 0);
			hScrollBar1.Minimum = 0;
			if (hScrollBar1.Value < 0)
				hScrollBar1.Value = 0;

			vScrollBar1.Maximum = Math.Max (mapHeight - tileRenderHeight + 10, 0);
			vScrollBar1.Minimum = 0;
			if (vScrollBar1.Value < 0)
				vScrollBar1.Value = 0;
            
			trb_zoom.Left = this.ClientSize.Width - trb_zoom.Width - 10;
			lbl_zoom.Left = trb_zoom.Left - lbl_zoom.Width - 10;
			trb_imagesize.Left = lbl_zoom.Left - trb_imagesize.Width - 10;
			lbl_imgsize.Left = trb_imagesize.Left - lbl_imgsize.Width - 10;
		}

		private void Render ()
		{
			if (initmap) {
				BufferedGraphicsContext bgc = BufferedGraphicsManager.Current;

				using (BufferedGraphics g = bgc.Allocate (this.CreateGraphics (), new Rectangle (new Point (splitter1.Left, toolStrip1.Bottom), new Size (vScrollBar1.Right - splitter1.Left, hScrollBar1.Top - toolStrip1.Bottom)))) {
					g.Graphics.Clear (Color.Black);
					g.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
					for (int x = 0; x < tileRenderWidth; x++) {
						for (int y = 0; y < tileRenderHeight; y++) {
							if (x + hScrollBar1.Value < mapWidth && y + vScrollBar1.Value < mapHeight) {
								g.Graphics.DrawRectangle (Pens.White, new Rectangle (x * tileSize + splitter1.Right, y * tileSize + toolStrip1.Bottom, tileSize, tileSize));
								g.Graphics.DrawImage (TileImages [(int)Map [x + hScrollBar1.Value, y + vScrollBar1.Value, 0]], x * tileSize + splitter1.Right, y * tileSize + toolStrip1.Bottom, tileSize, tileSize);
								g.Graphics.DrawImage (OverlayImages [(int)Map [x + hScrollBar1.Value, y + vScrollBar1.Value, 1]], x * tileSize + splitter1.Right, y * tileSize + toolStrip1.Bottom, tileSize, tileSize);
							}
						}
					}

					// draw spawnpoint
					g.Graphics.FillRectangle (Brushes.Red, new RectangleF (new PointF ((Spawn.X + 0.25f - hScrollBar1.Value) * tileSize + splitter1.Right, (Spawn.Y + 0.25f - vScrollBar1.Value) * tileSize + toolStrip1.Bottom), new SizeF (tileSize / 2, tileSize / 2)));

					g.Render ();
					g.Dispose ();
					bgc.Dispose ();
				}
			}
		}

		private void Form1_Resize (object sender, EventArgs e)
		{
			int listViewHeight = (splitter1.Height) / 2;
			lv_tile.Height = listViewHeight;
			lv_tile.Location = new Point (0, toolStrip1.Height);
			lv_overlay.Height = listViewHeight;
			lv_overlay.Location = new Point (0, lv_tile.Bottom);

			UpdateVar ();

			Render ();
		}

		private void lv_tile_ColumnWidthChanging (object sender, ColumnWidthChangingEventArgs e)
		{
			e.Cancel = true;
			e.NewWidth = lv_tile.Width - 4;
		}

		private void lv_overlay_ColumnWidthChanging (object sender, ColumnWidthChangingEventArgs e)
		{
			e.Cancel = true;
			e.NewWidth = lv_overlay.Width - 4;
		}

		private void vScrollBar1_ValueChanged (object sender, EventArgs e)
		{
			Render ();
		}

		private void hScrollBar1_ValueChanged (object sender, EventArgs e)
		{
			Render ();
		}

		private void toolStripLabel4_Click (object sender, EventArgs e)
		{
			selectedTool = Tool.Brush;
			toolStripLabel4.Checked = true;
			toolStripLabel5.Checked = false;
			toolStripLabel6.Checked = false;
			toolStripButton1.Checked = false;
			tsb_spawn.Checked = false;
		}

		private void toolStripLabel5_Click (object sender, EventArgs e)
		{
			selectedTool = Tool.Bucket;
			toolStripLabel4.Checked = false;
			toolStripLabel5.Checked = true;
			toolStripLabel6.Checked = false;
			toolStripButton1.Checked = false;
			tsb_spawn.Checked = false;
		}

		private void toolStripLabel6_Click (object sender, EventArgs e)
		{
			selectedTool = Tool.Eraser;
			toolStripLabel4.Checked = false;
			toolStripLabel5.Checked = false;
			toolStripLabel6.Checked = true;
			toolStripButton1.Checked = false;
			tsb_spawn.Checked = false;
		}

		bool clicked;

		private void Form1_MouseDown (object sender, MouseEventArgs e)
		{
			if (e.X > splitter1.Right && e.X < vScrollBar1.Left && e.Y > toolStrip1.Bottom && e.Y < hScrollBar1.Top && initmap) {
				Point locationOnMap = new Point (e.X - splitter1.Right, e.Y - toolStrip1.Bottom);
				Point clickedTile = new Point ((int)(locationOnMap.X / tileSize) + hScrollBar1.Value, (int)(locationOnMap.Y / tileSize) + vScrollBar1.Value);

				if (e.Button == MouseButtons.Left && clickedTile.X < mapWidth && clickedTile.Y < mapHeight) {
					switch (selectedTool) {
					case Tool.Brush:
						if (selectedTile == 0) {
							Map [clickedTile.X, clickedTile.Y, 1] = (ushort)selectedOverlay;
						} else {
							Map [clickedTile.X, clickedTile.Y, 0] = (ushort)selectedTile;
							Map [clickedTile.X, clickedTile.Y, 1] = (ushort)selectedOverlay;
						}
						clicked = true;
						break;
					case Tool.Eraser:
						Map [clickedTile.X, clickedTile.Y, 0] = 0;
						Map [clickedTile.X, clickedTile.Y, 1] = 0;
						clicked = true;
						break;
					case Tool.Bucket:
						Fill (clickedTile.X, clickedTile.Y, (ushort)selectedTile, (ushort)selectedOverlay);
						break;
					case Tool.SpawnPointer:
						Spawn = clickedTile;
						break;
					case Tool.Selector:
						lv_tile.SelectedIndices.Clear ();
						lv_tile.SelectedItems.Clear ();
						lv_overlay.SelectedIndices.Clear ();
						lv_overlay.SelectedIndices.Clear ();

						selectedTile = Map [clickedTile.X, clickedTile.Y, 0];
						selectedOverlay = Map [clickedTile.X, clickedTile.Y, 1];

						lv_tile.Items [selectedTile].Selected = true;
						lv_overlay.Items [selectedOverlay].Selected = true;
						break;
					}
				} else if (e.Button == MouseButtons.Right) {
					Map [clickedTile.X, clickedTile.Y, 0] = 0;
					Map [clickedTile.X, clickedTile.Y, 1] = 0;
					clicked = true;
				}
				Render ();
			}
		}

		private void Fill (int x, int y, ushort replacementtile, ushort replacementoverlay)
		{
			if (Map [x, y, 0] == replacementtile & Map [x, y, 1] == replacementoverlay)
				return;

			bool[,] allreadycomputed = new bool[mapWidth, mapHeight];
			ushort searchingtile = Map [x, y, 0];
			ushort searchingoverlay = Map [x, y, 1];

			Queue<Point> TileQueue = new Queue<Point> ();
			TileQueue.Enqueue (new Point (x, y));
			allreadycomputed [x, y] = true;

			while (TileQueue.Count > 0) {
				Point ComputingTile = TileQueue.Dequeue ();

				if (Map [ComputingTile.X, ComputingTile.Y, 0] == searchingtile && Map [ComputingTile.X, ComputingTile.Y, 1] == searchingoverlay) {
					Map [ComputingTile.X, ComputingTile.Y, 0] = replacementtile;
					Map [ComputingTile.X, ComputingTile.Y, 1] = replacementoverlay;

					if (ComputingTile.X > 0 && allreadycomputed [ComputingTile.X - 1, ComputingTile.Y] == false) {
						TileQueue.Enqueue (new Point (ComputingTile.X - 1, ComputingTile.Y));
						allreadycomputed [ComputingTile.X - 1, ComputingTile.Y] = true;
					}
					if (ComputingTile.X < mapWidth - 1 && allreadycomputed [ComputingTile.X + 1, ComputingTile.Y] == false) {
						TileQueue.Enqueue (new Point (ComputingTile.X + 1, ComputingTile.Y));
						allreadycomputed [ComputingTile.X + 1, ComputingTile.Y] = true;
					}
					if (ComputingTile.Y > 0 && allreadycomputed [ComputingTile.X, ComputingTile.Y - 1] == false) {
						TileQueue.Enqueue (new Point (ComputingTile.X, ComputingTile.Y - 1));
						allreadycomputed [ComputingTile.X, ComputingTile.Y - 1] = true;
					}
					if (ComputingTile.Y < mapHeight - 1 && allreadycomputed [ComputingTile.X, ComputingTile.Y + 1] == false) {
						TileQueue.Enqueue (new Point (ComputingTile.X, ComputingTile.Y + 1));
						allreadycomputed [ComputingTile.X, ComputingTile.Y + 1] = true;
					}
				}
			}
		}

		private void Form1_MouseMove (object sender, MouseEventArgs e)
		{

			if (e.X > splitter1.Right && e.X < vScrollBar1.Left && e.Y > toolStrip1.Bottom && e.Y < hScrollBar1.Top) {
				if (this.Cursor == Cursors.Default && toolStripButton2.Checked) {
					switch (selectedTool) {
					case Tool.Brush:
						this.Cursor = cursor_brush;
						break;
					case Tool.Bucket:
						this.Cursor = cursor_bucket;
						break;
					case Tool.Eraser:
						this.Cursor = cursor_eraser;
						break;
					case Tool.Selector:
						this.Cursor = cursor_selector;
						break;
					case Tool.SpawnPointer:
						this.Cursor = cursor_spawn;
						break;
					}
				}
				if (clicked) {
					Point locationOnMap = new Point (e.X - splitter1.Right, e.Y - toolStrip1.Bottom);
					Point clickedTile = new Point ((int)locationOnMap.X / tileSize + hScrollBar1.Value, (int)locationOnMap.Y / tileSize + vScrollBar1.Value);

					if (e.Button == MouseButtons.Left && clickedTile.X < mapWidth && clickedTile.Y < mapHeight) {
						switch (selectedTool) {
						case Tool.Brush:
							if (selectedTile == 0) {
								Map [clickedTile.X, clickedTile.Y, 1] = (ushort)selectedOverlay;
							} else {
								Map [clickedTile.X, clickedTile.Y, 0] = (ushort)selectedTile;
								Map [clickedTile.X, clickedTile.Y, 1] = (ushort)selectedOverlay;
							}
							clicked = true;
							break;
						case Tool.Eraser:
							Map [clickedTile.X, clickedTile.Y, 0] = 0;
							Map [clickedTile.X, clickedTile.Y, 1] = 0;
							clicked = true;
							break;
						}
					} else if (e.Button == MouseButtons.Right) {
						Map [clickedTile.X, clickedTile.Y, 0] = 0;
						Map [clickedTile.X, clickedTile.Y, 1] = 0;
					}

					Render ();
				}
			} else {
				this.Cursor = Cursors.Default;
			}
		}

		private void Form1_MouseUp (object sender, MouseEventArgs e)
		{
			clicked = false;

			UndoCache.Push (UndoLast);
			UndoLast = (ushort[,,])Map.Clone ();
			if (UndoCache.Count > maxUndoCacheEntrys)
				UndoCache.Pop ();
		}

		private void lv_tile_SelectedIndexChanged (object sender, EventArgs e)
		{
			if (lv_tile.SelectedIndices.Count > 0) {
				lv_tile.Items [selectedTile].BackColor = Color.White;
				selectedTile = 0;
				selectedTile = (ushort)lv_tile.SelectedIndices [0];
				lv_tile.Items [selectedTile].BackColor = Color.LightSkyBlue;
				lv_tile.SelectedIndices.Clear ();
			}
		}

		private void lv_overlay_SelectedIndexChanged (object sender, EventArgs e)
		{
			if (lv_overlay.SelectedIndices.Count > 0) {
				lv_overlay.Items [selectedOverlay].BackColor = Color.White;
				selectedOverlay = 0;
				selectedOverlay = (ushort)lv_overlay.SelectedIndices [0];
				lv_overlay.Items [selectedOverlay].BackColor = Color.LightSkyBlue;
				lv_overlay.SelectedIndices.Clear ();
			}
		}

		private void saveToolStripMenuItem1_Click (object sender, EventArgs e)
		{
			sfd_mapsave.ShowDialog ();
			if (sfd_mapsave.FileName != "") {
				Save (sfd_mapsave.FileName);
				currentMapPath = sfd_mapsave.FileName;
				overwriteToolStripMenuItem.Enabled = true;
			}
		}

		private void overwriteToolStripMenuItem_Click (object sender, EventArgs e)
		{
			Save (currentMapPath);
		}

		private void Save (string path)
		{
			if (File.Exists (path))
				File.Delete (path);
			XML.XMLElemental parsedelemental = XML.XMLElemental.EmptyRootElemental ("Map");
			parsedelemental.AddComment ("Created with mapKnight Map Editor");

			parsedelemental.Attributes.Add ("Author", mapAuthor);
			parsedelemental.Attributes.Add ("Name", mapName);
			parsedelemental.Attributes.Add ("Spawn", Spawn.X.ToString () + ";" + Spawn.X.ToString ());

			parsedelemental.AddChild ("Def");

			int CurrentIndex = 0;
			Dictionary<string, string> ValueStringIndex = new Dictionary<string, string> ();
			for (int tile = 0; tile < lv_tile.Items.Count; tile++) {
				for (int overlay = 0; overlay < lv_overlay.Items.Count; overlay++) {
					parsedelemental ["Def"].Value += CurrentIndex.ToString () + "=" + tileReadIndex [tile].ToString () + "," + overlayReadIndex [overlay].ToString () + ";";
					ValueStringIndex.Add (tileReadIndex [tile].ToString () + "*" + overlayReadIndex [overlay].ToString (), CurrentIndex.ToString ());
					CurrentIndex++;
				}
			}

			parsedelemental.AddChild ("Data");
			parsedelemental ["Data"].Attributes.Add ("Width", mapWidth.ToString ());
			parsedelemental ["Data"].Attributes.Add ("Height", mapHeight.ToString ());
			int CurrentCount = 0;
			ushort CurrentTile = Map [0, 0, 0];
			ushort CurrentOverlay = Map [0, 0, 1];
			for (int y = 0; y < mapHeight; y++) {
				for (int x = 0; x < mapWidth; x++) {
					if (CurrentTile != Map [x, y, 0] || CurrentOverlay != Map [x, y, 1]) {
						parsedelemental ["Data"].Value += CurrentCount.ToString () + "~" + ValueStringIndex [tileReadIndex [CurrentTile].ToString () + "*" + overlayReadIndex [CurrentOverlay].ToString ()].ToString () + ",";
						CurrentTile = Map [x, y, 0];
						CurrentOverlay = Map [x, y, 1];
						CurrentCount = 0;
					}
					CurrentCount++;
				}
			}
			if (CurrentCount != 0) {
				parsedelemental ["Data"].Value += CurrentCount.ToString () + "~" + ValueStringIndex [CurrentTile.ToString () + "*" + CurrentOverlay.ToString ()].ToString () + ",";
			}
			parsedelemental.AddChild ("Background");

			string elementalstring = parsedelemental.Flush ();

			using (StreamWriter writer = new StreamWriter (File.OpenWrite (path))) {
				writer.WriteLine (elementalstring);
				writer.Flush ();
			}

			MessageBox.Show ("Saving successful", "Complete", MessageBoxButtons.OK);
		}

		private void loadToolStripMenuItem_Click (object sender, EventArgs e)
		{
			ofd_mapfile.ShowDialog ();
			string mappath = ofd_mapfile.FileName;

			if (mappath != "") {
				saveToToolStripMenuItem.Enabled = true;
				overwriteToolStripMenuItem.Enabled = true;
				currentMapPath = mappath;

				XML.XMLElemental mapelemental = XML.XMLElemental.Load (File.OpenRead (mappath));

				mapAuthor = mapelemental.Attributes ["Author"];
				mapName = mapelemental.Attributes ["Name"];
				Spawn = new Point (Convert.ToInt32 (mapelemental.Attributes ["Spawn"].Split (new char[] { ';' }) [0]), Convert.ToInt32 (mapelemental.Attributes ["Spawn"].Split (new char[] { ';' }) [1]));

				mapWidth = Convert.ToInt32 (mapelemental ["Data"].Attributes ["Width"]);
				mapHeight = Convert.ToInt32 (mapelemental ["Data"].Attributes ["Height"]);
				Map = new ushort[mapWidth, mapHeight, 2];

				Dictionary<string, ushort[]> StringTileIndex = new Dictionary<string, ushort[]> ();

				foreach (string data in mapelemental["Def"].Value.Split(new char[] { ';' },StringSplitOptions.RemoveEmptyEntries)) {
					ushort tile = (ushort)tileNameIndex [data.Split ('=') [1].Split (',') [0]];
					ushort overlay = (ushort)overlayNameIndex [data.Split ('=') [1].Split (',') [1]];

					StringTileIndex.Add (data.Split ('=') [0], new ushort[] { tile, overlay });
				}

				int cX = 0;
				int cY = 0;
				foreach (string data in mapelemental["Data"].Value.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries)) {
					string cTile = data.Split ('~') [1];
					for (int i = 0; i < Convert.ToInt32 (data.Split ('~') [0]); i++) {
						Map [cX, cY, 0] = StringTileIndex [cTile] [0];
						Map [cX, cY, 1] = StringTileIndex [cTile] [1];

						cX++;
						if (cX == mapWidth) {
							cX = 0;
							cY++;
							if (cY == mapHeight) {
								break;
							}
						}
					}
				}

				initmap = true;
				UndoLast = (ushort[,,])Map.Clone ();
				Render ();
				this.OnResize (EventArgs.Empty);
			}
		}

		private void saveToolStripMenuItem_Click (object sender, EventArgs e)
		{
			DialogForm infoform = new DialogForm ();
			infoform.ShowDialog ();

			if (infoform.finished) {
				saveToToolStripMenuItem.Enabled = true;
				overwriteToolStripMenuItem.Enabled = false;

				mapWidth = (int)infoform.nud_Width.Value;
				mapHeight = (int)infoform.nud_Height.Value;
				Map = new ushort[mapWidth, mapHeight, 2];

				mapAuthor = infoform.tb_mapauthor.Text;
				mapName = infoform.tb_mapname.Text;

				initmap = true;
				UndoLast = (ushort[,,])Map.Clone ();
				Render ();
				this.OnResize (EventArgs.Empty);
			}
		}

		private void tsb_spawn_Click (object sender, EventArgs e)
		{
			selectedTool = Tool.SpawnPointer;
			toolStripLabel4.Checked = false;
			toolStripLabel5.Checked = false;
			toolStripLabel6.Checked = false;
			toolStripButton1.Checked = false;
			tsb_spawn.Checked = true;
		}

		private void toolStripButton1_Click (object sender, EventArgs e)
		{
			selectedTool = Tool.Selector;
			toolStripLabel4.Checked = false;
			toolStripLabel5.Checked = false;
			toolStripLabel6.Checked = false;
			toolStripButton1.Checked = true;
			tsb_spawn.Checked = false;
		}

		private void trb_zoom_ValueChanged (object sender, EventArgs e)
		{
			tileSize = Convert.ToInt32 ((maxTileSize - minTileSize) * trb_zoom.Value / 2 / 10 + minTileSize);
			UpdateVar ();
			Render ();
		}

		private void Form1_MouseWheel (object sender, MouseEventArgs e)
		{
			if (e.Delta > 0) {
				if (trb_zoom.Value < trb_zoom.Maximum)
					trb_zoom.Value += 1;
			} else if (e.Delta < 0) {
				if (trb_zoom.Value > trb_zoom.Minimum)
					trb_zoom.Value -= 1;
			}
		}

		private void Form1_Load (object sender, EventArgs e)
		{
			this.MouseWheel += Form1_MouseWheel;
            
			lv_tile.View = View.LargeIcon;

			TileImageList = new ImageList () { ImageSize = new Size (20, 20), ColorDepth = ColorDepth.Depth32Bit };
			TileImageList.Images.Add (new Bitmap (20, 20));

			lv_tile.SmallImageList = TileImageList;
			lv_tile.LargeImageList = TileImageList;

			lv_tile.Items.Add (new ListViewItem ("Air") { ImageIndex = 0 });

			lv_overlay.View = View.LargeIcon;

			OverlayImageList = new ImageList () { ImageSize = new Size (20, 20), ColorDepth = ColorDepth.Depth32Bit };
			OverlayImageList.Images.Add (new Bitmap (20, 20));

			lv_overlay.SmallImageList = OverlayImageList;
			lv_overlay.LargeImageList = OverlayImageList;

			lv_overlay.Items.Add (new ListViewItem ("None") { ImageIndex = 0 });

			XML.XMLElemental contentsonfig = XML.XMLElemental.Load (File.OpenRead ("content/config.xml"));
			foreach (XML.XMLElemental child in contentsonfig.GetAll()) {
				switch (child.Attributes ["Type"]) {
				case "Tile":
					tileNameIndex.Add (child.Attributes ["TextID"], TileImageList.Images.Count);
					tileNameIndex.Add (child.Attributes ["NumID"], TileImageList.Images.Count);
					tileReadIndex.Add (TileImageList.Images.Count, Convert.ToInt32 (child.Attributes ["NumID"]));
					lv_tile.Items.Add (new ListViewItem (child.Attributes ["Name"]) { ImageIndex = TileImageList.Images.Count });
					TileImageList.Images.Add (new Bitmap (child.Attributes ["Source"]));
					break;
				case "Overlay":
					overlayNameIndex.Add (child.Attributes ["TextID"], OverlayImageList.Images.Count);
					overlayNameIndex.Add (child.Attributes ["NumID"], OverlayImageList.Images.Count);
					overlayReadIndex.Add (OverlayImageList.Images.Count, Convert.ToInt32 (child.Attributes ["NumID"]));
					lv_overlay.Items.Add (new ListViewItem (child.Attributes ["Name"]) { ImageIndex = OverlayImageList.Images.Count });
					OverlayImageList.Images.Add (new Bitmap (child.Attributes ["Source"]));
					break;
				default:
					MessageBox.Show ("Error while loading config file! element = " + contentsonfig.GetAll ().IndexOf (child).ToString ());
					break;
				}
			}

			OverlayImages = OverlayImageList.Images.Cast<Bitmap> ().ToArray<Bitmap> ();
			TileImages = TileImageList.Images.Cast<Bitmap> ().ToArray<Bitmap> ();
		}

		private void Form1_Shown (object sender, EventArgs e)
		{
			this.Form1_Resize (this, EventArgs.Empty);
		}

		private void trb_imagesize_ValueChanged (object sender, EventArgs e)
		{
			int ListViewImageSize = Convert.ToInt32 ((maxImageSize - minImageSize) * Convert.ToDouble (trb_imagesize.Value / 2) / 10 + minImageSize);

			OverlayImageList = new ImageList () {
				ImageSize = new Size (ListViewImageSize, ListViewImageSize),
				ColorDepth = ColorDepth.Depth32Bit
			};
			OverlayImageList.Images.AddRange (OverlayImages);
			TileImageList = new ImageList () {
				ImageSize = new Size (ListViewImageSize, ListViewImageSize),
				ColorDepth = ColorDepth.Depth32Bit
			};
			TileImageList.Images.AddRange (TileImages);

			lv_tile.LargeImageList = TileImageList;
			lv_overlay.LargeImageList = OverlayImageList;
		}

		private void revertToolStripMenuItem_Click (object sender, EventArgs e)
		{
			if (UndoCache.Count > 0) {
				Map = UndoCache.Pop ();
				Render ();
				UndoLast = (ushort[,,])Map.Clone ();
			}
		}

		private void Form1_KeyDown (object sender, KeyEventArgs e)
		{
			if (e.KeyData == (Keys.Control | Keys.Z))
				revertToolStripMenuItem_Click (this, EventArgs.Empty);
		}
	}
}
