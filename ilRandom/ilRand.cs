using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma2
{
    [Serializable]
    public class ilRand
    {
        private const ulong m = 1442695040888963407;
        private const ulong a = 6364136223846793005;

        private ulong x = 0;

        public int Seed { get; private set; }

        public ilRand()
        {
            Seed = (int)(DateTime.Now.ToBinary() % uint.MaxValue);
            x = (ulong)Seed;
        }
        public ilRand(int seed)
        {
            Seed = seed;
            x = (ulong)Seed;
        }
        public ilRand(int seed, ulong x)
        {
            Seed = seed;
            x = x;
        }

        public void ChangeSeed()
        {
            Seed = (int)DateTime.Now.ToBinary();
            x = (ulong)Seed;
        }
        public void SetSeed(int seed)
        {
            Seed = seed;
            x = (ulong)Seed;
        }
        public void Initialize(int seed, ulong state)
        {
            Seed = seed;
            x = state;
        }

        public int Next()
        {
            return Next(0, int.MaxValue);
        }
        public int Next(int max)
        {
            return Next(0, max);
        }
        public int Next(int min, int max)
        {
            return (int)(next() % int.MaxValue) % (max - min) + min;
        }

        public double NextDouble()
        {
            return NextDouble(0, double.MaxValue);
        }
        public double NextDouble(double max)
        {
            return NextDouble(0, max);
        }
        public double NextDouble(double min, double max)
        {
            return (next() / (double)m) * (max - min) + min;
        }

        private ulong next()
        {
            x = ((x * a) % m);
            return x;
        }
    }
}
