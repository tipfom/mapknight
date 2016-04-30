namespace mapKnight.ToolKit {
    partial class AddTileForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
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
        private void InitializeComponent () {
            this.picturebox_tile = new System.Windows.Forms.PictureBox ();
            this.button2 = new System.Windows.Forms.Button ();
            this.textbox_name = new System.Windows.Forms.TextBox ();
            this.label1 = new System.Windows.Forms.Label ();
            ((System.ComponentModel.ISupportInitialize)(this.picturebox_tile)).BeginInit ();
            this.SuspendLayout ();
            // 
            // picturebox_tile
            // 
            this.picturebox_tile.Location = new System.Drawing.Point (0, 0);
            this.picturebox_tile.Name = "picturebox_tile";
            this.picturebox_tile.Size = new System.Drawing.Size (250, 250);
            this.picturebox_tile.TabIndex = 0;
            this.picturebox_tile.TabStop = false;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point (153, 282);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size (85, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler (this.button2_Click);
            // 
            // textbox_name
            // 
            this.textbox_name.Location = new System.Drawing.Point (38, 256);
            this.textbox_name.Name = "textbox_name";
            this.textbox_name.Size = new System.Drawing.Size (212, 20);
            this.textbox_name.TabIndex = 3;
            this.textbox_name.KeyDown += new System.Windows.Forms.KeyEventHandler (this.textbox_name_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point (-3, 259);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding (5, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size (40, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name";
            // 
            // AddTileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size (250, 312);
            this.Controls.Add (this.label1);
            this.Controls.Add (this.textbox_name);
            this.Controls.Add (this.button2);
            this.Controls.Add (this.picturebox_tile);
            this.Name = "AddTileForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Tile";
            ((System.ComponentModel.ISupportInitialize)(this.picturebox_tile)).EndInit ();
            this.ResumeLayout (false);
            this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.PictureBox picturebox_tile;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textbox_name;
    }
}