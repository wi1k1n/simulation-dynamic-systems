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

        public static SFNetworkOscillatorState Debinarize(BinaryReader br)
        {
            double time;
            int phaseCount;
            time = BitConverter.ToDouble(br.ReadBytes(8), 0);
            phaseCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
            List<double> phases = new List<double>();
            for (int i = 0; i < phaseCount; i++)
                phases.Add(BitConverter.ToDouble(br.ReadBytes(8), 0));
            return new SFNetworkOscillatorState(time, phases.ToArray());
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
                Frequencies[i] = ilRandom.Next((int)freqMin, (int)freqMax);
                Phases[i] = ilRandom.NextDouble(phaseMin, phaseMax);
            }
            States = new List<SFNetworkOscillatorState> { new SFNetworkOscillatorState(Time, Phases) };
            constructorFuncs();
        }
        private void constructorFuncs()
        {
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

        public SFNetworkOscillator(int nodeCount, int mlt, int seed, ulong x, Edge[] edges, double strength, double freqMin, double freqMax, double phaseMin, double phaseMax, double time_init, double time_step, double solve_step, double time, double[] phases, double[] freqs, SFNetworkOscillatorState[] states)
            : base(nodeCount, mlt, edges, new ilRand(seed, x))
        {
            Strength = strength;
            FrequenciesInitMin = freqMin;
            FrequenciesInitMax = freqMax;
            PhasesInitMin = phaseMin;
            PhasesInitMax = phaseMax;
            TimeInit = time_init;
            Time = time;
            TimeStep = time_step;
            SolveStep = solve_step;
            Phases = phases;
            Frequencies = freqs;
            States = new List<SFNetworkOscillatorState>(states);
            constructorFuncs();
        }
        public static SFNetworkOscillator Debinarize(string path)
        {
            try
            {
                int version, nodeCount, mlt, seed, edgeCount, phaseCount, freqCount, stateCount;
                ulong x;
                double strength,
                    freq_init_min,
                    freq_init_max,
                    phase_init_min,
                    phase_init_max,
                    time_init,
                    time_step,
                    solve_step,

                    time;
                using (BinaryReader br = new BinaryReader(File.OpenRead(path)))
                {
                    version = br.ReadBytes(1)[0];
                    nodeCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
                    mlt = BitConverter.ToInt32(br.ReadBytes(4), 0);
                    seed = BitConverter.ToInt32(br.ReadBytes(4), 0);
                    x = (ulong)BitConverter.ToInt64(br.ReadBytes(8), 0);

                    edgeCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
                    List<Edge> edges = new List<Edge>();
                    for (int i = 0; i < edgeCount; i++)
                        edges.Add(Edge.Debinarize(br));

                    strength = BitConverter.ToDouble(br.ReadBytes(8), 0);
                    freq_init_min = BitConverter.ToDouble(br.ReadBytes(8), 0);
                    freq_init_max = BitConverter.ToDouble(br.ReadBytes(8), 0);
                    phase_init_min = BitConverter.ToDouble(br.ReadBytes(8), 0);
                    phase_init_max = BitConverter.ToDouble(br.ReadBytes(8), 0);
                    time_init = BitConverter.ToDouble(br.ReadBytes(8), 0);
                    time_step = BitConverter.ToDouble(br.ReadBytes(8), 0);
                    solve_step = BitConverter.ToDouble(br.ReadBytes(8), 0);
                    time = BitConverter.ToDouble(br.ReadBytes(8), 0);

                    phaseCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
                    List<double> phases = new List<double>();
                    for (int i = 0; i < phaseCount; i++)
                        phases.Add(BitConverter.ToDouble(br.ReadBytes(8), 0));

                    freqCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
                    List<double> freqs = new List<double>();
                    for (int i = 0; i < freqCount; i++)
                        freqs.Add(BitConverter.ToDouble(br.ReadBytes(8), 0));

                    stateCount = BitConverter.ToInt32(br.ReadBytes(4), 0);
                    List<SFNetworkOscillatorState> states = new List<SFNetworkOscillatorState>();
                    for (int i = 0; i < stateCount; i++)
                        states.Add(SFNetworkOscillatorState.Debinarize(br));

                    return new SFNetworkOscillator(nodeCount, mlt, seed, x, edges.ToArray(), strength, freq_init_min, freq_init_max, phase_init_min, phase_init_max, time_init, time_step, solve_step, time, phases.ToArray(), freqs.ToArray(), states.ToArray());
                }
            }
            catch (Exception e) { return null; }
        }
    }
}
