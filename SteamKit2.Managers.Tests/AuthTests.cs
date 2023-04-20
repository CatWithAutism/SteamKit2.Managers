using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using SteamKit2.Managers.Enums;
using SteamKit2.Managers.Games.CSGO;
using SteamKit2.Managers.Managers.Entities.Inventory;

namespace SteamKit2.Trader.Tests;

public class AuthTests
{
    private string? _authCode;
    private CounterStrikeClient _csClient;
    private bool _isRunning;
    private CallbackManager _manager;
    private string? _password;
    private SteamClient _steamClient;
    private SteamUser _steamUser;
    private string? _twoFactorAuth;
    private string? _username;

    [SetUp]
    public void Setup()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<ParserTests>()
            .Build();

        _username = configuration["Username"];
        _password = configuration["Password"];
        _twoFactorAuth = configuration["AuthCode"];

        _steamClient = new SteamClient();
        _manager = new CallbackManager(_steamClient);
        _steamUser = _steamClient.GetHandler<SteamUser>();

        _manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
        _manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
        _manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
        _manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
        _manager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);

        var isRunning = true;

        _steamClient.Connect();

        new TaskFactory().StartNew(() =>
        {
            while (isRunning)
            {
                _manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        });

        var counter = 0;
        while (!_steamClient.IsConnected || _steamClient.SteamID == null)
        {
            counter++;
            if (counter > 20)
            {
                throw new ArgumentException("Cannot connect to the steam. Probably wrong password or auth code.");
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }

    [Test]
    public async Task GetInventory()
    {
        var response = await _csClient.GetInventory(new InventoryRequest()
        {
            AppId = 730,
            Language = Language.English,
            Count = 1000,
            ContextId = 2,
            NeedDescription = true,
            OnlyMarketable = true,
            OnlyTradable = true,
            StartFromItem = 0,
            TradeOfferVerificationRequired = true
        });
    }

    private void OnConnected(SteamClient.ConnectedCallback callback)
    {
        Console.WriteLine("Connected to Steam! Logging in '{0}'...", _username);

        byte[] sentryHash = null;
        if (File.Exists("sentry.bin"))
        {
            // if we have a saved sentry file, read and sha-1 hash it
            var sentryFile = File.ReadAllBytes("sentry.bin");
            sentryHash = CryptoHelper.SHAHash(sentryFile);
        }

        _steamUser.LogOn(new SteamUser.LogOnDetails
        {
            Username = _username,
            Password = _password,
            AuthCode = _authCode,
            TwoFactorCode = _twoFactorAuth,
            SentryFileHash = sentryHash
        });
    }

    private void OnDisconnected(SteamClient.DisconnectedCallback callback)
    {
        Console.WriteLine("Disconnected from Steam, reconnecting in 5...");

        Thread.Sleep(TimeSpan.FromSeconds(5));

        _steamClient.Connect();
    }

    private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
    {
        var isSteamGuard = callback.Result == EResult.AccountLogonDenied;
        var is2FA = callback.Result == EResult.AccountLoginDeniedNeedTwoFactor;

        if (isSteamGuard || is2FA)
        {
            Console.WriteLine("This account is SteamGuard protected!");

            if (is2FA)
            {
                //Console.Write( "Please enter your 2 factor auth code from your authenticator app: " );
                //_twoFactorAuth = Console.ReadLine();
            }
            else
            {
                Console.Write("Please enter the auth code sent to the email at {0}: ", callback.EmailDomain);
                _authCode = Console.ReadLine();
            }

            return;
        }

        if (callback.Result != EResult.OK)
        {
            Console.WriteLine("Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult);

            _isRunning = false;
            return;
        }

        Console.WriteLine("Successfully logged on!");
        
        _csClient = new CounterStrikeClient(_steamClient, callback.WebAPIUserNonce);

        // at this point, we'd be able to perform actions on Steam
    }

    private void OnLoggedOff(SteamUser.LoggedOffCallback callback)
    {
        Console.WriteLine("Logged off of Steam: {0}", callback.Result);
    }

    private void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
    {
        Console.WriteLine("Updating sentryfile...");

        int fileSize;
        byte[] sentryHash;
        using (var fs = File.Open("sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            fs.Seek(callback.Offset, SeekOrigin.Begin);
            fs.Write(callback.Data, 0, callback.BytesToWrite);
            fileSize = (int)fs.Length;

            fs.Seek(0, SeekOrigin.Begin);
            using var sha = SHA1.Create();
            sentryHash = sha.ComputeHash(fs);
        }

        // inform the steam servers that we're accepting this sentry file
        _steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
        {
            JobID = callback.JobID,
            FileName = callback.FileName,
            BytesWritten = callback.BytesToWrite,
            FileSize = fileSize,
            Offset = callback.Offset,
            Result = EResult.OK,
            LastError = 0,
            OneTimePassword = callback.OneTimePassword,
            SentryFileHash = sentryHash
        });

        Console.WriteLine("Done!");
    }
}