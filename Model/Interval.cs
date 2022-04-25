using System;

namespace HCI.Model
{
    public class Interval
    {
        public string ApiFunction { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }

        private Interval()
        {
            ApiFunction = String.Empty;
            Name = String.Empty;
            DisplayName = String.Empty;
        }

        public bool IsIntraday() => ApiFunction == "FX_INTRADAY";

        public override bool Equals(object? rhs) =>
            ReferenceEquals(this, rhs);

        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(Interval? lhs, Interval? rhs) =>
            ReferenceEquals(lhs, rhs);

        public static bool operator !=(Interval? lhs, Interval? rhs) =>
            !(lhs == rhs);

        public static Interval[] GetIntervals() => new[]
        {
            Intraday1Min,
            Intraday5Min,
            Intraday15Min,
            Intraday30Min,
            Intraday60Min,
            Daily,
            Weekly,
            Monthly
        };

        public static readonly Interval Intraday1Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "1min",
            DisplayName = "Na 1 minut",
        };

        public static readonly Interval Intraday5Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "5min",
            DisplayName = "Na 5 minuta",
        };

        public static readonly Interval Intraday15Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "15min",
            DisplayName = "Na 15 minuta",
        };

        public static readonly Interval Intraday30Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "30min",
            DisplayName = "Na 30 minuta",
        };

        public static readonly Interval Intraday60Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "60min",
            DisplayName = "Na 60 minuta",
        };

        public static readonly Interval Daily = new()
        {
            ApiFunction = "FX_DAILY",
            Name = "Daily",
            DisplayName = "Dnevno",
        };

        public static readonly Interval Weekly = new()
        {
            ApiFunction = "FX_WEEKLY",
            Name = "Weekly",
            DisplayName = "Nedeljno",
        };

        public static readonly Interval Monthly = new()
        {
            ApiFunction = "FX_MONTHLY",
            Name = "Monthly",
            DisplayName = "Mesečno",
        };
    }
}
