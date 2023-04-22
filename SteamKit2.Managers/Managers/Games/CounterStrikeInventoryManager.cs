using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Managers.Managers.Entities.Inventory;

namespace SteamKit2.Managers.Managers.Games;

internal class CounterStrikeInventoryManager : InventoryManager
{
    private const int CounterStrikeAppId = 730;

    public CounterStrikeInventoryManager(SteamClient steamClient) : base(steamClient)
    {
        GameCoordinator = SteamClient.GetHandler<SteamGameCoordinator>() ??
                          throw new InvalidOperationException(nameof(SteamGameCoordinator));
    }

    public CounterStrikeInventoryManager(SteamClient steamClient, SteamGameCoordinator gameCoordinator) :
        base(steamClient)
    {
        GameCoordinator = gameCoordinator;
        CallbackManager = new CallbackManager(steamClient);
        SubscribeToEvents();
    }

    public CounterStrikeInventoryManager(SteamClient steamClient, SteamGameCoordinator gameCoordinator,
        CallbackManager callbackManager) : base(steamClient)
    {
        GameCoordinator = gameCoordinator;
        CallbackManager = callbackManager;
        SubscribeToEvents();
    }

    public CallbackManager CallbackManager { get; }

    private SteamGameCoordinator GameCoordinator { get; }

    public Task<InventoryResponse> GetStorageUnitItems()
    {
        var clientMsgProtobuf = new ClientGCMsgProtobuf<CMsgCasketItem>((uint)EGCItemMsg.k_EMsgGCCasketItemAdd);
        GameCoordinator.Send(clientMsgProtobuf, CounterStrikeAppId);
        throw new NotImplementedException();
    }

    private void OnGcMessage(SteamGameCoordinator.MessageCallback callback)
    {
        throw new NotImplementedException();
    }

    private void SubscribeToEvents()
    {
        CallbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGcMessage);
    }
}