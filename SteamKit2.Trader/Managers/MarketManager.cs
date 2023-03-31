using RestSharp;
using SteamKit2.Trader.Entities;

namespace SteamKit2.Trader;

public class MarketManager : IDisposable
{
    private const int MaxItemsPerRequest = 500;

    private readonly HttpClient _httpClient;
    private readonly RestClient _restClient;

    public MarketManager()
    {
        _httpClient = new HttpClient();
        _restClient = new RestClient(_httpClient);
    }

    public MarketHistoryResponse GetMarketHistory(uint appId, uint start, uint count)
    {
        if (count > 500)
        {
            throw new ArgumentException(Exceptions.MaxPerRequestForMarketHistory);
        }

        throw new NotImplementedException();
    }


    public void Dispose()
    {
        _httpClient.Dispose();
        _restClient.Dispose();
    }
}