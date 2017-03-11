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
    public partial class NWManager : Form
    {
        public event EventHandler<int> OnNetworkOpened;
        public event EventHandler<SFNWOscEventArgs> OnNetworkChanged;

        List<SFNWOscGraph> networks = new List<SFNWOscGraph>();
        OpenFileDialog ofd = new OpenFileDialog();

        public NWManager()
        {
            InitializeComponent();
        }

        private void NWManager_Load(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
        }

        private void nwopenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                try
                {
                    SFNetworkOscillator nw = SFNetworkOscillator.Debinarize(ofd.FileName);
                    SFNWOscGraph nwg = new SFNWOscGraph(nw, ofd.FileName, Color.Red, Color.Blue);
                    networks.Add(nwg);

                    listBox1.Items.Clear();
                    foreach (SFNWOscGraph i in networks)
                        listBox1.Items.Add(i);

                    OnNetworkOpened?.Invoke(this, new SFNWOscEventArgs(nwg));
                }
                catch (Exception ex) { MessageBox.Show("Error occured while loading file: " + ex.Message, "Loading error"); }
            }
        }

        private float GetTrackValue(TrackBar tr, double kMin, double kMax)
        {
            // These formulas are derived from y=a/(x-b)+c by 3 points
            double r = 0,
                s = 1,
                p = tr.Minimum,
                k = tr.Maximum;
            double b = (k * (kMax * p - kMax * r - kMin * p + s * r) - r * p * (s - kMin)) / (s * k - s * p + kMin * r - kMin * k + kMax * p - kMax * r),
                a = (kMax - kMin) * (k - b) * (p - b) / (p - k),
                c = kMin - a / (p - b);
            return (float)(a / (tr.Value - b) + c);
        }
        private void trackBarSS_Scroll(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            SFNWOscGraph sfnwog = ((SFNWOscGraph)listBox1.SelectedItem);
            sfnwog.MacroSignalK = new PointF(
                GetTrackValue(trackBar1, 0.05, 20),
                GetTrackValue(trackBar2, 0.05, 20)
            );
            label5.Text = sfnwog.MacroSignalK.X.ToString();
            label6.Text = sfnwog.MacroSignalK.Y.ToString();
            OnNetworkChanged?.Invoke(this, sfnwog);
        }
        private void trackBarCH_Scroll(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            SFNWOscGraph sfnwog = ((SFNWOscGraph)listBox1.SelectedItem);
            sfnwog.MacroSignalK = new PointF(
                GetTrackValue(trackBar4, 0.05, 20),
                GetTrackValue(trackBar3, 0.05, 20)
            );
            label7.Text = sfnwog.MacroSignalK.X.ToString();
            label8.Text = sfnwog.MacroSignalK.Y.ToString();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            if (listBox1.SelectedIndex > -1)
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
            }
        }
    }
    public class SFNWOscGraph
    {
        public SFNetworkOscillator Network { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Color ColorSignal { get; set; }
        public Color ColorCoherency { get; set; }

        public List<KeyValuePair<double, double>> MacroSignal = new List<KeyValuePair<double, double>>();
        public List<KeyValuePair<double, double>> MacroCoherency = new List<KeyValuePair<double, double>>();
        public PointF MacroSignalK = new Point(1, 1);
        public PointF MacroCoherencyK = new Point(1, 1);
        
        public SFNWOscGraph(SFNetworkOscillator nw, string path, Color clrSS, Color clrCH)
        {
            Network = nw;
            Path = path;
            Name = System.IO.Path.GetFileName(path);
            ColorSignal = clrSS;
            ColorCoherency = clrCH;
            InitializeMacro();
        }
        private void InitializeMacro()
        {
            MacroSignal = new List<KeyValuePair<double, double>>();
            MacroCoherency = new List<KeyValuePair<double, double>>();
            foreach (SFNetworkOscillatorState d in Network.States)
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
                MacroSignal.Add(new KeyValuePair<double, double>(d.Time, sumSig));
                MacroCoherency.Add(new KeyValuePair<double, double>(d.Time, sumCoh));
            }
        }
        public void Draw(ilGrapher ilg)
        {
            for (int i = 1; i < MacroSignal.Count; i++)
            {
                ilg.DrawLine(
                    ColorSignal,
                    2,
                    (float)MacroSignal[i - 1].Key * MacroSignalK.X,
                    (float)MacroSignal[i - 1].Value * MacroSignalK.Y,
                    (float)MacroSignal[i].Key * MacroSignalK.X,
                    (float)MacroSignal[i].Value * MacroSignalK.Y
                );
                ilg.FillCirclePoint(
                    ColorSignal,
                    2,
                    new PointF(
                        (float)MacroSignal[i].Key * MacroSignalK.X,
                        (float)MacroSignal[i].Value * MacroSignalK.Y
                    )
                );
            }

            for (int i = 1; i < MacroCoherency.Count; i++)
            {
                ilg.DrawLine(
                    ColorCoherency,
                    2,
                    (float)MacroCoherency[i - 1].Key * MacroCoherencyK.X,
                    (float)MacroCoherency[i - 1].Value * MacroCoherencyK.Y,
                    (float)MacroCoherency[i].Key * MacroCoherencyK.X,
                    (float)MacroCoherency[i].Value * MacroCoherencyK.Y
                );
                ilg.FillCirclePoint(
                    Color.Blue,
                    2,
                    new PointF(
                        (float)MacroCoherency[i].Key * MacroCoherencyK.X,
                        (float)MacroCoherency[i].Value * MacroCoherencyK.Y
                    )
                );
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SFNWOscEventArgs : EventArgs
    {
        public SFNWOscGraph Network { get; set; }
        public SFNWOscEventArgs(SFNWOscGraph nw) { Network = nw; }
    }
}
