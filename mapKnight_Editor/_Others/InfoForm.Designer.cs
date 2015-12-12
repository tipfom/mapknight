namespace mapKnight.ToolKit
{
	partial class InfoWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.versionlabel = new System.Windows.Forms.Label ();
			this.label2 = new System.Windows.Forms.Label ();
			this.compiledonlabel = new System.Windows.Forms.Label ();
			this.label4 = new System.Windows.Forms.Label ();
			this.SuspendLayout ();
			// 
			// versionlabel
			// 
			this.versionlabel.AutoSize = true;
			this.versionlabel.Location = new System.Drawing.Point (12, 9);
			this.versionlabel.Name = "versionlabel";
			this.versionlabel.Size = new System.Drawing.Size (51, 13);
			this.versionlabel.TabIndex = 0;
			this.versionlabel.Text = "Version : ";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point (12, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (110, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "VersionState : internal";
			// 
			// compiledonlabel
			// 
			this.compiledonlabel.AutoSize = true;
			this.compiledonlabel.Location = new System.Drawing.Point (12, 62);
			this.compiledonlabel.Name = "compiledonlabel";
			this.compiledonlabel.Size = new System.Drawing.Size (74, 13);
			this.compiledonlabel.TabIndex = 2;
			this.compiledonlabel.Text = "Compiled on : ";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point (12, 86);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size (89, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Created by tipfom";
			// 
			// InfoWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (320, 113);
			this.Controls.Add (this.label4);
			this.Controls.Add (this.compiledonlabel);
			this.Controls.Add (this.label2);
			this.Controls.Add (this.versionlabel);
			this.MaximumSize = new System.Drawing.Size (336, 152);
			this.MinimumSize = new System.Drawing.Size (336, 152);
			this.Name = "InfoWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "mapKnight ToolKit - Info";
			this.Load += new System.EventHandler (this.InfoWindow_Load);
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.Label versionlabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label compiledonlabel;
		private System.Windows.Forms.Label label4;
	}
}