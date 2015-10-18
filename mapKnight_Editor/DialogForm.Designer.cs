namespace mapKnight_Editor
{
    partial class DialogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nud_Width = new System.Windows.Forms.NumericUpDown();
            this.nud_Height = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.tb_mapname = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_mapauthor = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Width)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Height)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Map Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Map Height";
            // 
            // nud_Width
            // 
            this.nud_Width.Location = new System.Drawing.Point(80, 12);
            this.nud_Width.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nud_Width.Minimum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.nud_Width.Name = "nud_Width";
            this.nud_Width.Size = new System.Drawing.Size(171, 20);
            this.nud_Width.TabIndex = 2;
            this.nud_Width.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nud_Width.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // nud_Height
            // 
            this.nud_Height.Location = new System.Drawing.Point(80, 38);
            this.nud_Height.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nud_Height.Minimum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.nud_Height.Name = "nud_Height";
            this.nud_Height.Size = new System.Drawing.Size(171, 20);
            this.nud_Height.TabIndex = 3;
            this.nud_Height.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nud_Height.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 116);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(236, 29);
            this.button1.TabIndex = 4;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb_mapname
            // 
            this.tb_mapname.Location = new System.Drawing.Point(80, 64);
            this.tb_mapname.Name = "tb_mapname";
            this.tb_mapname.Size = new System.Drawing.Size(171, 20);
            this.tb_mapname.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Map Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Map Author";
            // 
            // tb_mapauthor
            // 
            this.tb_mapauthor.Location = new System.Drawing.Point(80, 90);
            this.tb_mapauthor.Name = "tb_mapauthor";
            this.tb_mapauthor.Size = new System.Drawing.Size(171, 20);
            this.tb_mapauthor.TabIndex = 8;
            // 
            // DialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 154);
            this.Controls.Add(this.tb_mapauthor);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_mapname);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.nud_Height);
            this.Controls.Add(this.nud_Width);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(279, 193);
            this.MinimumSize = new System.Drawing.Size(279, 193);
            this.Name = "DialogForm";
            this.ShowIcon = false;
            this.Text = "Map Creation Window";
            ((System.ComponentModel.ISupportInitialize)(this.nud_Width)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Height)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.NumericUpDown nud_Width;
        public System.Windows.Forms.NumericUpDown nud_Height;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox tb_mapname;
        public System.Windows.Forms.TextBox tb_mapauthor;
    }
}