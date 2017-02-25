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

            public void Serialize(Stream str)
            {
                str.Write(BitConverter.GetBytes(From), 0, 4);
                str.Write(BitConverter.GetBytes(To), 0, 4);
                str.Write(BitConverter.GetBytes(Weight), 0, 4);
            }

            public override string ToString()
            {
                return "(" + From + "; " + To + "): " + Weight;
            }
        }

        [Serializable]
        public abstract class Network
        {
            // TODO: Change nodeCount when Edges is changed
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

            public void Serialize(Stream str)
            {
                str.Write(BitConverter.GetBytes(nodeCount), 0, 4);
                foreach (Edge edg in Edges)
                    edg.Serialize(str);
            }
        }
    }
}
