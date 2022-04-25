using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using HCI.Util;
using System.Windows.Documents;

namespace HCI.Chart
{
    internal class LineChart<LabelType, ValueType, MarkerType>
    {
        private static readonly List<Brush> Colors = new()
        {
            Brushes.DeepPink,
            Brushes.DarkGreen,
            Brushes.Crimson,
            Brushes.Black,
        };

        private readonly Canvas Canvas;

        private double GlobalMax = double.MinValue;
        private double GlobalMin = double.MaxValue;

        public int GridRows { get; set; } = 1;
        public int GridCols { get; set; } = 1;

        public Dictionary<LabelType, ListTransform<ValueType, double>> Ys;
        public ListTransform<MarkerType, string> Xs;

        public LineChart(Canvas canvas)
        {
            Canvas = canvas;
        }

        public void Draw()
        {
            Canvas.Children.Clear();
            GlobalMaxima(1.05);
            DrawGrid();
            AddMapLegend();

            var i = 0;
            foreach (var kv in Ys)
            {
                DrawLine(kv.Value.Apply().ToList(), kv.Key, Colors[i % Colors.Count]);
                ++i;
            }
        }

        public void DrawGrid()
        {
            if (GridRows >= 2)
            {
                var yLabel = GlobalMax;
                var yLabelStep = (GlobalMax - GlobalMin) / GridRows;
                var yStep = Canvas.Height / GridRows;
                var y = yStep;
                Canvas.Children.Add(new TextBlock()
                {
                    Text = String.Format("{0:N2}", yLabel),
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransform = new TranslateTransform(-45, -12),
                });
                yLabel -= yLabelStep;
                for (int i = 1; i < GridRows; ++i)
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
                    y += yStep;
                    yLabel -= yLabelStep;
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
                var xLabels = Sample();
                var xStep = Canvas.Width / GridCols;
                var x = xStep;
                Canvas.Children.Add(new TextBlock()
                {
                    Text = xLabels[0],
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    RenderTransform = new TranslateTransform(-5, Canvas.Height + 5),
                });
                for (int i = 1; i < GridCols; ++i)
                {
                    Canvas.Children.Add(new Line()
                    {
                        X1 = x, Y1 = 0,
                        X2 = x, Y2 = Canvas.Height,
                        Stroke = Brushes.DimGray,
                        StrokeThickness = 1,
                        StrokeDashArray = new() { 3.0, 3.0 },
                    });
                    Canvas.Children.Add(new TextBlock()
                    {
                        Text = xLabels[i],
                        FontSize = 12,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        RenderTransform = new TranslateTransform(x - 5, Canvas.Height + 5),
                    });
                    x += xStep;
                }
                Canvas.Children.Add(new TextBlock()
                {
                    Text = xLabels[GridCols],
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    RenderTransform = new TranslateTransform(Canvas.Width - 5, Canvas.Height + 5),
                });
            }
        }

        private void AddMapLegend()
        {
            var legend = new TextBlock()
            {
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransform = new TranslateTransform(0, -20)
            };
            var i = 0;
            foreach (var k in Ys.Keys)
            {
                legend.Inlines.Add(new Run()
                {
                    Text = "—— ",
                    Foreground = Colors[i],
                    FontWeight = FontWeights.ExtraBold,
                });
                legend.Inlines.Add(new Run()
                {
                    Text = k.ToString() + "  ",
                    Foreground = Brushes.Black,
                });
                ++i;
            }
            Canvas.Children.Add(legend);
        }

        private void DrawLineSegment(double x1, double y1, double x2, double y2, Brush color, string tooltipText)
        {
            var midX = (x1 + x2) / 2;
            var midY = (y1 + y2) / 2;

            var line = new Line()
            {
                X1 = x1, Y1 = y1, X2 = x2, Y2 = y2,
                Stroke = color,
                StrokeThickness = 3,
            };
            var ellipse = new Ellipse()
            {
                Width = 10, Height = 10,
                Fill = color,
                Tag = line,
                RenderTransform = new TranslateTransform(midX - 5, midY - 5),
            };
            var tooltip = new TextBlock()
            {
                Text = tooltipText,
                FontSize = 12,
                RenderTransform = new TranslateTransform(midX - 5, midY > 40 ? midY - 20 : midY + 20),
            };
            line.MouseEnter += (sender, e) =>
            {
                if (!Canvas.Children.Contains(ellipse))
                {
                    Canvas.Children.Add(ellipse);
                    Canvas.Children.Add(tooltip);
                }
            };
            ellipse.MouseLeave += (sender, e) =>
            {
                foreach (var el in Canvas.Children.OfType<Ellipse>())
                {
                    if (ReferenceEquals(ellipse, el))
                    {
                        Canvas.Children.Remove(el);
                        break;
                    }
                }
                foreach (var tt in Canvas.Children.OfType<TextBlock>())
                {
                    if (ReferenceEquals(tt, tooltip))
                    {
                        Canvas.Children.Remove(tt);
                        break;
                    }
                }
            };
            Canvas.Children.Add(line);
        }
        
        private void DrawLine(List<double> values, LabelType label, Brush color)
        {
            ExtremaByLine(values, out double lineMin, out double lineMax);
            if (ApproxEquals(lineMin, lineMax, 5e-6))
            {
                DrawLineSegment(
                    x1: 0, y1: lineMin, 
                    x2: Canvas.Width, y2: lineMin,
                    color: color,
                    tooltipText: $"{(lineMax + lineMin) / 2:N2} ({label})");
                return;
            }

            var step = Canvas.Width / values.Count;
            var prev = Canvas.Height - Scale(values[0], GlobalMin, GlobalMax, 0, Canvas.Height);
            for (int i = 1; i < values.Count; ++i)
            {
                var curr = Canvas.Height - Scale(values[i], GlobalMin, GlobalMax, 0, Canvas.Height);
                DrawLineSegment(
                    x1: (i - 1) * step, y1: prev,
                    x2: i * step, y2: curr,
                    color: color,
                    tooltipText: $"{(values[i] + values[i - 1]) / 2:N2} ({label})");
                prev = curr;
            }
        }

        private void GlobalMaxima(double paddingScale = 1.0)
        {
            var minmax = Ys
                .Select(kvp => kvp.Value.Apply())
                .SelectMany(s => s)
                .Aggregate(new { Min = double.MaxValue, Max = double.MinValue },
                (m, e) => new
                {
                    Min = e < m.Min ? e : m.Min,
                    Max = e > m.Max ? e : m.Max,
                });
            GlobalMin = minmax.Min * (2 - paddingScale);
            GlobalMax = minmax.Max * paddingScale;
        }

        private static void ExtremaByLine(IEnumerable<double> values, out double min, out double max)
        {
            var minmax = values
                .Aggregate(new { Min = double.MaxValue, Max = double.MinValue },
                (m, e) => new
                {
                    Min = e < m.Min ? e : m.Min,
                    Max = e > m.Max ? e : m.Max,
                });
            min = minmax.Min;
            max = minmax.Max;
        }

        private static bool ApproxEquals(double a, double b, double eps) =>
            Math.Abs(a - b) <= eps;

        private static double Scale(double value, double min, double max, double from, double to) =>
            from + (value - min) * (to - from) / (max - min);

        private List<string> Sample()
        {
            var skip = Xs.Count / GridCols;
            var sampled = new List<string>() { Xs.Transform(Xs.List[0]) };
            for (int i = 1; i <= Xs.Count; ++i)
            {
                if (i % skip == 0) sampled.Add(Xs.Transform(Xs.List[i - 1]));
            }
            return sampled;
        }
    }
}
