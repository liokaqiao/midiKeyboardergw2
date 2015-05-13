namespace midiKeyboarder
{
    partial class Form1
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
            kill = true;
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
            this.components = new System.ComponentModel.Container();
            this.nudDelayTuner = new System.Windows.Forms.NumericUpDown();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.cbFlat = new System.Windows.Forms.CheckBox();
            this.cbFlute = new System.Windows.Forms.CheckBox();
            this.cbBass = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.nudCapx = new System.Windows.Forms.NumericUpDown();
            this.nudCapy = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudDelayTuner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCapx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCapy)).BeginInit();
            this.SuspendLayout();
            // 
            // nudDelayTuner
            // 
            this.nudDelayTuner.Location = new System.Drawing.Point(86, 47);
            this.nudDelayTuner.Name = "nudDelayTuner";
            this.nudDelayTuner.Size = new System.Drawing.Size(124, 22);
            this.nudDelayTuner.TabIndex = 0;
            this.nudDelayTuner.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "C",
            "D",
            "E",
            "F",
            "G",
            "A",
            "B"});
            this.comboBox1.Location = new System.Drawing.Point(89, 86);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 24);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // cbFlat
            // 
            this.cbFlat.AutoSize = true;
            this.cbFlat.Location = new System.Drawing.Point(82, 220);
            this.cbFlat.Name = "cbFlat";
            this.cbFlat.Size = new System.Drawing.Size(53, 21);
            this.cbFlat.TabIndex = 2;
            this.cbFlat.Text = "Flat";
            this.cbFlat.UseVisualStyleBackColor = true;
            // 
            // cbFlute
            // 
            this.cbFlute.AutoSize = true;
            this.cbFlute.Location = new System.Drawing.Point(82, 166);
            this.cbFlute.Name = "cbFlute";
            this.cbFlute.Size = new System.Drawing.Size(70, 21);
            this.cbFlute.TabIndex = 3;
            this.cbFlute.Text = "fLUTE";
            this.cbFlute.UseVisualStyleBackColor = true;
            // 
            // cbBass
            // 
            this.cbBass.AutoSize = true;
            this.cbBass.Location = new System.Drawing.Point(82, 193);
            this.cbBass.Name = "cbBass";
            this.cbBass.Size = new System.Drawing.Size(61, 21);
            this.cbBass.TabIndex = 4;
            this.cbBass.Text = "Bass";
            this.cbBass.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(191, 166);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(79, 70);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // nudCapx
            // 
            this.nudCapx.Location = new System.Drawing.Point(19, 116);
            this.nudCapx.Maximum = new decimal(new int[] {
            1980,
            0,
            0,
            0});
            this.nudCapx.Name = "nudCapx";
            this.nudCapx.Size = new System.Drawing.Size(58, 22);
            this.nudCapx.TabIndex = 6;
            this.nudCapx.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudCapx.ValueChanged += new System.EventHandler(this.nudCapx_ValueChanged);
            // 
            // nudCapy
            // 
            this.nudCapy.Location = new System.Drawing.Point(116, 116);
            this.nudCapy.Maximum = new decimal(new int[] {
            1800,
            0,
            0,
            0});
            this.nudCapy.Name = "nudCapy";
            this.nudCapy.Size = new System.Drawing.Size(55, 22);
            this.nudCapy.TabIndex = 7;
            this.nudCapy.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudCapy.ValueChanged += new System.EventHandler(this.nudCapy_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudCapy);
            this.Controls.Add(this.nudCapx);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.cbBass);
            this.Controls.Add(this.cbFlute);
            this.Controls.Add(this.cbFlat);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.nudDelayTuner);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudDelayTuner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCapx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCapy)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudDelayTuner;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox cbFlat;
        private System.Windows.Forms.CheckBox cbFlute;
        private System.Windows.Forms.CheckBox cbBass;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NumericUpDown nudCapx;
        private System.Windows.Forms.NumericUpDown nudCapy;
        private System.Windows.Forms.Label label1;
    }
}

