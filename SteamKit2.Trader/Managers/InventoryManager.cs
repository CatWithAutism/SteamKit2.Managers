using SteamKit2.Internal;
using SteamKit2.Trader.Enums;

namespace SteamKit2.Trader.Managers;

public class InventoryManager
{
    private static readonly List<uint> _econGames = new()
    {
        730, 570, 440
    };

    private readonly SteamUnifiedMessages.UnifiedService<IEcon> _econService;
    private readonly SteamUnifiedMessages.UnifiedService<IInventory> _inventoryService;
    private readonly SteamClient _steamClient;
    private readonly SteamUnifiedMessages _steamUnifiedMessages;

    public InventoryManager(SteamClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (!client.IsConnected)
        {
            throw new ArgumentException("Your steam client should be connected.");
        }

        _steamClient = client;
        _steamUnifiedMessages = _steamClient.GetHandler<SteamUnifiedMessages>();
        _econService = _steamUnifiedMessages.CreateService<IEcon>();
    }

    /// <summary>
    ///     Request inventory items via IEcon system(works only with Dota, CS, TF2)
    /// </summary>
    /// <param name="appId">Id of your game</param>
    /// <param name="count">Count of requested items</param>
    /// <param name="needDescription">Do you need description for each item?</param>
    /// <param name="language">Language</param>
    public async Task<CEcon_GetInventoryItemsWithDescriptions_Response> GetInventory(uint appId, int count, bool needDescription = false,
        Language language = Language.English)
    {
        if (count <= 0)
        {
            throw new ArgumentException($"{nameof(count)} should be more than zero.");
        }

        var request = new CEcon_GetInventoryItemsWithDescriptions_Request
        {
            steamid = _steamClient.SteamID.ConvertToUInt64(),
            appid = appId,
            get_descriptions = needDescription,
            contextid = 2,
            count = count,
            language = language.ToString().ToLowerInvariant()
        };

        var rawResponse = await _econService.SendMessage(inventory => inventory.GetInventoryItemsWithDescriptions(request));
        var response = rawResponse.GetDeserializedResponse<CEcon_GetInventoryItemsWithDescriptions_Response>();

        return response;
    }
}