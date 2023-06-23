using SteamKit2.Managers.Enums;

namespace SteamKit2.Managers.Managers.Entities.Inventory;

public class InventoryRequest
{
    public uint AppId { get; set; }
    public int Count { get; set; }
    public bool NeedDescription { get; set; }
    public Language Language { get; set; }
    public ulong ContextId { get; set; }
    public ulong StartFromItem { get; set; }
    public bool TradeOfferVerificationRequired { get; set; }
    public bool OnlyMarketable { get; set; }
    public bool OnlyTradable { get; set; }
}