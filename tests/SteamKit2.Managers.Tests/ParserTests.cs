using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SteamKit2.Managers.Games.CSGO;
using SteamKit2.Managers.Managers.Entities.Market;

namespace SteamKit2.Trader.Tests;

public class ParserTests
{
    [Test]
    public void ParseTradeHistoryResponse()
    {
        var json = File.ReadAllText("TestData/JsonTradeHistory.json");
        var obj = JsonConvert.DeserializeObject<MarketHistoryResponse>(json);
    }
}