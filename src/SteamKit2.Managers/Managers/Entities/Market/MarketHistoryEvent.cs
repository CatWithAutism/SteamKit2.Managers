using Newtonsoft.Json;
using SteamKit2.Managers.Managers.Entities.Market.Enums;

namespace SteamKit2.Managers.Managers.Entities.Market;

public class MarketHistoryEvent
{
    [JsonProperty("listingid")]
    public string Listingid { get; set; }

    [JsonProperty("purchaseid", NullValueHandling = NullValueHandling.Ignore)]
    public string Purchaseid { get; set; }

    [JsonProperty("event_type")]
    public ActionType EventType { get; set; }

    [JsonProperty("time_event")]
    public int TimeEvent { get; set; }

    [JsonProperty("time_event_fraction")]
    public int TimeEventFraction { get; set; }

    [JsonProperty("steamid_actor")]
    public string SteamidActor { get; set; }

    [JsonProperty("date_event")]
    public string DateEvent { get; set; }
}