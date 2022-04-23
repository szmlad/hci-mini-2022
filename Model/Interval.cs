using System;

namespace HCI.Model
{
    internal class Interval
    {
        public string ApiFunction { get; private set; }
        public string Name { get; private set; }

        private Interval()
        {
            ApiFunction = String.Empty;
            Name = String.Empty;
        }

        public bool IsIntraday() => ApiFunction == "FX_INTRADAY";

        public override bool Equals(object? rhs) =>
            ReferenceEquals(this, rhs);

        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(Interval? lhs, Interval? rhs) =>
            ReferenceEquals(lhs, rhs);

        public static bool operator !=(Interval? lhs, Interval? rhs) =>
            !(lhs == rhs);

        public static readonly Interval Intraday1Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "1min",
        };

        public static readonly Interval Intraday5Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "5min",
        };

        public static readonly Interval Intraday15Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "15min",
        };

        public static readonly Interval Intraday30Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "30min",
        };

        public static readonly Interval Intraday60Min = new()
        {
            ApiFunction = "FX_INTRADAY",
            Name = "60min",
        };

        public static readonly Interval Daily = new()
        {
            ApiFunction = "FX_DAILY",
            Name = "Daily",
        };

        public static readonly Interval Weekly = new()
        {
            ApiFunction = "FX_WEEKLY",
            Name = "Weekly",
        };

        public static readonly Interval Monthly = new()
        {
            ApiFunction = "FX_MONTHLY",
            Name = "Monthly"
        };
    }
}
