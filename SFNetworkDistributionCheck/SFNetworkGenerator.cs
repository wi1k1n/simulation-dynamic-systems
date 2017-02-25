using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Diploma2.Networks;
using System.Diagnostics;

namespace Diploma2
{
    public class SFNetworkGenerator
    {
        public struct Pair<T1, T2> { public T1 V1; public T2 V2; }

        private int tests = 100, progress = 0, inProgress = 0, nodes = 10000, mlt = 3;
        private List<SFNetwork> networks = new List<SFNetwork>();
        private List<Dictionary<int, int>> stats = new List<Dictionary<int, int>>();
        private object locker = new object();
        private Stopwatch sw = new Stopwatch();
        private Random rnd = new Random();
        private CancellationToken cancel;
        private TimeSpan timeRem = new TimeSpan();

        public TimeSpan TimeLeft { get { return sw.Elapsed; } }
        public TimeSpan TimeRemaining
        {
            get
            {
                if (progress == 0)
                    return TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds * tests);
                return timeRem;
            }
        }
        public double Progress { get { return (double)progress / tests; } }

        public Pair<List<SFNetwork>, List<Dictionary<int, int>>> GenerateSFNetworks(int node_count, int multiplier, int quantity, CancellationToken ct, int threads = 4)
        {
            cancel = ct;

            nodes = node_count;
            mlt = multiplier;
            tests = quantity;
            progress = 0;
            inProgress = 0;
            networks = new List<SFNetwork>();
            stats = new List<Dictionary<int, int>>();

            List<Task> tsks = new List<Task>();
            for (int i = 0; i < threads; i++) tsks.Add(new Task(GenerateTestNetwork, ct));
            sw.Start();
            foreach (Task tsk in tsks) tsk.Start();
            foreach (Task tsk in tsks) tsk.Wait();
            sw.Stop();

            var ret = new Pair<List<SFNetwork>, List<Dictionary<int, int>>>();
            ret.V1 = networks;
            ret.V2 = stats;
            return ret;
        }
        private void GenerateTestNetwork()
        {
            while (progress + inProgress < tests && !cancel.IsCancellationRequested)
            {
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
                lock (networks) lock (stats)
                    {
                        networks.Add(nw);
                        stats.Add(stat);
                        progress++;
                        inProgress--;
                        //Console.WriteLine("{0}/{1}\tTime:\t{2}", progress, tests, sw.Elapsed);
                    }
                timeRem = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds / progress * (tests - progress));
            }
            //Console.WriteLine("Finished!");
        }

        public Dictionary<int, double> GenerateSFNetworksAverage(int node_count, int multiplier, int quantity, CancellationToken ct, int threads = 4)
        {
            GenerateSFNetworks(node_count, multiplier, quantity, ct, threads);
            Dictionary<int, double> ret = new Dictionary<int, double>();
            for (int i = 0; i < stats[0].Count; i++)
            {
                double sum = 0;
                int n = 0;
                for (int j = 0; j < stats.Count; j++) sum = ((sum * n) + stats[j][i]) / ++n;
                ret.Add(i, sum);
            }
            return ret;
        }
    }
}
