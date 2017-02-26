namespace Diploma2
{
    partial class ilGrapher
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ilGrapher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ilGrapher";
            this.Size = new System.Drawing.Size(214, 201);
            this.Load += new System.EventHandler(this.ilGrapher_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ilGrapher_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ilGrapher_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ilGrapher_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ilGrapher_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ilGrapher_MouseWheel);
            this.Resize += new System.EventHandler(this.ilGrapher_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
