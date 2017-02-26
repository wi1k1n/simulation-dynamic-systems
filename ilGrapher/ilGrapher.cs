using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diploma2
{
    public partial class ilGrapher : UserControl
    {
        PointD center = new PointD();
        long zoomX = 1; // Local value of each axisItem
        long zoomY = 1;
        double zx = 1; // This is needed to reduce calculations on Math.Pow(zoomX, zoonDir.X)
        double zy = 1;
        Point zoomDir = new Point(1, 1);
        double zoomRestriction = 1e8;
        float zoomStep = 1.15f; // Rate of changing itemWidth
        Point lineNumber = new Point(5, 5);
        float axisItemMin = 128, axisItemMax = 0;
        float axisItemWidth = 128; // Grid item size in pixels

        SizeD axisArrowSize = new SizeD(8, 3);
        float axisSignIndent = 14;

        PointD mouseClick = new PointD();
        bool isMoving = false;

        Pen penAxisMain = new Pen(Color.FromArgb(120, 120, 120), 1f);
        Pen penAxisAux1 = new Pen(Color.FromArgb(200, 200, 200), 1f);
        Pen penAxisAux2 = new Pen(Color.FromArgb(240, 240, 240), 1f);
        Font fontSignAxis = new Font("Segoe UI", 8f);
        Brush brushSignAxis = Brushes.Gray;
        Brush brushSignAxisBg = new SolidBrush(Color.Gray);

        Graphics g = null;

        public ilGrapher()
        {
            InitializeComponent();
        }

        private void ilGrapher_Load(object sender, EventArgs e)
        {
            brushSignAxisBg = new SolidBrush(BackColor);
            center = new PointD(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            axisItemMax = axisItemMin * 2 / zoomStep;
        }

        private void ilGrapher_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ilGrapher_MouseDown(object sender, MouseEventArgs e)
        {
            mouseClick.X = e.Location.X - center.X;
            mouseClick.Y = e.Location.Y - center.Y;
            isMoving = true;
        }
        private void ilGrapher_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                center.X = e.X - mouseClick.X;
                center.Y = e.Y - mouseClick.Y;
                Invalidate();
            }
        }
        private void ilGrapher_MouseUp(object sender, MouseEventArgs e)
        {
            isMoving = false;
        }
        private void ilGrapher_MouseWheel(object sender, MouseEventArgs e)
        {
            zx = Math.Pow(zoomX, zoomDir.X);
            zy = Math.Pow(zoomY, zoomDir.Y);
            double zoomMltX = 2;
            double zoomMltY = 2;
            double zoomCopyX = zx;
            double zoomCopyY = zy;
            while (zoomCopyX < 1) zoomCopyX *= 10;
            while (zoomCopyY < 1) zoomCopyY *= 10;
            while (zoomCopyX > 5) zoomCopyX /= 10;
            while (zoomCopyY > 5) zoomCopyY /= 10;

            // Zoom In
            if (e.Delta > 0)
            {
                axisItemWidth = axisItemWidth * zoomStep;
                center.X = e.X + (center.X - e.X) * zoomStep;
                center.Y = e.Y + (center.Y - e.Y) * zoomStep;
            }
            // Zoom Out
            else
            {
                axisItemWidth = axisItemWidth / zoomStep;
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
                if (zoomDir.X == 1 && zoomX / zoomMltX < 1) zoomDir.X = -1;
                if (zoomDir.Y == 1 && zoomY / zoomMltY < 1) zoomDir.Y = -1;
                zoomX = (long)(zoomX * Math.Pow(zoomMltX, -zoomDir.X));
                zoomY = (long)(zoomY * Math.Pow(zoomMltY, -zoomDir.Y));
                center.X = e.X + (center.X - e.X) * zoomMltX * axisItemWidth / w;
                center.Y = e.Y + (center.Y - e.Y) * zoomMltY * axisItemWidth / w;
            }
            // Zoom Out
            if (axisItemWidth < axisItemMin)
            {
                if (zoomCopyX == 2) zoomMltX = 2.5;
                if (zoomCopyY == 2) zoomMltY = 2.5;
                if (zoomCopyX == 1) lineNumber.X = 4; else lineNumber.X = 5;
                if (zoomCopyY == 1) lineNumber.Y = 4; else lineNumber.Y = 5;
                axisItemWidth = axisItemMax;
                if (zoomDir.X == -1 && 1 / zoomX * zoomMltX > 1) zoomDir.X = 1;
                if (zoomDir.Y == -1 && 1 / zoomY * zoomMltY > 1) zoomDir.Y = 1;
                zoomX = (long)(zoomX / Math.Pow(zoomMltX, -zoomDir.X));
                zoomY = (long)(zoomY / Math.Pow(zoomMltY, -zoomDir.Y));
                center.X = e.X + (center.X - e.X) / zoomMltX * axisItemWidth / w;
                center.Y = e.Y + (center.Y - e.Y) / zoomMltY * axisItemWidth / w;
            }
            zx = Math.Pow(zoomX, zoomDir.X);
            zy = Math.Pow(zoomY, zoomDir.Y);
            Invalidate();
        }

        private void ilGrapher_Paint(object sender, PaintEventArgs e)
        {
            // Vector of width (in pixels) of each segment between 2 aux2 lines
            PointF axisElementWidth = new PointF(axisItemWidth / lineNumber.X, axisItemWidth / lineNumber.Y);

            g = e.Graphics;
            //if (качествоToolStripMenuItem.Checked)
            //    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //if (скоростьToolStripMenuItem.Checked)
            //    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            BeforePaintAxes?.Invoke(this, e);

            ///////////////////////////////////////////
            ////////////// Drawing axes ///////////////
            ///////////////////////////////////////////


            #region Drawing Axes 2
            ///// Vertical auxiliary lines
            // dinitx - pixels that are needed to gain first integer (divisible by zoom) to the left from global 0
            double dinitx = ((int)(center.X / axisItemWidth) - center.X / axisItemWidth) * axisItemWidth;
            // imaxx - the number of graph steps that are needed to be pictured with lines and signs
            int imaxx = (int)(Width / axisItemWidth) + 1;
            // startValue - the local value of the point corresponding to global: global0-dinitx
            double startValueX = -(center.X + dinitx) / (axisItemWidth / zx);
            for (int i = -1; i <= imaxx; i++)
            {
                g.DrawLine(penAxisAux1, (float)(i * axisItemWidth - dinitx), 0, (float)(i * axisItemWidth - dinitx), Height);
                for (int j = 1; j < lineNumber.X; j++)
                    g.DrawLine(penAxisAux2, (float)(i * axisItemWidth - dinitx + j * axisElementWidth.X), 0, (float)(i * axisItemWidth - dinitx + j * axisElementWidth.X), Height);
            }
            ///// Horizontal auxiliary lines
            double dinity = ((int)(center.Y / axisItemWidth) - center.Y / axisItemWidth) * axisItemWidth;
            int imaxy = (int)(Height / axisItemWidth) + 1;
            double startValueY = -(center.Y + dinity) / (axisItemWidth / Math.Pow(zoomY, zoomDir.X));
            for (int i = -1; i <= imaxy; i++)
            {
                g.DrawLine(penAxisAux1, 0, (float)(i * axisItemWidth - dinity), Width, (float)(i * axisItemWidth - dinity));
                for (int j = 1; j < lineNumber.X; j++)
                    g.DrawLine(penAxisAux2, 0, (float)(i * axisItemWidth - dinity + j * axisElementWidth.Y), Width, (float)(i * axisItemWidth - dinity + j * axisElementWidth.Y));
            }

            ///// X-axis
            if (IsGlobalOnScreen(0, center.Y, axisArrowSize.Width, axisArrowSize.Width))
            {
                g.DrawLine(penAxisMain, 0, (float)center.Y, Width, (float)center.Y);
                g.DrawLines(penAxisMain, new PointF[] {
                    new PointF(Width - 8, (float)center.Y - 3),
                    new PointF(Width, (float)center.Y),
                    new PointF(Width - 8, (float)(center.Y + 3))
                });
            }
            ///// Y-axis
            if (IsGlobalOnScreen(center.X, 0, axisArrowSize.Width, axisArrowSize.Width))
            {
                g.DrawLine(penAxisMain, (float)center.X, 0, (float)center.X, Height);
                g.DrawLines(penAxisMain, new PointF[] {
                    new PointF((float)(center.X - axisArrowSize.Height), (float)axisArrowSize.Width),
                    new PointF((float)center.X, 0),
                    new PointF((float)(center.X + axisArrowSize.Height), (float)axisArrowSize.Width)
                });
            }

            // Signatures for X-axis
            for (int i = -1; i <= imaxx; i++)
            {
                // Patch against -9.9999999
                double v = startValueX + i * zx;
                if (Math.Abs(zx) == 1 || zx > 1) v = Math.Round(v, 0);
                else v = Math.Round(v, (int)Math.Ceiling(-Math.Log10(zx)));

                string s = v.ToString();
                SizeF size = g.MeasureString(s, fontSignAxis);

                if (size.Width < axisItemWidth && v != 0)
                {
                    double y = center.Y + axisSignIndent;
                    if (center.Y < 0) y = axisSignIndent;
                    if (center.Y + axisSignIndent >= Height - size.Height - axisSignIndent)
                        y = Height - size.Height - axisSignIndent;

                    g.FillRectangle(brushSignAxisBg, (float)(i * axisItemWidth - dinitx - size.Width / 2), (float)y, size.Width, size.Height);
                    g.DrawString(s, fontSignAxis, brushSignAxis, (float)(i * axisItemWidth - dinitx - size.Width / 2), (float)y);
                }
            }
            // Signatures for Y-axis
            for (int i = -1; i <= imaxy; i++)
            {
                double v = startValueY + i * zy;
                if (Math.Abs(zy) == 1 || zy > 1) v = Math.Round(v, 0);
                else v = Math.Round(v, (int)Math.Ceiling(-Math.Log10(zy)));

                string s = (-v).ToString();
                SizeF size = g.MeasureString(s, fontSignAxis);

                if (size.Width < axisItemWidth && v != 0)
                {
                    double x = center.X - size.Width - axisSignIndent;
                    if (x < axisSignIndent) x = axisSignIndent;
                    if (center.X >= Width)
                        x = Width - size.Width - axisSignIndent;

                    g.FillRectangle(brushSignAxisBg, (float)x, (float)(i * axisItemWidth - dinity - size.Height / 2), size.Width, size.Height);
                    g.DrawString(s, fontSignAxis, brushSignAxis, (float)x, (float)(i * axisItemWidth - dinity - size.Height / 2));
                }
            }
            #endregion

            ///////////////////////////////////////////
            //////////// End Drawing axes /////////////
            ///////////////////////////////////////////

            AfterPaintAxes?.Invoke(this, e);

            g = null;
        }
        private bool IsGlobalOnScreen(double x, double y, double indentX = 0, double indentY = 0)
        {
            return x >= -indentX && x <= Width + indentX && y >= -indentY && y <= Height + indentY;
        }
        private bool IsGlobalOnScreen(PointD p, double indentX = 0, double indentY = 0)
        {
            return IsGlobalOnScreen(p.X, p.Y, indentX, indentY);
        }

        private PointD Local2Global(double localX, double localY)
        {
            return new PointD((float)(center.X + localX * axisItemWidth / zx), (float)(center.Y - localY * axisItemWidth / zy));
        }
        private PointD Local2Global(PointD localPoint)
        {
            return Local2Global(localPoint.X, localPoint.Y);
        }
        private PointD Global2Local(double globalX, double globalY)
        {
            return new PointD((float)(zx * (globalX - center.X) / axisItemWidth), (float)(zy * (globalY - center.Y) / axisItemWidth));
        }
        private PointD Global2Local(PointD globalPoint)
        {
            return Global2Local(globalPoint.X, globalPoint.Y);
        }


        ////// Interface elements
        public event PaintEventHandler BeforePaintAxes;
        public event PaintEventHandler AfterPaintAxes;

        // Navigation
        public void Home()
        {
            center = new PointD(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            zoomX = 1;
            zoomY = 1;
            axisItemWidth = axisItemMin;
            zx = Math.Pow(zoomX, zoomDir.X);
            zy = Math.Pow(zoomY, zoomDir.Y);
            Invalidate();
        }

        // Drawing
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
        }
        public void DrawLine(Pen pen, PointF p1, PointF p2)
        {
            if (g == null) return;
            p1 = Local2Global(p1);
            p2 = Local2Global(p2);
            if (!IsGlobalOnScreen(p1) && !IsGlobalOnScreen(p2)) return;
            g.DrawLine(pen, p1, p2);
        }
        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            DrawRectangle(pen, new RectangleF(x, y, width, height));
        }
        public void DrawRectangle(Pen pen, RectangleF rect)
        {
            if (g == null) return;
            Point p1 = Local2Global(rect.Location);
            Point p2 = Local2Global(rect.Right, rect.Bottom);
            Rectangle r = new Rectangle(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
            if (!IsGlobalOnScreen(rect.Location) && !IsGlobalOnScreen(rect.Right, rect.Top) && !IsGlobalOnScreen(rect.Left, rect.Bottom) && !IsGlobalOnScreen(rect.Right, rect.Bottom)) return;
            g.DrawRectangle(pen, r);
        }
        public void DrawCircle(Pen pen, PointF p, float r)
        {
            if (g == null) return;
            p = Local2Global(p);
            if (!IsGlobalOnScreen(p, r, r)) return;
            g.DrawEllipse(pen, p.X - r, p.Y - r, r * 2, r * 2);
        }
        public void DrawString(string s, Font font, Brush b, float x, float y)
        {
            DrawString(s, font, b, new PointF(x, y));
        }
        public void DrawString(string s, Font font, Brush b, PointF p)
        {
            if (g == null) return;
            p = Local2Global(p);
            SizeF size = g.MeasureString(s, font);
            if (!IsGlobalOnScreen(p) && !IsGlobalOnScreen(new PointF(p.X + size.Width, p.Y)) && !IsGlobalOnScreen(new PointF(p.X, p.Y + size.Height)) && !IsGlobalOnScreen(new PointF(p.X + size.Width, p.Y + size.Height))) return;
            g.DrawString(s, font, b, p);
        }

        // Filling
        public void FillCircle(Brush b, float r, float x, float y)
        {
            FillCircle(b, r, new PointF(x, y));
        }
        public void FillCircle(Brush b, float r, PointF p)
        {
            if (g == null) return;
            p = Local2Global(p);
            if (!IsGlobalOnScreen(p, r, r)) return;
            g.FillEllipse(b, p.X - r, p.Y - r, r * 2, r * 2);
        }
        public void FillRectangle(Brush b, float x, float y, float width, float height)
        {
            FillRectangle(b, new RectangleF(x, y, width, height));
        }
        public void FillRectangle(Brush b, RectangleF rect)
        {
            if (g == null) return;
            Point p1 = Local2Global(rect.Location);
            Point p2 = Local2Global(rect.Right, rect.Bottom);
            Rectangle r = new Rectangle(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
            if (!IsGlobalOnScreen(rect.Location) && !IsGlobalOnScreen(rect.Right, rect.Top) && !IsGlobalOnScreen(rect.Left, rect.Bottom) && !IsGlobalOnScreen(rect.Right, rect.Bottom)) return;
            g.FillRectangle(b, r);
        }
    }
}
