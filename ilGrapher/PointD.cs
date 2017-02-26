using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Diploma2
{
    struct PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointD(double x, double y) { X = x; Y = y; }

        static public implicit operator PointD(Size p)
        {
            return new PointD(p.Width, p.Height);
        }
        static public implicit operator PointD(SizeF p)
        {
            return new PointD(p.Width, p.Height);
        }
        static public implicit operator PointD(SizeD p)
        {
            return new PointD(p.Width, p.Height);
        }
        static public implicit operator PointD(Point p)
        {
            return new PointD(p.X, p.Y);
        }
        static public implicit operator PointD(PointF p)
        {
            return new PointD(p.X, p.Y);
        }

        static public implicit operator Size(PointD p)
        {
            return new Size((int)p.X, (int)p.Y);
        }
        static public implicit operator SizeF(PointD p)
        {
            return new SizeF((float)p.X, (float)p.Y);
        }
        static public implicit operator SizeD(PointD p)
        {
            return new SizeD((int)p.X, (int)p.Y);
        }
        static public implicit operator Point(PointD p)
        {
            return new Point((int)p.X, (int)p.Y);
        }
        static public implicit operator PointF(PointD p)
        {
            return new PointF((int)p.X, (int)p.Y);
        }
    }
    struct SizeD
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public SizeD(double x, double y) { Width = x; Height = y; }

        static public implicit operator SizeD(Size p)
        {
            return new SizeD(p.Width, p.Height);
        }
        static public implicit operator SizeD(SizeF p)
        {
            return new SizeD(p.Width, p.Height);
        }
        static public implicit operator SizeD(Point p)
        {
            return new SizeD(p.X, p.Y);
        }
        static public implicit operator SizeD(PointF p)
        {
            return new SizeD(p.X, p.Y);
        }
        static public implicit operator SizeD(PointD p)
        {
            return new SizeD(p.X, p.Y);
        }

        static public implicit operator Size(SizeD p)
        {
            return new Size((int)p.Width, (int)p.Height);
        }
        static public implicit operator SizeF(SizeD p)
        {
            return new SizeF((float)p.Width, (float)p.Height);
        }
        static public implicit operator Point(SizeD p)
        {
            return new Point((int)p.Width, (int)p.Height);
        }
        static public implicit operator PointF(SizeD p)
        {
            return new PointF((int)p.Width, (int)p.Height);
        }
        static public implicit operator PointD(SizeD p)
        {
            return new PointD((int)p.Width, (int)p.Height);
        }
    }
}
