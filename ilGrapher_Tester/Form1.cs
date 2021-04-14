using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ilGrapher_Tester
{
    public partial class Form1 : Form
    {
        OpenFileDialog ofd = new OpenFileDialog();
        List<Sphere> input_clouds = new List<Sphere>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                input_clouds.Clear();
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "")
                        {
                            continue;
                        }
                        line = line.Replace('.', ',');
                        double R = double.Parse(line);
                        string[] str_coord = sr.ReadLine().Replace('.', ',').Split(' ');
                        List<double> coord = new List<double>();
                        for (int i = 0; i < str_coord.Length; ++i)
                        {
                            if (str_coord[i] == "")
                            {
                                continue;
                            }
                            coord.Add(double.Parse(str_coord[i]));
                        }
                        int id = input_clouds.Count;
                        input_clouds.Add(new Sphere(R, new MyPoint(coord.ToArray())));
                    }
                }
            }
        }

        private void ilGrapher1_AfterPaintAxes(object sender, PaintEventArgs e)
        {
            foreach (Sphere sph in input_clouds)
            {
                ilGrapher1.DrawCircle(Color.Red, (float)sph.C.coord[0], (float)sph.C.coord[1], (float)sph.R);
                ilGrapher1.FillCircle(Color.Red, (float)sph.C.coord[0], (float)sph.C.coord[1], (float)sph.R);
            }
            //ilGrapher1.FillCircle(Color.Blue, 72, 574.48F, 445.751F);
        }
    }

    public class MyPoint
    {
        public int dim { get; set; }
        public double[] coord { get; set; }

        public MyPoint(int _dim)
        {
            dim = _dim;
            coord = new double[dim];
        }

        public MyPoint(double[] _coord)
        {
            dim = _coord.Length;
            coord = _coord;
        }

        public MyPoint() { coord = new double[0]; }
    }
    public class Sphere
    {
        public double R { get; set; }
        public MyPoint C { get; set; }

        public Sphere(double _R, MyPoint _center)
        {
            R = _R;
            C = _center;
        }

        public Sphere()
        {
            R = 0;
            C = new MyPoint();
        }
    }
}
