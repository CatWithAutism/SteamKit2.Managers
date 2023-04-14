using SteamKit2.Internal;

namespace SteamKit2.Managers.Managers.Entities.Inventory;

public class InventoryItem
{
    public InventoryItem(CEcon_Asset asset, CEconItem_Description descriptionAsset)
    {
        ArgumentNullException.ThrowIfNull(asset, nameof(asset));
        ArgumentNullException.ThrowIfNull(descriptionAsset, nameof(descriptionAsset));

        if (asset.classid != descriptionAsset.classid)
        {
            throw new ArgumentException($"ClassIds of {nameof(asset)} and {nameof(descriptionAsset)} are not equals.");
        }

        AssetId = asset.assetid;
        ContextId = asset.contextid;
        AppId = asset.appid;
        Amount = asset.amount;
        ClassId = asset.classid;
        InstanceId = asset.instanceid;
        Tradable = descriptionAsset.tradable;
        MarketHashName = descriptionAsset.market_hash_name;
    }

    public string MarketHashName { get; set; }

    public ulong InstanceId { get; set; }

    public long Amount { get; set; }

    public uint AppId { get; set; }

    public ulong ContextId { get; set; }

    public ulong ClassId { get; set; }

    public ulong AssetId { get; set; }

    public bool Tradable { get; set; }
}