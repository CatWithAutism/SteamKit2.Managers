namespace SteamKit2.Managers.Managers.Entities.Market;

public class MarketHistoryData
{
    public bool Success { get; set; }
    
    public int PageSize { get; set; }

    public int TotalCount { get; set; }
    
    public int Start { get; set; }
    
    public List<MarketHistoryRecord> MarketRecords { get; set; }
}