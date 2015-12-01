using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Reflection;

using XML;

namespace mapKnight_Editor
{
	public partial class Main : Form
	{
		#region static member

		private static int iMinTileSize = 20;
		private static int iMaxTileSize = 100;

		private static int iMinImageSize = 20;
		private static int iMaxImageSize = 60;

		private static Color iSelectionColor = Color.DimGray;
		private static Color iMapColor = Color.DimGray;

        private static string iContentDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"mapKnight ToolKit\content");

		#endregion

		private enum Tool
		{
			Brush,
			Bucket,
			Eraser,
			Select,
			SpawnPointer
		}

		#region attributes

		// used to parse a mapfile to mapshort

		private Dictionary<string, ushort> iTileNameIndex = new Dictionary<string, ushort> () {
			{ "Air", 0 },
			{ "0", 0 },
			{ "", 0 }
		};

		private Dictionary<string, ushort> iOverlayNameIndex = new Dictionary<string, ushort> () {
			{ "None", 0 },
			{ "0", 0 },
			{ "", 0 }
		};

		private Dictionary<int, ushort> iTileIDIndex = new Dictionary<int, ushort> () {
			{ 0,0 }
		};

		private Dictionary<int, ushort> iOverlayIDIndex = new Dictionary<int, ushort> () {
			{ 0,0 }
		};

		private ToolStripTextTrackbar zoombar;
		private ToolStripTextTrackbar imagesizebar;

		private Bitmap[] iTileImages;
		private Bitmap[] iOverlayImages;

		private List<Map> iMaps = new List<Map> ();
		private int iSelectedMap = -1;

		private int iTileSize = iMinTileSize;
		private int iTileRenderWidth;
		private int iTileRenderHeight;

		private Tool iSelectedTool = Tool.Brush;

		private string iCurrentFilePath = null;

		private bool iClicked = false;

		private int iSelectedTile;
		private int iSelectedOverlay;

		#endregion

		public Main (string filepath)
		{
			InitializeComponent ();
            
            iCurrentFilePath = filepath;

			this.tlstrp_map.Renderer = new CustomToolStripRenderer (true);
			this.tlstrp_main.Renderer = new CustomToolStripRenderer (false);
		}

		private void tbctrl_main_SelectedIndexChanged (object sender, EventArgs e)
		{
			if (tbctrl_main.SelectedIndex == 0) {
				renderMap ();
			}
		}

		#region form events

		private void Main_Load (object sender, EventArgs e)
		{
			zoombar = new ToolStripTextTrackbar (tlstrp_map, 0, 20, "Zoom");
			zoombar.TrackBar.TrackBar.ValueChanged += (object tsender, EventArgs te) => {
				iTileSize = Convert.ToInt32 ((iMaxTileSize - iMinTileSize) * zoombar.TrackBar.TrackBar.Value / 2 / 10 + iMinTileSize);
				updateMapVar ();
				renderMap ();
			};

			imagesizebar = new ToolStripTextTrackbar (tlstrp_map, 0, 20, "Image Size");
			imagesizebar.TrackBar.TrackBar.ValueChanged += (object tsender, EventArgs te) => {
				int iListViewImageSize = Convert.ToInt32 ((iMaxImageSize - iMinImageSize) * Convert.ToDouble (imagesizebar.TrackBar.TrackBar.Value / 2) / 10 + iMinImageSize);

				lvw_tiles.LargeImageList.ImageSize = new Size (iListViewImageSize, iListViewImageSize);
				lvw_tiles.LargeImageList.Images.AddRange (iTileImages);

				lvw_overlays.LargeImageList.ImageSize = new Size (iListViewImageSize, iListViewImageSize);
				lvw_overlays.LargeImageList.Images.AddRange (iOverlayImages);
			};

			initMapListView ();

			lvw_tiles.SelectedIndices.Add (0);
			lvw_overlays.SelectedIndices.Add (0);

            if(iCurrentFilePath != null)
            {
                load();
            }
		}

		private void Main_Resize (object sender, EventArgs e)
		{
			updateMapVar ();
		}

		private void Main_KeyDown (object sender, KeyEventArgs e)
		{
			if (e.KeyData == (Keys.Control | Keys.Z))
				tsmi_undo_Click (this, EventArgs.Empty);
			if (e.KeyData == (Keys.Control | Keys.S))
				tsmi_save_Click (this, EventArgs.Empty);
			if (e.KeyData == (Keys.Control | Keys.L))
				tsmi_load_Click (this, EventArgs.Empty);
		}

		#endregion

		#region map

		private void renderMap ()
		{
			if (iSelectedMap > -1) {
				BufferedGraphicsContext bgc = BufferedGraphicsManager.Current;

				using (BufferedGraphics g = bgc.Allocate (tbpg_mapeditor.CreateGraphics (), new Rectangle (new Point (spltcntr_map.Left, tlstrp_map.Bottom), new Size (vscrlbar_map.Right - spltcntr_map.Left, hscrlbar_map.Top - tlstrp_map.Bottom)))) {
					g.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					g.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
					g.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
					g.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
					g.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

					g.Graphics.Clear (iMapColor);

					for (int x = 0; x < iTileRenderWidth; x++) {
						for (int y = 0; y < iTileRenderHeight; y++) {
							if (x + hscrlbar_map.Value < iMaps [iSelectedMap].Width && y + vscrlbar_map.Value < iMaps [iSelectedMap].Height) {
								g.Graphics.DrawRectangle (Pens.White, new Rectangle (x * iTileSize + spltcntr_map.Right, y * iTileSize + tlstrp_map.Bottom, iTileSize, iTileSize));
								g.Graphics.DrawImage (iTileImages [(int)iMaps [iSelectedMap].Data [x + hscrlbar_map.Value, y + vscrlbar_map.Value, 0]], x * iTileSize + spltcntr_map.Right, y * iTileSize + tlstrp_map.Bottom, iTileSize, iTileSize);
								g.Graphics.DrawImage (iOverlayImages [(int)iMaps [iSelectedMap].Data [x + hscrlbar_map.Value, y + vscrlbar_map.Value, 1]], x * iTileSize + spltcntr_map.Right, y * iTileSize + tlstrp_map.Bottom, iTileSize, iTileSize);
							}
						}
					}

					// draw spawnpoint
					g.Graphics.FillRectangle (Brushes.Red, new RectangleF (new PointF ((iMaps [iSelectedMap].Spawn.X + 0.25f - hscrlbar_map.Value) * iTileSize + spltcntr_map.Right, (iMaps [iSelectedMap].Spawn.Y + 0.25f - vscrlbar_map.Value) * iTileSize + tlstrp_map.Bottom), new SizeF (iTileSize / 2, iTileSize / 2)));

					g.Render ();
					g.Dispose ();
					bgc.Dispose ();
				}
			}
		}

		private void updateMapVar ()
		{
			iTileRenderWidth = (int)((hscrlbar_map.ClientSize.Width) / iTileSize) + 1;
			iTileRenderHeight = (int)((vscrlbar_map.ClientSize.Height) / iTileSize) + 1;

			if (iSelectedMap > -1) {
				hscrlbar_map.Maximum = Math.Max (iMaps [iSelectedMap].Width - iTileRenderWidth + 10, 0);
				hscrlbar_map.Minimum = 0;
				hscrlbar_map.Value = 0;

				vscrlbar_map.Maximum = Math.Max (iMaps [iSelectedMap].Height - iTileRenderHeight + 10, 0);
				vscrlbar_map.Minimum = 0;
				vscrlbar_map.Value = 0;
			}
		}

		private void initMapListView ()
		{
			lvw_tiles.Items.Add (new ListViewItem ("Air") { ImageIndex = 0 });
			lvw_tiles.LargeImageList.Images.Add (new Bitmap (20, 20));

			lvw_overlays.Items.Add (new ListViewItem ("None") { ImageIndex = 0 });
			lvw_overlays.LargeImageList.Images.Add (new Bitmap (20, 20));

			XMLElemental contentconfig = XMLElemental.Load (File.OpenRead (Path.Combine(iContentDirectory, ".config")));
            XMLElemental additionalcontentconfig = XMLElemental.Load(File.OpenRead(Path.Combine(iContentDirectory, "additional.config")));
            foreach (XMLElemental child in contentconfig.GetAll().Concat(additionalcontentconfig.GetAll())) {
				switch (child.Attributes ["Type"]) {
				case "Tile":
					iTileNameIndex.Add (child.Attributes ["TextID"], (ushort)lvw_tiles.LargeImageList.Images.Count);
					iTileNameIndex.Add (child.Attributes ["NumID"], (ushort)lvw_tiles.LargeImageList.Images.Count);

					iTileIDIndex.Add (lvw_tiles.LargeImageList.Images.Count, (ushort)Convert.ToInt32 (child.Attributes ["NumID"]));

					lvw_tiles.Items.Add (new ListViewItem (child.Attributes ["Name"]) { ImageIndex = lvw_tiles.LargeImageList.Images.Count });
					lvw_tiles.LargeImageList.Images.Add (child.Attributes ["NumID"], new Bitmap (Path.Combine(iContentDirectory,child.Attributes ["Source"])));
					break;
				case "Overlay":
					iOverlayNameIndex.Add (child.Attributes ["TextID"], (ushort)lvw_overlays.LargeImageList.Images.Count);
					iOverlayNameIndex.Add (child.Attributes ["NumID"], (ushort)lvw_overlays.LargeImageList.Images.Count);

					iOverlayIDIndex.Add (lvw_overlays.LargeImageList.Images.Count, (ushort)Convert.ToInt32 (child.Attributes ["NumID"]));

					lvw_overlays.Items.Add (new ListViewItem (child.Attributes ["Name"]) { ImageKey = child.Attributes ["NumID"] });
					lvw_overlays.LargeImageList.Images.Add (child.Attributes ["NumID"], new Bitmap (Path.Combine(iContentDirectory,child.Attributes ["Source"])));
					break;
				}
			}

			iOverlayImages = lvw_overlays.LargeImageList.Images.Cast<Bitmap> ().ToArray<Bitmap> ();
			iTileImages = lvw_tiles.LargeImageList.Images.Cast<Bitmap> ().ToArray<Bitmap> ();
		}

		private void hscrlbar_map_Scroll (object sender, ScrollEventArgs e)
		{
			renderMap ();
		}

		private void vscrlbr_map_Scroll (object sender, ScrollEventArgs e)
		{
			renderMap ();
		}

		private void tbpg_mapeditor_MouseWheel (object sender, MouseEventArgs e)
		{
			if (e.Delta > 0) {
				if (zoombar.TrackBar.TrackBar.Value < zoombar.TrackBar.TrackBar.Maximum)
					zoombar.TrackBar.TrackBar.Value += 1;
			} else if (e.Delta < 0) {
				if (zoombar.TrackBar.TrackBar.Value > zoombar.TrackBar.TrackBar.Minimum)
					zoombar.TrackBar.TrackBar.Value -= 1;
			}
		}

		private void tbpg_mapeditor_MouseDown (object sender, MouseEventArgs e)
		{
			if (e.X > spltcntr_map.Right && e.X < vscrlbar_map.Left && e.Y > tlstrp_map.Bottom && e.Y < hscrlbar_map.Top && iSelectedMap > -1) {
				Point locationOnMap = new Point (e.X - spltcntr_map.Right, e.Y - tlstrp_map.Bottom);
				Point clickedTile = new Point ((int)locationOnMap.X / iTileSize + hscrlbar_map.Value, (int)locationOnMap.Y / iTileSize + vscrlbar_map.Value);

				if (e.Button == MouseButtons.Left && clickedTile.X < iMaps [iSelectedMap].Width && clickedTile.Y < iMaps [iSelectedMap].Height) {
					switch (iSelectedTool) {
					case Tool.Brush:
						if (iSelectedTile == 0) {
							iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1] = (ushort)iSelectedOverlay;
						} else {
							iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 0] = (ushort)iSelectedTile;
							iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1] = (ushort)iSelectedOverlay;
						}
						iClicked = true;
						break;
					case Tool.Eraser:
						iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 0] = 0;
						iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1] = 0;
						iClicked = true;
						break;
					case Tool.Bucket:
						iMaps [iSelectedMap].Fill (clickedTile.X, clickedTile.Y, (ushort)iSelectedTile, (ushort)iSelectedOverlay);
						break;
					case Tool.SpawnPointer:
						iMaps [iSelectedMap].Spawn = clickedTile;
						break;
					case Tool.Select:
						lvw_tiles.Items [(int)iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 0]].Selected = true;
						lvw_tiles.Items [(int)iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 0]].Focused = true;

						lvw_overlays.Items [(int)iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1]].Selected = true;
						lvw_overlays.Items [(int)iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1]].Focused = true;
						break;
					}
				} else if (e.Button == MouseButtons.Right) {
					iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 0] = 0;
					iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1] = 0;
					iClicked = true;
				}

				renderMap ();
			}
		}

		private void tbpg_mapeditor_MouseMove (object sender, MouseEventArgs e)
		{
			if (e.X > spltcntr_map.Right && e.X < vscrlbar_map.Left && e.Y > tlstrp_map.Bottom && e.Y < hscrlbar_map.Top && iClicked) {
				Point locationOnMap = new Point (e.X - spltcntr_map.Right, e.Y - tlstrp_map.Bottom);
				Point clickedTile = new Point ((int)locationOnMap.X / iTileSize + hscrlbar_map.Value, (int)locationOnMap.Y / iTileSize + vscrlbar_map.Value);

				if (e.Button == MouseButtons.Left && clickedTile.X < iMaps [iSelectedMap].Width && clickedTile.Y < iMaps [iSelectedMap].Height) {
					switch (iSelectedTool) {
					case Tool.Brush:
						if (iSelectedTile == 0) {
							iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1] = (ushort)iSelectedOverlay;
						} else {
							iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 0] = (ushort)iSelectedTile;
							iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1] = (ushort)iSelectedOverlay;
						}
						break;
					case Tool.Eraser:
						iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 0] = 0;
						iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1] = 0;
						break;
					}
				} else if (e.Button == MouseButtons.Right) {
					iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 0] = 0;
					iMaps [iSelectedMap].Data [clickedTile.X, clickedTile.Y, 1] = 0;
				}

				renderMap ();
			}
		}

		private void tbpg_mapeditor_MouseUp (object sender, MouseEventArgs e)
		{
			if (iClicked) {
				iClicked = false;
				iMaps [iSelectedMap].PrepareUndo ();
			}
		}

		private void tbpg_mapeditor_Paint (object sender, PaintEventArgs e)
		{
			renderMap ();
		}

		private void tsb_map_brush_Click (object sender, EventArgs e)
		{
			iSelectedTool = Tool.Brush;
			updateMapToolButtons (tsb_map_brush);
		}

		private void tsb_map_eraser_Click (object sender, EventArgs e)
		{
			iSelectedTool = Tool.Eraser;
			updateMapToolButtons (tsb_map_eraser);
		}

		private void tsb_map_select_Click (object sender, EventArgs e)
		{
			iSelectedTool = Tool.Select;
			updateMapToolButtons (tsb_map_select);
		}

		private void tsb_map_fill_Click (object sender, EventArgs e)
		{
			iSelectedTool = Tool.Bucket;
			updateMapToolButtons (tsb_map_fill);
		}

		private void tsb_map_wand_Click (object sender, EventArgs e)
		{
			iSelectedTool = Tool.SpawnPointer;
			updateMapToolButtons (tsb_map_wand);
		}

		private void updateMapToolButtons (ToolStripButton sender)
		{
			sender.Checked = true;

			foreach (ToolStripItem item in tlstrp_map.Items) {
				if (item.GetType () == typeof(ToolStripButton) && (ToolStripButton)item != sender) {
					((ToolStripButton)item).Checked = false;
				}
			}
		}

		#endregion

		#region toolstrip menu items

		private void tsmi_undo_Click (object sender, EventArgs e)
		{
			if (tbctrl_main.SelectedIndex == 0) {
				renderMap ();
			}
		}

		private void tsmi_load_Click (object sender, EventArgs e)
		{
			ofd_mapfile.ShowDialog ();
			string filepath = ofd_mapfile.FileName;

			if (filepath != "") {
				tsmi_save.Enabled = true;
				tsmi_saveas.Enabled = true;
				iCurrentFilePath = filepath;
				load ();
			}
		}

		private void tsmi_save_Click (object sender, EventArgs e)
		{
			if (iCurrentFilePath == null)
				tsmi_saveas_Click (this, EventArgs.Empty);
			else
				save ();
		}

		private void tsmi_saveas_Click (object sender, EventArgs e)
		{
			sfd_mapfile.ShowDialog ();
			if (sfd_mapfile.FileName != "") {
				iCurrentFilePath = sfd_mapfile.FileName;
				save ();
			}

		}

		private void tsb_info_Click (object sender, EventArgs e)
		{
			InfoWindow infowinfow = new InfoWindow (new XML.Version (Assembly.GetExecutingAssembly ().GetName ().Version.ToString ()));
			infowinfow.ShowDialog ();
		}

		#endregion

		private void save ()
		{
			XMLElemental savefile = XMLElemental.EmptyRootElemental ("workfile");
			foreach (Map map in iMaps) {
				savefile.AddChild (map.Save (iTileIDIndex, iOverlayIDIndex));
			}

			using (StreamWriter writer = new StreamWriter (File.OpenWrite (iCurrentFilePath))) {
				writer.WriteLine (savefile.Flush ());
				writer.Flush ();
			}

			MessageBox.Show ("Saving sucessfull", "Sucess!", MessageBoxButtons.OK);
		}

		private void load ()
		{
			XMLElemental loadfile = XMLElemental.Load (File.OpenRead (iCurrentFilePath));

			foreach (XMLElemental child in loadfile.GetAll()) {
				if (child.Name == "map") {
					iMaps.Add (new Map (child, iTileNameIndex, iOverlayNameIndex));
					tscb_map_mapselect.Items.Add (iMaps [iMaps.Count - 1].Name);
				}
			}

			if (iMaps.Count > 0) {
				iSelectedMap = 0;
				tscb_map_mapselect.SelectedIndex = 0;

				updateMapVar ();
				renderMap ();
			}
		}

		private void lvw_tiles_SelectedIndexChanged (object sender, EventArgs e)
		{
			if (lvw_tiles.SelectedIndices.Count > 0) {
				lvw_tiles.Items [iSelectedTile].BackColor = lvw_tiles.BackColor;
				iSelectedTile = lvw_tiles.SelectedIndices [0];
				lvw_tiles.Items [lvw_tiles.SelectedIndices [0]].BackColor = iSelectionColor;
				lvw_tiles.SelectedIndices.Clear ();
			}
		}

		private void lvw_overlays_SelectedIndexChanged (object sender, EventArgs e)
		{

			if (lvw_overlays.SelectedIndices.Count > 0) {
				lvw_overlays.Items [iSelectedOverlay].BackColor = lvw_overlays.BackColor;
				iSelectedOverlay = lvw_overlays.SelectedIndices [0];
				lvw_overlays.Items [lvw_overlays.SelectedIndices [0]].BackColor = iSelectionColor;
				lvw_overlays.SelectedIndices.Clear ();
			}
		}

		private void tscb_map_mapselect_SelectedIndexChanged (object sender, EventArgs e)
		{
			iSelectedMap = tscb_map_mapselect.SelectedIndex;
			updateMapVar ();
			renderMap ();
		}

		private void tsb_addnew_Click (object sender, EventArgs e)
		{
			DialogForm mapCreateForm = new DialogForm ();
			mapCreateForm.ShowDialog ();
			if (mapCreateForm.Finished) {
				iMaps.Add (new Map ((int)mapCreateForm.nud_Width.Value, (int)mapCreateForm.nud_Height.Value, mapCreateForm.tb_mapauthor.Text, mapCreateForm.tb_mapname.Text, new Point (0, 0)));
				tscb_map_mapselect.Items.Add (iMaps [iMaps.Count - 1].Name);
				tscb_map_mapselect.SelectedIndex = iMaps.Count - 1;
			}
		}

        private void tsmmi_exportraw_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            string path = folderBrowserDialog.SelectedPath;

            string mappath = Path.Combine(path, "maps");
            string characterpath = Path.Combine(path, "character");
            string animationpath = Path.Combine(path, "animations");
            string entitypath = Path.Combine(path, "entitys");

            if (!Directory.Exists(mappath)) Directory.CreateDirectory(mappath);
            if (!Directory.Exists(characterpath)) Directory.CreateDirectory(characterpath);
            if (!Directory.Exists(animationpath)) Directory.CreateDirectory(animationpath);
            if (!Directory.Exists(entitypath)) Directory.CreateDirectory(entitypath);

            // convert all saved maps to the mapfiles
            foreach (Map map in iMaps)
            {
                XMLElemental mapelement = map.Save(iTileIDIndex, iOverlayIDIndex);
                File.Create(Path.Combine(mappath, map.Name + ".devmap")).Close();
                File.WriteAllText(Path.Combine(mappath, map.Name + ".devmap"), mapelement.Flush());
                File.Create(Path.Combine(mappath, map.Name + ".map")).Close();
                File.WriteAllText(Path.Combine(mappath, map.Name + ".map"), StringManager.ZipString(mapelement.Flush()));
            }

            MessageBox.Show("Export successful");
        }
    }
}
