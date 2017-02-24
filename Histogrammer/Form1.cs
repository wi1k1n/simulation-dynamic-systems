using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Histogrammer
{
    public partial class Form1 : Form
    {
        PointF center = new PointF();
        PointF zoom = new PointF(32, 32); // Count of pixels in local unit
        float zoomStep = 2f;
        float axisItemMin = 32, axisItemMax = 64;
        float axisItemWidth = 32;

        float axisMarkSize = 2;
        float axisSignIndent = 15;

        PointF mouseClick = new PointF();
        bool isMoving = false;

        Pen penAxisMain = new Pen(Color.Gray, 1f);
        Pen penAxisAux = new Pen(Color.FromArgb(240, 240, 240), 1f);
        Font fontAxis = new Font("Segoe UI", 8f);
        Brush brushAxis = Brushes.Gray;

        Brush brushPoint1 = Brushes.Red;
        float radiusPoint1 = 2;

        List<PointF> points = new List<PointF>();

        public Form1()
        {
            InitializeComponent();
        }

        public void Visualize(float[] x, float[] y)
        {
            pictureBox1.Invalidate();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            center = new PointF(ClientRectangle.Width / 2, ClientRectangle.Height / 2);

            points = new List<PointF>();
            for (float i = 0; i <= 200; i+=0.5f)
            {
                points.Add(new PointF(i, i));
            }
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
            if (e.Delta > 0)
            {
                zoom.X *= zoomStep;
                zoom.Y *= zoomStep;
                center.X = e.X + (center.X - e.X) * zoomStep;
                center.Y = e.Y + (center.Y - e.Y) * zoomStep;
            }
            else
            {
                zoom.X /= zoomStep;
                zoom.Y /= zoomStep;
                center.X = e.X + (center.X - e.X) / zoomStep;
                center.Y = e.Y + (center.Y - e.Y) / zoomStep;
            }
            
            pictureBox1.Invalidate();
            Text = zoom.ToString();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            #region Drawing Axises
            // TODO: optimize stepping

            ///// X-axis
            // axis
            g.DrawLine(penAxisMain, 0, center.Y, pictureBox1.Width, center.Y);
            // arrow
            g.DrawLines(penAxisMain, new PointF[] {
                new PointF(pictureBox1.Width - 8, center.Y - 4),
                new PointF(pictureBox1.Width, center.Y),
                new PointF(pictureBox1.Width - 8, center.Y + 4)
            });
            // markers & signatures
            int imaxx = (int)Math.Max(center.X / axisItemWidth, (pictureBox1.Width - center.X) / axisItemWidth);
            for (int i = 1; i <= imaxx; i++)
            {
                // floating markers
                float y = center.Y - axisMarkSize;
                if (y < 0) y = 0;
                if (center.Y + axisMarkSize > pictureBox1.Height)
                    y = pictureBox1.Height - axisMarkSize * 2;

                // markers
                if (center.X + i * axisItemWidth < pictureBox1.Width)
                {
                    g.DrawLine(penAxisAux, center.X + i * axisItemWidth, 0, center.X + i * axisItemWidth, pictureBox1.Height);
                    g.DrawLine(penAxisMain, center.X + i * axisItemWidth, y, center.X + i * axisItemWidth, y + 2 * axisMarkSize);
                }
                if (center.X + i * axisItemWidth > 0)
                {
                    g.DrawLine(penAxisAux, center.X - i * axisItemWidth, 0, center.X - i * axisItemWidth, pictureBox1.Height);
                    g.DrawLine(penAxisMain, center.X - i * axisItemWidth, y, center.X - i * axisItemWidth, y + 2 * axisMarkSize);
                }

                // "+" signatures
                string s = (Math.Round(i * axisItemWidth / zoom.X * 1000) / 1000).ToString();
                SizeF size = g.MeasureString(s, fontAxis);
                if (size.Width < axisItemWidth)
                {
                    // floating signatures
                    y = center.Y + axisSignIndent;
                    if (center.Y < 0) y = axisSignIndent;
                    if (center.Y + axisSignIndent >= pictureBox1.Height - size.Height - axisSignIndent)
                        y = pictureBox1.Height - size.Height - axisSignIndent;
                    //  signatures
                    g.DrawString(s, fontAxis, brushAxis, center.X + i * axisItemWidth - size.Width / 2, y);
                }
                
                // "-" signatures
                s = (-i * axisItemWidth / zoom.X).ToString();
                size = g.MeasureString(s, fontAxis);
                if (size.Width < axisItemWidth)
                {
                    y = center.Y + axisSignIndent;
                    if (center.Y < 0) y = axisSignIndent;
                    if (center.Y + axisSignIndent >= pictureBox1.Height - size.Height - axisSignIndent)
                        y = pictureBox1.Height - size.Height - axisSignIndent;
                    g.DrawString(s, fontAxis, brushAxis, center.X - i * axisItemWidth - size.Width / 2, y);
                }
            }

            ///// Y-axis
            // axis
            g.DrawLine(penAxisMain, center.X, 0, center.X, pictureBox1.Height);
            // markers
            g.DrawLines(penAxisMain, new PointF[] {
                new PointF(center.X - 4, 8),
                new PointF(center.X, 0),
                new PointF(center.X + 4, 8)
            });
            // markers & signatures
            int imaxy = (int)Math.Max(center.Y / axisItemWidth, (pictureBox1.Height - center.Y) / axisItemWidth);
            for (int i = 1; i <= imaxy; i++)
            {
                // floating markers
                float x = center.X - axisMarkSize;
                if (x < 0) x = 0;
                if (center.X + axisMarkSize > pictureBox1.Width)
                    x = pictureBox1.Width - axisMarkSize * 2;

                // markers
                if (center.Y + i * axisItemWidth < pictureBox1.Height)
                {
                    g.DrawLine(penAxisAux, 0, center.Y + i * axisItemWidth, pictureBox1.Width, center.Y + i * axisItemWidth);
                    g.DrawLine(penAxisMain, x, center.Y + i * axisItemWidth, x + 2 * axisMarkSize, center.Y + i * axisItemWidth);
                }
                if (center.Y + i * axisItemWidth > 0)
                {
                    g.DrawLine(penAxisAux, 0, center.Y - i * axisItemWidth, pictureBox1.Width, center.Y - i * axisItemWidth);
                    g.DrawLine(penAxisMain, x, center.Y - i * axisItemWidth, x + 2 * axisMarkSize, center.Y - i * axisItemWidth);
                }
                
                // "+" signatures
                string s = (i * axisItemWidth / zoom.Y).ToString();
                SizeF size = g.MeasureString(s, fontAxis);
                if (size.Height < axisItemWidth)
                {
                    // floating signatures
                    x = center.X - size.Width - axisSignIndent;
                    if (x < axisSignIndent) x = axisSignIndent;
                    if (center.X >= pictureBox1.Width)
                        x = pictureBox1.Width - size.Width - axisSignIndent;
                    //signatures
                    g.DrawString(s, fontAxis, brushAxis, x, center.Y - i * axisItemWidth);
                }

                // "-" signatures
                s = (-i * axisItemWidth / zoom.Y).ToString();
                size = g.MeasureString(s, fontAxis);
                if (size.Height < axisItemWidth)
                {
                    // floating signatures
                    x = center.X - size.Width - axisSignIndent;
                    if (x < axisSignIndent) x = axisSignIndent;
                    if (center.X >= pictureBox1.Width)
                        x = pictureBox1.Width - size.Width - axisSignIndent;
                    //signatures
                    g.DrawString(s, fontAxis, brushAxis, x, center.Y + i * axisItemWidth);
                }
            }
            #endregion
            
            for (int i = 0; i < points.Count; i++)
            {
                PointF p = Local2Global(points[i].X, points[i].Y);
                g.FillEllipse(brushPoint1, p.X - radiusPoint1, p.Y - radiusPoint1, radiusPoint1 * 2, radiusPoint1 * 2);
            }
        }

        private PointF Local2Global(float x, float y)
        {
            return new PointF(center.X + x * zoom.X, center.Y - y * zoom.Y);
        }
        private PointF Local2Global(PointF localPoint)
        {
            return Local2Global(localPoint.X, localPoint.Y);
        }
    }
}
