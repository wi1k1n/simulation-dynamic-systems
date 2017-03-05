using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Diploma2
{
    public partial class Main : Form
    {
        OpenFileDialog ofd = new OpenFileDialog();

        SFNetworkOscillator nw = null;

        List<Vertex> pts = new List<Vertex>();
        List<EdgeRef> edgs = new List<EdgeRef>();

        Image graph = new Bitmap(400, 100);
        Point graphLocation = new Point(10, 10);

        Timer timer1 = new Timer();
        bool isRunning = false;
        int stateCurrent = 0;

        public Main()
        {
            InitializeComponent();

            graphLocation.Y += menuStrip1.Bottom;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            timer1.Interval = 100;
            timer1.Tick += (o, evt) =>
            {
                if (stateCurrent >= nw.States.Count)
                {
                    timerUIControl(false);
                    return;
                }
                for (int i = 0; i < pts.Count; i++)
                    pts[i].Color = Vertex.ColorFromHSV(nw.States[stateCurrent].Phases[i] * 180 / Math.PI, 1, 1);
                Invalidate();
                stateCurrent++;
            };
        }
        private void Main_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            
            Font f = new Font("Segoe UI", 8.0f);
            foreach (EdgeRef edg in edgs)
                edg.Draw(g);
            for (int i = 0; i < pts.Count; i++)
            {
                pts[i].Draw(g);
                string s = "[" + i.ToString() + "]: " + nw.Nodes[pts[i].Id].ToString();
                SizeF size = g.MeasureString(s, f);
                g.DrawString(s, f, Brushes.Black, pts[i].Location.X - size.Width / 2, pts[i].Location.Y - size.Height / 2);
            }

            if (nw != null)
            {
                g.DrawImageUnscaled(graph, graphLocation);
                float kX = (float)((graph.Width - 1) / (nw.States[nw.States.Count - 1].Time - nw.States[0].Time));
                g.DrawLine(
                    Pens.Black,
                    (float)(graphLocation.X + nw.States[stateCurrent].Time * kX),
                    graphLocation.Y,
                    (float)(graphLocation.X + nw.States[stateCurrent].Time * kX), graphLocation.Y + graph.Height
                );
                g.DrawRectangle(Pens.Black, new Rectangle(graphLocation, graph.Size));
            }
        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Vertex v in pts)
                if (v.isInside(e.Location))
                {
                    v.IsMoving = true;
                    break;
                }
        }
        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None) return;
            foreach (Vertex v in pts)
            {
                if (v.IsMoving)
                {
                    v.Location = e.Location;
                    break;
                }
            }
            Invalidate();
        }
        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (Vertex v in pts)
                v.IsMoving = false;
        }

        private void loadNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                try
                {
                    nw = SFNetworkOscillator.Debinarize(ofd.FileName);
                    VisualizeNetwork();
                    InitializeMacro();
                }
                catch (Exception ex) { MessageBox.Show("Error occured while loading file: " + ex.Message, "Loading error"); }
            }
        }
        private void VisualizeNetwork()
        {
            // Radius sizes of the whole network
            float min_rad = 40, max_rad = 15;
            // Vertex radiuses
            float max_size = 32, min_size = 8;

            int max_degree = nw.Nodes[0], min_degree = nw.Nodes[0];

            max_rad = (ClientRectangle.Height - menuStrip1.Height) / 2 * 0.9f - min_rad;
            foreach (var d in nw.Nodes)
            {
                if (max_degree < d.Value)
                    max_degree = d.Value;
                if (min_degree == -1) min_degree = d.Value;
                if (min_degree > d.Value)
                    min_degree = d.Value;
            }
            float it = (max_rad - min_rad) / (max_degree - min_degree);
            float it_size = (max_size - min_size) / (max_degree - min_degree);

            pts = new List<Vertex>();
            edgs = new List<EdgeRef>();

            Random rnd = new Random();
            double a = 0;
            for (int i = 0; i < nw.Nodes.Count; i++)
            {
                a = rnd.NextDouble() * 360;
                bool iterate = true;
                float radius = nw.Nodes[i] * it_size + min_size;
                int wtchdg = 0;
                do
                {
                    int x = (int)(ClientRectangle.Width / 2 + ((max_degree - nw.Nodes[i]) * it * (rnd.NextDouble() * 0.4 + 0.8) + min_rad) * Math.Cos(a)),
                        y = (int)((ClientRectangle.Height + menuStrip1.Height) / 2 + ((max_degree - nw.Nodes[i]) * it * (rnd.NextDouble() * 0.4 + 0.8) + min_rad) * Math.Sin(a));
                    bool overlap = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (Math.Sqrt(Math.Pow(pts[j].Location.X - x, 2) + Math.Pow(pts[j].Location.Y - y, 2)) <= pts[j].Radius + radius) { overlap = true; break; }
                    }
                    if (!overlap || ++wtchdg > 1000)
                    {
                        pts.Add(new Vertex(i, new Point(x, y), nw.Nodes[i], nw.Nodes[i] * it_size + min_size));
                        iterate = false;
                    }
                } while (iterate);
            }
            for (int i = 0; i < nw.Edges.Count; i++)
            {
                edgs.Add(new EdgeRef(pts[nw.Edges[i].From], pts[nw.Edges[i].To], nw.Edges[i].Weight, 2, false));
                edgs[i].Color = Color.Black;
            }
            Invalidate();
        }
        private void InitializeMacro()
        {

            List<KeyValuePair<double, double>> macroSumSignal = new List<KeyValuePair<double, double>>();
            List<KeyValuePair<double, double>> macroCoherency = new List<KeyValuePair<double, double>>();
            PointF macroSumSignalMinMax = new PointF(float.PositiveInfinity, float.NegativeInfinity);
            PointF macroCoherencyMinMax = new PointF(float.PositiveInfinity, float.NegativeInfinity);
            foreach (SFNetworkOscillatorState d in nw.States)
            {
                double sumSig = 0;
                double sumCoh = 0;
                double sumCohi = 0;
                double cos = 0;
                for (int j = 0; j < d.Phases.Length; j++)
                {
                    cos = Math.Cos(d.Phases[j]);
                    sumSig += cos;
                    sumCoh += cos;
                    sumCohi += Math.Sin(d.Phases[j]);
                }
                sumCoh = (Math.Sqrt(Math.Pow(sumCoh / d.Phases.Length, 2) + Math.Pow(sumCohi / d.Phases.Length, 2)));

                if (sumSig < macroSumSignalMinMax.X) macroSumSignalMinMax.X = (float)(sumSig);
                if (sumSig > macroSumSignalMinMax.Y) macroSumSignalMinMax.Y = (float)(sumSig);
                if (sumCoh < macroCoherencyMinMax.X) macroCoherencyMinMax.X = (float)(sumCoh);
                if (sumCoh > macroCoherencyMinMax.Y) macroCoherencyMinMax.Y = (float)(sumCoh);

                macroSumSignal.Add(new KeyValuePair<double, double>(d.Time, sumSig));
                macroCoherency.Add(new KeyValuePair<double, double>(d.Time, sumCoh));
            }

            Graphics g = Graphics.FromImage(graph);
            // kW - normalize multiplier of X-axis for both macroSumSignal & macroCoherency
            float kX = (float)((graph.Width - 1) / (nw.States[nw.States.Count - 1].Time - nw.States[0].Time));
            // X - normalize multiplier of Y-axis for macroSumSignal
            // Y - normalize multiplier of Y-axis for macroCoherency
            PointF kY = new PointF(
                (graph.Height - 1) / (macroSumSignalMinMax.Y - macroSumSignalMinMax.X),
                (graph.Height - 1) / (macroCoherencyMinMax.Y - macroCoherencyMinMax.X)
            );

            // x-axis
            g.DrawLine(Pens.Pink,
                0,
                (graph.Height - 1) + macroSumSignalMinMax.X * kY.X,
                (graph.Width - 1),
                (graph.Height - 1) + macroSumSignalMinMax.X * kY.X
            );

            for (int i = 1; i < macroSumSignal.Count; i++)
            {
                g.DrawLine(
                    new Pen(Color.Red),
                    (float)(macroSumSignal[i - 1].Key + nw.States[0].Time) * kX,
                    (graph.Height - 1) - (float)(macroSumSignal[i - 1].Value - macroSumSignalMinMax.X) * kY.X,
                    (float)(macroSumSignal[i].Key + nw.States[0].Time) * kX,
                    (graph.Height - 1) - (float)(macroSumSignal[i].Value - macroSumSignalMinMax.X) * kY.X
                );
            }
            for (int i = 1; i < macroCoherency.Count; i++)
            {
                g.DrawLine(
                    new Pen(Color.Blue),
                    (float)(macroCoherency[i - 1].Key + nw.States[0].Time) * kX,
                    (graph.Height - 1) - (float)(macroCoherency[i - 1].Value - macroCoherencyMinMax.X) * kY.Y,
                    (float)(macroCoherency[i].Key + nw.States[0].Time) * kX,
                    (graph.Height - 1) - (float)(macroCoherency[i].Value - macroCoherencyMinMax.X) * kY.Y
                );
            }
        }


        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerUIControl(!isRunning);
            Invalidate();
        }
        private void timerUIControl(bool action)
        {
            if (action)
            {
                timer1.Start();
                startToolStripMenuItem.Text = "Стоп";
                isRunning = true;
            }
            else
            {
                timer1.Stop();
                startToolStripMenuItem.Text = "Старт";
                isRunning = false;
            }
        }
    }
    class Vertex
    {
        Font f = new Font("Segoe UI", 8.0f);

        public int Id { get; set; }
        public int Degree { get; set; }
        public Point Location { get; set; }
        public float Radius { get; set; }
        public Color Color { get; set; }
        public bool IsMoving { get; set; }



        public Vertex(int id, Point p, int degree, float r = 5)
        {
            Degree = degree;
            Id = id;
            Location = p;
            Radius = r;
            Color = Color.Red;
            IsMoving = false;
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Color), Location.X - Radius, Location.Y - Radius, Radius * 2, Radius * 2);
        }
        public bool isInside(Point p)
        {
            return Math.Sqrt(Math.Pow(p.X - Location.X, 2) + Math.Pow(p.Y - Location.Y, 2)) <= Radius;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
    class EdgeRef
    {
        Font f = new Font("Segoe UI", 8.0f);

        public Vertex From { get; set; }
        public Vertex To { get; set; }
        public int Degree { get; set; }
        public bool DrawEdgeDegree { get; set; }

        public Color Color { get; set; }
        public float Length { get { return (float)Math.Sqrt(Math.Pow(From.Location.X - To.Location.X, 2) + Math.Pow(From.Location.Y - To.Location.Y, 2)); } }
        public float Angle { get { return (float)Math.Atan2(From.Location.Y - To.Location.Y, From.Location.X - To.Location.X); } }
        public Point Location { get { return From.Location; } set { From.Location = value; } }
        public Point LocationEnd { get { return To.Location; } set { To.Location = value; } }

        public EdgeRef(Vertex v1, Vertex v2, int degree, float r = 5, bool drawEdges = true)
        {
            Degree = degree;
            From = v1;
            To = v2;
            Color = Color.Black;
            DrawEdgeDegree = drawEdges;
        }

        public void Draw(Graphics g)
        {
            g.DrawLine(new Pen(Color, Degree), Location, LocationEnd);
            if (!DrawEdgeDegree)
                return;
            SizeF s = g.MeasureString(Degree.ToString(), f);
            float m = Math.Max(s.Width, s.Height);
            g.FillEllipse(Brushes.DarkBlue,
                (float)(Location.X - Length / 2 * Math.Cos(Angle) - m / 2),
                (float)(Location.Y - Length / 2 * Math.Sin(Angle) - m / 2),
                m, m);
            g.DrawString(Degree.ToString(), f, Brushes.Yellow,
                (float)(Location.X - Length / 2 * Math.Cos(Angle) - s.Width / 2),
                (float)(Location.Y - Length / 2 * Math.Sin(Angle) - s.Height / 2));
        }
        public bool isInside(Point p)
        {
            return false;
        }
    }
}
