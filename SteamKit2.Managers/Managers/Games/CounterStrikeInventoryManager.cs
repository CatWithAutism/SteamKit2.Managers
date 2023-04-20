namespace SteamKit2.Managers.Managers.Games;

internal class CounterStrikeInventoryManager : InventoryManager, IDisposable
{
    private CounterStrikeInventoryManager(SteamClient steamClient) : base(steamClient)
    {
        GameCoordinator = SteamSteamClient.GetHandler<SteamGameCoordinator>() ??
                          throw new InvalidOperationException(nameof(SteamGameCoordinator));
    }

    private static Dictionary<ulong, CounterStrikeInventoryManager> InstanceManager { get; } = new();
    private SteamGameCoordinator GameCoordinator { get; set; }

    internal static CounterStrikeInventoryManager GetCounterStrikeInventoryManager(SteamClient steamClient)
    {
        if (!steamClient.IsConnected || steamClient.SteamID == null)
        {
            throw new ArgumentException("Your steam client should be connected.");
        }
        
        if (InstanceManager.TryGetValue(steamClient.SteamID.ConvertToUInt64(), out var cachedManager))
            return cachedManager;

        var inventoryManager = new CounterStrikeInventoryManager(steamClient);
        InstanceManager.Add(steamClient.SteamID.ConvertToUInt64(), inventoryManager);

        return inventoryManager;
    }


    public void GetStorageUnitItems()
    {
        
    }

    /// <summary>
    /// Remove instance from instance manager.
    /// </summary>
    public void Dispose()
    {
        InstanceManager.Remove(SteamSteamClient.SteamID!.ConvertToUInt64());
    }
}