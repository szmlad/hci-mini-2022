using System;
using System.Collections.Generic;

namespace HCI.Model
{
    public enum ExchangeRateAttribute
    {
        Open,
        High,
        Low,
        Close
    }

    public class ExchangeRateDataPoint
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    public class ExchangeRateTimeSeries
    {
        public List<ExchangeRateDataPoint> Points { get; set; }

        public ExchangeRateTimeSeries()
        {
            Points = new();
        }
    }
}
