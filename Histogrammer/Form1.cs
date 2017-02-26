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
        Dictionary<int, double> data = new Dictionary<int, double>();

        public Form1()
        {
            InitializeComponent();
            ilGrapher1.AfterPaintAxes += IlGrapher1_AfterPaintAxes;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //SFNetworkGenerator sfng = new SFNetworkGenerator();
            //System.Threading.CancellationTokenSource cancel = new System.Threading.CancellationTokenSource();
            //Timer timer = new Timer();
            //timer.Interval = 1000;
            //timer.Tick += (obj, evt) => { string frmt = @"hh\:mm\:ss"; Text = "Progress: " + (int)(sfng.Progress * 10000) / 100.0 + "%   Left: " + sfng.TimeLeft.ToString(frmt) + "   Remaining: " + sfng.TimeRemaining.ToString(frmt); };
            //new Task(() =>
            //{
            //    data = sfng.GenerateSFNetworksAverage(1500, 3, 100, cancel.Token, 3);
            //    timer.Stop();

            //    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //    using (System.IO.FileStream fs = System.IO.File.OpenWrite("datastat"))
            //            bf.Serialize(fs, data);
            //}).Start();
            //timer.Start();

            //Random rnd = new Random();
            //for (int i = 0; i <= 10000; i++)
            //    //data.Add(i, rnd.Next(0, 10000));
            //    data.Add(i, i);
        }

        private void IlGrapher1_AfterPaintAxes(object sender, EventArgs e)
        {
            for (float i = -100; i <= 100; i += 1f)
                ilGrapher1.FillCircle(Brushes.Red, 2, i, i);
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

        private void домойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ilGrapher1.Home();
        }
    }
}
