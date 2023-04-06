using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SteamKit2.Trader.Games.CSGO;
using SteamKit2.Trader.Managers.Entities.MarketEntities;

namespace SteamKit2.CSGO.Tests;

public class ParserTests
{
    [Test]
    public void ParseTradeHistoryResponse()
    {
        var json = File.ReadAllText("TestData/JsonTradeHistory.json");
        var obj = JsonConvert.DeserializeObject<MarketHistoryResponse>(json);
    }
}