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
        static void Main(string[] args)
        {
            Console.WriteLine("Hit:\n\t0: Calculate dynamics on SFNetwork\n\t1: Serialize manager");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.KeyChar == '0')
                CalculateSFNetworkDynamics();
            if (key.KeyChar == '1')
                SerializeManager();
        }

        private static void CalculateSFNetworkDynamics()
        {
            Console.CursorLeft = 0;
            Console.WriteLine(" ");
            SFNetworkOscillator nw = new SFNetworkOscillator(100, 3, .65, 1, 10, -Math.PI, Math.PI, 0, .1, .001, 170302);
            Stopwatch swGlobal = new Stopwatch();
            CancellationTokenSource ctSrc = new CancellationTokenSource();
            Task task = new Task(() =>
            {
                Console.WriteLine("Time: {0:F5}\t0\t0", nw.Time);
                Stopwatch sw = new Stopwatch();
                while (!ctSrc.IsCancellationRequested)
                {
                    sw.Restart();
                    nw.SimulateDynamicStep();
                    sw.Stop();
                    swGlobal.Stop();
                    Console.WriteLine("Time: {0:F5}\t{1}\t{2}", nw.Time, sw.Elapsed, swGlobal.Elapsed);
                    swGlobal.Start();
                }
                swGlobal.Stop();
                Console.WriteLine("Calculating finished in {0}\n{1} states calculated from t={2} to t={3}", swGlobal.Elapsed, nw.States.Count, nw.States[0].Time, nw.Time);
                string path = "SFNetworkOscillator_" + nw.Nodes.Count + "_" + nw.Multiplier + "_" + nw.Strength + "_" + nw.FrequenciesInitMin + "_" + nw.FrequenciesInitMax + "_" + nw.PhasesInitMin + "_" + nw.PhasesInitMax + "_" + nw.TimeInit + "_" + nw.TimeStep + "_" + nw.SolveStep + "_" + nw.RandomSeed + "_" + nw.States.Count + ".sfnwosc";
                nw.Serialize(path);
                Console.WriteLine("Serializing finished successfully!\nNetwork saved by path: {0}\nFile size: {1} bytes", path, new FileInfo(path).Length);
            }, ctSrc.Token);
            task.Start();
            swGlobal.Start();

            bool cliStop = false;
            while (!cliStop)
            {
                string s = Console.ReadLine();
                if (s == "stop")
                    ctSrc.Cancel();
            }
        }

        private static void SerializeManager()
        {
            throw new NotImplementedException();
        }
    }
}
