using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Diploma2
{
    public partial class Settings : Form
    {
        public event EventHandler<MacroEventArgs> OnMacroSumSignalScaleChanged;
        public event EventHandler<MacroEventArgs> OnMacroCoherencyScaleChanged;

        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        double kMin = 0.05,
            kMax = 20;
        private float GetTrackValue(TrackBar tr)
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
            MacroEventArgs evt = new MacroEventArgs(
                GetTrackValue(trackBar1),
                GetTrackValue(trackBar2)
            );
            label5.Text = evt.V.X.ToString();
            label6.Text = evt.V.Y.ToString();
            OnMacroSumSignalScaleChanged?.Invoke(this, evt);
        }
        private void trackBarCH_Scroll(object sender, EventArgs e)
        {
            MacroEventArgs evt = new MacroEventArgs(
                GetTrackValue(trackBar4),
                GetTrackValue(trackBar3)
            );
            label7.Text = evt.V.X.ToString();
            label8.Text = evt.V.Y.ToString();
            OnMacroCoherencyScaleChanged?.Invoke(this, evt);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 0;
            trackBarSS_Scroll(this, new EventArgs());
        }
        private void label2_Click(object sender, EventArgs e)
        {
            trackBar2.Value = 0;
            trackBarSS_Scroll(this, new EventArgs());
        }
        private void label4_Click(object sender, EventArgs e)
        {
            trackBar4.Value = 0;
            trackBarCH_Scroll(this, new EventArgs());
        }
        private void label3_Click(object sender, EventArgs e)
        {
            trackBar3.Value = 0;
            trackBarCH_Scroll(this, new EventArgs());
        }
    }
    public class MacroEventArgs : EventArgs
    {
        public PointF V {get;set;}
        public MacroEventArgs(float x, float y) { V = new PointF(x, y); }
    }
}
