using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using SteamKit2.Internal;
using SteamKit2.Managers.Managers.Entities.Inventory;
using CMsgClientHello = SteamKit2.Internal.CMsgClientHello;

namespace SteamKit2.Managers.Managers.Games;

internal class CounterStrikeInventoryManager : InventoryManager
{
    /// <summary>
    /// Counter Strike application ID
    /// </summary>
    private const int AppId = 730;

    public CounterStrikeInventoryManager(SteamClient steamClient) : base(steamClient)
    {
        GameCoordinator = SteamClient.GetHandler<SteamGameCoordinator>() ??
                          throw new InvalidOperationException(nameof(SteamGameCoordinator));
        CallbackManager = new CallbackManager(steamClient);
        
        SubscribeToEvents();
        EstablishGcConnection();
    }

    public CallbackManager CallbackManager { get; }

    private SteamGameCoordinator GameCoordinator { get; }

    public InventoryResponse GetStorageUnitItems()
    {
        /*var clientMsgProtobuf = new ClientGCMsgProtobuf<CMsgCasketItem>((uint)EGCItemMsg.k_EMsgGCCasketItemAdd);
        clientMsgProtobuf.Body.casket_item_id = 29022305680;
        clientMsgProtobuf.Body.item_item_id = 29022305680;
        GameCoordinator.Send(clientMsgProtobuf, AppId);
        throw new NotImplementedException();*/
        
        return new InventoryResponse();
    }

    private void OnGcMessage(SteamGameCoordinator.MessageCallback callback)
    {
        // setup our dispatch table for messages
        // this makes the code cleaner and easier to maintain
        var messageMap = new Dictionary<uint, Action<IPacketGCMsg>>
        {
            { ( uint )EGCBaseClientMsg.k_EMsgGCClientWelcome, OnClientWelcome },
            //{ ( uint )EDOTAGCMsg.k_EMsgGCMatchDetailsResponse, OnMatchDetails },
        };

        Action<IPacketGCMsg> func;
        if ( !messageMap.TryGetValue( callback.EMsg, out func ) )
        {
            // this will happen when we recieve some GC messages that we're not handling
            // this is okay because we're handling every essential message, and the rest can be ignored
            return;
        }

        func( callback.Message );
    }
    
    void OnClientWelcome(IPacketGCMsg packetMsg)
    {
        // in order to get at the contents of the message, we need to create a ClientGCMsgProtobuf from the packet message we recieve
        // note here the difference between ClientGCMsgProtobuf and the ClientMsgProtobuf used when sending ClientGamesPlayed
        // this message is used for the GC, while the other is used for general steam messages
        var msg = new ClientGCMsgProtobuf<CMsgClientWelcome>( packetMsg );

        Console.WriteLine( "GC is welcoming us. Version: {0}", msg.Body.version );

        //Console.WriteLine( "Requesting details of match {0}", matchId );

        // at this point, the GC is now ready to accept messages from us
        // so now we'll request the details of the match we're looking for
        
        var clientMsgProtobuf = new ClientGCMsgProtobuf<CMsgCasketItem>((uint)EGCItemMsg.k_EMsgGCCasketItemAdd);
        clientMsgProtobuf.Body.casket_item_id = 29022305680;
        clientMsgProtobuf.Body.item_item_id = 29022305680;
        GameCoordinator.Send(clientMsgProtobuf, AppId);
    }

    private void EstablishGcConnection()
    {
        var playGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

        playGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
        {
            game_id = new GameID(AppId),
        });
        
        SteamClient.Send(playGame);
        Thread.Sleep(5000);
        
        var clientHello = new ClientGCMsgProtobuf<CMsgClientHello>(( uint )EGCBaseClientMsg.k_EMsgGCClientHello);
        GameCoordinator.Send(clientHello, AppId);
    }
    
    private void SubscribeToEvents()
    {
        CallbackManager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGcMessage);

        new TaskFactory().StartNew(() =>
        {
            while (true)
            {
                CallbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        });
    }
}