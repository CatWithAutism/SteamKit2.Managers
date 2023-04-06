using SteamKit2.Internal;
using SteamKit2.Trader.Enums;
using SteamKit2.Trader.Managers;
using SteamKit2.Trader.Managers.Entities.MarketEntities;

namespace SteamKit2.Trader.Games.CSGO;

public class CounterStrikeClient
{
    private const int InventoryMaxSize = 1000;
    private const uint AppId = 730;
    
    private readonly InventoryManager _inventoryManager;
    private readonly MarketManager _marketManager;

    public CounterStrikeClient(SteamClient client, string userNonce)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (!client.IsConnected)
        {
            throw new ArgumentException(Exceptions.IsNotConnected);
        }

        if (client.SteamID == null)
        {
            throw new ArgumentNullException(Exceptions.IsNotAuthorized);
        }

        if (string.IsNullOrWhiteSpace(userNonce))
        {
            throw new ArgumentNullException(nameof(userNonce));
        }

        _inventoryManager = new InventoryManager(client);

        var userId = client.SteamID.ConvertToUInt64();
        _marketManager = new MarketManager(userId, EUniverse.Public, userNonce);
    }


    public async Task<CEcon_GetInventoryItemsWithDescriptions_Response> GetInventory(int count = 1000, bool needDescription = false,
        Language language = Language.English)
    {
        return await _inventoryManager.GetInventory(AppId, count, needDescription, language);
    }

    public async Task<MarketHistoryResponse> GetMarketHistory(uint start, uint count, bool noRender)
    {
        return await _marketManager.GetMarketHistory(start, count, noRender, AppId);
    }
}