using Newtonsoft.Json;

namespace SteamKit2.Managers.Managers.Entities.Market;

public class MarketHistoryResponse
{
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("pagesize")] public int PageSize { get; set; }

    [JsonProperty("total_count")] public int TotalCount { get; set; }
    
    [JsonProperty("start")] public int Start { get; set; }
    
    [JsonProperty("assets")] public Dictionary<string, Dictionary<string, Dictionary<string, MarketHistoryItemRecord>>> Assets { get; set; }
    
    [JsonProperty("purchases")] public Dictionary<string, MarketHistoryPurchaseRecord> PurchaseRecords { get; set; }
    
    [JsonProperty("events")] public List<MarketHistoryEvent> MarketHistoryEvents { get; set; }

    public Dictionary<MarketHistoryItemRecord, MarketHistoryPurchaseRecord> HistoryRecords { get; set; }
}