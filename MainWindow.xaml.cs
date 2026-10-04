using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp5
{
    public partial class MainWindow : Window
    {
        private readonly Interpolation[] interpolations =
        {
            new LinearInterpolation(),
            new HermiteInterpolation()
        };

        private List<PointD> supportPoints = new();

        private const double XMin = 0;
        private const double XMax = 2 * Math.PI;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BuildGraph();
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            BuildGraph();
        }

        private void BuildGraph()
        {
            string h1Text = H1TextBox.Text.Replace(',', '.');
            string h2Text = H2TextBox.Text.Replace(',', '.');

            if (!double.TryParse(
                    h1Text,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double h1) ||
                !double.TryParse(
                    h2Text,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double h2))
            {
                MessageBox.Show("Введіть правильні числові значення h1 та h2.");
                return;
            }

            if (h1 <= 0 || h2 <= 0)
            {
                MessageBox.Show("h1 та h2 повинні бути більшими за 0.");
                return;
            }

            if (h2 >= h1)
            {
                MessageBox.Show("Повинно виконуватися h2 < h1.");
                return;
            }


            supportPoints.Clear();

            for (double x = XMin; x < XMax; x += h1)
            {
                supportPoints.Add(new PointD(x, Math.Sin(x)));
            }

            supportPoints.Add(new PointD(XMax, Math.Sin(XMax)));



            ReferencePolyline.Points.Clear();
            LinearPolyline.Points.Clear();
            HermitePolyline.Points.Clear();



            for (int i = 0; i <= 500; i++)
            {
                double x = XMin + (XMax - XMin) * i / 500.0;

                double y = Math.Sin(x);

                ReferencePolyline.Points.Add(ToCanvasPoint(x, y));
            }



            for (double x = XMin; x < XMax; x += h2)
            {
                double y = interpolations[0].Calculate(supportPoints, x);

                LinearPolyline.Points.Add(ToCanvasPoint(x, y));
            }

            LinearPolyline.Points.Add(ToCanvasPoint(XMax, interpolations[0].Calculate(supportPoints, XMax)));



            for (double x = XMin; x < XMax; x += h2)
            {
                double y = interpolations[1].Calculate(supportPoints, x);

                HermitePolyline.Points.Add(ToCanvasPoint(x, y));
            }

            HermitePolyline.Points.Add(ToCanvasPoint(XMax, interpolations[1].Calculate(supportPoints, XMax)));

            DrawSupportPoints();

            UpdateVisibility();
        }

        private Point ToCanvasPoint(double x, double y)
        {
            double width = PlotCanvas.ActualWidth;
            double height = PlotCanvas.ActualHeight;

            double margin = 40;

            double canvasX = margin + (x - XMin) / (XMax - XMin) * (width - 2 * margin);

            double canvasY = height - margin - (y + 1) / 2.0 * (height - 2 * margin);

            return new Point(canvasX, canvasY);
        }

        private void DrawSupportPoints()
        {
            for (int i = PlotCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (PlotCanvas.Children[i] is Ellipse)
                {
                    PlotCanvas.Children.RemoveAt(i);
                }
            }

            foreach (PointD p in supportPoints)
            {
                Point point = ToCanvasPoint(p.x, p.y);

                Ellipse ellipse = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = Brushes.Black
                };

                Canvas.SetLeft(ellipse, point.X - 4);

                Canvas.SetTop(ellipse, point.Y - 4);

                PlotCanvas.Children.Add(ellipse);
            }
        }

        private void UpdateVisibility()
        {
            ReferencePolyline.Visibility = ReferenceCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            LinearPolyline.Visibility = LinearCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            HermitePolyline.Visibility = HermiteCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (PlotCanvas == null)
                return;

            UpdateVisibility();
        }
    }
}