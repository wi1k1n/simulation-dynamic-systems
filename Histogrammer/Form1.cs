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
        List<Color> colors = new List<Color> {
            Color.Blue, Color.Green, Color.Red, Color.Orange, Color.Gray, Color.Indigo, Color.DarkCyan, Color.DarkSalmon, Color.DeepSkyBlue, Color.DeepPink, Color.Firebrick, Color.LemonChiffon
        };
        SortedDictionary<double, double[]> dataRaw = new SortedDictionary<double, double[]>();
        SortedDictionary<double, float> dataSumSignal = new SortedDictionary<double, float>();
        SortedDictionary<double, float> dataCoherency = new SortedDictionary<double, float>();

        List<List<double>> hists = new List<List<double>>();
        List<List<double>> histsHubs = new List<List<double>>();
        List<List<double>> hubsPhases = new List<List<double>>();
        const int M = 10;

        public Form1()
        {
            InitializeComponent();
            ilGrapher1.AfterPaintAxes += IlGrapher1_AfterPaintAxes;
            ilGrapher1.Quality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            sb.BinaryFormatter bf = new sb.BinaryFormatter();
            using (io.FileStream fs = io.File.OpenRead(@"phases"))
                dataRaw = (SortedDictionary<double, double[]>)bf.Deserialize(fs);

            SFNetworkOscillator nw = SFNetworkOscillator.Deserialize(@"D:/MISiS/НИР/8с Анализ безмасштабной сети/Project/Diploma2/NetworksTest/bin/Debug/network-175-3-045-1-10-1--10-001");
            
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
                    if (nw.Nodes[i] < M) continue;
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
                    if (nw.Nodes[i] < M) continue;
                    res.Add(data[i]);
                }
                hubsPhases.Add(res);
            }
        }

        private void IlGrapher1_AfterPaintAxes(object sender, EventArgs e)
        {
            float y = -200;
            float x = 0;
            //foreach (var d in dataSumSignal)
            //    ilGrapher1.FillRectangle(Brushes.Red, (float)d.Key, y, 0.8f, d.Value);
            y = 0;
            //foreach (var d in dataCoherency)
            //    ilGrapher1.FillRectangle(Brushes.Blue, (float)d.Key, y, 0.8f, d.Value);
            y = 100;
            for (int j = 0; j < hists.Count; j++)
                for (int i = 0; i < hists[j].Count; i++)
                    ilGrapher1.FillRectangle(Brushes.Purple, i, y + j * 20, 1, (float)hists[j][i]);

            y = 100;
            x = hists[0].Count;
            for (int j = 0; j < histsHubs.Count; j++)
                for (int i = 0; i < histsHubs[j].Count; i++)
                    ilGrapher1.FillRectangle(Brushes.Orange, x + i, y + j * 20, 1, (float)histsHubs[j][i]);

            y = 100;
            x += histsHubs[0].Count + 10;
            for (int j = 0; j < hubsPhases.Count; j++)
                for (int i = 0; i < hubsPhases[j].Count; i++)
                    ilGrapher1.FillRectangle(Brushes.DarkGreen, x + i, y + j * 20, 1, (float)hubsPhases[j][i]);
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
