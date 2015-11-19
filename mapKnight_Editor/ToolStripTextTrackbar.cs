using System;
using System.Windows.Forms;

namespace mapKnight_Editor
{
    class ToolStripTextTrackbar
    {
        public ToolStripTraceBarItem TrackBar;
        public ToolStripStatusLabel Label;
        public ToolStripSeparator Separator;
        
        public ToolStripTextTrackbar(ToolStrip holder,int minimum, int maximum, string text)
        {
            TrackBar = new ToolStripTraceBarItem();
            TrackBar.TrackBar.Maximum = maximum;
            TrackBar.TrackBar.Minimum = minimum;
            TrackBar.TrackBar.Value = 0;           
            TrackBar.TrackBar.AutoSize = false;
            TrackBar.TrackBar.Height = holder.Height;
            TrackBar.TrackBar.TickStyle = TickStyle.None;
            TrackBar.TrackBar.RightToLeft = RightToLeft.No;

            Label = new ToolStripStatusLabel(text);

            Separator = new ToolStripSeparator();

            holder.Items.Add(Separator);
            holder.Items.Add(TrackBar);
            holder.Items.Add(Label);
        }
    }
}
