using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
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
        }

        private MainViewModel GetViewModel() =>
            (DataContext as MainViewModel)!;

        private static string TimeFormat(Interval interval)
        {
            if (interval.IsIntraday()) return "T";
            else if (interval == Interval.Daily) return "d";
            else if (interval == Interval.Weekly) return "M";
            else return "Y";
        }

        private void AddCurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            var ctx = (DataContext as MainViewModel)!;
            var c = ctx.SelectedAvailableCurrency;
            if (ctx.AddedCurrencies.Count >= 5)
            {
                MessageBox.Show(
                    $"Maksimalan broj dodatih valuta je 5. Ukoliko želite da dodate valutu {c}, prvo uklonite jednu od prethodno dodatih valuta.",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
            var tasks = new List<Task<FetchResult>>();
            foreach (var c in vm.AddedCurrencies.Select(c => Enum.Parse<Currency>(c)))
                tasks.Add(Model.AddCurrency(c));
            await Task.WhenAll(tasks);
            var results = tasks.Select(t => t.Result).ToList();

            if (results.Contains(FetchResult.BadConnection))
            {
                MessageBox.Show(
                    "Nije moguće povezati se sa serverom. Molimo Vas proverite svoju internet konekciju.", 
                    "Loša konekcija", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (results.Contains(FetchResult.InternalError))
            {
                MessageBox.Show(
                    "Interna greška. Molimo Vas prijavite da je došlo do interne greške na adresu szmlad@gmail.com da bismo je što pre rešili!",
                    "Interna greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (results.Contains(FetchResult.APILimitExceeded))
            {
                MessageBox.Show(
                    "Server je preopterećen. Molimo Vas sačekajte bar minut pre ponovnog osvežavanja.",
                    "Preopterećen server", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
            vm.Series = Model.Series;
            vm.SelectedTab = 0;
            chart.Draw();
        }

        private void ShowTableButton_Click(object sender, RoutedEventArgs e)
        {
            var tableWindow = new TableWindow(GetViewModel())
            {
                Owner = this,
            };
            tableWindow.Show();
        }
    }
}
