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
    class Program
    {
        static double sin(double x) { return Math.Sin(x); }
        static double cos(double x) { return Math.Cos(x); }
        static double e(double x) { return Math.Exp(x); }
        static void Main(string[] args)
        {
            SFNetworkOscillator nw = new SFNetworkOscillator(175, 3, 0.45, 1, 10, -Math.PI, Math.PI, 1, -10, 0.01, 123);
            nw.Serialize("network-175-3-045-1-10-1--10-001");
            SortedDictionary<double, double[]> l = new SortedDictionary<double, double[]>();
            Stopwatch sw = new Stopwatch();

            CancellationTokenSource ctSrc = new CancellationTokenSource();
            Task task = new Task(() =>
            {
                l.Add(nw.Time, nw.Phases);
                Console.WriteLine("Time: {0:F5}\t\t0", nw.Time);
                for (int j = 0; j < 100000; j++)
                {
                    if (ctSrc.IsCancellationRequested)
                        break;
                    sw.Restart();
                    nw.SimulateDynamicStep();
                    sw.Stop();
                    l.Add(nw.Time, nw.Phases);
                    Console.WriteLine("Time: {0:F5}\t\t{1}", nw.Time, sw.Elapsed);
                }
                Serialize("phases-175-3-045-1-10-1--10-001-100000", l);
                Console.WriteLine("Serializing finished!");
            }, ctSrc.Token);
            task.Start();

            bool cliStop = false;
            while (!cliStop)
            {
                string s = Console.ReadLine();
                if (s == "stop")
                    ctSrc.Cancel();
            }

            Console.ReadKey();
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
