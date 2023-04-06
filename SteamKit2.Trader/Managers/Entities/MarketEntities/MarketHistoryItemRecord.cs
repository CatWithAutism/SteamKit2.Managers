using Newtonsoft.Json;
using SteamKit2.Trader.Managßers.Entities.MarketEntities;

namespace SteamKit2.Trader.Managers.Entities.MarketEntities;

public class MarketHistoryItemRecord
{
    [JsonProperty("currency")] public int Currency { get; set; }

    [JsonProperty("appid")] public int Appid { get; set; }

    [JsonProperty("contextid")] public string Contextid { get; set; }

    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("classid")] public string Classid { get; set; }

    [JsonProperty("instanceid")] public string Instanceid { get; set; }

    [JsonProperty("amount")] public string Amount { get; set; }

    [JsonProperty("status")] public int Status { get; set; }

    [JsonProperty("original_amount")] public string OriginalAmount { get; set; }

    [JsonProperty("unowned_id")] public string UnownedId { get; set; }

    [JsonProperty("unowned_contextid")] public string UnownedContextid { get; set; }

    [JsonProperty("background_color")] public string BackgroundColor { get; set; }

    [JsonProperty("icon_url")] public string IconUrl { get; set; }

    [JsonProperty("descriptions")] public List<MarketHistoryItemDescription> Descriptions { get; set; }

    [JsonProperty("tradable")] public int Tradable { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("name_color")] public string NameColor { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("market_name")] public string MarketName { get; set; }

    [JsonProperty("market_hash_name")] public string MarketHashName { get; set; }

    [JsonProperty("commodity")] public int Commodity { get; set; }

    [JsonProperty("market_tradable_restriction")]
    public int MarketTradableRestriction { get; set; }

    [JsonProperty("marketable")] public int Marketable { get; set; }

    [JsonProperty("market_buy_country_restriction")]
    public string MarketBuyCountryRestriction { get; set; }

    [JsonProperty("app_icon")] public string AppIcon { get; set; }

    [JsonProperty("owner")] public int Owner { get; set; }
}