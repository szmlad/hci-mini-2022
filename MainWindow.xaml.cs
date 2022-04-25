using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Threading.Tasks;
using HCI.Model;
using HCI.Chart;
using System.Windows.Controls;

namespace HCI
{
    public partial class MainWindow : Window
    {
        private DataModel Model { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Model = new DataModel(Environment.GetEnvironmentVariable("API_KEY")!, Interval.Daily);
            // DrawGraph();
        }

        private MainViewModel GetViewModel() =>
            (DataContext as MainViewModel)!;

        private async void DrawGraph()
        {
            await Task.WhenAll(
                // Model.AddCurrency(Currency.USD),
                // Model.AddCurrency(Currency.EUR),
                Model.AddCurrency(Currency.BAM));

            var timestamps = Model.GetTimestamps();

            var chart = new LineChart<Currency, ExchangeRateDataPoint, DateTimeOffset>(ChartCanvas)
            {
                GridRows = 4,
                GridCols = 4,
                Xs = new(timestamps, t => t.ToString(TimeFormat(Model.Interval))),
                Ys = new()
                {
                    // [Currency.USD] = new(Model.Series[Currency.USD].Points, p => Convert.ToDouble(p.Close)),
                    // [Currency.EUR] = new(Model.Series[Currency.EUR].Points, p => Convert.ToDouble(p.Close)),
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

        private void AddCurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            var ctx = (DataContext as MainViewModel)!;
            var c = ctx.SelectedAvailableCurrency;
            ctx.AvailableCurrencies.Remove(c);
            ctx.AddedCurrencies.Add(c);
            ctx.SelectedAvailableCurrency = ctx.AvailableCurrencies.FirstOrDefault(String.Empty);
            if (ctx.AvailableCurrencies.Count == 0)
            {
                AddCurrencyComboBox.IsEnabled = false;
                AddCurrencyButton.IsEnabled = false;
            }
        }

        private void RemoveCurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            var c = ((sender as Button)!.Tag as string)!;
            var ctx = (DataContext as MainViewModel)!;
            ctx.AddedCurrencies.Remove(c);
            ctx.AvailableCurrencies.Add(c);
            if (ctx.AvailableCurrencies.Count != 0)
            {
                ctx.SelectedAvailableCurrency = ctx.AvailableCurrencies.FirstOrDefault(String.Empty);
                AddCurrencyComboBox.IsEnabled = true;
                AddCurrencyButton.IsEnabled = true;
            }
        }

        private static Func<ExchangeRateDataPoint, double> GetAttributeAccessor(ExchangeRateAttribute attribute) =>
            attribute switch
            {
                ExchangeRateAttribute.Open => (p => Convert.ToDouble(p.Open)),
                ExchangeRateAttribute.High => (p => Convert.ToDouble(p.High)),
                ExchangeRateAttribute.Low => (p => Convert.ToDouble(p.Low)),
                ExchangeRateAttribute.Close=> (p => Convert.ToDouble(p.Close)),
                _ => (p => Convert.ToDouble(p.Close)),
            };

        private async void RefreshChartButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = GetViewModel();
            if (vm.AddedCurrencies.Count == 0)
            {
                MessageBox.Show(
                    "Morate imati bar jednu dodatu valutu pre nego što biste osvežili podatke.", 
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Model = new(Environment.GetEnvironmentVariable("API_KEY")!, GetViewModel().SelectedInterval);
            var tasks = new List<Task>();
            foreach (var c in vm.AddedCurrencies.Select(c => Enum.Parse<Currency>(c)))
                tasks.Add(Model.AddCurrency(c));
            await Task.WhenAll(tasks);

            var timestamps = Model.GetTimestamps();
            var chart = new LineChart<Currency, ExchangeRateDataPoint, DateTimeOffset>(ChartCanvas)
            {
                GridRows = 4,
                GridCols = 4,
                Xs = new(timestamps, t => t.ToString(TimeFormat(Model.Interval))),
            };
            chart.Ys = new();
            foreach (var kvp in Model.Series)
            {
                chart.Ys[kvp.Key] = new(kvp.Value.Points, GetAttributeAccessor(vm.SelectedExchangeRateAttribute));
            }
            chart.Draw();
            vm.Series = Model.Series;
        }

        private void ShowTableButton_Click(object sender, RoutedEventArgs e)
        {
            var tableWindow = new TableWindow(GetViewModel());
            tableWindow.Show();
        }
    }
}
