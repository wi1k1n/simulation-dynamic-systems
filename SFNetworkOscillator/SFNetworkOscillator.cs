using Diploma2.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Diploma2
{
    [Serializable]
    public class SFNetworkOscillatorState
    {
        public double Time { get; set; }
        public double[] Phases { get; set; }
        public SFNetworkOscillatorState(double time, double[] phases)
        {
            Time = time;
            Phases = phases;
        }
        public override string ToString()
        {
            return "[" + Time + ": " + Phases.Length + " phases]";
        }
    }
    [Serializable]
    public class SFNetworkOscillator : SFNetwork
    {
        private const double pi2 = Math.PI * 2;

        public double Strength { get; private set; }
        public double FrequenciesInitMin { get; private set; }
        public double FrequenciesInitMax { get; private set; }
        public double PhasesInitMin { get; private set; }
        public double PhasesInitMax { get; private set; }
        public double TimeInit { get; private set; }
        public double TimeStep { get; private set; }
        public double SolveStep { get; private set; }

        public double Time { get; private set; }

        ///// Oscillators /////
        public double[] Frequencies { get; private set; }
        public double[] Phases { get; private set; }
        public double[] PhasesInit { get; private set; } // left for compability
        public List<SFNetworkOscillatorState> States { get; private set; }
        ///// Oscillators /////

        private RK4S.RK4SFunc[] funcs = null;

        public SFNetworkOscillator(int node_count, int mlt, double strength, double freqMin, double freqMax, double phaseMin, double phaseMax, double time_init, double time_step, double solve_step) : base(node_count, mlt)
        {
            constructor(strength, time_init, freqMin, freqMax, phaseMin, phaseMax, time_step, solve_step);
        }
        public SFNetworkOscillator(int node_count, int mlt, double strength, double freqMin, double freqMax, double phaseMin, double phaseMax, double time_init, double time_step, double solve_step, int seed) : base(node_count, mlt, seed)
        {
            constructor(strength, time_init, freqMin, freqMax, phaseMin, phaseMax, time_step, solve_step);
        }
        private void constructor(double strength, double time_init, double freqMin, double freqMax, double phaseMin, double phaseMax, double time_step, double solve_step)
        {
            Strength = strength;
            FrequenciesInitMin = freqMin;
            FrequenciesInitMax = freqMax;
            PhasesInitMin = phaseMin;
            PhasesInitMax = phaseMax;
            TimeInit = time_init;
            Time = time_init;
            TimeStep = time_step;
            SolveStep = solve_step;

            Frequencies = new double[Nodes.Count];
            Phases = new double[Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
            {
                Frequencies[i] = ilRandom.NextDouble(freqMin, freqMax);
                Phases[i] = ilRandom.NextDouble(phaseMin, phaseMax);
            }
            States = new List<SFNetworkOscillatorState> { new SFNetworkOscillatorState(Time, Phases) };

            funcs = new RK4S.RK4SFunc[Nodes.Count];
            for (int i = 0; i < funcs.Length; i++)
                funcs[i] = (ind, t, x) =>
                {
                    double res = 0;
                    for (int j = 0; j < x.Length; j++) // Foreach phase
                    {
                        //if (j == ind) continue; // Skip if checking itself
                        int w = 0; // w_ij
                        foreach (Edge edg in Edges)
                            if (edg.From == j && edg.To == ind || edg.From == ind && edg.To == j)
                            {
                                w += edg.Weight;
                                break;
                            }

                        // Iterative part of main equation
                        res += w * Math.Sin(x[j] - x[ind]);
                    }
                    // Main equation
                    return Phases[ind] + Strength * res;
                };
        }

        public void SimulateDynamicStep()
        {
            RK4S.RK4SResult result = RK4S.Solve(funcs, Time, Phases, Time + TimeStep, SolveStep);
            Time = result.Time;
            Phases = result.Values;
            phasesNormalize();
            States.Add(new SFNetworkOscillatorState(Time, Phases));
        }
        
        public new int Serialize(string path)
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
        public static new SFNetworkOscillator Deserialize(string path)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = File.OpenRead(path))
                    return (SFNetworkOscillator)bf.Deserialize(fs);
            }
            catch { }
            return null;
        }

        private void phasesNormalize()
        {
            for (int i = 0; i < Phases.Length; i++)
                Phases[i] = Phases[i] - (int)(Phases[i] / pi2) * pi2;
        }
    }
}
