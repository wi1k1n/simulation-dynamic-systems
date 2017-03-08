using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thr = System.Threading;

namespace Diploma2
{
    public static class RK4S
    {
        // http://www.codenet.ru/progr/alg/Runge-Kutt-Method/

        public class RK4SResult
        {
            public double Time { get; private set; }
            public double[] Values { get; private set; }
            public RK4SResult(double time, double[] values) { Time = time; Values = values; }
        }

        public delegate double RK4SFunc(int ind, double t, params double[] x);

        private static double sixth = 1.0 / 6.0;
        private static double[] koefs = new double[] { 0, 0.5, 0.5, 1 };

        public static RK4SResult Solve(RK4SFunc[] funcs, double t_init, double[] vals_init, double t_target, double t_step)
        {
            double t_current = t_init;
            double[] vals_current = new double[vals_init.Length];
            vals_init.CopyTo(vals_current, 0);
            double[][] rates = new double[5][];
            for (int i = 0; i < 5; i++)
                rates[i] = new double[vals_init.Length];
            while (t_current <= t_target)
            {
                for (int i = 1; i < 5; i++)
                    ratesCalc2(
                        funcs,
                        t_current,
                        vals_current.ToArray(),
                        t_step,
                        rates[i - 1],
                        rates[i],
                        koefs[i - 1]
                    );
                for (int i = 0; i < vals_init.Length; i++)
                    vals_current[i] += sixth * (rates[1][i] + 2 * rates[2][i] + 2 * rates[3][i] + rates[4][i]);
                t_current += t_step;
            }
            return new RK4SResult(t_current, vals_current);
        }
        
        private static void ratesCalc2(RK4SFunc[] funcs, double t_current, double[] vals_current, double t_step, double[] rates_prev, double[] rates, double k)
        {
            double[] vals_new = new double[vals_current.Length];
            for (int i = 0; i < vals_current.Length; i++) vals_new[i] = vals_current[i] + k * rates_prev[i];
            double t_new = t_current + k * t_step;
            for (int i = 0; i < vals_current.Length; i++) rates[i] = funcs[i](i, t_new, vals_new) * t_step;
        }
    }
}
