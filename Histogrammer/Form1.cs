using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma2
{
    public partial class Form1 : Form
    {
        PointF center = new PointF();
        double zoomX = 1; // Local value of each axisItem
        double zoomY = 1;
        float zoomStep = 1.8f; // Rate of changing itemWidth
        Point lineNumber = new Point(5, 5);
        float axisItemMin = 64, axisItemMax = 0;
        float axisItemWidth = 64; // Grid item size in pixels

        float axisMarkSize = 2;
        float axisSignIndent = 8;

        PointF mouseClick = new PointF();
        bool isMoving = false;

        Pen penAxisMain = new Pen(Color.FromArgb(160, 160, 160), 1f);
        Pen penAxisAux1 = new Pen(Color.FromArgb(200, 200, 200), 1f);
        Pen penAxisAux2 = new Pen(Color.FromArgb(240, 240, 240), 1f);
        Font fontSignAxis = new Font("Segoe UI", 8f);
        Brush brushSignAxis = Brushes.Gray;
        Brush brushSignAxisBg = new SolidBrush(Color.Gray);

        Dictionary<int, double> data = new Dictionary<int, double>();

        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            brushSignAxisBg = new SolidBrush(BackColor);
            center = new PointF(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            axisItemMax = axisItemMin * 2 / zoomStep;

            SFNetworkGenerator sfng = new SFNetworkGenerator();
            System.Threading.CancellationTokenSource cancel = new System.Threading.CancellationTokenSource();
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (obj, evt) => { string frmt = @"hh\:mm\:ss"; Text = "Progress: " + (int)(sfng.Progress * 10000) / 100.0 + "%   Left: " + sfng.TimeLeft.ToString(frmt) + "   Remaining: " + sfng.TimeRemaining.ToString(frmt); };
            new Task(() =>
            {
                data = sfng.GenerateSFNetworksAverage(1000, 3, 10, cancel.Token, 3);
                timer.Stop();
            }).Start();
            timer.Start();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseClick.X = e.Location.X - center.X;
            mouseClick.Y = e.Location.Y - center.Y;
            isMoving = true;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                center.X = e.X - mouseClick.X;
                center.Y = e.Y - mouseClick.Y;
                pictureBox1.Invalidate();
            }
            //Text = center.ToString();
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMoving = false;
        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            double zoomMltX = 2;
            double zoomMltY = 2;
            double zoomCopyX = zoomX;
            double zoomCopyY = zoomY;
            while (zoomCopyX < 1) zoomCopyX *= 10;
            while (zoomCopyY < 1) zoomCopyY *= 10;
            while (zoomCopyX > 5) zoomCopyX /= 10;
            while (zoomCopyY > 5) zoomCopyY /= 10;

            //Text = zoomX.ToString() + "; " + zoomY + " " + zoomMltX.ToString() + "; " + zoomMltY;

            // Zoom In
            if (e.Delta > 0)
            {
                axisItemWidth *= zoomStep;
                center.X = e.X + (center.X - e.X) * zoomStep;
                center.Y = e.Y + (center.Y - e.Y) * zoomStep;
            }
            // Zoom Out
            else
            {
                axisItemWidth /= zoomStep;
                center.X = e.X + (center.X - e.X) / zoomStep;
                center.Y = e.Y + (center.Y - e.Y) / zoomStep;
            }

            float w = axisItemWidth;
            // Zoom In
            if (axisItemWidth > axisItemMax)
            {
                if (zoomCopyX == 5) lineNumber.X = 4; else lineNumber.X = 5;
                if (zoomCopyY == 5) lineNumber.Y = 4; else lineNumber.Y = 5;
                if (zoomCopyX == 5) zoomMltX = 2.5;
                if (zoomCopyY == 5) zoomMltY = 2.5;
                axisItemWidth = axisItemMin;
                zoomX /= zoomMltX;
                zoomY /= zoomMltY;
                center.X = (float)(e.X + (center.X - e.X) * zoomMltX * axisItemWidth / w);
                center.Y = (float)(e.Y + (center.Y - e.Y) * zoomMltY * axisItemWidth / w);
            }
            // Zoom Out
            if (axisItemWidth < axisItemMin)
            {
                if (zoomCopyX == 2) zoomMltX = 2.5;
                if (zoomCopyY == 2) zoomMltY = 2.5;
                if (zoomCopyX == 1) lineNumber.X = 4; else lineNumber.X = 5;
                if (zoomCopyY == 1) lineNumber.Y = 4; else lineNumber.Y = 5;
                axisItemWidth = axisItemMax;
                zoomX *= zoomMltX;
                zoomY *= zoomMltY;
                center.X = (float)(e.X + (center.X - e.X) / zoomMltX * axisItemWidth / w);
                center.Y = (float)(e.Y + (center.Y - e.Y) / zoomMltY * axisItemWidth / w);
            }
            //k = new PointF((axisItemWidth / zoom.X) / k.X, (axisItemWidth / zoom.Y) / k.Y);

            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            PointF axisElementWidth = new PointF(axisItemWidth / lineNumber.X, axisItemWidth / lineNumber.Y);

            Graphics g = e.Graphics;
            if (качествоToolStripMenuItem.Checked)
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            if (скоростьToolStripMenuItem.Checked)
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            #region Drawing Axises
            // TODO: optimize stepping

            ///// X-axis
            // markers & signatures
            int imaxx = (int)Math.Max(center.X / axisItemWidth, (pictureBox1.Width - center.X) / axisItemWidth);
            for (int i = 1; i <= imaxx + 1; i++)
            {
                // floating markers
                //float y = center.Y - axisMarkSize;
                //if (y < 0) y = 0;
                //if (center.Y + axisMarkSize > pictureBox1.Height)
                //    y = pictureBox1.Height - axisMarkSize * 2;

                // markers
                if (center.X + (i - 1) * axisItemWidth < pictureBox1.Width)
                {
                    g.DrawLine(penAxisAux1, center.X + i * axisItemWidth, 0, center.X + i * axisItemWidth, pictureBox1.Height);
                    for (int j = 1; j < lineNumber.X; j++)
                    {
                        g.DrawLine(penAxisAux2, center.X + i * axisItemWidth - j * axisElementWidth.X, 0, center.X + i * axisItemWidth - j * axisElementWidth.X, pictureBox1.Height);
                    }
                    //g.DrawLine(penAxisMain, center.X + i * axisItemWidth, y, center.X + i * axisItemWidth, y + 2 * axisMarkSize);
                }
                if (center.X + i * axisItemWidth > 0)
                {
                    for (int j = 1; j < lineNumber.X; j++)
                    {
                        g.DrawLine(penAxisAux2, center.X - i * axisItemWidth + j * axisElementWidth.X, 0, center.X - i * axisItemWidth + j * axisElementWidth.X, pictureBox1.Height);
                    }
                    g.DrawLine(penAxisAux1, center.X - i * axisItemWidth, 0, center.X - i * axisItemWidth, pictureBox1.Height);
                    //g.DrawLine(penAxisMain, center.X - i * axisItemWidth, y, center.X - i * axisItemWidth, y + 2 * axisMarkSize);
                }
            }
            // axis
            g.DrawLine(penAxisMain, 0, center.Y, pictureBox1.Width, center.Y);
            // arrow
            g.DrawLines(penAxisMain, new PointF[] {
                new PointF(pictureBox1.Width - 8, center.Y - 3),
                new PointF(pictureBox1.Width, center.Y),
                new PointF(pictureBox1.Width - 8, center.Y + 3)
            });

            ///// Y-axis
            // markers & signatures
            int imaxy = (int)Math.Max(center.Y / axisItemWidth, (pictureBox1.Height - center.Y) / axisItemWidth);
            for (int i = 1; i <= imaxy + 1; i++)
            {
                // floating markers
                //float x = center.X - axisMarkSize;
                //if (x < 0) x = 0;
                //if (center.X + axisMarkSize > pictureBox1.Width)
                //    x = pictureBox1.Width - axisMarkSize * 2;

                // markers
                if (center.Y + (i - 1) * axisItemWidth < pictureBox1.Height)
                {
                    //g.DrawLine(penAxisMain, x, center.Y + i * axisItemWidth, x + 2 * axisMarkSize, center.Y + i * axisItemWidth);
                    g.DrawLine(penAxisAux1, 0, center.Y + i * axisItemWidth, pictureBox1.Width, center.Y + i * axisItemWidth);
                    for (int j = 1; j < lineNumber.Y; j++)
                    {
                        g.DrawLine(penAxisAux2, 0, center.Y + i * axisItemWidth - j * axisElementWidth.Y, pictureBox1.Width, center.Y + i * axisItemWidth - j * axisElementWidth.Y);
                    }
                }
                if (center.Y + (i + 1) * axisItemWidth > 0)
                {
                    //g.DrawLine(penAxisMain, x, center.Y - i * axisItemWidth, x + 2 * axisMarkSize, center.Y - i * axisItemWidth);
                    g.DrawLine(penAxisAux1, 0, center.Y - i * axisItemWidth, pictureBox1.Width, center.Y - i * axisItemWidth);
                    for (int j = 1; j < lineNumber.Y; j++)
                    {
                        g.DrawLine(penAxisAux2, 0, center.Y - i * axisItemWidth + j * axisElementWidth.Y, pictureBox1.Width, center.Y - i * axisItemWidth + j * axisElementWidth.Y);
                    }
                }
            }
            // X-signatures
            for (int i = 1; i < imaxx; i++)
            {
                float y = center.Y - axisMarkSize;
                // "+" signatures
                string s = (i * zoomX).ToString();
                SizeF size = g.MeasureString(s, fontSignAxis);
                if (size.Width < axisItemWidth)
                {
                    // floating signatures
                    y = center.Y + axisSignIndent;
                    if (center.Y < 0) y = axisSignIndent;
                    if (center.Y + axisSignIndent >= pictureBox1.Height - size.Height - axisSignIndent)
                        y = pictureBox1.Height - size.Height - axisSignIndent;
                    //  signatures
                    g.FillRectangle(brushSignAxisBg, center.X + i * axisItemWidth - size.Width / 2, y, size.Width, size.Height);
                    g.DrawString(s, fontSignAxis, brushSignAxis, center.X + i * axisItemWidth - size.Width / 2, y);
                }

                // "-" signatures
                s = (-i * zoomX).ToString();
                size = g.MeasureString(s, fontSignAxis);
                if (size.Width < axisItemWidth)
                {
                    y = center.Y + axisSignIndent;
                    if (center.Y < 0) y = axisSignIndent;
                    if (center.Y + axisSignIndent >= pictureBox1.Height - size.Height - axisSignIndent)
                        y = pictureBox1.Height - size.Height - axisSignIndent;
                    g.FillRectangle(brushSignAxisBg, center.X - i * axisItemWidth - size.Width / 2, y, size.Width, size.Height);
                    g.DrawString(s, fontSignAxis, brushSignAxis, center.X - i * axisItemWidth - size.Width / 2, y);
                }
            }
            
            // Y-signatures
            for (int i = 1; i < imaxy; i++)
            {
                float x = center.X - axisMarkSize;
                // "+" signatures
                string s = (i * zoomY).ToString();
                SizeF size = g.MeasureString(s, fontSignAxis);
                if (size.Height < axisItemWidth)
                {
                    // floating signatures
                    x = center.X - size.Width - axisSignIndent;
                    if (x < axisSignIndent) x = axisSignIndent;
                    if (center.X >= pictureBox1.Width)
                        x = pictureBox1.Width - size.Width - axisSignIndent;
                    //signatures
                    g.FillRectangle(brushSignAxisBg, x, center.Y - i * axisItemWidth - size.Height / 2, size.Width, size.Height);
                    g.DrawString(s, fontSignAxis, brushSignAxis, x, center.Y - i * axisItemWidth - size.Height / 2);
                }

                // "-" signatures
                s = (-i * zoomY).ToString();
                size = g.MeasureString(s, fontSignAxis);
                if (size.Height < axisItemWidth)
                {
                    // floating signatures
                    x = center.X - size.Width - axisSignIndent;
                    if (x < axisSignIndent) x = axisSignIndent;
                    if (center.X >= pictureBox1.Width)
                        x = pictureBox1.Width - size.Width - axisSignIndent;
                    //signatures
                    g.FillRectangle(brushSignAxisBg, x, center.Y + i * axisItemWidth - size.Height / 2, size.Width, size.Height);
                    g.DrawString(s, fontSignAxis, brushSignAxis, x, center.Y + i * axisItemWidth - size.Height / 2);
                }
            }

            // axis
            g.DrawLine(penAxisMain, center.X, 0, center.X, pictureBox1.Height);
            // markers
            g.DrawLines(penAxisMain, new PointF[] {
                new PointF(center.X - 3, 8),
                new PointF(center.X, 0),
                new PointF(center.X + 3, 8)
            });
            #endregion
            //Text = sw.Elapsed.ToString();
            PointF pLast = new PointF();
            bool first = true;
            for (int i = 0; i < data.Count; i++)
            {
                PointF p = Local2Global(i, (float)data[i]);
                if (IsOnScreen(p))
                {
                    if (!first)
                        g.DrawLine(Pens.Blue, pLast, p);
                    pLast = new PointF(p.X, p.Y);
                    first = false;
                    g.FillEllipse(Brushes.Red, p.X - 2, p.Y - 2, 4, 4);
                }
            }
        }

        private void качествоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(ToolStripMenuItem it in отрисовкаToolStripMenuItem.DropDownItems) it.Checked = false;
            качествоToolStripMenuItem.Checked = true;
        }
        private void скоростьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem it in отрисовкаToolStripMenuItem.DropDownItems) it.Checked = false;
            скоростьToolStripMenuItem.Checked = true;
        }


        private bool IsOnScreen(PointF p, float indentX = 0, float indentY = 0)
        {
            return p.X >= -indentX && p.X <= pictureBox1.Width + indentX && p.Y >= -indentY && p.Y <= pictureBox1.Height + indentY;
        }

        private PointF Local2Global(float localX, float localY)
        {
            return new PointF((float)(center.X + localX * axisItemWidth / zoomX), (float)(center.Y - localY * axisItemWidth / zoomY));
        }
        private PointF Local2Global(PointF localPoint)
        {
            return Local2Global(localPoint.X, localPoint.Y);
        }
        private PointF Global2Local(float globalX, float globalY)
        {
            return new PointF((float)(zoomX * (globalX - center.X) / axisItemWidth), (float)(zoomY * (globalY - center.Y) / axisItemWidth));
        }
        private PointF Global2Local(PointF globalPoint)
        {
            return Global2Local(globalPoint.X, globalPoint.Y);
        }
    }
}
