namespace mapKnight.ToolKit {
    partial class NewMapForm {
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textbox_creator = new System.Windows.Forms.TextBox();
            this.textbox_name = new System.Windows.Forms.TextBox();
            this.combobox_tileset = new System.Windows.Forms.ComboBox();
            this.numericupdown_width = new System.Windows.Forms.NumericUpDown();
            this.numericupdown_height = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericupdown_width)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericupdown_height)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.Location = new System.Drawing.Point(152, 141);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 25);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.button2.Location = new System.Drawing.Point(12, 141);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 25);
            this.button2.TabIndex = 1;
            this.button2.Text = "CANCEL";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // textbox_creator
            // 
            this.textbox_creator.Location = new System.Drawing.Point(92, 12);
            this.textbox_creator.Name = "textbox_creator";
            this.textbox_creator.Size = new System.Drawing.Size(180, 20);
            this.textbox_creator.TabIndex = 2;
            // 
            // textbox_name
            // 
            this.textbox_name.Location = new System.Drawing.Point(92, 36);
            this.textbox_name.Name = "textbox_name";
            this.textbox_name.Size = new System.Drawing.Size(180, 20);
            this.textbox_name.TabIndex = 3;
            // 
            // combobox_tileset
            // 
            this.combobox_tileset.FormattingEnabled = true;
            this.combobox_tileset.Items.AddRange(new object[] {
            "Create New ..."});
            this.combobox_tileset.Location = new System.Drawing.Point(92, 62);
            this.combobox_tileset.Name = "combobox_tileset";
            this.combobox_tileset.Size = new System.Drawing.Size(180, 21);
            this.combobox_tileset.TabIndex = 4;
            // 
            // numericupdown_width
            // 
            this.numericupdown_width.Location = new System.Drawing.Point(92, 89);
            this.numericupdown_width.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericupdown_width.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericupdown_width.Name = "numericupdown_width";
            this.numericupdown_width.Size = new System.Drawing.Size(180, 20);
            this.numericupdown_width.TabIndex = 5;
            this.numericupdown_width.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // numericupdown_height
            // 
            this.numericupdown_height.Location = new System.Drawing.Point(92, 115);
            this.numericupdown_height.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericupdown_height.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericupdown_height.Name = "numericupdown_height";
            this.numericupdown_height.Size = new System.Drawing.Size(180, 20);
            this.numericupdown_height.TabIndex = 6;
            this.numericupdown_height.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "CREATOR";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "NAME";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "HEIGHT";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "WIDTH";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "TILESET";
            // 
            // NewMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 176);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericupdown_height);
            this.Controls.Add(this.numericupdown_width);
            this.Controls.Add(this.combobox_tileset);
            this.Controls.Add(this.textbox_name);
            this.Controls.Add(this.textbox_creator);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NewMapForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Map";
            ((System.ComponentModel.ISupportInitialize)(this.numericupdown_width)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericupdown_height)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox textbox_creator;
        public System.Windows.Forms.TextBox textbox_name;
        public System.Windows.Forms.ComboBox combobox_tileset;
        public System.Windows.Forms.NumericUpDown numericupdown_width;
        public System.Windows.Forms.NumericUpDown numericupdown_height;
    }
}