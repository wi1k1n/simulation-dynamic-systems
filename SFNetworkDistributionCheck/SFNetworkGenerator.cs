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
        private Dictionary<int, double> stats = new Dictionary<int, double>();
        private int nwCounter = 0;
        private object locker = new object();
        private Stopwatch sw = new Stopwatch();
        private ilRand rnd = new ilRand();
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
        
        /// <summary>
        /// Generates a number of SFNetworks to store an average distribution of quantity of SFNetwork-nodes by their degree
        /// </summary>
        /// <param name="node_count">The quantity of nodes for each SFNetwork</param>
        /// <param name="multiplier">The multiplier for edges: qtyEdges = mlt * qtyNodes</param>
        /// <param name="quantity">The number of networks that need to be generated</param>
        /// <param name="ct">CancelToken - the token to stop calculating without loosing already calculated data</param>
        /// <param name="threads">The number of threads that will be used to increase the effectiveness of pc</param>
        /// <returns>List of pairs. Key - the degree of node, Value - quantity of nodes with this degree</returns>
        public Dictionary<int, double> GenerateSFNetworksAverage(int node_count, int multiplier, int quantity, CancellationToken ct, int threads = 4)
        {
            cancel = ct;

            nodes = node_count;
            mlt = multiplier;
            tests = quantity;
            progress = 0;
            inProgress = 0;
            nwCounter = 0;
            stats = new Dictionary<int, double>(node_count);
            for (int i = 0; i < node_count; i++) stats.Add(i, 0);

            List<Task> tsks = new List<Task>();
            for (int i = 0; i < threads; i++) tsks.Add(new Task(GenerateTestNetwork, ct));
            sw.Start();
            foreach (Task tsk in tsks) tsk.Start();
            foreach (Task tsk in tsks) tsk.Wait();
            sw.Stop();

            return stats;
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
                lock (stats)
                {
                    for (int i = 0; i < stats.Count; i++)
                        stats[i] = (stats[i] * nwCounter + stat[i]) / (nwCounter + 1);
                    nwCounter++;
                    progress++;
                    inProgress--;
                    //Console.WriteLine("{0}/{1}\tTime:\t{2}", progress, tests, sw.Elapsed);
                }
                timeRem = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds / progress * (tests - progress));
            }
            //Console.WriteLine("Finished!");
        }
    }
}
