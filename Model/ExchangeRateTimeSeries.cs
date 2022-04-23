using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCI.Model
{
    internal class ExchangeRateDataPoint
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    internal class ExchangeRateTimeSeries
    {
        public List<ExchangeRateDataPoint> Points { get; set; }

        public ExchangeRateTimeSeries()
        {
            Points = new();
        }
    }
}
