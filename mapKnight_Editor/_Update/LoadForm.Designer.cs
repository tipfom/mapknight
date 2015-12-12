namespace mapKnight.ToolKit
{
	partial class LoadForm
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
			this.label_version = new System.Windows.Forms.Label ();
			this.label2 = new System.Windows.Forms.Label ();
			this.label1 = new System.Windows.Forms.Label ();
			this.SuspendLayout ();
			// 
			// label_version
			// 
			this.label_version.AutoSize = true;
			this.label_version.ForeColor = System.Drawing.Color.LightGray;
			this.label_version.Location = new System.Drawing.Point (16, 85);
			this.label_version.Name = "label_version";
			this.label_version.Size = new System.Drawing.Size (35, 13);
			this.label_version.TabIndex = 0;
			this.label_version.Text = "label1";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font ("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.LightGray;
			this.label2.Location = new System.Drawing.Point (12, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (347, 42);
			this.label2.TabIndex = 1;
			this.label2.Text = "mapKnight ToolKit™";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.LightGray;
			this.label1.Location = new System.Drawing.Point (300, 85);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (53, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "loading ...";
			// 
			// LoadForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.MediumBlue;
			this.ClientSize = new System.Drawing.Size (365, 107);
			this.Controls.Add (this.label1);
			this.Controls.Add (this.label2);
			this.Controls.Add (this.label_version);
			this.Cursor = System.Windows.Forms.Cursors.AppStarting;
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "LoadForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "LoadForm";
			this.Load += new System.EventHandler (this.LoadForm_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler (this.LoadForm_Paint);
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.Label label_version;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}