namespace Diploma2
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
            this.отрисовкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.качествоToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.скоростьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.домойToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nwmanagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ilGrapher1 = new Diploma2.ilGrapher();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.отрисовкаToolStripMenuItem,
            this.домойToolStripMenuItem,
            this.nwmanagerToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1078, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // отрисовкаToolStripMenuItem
            // 
            this.отрисовкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.качествоToolStripMenuItem,
            this.скоростьToolStripMenuItem});
            this.отрисовкаToolStripMenuItem.Name = "отрисовкаToolStripMenuItem";
            this.отрисовкаToolStripMenuItem.Size = new System.Drawing.Size(95, 24);
            this.отрисовкаToolStripMenuItem.Text = "Отрисовка";
            // 
            // качествоToolStripMenuItem
            // 
            this.качествоToolStripMenuItem.Checked = true;
            this.качествоToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.качествоToolStripMenuItem.Name = "качествоToolStripMenuItem";
            this.качествоToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.качествоToolStripMenuItem.Text = "Качество";
            this.качествоToolStripMenuItem.Click += new System.EventHandler(this.качествоToolStripMenuItem_Click);
            // 
            // скоростьToolStripMenuItem
            // 
            this.скоростьToolStripMenuItem.Name = "скоростьToolStripMenuItem";
            this.скоростьToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.скоростьToolStripMenuItem.Text = "Скорость";
            this.скоростьToolStripMenuItem.Click += new System.EventHandler(this.скоростьToolStripMenuItem_Click);
            // 
            // домойToolStripMenuItem
            // 
            this.домойToolStripMenuItem.Name = "домойToolStripMenuItem";
            this.домойToolStripMenuItem.Size = new System.Drawing.Size(69, 24);
            this.домойToolStripMenuItem.Text = "Домой";
            this.домойToolStripMenuItem.Click += new System.EventHandler(this.домойToolStripMenuItem_Click);
            // 
            // nwmanagerToolStripMenuItem
            // 
            this.nwmanagerToolStripMenuItem.Name = "nwmanagerToolStripMenuItem";
            this.nwmanagerToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.nwmanagerToolStripMenuItem.Text = "Сети";
            this.nwmanagerToolStripMenuItem.Click += new System.EventHandler(this.nwmanagerToolStripMenuItem_Click);
            // 
            // ilGrapher1
            // 
            this.ilGrapher1.BackColor = System.Drawing.Color.White;
            this.ilGrapher1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ilGrapher1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ilGrapher1.Location = new System.Drawing.Point(0, 28);
            this.ilGrapher1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ilGrapher1.Name = "ilGrapher1";
            this.ilGrapher1.Quality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            this.ilGrapher1.Size = new System.Drawing.Size(1078, 539);
            this.ilGrapher1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1078, 567);
            this.Controls.Add(this.ilGrapher1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Histogram Visualizer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem отрисовкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem качествоToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem скоростьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem домойToolStripMenuItem;
        private ilGrapher ilGrapher1;
        private System.Windows.Forms.ToolStripMenuItem nwmanagerToolStripMenuItem;
    }
}

