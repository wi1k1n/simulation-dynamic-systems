using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Diploma2.Networks;
using System.Diagnostics;

namespace Diploma2
{
    class Program
    {
        static int tests = 100, progress = 0, inProgress = 0, nodes = 10000, mlt = 3;
        static Dictionary<int, int> nodesDict = new Dictionary<int, int>();
        static List<SFNetwork> networks = new List<SFNetwork>();
        static List<Dictionary<int, int>> stats = new List<Dictionary<int, int>>();
        static object locker = new object();
        static TimeSpan elapsed = new TimeSpan();
        static Stopwatch swGlobal = new Stopwatch();
        static bool stop = false;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            swGlobal.Start();
            object[] objs = Deserialize("tests-50-3-10000");
            networks = (List<SFNetwork>)objs[0];
            stats = (List<Dictionary<int, int>>)objs[1];
            swGlobal.Stop();
            Console.WriteLine(networks.Count);
            Console.WriteLine(stats.Count);
            Console.WriteLine(swGlobal.Elapsed);

            return;

            nodesDict.Add(50, 10000);
            nodesDict.Add(100, 5000);
            nodesDict.Add(200, 1000);
            nodesDict.Add(1000, 500);
            nodesDict.Add(5000, 100);

            foreach (var pair in nodesDict)
            {
                nodes = pair.Key;
                tests = pair.Value;
                progress = 0;
                inProgress = 0;

                swGlobal.Restart();

                Task tsk1 = new Task(GenerateTestNetwork);
                Task tsk2 = new Task(GenerateTestNetwork);
                Task tsk3 = new Task(GenerateTestNetwork);
                Task tsk4 = new Task(GenerateTestNetwork);
                tsk1.Start();
                tsk2.Start();
                tsk3.Start();
                tsk4.Start();
                tsk1.Wait();
                tsk2.Wait();
                tsk3.Wait();
                tsk4.Wait();

                swGlobal.Stop();
                Console.WriteLine("tests-" + nodes + "-" + mlt + "-" + tests + " Ended!");
                Console.WriteLine("Sum of time from each task:\n\t{0}\nReal time left:\n\t{1}", elapsed, swGlobal.Elapsed);

                Serialize("tests-"+nodes+"-"+mlt+"-"+tests, networks, stats);
            }
            while (true)
            {
                string s = Console.ReadLine();
                if (s == "stop") stop = true;
                else if (s == "break") break;
            }

            Console.ReadKey();
            return;
        }
        static void GenerateTestNetwork()
        {
            Stopwatch sw = new Stopwatch();
            while (progress + inProgress < tests && !stop)
            {
                sw.Restart();
                inProgress++;
                SFNetwork nw = new SFNetwork(nodes, mlt, rnd.Next());
                Dictionary<int, int> stat = new Dictionary<int, int>();
                for (int i = 0; i < nw.Edges.Count; i++)
                {
                    Edge edg = nw.Edges[i];
                    if (!stat.ContainsKey(edg.From))
                        stat.Add(edg.From, 0);
                    stat[edg.From] += edg.Weight;
                    if (edg.From != edg.To)
                    {
                        if (!stat.ContainsKey(edg.To))
                            stat.Add(edg.To, 0);
                        stat[edg.To] += edg.Weight;
                    }
                }
                sw.Stop();
                elapsed = elapsed.Add(sw.Elapsed);
                lock (networks) lock (stats)
                    {
                        networks.Add(nw);
                        stats.Add(stat);
                        progress++;
                        inProgress--;
                        Console.WriteLine("{0}/{1}\tcurrent:{2}\t\tTotal:{3}", progress, tests, sw.Elapsed, swGlobal.Elapsed);
                    }
            }
            Console.WriteLine("Finished!");
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
