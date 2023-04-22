using SteamKit2.Internal;
using SteamKit2.Managers.Enums;
using SteamKit2.Managers.Managers;
using SteamKit2.Managers.Managers.Entities.Inventory;
using SteamKit2.Managers.Managers.Entities.Market;
using SteamKit2.Managers.Managers.Games;

namespace SteamKit2.Managers.Games.CSGO;

public class CounterStrikeClient
{
    private const int InventoryMaxSize = 1000;
    private const uint AppId = 730;

    private CounterStrikeInventoryManager InventoryManager { get; }

    public CounterStrikeClient(SteamClient client, string userNonce)
    {
        ArgumentException.ThrowIfNullOrEmpty(userNonce);
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

        InventoryManager = new CounterStrikeInventoryManager(client);
    }


    public async Task<InventoryResponse> GetInventory(InventoryRequest inventoryRequest)
    {
        return await InventoryManager.GetInventory(inventoryRequest);
    }

    public async Task<InventoryResponse> GetStorageUnitItems(uint storageUnitId)
    {
        return await InventoryManager.GetStorageUnitItems();
    }
}