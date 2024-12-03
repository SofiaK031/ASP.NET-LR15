using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsService4
{
    public class ExchangeInfo
    {
        public string Timezone { get; set; }
        public long ServerTime { get; set; }
        public List<RateLimit> RateLimits { get; set; }
        public List<object> ExchangeFilters { get; set; }
        public List<Symbol> Symbols { get; set; }

        public string GetExchangeInfoSummary()
        {
            string symbolsInfo = Symbols != null
                ? string.Join(", ", Symbols.Select(s => $"{s.BaseAsset}{s.QuoteAsset}"))
                : "No symbols";

            return $"Timezone: {Timezone}, " +
                   $"ServerTime: {DateTimeOffset.FromUnixTimeMilliseconds(ServerTime).UtcDateTime}, " +
                   $"RateLimits: {RateLimits?.Count ?? 0}, " +
                   $"Symbols: {Symbols?.Count ?? 0} ({symbolsInfo})";
        }
    }

    public class RateLimit
    {
        public string RateLimitType { get; set; }
        public string Interval { get; set; }
        public int IntervalNum { get; set; }
        public int Limit { get; set; }
    }

    public class Symbol
    {
        public string SymbolName { get; set; }
        public string Status { get; set; }
        public string BaseAsset { get; set; }
        public int BaseAssetPrecision { get; set; }
        public string QuoteAsset { get; set; }
        public int QuotePrecision { get; set; }
        public int QuoteAssetPrecision { get; set; }
        public int BaseCommissionPrecision { get; set; }
        public int QuoteCommissionPrecision { get; set; }
        public List<string> OrderTypes { get; set; }
        public bool IcebergAllowed { get; set; }
        public bool OcoAllowed { get; set; }
        public bool OtoAllowed { get; set; }
        public bool QuoteOrderQtyMarketAllowed { get; set; }
        public bool AllowTrailingStop { get; set; }
        public bool CancelReplaceAllowed { get; set; }
        public bool IsSpotTradingAllowed { get; set; }
        public bool IsMarginTradingAllowed { get; set; }
        public List<Filter> Filters { get; set; }
        public List<string> Permissions { get; set; }
        public List<List<string>> PermissionSets { get; set; }
        public string DefaultSelfTradePreventionMode { get; set; }
        public List<string> AllowedSelfTradePreventionModes { get; set; }
    }

    public class Filter
    {
        public string FilterType { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public string TickSize { get; set; }
        public string MinQty { get; set; }
        public string MaxQty { get; set; }
        public string StepSize { get; set; }
        public int? Limit { get; set; }
        public string MinNotional { get; set; }
        public bool? ApplyMinToMarket { get; set; }
        public string MaxNotional { get; set; }
        public bool? ApplyMaxToMarket { get; set; }
        public int? AvgPriceMins { get; set; }
        public string BidMultiplierUp { get; set; }
        public string BidMultiplierDown { get; set; }
        public string AskMultiplierUp { get; set; }
        public string AskMultiplierDown { get; set; }
        public int? MinTrailingAboveDelta { get; set; }
        public int? MaxTrailingAboveDelta { get; set; }
        public int? MinTrailingBelowDelta { get; set; }
        public int? MaxTrailingBelowDelta { get; set; }
        public int? MaxNumOrders { get; set; }
        public int? MaxNumAlgoOrders { get; set; }
    }
}