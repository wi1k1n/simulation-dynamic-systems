using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

namespace Diploma2
{
    public partial class Form1 : Form
    {
        const string registryKeyName = "SOFTWARE\\Ilyko\\Diploma2\\CSharp\\SFNetworkOscViewer";
        RegistryKey registryKey = null;

        Settings frmSettings = new Settings();
        OpenFileDialog ofd = new OpenFileDialog();

        SFNetworkOscillator nw = null;
        List<KeyValuePair<double, double>> macroSumSignal = new List<KeyValuePair<double, double>>();
        PointF macroSumSignalK = new Point(1, 1);
        List<KeyValuePair<double, double>> macroCoherency = new List<KeyValuePair<double, double>>();
        PointF macroCoherencyK = new Point(1, 1);

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            ilGrapher1.BeforePaintAxes += IlGrapher1_BeforePaintAxes;
            ilGrapher1.AfterPaintAxes += IlGrapher1_AfterPaintAxes;
            ilGrapher1.Quality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            try { registryKey = Registry.CurrentUser.CreateSubKey(registryKeyName); } catch { }

            frmSettings.OnMacroSumSignalScaleChanged += (o, e) => {
                macroSumSignalK = e.V;
                ilGrapher1.Invalidate();
            };
            frmSettings.OnMacroCoherencyScaleChanged += (o, e) => {
                macroCoherencyK = e.V;
                ilGrapher1.Invalidate();
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void IlGrapher1_BeforePaintAxes(object sender, PaintEventArgs e)
        {
        }
        private void IlGrapher1_AfterPaintAxes(object sender, EventArgs e)
        {
            for (int i = 1; i < macroSumSignal.Count; i++)
            {
                ilGrapher1.DrawLine(
                    Color.Red,
                    2,
                    (float)macroSumSignal[i - 1].Key * macroSumSignalK.X,
                    (float)macroSumSignal[i - 1].Value * macroSumSignalK.Y,
                    (float)macroSumSignal[i].Key * macroSumSignalK.X,
                    (float)macroSumSignal[i].Value * macroSumSignalK.Y
                );
            }

            for (int i = 1; i < macroCoherency.Count; i++)
            {
                ilGrapher1.DrawLine(
                    Color.Blue,
                    2,
                    (float)macroCoherency[i - 1].Key * macroCoherencyK.X,
                    (float)macroCoherency[i - 1].Value * macroCoherencyK.Y,
                    (float)macroCoherency[i].Key * macroCoherencyK.X,
                    (float)macroCoherency[i].Value * macroCoherencyK.Y
                );
            }
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


        private void openSFNWOscToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                try
                {
                    nw = SFNetworkOscillator.Debinarize(ofd.FileName);
                    InitializeMacro();
                    ilGrapher1.Invalidate();
                }
                catch (Exception ex) { MessageBox.Show("Error occured while loading file: " + ex.Message, "Loading error"); }
            }
        }
        private void InitializeMacro()
        {
            macroSumSignal = new List<KeyValuePair<double, double>>();
            macroCoherency = new List<KeyValuePair<double, double>>();
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
                sumCoh = (Math.Sqrt(Math.Pow(sumCoh, 2) + Math.Pow(sumCohi, 2)) / d.Phases.Length);
                macroSumSignal.Add(new KeyValuePair<double, double>(d.Time, sumSig));
                macroCoherency.Add(new KeyValuePair<double, double>(d.Time, sumCoh));
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSettings.Show();
        }
    }
}
