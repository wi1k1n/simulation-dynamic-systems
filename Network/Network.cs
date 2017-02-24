using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma2
{
    namespace Networks
    {
        [Serializable]
        public class Edge
        {
            public int From { get; set; }
            public int To { get; set; }
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
        }

        [Serializable]
        public abstract class Network
        {
            private int nodeCount = 0;
            public int NodeCount
            {
                get
                {
                    List<int> nodes = new List<int>();
                    foreach (Edge edg in Edges)
                    {
                        if (!nodes.Contains(edg.From))
                            nodes.Add(edg.From);
                        if (!nodes.Contains(edg.To))
                            nodes.Add(edg.To);
                    }
                    return nodes.Count;
                }
            }
            public List<Edge> Edges { get; set; }

            public Network()
            {
                Edges = new List<Edge>();
            }
            public Network(IEnumerable<Edge> edges)
            {
                Edges = new List<Edge>(edges);
            }
        }
    }
}
