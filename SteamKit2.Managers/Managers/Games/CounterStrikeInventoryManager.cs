using SteamKit2.GC.Artifact.Internal;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;
using SteamKit2.Managers.Managers.Entities.Inventory;

namespace SteamKit2.Managers.Managers.Games;

public class CounterStrikeInventoryManager : InventoryManager
{
    public CounterStrikeInventoryManager(SteamClient client) : base(client)
    {
        
    }

    public void GetStorageUnitItems()
    {
        throw new NotImplementedException();
    }
    
}