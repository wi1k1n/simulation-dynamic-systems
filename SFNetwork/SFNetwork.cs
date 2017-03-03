using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diploma2.Networks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Diploma2
{
    [Serializable]
    public class SFNetwork : Network
    {
        protected ilRand ilRandom = null;

        public int Multiplier { get; private set; }
        public double RandomSeed { get; private set; }

        public SFNetwork() { }
        public SFNetwork(int node_count, int edge_multiplier)
        {
            ilRandom = new ilRand();
            RandomSeed = ilRandom.Seed;
            Generate(node_count, edge_multiplier);
        }
        public SFNetwork(int node_count, int edge_multiplier, int seed)
        {
            ilRandom = new ilRand(seed);
            RandomSeed = ilRandom.Seed;
            Generate(node_count, edge_multiplier);
        }
        private void Generate(int node_count, int edge_multiplier)
        {
            Multiplier = edge_multiplier;
            Edges = new List<Edge>();

            // Generating LCD diagram
            int m = node_count * edge_multiplier,
                l = 2 * m; // Length of lcd to create G(n, k)
            List<int> alphabet = new List<int>(); // Alphabet to get random nonreccuring integers from it
            for (int i = 0; i < l; i++) alphabet.Add(i);
            int[] lcd = Enumerable.Repeat(-1, l).ToArray(); // The main lcd diagram array
            for (int i = 0; i < m; i++)
            {
                // Searching random values from the alphabet for both starting and ending of lcd-edge
                int r1 = alphabet[ilRandom.Next(0, alphabet.Count)];
                alphabet.Remove(r1);
                //ilRandom.Next(); // Do not rly need, but w/o it lcd diagrams seems too similar
                int r2 = alphabet[ilRandom.Next(0, alphabet.Count)];
                alphabet.Remove(r2);
                lcd[Math.Max(r1, r2)] = Math.Min(r1, r2);
            }

            // Corresponding lcd-nodes to network-nodes
            int[] nodeMarker = new int[l];
            int kCnt = 0,
                globalNode = 0;
            for (int i = 0; i < l; i++)
            {
                nodeMarker[i] = globalNode;
                if (lcd[i] > -1 && ++kCnt == 3)
                {
                    globalNode++;
                    kCnt = 0;
                }
            }

            // Corresponding lcd-edges to network-edges
            for (int i = 0; i < l; i++)
            {
                if (lcd[i] == -1) continue;
                Edge edg = new Edge(nodeMarker[i], nodeMarker[lcd[i]], 1);
                if (!Edges.Any(x => { return x.From == edg.From && x.To == edg.To; })) Edges.Add(edg);
                else Edges.Find(x => { return x.From == edg.From && x.To == edg.To; }).Weight++;
            }
            NodesRecalculate();
        }

        public int Serialize(string path)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = File.OpenWrite(path))
                    bf.Serialize(fs, this);
            }
            catch
            {
                return 1;
            }
            return 0;
        }
        public static SFNetwork Deserialize(string path)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = File.OpenRead(path))
                    return (SFNetwork)bf.Deserialize(fs);
            }
            catch { }
            return null;
        }
    }
}
