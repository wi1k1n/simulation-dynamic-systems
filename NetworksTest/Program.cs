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
            SFNetworkGenerator sfnwGen = new SFNetworkGenerator();
            Dictionary<int, double> data = sfnwGen.GenerateSFNetworksAverage(50, 3, 100, new CancellationToken(), 2);
            Serialize("network_stat_average_50_3_100", data);
            return;
            ilRand rnd = new ilRand(170303);
            SFNetwork nww = new SFNetwork(100, 3);
            string s = "";
            for (int i = 0; i < nww.Edges.Count; i++)
                s += nww.Edges[i].From + "\t" + nww.Edges[i].To + "\t" + nww.Edges[i].Weight + "\n";
            File.WriteAllText("network_ilrand_test.txt", s);
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
