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
            using (BinaryReader br = new BinaryReader(File.OpenRead(@"D:/MISiS/НИР/8с Анализ безмасштабной сети/Project/Diploma2/CPPSFNetwork/network.bin")))
            {
                int nodeCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
                int mlt = BitConverter.ToInt32(br.ReadBytes(4), 0);
                int seed = BitConverter.ToInt32(br.ReadBytes(4), 0);
                ulong x = (ulong)BitConverter.ToInt64(br.ReadBytes(8), 0);
                int edgeCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
                for (int i = 0; i < edgeCount; i++)
                    Console.WriteLine(new Edge(BitConverter.ToInt32(br.ReadBytes(4), 0), BitConverter.ToInt32(br.ReadBytes(4), 0), BitConverter.ToInt32(br.ReadBytes(4), 0)));
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
