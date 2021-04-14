using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Diploma2
{
    public partial class Form1 : Form
    {
        System.Threading.CancellationTokenSource cancel = null;
        OpenFileDialog ofd = new OpenFileDialog();
        List<Dictionary<int, double>> data = new List<Dictionary<int, double>>();

        string[] colorNames = new string[] {
            #region Color Names
            "Red",
            "Blue",
            "Green",
            "Magenta",
            "DarkTurquoise",
            "Gold",
            "Black",
            "RoyalBlue",
            "SaddleBrown",
            "Khaki",
            "GreenYellow",
            "Crimson",
            "Teal",
            "LightBlue",
            "Yellow",
            "DarkRed",
            "Magenta",
            "MenuHighlight",
            "MediumVioletRed",
            "DarkMagenta",
            "HotTrack",
            "Chartreuse",
            "DeepPink",
            "DodgerBlue",
            "BlueViolet",
            "NavajoWhite",
            "Tan",
            "WindowFrame",
            "Desktop",
            "ActiveCaptionText",
            "DarkOrange",
            "SpringGreen",
            "ForestGreen",
            "OliveDrab",
            "Olive",
            "Lime",
            "SeaGreen"
#endregion
        };
        Color[] colors = null;

        public Form1()
        {
            InitializeComponent();

            colors = new Color[colorNames.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.FromName(colorNames[i]);

            ilGrapher1.AfterPaintAxes += IlGrapher1_AfterPaintAxes;
        }

        private void IlGrapher1_AfterPaintAxes(object sender, PaintEventArgs e)
        {
            for (int j = 0; j < data.Count; j++)
            {
                Dictionary<int, double> d = data[j];
                for (int i = 2; i < d.Count; i++)
                {
                    ilGrapher1.DrawLine(colors[j], 4, i - 1, (float)d[i - 1], i, (float)d[i]);
                }
            }
        }

        private void генерироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();

            SFNetworkGenerator sfng = new SFNetworkGenerator();
            cancel = new System.Threading.CancellationTokenSource();
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (obj, evt) => { string frmt = @"hh\:mm\:ss"; Text = "Progress: " + (int)(sfng.Progress * 10000) / 100.0 + "%   Left: " + sfng.TimeLeft.ToString(frmt) + "   Remaining: " + sfng.TimeRemaining.ToString(frmt); };
            new Task(() =>
            {
                var data = sfng.GenerateSFNetworksAverage(5000, 3, 50000, cancel.Token, 4);
                timer.Stop();

                bf = new BinaryFormatter();
                using (FileStream fs = File.OpenWrite("datastat-5000-3-50000"))
                    bf.Serialize(fs, data);
            }).Start();
            timer.Start();
        }

        private void открытьСетиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                data = new List<Dictionary<int, double>>();
                BinaryFormatter bf = new BinaryFormatter();
                foreach (string p in ofd.FileNames)
                {
                    using (FileStream fs = File.OpenRead(p))
                        data.Add((Dictionary<int, double>)bf.Deserialize(fs));
                }
            }
        }

        private void остановитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cancel.Cancel();
        }
    }
}
