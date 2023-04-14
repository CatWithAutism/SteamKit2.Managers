using SteamKit2.Internal;
using SteamKit2.Managers.Enums;
using SteamKit2.Managers.Managers;
using SteamKit2.Managers.Managers.Entities.Inventory;
using SteamKit2.Managers.Managers.Entities.Market;

namespace SteamKit2.Managers.Games.CSGO;

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


    public async Task<InventoryResponse> GetInventory(InventoryRequest inventoryRequest)
    {
        return await _inventoryManager.GetInventory(inventoryRequest);
    }

    public async Task<MarketHistoryResponse> GetMarketHistory(uint start, uint count, bool noRender)
    {
        return await _marketManager.GetMarketHistory(start, count, noRender, AppId);
    }
}