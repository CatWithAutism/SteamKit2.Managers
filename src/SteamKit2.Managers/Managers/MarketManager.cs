using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamKit2.Managers.Managers.Entities.Market;
using SteamKit2.Managers.Utils;

namespace SteamKit2.Managers.Managers;

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

    public async Task<MarketHistoryResponse> GetMarketHistory(uint start, uint count, bool noRender = true, uint? appId = null, uint? contextId = null)
    {
        if (count > 500)
        {
            throw new ArgumentException(Exceptions.MaxPerRequestForMarketHistory);
        }

        var requestUrl = string.Format(GetMarketHistoryPattern, start, count, noRender);
        var json = await _steamWeb.Fetch(requestUrl, HttpMethod.Get);

        var marketHistory = JsonConvert.DeserializeObject<MarketHistoryResponse>(json);
        
        var response = JsonConvert.DeserializeObject<MarketHistoryResponse>(json);

        return marketHistory;
    }
}