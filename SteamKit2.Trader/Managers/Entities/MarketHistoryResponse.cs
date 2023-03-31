using Newtonsoft.Json;

namespace SteamKit2.Trader.Entities;

public class MarketHistoryResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("pagesize")]
    public int PageSize { get; set; }

    [JsonProperty("total_count")]
    public int TotalCount { get; set; }

    [JsonProperty("start")]
    public int Start { get; set; }
    
    public Dictionary<MarketHistoryItemRecord, MarketHistoryPurchaseRecord> historyRecords { get; set; }
}