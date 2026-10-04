using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private double hh1 = 0;
        private double hh2 = 0;

        private bool showReference2 = true;
        private bool showLinear2 = true;
        private bool showHermite2 = true;
        private bool showPoints2 = true;
        private bool showError2 = false;

        public ObservableCollection<PointD> ReferencePoints { get; } = new();
        public ObservableCollection<PointD> LinearPoints { get; } = new();
        public ObservableCollection<PointD> HermitePoints { get; } = new();
        public ObservableCollection<PointD> ReferenceFunction { get; } = new();
        public ObservableCollection<PointD> ErrorLinear { get; } = new();
        public ObservableCollection<PointD> ErrorHermite { get; } = new();

        public double H1
        {
            get => hh1;
            set
            {
                if (hh1 == value)
                    return;

                hh1 = value;
                OnPropertyChanged();
                Recalculate();
            }
        }

        public double H2
        {
            get => hh2;
            set
            {
                if (hh2 == value)
                    return;

                hh2 = value;
                OnPropertyChanged();
                Recalculate();
            }
        }

        public bool ShowReference
        {
            get => showReference2;
            set
            {
                showReference2 = value;
                OnPropertyChanged();
            }
        }

        public bool ShowLinear
        {
            get => showLinear2;
            set
            {
                showLinear2 = value;
                OnPropertyChanged();
            }
        }

        public bool ShowHermite
        {
            get => showHermite2;
            set
            {
                showHermite2 = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPoints
        {
            get => showPoints2;
            set
            {
                showPoints2 = value;
                OnPropertyChanged();
            }
        }

        public bool ShowError
        {
            get => showError2;
            set
            {
                showError2 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            Recalculate();
        }
        public void Recalculate()
        {
            if (H1 <= 0 || H2 <= 0 || H2 >= H1)
                return;

            ReferencePoints.Clear();
            LinearPoints.Clear();
            HermitePoints.Clear();
            ReferenceFunction.Clear();
            ErrorLinear.Clear();
            ErrorHermite.Clear();

            const double start = 0;
            const double end = 2 * Math.PI;

            for (double x = start; x <= end + 1e-10; x += H1)
            {
                double xx = Math.Min(x, end);
                ReferencePoints.Add(
                    new PointD(xx, Math.Sin(xx)));
            }

            if (ReferencePoints[^1].x < end - 1e-10)
            {
                ReferencePoints.Add(
                    new PointD(end, Math.Sin(end)));
            }

            Interpolation[] interpolations =
            {
                new LinearInterpolation(),
                new HermiteInterpolation()
            };

            foreach (var interpolation in interpolations)
            {
                for (double x = start; x <= end + 1e-10; x += H2)
                {
                    double xx = Math.Min(x, end);

                    double y = interpolation.Calculate(
                        ReferencePoints,
                        xx);

                    if (interpolation is LinearInterpolation)
                        LinearPoints.Add(new PointD(xx, y));
                    else
                        HermitePoints.Add(new PointD(xx, y));
                }

                if (interpolation is LinearInterpolation)
                    AddEndPoint(LinearPoints, end, Math.Sin(end));
                else
                    AddEndPoint(HermitePoints, end, Math.Sin(end));
            }

            double referenceStep = H2 / 2;

            for (double x = start;
                 x <= end + 1e-10;
                 x += referenceStep)
            {
                double xx = Math.Min(x, end);

                ReferenceFunction.Add(
                    new PointD(xx, Math.Sin(xx)));
            }

            AddEndPoint(
                ReferenceFunction,
                end,
                Math.Sin(end));

            foreach (var p in LinearPoints)
            {
                double error =
                    Math.Abs(p.y - Math.Sin(p.x));

                ErrorLinear.Add(
                    new PointD(p.x, error));
            }

            foreach (var p in HermitePoints)
            {
                double error =
                    Math.Abs(p.y - Math.Sin(p.x));

                ErrorHermite.Add(
                    new PointD(p.x, error));
            }

            OnPropertyChanged(nameof(ReferencePoints));
            OnPropertyChanged(nameof(LinearPoints));
            OnPropertyChanged(nameof(HermitePoints));
            OnPropertyChanged(nameof(ReferenceFunction));
            OnPropertyChanged(nameof(ErrorLinear));
            OnPropertyChanged(nameof(ErrorHermite));
        }

        private static void AddEndPoint(
            ObservableCollection<PointD> collection,
            double x,
            double y)
        {
            if (collection.Count == 0 ||
                Math.Abs(collection[^1].x - x) > 1e-10)
            {
                collection.Add(new PointD(x, y));
            }
        }

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
