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

        NWManager frmNWManager = new NWManager();

        List<SFNWOscGraph> networkGraphMacros = new List<SFNWOscGraph>();

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
            
            frmNWManager.OnNetworkOpened += (o, e) =>
            {
                networkGraphMacros.Add(e.Network);
                Invalidate();
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
            foreach (SFNWOscGraph nwgm in networkGraphMacros)
                nwgm.Draw(ilGrapher1);
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

        private void nwmanagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNWManager.Show();
        }
    }
}
