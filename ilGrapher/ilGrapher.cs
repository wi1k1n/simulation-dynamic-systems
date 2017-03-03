using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using g2d = System.Drawing.Drawing2D;
using io = System.IO;
using sb = System.Runtime.Serialization.Formatters.Binary;

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
        float zoomStep = 1.15f; // Rate of changing itemWidth
        Point lineNumber = new Point(5, 5);
        float axisItemMin = 128, axisItemMax = 0;
        float axisItemWidth = 128; // Grid item size in pixels

        SizeD axisArrowSize = new SizeD(8, 3);
        float axisSignIndent = 5;

        PointD mouseClick = new PointD();
        bool isMoving = false;

        Pen penAxisMain = new Pen(Color.FromArgb(120, 120, 120), 1f);
        Pen penAxisAux1 = new Pen(Color.FromArgb(200, 200, 200), 1f);
        Pen penAxisAux2 = new Pen(Color.FromArgb(240, 240, 240), 1f);
        Font fontSignAxis = new Font("Segoe UI", 10f);
        Brush brushSignAxis = Brushes.Black;
        Brush brushSignAxisBg = new SolidBrush(Color.Gray);

        Graphics g = null;
        g2d.CompositingQuality gQuality = g2d.CompositingQuality.HighSpeed;

        List<ilGraphCaptureDraw> captureDraw = new List<ilGraphCaptureDraw>();
        List<ilGraphCaptureDrawPoint> captureDrawPoint = new List<ilGraphCaptureDrawPoint>();
        List<ilGraphCaptureFill> captureFill = new List<ilGraphCaptureFill>();
        List<ilGraphCaptureFillPoint> captureFillPoint = new List<ilGraphCaptureFillPoint>();
        bool capture = false;

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
            else if (e.Delta < 0)
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
            g.CompositingQuality = gQuality;

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
            #endregion

            ///////////////////////////////////////////
            //////////// End Drawing axes /////////////
            ///////////////////////////////////////////

            // TODO: Add a class incapsuling all 4 types with controlling z-order 
            foreach (var d in captureDraw)
                d.Draw(this);
            foreach (var d in captureDrawPoint)
                d.DrawPoint(this);
            foreach (var d in captureFill)
                d.Fill(this);
            foreach (var d in captureFillPoint)
                d.FillPoint(this);

            AfterPaintAxes?.Invoke(this, e);

            ///////////////////////////////////////////
            /////////// Drawing signatures ////////////
            ///////////////////////////////////////////

            #region Drawing signatures


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

                    //g.FillRectangle(brushSignAxisBg, (float)(i * axisItemWidth - dinitx - size.Width / 2), (float)y, size.Width, size.Height);
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

                    //g.FillRectangle(brushSignAxisBg, (float)x, (float)(i * axisItemWidth - dinity - size.Height / 2), size.Width, size.Height);
                    g.DrawString(s, fontSignAxis, brushSignAxis, (float)x, (float)(i * axisItemWidth - dinity - size.Height / 2));
                }
            }
            #endregion

            ///////////////////////////////////////////
            ///////// End Drawing signatures //////////
            ///////////////////////////////////////////

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
            return new PointD((float)(zx * (globalX - center.X) / axisItemWidth), (float)(zy * (center.Y - globalY) / axisItemWidth));
        }
        private PointD Global2Local(PointD globalPoint)
        {
            return Global2Local(globalPoint.X, globalPoint.Y);
        }


        ////// Interface elements
        public event PaintEventHandler BeforePaintAxes;
        public event PaintEventHandler AfterPaintAxes;

        ////  Properties
        public g2d.CompositingQuality Quality { get { return gQuality; } set { gQuality = value; } }

        //// Methods
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

        // isLocalOnScreen
        public bool IsOnScreen(double x, double y)
        {
            Point p = Local2Global(x, y);
            return p.X >= 0 && p.X <= Width && p.Y >= 0 && p.Y <= Height;
        }
        public bool IsOnScreen(Point p)
        {
            return IsOnScreen(p.X, p.Y);
        }

        // Drawing
        public void DrawLine(Color clr, float x1, float y1, float x2, float y2)
        {
            DrawLine(clr, new PointF(x1, y1), new PointF(x2, y2));
        }
        public void DrawLine(Color clr, PointF p1, PointF p2)
        {
            if (capture) captureDraw.Add(new ilGraphCaptureDrawLine(clr, p1, p2));
            if (g == null) return;
            p1 = Local2Global(p1);
            p2 = Local2Global(p2);
            if (!IsGlobalOnScreen(p1) && !IsGlobalOnScreen(p2)) return;
            g.DrawLine(new Pen(clr), p1, p2);
        }
        public void DrawRectangle(Color clr, float x, float y, float width, float height)
        {
            DrawRectangle(clr, new RectangleF(x, y, width, height));
        }
        public void DrawRectangle(Color clr, RectangleF rect)
        {
            if (capture) captureDraw.Add(new ilGraphCaptureDrawRectangle(clr, rect));
            if (g == null) return;
            Point p1 = Local2Global(rect.X, rect.Y);
            Point p2 = Local2Global(rect.X + rect.Width, rect.Y + rect.Height);
            if (!IsGlobalOnScreen(p1) && !IsGlobalOnScreen(p2) && !IsGlobalOnScreen(p1.X, p2.Y) && !IsGlobalOnScreen(p1.Y, p2.X)) return;
            int w = p2.X - p1.X;
            if (Math.Abs(w) < 1) return;
            int x = w > 0 ? p1.X : p2.X;
            int h = p1.Y - p2.Y;
            if (Math.Abs(h) < 1) return;
            int y = h > 0 ? p2.Y : p1.Y;
            g.DrawRectangle(new Pen(clr), new Rectangle(x, y, Math.Abs(w), Math.Abs(h)));
        }
        public void DrawCircle(Color clr, float x, float y, float r)
        {
            DrawCircle(clr, r, new PointF(x, y));
        }
        public void DrawCircle(Color clr, float r, PointF p)
        {
            if (capture) captureDraw.Add(new ilGraphCaptureDrawCircle(clr, r, p));
            if (g == null) return;
            p = Local2Global(p);
            float rx = r * axisItemWidth / zoomX,
                ry = r * axisItemWidth / zoomY;
            if (!IsGlobalOnScreen(p, rx, ry)) return;
            g.DrawEllipse(new Pen(clr), p.X - rx, p.Y - ry, rx * 2, ry * 2);
        }
        public void DrawString(string s, Font font, Color clr, float x, float y)
        {
            DrawString(s, font, clr, new PointF(x, y));
        }
        public void DrawString(string s, Font font, Color clr, PointF p)
        {
            if (capture) captureFill.Add(new ilGraphCaptureDrawString(s, font, clr, p));
            if (g == null) return;
            p = Local2Global(p);
            Font fontNew = new Font(font.FontFamily, (float)(font.Size * axisItemWidth / zoomX));
            SizeF size = g.MeasureString(s, fontNew);
            if (!IsGlobalOnScreen(p) && !IsGlobalOnScreen(new PointF(p.X + size.Width, p.Y)) && !IsGlobalOnScreen(new PointF(p.X, p.Y + size.Height)) && !IsGlobalOnScreen(new PointF(p.X + size.Width, p.Y + size.Height))) return;
            g.DrawString(s, fontNew, new SolidBrush(clr), p);
        }

        // Filling
        public void FillCircle(Color clr, float r, float x, float y)
        {
            FillCircle(clr, r, new PointF(x, y));
        }
        public void FillCircle(Color clr, float r, PointF p)
        {
            if (capture) captureFill.Add(new ilGraphCaptureFillCircle(clr, r, p));
            if (g == null) return;
            p = Local2Global(p);
            float rx = r * axisItemWidth / zoomX,
                ry = r * axisItemWidth / zoomY;
            if (!IsGlobalOnScreen(p, rx, ry)) return;
            g.FillEllipse(new SolidBrush(clr), p.X - rx, p.Y - ry, rx * 2, ry * 2);
        }
        public void FillRectangle(Color clr, float x, float y, float width, float height)
        {
            FillRectangle(clr, new RectangleF(x, y, width, height));
        }
        public void FillRectangle(Color clr, RectangleF rect)
        {
            if (capture) captureFill.Add(new ilGraphCaptureFillRectangle(clr, rect));
            if (g == null) return;
            Point p1 = Local2Global(rect.X, rect.Y);
            Point p2 = Local2Global(rect.X + rect.Width, rect.Y + rect.Height);
            if (!IsGlobalOnScreen(p1) && !IsGlobalOnScreen(p2) && !IsGlobalOnScreen(p1.X, p2.Y) && !IsGlobalOnScreen(p1.Y, p2.X)) return;
            float w = p2.X - p1.X;
            if (Math.Abs(w) < 1) return;
            float x = w > 0 ? p1.X : p2.X;
            float h = p1.Y - p2.Y;
            if (Math.Abs(h) < 1) return;
            float y = h > 0 ? p2.Y : p1.Y;
            g.FillRectangle(new SolidBrush(clr), new RectangleF(x, y, Math.Abs(w), Math.Abs(h)));
        }

        // Drawing Point
        public void DrawRectanglePoint(Color clr, float x, float y, float width, float height)
        {
            DrawRectanglePoint(clr, new RectangleF(x, y, width, height));
        }
        public void DrawRectanglePoint(Color clr, RectangleF rect)
        {
            if (capture) captureDrawPoint.Add(new ilGraphCaptureDrawRectanglePoint(clr, rect));
            if (g == null) return;
            Point p = Local2Global(rect.Location);
            SizeF sizeHalf = new SizeF(rect.Width / 2, rect.Height / 2);
            if (!IsGlobalOnScreen(p.X - sizeHalf.Width, p.Y - sizeHalf.Height / 2) && !IsGlobalOnScreen(p.X - sizeHalf.Width, p.Y + sizeHalf.Height / 2) && !IsGlobalOnScreen(p.X + sizeHalf.Width, p.Y + sizeHalf.Height / 2) && !IsGlobalOnScreen(p.X + sizeHalf.Width, p.Y - sizeHalf.Height / 2)) return;
            g.DrawRectangle(new Pen(clr), p.X - sizeHalf.Width, p.Y - sizeHalf.Height, rect.Width, rect.Height);
        }
        public void DrawCirclePoint(Color clr, float x, float y, float r)
        {
            DrawCirclePoint(clr, new PointF(x, y), r);
        }
        public void DrawCirclePoint(Color clr, PointF p, float r)
        {
            if (capture) captureDrawPoint.Add(new ilGraphCaptureDrawCirclePoint(clr, r, p));
            if (g == null) return;
            p = Local2Global(p);
            if (!IsGlobalOnScreen(p, r, r)) return;
            g.DrawEllipse(new Pen(clr), p.X - r, p.Y - r, r * 2, r * 2);
        }
        public void DrawStringPoint(string s, Font font, Color clr, float x, float y)
        {
            DrawStringPoint(s, font, clr, new PointF(x, y));
        }
        public void DrawStringPoint(string s, Font font, Color clr, PointF p)
        {
            if (capture) captureFillPoint.Add(new ilGraphCaptureDrawStringPoint(s, font, clr, p));
            if (g == null) return;
            p = Local2Global(p);
            SizeF size = g.MeasureString(s, font);
            if (!IsGlobalOnScreen(p) && !IsGlobalOnScreen(new PointF(p.X + size.Width, p.Y)) && !IsGlobalOnScreen(new PointF(p.X, p.Y + size.Height)) && !IsGlobalOnScreen(new PointF(p.X + size.Width, p.Y + size.Height))) return;
            g.DrawString(s, font, new SolidBrush(clr), p);
        }

        // Filling Point
        public void FillCirclePoint(Color clr, float r, float x, float y)
        {
            FillCirclePoint(clr, r, new PointF(x, y));
        }
        public void FillCirclePoint(Color clr, float r, PointF p)
        {
            if (capture) captureFillPoint.Add(new ilGraphCaptureFillCirclePoint(clr, r, p));
            if (g == null) return;
            p = Local2Global(p);
            if (!IsGlobalOnScreen(p, r, r)) return;
            g.FillEllipse(new SolidBrush(clr), p.X - r, p.Y - r, r * 2, r * 2);
        }
        public void FillRectanglePoint(Color clr, float x, float y, float width, float height)
        {
            FillRectanglePoint(clr, new RectangleF(x, y, width, height));
        }
        public void FillRectanglePoint(Color clr, RectangleF rect)
        {
            if (capture) captureFillPoint.Add(new ilGraphCaptureFillRectanglePoint(clr, rect));
            if (g == null) return;
            Point p = Local2Global(rect.Location);
            SizeF sizeHalf = new SizeF(rect.Width / 2, rect.Height / 2);
            if (!IsGlobalOnScreen(p.X - sizeHalf.Width, p.Y - sizeHalf.Height / 2) && !IsGlobalOnScreen(p.X - sizeHalf.Width, p.Y + sizeHalf.Height / 2) && !IsGlobalOnScreen(p.X + sizeHalf.Width, p.Y + sizeHalf.Height / 2) && !IsGlobalOnScreen(p.X + sizeHalf.Width, p.Y - sizeHalf.Height / 2)) return;
            g.FillRectangle(new SolidBrush(clr), p.X - sizeHalf.Width, p.Y - sizeHalf.Height, rect.Width, rect.Height);
        }


        //// Storing data to disk
        public void CaptureRestart()
        {
            capture = true;
            captureDraw = new List<ilGraphCaptureDraw>();
            captureDrawPoint = new List<ilGraphCaptureDrawPoint>();
            captureFill = new List<ilGraphCaptureFill>();
            captureFillPoint = new List<ilGraphCaptureFillPoint>();
        }
        public void CaptureStart()
        {
            capture = true;
        }
        public void CaptureStop()
        {
            capture = false;
        }
        public int CaptureSave(string path)
        {
            try
            {
                sb.BinaryFormatter bf = new sb.BinaryFormatter();
                using (io.FileStream fs = io.File.OpenWrite(path))
                {
                    bf.Serialize(fs, captureDraw);
                    bf.Serialize(fs, captureDrawPoint);
                    bf.Serialize(fs, captureFill);
                    bf.Serialize(fs, captureFillPoint);
                }
            } catch (Exception e)
            {
                return 1;
            }
            return 0;
        }
        public int CaptureLoad(string path)
        {
            try
            {
                sb.BinaryFormatter bf = new sb.BinaryFormatter();
                using (io.FileStream fs = io.File.OpenRead(path))
                {
                    captureDraw = (List<ilGraphCaptureDraw>)bf.Deserialize(fs);
                    captureDrawPoint = (List<ilGraphCaptureDrawPoint>)bf.Deserialize(fs);
                    captureFill = (List<ilGraphCaptureFill>)bf.Deserialize(fs);
                    captureFillPoint = (List<ilGraphCaptureFillPoint>)bf.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                return 1;
            }
            return 0;
        }
    }




    [Serializable]
    abstract class ilGraphCaptureDraw
    {
        public Color Color { get; set; }
        public ilGraphCaptureDraw(Color p) { Color = p; }
        public abstract void Draw(ilGrapher g);
    }
    [Serializable]
    abstract class ilGraphCaptureDrawPoint
    {
        public Color Color { get; set; }
        public ilGraphCaptureDrawPoint(Color p) { Color = p; }
        public abstract void DrawPoint(ilGrapher g);
    }
    [Serializable]
    abstract class ilGraphCaptureFill
    {
        public Color Color { get; set; }
        public ilGraphCaptureFill(Color b) { Color = b; }
        public abstract void Fill(ilGrapher g);
    }
    [Serializable]
    abstract class ilGraphCaptureFillPoint
    {
        public Color Color { get; set; }
        public ilGraphCaptureFillPoint(Color b) { Color = b; }
        public abstract void FillPoint(ilGrapher g);
    }

    [Serializable]
    class ilGraphCaptureDrawLine : ilGraphCaptureDraw
    {
        public PointF P1 { get; set; }
        public PointF P2 { get; set; }
        public ilGraphCaptureDrawLine(Color pen, PointF p1, PointF p2) : base(pen)
        {
            P1 = p1;
            P2 = p2;
        }
        public override void Draw(ilGrapher g)
        {
            g.DrawLine(Color, P1, P2);
        }
    }
    [Serializable]
    class ilGraphCaptureDrawString : ilGraphCaptureFill
    {
        public string S { get; set; }
        public Font F { get; set; }
        public PointF P { get; set; }
        public ilGraphCaptureDrawString(string s, Font f, Color b, PointF p) : base(b)
        {
            S = s;
            F = f;
            P = p;
        }
        public override void Fill(ilGrapher g)
        {
            g.DrawString(S, F, Color, P);
        }
    }
    [Serializable]
    class ilGraphCaptureDrawCircle : ilGraphCaptureDraw
    {
        public PointF P { get; set; }
        public float R { get; set; }
        public ilGraphCaptureDrawCircle(Color pen, float r, PointF p) : base(pen)
        {
            P = p;
            R = r;
        }
        public override void Draw(ilGrapher g)
        {
            g.DrawCircle(Color, R, P);
        }
    }
    [Serializable]
    class ilGraphCaptureDrawRectangle : ilGraphCaptureDraw
    {
        public RectangleF Rect { get; set; }
        public ilGraphCaptureDrawRectangle(Color pen, RectangleF rect) : base(pen)
        {
            Rect = rect;
        }
        public override void Draw(ilGrapher g)
        {
            g.DrawRectangle(Color, Rect);
        }
    }

    [Serializable]
    class ilGraphCaptureDrawStringPoint : ilGraphCaptureFillPoint
    {
        public string S { get; set; }
        public Font F { get; set; }
        public PointF P { get; set; }
        public ilGraphCaptureDrawStringPoint(string s, Font f, Color b, PointF p) : base(b)
        {
            S = s;
            F = f;
            P = p;
        }
        public override void FillPoint(ilGrapher g)
        {
            g.DrawStringPoint(S, F, Color, P);
        }
    }
    [Serializable]
    class ilGraphCaptureDrawCirclePoint : ilGraphCaptureDrawPoint
    {
        public PointF P { get; set; }
        public float R { get; set; }
        public ilGraphCaptureDrawCirclePoint(Color pen, float r, PointF p) : base(pen)
        {
            P = p;
            R = r;
        }
        public override void DrawPoint(ilGrapher g)
        {
            g.DrawCirclePoint(Color, P, R);
        }
    }
    [Serializable]
    class ilGraphCaptureDrawRectanglePoint : ilGraphCaptureDrawPoint
    {
        public RectangleF Rect { get; set; }
        public ilGraphCaptureDrawRectanglePoint(Color pen, RectangleF rect) : base(pen)
        {
            Rect = rect;
        }
        public override void DrawPoint(ilGrapher g)
        {
            g.DrawRectanglePoint(Color, Rect);
        }
    }

    [Serializable]
    class ilGraphCaptureFillCircle : ilGraphCaptureFill
    {
        public PointF P { get; set; }
        public float R { get; set; }
        public ilGraphCaptureFillCircle(Color b, float r, PointF p) : base(b)
        {
            P = p;
            R = r;
        }
        public override void Fill(ilGrapher g)
        {
            g.FillCircle(Color, R, P);
        }
    }
    [Serializable]
    class ilGraphCaptureFillRectangle : ilGraphCaptureFill
    {
        public RectangleF Rect { get; set; }
        public ilGraphCaptureFillRectangle(Color b, RectangleF rect) : base(b)
        {
            Rect = rect;
        }
        public override void Fill(ilGrapher g)
        {
            g.FillRectangle(Color, Rect);
        }
    }

    [Serializable]
    class ilGraphCaptureFillCirclePoint : ilGraphCaptureFillPoint
    {
        public PointF P { get; set; }
        public float R { get; set; }
        public ilGraphCaptureFillCirclePoint(Color b, float r, PointF p) : base(b)
        {
            P = p;
            R = r;
        }
        public override void FillPoint(ilGrapher g)
        {
            g.FillCirclePoint(Color, R, P);
        }
    }
    [Serializable]
    class ilGraphCaptureFillRectanglePoint : ilGraphCaptureFillPoint
    {
        public RectangleF Rect { get; set; }
        public ilGraphCaptureFillRectanglePoint(Color b, RectangleF rect) : base(b)
        {
            Rect = rect;
        }
        public override void FillPoint(ilGrapher g)
        {
            g.FillRectanglePoint(Color, Rect);
        }
    }
}