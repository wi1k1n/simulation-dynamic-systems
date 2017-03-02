using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Diploma2
{
    namespace Networks
    {
        [Serializable]
        public class Edge
        {
            public int From { get; protected set; }
            public int To { get; protected set; }
            public int Weight { get; set; }

            public Edge() { }
            public Edge(int from, int to)
            {
                From = from;
                To = to;
            }
            public Edge(int from, int to, int weight)
            {
                From = from;
                To = to;
                Weight = weight;
            }
            public Edge(int weight)
            {
                Weight = weight;
            }

            public override string ToString()
            {
                return "(" + From + "; " + To + "): " + Weight;
            }
        }

        [Serializable]
        public abstract class Network
        {
            public List<Edge> Edges { get; protected set; }
            public Dictionary<int, int> Nodes { get; protected set; }

            public Network()
            {
                Edges = new List<Edge>();
                Nodes = new Dictionary<int, int>();
            }
            public Network(IEnumerable<Edge> edges)
            {
                Edges = new List<Edge>(edges);
                NodesRecalculate();
            }

            protected void NodesRecalculate()
            {
                Nodes = new Dictionary<int, int>();
                foreach (Edge edg in Edges)
                {
                    if (!Nodes.ContainsKey(edg.From))
                        Nodes.Add(edg.From, 0);
                    Nodes[edg.From]++;
                    if (edg.From == edg.To) continue;
                    if (!Nodes.ContainsKey(edg.To))
                        Nodes.Add(edg.To, 0);
                    Nodes[edg.To]++;
                }
            }
        }
    }
}
