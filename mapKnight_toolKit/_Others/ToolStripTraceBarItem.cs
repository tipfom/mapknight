using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace mapKnight.ToolKit
{
	[ToolStripItemDesignerAvailability

  (ToolStripItemDesignerAvailability.ToolStrip |

	ToolStripItemDesignerAvailability.StatusStrip)]

	class ToolStripTraceBarItem : ToolStripControlHost
	{
		public TrackBar TrackBar { get { return (TrackBar)this.Control; } }

		public ToolStripTraceBarItem () : base (new TrackBar ())
		{

		}
	}
}
