using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5
{
    public abstract class Interpolation
    {
        public abstract double Calculate(
            IReadOnlyList<PointD> points, double xx);
    }
}
