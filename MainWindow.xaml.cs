using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using HCI.Model;
using HCI.Chart;

namespace HCI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DrawGraph();
        }

        private async void DrawGraph()
        {
            var model = new DataModel(Environment.GetEnvironmentVariable("API_KEY")!, Interval.Daily);
            await model.AddCurrency(Currency.USD);

            var points = model.Series[Currency.USD].Points
                .Select(p => Convert.ToDouble(p.Close))
                .ToList();
            var timestamps = model.Series[Currency.USD].Points
                .Select(p => p.Timestamp.ToString())
                .ToList();

            var chart = new LineChart(ChartCanvas)
            {
                GridRows = 4,
                GridCols = 4,
                Stroke = Brushes.Red,
                Values = points,
                Labels = timestamps,
            };
            chart.DrawGrid();
        }
    }
}
