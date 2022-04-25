using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using HCI.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HCI
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> AvailableCurrencies { get; set; }
        public ObservableCollection<string> AddedCurrencies { get; set; }

        private string _selectedAvailableCurrency;
        public string SelectedAvailableCurrency
        { 
            get => _selectedAvailableCurrency;
            set
            {
                if (value != _selectedAvailableCurrency)
                {
                    _selectedAvailableCurrency = value;
                    OnPropertyChanged(nameof(SelectedAvailableCurrency));
                }
            }
        }

        public ObservableCollection<Interval> Intervals { get; set; }

        private Interval _selectedInterval;
        public Interval SelectedInterval
        {
            get => _selectedInterval;
            set
            {
                if (value != _selectedInterval)
                {
                    _selectedInterval = value;
                    OnPropertyChanged(nameof(SelectedInterval));
                }
            }
        }

        public ObservableCollection<ExchangeRateAttribute> ExchangeRateAttributes { get; set; }
        private ExchangeRateAttribute _selectedExchangeRateAttribute;
        public ExchangeRateAttribute SelectedExchangeRateAttribute
        {
            get => _selectedExchangeRateAttribute;
            set
            {
                if (value != _selectedExchangeRateAttribute)
                {
                    _selectedExchangeRateAttribute = value;
                    OnPropertyChanged(nameof(SelectedExchangeRateAttribute));
                }
            }
        }

        public MainViewModel()
        {
            AvailableCurrencies = new(Enum.GetValues<Currency>().Select(c => c.ToString()));
            SelectedAvailableCurrency = AvailableCurrencies.First();

            AddedCurrencies = new();
            
            Intervals = new(Interval.GetIntervals());
            SelectedInterval = Interval.Daily;

            ExchangeRateAttributes = new(Enum.GetValues<ExchangeRateAttribute>());
            SelectedExchangeRateAttribute = ExchangeRateAttributes.First();
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
