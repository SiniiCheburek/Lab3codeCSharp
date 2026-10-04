using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5
{
    public sealed class LinearInterpolation : Interpolation
    {
        public override double Calculate(IReadOnlyList<PointD> points, double xx)
        {
            if (points == null || points.Count < 2)
                throw new ArgumentException("Потрібно щонайменше 2 точки.");

            if (xx < points[0].x || xx > points[^1].x)
                throw new ArgumentOutOfRangeException(nameof(xx));

            int i = FindSegment(points, xx);

            PointD p1 = points[i];
            PointD p2 = points[i + 1];

            double dx = p2.x - p1.x;

            if (Math.Abs(dx) < 1e-12)
                throw new ArgumentException("Однакові значення X заборонені.");

            double t = (xx - p1.x) / dx;

            return p1.y + t * (p2.y - p1.y);
        }

        private static int FindSegment(IReadOnlyList<PointD> points,
            double x)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (x >= points[i].x && x <= points[i + 1].x)
                    return i;
            }

            return points.Count - 2;
        }
    }
}
