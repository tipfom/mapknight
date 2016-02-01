using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace mapKnight.ToolKit
{
	public class AttributeListView : ListView
	{
		[Browsable (true)]
		public event EventHandler<Dictionary<Attribute, string>> AttributeChanged;

		private TextBox iEditBox;
		private ListViewItem.ListViewSubItem ClickedAttribute;

		public AttributeListView () : base ()
		{
			this.MultiSelect = false;
			this.Dock = DockStyle.Fill;
			this.View = View.Details;

			this.Columns.Add (new ColumnHeader () { Text = "Attribute" });
			this.Columns.Add (new ColumnHeader () { Text = "Value" });

			this.MouseDown += HandleOnClick;
            
			iEditBox = new TextBox ();
			iEditBox.BorderStyle = BorderStyle.FixedSingle;
			iEditBox.Visible = false;
			iEditBox.KeyDown += HandleOnTextBoxKeyDown;
			iEditBox.Leave += HandleOnTexBoxLeave;

			this.Controls.Add (iEditBox);

			foreach (Attribute attribute in Enum.GetValues(typeof(Attribute))) {
				ListViewItem item = new ListViewItem (Enum.GetName (typeof(Attribute), attribute));
				item.SubItems.Add (new ListViewItem.ListViewSubItem () { Text = "" });

				this.Items.Add (item);
			}
		}

		public AttributeListView (Form handler, Dictionary<Attribute, string> values) : this ()
		{
			foreach (Attribute attribute in Enum.GetValues(typeof(Attribute))) {
				ListViewItem item = new ListViewItem (Enum.GetName (typeof(Attribute), attribute));
				if (values.ContainsKey (attribute))
					item.SubItems.Add (new ListViewItem.ListViewSubItem () { Text = values [attribute] });
				else
					item.SubItems.Add (new ListViewItem.ListViewSubItem () { Text = "" });

				this.Items.Add (item);
			}

			handler.Controls.Add (this);
		}

		public void UpdateAttributes (Dictionary<Attribute,string> updatedAttributes)
		{
			this.Items.Clear ();
			foreach (Attribute attribute in Enum.GetValues(typeof(Attribute))) {
				ListViewItem item = new ListViewItem (Enum.GetName (typeof(Attribute), attribute));
				if (updatedAttributes.ContainsKey (attribute))
					item.SubItems.Add (new ListViewItem.ListViewSubItem () { Text = updatedAttributes [attribute] });
				else
					item.SubItems.Add (new ListViewItem.ListViewSubItem () { Text = "" });

				this.Items.Add (item);
			}
		}

		private void HandleOnClick (object sender, EventArgs e)
		{
			Point mousePos = PointToClient (Control.MousePosition);
			ListViewHitTestInfo hitTest = HitTest (mousePos);
			if (hitTest.Item == null)
				return;

			if (hitTest.Item.SubItems.IndexOf (hitTest.SubItem) == 1) {
				this.ClickedAttribute = hitTest.SubItem;

				this.iEditBox.Width = hitTest.SubItem.Bounds.Width;
				this.iEditBox.Height = hitTest.SubItem.Bounds.Height;
				this.iEditBox.Top = this.Top + hitTest.SubItem.Bounds.Top + 2;
				this.iEditBox.Left = this.Left + hitTest.SubItem.Bounds.Left + 2;
				this.iEditBox.Text = hitTest.SubItem.Text;
				this.iEditBox.Visible = true;

				this.iEditBox.BringToFront ();
				this.iEditBox.Focus ();
			}
		}

		private void HandleOnTextBoxKeyDown (object sender, KeyEventArgs e)
		{
			switch (e.KeyData) {
			case Keys.Enter:
				FinishEditing (true);
				break;
			case Keys.Escape:
				FinishEditing (false);
				break;
			}
		}

		private void HandleOnTexBoxLeave (object sender, EventArgs e)
		{
			FinishEditing (true);
		}

		private void FinishEditing (bool completed)
		{
			if (completed) {
				this.ClickedAttribute.Text = this.iEditBox.Text;
				AttributeChanged (this, Collect ());
			}

			this.iEditBox.Visible = false;
			this.Focus ();
		}

		public Dictionary<Attribute,string> Collect ()
		{
			Dictionary<Attribute, string> result = new Dictionary<Attribute, string> ();

			foreach (ListViewItem entry in this.Items) {
				if (entry.SubItems [1].Text != "") {
					result.Add ((Attribute)Enum.Parse (typeof(Attribute), entry.Text), entry.SubItems [1].Text);
				}
			}

			return result;
		}
	}
}
