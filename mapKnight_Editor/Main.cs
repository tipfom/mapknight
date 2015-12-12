using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualBasic;

using mapKnight.Utils;

namespace mapKnight.ToolKit
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

		private static string iContentDirectory = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), @"mapKnight ToolKit\content");

		private static Dictionary<Character, List<Slot>> supportedSlots = new Dictionary<Character, List<Slot>> () {
			{ Character.Robot, new List<Slot> () { Slot.Chestplate, Slot.Gloves, Slot.Helmet, Slot.Shoes } }
		};
		private static int unselected = -1;

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

		private Dictionary<Character, List<Set>> iSets = new Dictionary<Character, List<Set>> ();
		private Character iSelectedCharacter;
		private int iSelectedSet = -1;
		private int selectedPart = -1;

		private AttributeListView attributelistview_item = new AttributeListView ();
		private ItemPictureBox picturebox_itempreview = new ItemPictureBox ();

		#endregion

		public Main (string filepath)
		{
			InitializeComponent ();

			this.attributelistview_item.Dock = DockStyle.Fill;
			this.splitContainer4.Panel2.Controls.Add (this.attributelistview_item);
			this.picturebox_itempreview.Dock = DockStyle.Fill;
			this.splitContainer4.Panel1.Controls.Add (this.picturebox_itempreview);
			this.picturebox_itempreview.SendToBack ();

			iCurrentFilePath = filepath;

			this.tlstrp_map.Renderer = new CustomToolStripRenderer (true);
			this.tlstrp_main.Renderer = new CustomToolStripRenderer (false);
			this.attributelistview_item.AttributeChanged += Attributelistview_item_AttributeChanged;
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
			listview_slot.SmallImageList = new ImageList () {
				ColorDepth = ColorDepth.Depth32Bit,
				ImageSize = new Size (20, 20)
			};

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
			initCharacterEdit ();

			lvw_tiles.SelectedIndices.Add (0);
			lvw_overlays.SelectedIndices.Add (0);

			if (iCurrentFilePath != null) {
				load ();
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

			XMLElemental contentconfig = XMLElemental.Load (File.OpenRead (Path.Combine (iContentDirectory, ".config")));
			XMLElemental additionalcontentconfig = XMLElemental.Load (File.OpenRead (Path.Combine (iContentDirectory, "additional.config")));
			foreach (XMLElemental child in contentconfig.GetAll().Concat(additionalcontentconfig.GetAll())) {
				switch (child.Attributes ["Type"]) {
				case "Tile":
					iTileNameIndex.Add (child.Attributes ["TextID"], (ushort)lvw_tiles.LargeImageList.Images.Count);
					iTileNameIndex.Add (child.Attributes ["NumID"], (ushort)lvw_tiles.LargeImageList.Images.Count);

					iTileIDIndex.Add (lvw_tiles.LargeImageList.Images.Count, (ushort)Convert.ToInt32 (child.Attributes ["NumID"]));

					lvw_tiles.Items.Add (new ListViewItem (child.Attributes ["Name"]) { ImageIndex = lvw_tiles.LargeImageList.Images.Count });
					lvw_tiles.LargeImageList.Images.Add (child.Attributes ["NumID"], new Bitmap (Path.Combine (iContentDirectory, child.Attributes ["Source"])));
					break;
				case "Overlay":
					iOverlayNameIndex.Add (child.Attributes ["TextID"], (ushort)lvw_overlays.LargeImageList.Images.Count);
					iOverlayNameIndex.Add (child.Attributes ["NumID"], (ushort)lvw_overlays.LargeImageList.Images.Count);

					iOverlayIDIndex.Add (lvw_overlays.LargeImageList.Images.Count, (ushort)Convert.ToInt32 (child.Attributes ["NumID"]));

					lvw_overlays.Items.Add (new ListViewItem (child.Attributes ["Name"]) { ImageKey = child.Attributes ["NumID"] });
					lvw_overlays.LargeImageList.Images.Add (child.Attributes ["NumID"], new Bitmap (Path.Combine (iContentDirectory, child.Attributes ["Source"])));
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
			InfoWindow infowinfow = new InfoWindow (new Values.Version (Assembly.GetExecutingAssembly ().GetName ().Version.ToString ()));
			infowinfow.ShowDialog ();
		}

		#endregion

		private void save ()
		{
			XMLElemental savefile = XMLElemental.EmptyRootElemental ("workfile");

			// save the maps
			foreach (Map map in iMaps) {
				savefile.AddChild (map.Save (iTileIDIndex, iOverlayIDIndex));
			}

			// save the character sets
			foreach (Character character in Enum.GetValues(typeof(Character))) {
				foreach (Set set in iSets[character]) {
					string refid = "___implementedimage" + (savefile.GetAll ((XMLElemental obj) => obj.Name.StartsWith ("___implementedimage")).Count / 4).ToString ();

					XMLElemental setElemental = new XMLElemental ("set");
					setElemental.Attributes.Add ("name", set.Name);
					setElemental.Attributes.Add ("character", character.ToString ());
					setElemental.Attributes.Add ("images", refid);
					setElemental.Attributes.Add ("description", set.Description);

					foreach (var kvpair in set.Parts) {
						setElemental.Attributes.Add ("name_" + kvpair.Key.ToString ().ToLower (), kvpair.Value.Name);
						setElemental.AddChild (kvpair.Key.ToString ().ToLower ()).Attributes = kvpair.Value.Attributes.ToDictionary (k => k.Key.ToString (), k => k.Value.ToString ());
						// save image in workfile
						savefile.AddChild (refid + "_" + kvpair.Key.ToString ().ToLower ()).Value = PrepareImage (kvpair.Value.Bitmap);
					}

					savefile.AddChild (setElemental);
				}
			}

			savefile.Sort ();

			using (StreamWriter writer = new StreamWriter (iCurrentFilePath, false)) {
				writer.WriteLine (savefile.Flush ());
				writer.Flush ();
			}

			MessageBox.Show ("Saving sucessfull", "Sucess!", MessageBoxButtons.OK);
		}

		private void load ()
		{
			iSets.Clear ();
			iMaps.Clear ();
			treeview_character.Nodes.Clear ();

			initCharacterEdit ();

			XMLElemental loadfile = XMLElemental.Load (File.OpenRead (iCurrentFilePath));

			foreach (XMLElemental child in loadfile.GetAll()) {
				if (child.Name == "map") {
					iMaps.Add (new Map (child, iTileNameIndex, iOverlayNameIndex));
					tscb_map_mapselect.Items.Add (iMaps [iMaps.Count - 1].Name);
				} else if (child.Name == "set") {
					Set loadedSet = new Set (child.Attributes ["name"]);
					loadedSet.Description = child.Attributes ["description"];
					foreach (XMLElemental part in child.GetAll()) {
						loadedSet.Parts.Add ((Slot)Enum.Parse (typeof(Slot), part.Name, true), new Set.Part (child.Attributes ["name_" + part.Name], part.Attributes.ToDictionary (k => (Attribute)Enum.Parse (typeof(Attribute), k.Key), k => k.Value), LoadBitmap (loadfile [child.Attributes ["images"] + "_" + part.Name].Value)));
					}
					iSets [(Character)Enum.Parse (typeof(Character), child.Attributes ["character"])].Add (loadedSet);

					treeview_character.Nodes [treeview_character.Nodes.IndexOfKey (Enum.Parse (typeof(Character), child.Attributes ["character"]).ToString ())].Nodes.Add (loadedSet.Name);
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

		private void tsmmi_exportraw_Click (object sender, EventArgs e)
		{
			folderBrowserDialog.ShowDialog ();
			string path = folderBrowserDialog.SelectedPath;

			string mappath = Path.Combine (path, "maps");
			string characterpath = Path.Combine (path, "character");
			string animationpath = Path.Combine (path, "animations");
			string entitypath = Path.Combine (path, "entitys");

			if (!Directory.Exists (mappath))
				Directory.CreateDirectory (mappath);
			if (!Directory.Exists (characterpath))
				Directory.CreateDirectory (characterpath);
			if (!Directory.Exists (animationpath))
				Directory.CreateDirectory (animationpath);
			if (!Directory.Exists (entitypath))
				Directory.CreateDirectory (entitypath);

			// convert all saved maps to the mapfiles
			foreach (Map map in iMaps) {
				XMLElemental mapelement = map.Save (iTileIDIndex, iOverlayIDIndex);
				File.Create (Path.Combine (mappath, map.Name + ".devmap")).Close ();
				File.WriteAllText (Path.Combine (mappath, map.Name + ".devmap"), mapelement.Flush ());
				File.Create (Path.Combine (mappath, map.Name + ".map")).Close ();
				File.WriteAllText (Path.Combine (mappath, map.Name + ".map"), AO.ZipString (mapelement.Flush ()));
			}

			MessageBox.Show ("Export successful");
		}

		private void Main_FormClosing (object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show ("Do you want to save the current state?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
			case DialogResult.Yes:
				tsmi_save_Click (this, EventArgs.Empty);
				break;
			case DialogResult.No:

				break;
			case DialogResult.Cancel:
				e.Cancel = true;
				break;
			}
		}

		#region characteredit

		private void initCharacterEdit ()
		{
			foreach (string name in Enum.GetNames(typeof(Character))) {
				treeview_character.Nodes.Add (name, name);
				iSets.Add ((Character)Enum.Parse (typeof(Character), name), new List<Set> ());
			}

			treeview_character.SelectedNode = treeview_character.Nodes [0];
		}

		public static string PrepareImage (Bitmap bitmap)
		{
			using (MemoryStream stream = new MemoryStream ()) {
				bitmap.Save (stream, System.Drawing.Imaging.ImageFormat.Png);
				stream.Close ();

				return Convert.ToBase64String (stream.ToArray ());
			}
		}

		public static Bitmap LoadBitmap (string imgstring)
		{
			using (MemoryStream stream = new MemoryStream (Convert.FromBase64String (imgstring))) {
				return (Bitmap)Bitmap.FromStream (stream);
			}
		}

		private void button_itembitmap_Click (object sender, EventArgs e)
		{
			if (selectedPart != unselected && ofd_imageedit.ShowDialog () == DialogResult.OK) {
				iSets [iSelectedCharacter] [iSelectedSet].Parts [supportedSlots [iSelectedCharacter] [selectedPart]].Bitmap = new Bitmap (ofd_imageedit.FileName);
				listview_slot.SmallImageList.Images [selectedPart] = new Bitmap (iSets [iSelectedCharacter] [iSelectedSet].Parts [supportedSlots [iSelectedCharacter] [selectedPart]].Bitmap, listview_slot.SmallImageList.ImageSize);
				listview_slot.Refresh ();
				updateSetInterface ();
			}
		}

		private void tsbutton_addset_Click (object sender, EventArgs e)
		{
			if (treeview_character.SelectedNode != null && treeview_character.Nodes.Contains (treeview_character.SelectedNode)) {
				iSets [iSelectedCharacter].Add (new Set (Interaction.InputBox ("Name?", "Add Set", "Name")));
				treeview_character.SelectedNode.Nodes.Add (iSets [iSelectedCharacter] [iSets [iSelectedCharacter].Count - 1].Name);
				switch (iSelectedCharacter) {
				case Character.Robot:
					iSets [iSelectedCharacter] [iSets [iSelectedCharacter].Count - 1].Parts.Add (Slot.Helmet, new Set.Part ("default_helmet"));
					iSets [iSelectedCharacter] [iSets [iSelectedCharacter].Count - 1].Parts.Add (Slot.Chestplate, new Set.Part ("default_chestplate"));
					iSets [iSelectedCharacter] [iSets [iSelectedCharacter].Count - 1].Parts.Add (Slot.Gloves, new Set.Part ("default_gloves"));
					iSets [iSelectedCharacter] [iSets [iSelectedCharacter].Count - 1].Parts.Add (Slot.Shoes, new Set.Part ("default_shoes"));
					break;
				}
			}
		}

		private void tsbutton_removeset_Click (object sender, EventArgs e)
		{
			if (treeview_character.SelectedNode != null && !treeview_character.Nodes.Contains (treeview_character.SelectedNode)) {
				iSets [iSelectedCharacter].RemoveAt (treeview_character.SelectedNode.Parent.Nodes.IndexOf (treeview_character.SelectedNode));
				treeview_character.SelectedNode.Parent.Nodes.RemoveAt (treeview_character.SelectedNode.Parent.Nodes.IndexOf (treeview_character.SelectedNode));
			}
		}

		private void updateSetInterface ()
		{
			if (iSelectedSet != unselected) {
				tstextbox_setname.Text = iSets [iSelectedCharacter] [iSelectedSet].Name;
				tstextbox_setname.Enabled = true;
				textbox_itemname.Text = iSets [iSelectedCharacter] [iSelectedSet].Parts [(Slot)Enum.Parse (typeof(Slot), listview_slot.SelectedItems [0].Text)].Name;
				textbox_itemname.Enabled = true;
				textbox_itemdescription.Text = iSets [iSelectedCharacter] [iSelectedSet].Description;
				textbox_itemdescription.Enabled = true;
				button_itembitmap.Enabled = true;
				attributelistview_item.Enabled = true;
				attributelistview_item.UpdateAttributes (iSets [iSelectedCharacter] [iSelectedSet].Parts [(Slot)Enum.Parse (typeof(Slot), listview_slot.SelectedItems [0].Text)].Attributes);
				picturebox_itempreview.BackgroundImage = iSets [iSelectedCharacter] [iSelectedSet].Parts [(Slot)Enum.Parse (typeof(Slot), listview_slot.SelectedItems [0].Text)].Bitmap;
				picturebox_itempreview.Enabled = true;
				listview_slot.Enabled = true;
			} else {
				tstextbox_setname.Text = iSelectedCharacter.ToString ();
				tstextbox_setname.Enabled = false;
				tstextbox_setname.Text = "";
				tstextbox_setname.Enabled = false;
				textbox_itemname.Text = "";
				textbox_itemname.Enabled = false;
				textbox_itemdescription.Text = "";
				textbox_itemdescription.Enabled = false;
				button_itembitmap.Enabled = false;
				attributelistview_item.Enabled = false;
				picturebox_itempreview.BackgroundImage = new Bitmap (1, 1);
				picturebox_itempreview.Enabled = false;
				listview_slot.Enabled = false;
			}
		}

		private void treeview_character_NodeMouseClick (object sender, TreeNodeMouseClickEventArgs e)
		{
			if (!treeview_character.Nodes.Contains (e.Node)) {
				iSelectedCharacter = (Character)Enum.Parse (typeof(Character), e.Node.Parent.Text);
				iSelectedSet = e.Node.Parent.Nodes.IndexOf (e.Node);
				listview_slot.Items.Clear ();
				listview_slot.SmallImageList.Images.Clear ();
				foreach (Slot slot in supportedSlots[iSelectedCharacter]) {
					if (iSets [iSelectedCharacter] [iSelectedSet].Parts.ContainsKey (slot)) {
						listview_slot.SmallImageList.Images.Add (new Bitmap (iSets [iSelectedCharacter] [iSelectedSet].Parts [slot].Bitmap, listview_slot.SmallImageList.ImageSize));
						listview_slot.Items.Add (slot.ToString (), listview_slot.SmallImageList.Images.Count - 1);
					} else {
						listview_slot.Items.Add (slot.ToString ());
					}
				}
				listview_slot.SelectedIndices.Add (0);
				listview_slot_SelectedIndexChanged (this, EventArgs.Empty);
			} else {
				iSelectedCharacter = (Character)Enum.Parse (typeof(Character), e.Node.Text);
				iSelectedSet = unselected;
			}

			updateSetInterface ();
		}

		private void Attributelistview_item_AttributeChanged (object sender, Dictionary<Attribute, string> e)
		{
			iSets [iSelectedCharacter] [iSelectedSet].Parts [(Slot)selectedPart].Attributes = e;
		}

		private void tstextbox_setname_KeyUp (object sender, KeyEventArgs e)
		{
			iSets [iSelectedCharacter] [iSelectedSet].Name = tstextbox_setname.Text;
			treeview_character.Nodes [treeview_character.Nodes.IndexOfKey (iSelectedCharacter.ToString ())].Nodes [iSelectedSet].Text = tstextbox_setname.Text;
		}

		private void textbox_itemname_KeyUp (object sender, KeyEventArgs e)
		{
			iSets [iSelectedCharacter] [iSelectedSet].Parts [(Slot)selectedPart].Name = tstextbox_setname.Text;
		}

		private void listview_slot_SelectedIndexChanged (object sender, EventArgs e)
		{
			if (listview_slot.SelectedIndices.Count > 0) {
				selectedPart = listview_slot.SelectedIndices [0];
				updateSetInterface ();
			}
		}

		private void listview_slot_EnabledChanged (object sender, EventArgs e)
		{
			selectedPart = unselected;
		}

		#endregion

		private void textbox_itemdescription_KeyUp (object sender, KeyEventArgs e)
		{
			iSets [iSelectedCharacter] [iSelectedSet].Description = textbox_itemdescription.Text;
		}
	}
}
