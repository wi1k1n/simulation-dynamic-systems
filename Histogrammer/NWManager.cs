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
        public event EventHandler<SFNWOscGraph> OnNetworkOpened;
        public event EventHandler<SFNWOscGraph> OnNetworkChanged;
        public event EventHandler<SFNWOscGraph> OnNetworkRemoved;

        List<SFNWOscGraph> networks = new List<SFNWOscGraph>();

        OpenFileDialog ofd = new OpenFileDialog();
        ColorDialog cd = new ColorDialog();

        public NWManager()
        {
            InitializeComponent();

            ofd.Multiselect = true;
        }

        private void NWManager_Load(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            checkBox1.Enabled = false;
        }

        private void nwopenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                try
                {
                    foreach (string fn in ofd.FileNames)
                    {
                        SFNetworkOscillator nw = SFNetworkOscillator.Debinarize(fn);
                        SFNWOscGraph nwg = new SFNWOscGraph(nw, fn, Color.Red, Color.Blue);
                        networks.Add(nwg);

                        listBox1.Items.Clear();
                        foreach (SFNWOscGraph i in networks)
                            listBox1.Items.Add(i);

                        OnNetworkOpened?.Invoke(this, nwg);
                    }
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
            sfnwog.MacroSignalKTB = new PointF(trackBar1.Value, trackBar2.Value);
            label5.Text = sfnwog.MacroSignalK.X.ToString();
            label6.Text = sfnwog.MacroSignalK.Y.ToString();
            OnNetworkChanged?.Invoke(this, sfnwog);
        }
        private void trackBarCH_Scroll(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            SFNWOscGraph sfnwog = ((SFNWOscGraph)listBox1.SelectedItem);
            sfnwog.MacroCoherencyK = new PointF(
                GetTrackValue(trackBar4, 0.05, 20),
                GetTrackValue(trackBar3, 0.05, 20)
            );
            sfnwog.MacroCoherencyKTB = new PointF(trackBar1.Value, trackBar2.Value);
            label7.Text = sfnwog.MacroCoherencyK.X.ToString();
            label8.Text = sfnwog.MacroCoherencyK.Y.ToString();
            OnNetworkChanged?.Invoke(this, sfnwog);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            checkBox1.Enabled = false;
            if (listBox1.SelectedIndex > -1)
            {
                SFNWOscGraph sfnwog = ((SFNWOscGraph)listBox1.SelectedItem);
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                checkBox1.Enabled = true;
                checkBox1.Checked = sfnwog.Visible;
                trackBar1.Value = (int)sfnwog.MacroSignalKTB.X;
                trackBar2.Value = (int)sfnwog.MacroSignalKTB.Y;
                label5.Text = sfnwog.MacroSignalK.X.ToString();
                label6.Text = sfnwog.MacroSignalK.Y.ToString();
                trackBar4.Value = (int)sfnwog.MacroCoherencyKTB.X;
                trackBar3.Value = (int)sfnwog.MacroCoherencyKTB.Y;
                label7.Text = sfnwog.MacroCoherencyK.X.ToString();
                label8.Text = sfnwog.MacroCoherencyK.Y.ToString();
                pictureBox1.BackColor = sfnwog.ColorSignal;
                pictureBox2.BackColor = sfnwog.ColorCoherency;
            }
        }

        private void NWManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void rmnetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            OnNetworkRemoved?.Invoke(this, networks[listBox1.SelectedIndex]);
            networks.RemoveAt(listBox1.SelectedIndex);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            networks[listBox1.SelectedIndex].Visible = checkBox1.Checked;
            OnNetworkChanged?.Invoke(this, networks[listBox1.SelectedIndex]);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackColor = cd.Color;
                networks[listBox1.SelectedIndex].ColorSignal = cd.Color;
                OnNetworkChanged?.Invoke(this, networks[listBox1.SelectedIndex]);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.BackColor = cd.Color;
                networks[listBox1.SelectedIndex].ColorCoherency = cd.Color;
                OnNetworkChanged?.Invoke(this, networks[listBox1.SelectedIndex]);
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
        public PointF MacroSignalKTB = new Point(1, 1);
        public PointF MacroSignalK = new Point(1, 1);
        public PointF MacroCoherencyK = new Point(1, 1);
        public PointF MacroCoherencyKTB = new Point(1, 1);

        public bool Visible { get; set; }

        public SFNWOscGraph(SFNetworkOscillator nw, string path, Color clrSS, Color clrCH)
        {
            Network = nw;
            Path = path;
            Name = System.IO.Path.GetFileName(path);
            ColorSignal = clrSS;
            ColorCoherency = clrCH;
            Visible = true;

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
                sumSig /= (double)d.Phases.Length;
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
}
