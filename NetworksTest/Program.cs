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
            SFNetworkOscillator nw = SFNetworkOscillator.Debinarize(@"D:/MISiS/НИР/8с Анализ безмасштабной сети/Project/Diploma2/x64/Release/network_50_3.bin");
            for (int i = 0; i < 5; i++)
            {
                nw.SimulateDynamicStep();
                Console.WriteLine("time: {0}", nw.Time);
            }
            Console.WriteLine("\ntime: {0}", nw.Time);
            foreach (double d in nw.Phases)
                Console.WriteLine(d);
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
