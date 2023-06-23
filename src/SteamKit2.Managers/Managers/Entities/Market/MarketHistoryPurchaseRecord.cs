using Newtonsoft.Json;

namespace SteamKit2.Managers.Managers.Entities.Market;

public class MarketHistoryPurchaseRecord
{
    [JsonProperty("listingid")] public string Listingid { get; set; }

    [JsonProperty("purchaseid")] public string Purchaseid { get; set; }

    [JsonProperty("time_sold")] public int TimeSold { get; set; }

    [JsonProperty("steamid_purchaser")] public string SteamidPurchaser { get; set; }

    [JsonProperty("needs_rollback")] public int NeedsRollback { get; set; }

    [JsonProperty("failed")] public int Failed { get; set; }

    [JsonProperty("asset")] public MarketHistoryPurchaseAssetRecord MarketHistoryPurchaseAssetRecord { get; set; }

    [JsonProperty("paid_amount")] public int PaidAmount { get; set; }

    [JsonProperty("paid_fee")] public int PaidFee { get; set; }

    [JsonProperty("currencyid")] public string Currencyid { get; set; }

    [JsonProperty("steam_fee")] public int SteamFee { get; set; }

    [JsonProperty("publisher_fee")] public int PublisherFee { get; set; }

    [JsonProperty("publisher_fee_percent")]
    public string PublisherFeePercent { get; set; }

    [JsonProperty("publisher_fee_app")] public int PublisherFeeApp { get; set; }

    [JsonProperty("received_amount")] public int ReceivedAmount { get; set; }

    [JsonProperty("received_currencyid")] public string ReceivedCurrencyid { get; set; }

    [JsonProperty("funds_held")] public int FundsHeld { get; set; }

    [JsonProperty("time_funds_held_until")]
    public int TimeFundsHeldUntil { get; set; }

    [JsonProperty("funds_revoked")] public int FundsRevoked { get; set; }

    [JsonProperty("funds_returned")] public int FundsReturned { get; set; }

    [JsonProperty("avatar_actor")] public string AvatarActor { get; set; }

    [JsonProperty("persona_actor")] public string PersonaActor { get; set; }
}