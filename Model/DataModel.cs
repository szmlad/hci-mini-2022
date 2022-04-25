using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json;
using System.Linq;

namespace HCI.Model
{
    public enum FetchResult
    {
        Success,
        BadConnection,
        APILimitExceeded,
        InternalError,
    }

    public class DataModel
    {
        private const string BASE_CURRENCY = "RSD";

        private readonly string ApiKey;
        public Interval Interval { get; private set; }
        public Dictionary<Currency, ExchangeRateTimeSeries> Series { get; private set; }

        public DataModel(string apiKey, Interval interval)
        {
            ApiKey = apiKey;
            Interval = interval;
            Series = new();
        }

        public List<DateTimeOffset> GetTimestamps() =>
            Series[Series.Keys.First()]
                .Points
                .Select(p => p.Timestamp)
                .ToList();

        public async Task<FetchResult> AddCurrency(Currency currency)
        {
            var (resp, status) = await FetchTimeSeriesData(currency);
            if (status != FetchResult.Success) return status;
            var json = JsonSerializer
                .Deserialize<Dictionary<string, JsonElement>>(resp)!;

            if (json.ContainsKey("Note")) return FetchResult.APILimitExceeded;

            var series = new ExchangeRateTimeSeries();

            try
            {
                var dataJson = json[$"Time Series FX ({Interval.Name})"];
                var data = dataJson
                    .Deserialize<Dictionary<string, JsonElement>>()!;

                foreach (var datum in data)
                {
                    var values = datum.Value
                        .Deserialize<Dictionary<string, string>>()!;
                    series.Points.Add(new()
                    {
                        Open = decimal.Parse(values["1. open"]),
                        High = decimal.Parse(values["2. high"]),
                        Low = decimal.Parse(values["3. low"]),
                        Close = decimal.Parse(values["4. close"]),
                        Timestamp = DateTimeOffset.Parse(datum.Key)
                    });
                }

                series.Points.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));
                Series[currency] = series;

                return FetchResult.Success;
            }
            catch (Exception)
            {
                return FetchResult.InternalError;
            }
        }

        private string BuildQuery(Currency currency)
        {
            var builder = new UriBuilder("https://www.alphavantage.co/query?")
            {
                Port = -1,
            };
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add(new()
            {
                ["function"] = Interval.ApiFunction,
                ["from_symbol"] = currency.ToString(),
                ["to_symbol"] = BASE_CURRENCY,
                ["apikey"] = ApiKey,
            });
            if (Interval.IsIntraday())
                query.Add("interval", Interval.Name);
            builder.Query = query.ToString();
            return builder.ToString();
        }

        private async Task<(string, FetchResult)> FetchTimeSeriesData(Currency currency)
        {
            try
            {
                using var client = new HttpClient();
                return (await client.GetStringAsync(BuildQuery(currency)), FetchResult.Success);
            }
            catch (HttpRequestException)
            {
                return (String.Empty, FetchResult.BadConnection);
            }
        }
    }
}
