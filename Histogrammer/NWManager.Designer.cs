namespace Diploma2
{
    partial class NWManager
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.nwopenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rmnetworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBar4 = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nwopenToolStripMenuItem,
            this.rmnetworkToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(894, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // nwopenToolStripMenuItem
            // 
            this.nwopenToolStripMenuItem.Name = "nwopenToolStripMenuItem";
            this.nwopenToolStripMenuItem.Size = new System.Drawing.Size(112, 24);
            this.nwopenToolStripMenuItem.Text = "Открыть сеть";
            this.nwopenToolStripMenuItem.Click += new System.EventHandler(this.nwopenToolStripMenuItem_Click);
            // 
            // rmnetworkToolStripMenuItem
            // 
            this.rmnetworkToolStripMenuItem.Name = "rmnetworkToolStripMenuItem";
            this.rmnetworkToolStripMenuItem.Size = new System.Drawing.Size(110, 24);
            this.rmnetworkToolStripMenuItem.Text = "Удалить сеть";
            this.rmnetworkToolStripMenuItem.Click += new System.EventHandler(this.rmnetworkToolStripMenuItem_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 28;
            this.listBox1.Location = new System.Drawing.Point(12, 31);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(509, 312);
            this.listBox1.TabIndex = 1;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trackBar2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(527, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(352, 123);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sum Signal";
            // 
            // trackBar2
            // 
            this.trackBar2.AutoSize = false;
            this.trackBar2.Location = new System.Drawing.Point(146, 70);
            this.trackBar2.Minimum = -10;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(200, 40);
            this.trackBar2.TabIndex = 0;
            this.trackBar2.TickFrequency = 2;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBarSS_Scroll);
            // 
            // label2
            // 
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Location = new System.Drawing.Point(6, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 40);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y-scale";
            // 
            // trackBar1
            // 
            this.trackBar1.AutoSize = false;
            this.trackBar1.Location = new System.Drawing.Point(146, 30);
            this.trackBar1.Minimum = -10;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(200, 40);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.TickFrequency = 2;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBarSS_Scroll);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(89, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 40);
            this.label6.TabIndex = 1;
            this.label6.Text = "1";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(89, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 40);
            this.label5.TabIndex = 1;
            this.label5.Text = "1";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label1.Location = new System.Drawing.Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 40);
            this.label1.TabIndex = 1;
            this.label1.Text = "X-scale";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.trackBar3);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.trackBar4);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(527, 160);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(352, 123);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Coherency";
            // 
            // trackBar3
            // 
            this.trackBar3.AutoSize = false;
            this.trackBar3.Location = new System.Drawing.Point(146, 70);
            this.trackBar3.Maximum = 99;
            this.trackBar3.Minimum = -10;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(200, 40);
            this.trackBar3.SmallChange = 5;
            this.trackBar3.TabIndex = 0;
            this.trackBar3.TickFrequency = 10;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBarCH_Scroll);
            // 
            // label3
            // 
            this.label3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label3.Location = new System.Drawing.Point(6, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 40);
            this.label3.TabIndex = 1;
            this.label3.Text = "Y-scale";
            // 
            // trackBar4
            // 
            this.trackBar4.AutoSize = false;
            this.trackBar4.Location = new System.Drawing.Point(146, 30);
            this.trackBar4.Minimum = -10;
            this.trackBar4.Name = "trackBar4";
            this.trackBar4.Size = new System.Drawing.Size(200, 40);
            this.trackBar4.TabIndex = 0;
            this.trackBar4.TickFrequency = 2;
            this.trackBar4.Scroll += new System.EventHandler(this.trackBarCH_Scroll);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(89, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 40);
            this.label8.TabIndex = 1;
            this.label8.Text = "1";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(89, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 40);
            this.label7.TabIndex = 1;
            this.label7.Text = "1";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label4.Location = new System.Drawing.Point(6, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 40);
            this.label4.TabIndex = 1;
            this.label4.Text = "X-scale";
            // 
            // checkBox1
            // 
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(527, 289);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 54);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Visible";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(628, 290);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 50);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(684, 289);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(50, 50);
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // NWManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 352);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "NWManager";
            this.Text = "NWManager";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NWManager_FormClosing);
            this.Load += new System.EventHandler(this.NWManager_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem nwopenToolStripMenuItem;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackBar4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem rmnetworkToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}