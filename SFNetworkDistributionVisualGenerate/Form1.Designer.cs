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
            this.генерироватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ilGrapher1 = new Diploma2.ilGrapher();
            this.открытьСетиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.остановитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.генерироватьToolStripMenuItem,
            this.остановитьToolStripMenuItem,
            this.открытьСетиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(887, 35);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // генерироватьToolStripMenuItem
            // 
            this.генерироватьToolStripMenuItem.Name = "генерироватьToolStripMenuItem";
            this.генерироватьToolStripMenuItem.Size = new System.Drawing.Size(132, 27);
            this.генерироватьToolStripMenuItem.Text = "Генерировать";
            this.генерироватьToolStripMenuItem.Click += new System.EventHandler(this.генерироватьToolStripMenuItem_Click);
            // 
            // ilGrapher1
            // 
            this.ilGrapher1.BackColor = System.Drawing.Color.White;
            this.ilGrapher1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ilGrapher1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ilGrapher1.Location = new System.Drawing.Point(0, 35);
            this.ilGrapher1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ilGrapher1.Name = "ilGrapher1";
            this.ilGrapher1.Quality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            this.ilGrapher1.Size = new System.Drawing.Size(887, 765);
            this.ilGrapher1.TabIndex = 1;
            // 
            // открытьСетиToolStripMenuItem
            // 
            this.открытьСетиToolStripMenuItem.Name = "открытьСетиToolStripMenuItem";
            this.открытьСетиToolStripMenuItem.Size = new System.Drawing.Size(127, 27);
            this.открытьСетиToolStripMenuItem.Text = "Открыть сети";
            this.открытьСетиToolStripMenuItem.Click += new System.EventHandler(this.открытьСетиToolStripMenuItem_Click);
            // 
            // остановитьToolStripMenuItem
            // 
            this.остановитьToolStripMenuItem.Name = "остановитьToolStripMenuItem";
            this.остановитьToolStripMenuItem.Size = new System.Drawing.Size(114, 27);
            this.остановитьToolStripMenuItem.Text = "Остановить";
            this.остановитьToolStripMenuItem.Click += new System.EventHandler(this.остановитьToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 800);
            this.Controls.Add(this.ilGrapher1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem генерироватьToolStripMenuItem;
        private Diploma2.ilGrapher ilGrapher1;
        private System.Windows.Forms.ToolStripMenuItem открытьСетиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem остановитьToolStripMenuItem;
    }
}

