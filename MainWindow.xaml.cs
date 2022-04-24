using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
using HCI.Model;
using HCI.Chart;

namespace HCI
{
    public partial class MainWindow : Window
    {
        private DataModel Model { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Model = new DataModel(Environment.GetEnvironmentVariable("API_KEY")!, Interval.Daily);
            DrawGraph();
        }

        private async void DrawGraph()
        {
            await Task.WhenAll(
                Model.AddCurrency(Currency.USD),
                Model.AddCurrency(Currency.EUR),
                Model.AddCurrency(Currency.BAM));

            var timestamps = Model.Series[Currency.USD].Points
                .Select(p => p.Timestamp)
                .ToList();

            var chart = new LineChart<Currency, ExchangeRateDataPoint, DateTimeOffset>(ChartCanvas)
            {
                GridRows = 4,
                GridCols = 4,
                Xs = new(timestamps, t => t.ToString(TimeFormat(Model.Interval))),
                Ys = new()
                {
                    [Currency.USD] = new(Model.Series[Currency.USD].Points, p => Convert.ToDouble(p.Close)),
                    [Currency.EUR] = new(Model.Series[Currency.EUR].Points, p => Convert.ToDouble(p.Close)),
                    [Currency.BAM] = new(Model.Series[Currency.BAM].Points, p => Convert.ToDouble(p.Close)),
                }
            };
            chart.Draw();
        }

        private static string TimeFormat(Interval interval)
        {
            if (interval.IsIntraday()) return "T";
            else if (interval == Interval.Daily) return "d";
            else if (interval == Interval.Weekly) return "";
            else return "Y";
        }
    }
}
