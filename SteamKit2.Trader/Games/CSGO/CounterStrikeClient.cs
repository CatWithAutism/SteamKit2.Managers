using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;
using SteamKit2.Trader.Enums;

namespace SteamKit2.Trader.Games.CSGO;

public class CounterStrikeClient
{
    private InventoryManager _inventoryManager;
    private const int InventoryMaxSize = 1000;
    private const uint AppId = 730;

    public CounterStrikeClient(SteamClient client)
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

        _inventoryManager = new InventoryManager(client);
    }
    
    
    public async Task<CEcon_GetInventoryItemsWithDescriptions_Response> GetInventory(int count = 1000, bool needDescription = false, Language language = Language.English)
    {
        return await _inventoryManager.GetInventory(AppId, count, needDescription, language);
    }
    
    
    
    
}