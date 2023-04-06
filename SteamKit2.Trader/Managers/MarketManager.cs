using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamKit2.Trader.Managers.Entities.MarketEntities;
using SteamKit2.Trader.Utils;

namespace SteamKit2.Trader.Managers;

public class MarketManager
{
    private const int MaxItemsPerRequest = 500;

    private const string GetMarketHistoryPattern =
        "https://steamcommunity.com/market/myhistory/?start={0}&count={1}&norender={2}";

    private readonly SteamWeb _steamWeb;

    public MarketManager(ulong steamId, EUniverse universe, string webAPIUserNonce)
    {
        _steamWeb = new SteamWeb(steamId, universe, webAPIUserNonce);
    }

    public async Task<MarketHistoryResponse> GetMarketHistory(uint start, uint count, bool noRender = true, uint? appId = null)
    {
        if (count > 500)
        {
            throw new ArgumentException(Exceptions.MaxPerRequestForMarketHistory);
        }

        var requestUrl = string.Format(GetMarketHistoryPattern, start, count, noRender);
        var json = await _steamWeb.Fetch(requestUrl, HttpMethod.Get);

        var marketHistory = JsonConvert.DeserializeObject<MarketHistoryResponse>(json);
        
        List<MarketHistoryPurchaseRecord> purchaseRecords =  GetPurchaseRecords(json, appId);
        List<MarketHistoryItemRecord> marketHistoryItemRecords = GetItemRecords(json, appId, 2);

        Dictionary<MarketHistoryItemRecord, MarketHistoryPurchaseRecord> result = new();
        foreach (var purchaseRecord in purchaseRecords)
        {
            foreach (var itemRecord in marketHistoryItemRecords)
            {
                if ((purchaseRecord.MarketHistoryPurchaseAssetRecord.Classid == itemRecord.Classid &&
                     purchaseRecord.MarketHistoryPurchaseAssetRecord.Id == itemRecord.Id) || 
                    purchaseRecord.MarketHistoryPurchaseAssetRecord.NewId == itemRecord.Id)
                {
                    result.Add(itemRecord, purchaseRecord);
                    break;
                }
            }
        }

        marketHistory.HistoryRecords = result;

        return marketHistory;
    }

    private List<MarketHistoryPurchaseRecord> GetPurchaseRecords(string json, uint? appId = null)
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

    private List<MarketHistoryItemRecord> GetItemRecords(string json, uint? appId = null, uint? contextId = null)
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