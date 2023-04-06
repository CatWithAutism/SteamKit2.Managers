using RestSharp;

namespace SteamKit2.Trader.Managers;

public static class SteamWebAuthManager
{
    private static readonly HttpClient _httpClient = new();
    private static readonly RestClient _restClient = new();

    public static void GetAuthorizedCookies(string login, string password, string authcode)
    {
        //new SteamClient().Configuration.
    }
}