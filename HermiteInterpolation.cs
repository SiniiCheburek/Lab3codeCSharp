using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5
{
    public sealed class HermiteInterpolation : Interpolation
    {
        public override double Calculate(IReadOnlyList<PointD> points,
        double xx)
        {
            if (points == null || points.Count < 2)
                throw new ArgumentException("Потрібно щонайменше 2 точки.");

            if (xx < points[0].x || xx > points[^1].x)
                throw new ArgumentOutOfRangeException(nameof(xx));

            int i = FindSegment(points, xx);

            PointD p0 = points[i];
            PointD p1 = points[i + 1];

            double dx = p1.x - p0.x;

            if (Math.Abs(dx) < 1e-12)
                throw new ArgumentException("Однакові значення X заборонені.");

            double m0 = Derivative(points, i);
            double m1 = Derivative(points, i + 1);

            double t = (xx - p0.x) / dx;

            double t2 = t * t;
            double t3 = t2 * t;

            double h00 = 2 * t3 - 3 * t2 + 1;
            double h10 = t3 - 2 * t2 + t;
            double h01 = -2 * t3 + 3 * t2;
            double h11 = t3 - t2;

            return
                h00 * p0.y +
                h10 * dx * m0 +
                h01 * p1.y +
                h11 * dx * m1;
        }

        private static double Derivative(IReadOnlyList<PointD> points,
            int i)
        {

            if (i == 0)
            {
                return (points[1].y - points[0].y) /
                       (points[1].x - points[0].x);
            }

            if (i == points.Count - 1)
            {
                int n = points.Count - 1;

                return (points[n].y - points[n - 1].y) /
                       (points[n].x - points[n - 1].x);
            }

            return (points[i + 1].y - points[i - 1].y) /
                   (points[i + 1].x - points[i - 1].x);
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
