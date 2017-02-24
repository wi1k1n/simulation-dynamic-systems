using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histogrammer
{
    class Histogramm<T> where T: IComparable
    {
        public List<T> X { get; set; }
        public List<T> Y { get; set; }

        public Histogramm()
        {
            X = new List<T>();
            Y = new List<T>();
        }
        public Histogramm(T[] x, T[] y)
        {
            X = new List<T>(x);
            Y = new List<T>(y);
        }
    }
}
