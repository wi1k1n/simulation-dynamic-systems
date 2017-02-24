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
        PointF zoom = new PointF(0.03125f, 0.03125f);
        float axisStep = 32;
        float axisMarkSize = 2;
        float axisSignIndent = 5;

        PointF mouseClick = new PointF();
        bool isMoving = false;

        Pen penAxis = new Pen(Color.Gray, 1f);
        Font fontAxis = new Font("Segoe UI", 8f);
        Brush brushAxis = Brushes.Gray;

        Brush brushPoint1 = Brushes.Red;
        float radiusPoint1 = 2;

        Histogramm<float> hist = new Histogramm<float>();

        public Form1()
        {
            InitializeComponent();
        }

        public void Visualize(float[] x, float[] y)
        {
            hist = new Histogramm<float>(x, y);
            pictureBox1.Invalidate();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            center = new PointF(ClientRectangle.Width / 2, ClientRectangle.Height / 2);

            hist = new Histogramm<float>();
            for (float i = 0; i <= 100; i+=0.1f)
            {
                hist.X.Add((float)Math.Sqrt(i));
                hist.Y.Add(i);
            }
        }

        private void Form1_Resize(object sender, EventArgs e) { }

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
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMoving = false;
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                zoom.X /= 2;
                zoom.Y /= 2;
            }
            else
            {
                zoom.X *= 2;
                zoom.Y *= 2;
            }
            Text = zoom.ToString();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            #region Drawing Axises
            // TODO: optimize stepping

            //X-axis
            g.DrawLine(penAxis, 0, center.Y, pictureBox1.Width, center.Y);
            g.DrawLines(penAxis, new PointF[] {
                new PointF(pictureBox1.Width - 8, center.Y - 4),
                new PointF(pictureBox1.Width, center.Y),
                new PointF(pictureBox1.Width - 8, center.Y + 4)
            });
            for (float i = center.X + axisStep; i < pictureBox1.Width; i += axisStep)
            {
                float y = center.Y - axisMarkSize;
                if (y < 0) y = 0;
                if (y > pictureBox1.Height - axisMarkSize * 2)
                    y = pictureBox1.Height - axisMarkSize * 2;

                g.DrawLine(penAxis, i, y, i, y + 2 * axisMarkSize);
                string s = ((i - center.X) * zoom.X).ToString();
                SizeF size = g.MeasureString(s, fontAxis);
                if (size.Width < axisStep)
                {
                    y = center.Y + axisSignIndent;
                    if (center.Y < 0) y = axisSignIndent;
                    if (center.Y + axisSignIndent >= pictureBox1.Height - size.Height - axisSignIndent)
                        y = pictureBox1.Height - size.Height - axisSignIndent;
                    g.DrawString(s, fontAxis, brushAxis, i - size.Width / 2, y);
                }
            }
            for (float i = center.X - axisStep; i > 0; i -= axisStep)
            {
                g.DrawLine(penAxis, i, center.Y - axisMarkSize, i, center.Y + axisMarkSize);
                string s = ((i - center.X) * zoom.X).ToString();
                SizeF size = g.MeasureString(s, fontAxis);
                if (size.Width < axisStep)
                {
                    float y = center.Y + axisSignIndent;
                    if (y < 0) y = 0;
                    if (y >= pictureBox1.Height - size.Height - axisSignIndent)
                        y = pictureBox1.Height - size.Height - axisSignIndent;
                    g.DrawString(s, fontAxis, brushAxis, i - size.Width / 2, y);
                }
            }

            //Y-axis
            g.DrawLine(penAxis, center.X, 0, center.X, pictureBox1.Height);
            g.DrawLines(penAxis, new PointF[] {
                new PointF(center.X - 4, 8),
                new PointF(center.X, 0),
                new PointF(center.X + 4, 8)
            });
            for (float i = center.Y + axisStep; i < pictureBox1.Height; i += axisStep)
            {
                g.DrawLine(penAxis, center.X - axisMarkSize, i, center.X + axisMarkSize, i);
                string s = (-(i - center.Y) * zoom.Y).ToString();
                SizeF size = g.MeasureString(s, fontAxis);
                g.DrawString(s, fontAxis, brushAxis, center.X - size.Width - axisSignIndent, i - size.Height / 2);
            }
            for (float i = center.Y - axisStep; i > 0; i -= axisStep)
            {
                g.DrawLine(penAxis, center.X - axisMarkSize, i, center.X + axisMarkSize, i);
                string s = (-(i - center.Y) * zoom.Y).ToString();
                SizeF size = g.MeasureString(s, fontAxis);
                g.DrawString(s, fontAxis, brushAxis, center.X - size.Width - axisSignIndent, i - size.Height / 2);
            }
            #endregion
            
            for (int i = 0; i < hist.X.Count; i++)
            {
                PointF p = getRenderPointF(hist.X[i], hist.Y[i]);
                g.FillEllipse(brushPoint1, p.X - radiusPoint1, p.Y - radiusPoint1, radiusPoint1 * 2, radiusPoint1 * 2);
            }
        }

        private PointF getRenderPointF(float x, float y)
        {
            return new PointF(center.X + x / zoom.X, center.Y - y / zoom.Y);
        }
        private PointF getRenderPointF(PointF localPoint)
        {
            return getRenderPointF(localPoint.X, localPoint.Y);
        }
    }
}
