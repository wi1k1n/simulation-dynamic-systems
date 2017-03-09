using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Diploma2.Networks;
using System.Diagnostics;
using Microsoft.Win32;

namespace Diploma2
{
    [Serializable]
    class Program
    {
        static double sin(double x) { return Math.Sin(x); }
        static double cos(double x) { return Math.Cos(x); }
        static double e(double x) { return Math.Exp(x); }
        static void Main(string[] args)
        {
            SFNetworkOscillator nw = new SFNetworkOscillator(75, 3, 0.3, 1, 10, -Math.PI, Math.PI, 0, 0.1, 0.005, 626);
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < 300; i++)
            {
                sw.Restart();
                nw.SimulateDynamicStep();
                Console.WriteLine(sw.ElapsedMilliseconds);
            }
            nw.Serialize("cs_network_dynamics_check");
            return;
            string s = "";
            for (int i = 0; i < nw.Phases.Length; i++)
                s += nw.Phases[i] + "\n";
            File.WriteAllText("cs_network_check_dynamics", s);
        }
        static void Serialize(string path, params object[] obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.OpenWrite(path))
                foreach(object o in obj)
                    bf.Serialize(fs, o);
        }
        static object[] Deserialize(string path)
        {
            List<object> ret = new List<object>();
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.OpenRead(path))
                while (fs.Position < fs.Length)
                    ret.Add(bf.Deserialize(fs));
            return ret.ToArray();
        }
    }
}
