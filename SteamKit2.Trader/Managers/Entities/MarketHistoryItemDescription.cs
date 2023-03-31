using Newtonsoft.Json;

namespace SteamKit2.Trader.Entities;

public class MarketHistoryItemDescription
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("color")]
    public string Color { get; set; }
}
