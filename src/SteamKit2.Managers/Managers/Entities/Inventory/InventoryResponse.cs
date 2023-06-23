using SteamKit2.Internal;

namespace SteamKit2.Managers.Managers.Entities.Inventory;

public class InventoryResponse
{
    public InventoryResponse(){}

    public InventoryResponse(CEcon_GetInventoryItemsWithDescriptions_Response inventoryResponse)
    {
        TotalInventoryCount = inventoryResponse.total_inventory_count;
        MoreItems = inventoryResponse.more_items;

        var mappedData = inventoryResponse.descriptions.Join(inventoryResponse.assets, description => description.classid, asset => asset.classid,
            (description, asset) => new
            {
                DescriptionAsset = description,
                Asset = asset,
            });

        InventoryItems = mappedData.Select(t => new InventoryItem(t.Asset, t.DescriptionAsset)).ToList();
    }
    
    public List<InventoryItem>? InventoryItems { get; set; }
    public uint TotalInventoryCount { get; set; }
    public bool MoreItems { get; set; }
}