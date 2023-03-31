using Newtonsoft.Json;

namespace SteamKit2.Trader.Entities;

public class MarketHistoryPurchaseAssetRecord
{
    [JsonProperty("currency")]
    public int Currency { get; set; }

    [JsonProperty("appid")]
    public int Appid { get; set; }

    [JsonProperty("contextid")]
    public string Contextid { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("classid")]
    public string Classid { get; set; }

    [JsonProperty("instanceid")]
    public string Instanceid { get; set; }

    [JsonProperty("amount")]
    public string Amount { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("new_id")]
    public string NewId { get; set; }

    [JsonProperty("new_contextid")]
    public string NewContextid { get; set; }
}