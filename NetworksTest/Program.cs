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
            int min = 0, max = 10, seed = 170303, n = 100;
            //Console.WriteLine("Enter min"); min = int.Parse(Console.ReadLine());
            //Console.WriteLine("Enter max"); min = int.Parse(Console.ReadLine());
            //Console.WriteLine("Enter seed"); min = int.Parse(Console.ReadLine());
            //Console.WriteLine("Enter n"); min = int.Parse(Console.ReadLine());

            ilRand rnd = new ilRand(seed);
            for (int i = 0; i < n; i++)
                Console.WriteLine(rnd.Next(-50, 50));
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
