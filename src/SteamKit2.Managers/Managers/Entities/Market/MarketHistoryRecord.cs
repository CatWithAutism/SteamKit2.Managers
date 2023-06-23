using SteamKit2.Managers.Managers.Entities.Market.Enums;

namespace SteamKit2.Managers.Managers.Entities.Market;

public class MarketHistoryRecord
{
    public string Id { get; set; }

    public string Classid { get; set; }
    
    public string NewId { get; set; }
    
    public int Appid { get; set; }

    public string Contextid { get; set; }
    
    public ActionType ActionType { get; set; }
    
    public uint Price { get; set; }
    
    
}