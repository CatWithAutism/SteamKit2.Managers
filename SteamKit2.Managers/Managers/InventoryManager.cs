using SteamKit2.Internal;
using SteamKit2.Managers.Enums;
using SteamKit2.Managers.Managers.Entities.Inventory;

namespace SteamKit2.Managers.Managers;

public class InventoryManager
{
    private static readonly List<uint> _econGames = new()
    {
        730, 570, 440
    };
    
    protected readonly SteamClient _steamClient;
    protected readonly SteamUnifiedMessages _steamUnifiedMessages;
    protected readonly SteamUnifiedMessages.UnifiedService<IEcon> _econService;
    
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
    public async Task<InventoryResponse> GetInventory(InventoryRequest inventoryRequest)
    {
        ArgumentNullException.ThrowIfNull(inventoryRequest, nameof(inventoryRequest));
        
        var steamKitRequest = new CEcon_GetInventoryItemsWithDescriptions_Request
        {
            steamid = _steamClient.SteamID.ConvertToUInt64(),
            appid = inventoryRequest.AppId,
            get_descriptions = inventoryRequest.NeedDescription,
            contextid = inventoryRequest.ContextId,
            count = inventoryRequest.Count,
            language = inventoryRequest.Language.ToString(),
            start_assetid = inventoryRequest.StartFromItem,
            for_trade_offer_verification = inventoryRequest.TradeOfferVerificationRequired,
            filters = new CEcon_GetInventoryItemsWithDescriptions_Request.FilterOptions()
            {
                marketable_only = inventoryRequest.OnlyMarketable,
                tradable_only = inventoryRequest.OnlyTradable,
            }
        };
        
        var rawResponse = await _econService.SendMessage(inventory => inventory.GetInventoryItemsWithDescriptions(steamKitRequest));
        var response = rawResponse.GetDeserializedResponse<CEcon_GetInventoryItemsWithDescriptions_Response>();
        var inventoryResponse = new InventoryResponse(response);
        
        return inventoryResponse;
    }
}