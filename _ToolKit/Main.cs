using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Net;
using System.Net.Sockets;

using Microsoft.VisualBasic;

using XMLElemental = mapKnight.Basic.XMLElemental;
using AO = mapKnight.Basic.AO;

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
		private static Dictionary<Character, SlotSpriteData> slotImageData = new Dictionary<Character, SlotSpriteData> () { {Character.Robot,new SlotSpriteData (new Size (18, 33), new Dictionary<Slot, Rectangle> () {
						{ Slot.Helmet, new Rectangle (0, 0, 18, 18) },
						{ Slot.Chestplate, new Rectangle (0, 18, 16, 9) },
						{ Slot.Shoes, new Rectangle (0, 27, 8, 6) },
						{ Slot.Gloves, new Rectangle (8, 27, 6, 6) }
					})
			}
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
		private Slot selectedSlot = Slot.None;

		private AttributeListView attributelistview_item = new AttributeListView ();
		private ItemPictureBox picturebox_itempreview = new ItemPictureBox ();

		private Dictionary<Character, List<Animation>> animations = new Dictionary<Character, List<Animation>> ();
		private static Dictionary<Character, string[]> boundedPoints = new Dictionary<Character, string[]> () { {
				Character.Robot,
				new string[] {
					"helmet_0",
					"chestplate_0",
					"gloves_0",
					"gloves_1",
					"shoes_0",
					"shoes_1"
				}
			}
		};
		private static Dictionary<Character, Bitmap[]> defaultAnimBitmaps = new Dictionary<Character, Bitmap[]> () { {
				Character.Robot,
				new Bitmap[] {
					Properties.Resources.character_robot,
					Properties.Resources.character_robot_mirrored
				}
			}
		};
		private static Dictionary<Character, Dictionary<string, Rectangle>> defaultAnimBPRects = new Dictionary<Character, Dictionary<string, Rectangle>> () { {Character.Robot,new Dictionary<string, Rectangle> () { {
						"helmet_0",
						new Rectangle (0, 0, 18, 18)
					}, {
						"chestplate_0",
						new Rectangle (0, 18, 16, 9)
					}, {
						"gloves_0",
						new Rectangle (8, 27, 6, 6)
					}, {
						"gloves_1",
						new Rectangle (8, 27, 6, 6)
					}, {
						"shoes_0",
						new Rectangle (0, 27, 8, 6)
					}, {
						"shoes_1",
						new Rectangle (0, 27, 8, 6)
					}
				}
			}
		};
		private static Dictionary<Character, Dictionary<string, SizeF>> defaultAnimDrawRects = new Dictionary<Character, Dictionary<string, SizeF>> () { {Character.Robot,new Dictionary<string, SizeF> () { {
						"helmet_0",
						new SizeF (0.22f / 2f, 0.22f / 2f)
					}, {
						"chestplate_0",
						new SizeF (0.19f / 2f, 0.107f / 2f)
					}, {
						"gloves_0",
						new SizeF (0.063f / 2f, 0.063f / 2f)
					}, {
						"gloves_1",
						new SizeF (0.063f / 2f, 0.063f / 2f)
					}, {
						"shoes_0",
						new SizeF (0.078f / 2f, 0.053f / 2f)
					}, {
						"shoes_1",
						new SizeF (0.078f / 2f, 0.053f / 2f)
					}
				}
			}
		};


		private int selectedAnim = -1;
		private int selectedStep = -1;
		private string selectedStepPart = null;
		private Character selectedCharacter = Character.Robot;
		private bool currentlyClicked = false;

		private BufferedGraphicsContext bfgraphicscontext = BufferedGraphicsManager.Current;
		private BufferedGraphics bfgraphicsAnim;

#endregion

		public Main (string filepath)
		{
			InitializeComponent ();
			this.SetStyle (ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

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
			initAnim ();

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
					g.Graphics.FillRectangle (Brushes.Red, new RectangleF (new PointF ((iMaps [iSelectedMap].Spawn.X + 0.25f - hscrlbar_map.Value) * iTileSize + spltcntr_map.Right, (iMaps [iSelectedMap].Height - iMaps [iSelectedMap].Spawn.Y + 0.25f - vscrlbar_map.Value) * iTileSize + tlstrp_map.Bottom), new SizeF (iTileSize / 2, iTileSize / 2)));

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
			foreach (XMLElemental child in contentconfig.GetAll ().Concat (additionalcontentconfig.GetAll ())) {
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
						iMaps [iSelectedMap].Spawn = new Point (clickedTile.X, iMaps [iSelectedMap].Height - clickedTile.Y);
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
				if (iSelectedMap >= 0)
					iMaps [0].Undo ();
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
			InfoWindow infowinfow = new InfoWindow (new Basic.Version (Assembly.GetExecutingAssembly ().GetName ().Version.ToString ()));
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
			foreach (Character character in Enum.GetValues (typeof (Character))) {
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

			// save the anims
			foreach (KeyValuePair<Character, List<Animation>> characterAnims in animations) {
				XMLElemental characterElemental = new XMLElemental ("animcollection");
				characterElemental.Attributes.Add ("character", characterAnims.Key.ToString ().ToLower ());

				foreach (Animation anim in characterAnims.Value) {
					characterElemental.AddChild (anim.Flush ());
				}

				savefile.AddChild (characterElemental);
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
			animations.Clear ();
			treeView_anim.Nodes.Clear ();

			initAnim ();
			initCharacterEdit ();

			XMLElemental loadfile = XMLElemental.Load (File.OpenRead (iCurrentFilePath));

			foreach (XMLElemental child in loadfile.GetAll ()) {
				if (child.Name == "map") {
					iMaps.Add (new Map (child, iTileNameIndex, iOverlayNameIndex));
					tscb_map_mapselect.Items.Add (iMaps [iMaps.Count - 1].Name);
				} else if (child.Name == "set") {
					Set loadedSet = new Set (child.Attributes ["name"]);
					loadedSet.Description = child.Attributes ["description"];
					foreach (XMLElemental part in child.GetAll ()) {
						loadedSet.Parts.Add ((Slot)Enum.Parse (typeof(Slot), part.Name, true), new Set.Part (child.Attributes ["name_" + part.Name], part.Attributes.ToDictionary (k => (Attribute)Enum.Parse (typeof(Attribute), k.Key), k => k.Value), LoadBitmap (loadfile [child.Attributes ["images"] + "_" + part.Name].Value)));
					}
					iSets [(Character)Enum.Parse (typeof(Character), child.Attributes ["character"])].Add (loadedSet);

					treeview_character.Nodes [treeview_character.Nodes.IndexOfKey (Enum.Parse (typeof(Character), child.Attributes ["character"]).ToString ())].Nodes.Add (loadedSet.Name);
				} else if (child.Name == "animcollection") {
					Character character = (Character)Enum.Parse (typeof(Character), child.Attributes ["character"], true);
					foreach (XMLElemental animConfig in child.GetAll ()) {
						Animation loadedAnim = new Animation (animConfig);

						treeView_anim.Nodes [(int)character].Nodes.Add (new TreeNode ("Animation") { Name = "anim:1" });
						treeView_anim.Nodes [(int)character].Nodes [treeView_anim.Nodes [(int)character].Nodes.Count - 1].Nodes.Add (new TreeNode ("Default") { Name = "step:default" });
						treeView_anim.EnableAddNormalButton (treeView_anim.Nodes [(int)character].Nodes [treeView_anim.Nodes [(int)character].Nodes.Count - 1]);
						treeView_anim.EnableAddDefaultButton (treeView_anim.Nodes [(int)character].Nodes [treeView_anim.Nodes [(int)character].Nodes.Count - 1]);
						treeView_anim.EnableRemoveButton (treeView_anim.Nodes [(int)character].Nodes [treeView_anim.Nodes [(int)character].Nodes.Count - 1]);
						foreach (Tuple<int, Dictionary<string, float[]>> step in loadedAnim.steps) {
							treeView_anim.Nodes [(int)character].Nodes [treeView_anim.Nodes [(int)character].Nodes.Count - 1].Nodes.Add (new TreeNode ("Step") { Name = "step:1" });
							treeView_anim.EnableRemoveButton (treeView_anim.Nodes [(int)character].Nodes [treeView_anim.Nodes [(int)character].Nodes.Count - 1].Nodes [treeView_anim.Nodes [(int)character].Nodes [treeView_anim.Nodes [(int)character].Nodes.Count - 1].Nodes.Count - 1]);
						}
						animations [character].Add (loadedAnim);
					}
				}

				if (iMaps.Count > 0) {
					iSelectedMap = 0;
					tscb_map_mapselect.SelectedIndex = 0;

					updateMapVar ();
					renderMap ();
				}
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
			if (folderBrowserDialog.ShowDialog () == DialogResult.OK) {
				string path = folderBrowserDialog.SelectedPath;

				string mappath = Path.Combine (path, "maps");
				string setdatapath = Path.Combine (path, Path.Combine ("set", "data"));
				string setimagepath = Path.Combine (path, Path.Combine ("set", "image"));
				string animationpath = Path.Combine (path, "animations");
				string entitypath = Path.Combine (path, "entitys");

				if (!Directory.Exists (mappath))
					Directory.CreateDirectory (mappath);
				if (!Directory.Exists (setdatapath))
					Directory.CreateDirectory (setdatapath);
				if (!Directory.Exists (setimagepath))
					Directory.CreateDirectory (setimagepath);
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
					File.WriteAllText (Path.Combine (mappath, map.Name + ".map"), Basic.AO.ZipString (mapelement.Flush ()));
				}

				//convert all the sets
				foreach (KeyValuePair<Character, List<Set>> kvpair in iSets) {
					foreach (Set set in kvpair.Value) {
						XMLElemental setElemental = new XMLElemental ("set");
						Bitmap setBitmap = new Bitmap (slotImageData [kvpair.Key].SpriteSize.Width, slotImageData [kvpair.Key].SpriteSize.Height);
						setElemental.Attributes.Add ("name", set.Name);
						setElemental.AddChild ("description").Value = set.Description;

						foreach (KeyValuePair<Slot, Set.Part> partkvpair in set.Parts) {
							XMLElemental partElemental = new XMLElemental ("part");
							partElemental.Attributes.Add ("slot", partkvpair.Key.ToString ().ToLower ());
							partElemental.Attributes.Add ("name", partkvpair.Value.Name);

							foreach (KeyValuePair<Attribute, string> attributekvpair in partkvpair.Value.Attributes) {
								partElemental.AddChild (attributekvpair.Key.ToString ().ToLower ()).Attributes = new Dictionary<string, string> () { {
										"value",
										attributekvpair.Value
									}
								};
							}

							Graphics.FromImage (setBitmap).DrawImage (partkvpair.Value.Bitmap, slotImageData [kvpair.Key].SlotPositions [partkvpair.Key]);

							setElemental.AddChild (partElemental);
						}

						setBitmap.Save (Path.ChangeExtension (Path.Combine (setimagepath, set.Name), "png"), ImageFormat.Png);
						File.Create (Path.ChangeExtension (Path.Combine (setdatapath, set.Name), "set")).Close ();
						File.WriteAllText (Path.ChangeExtension (Path.Combine (setdatapath, set.Name), "set"), setElemental.Flush ());

					}
				}

				MessageBox.Show ("Export successful");
			}
		}

		private void Main_FormClosing (object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show ("Do you want to save the current state?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
			case DialogResult.Yes:
				tsmi_save_Click (this, EventArgs.Empty);
				Disconnect ();
				break;
			case DialogResult.No:
				Disconnect ();
				break;
			case DialogResult.Cancel:
				e.Cancel = true;
				break;
			}
		}

#region characteredit

		private void initCharacterEdit ()
		{
			foreach (string name in Enum.GetNames (typeof (Character))) {
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

		private Bitmap resizeBitmapForListview (Bitmap bitmap)
		{
			Bitmap result = new Bitmap (listview_slot.SmallImageList.ImageSize.Width, listview_slot.SmallImageList.ImageSize.Height);
			using (Graphics g = Graphics.FromImage (result)) {
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				g.PixelOffsetMode = PixelOffsetMode.Half;
				g.DrawImage (bitmap, new Rectangle (new Point (0, 0), listview_slot.SmallImageList.ImageSize));
			}
			return result;
		}

		private void button_itembitmap_Click (object sender, EventArgs e)
		{
			if (selectedSlot != Slot.None && ofd_imageedit.ShowDialog () == DialogResult.OK) {
				iSets [iSelectedCharacter] [iSelectedSet].Parts [supportedSlots [iSelectedCharacter] [listview_slot.Items.IndexOfKey (selectedSlot.ToString ())]].Bitmap = new Bitmap (ofd_imageedit.FileName);
				listview_slot.SmallImageList.Images [listview_slot.Items.IndexOfKey (selectedSlot.ToString ())] = resizeBitmapForListview (iSets [iSelectedCharacter] [iSelectedSet].Parts [supportedSlots [iSelectedCharacter] [listview_slot.Items.IndexOfKey (selectedSlot.ToString ())]].Bitmap);
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
				iSelectedSet = unselected;
				selectedSlot = Slot.None;
				updateSetInterface ();
			}
		}

		private void updateSetInterface ()
		{
			if (iSelectedSet != unselected) {
				tstextbox_setname.Text = iSets [iSelectedCharacter] [iSelectedSet].Name;
				tstextbox_setname.Enabled = true;
				textbox_itemname.Text = iSets [iSelectedCharacter] [iSelectedSet].Parts [selectedSlot].Name;
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
				textbox_itemname.Text = "";
				textbox_itemname.Enabled = false;
				textbox_itemdescription.Text = "";
				textbox_itemdescription.Enabled = false;
				button_itembitmap.Enabled = false;
				attributelistview_item.Enabled = false;
				picturebox_itempreview.BackgroundImage = new Bitmap (1, 1);
				picturebox_itempreview.Enabled = false;
				listview_slot.Enabled = false;
				listview_slot.Items.Clear ();
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
						listview_slot.SmallImageList.Images.Add (resizeBitmapForListview (iSets [iSelectedCharacter] [iSelectedSet].Parts [slot].Bitmap));
						listview_slot.Items.Add (slot.ToString (), slot.ToString (), listview_slot.SmallImageList.Images.Count - 1);
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
			iSets [iSelectedCharacter] [iSelectedSet].Parts [selectedSlot].Attributes = e;
		}

		private void tstextbox_setname_KeyUp (object sender, KeyEventArgs e)
		{
			iSets [iSelectedCharacter] [iSelectedSet].Name = tstextbox_setname.Text;
			treeview_character.Nodes [treeview_character.Nodes.IndexOfKey (iSelectedCharacter.ToString ())].Nodes [iSelectedSet].Text = tstextbox_setname.Text;
		}

		private void textbox_itemname_KeyUp (object sender, KeyEventArgs e)
		{
			iSets [iSelectedCharacter] [iSelectedSet].Parts [selectedSlot].Name = textbox_itemname.Text;
		}

		private void listview_slot_SelectedIndexChanged (object sender, EventArgs e)
		{
			if (listview_slot.SelectedIndices.Count > 0) {
				selectedSlot = (Slot)Enum.Parse (typeof(Slot), listview_slot.Items [listview_slot.SelectedIndices [0]].Text);
				updateSetInterface ();
			}
		}

		private void listview_slot_EnabledChanged (object sender, EventArgs e)
		{
			selectedSlot = Slot.None;
		}

		private void textbox_itemdescription_KeyUp (object sender, KeyEventArgs e)
		{
			iSets [iSelectedCharacter] [iSelectedSet].Description = textbox_itemdescription.Text;
		}

#endregion

#region animedit

		private void initAnim ()
		{
			foreach (string character in Enum.GetNames (typeof (Character))) {
				treeView_anim.Nodes.Add (new TreeNode (character) { Name = "character:" + character });
				treeView_anim.EnableAddNormalButton (treeView_anim.Nodes [treeView_anim.Nodes.Count - 1]);

				animations.Add ((Character)Enum.Parse (typeof(Character), character), new List<Animation> ());
			}

			panel_anim_editstep.MouseWheel += Panel_anim_editstep_MouseWheel;
		}

		private void treeView_anim_NodeMouseClick (object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node != null) {
				switch (e.Node.Name.Split (new char[] { ':' }) [0]) {
				case "character":
					panel_anim_editanim.Visible = false;
					panel_anim_editstep.Visible = false;
					selectedCharacter = (Character)Enum.Parse (typeof(Character), e.Node.Text);
					selectedAnim = unselected;
					selectedStep = unselected;
					break;
				case "anim":
					panel_anim_editanim.Visible = true;
					panel_anim_editanim.BringToFront ();
					panel_anim_editstep.Visible = false;
					selectedCharacter = (Character)Enum.Parse (typeof(Character), e.Node.Parent.Text);
					selectedAnim = e.Node.Index;
					selectedStep = unselected;

					button_anim_abortable.Text = animations [selectedCharacter] [selectedAnim].Abortable.ToString ().ToLower ();
					button_anim_loopable.Text = animations [selectedCharacter] [selectedAnim].Loopable.ToString ().ToLower ();
					textBox_anim_action.Text = animations [selectedCharacter] [selectedAnim].Action;
					break;
				case "step":
					panel_anim_editanim.Visible = false;
					panel_anim_editstep.Visible = true;
					panel_anim_editstep.BringToFront ();
					selectedCharacter = (Character)Enum.Parse (typeof(Character), e.Node.Parent.Parent.Text);
					selectedAnim = e.Node.Parent.Index;
					selectedStep = e.Node.Index;

					numericUpDown_anim_time.Value = animations [selectedCharacter] [selectedAnim].GetTime (selectedStep);

					panel_anim_editstep.Refresh ();
					break;
				}
			}
		}

		private void splitContainer5_Panel2_SizeChanged (object sender, EventArgs e)
		{
			panel_anim_editanim.Location = new Point (splitContainer_anim.Location.X + splitContainer_anim.Panel1.Width, splitContainer_anim.Location.Y);
			panel_anim_editanim.Size = splitContainer_anim.Panel2.Size;
			panel_anim_editstep.Location = new Point (splitContainer_anim.Location.X + splitContainer_anim.Panel1.Width, splitContainer_anim.Location.Y);
			panel_anim_editstep.Size = splitContainer_anim.Panel2.Size;

			this.bfgraphicsAnim = bfgraphicscontext.Allocate (panel_anim_editstep.CreateGraphics (), new Rectangle (0, 0, panel_anim_editstep.Width, panel_anim_editstep.Height));
		}

		private void treeView_anim_OnAddNormalButtonClicked (object sender, TreeNode e)
		{
			if (e != null) {
				switch (e.Name.Split (new char[] { ':' }) [0]) {
				case "character":
					e.Nodes.Add (new TreeNode ("Animation") { Name = "anim:1" });
					e.Nodes [e.Nodes.Count - 1].Nodes.Add (new TreeNode ("Default") { Name = "step:default" });
					treeView_anim.EnableAddNormalButton (e.Nodes [e.Nodes.Count - 1]);
					treeView_anim.EnableAddDefaultButton (e.Nodes [e.Nodes.Count - 1]);
					treeView_anim.EnableRemoveButton (e.Nodes [e.Nodes.Count - 1]);
					animations [(Character)Enum.Parse (typeof(Character), e.Text)].Add (new Animation (boundedPoints [(Character)Enum.Parse (typeof(Character), e.Text)]));
					break;
				case "anim":
					e.Nodes.Add (new TreeNode ("Step") { Name = "step:1" });
					treeView_anim.EnableRemoveButton (e.Nodes [e.Nodes.Count - 1]);
					animations [(Character)Enum.Parse (typeof(Character), e.Parent.Text)] [e.Parent.Nodes.IndexOf (e)].AddStep ();
					break;
				}
			}
		}

		private void treeView_anim_OnAddDefaultButtonClicked (object sender, TreeNode e)
		{
			if (e != null) {
				switch (e.Name.Split (new char[] { ':' }) [0]) {
				case "anim":
					e.Nodes.Add (new TreeNode ("Step") { Name = "step:1" });
					treeView_anim.EnableRemoveButton (e.Nodes [e.Nodes.Count - 1]);
					animations [(Character)Enum.Parse (typeof(Character), e.Parent.Text)] [e.Parent.Nodes.IndexOf (e)].AddStepDefault ();
					break;
				}
			}
		}

		private void treeView_anim_OnRemoveButtonClicked (object sender, TreeNode e)
		{
			if (e != null) {
				switch (e.Name.Split (new char[] { ':' }) [0]) {
				case "anim":
					animations [(Character)Enum.Parse (typeof(Character), e.Parent.Text)].RemoveAt (e.Parent.Nodes.IndexOf (e));
					e.Parent.Nodes.Remove (e);
					break;
				case "step":
                    animations [(Character)Enum.Parse (typeof(Character), e.Parent.Parent.Text)] [e.Parent.Parent.Nodes.IndexOf (e.Parent)].RemoveStep (e.Parent.Nodes.IndexOf (e));
					e.Parent.Nodes.Remove (e);
					break;
				}
			}
		}

		private void panel_anim_editstep_Paint (object sender, PaintEventArgs e)
		{
			if (tbctrl_main.SelectedIndex == 2) {
				if (panel_anim_editstep.Visible == true) {
					// render step
					bfgraphicsAnim.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					bfgraphicsAnim.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					bfgraphicsAnim.Graphics.Clear (Color.White);

					foreach (KeyValuePair<string, float[]> kvpair in animations[selectedCharacter][selectedAnim].GetStep (selectedStep)) {
						Rectangle drawRect = getAnimRectangle (kvpair);
						if (kvpair.Value [2] % 360f != 0) {
							bfgraphicsAnim.Graphics.TranslateTransform (drawRect.X + drawRect.Width / 2, drawRect.Y + drawRect.Height / 2);
							//rotate
							bfgraphicsAnim.Graphics.RotateTransform (kvpair.Value [2]);
							bfgraphicsAnim.Graphics.TranslateTransform (-drawRect.X - drawRect.Width / 2, -drawRect.Y - drawRect.Height / 2);
							//move image back
							//bfgraphicsAnim.Graphics.TranslateTransform ( -(float)(defaultAnimBPRects[selectedCharacter][kvpair.Key].Location.X + defaultAnimBPRects[selectedCharacter][kvpair.Key].Width / 2), -(float)(defaultAnimBPRects[selectedCharacter][kvpair.Key].Y + defaultAnimBPRects[selectedCharacter][kvpair.Key].Height / 2));
							//draw passed in image onto graphics object

							bfgraphicsAnim.Graphics.DrawImage (defaultAnimBitmaps [selectedCharacter] [(int)kvpair.Value [3]], drawRect, defaultAnimBPRects [selectedCharacter] [kvpair.Key], GraphicsUnit.Pixel);
							bfgraphicsAnim.Graphics.DrawLine (Pens.Red, new Point (drawRect.X - 10 + drawRect.Width / 2, drawRect.Y + drawRect.Height / 2), new Point (drawRect.X + 10 + drawRect.Width / 2, drawRect.Y + drawRect.Height / 2));
							bfgraphicsAnim.Graphics.DrawLine (Pens.Red, new Point (drawRect.X + drawRect.Width / 2, drawRect.Y - 10 + drawRect.Height / 2), new Point (drawRect.X + drawRect.Width / 2, drawRect.Y + 10 + drawRect.Height / 2));

							bfgraphicsAnim.Graphics.ResetTransform ();
						} else {
							bfgraphicsAnim.Graphics.DrawImage (defaultAnimBitmaps [selectedCharacter] [(int)kvpair.Value [3]], drawRect, defaultAnimBPRects [selectedCharacter] [kvpair.Key], GraphicsUnit.Pixel);
							bfgraphicsAnim.Graphics.DrawLine (Pens.Red, new Point (drawRect.X - 10 + drawRect.Width / 2, drawRect.Y + drawRect.Height / 2), new Point (drawRect.X + 10 + drawRect.Width / 2, drawRect.Y + drawRect.Height / 2));
							bfgraphicsAnim.Graphics.DrawLine (Pens.Red, new Point (drawRect.X + drawRect.Width / 2, drawRect.Y - 10 + drawRect.Height / 2), new Point (drawRect.X + drawRect.Width / 2, drawRect.Y + 10 + drawRect.Height / 2));
						}

					}

					bfgraphicsAnim.Graphics.DrawLine (Pens.Black, new Point (0, panel_anim_editstep.Height / 2), new Point (panel_anim_editstep.Width, panel_anim_editstep.Height / 2));
					bfgraphicsAnim.Graphics.DrawLine (Pens.Black, new Point (panel_anim_editstep.Width / 2, 0), new Point (panel_anim_editstep.Width / 2, panel_anim_editstep.Height));

					// bottom line
					bfgraphicsAnim.Graphics.DrawLine (Pens.Green, new Point (0, panel_anim_editstep.Height / 2 + (int)(panel_anim_editstep.Height / 20)), new Point (panel_anim_editstep.Width, panel_anim_editstep.Height / 2 + (int)(panel_anim_editstep.Height / 20)));
					bfgraphicsAnim.Render ();

				}
			}
		}

		private Rectangle getAnimRectangle (KeyValuePair<string, float[]> kvpair)
		{
			Size scaleSize = new Size (Math.Min (panel_anim_editstep.Width / 2, (panel_anim_editstep.Height - splitter2.Height) / 2), Math.Min (panel_anim_editstep.Width / 2, (panel_anim_editstep.Height - splitter2.Height) / 2));
			Point pointZero = new Point (panel_anim_editstep.Width / 2, panel_anim_editstep.Height / 2);

			return new Rectangle (new Point ((int)(pointZero.X + kvpair.Value [0] * (float)scaleSize.Width - defaultAnimDrawRects [selectedCharacter] [kvpair.Key].Width * (float)scaleSize.Width), pointZero.Y + (int)(-kvpair.Value [1] * (float)scaleSize.Height - defaultAnimDrawRects [selectedCharacter] [kvpair.Key].Height * (float)scaleSize.Height)), new Size ((int)(2f * defaultAnimDrawRects [selectedCharacter] [kvpair.Key].Width * (float)scaleSize.Width), (int)(2f * defaultAnimDrawRects [selectedCharacter] [kvpair.Key].Height * (float)scaleSize.Height)));
		}

		private void button_anim_abortable_Click (object sender, EventArgs e)
		{
			if (selectedAnim != unselected) {
				animations [selectedCharacter] [selectedAnim].Abortable = !animations [selectedCharacter] [selectedAnim].Abortable;
				button_anim_abortable.Text = animations [selectedCharacter] [selectedAnim].Abortable.ToString ().ToLower ();
			}
		}

		private void button_anim_loopable_Click (object sender, EventArgs e)
		{
			if (selectedAnim != unselected) {
				animations [selectedCharacter] [selectedAnim].Loopable = !animations [selectedCharacter] [selectedAnim].Loopable;
				button_anim_loopable.Text = animations [selectedCharacter] [selectedAnim].Loopable.ToString ().ToLower ();
			}
		}

		private void textBox_anim_action_TextChanged (object sender, EventArgs e)
		{
			if (selectedAnim != unselected) {
				animations [selectedCharacter] [selectedAnim].Action = textBox_anim_action.Text;
			}
		}

		private void button_anim_play_Click (object sender, EventArgs e)
		{
			if (connected) {
				byte[] animdata = Encoding.UTF8.GetBytes (AO.ZipString (animations [selectedCharacter] [selectedAnim].Flush ().Flush ()));
				List<byte> data = new List<byte> () { 1, 0 };
				data.AddRange (BitConverter.GetBytes (animdata.Length));
				send (data.ToArray ());
				send (animdata, true);
			} else {
				MessageBox.Show ("please go to the terminal section and connect the toolkit to the device");
			}
		}

		private void button_anim_pause_Click (object sender, EventArgs e)
		{

		}

		private void button_anim_repeat_Click (object sender, EventArgs e)
		{

		}

		private void trackBar_anim_progress_ValueChanged (object sender, EventArgs e)
		{

		}

		private void Panel_anim_editstep_MouseWheel (object sender, MouseEventArgs e)
		{
			if (selectedStepPart != null) {
				animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart].SetValue (animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart] [2] + e.Delta / 10, 2);
				numericUpDown_anim_rot.Value = (int)(animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart] [2]);
			}
		}

		private void panel_anim_editstep_MouseDown (object sender, MouseEventArgs e)
		{
			PointF relativePosition = new PointF (Math.Max (-1f, Math.Min (1f, (float)(e.Location.X - panel_anim_editstep.Width / 2) / (float)(panel_anim_editstep.Height / 2))), Math.Max (-1f, Math.Min (1f, -(float)(e.Location.Y - panel_anim_editstep.Height / 2) / (float)(Math.Min (panel_anim_editstep.Width, panel_anim_editstep.Height) / 2))));
			foreach (KeyValuePair<string, float[]> part in animations[selectedCharacter][selectedAnim].GetStep (selectedStep)) {
				if (stepCollides (part, relativePosition)) {
					selectedStepPart = part.Key;
					currentlyClicked = true;

					numericUpDown_anim_x.Value = (int)(part.Value [0] * 100f);
					numericUpDown_anim_y.Value = (int)(part.Value [1] * 100f);
					numericUpDown_anim_rot.Value = (int)(part.Value [2]);
					button_anim_mirrored.Text = (part.Value [3] == 1f ? true : false).ToString ().ToLower ();
				}
			}
		}

		private void numericUpDown_anim_time_ValueChanged (object sender, EventArgs e)
		{
			animations [selectedCharacter] [selectedAnim].SetTime ((int)numericUpDown_anim_time.Value, selectedStep);
		}

		private void numericUpDown_anim_x_ValueChanged (object sender, EventArgs e)
		{
			animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart].SetValue (((float)numericUpDown_anim_x.Value / 100f), 0);
			panel_anim_editstep.Refresh ();
		}

		private void numericUpDown_anim_y_ValueChanged (object sender, EventArgs e)
		{
			animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart].SetValue (((float)numericUpDown_anim_y.Value / 100f), 1);
			panel_anim_editstep.Refresh ();
		}

		private void numericUpDown_anim_rot_ValueChanged (object sender, EventArgs e)
		{
			animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart].SetValue ((float)(numericUpDown_anim_rot.Value), 2);
			panel_anim_editstep.Refresh ();
		}

		private void button_anim_mirrored_Click (object sender, EventArgs e)
		{
			animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart].SetValue (1f - animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart] [3], 3);
			button_anim_mirrored.Text = (animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart] [3] == 1f ? true : false).ToString ().ToLower ();
			panel_anim_editstep.Refresh ();
		}

		private bool stepCollides (KeyValuePair<string, float[]> kvpair, PointF relativePosition)
		{
			if (kvpair.Value [0] - defaultAnimDrawRects [selectedCharacter] [kvpair.Key].Width < relativePosition.X &&
			    kvpair.Value [0] + defaultAnimDrawRects [selectedCharacter] [kvpair.Key].Width > relativePosition.X &&
			    kvpair.Value [1] - defaultAnimDrawRects [selectedCharacter] [kvpair.Key].Height < relativePosition.Y &&
			    kvpair.Value [1] + defaultAnimDrawRects [selectedCharacter] [kvpair.Key].Height > relativePosition.Y)
				return true;
			return false;
		}

		private void panel_anim_editstep_MouseUp (object sender, MouseEventArgs e)
		{
			currentlyClicked = false;
		}

		private void panel_anim_editstep_MouseMove (object sender, MouseEventArgs e)
		{
			if (currentlyClicked) {
				Point rasteredLocation = new Point (e.Location.X / 10, e.Location.Y / 10);
				rasteredLocation.X *= 10;
				rasteredLocation.Y *= 10;

				PointF newPosition = new PointF (Math.Max (-1f, Math.Min (1f, (float)(rasteredLocation.X - (int)(panel_anim_editstep.Width / 10) * 10 / 2) / (float)(Math.Min (panel_anim_editstep.Width, panel_anim_editstep.Height) / 2))), Math.Max (-1f, Math.Min (1f, -(float)(rasteredLocation.Y - (int)(panel_anim_editstep.Height / 10) * 10 / 2) / (float)(Math.Min (panel_anim_editstep.Width, panel_anim_editstep.Height) / 2))));

				animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart].SetValue (newPosition.X, 0);
				animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart].SetValue (newPosition.Y, 1);

				panel_anim_editstep.Refresh ();

				numericUpDown_anim_x.Value = (int)(animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart] [0] * 100f);
				numericUpDown_anim_y.Value = (int)(animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart] [1] * 100f);
				numericUpDown_anim_rot.Value = (int)(animations [selectedCharacter] [selectedAnim].GetStep (selectedStep) [selectedStepPart] [2]);
			}
		}

#endregion

#region Terminal

		private const int bufferSize = 1024;
		private const int port = 1337;

		private byte[] buffer = new byte[bufferSize];

		private IPEndPoint ipEndPoint;
		private Socket clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private bool connected = false;

		private void button_connect_Click (object sender, EventArgs e)
		{
			if (!connected) {
				// connect function
				try {
					IPAddress mobilephoneip;
					if (IPAddress.TryParse (textbox_ip.Text, out mobilephoneip)) {
						ipEndPoint = new IPEndPoint (mobilephoneip, port);
						clientSocket.BeginConnect (ipEndPoint, new AsyncCallback (connectCallback), clientSocket);
						label_connectionstate.Text = "Status : waiting for connection";
						setTerminalCursor (Cursors.WaitCursor);
						updateTerminalToolStrip (false, false);
					} else {
						MessageBox.Show ("please enter a valid ip");
					}
				} catch (Exception ex) {
					MessageBox.Show (ex.ToString ());
				}
			} else {
				//disconnect function
				Disconnect ();
			}
		}

		private void connectCallback (IAsyncResult ar)
		{
			try {
				// casts the object parsed to the .BeginConnect method and calls the method to complete the connection-phase
				if (((Socket)ar.AsyncState).Connected) {
					((Socket)ar.AsyncState).EndConnect (ar);
					((Socket)ar.AsyncState).BeginReceive (buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback (receiveCallback), ((Socket)ar.AsyncState));
					connected = true;
					label_connectionstate.Text = "Status : connected";
					addLogItem ("< server connected");
					setTerminalCursor (Cursors.Arrow);
					updateTerminalInterface ();
					button_connect.Text = "Disconnect";
				} else {
					clientSocket.BeginConnect (ipEndPoint, new AsyncCallback (connectCallback), clientSocket);
				}
			} catch (SocketException se) {
				((Socket)ar.AsyncState).EndConnect (ar);
				clientSocket.BeginConnect (ipEndPoint, new AsyncCallback (connectCallback), clientSocket);
			} catch (Exception ex) {
				MessageBox.Show (ex.ToString ());
			}
		}

		delegate void SetTerminalCursorDelegate (Cursor cursor);

		private void setTerminalCursor (Cursor cursor)
		{
			if (this.tbpg_terminal.InvokeRequired) {
				SetTerminalCursorDelegate _delegate = new SetTerminalCursorDelegate (setTerminalCursor);
				this.Invoke (_delegate, cursor);
			} else {
				// im selben thread wie die tabpage
				this.tbpg_terminal.Cursor = cursor;
			}
		}

		delegate void UpdateTerminalInterfaceDelegate ();

		private void updateTerminalInterface ()
		{
			if (this.textbox_commandinput.InvokeRequired || this.listbox_commands.InvokeRequired || this.listbox_log.InvokeRequired) {
				UpdateTerminalInterfaceDelegate _delegate = new UpdateTerminalInterfaceDelegate (updateTerminalInterface);
				this.Invoke (_delegate);
			} else {
				// im selben thread wie die tabpage
				this.textbox_commandinput.Enabled = connected;
				this.listbox_commands.Enabled = connected;
				this.listbox_log.Enabled = connected;
			}

		}

		delegate void UpdateTerminalToolStripDelegate (bool buttonenabled, bool textboxenabled);

		private void updateTerminalToolStrip (bool buttonenabled, bool textboxenabled)
		{
			if (this.toolStrip2.InvokeRequired) {
				UpdateTerminalToolStripDelegate _delegate = new UpdateTerminalToolStripDelegate (updateTerminalToolStrip);
				this.Invoke (_delegate, buttonenabled, textboxenabled);
			} else {
				// im selben thread wie die tabpage
				this.button_connect.Enabled = buttonenabled;
				this.textbox_ip.Enabled = textboxenabled;
			}
		}

		delegate void AddLogItemDelegate (object item);

		private void addLogItem (object item)
		{
			if (this.listbox_log.InvokeRequired) {
				AddLogItemDelegate _delegate = new AddLogItemDelegate (addLogItem);
				this.Invoke (_delegate, item);
			} else {
				// im selben thread wie die tabpage
				this.listbox_log.Items.Add (item);
			}
		}

		private void receiveCallback (IAsyncResult ar)
		{
			try {
				int bytesReceived = ((Socket)ar.AsyncState).EndReceive (ar);
				if (bytesReceived == 0)
					return; //client diconnected
				if (buffer [0] == 255) {
					label_connectionstate.Text = "Status : server disconnected! trying to reconnect!";
					addLogItem ("< server disconnected");
					connected = false;
					updateTerminalToolStrip (false, false);
					clientSocket.Close ();
					clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					clientSocket.BeginConnect (ipEndPoint, new AsyncCallback (connectCallback), clientSocket);
					updateTerminalInterface ();
				} else {
					addLogItem ("< " + Encoding.UTF8.GetString (buffer, 0, bytesReceived));
					clientSocket.BeginReceive (buffer, 0, bufferSize, SocketFlags.None, new AsyncCallback (receiveCallback), clientSocket);
				}
			} catch (Exception ex) {
				MessageBox.Show (ex.ToString ());
			}
		}

		private void send (byte[] bytes, bool force = false)
		{
			if (connected) {
				try {
					if (bytes.Length < bufferSize || force) {
						clientSocket.BeginSend (bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback (sendingCallback), clientSocket);
					} else {
						MessageBox.Show ("the command is to long", "to long", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				} catch (Exception ex) {
					MessageBox.Show (ex.ToString ());
				}
			}
		}

		private void sendingCallback (IAsyncResult ar)
		{
			try {
				// returns the amount of bytes send
				int bytesSend = ((Socket)ar.AsyncState).EndSend (ar);
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
			}
		}

		private void Disconnect ()
		{
			send (new byte[] { 255 });
			connected = false;
			updateTerminalInterface ();
			clientSocket.Close ();
			updateTerminalToolStrip (true, true);
		}

		private void textbox_commandinput_KeyDown (object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && textbox_commandinput.Text != "__:disconnect:__") {
				// send(textbox_commandinput.Text);
				addLogItem ("> " + textbox_commandinput.Text);
				textbox_commandinput.Text = "";
			}
		}

#endregion
	}
}
