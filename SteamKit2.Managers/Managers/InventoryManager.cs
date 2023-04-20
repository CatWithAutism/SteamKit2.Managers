using SteamKit2.Internal;
using SteamKit2.Managers.Enums;
using SteamKit2.Managers.Managers.Entities.Inventory;

namespace SteamKit2.Managers.Managers;

public class InventoryManager
{
    protected SteamClient SteamSteamClient { get; }
    protected SteamUnifiedMessages SteamMessages { get; }
    protected SteamUnifiedMessages.UnifiedService<IEcon> EconService { get; }

    public InventoryManager(SteamClient steamClient)
    {
        if (!steamClient.IsConnected || steamClient.SteamID == null)
        {
            throw new ArgumentException("Your steam client should be connected.");
        }

        SteamSteamClient = steamClient;
        SteamMessages = SteamSteamClient.GetHandler<SteamUnifiedMessages>() ?? throw new InvalidOperationException(nameof(SteamUnifiedMessages));
        EconService = SteamMessages.CreateService<IEcon>();
    }

    /// <summary>
    ///     Request inventory items via IEcon system
    /// </summary>
    public async Task<InventoryResponse> GetInventory(InventoryRequest inventoryRequest)
    {
        ArgumentNullException.ThrowIfNull(inventoryRequest, nameof(inventoryRequest));
        
        var steamKitRequest = new CEcon_GetInventoryItemsWithDescriptions_Request
        {
            steamid = SteamSteamClient.SteamID!.ConvertToUInt64(),
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
        
        var rawResponse = await EconService.SendMessage(inventory => inventory.GetInventoryItemsWithDescriptions(steamKitRequest));
        var response = rawResponse.GetDeserializedResponse<CEcon_GetInventoryItemsWithDescriptions_Response>();
        var inventoryResponse = new InventoryResponse(response);
        
        return inventoryResponse;
    }
}