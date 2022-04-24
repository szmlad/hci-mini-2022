using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace HCI.Chart
{
    internal class LineChart
    {
        private readonly Canvas Canvas;
        private double MinValue = 0.0;
        private double MaxValue = 0.0;
        private List<double> _Values = new();

        public uint GridRows { get; set; } = 0;
        public uint GridCols { get; set; } = 0;
        public Brush Stroke { get; set; } = Brushes.Black;
        public List<double> Values
        {
            get => Values;
            set
            {
                _Values = value;
                MinValue = value.Min();
                MaxValue = value.Max();
            }
        }
        public List<string> Labels { get; set; } = new();

        public LineChart(Canvas canvas)
        {
            Canvas = canvas;
        }

        public void Draw()
        {
            DrawGrid();
        }

        public void DrawGrid()
        {
            if (GridRows >= 2)
            {
                var yLabel = MaxValue;
                var yStep = (MaxValue - MinValue) / (GridRows + 1);
                var height = Canvas.Height / GridRows;
                var y = height;
                Canvas.Children.Add(new TextBlock()
                {
                    Text = String.Format("{0:N2}", yLabel),
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransform = new TranslateTransform(-45, -12),
                });
                yLabel -= yStep;
                for (uint i = 1; i < GridRows; ++i)
                {
                    Canvas.Children.Add(new Line()
                    {
                        X1 = 0, Y1 = y,
                        X2 = Canvas.Width, Y2 = y,
                        Stroke = Brushes.DimGray,
                        StrokeThickness = 1,
                        StrokeDashArray = new() { 3.0, 3.0 },
                    });
                    Canvas.Children.Add(new TextBlock()
                    {
                        Text = string.Format("{0:N2}", yLabel),
                        FontSize = 12,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransform = new TranslateTransform(-45, y - 12),
                    });
                    y += height;
                    yLabel -= yStep;
                }
                Canvas.Children.Add(new TextBlock()
                {
                    Text = string.Format("{0:N2}", yLabel),
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransform = new TranslateTransform(-45, Canvas.Height - 12),
                });
            }

            if (GridCols >= 2)
            {
                var xLabels = Sample(Labels, GridCols + 1);
                var width = Canvas.Width / GridCols;
                var x = width;
                Canvas.Children.Add(new TextBlock()
                {
                    Text = xLabels[0],
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    RenderTransform = new TranslateTransform(6, Canvas.Height + 5),
                });
                for (uint i = 1; i < GridCols; ++i)
                {
                    Canvas.Children.Add(new Line()
                    {
                        X1 = x, Y1 = 0,
                        X2 = x, Y2 = Canvas.Height,
                        Stroke = Brushes.DimGray,
                        StrokeThickness = 1,
                        StrokeDashArray = new() { 3.0, 3.0 }
                    });
                    Canvas.Children.Add(new TextBlock()
                    {
                        Text = xLabels[Convert.ToInt32(i)],
                        FontSize = 12,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        RenderTransform = new TranslateTransform(x + 6, Canvas.Height + 5),
                    });
                    x += width;
                }
                Canvas.Children.Add(new TextBlock()
                {
                    Text = xLabels[Convert.ToInt32(GridCols)],
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    RenderTransform = new TranslateTransform(Canvas.Width + 6, Canvas.Height + 5),
                });
            }
        }

        public void DrawLine(List<double> xs, List<double> ys)
        {
            var width = Canvas.Width / (xs.Count - 1);
            var maxValue = ys.Max();
            var minValue = ys.Min();

            if (ApproxEquals(minValue, maxValue, 5e-6))
            {
                Canvas.Children.Add(new Line()
                {
                    X1 = 0,
                    Y1 = minValue,
                    X2 = Canvas.Width,
                    Y2 = minValue,
                    Stroke = Brushes.DeepPink,
                    StrokeThickness = 2,
                });
                return;
            }

            var previous = Canvas.Height - Scale(ys[0], minValue, maxValue, 0, Canvas.Height);
            for (int i = 1; i < ys.Count; ++i)
            {
                var current = Canvas.Height - Scale(ys[i], minValue, maxValue, 0, Canvas.Height);
                Canvas.Children.Add(new Line()
                {
                    X1 = (i - 1) * width,
                    Y1 = previous,
                    X2 = i * width,
                    Y2 = current,
                    Stroke = Brushes.DeepPink,
                    StrokeThickness = 2,
                });
                previous = current;
            }
        }

        private static bool ApproxEquals(double a, double b, double eps) =>
            Math.Abs(a - b) <= eps;

        private static double Scale(double value, double min, double max, double from, double to) =>
            from + (value - min) * (to - from) / (max - min);

        private static List<string> Sample(List<string> xs, uint count)
        {
            var skip = xs.Count / count;
            var sampled = new List<string>() { xs[0] };
            for (int i = 1; i <= xs.Count; ++i)
            {
                if (i % skip == 0) sampled.Add(xs[i - 1]);
            }
            return sampled;
        }
    }
}
