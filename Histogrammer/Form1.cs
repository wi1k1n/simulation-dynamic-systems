using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using io = System.IO;
using sb = System.Runtime.Serialization.Formatters.Binary;

namespace Diploma2
{
    public partial class Form1 : Form
    {
        SortedDictionary<double, double[]> dataRaw = new SortedDictionary<double, double[]>();
        SortedDictionary<double, float> dataSumSignal = new SortedDictionary<double, float>();
        SortedDictionary<double, float> dataCoherency = new SortedDictionary<double, float>();

        List<List<double>> hists = new List<List<double>>();
        List<List<double>> histsHubs = new List<List<double>>();
        List<List<double>> hubsPhases = new List<List<double>>();
        const int M = 10;

        int min = (int)-1e5, max = (int)1e5;
        List<double> rndData = new List<double>();

        public Form1()
        {
            InitializeComponent();

            ilGrapher1.BeforePaintAxes += IlGrapher1_BeforePaintAxes;
            ilGrapher1.AfterPaintAxes += IlGrapher1_AfterPaintAxes;
            ilGrapher1.Quality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ilGrapher1.CaptureLoad("ilgraph_capture_test");
            Dictionary<int, double> data = null;
            sb.BinaryFormatter bf = new sb.BinaryFormatter();
            using (io.FileStream fs = io.File.OpenRead(@"network_stat_average_50_3_100"))
                data = (Dictionary<int, double>)bf.Deserialize(fs);

            ilGrapher1.CaptureStart();
            foreach (var d in data)
            {
                ilGrapher1.FillCirclePoint(Color.Blue, 2, d.Key, (float)d.Value);
            }
            ilGrapher1.CaptureStop();
            ilGrapher1.CaptureSave("ilgraph_capture_test");

            #region Previous code
            /*
            return;
            int n = 10000;
            double wrnd = (max - min) / n;
            Random rnd = new Random();
            int cnt = 0;
            rndData = new List<double>(Enumerable.Repeat(0D, n));
            for (int j = 0; j < 100; j++)
            {
                ilRand irnd = new ilRand(rnd.Next(1, 100));
                List<int> data = new List<int>();
                for (int i = 0; i < 4e5; i++)
                    data.Add(irnd.Next(min, max) - min);

                List<int> h = new List<int>(Enumerable.Repeat<int>(0, n));
                for (int i = 0; i < data.Count; i++)
                    h[(int)(data[i] / wrnd)]++;

                for (int i = 0; i < rndData.Count; i++)
                    rndData[i] = (rndData[i] * cnt + h[i]) / (cnt + 1);
                cnt++;
            }

            return;
            #region Research work data
            sb.BinaryFormatter bf = new sb.BinaryFormatter();
            using (io.FileStream fs = io.File.OpenRead(@"phases"))
                dataRaw = (SortedDictionary<double, double[]>)bf.Deserialize(fs);

            Dictionary<int, int> nodes = null;
            using (io.FileStream fs = io.File.OpenRead(@"network-175-3-045-1-10-1--10-001.nodes"))
                nodes = (Dictionary<int, int>)bf.Deserialize(fs);

            foreach (var d in dataRaw)
            {
                double sum = 0;
                double sum2 = 0;
                double sum2i = 0;
                for (int j = 0; j < d.Value.Length; j++)
                {
                    sum += Math.Cos(d.Value[j]);
                    sum2 += Math.Cos(d.Value[j]);
                    sum2i += Math.Sin(d.Value[j]);
                }
                //sum /= 6.0;
                sum2 = (50 * Math.Sqrt(Math.Pow(sum2 / d.Value.Length, 2) + Math.Pow(sum2i / d.Value.Length, 2)));
                dataSumSignal.Add(d.Key, (float)sum);
                dataCoherency.Add(d.Key, (float)sum2);
            }

            foreach(var d in dataRaw)
            {
                double[] data = d.Value;
                List<double> res = new List<double>(Enumerable.Repeat<double>(0, 100));
                double w = 2 * Math.PI / res.Count;
                for (int i = 0; i < data.Length; i++)
                {
                    double v = data[i] < 0 ? data[i] + 2 * Math.PI : data[i];
                    res[(int)(v * res.Count / (2 * Math.PI))]++;
                }
                hists.Add(res);
            }

            foreach (var d in dataRaw)
            {
                double[] data = d.Value;
                List<double> res = new List<double>(Enumerable.Repeat<double>(0, 20));
                double w = 2 * Math.PI / res.Count;
                for (int i = 0; i < data.Length; i++)
                {
                    if (nodes[i] < M) continue;
                    double v = data[i] < 0 ? data[i] + 2 * Math.PI : data[i];
                    res[(int)(v * res.Count / (2 * Math.PI))]++;
                }
                histsHubs.Add(res);
            }

            foreach (var d in dataRaw)
            {
                double[] data = d.Value;
                List<double> res = new List<double>();
                for (int i = 0; i < data.Length; i++)
                {
                    if (nodes[i] < M) continue;
                    res.Add(data[i]);
                }
                hubsPhases.Add(res);
            }
            #endregion
    */
            #endregion
        }

        private void IlGrapher1_BeforePaintAxes(object sender, PaintEventArgs e)
        {
        }

        private void IlGrapher1_AfterPaintAxes(object sender, EventArgs e)
        {
            #region Previous code
            /*
            return;
            float y = 0;
            float x = 0;

            for (int i = 0; i < rndData.Count; i++)
                ilGrapher1.FillRectangle(Brushes.Red, x+i, y, 1f, (float)rndData[i]);

            return;
            y = -200;
            x = 0;
            foreach (var d in dataSumSignal)
                ilGrapher1.FillRectangle(Brushes.Red, (float)d.Key, y, 0.08f, d.Value);
            y = 0;
            foreach (var d in dataCoherency)
                ilGrapher1.FillRectangle(Brushes.Blue, (float)d.Key, y, 0.08f, d.Value);
            
            x = 0;
            y = 100;
            for (int j = 0; j < hists.Count; j++)
                for (int i = 0; i < hists[j].Count; i++) {
                    float xt = x + i,
                        yt = y + j * 20;
                    if (ilGrapher1.IsOnScreen(xt, yt))
                        ilGrapher1.FillRectangle(Brushes.Purple, xt, yt, 1, (float)hists[j][i]);
                }

            y = 100;
            x = hists[0].Count;
            for (int j = 0; j < histsHubs.Count; j++)
                for (int i = 0; i < histsHubs[j].Count; i++)
                {
                    float xt = x + i,
                        yt = y + j * 20;
                    if (ilGrapher1.IsOnScreen(xt, yt))
                        ilGrapher1.FillRectangle(Brushes.Orange, xt, yt, 1, (float)histsHubs[j][i]);
                }

            y = 100;
            x += histsHubs[0].Count + 10;
            for (int j = 0; j < hubsPhases.Count; j++)
                for (int i = 0; i < hubsPhases[j].Count; i++)
                {
                    float xt = x + i,
                        yt = y + j * 20;
                    if (ilGrapher1.IsOnScreen(xt, yt))
                        ilGrapher1.FillRectangle(Brushes.DarkGreen, xt, yt, 1, (float)hubsPhases[j][i]);
                }
                */
            #endregion
        }

        private void качествоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(ToolStripMenuItem it in отрисовкаToolStripMenuItem.DropDownItems) it.Checked = false;
            качествоToolStripMenuItem.Checked = true;
            ilGrapher1.Quality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        }
        private void скоростьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem it in отрисовкаToolStripMenuItem.DropDownItems) it.Checked = false;
            скоростьToolStripMenuItem.Checked = true;
            ilGrapher1.Quality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        }

        private void домойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ilGrapher1.Home();
        }
    }
}
