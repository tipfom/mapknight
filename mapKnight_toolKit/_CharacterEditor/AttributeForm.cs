using System.Collections.Generic;
using System.Windows.Forms;

namespace mapKnight.ToolKit
{
	public partial class AttributeForm : Form
	{
		AttributeListView attrlistview;

		public AttributeForm (string partname, Dictionary<Attribute,string> attributes)
		{
			InitializeComponent ();

			this.Text = "Editing attributes of " + partname;

			attrlistview = new AttributeListView (this, attributes);
            
		}

		public Dictionary<Attribute, string> Collect ()
		{
			return attrlistview.Collect ();
		}
	}
}
