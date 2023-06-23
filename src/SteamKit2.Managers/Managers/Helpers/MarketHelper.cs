using Newtonsoft.Json.Linq;
using SteamKit2.Managers.Managers.Entities.Market;

namespace SteamKit2.Managers.Managers.Helpers;

internal static class MarketHelper
{
    public static MarketHistoryResponse SortByParams(MarketHistoryResponse marketResponse, uint? appId = null, uint? contextId = null)
    {
        if (marketResponse == null)
        {
            throw new ArgumentNullException(nameof(marketResponse));
        }

        throw new NotImplementedException();
    }
    
    public static List<MarketHistoryPurchaseRecord> GetPurchaseRecords(string json, uint? appId = null)
    {
        var jObject = JObject.Parse(json);
        var jToken = jObject["purchases"];
        
        if (jToken == null)
        {
            return new List<MarketHistoryPurchaseRecord>();
        }

        List<MarketHistoryPurchaseRecord> purchaseRecords = jToken.Children<JProperty>().Select(t => t.Value.ToObject<MarketHistoryPurchaseRecord>()).ToList();

        if (appId == null)
        {
            return purchaseRecords;
        }

        List<MarketHistoryPurchaseRecord> filteredByAppId =
            purchaseRecords.Where(record => record.MarketHistoryPurchaseAssetRecord.Appid == appId).ToList();

        return filteredByAppId;
    }

    public static List<MarketHistoryItemRecord> GetItemRecords(string json, uint? appId = null, uint? contextId = null)
    {
        var jObject = JObject.Parse(json);

        if (jObject == null)
        {
            return new List<MarketHistoryItemRecord>();
        }

        var assets = jObject["assets"];


        if (appId != null)
        {
            var specifiedGame = assets[appId.ToString()];
            if (specifiedGame == null)
            {
                return new List<MarketHistoryItemRecord>();
            }

            if (contextId == null)
            {
                List<MarketHistoryItemRecord> items = specifiedGame.SelectMany(t => t.Values<MarketHistoryItemRecord>()).ToList();
                return items;
            }
            else
            {
                List<MarketHistoryItemRecord>? items = specifiedGame[contextId.ToString()]?.Children<JProperty>()
                    .Select(t => t.Value.ToObject<MarketHistoryItemRecord>()).ToList();
                return items ?? new List<MarketHistoryItemRecord>();
            }
        }

        if (contextId != null)
        {
            IEnumerable<JToken> specifiedContext = assets.Select(t => t[contextId.ToString()]);
            List<MarketHistoryItemRecord> items = specifiedContext.Values<MarketHistoryItemRecord>().ToList();
            return items;
        }

        return assets.SelectMany(t => t.Values<MarketHistoryItemRecord>()).ToList();
    }
}