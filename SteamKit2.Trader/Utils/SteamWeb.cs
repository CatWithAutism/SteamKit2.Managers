using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using System;
using SteamKit2.Authentication;

namespace SteamKit2.Trader.Utils;

/// <summary>
///     SteamWeb class to create an API endpoint to the Steam Web.
/// </summary>
public class SteamWeb
{
    public SteamWeb(ulong steamId, EUniverse universe, string webAPIUserNonce)
    {
        if (steamId == 0 || !new SteamID(steamId).IsIndividualAccount)
        {
            throw new ArgumentOutOfRangeException(nameof(steamId));
        }

        if (universe == EUniverse.Invalid || !Enum.IsDefined(universe))
        {
            throw new InvalidEnumArgumentException(nameof(universe), (int)universe, typeof(EUniverse));
        }

        if (string.IsNullOrEmpty(webAPIUserNonce))
        {
            throw new ArgumentNullException(nameof(webAPIUserNonce));
        }

        if (!Authenticate(steamId, universe, webAPIUserNonce))
        {
            throw new AuthenticationException("Cannot authenticate via given credentials.", EResult.InvalidLoginAuthCode);
        }
    }
    
    /// <summary>
    ///     Base steam community domain.
    /// </summary>
    private const string SteamCommunityDomain = "steamcommunity.com";

    /// <summary>
    ///     CookieContainer to save all cookies during the Login.
    /// </summary>
    private readonly CookieContainer _cookies = new();

    /// <summary>
    ///     This method is using the Request method to return the full http stream from a web request as string.
    /// </summary>
    /// <param name="url">URL of the http request.</param>
    /// <param name="method">Gets the HTTP data transfer method (such as GET, POST, or HEAD) used by the client.</param>
    /// <param name="data">A NameValueCollection including Headers added to the request.</param>
    /// <param name="ajax">A bool to define if the http request is an ajax request.</param>
    /// <param name="referer">Gets information about the URL of the client's previous request that linked to the current URL.</param>
    /// <param name="fetchError">
    ///     If true, response codes other than HTTP 200 will still be returned, rather than throwing
    ///     exceptions
    /// </param>
    /// <returns>The string of the http return stream.</returns>
    /// <remarks>If you want to know how the request method works, use: <see cref="SteamWeb.Request" /></remarks>
    public async Task<string> Fetch(string url, HttpMethod method, NameValueCollection data = null, bool ajax = true,
        string referer = "", bool fetchError = false)
    {
        using var response = await Request(url, method, data, ajax, referer, fetchError);
        await using var responseStream = response.GetResponseStream();

        if (responseStream == null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(responseStream);
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    ///     Custom wrapper for creating a HttpWebRequest, edited for Steam.
    /// </summary>
    /// <param name="url">Gets information about the URL of the current request.</param>
    /// <param name="method">Gets the HTTP data transfer method (such as GET, POST, or HEAD) used by the client.</param>
    /// <param name="data">A NameValueCollection including Headers added to the request.</param>
    /// <param name="ajax">A bool to define if the http request is an ajax request.</param>
    /// <param name="referer">Gets information about the URL of the client's previous request that linked to the current URL.</param>
    /// <param name="fetchError">Return response even if its status code is not 200</param>
    /// <returns>An instance of a HttpWebResponse object.</returns>
    public async Task<HttpWebResponse> Request(string url, HttpMethod method, NameValueCollection data = null,
        bool ajax = true, string referer = "", bool fetchError = false)
    {
        var dataString = data == null
            ? null
            : string.Join("&", Array.ConvertAll(data.AllKeys, key =>
                $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(data[key])}"
            ));

        if (method == HttpMethod.Get && !string.IsNullOrEmpty(dataString))
        {
            url += (url.Contains('?') ? "&" : "?") + dataString;
        }

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = HttpMethod.Get.Method;
        request.Accept = "application/json, text/javascript;q=0.9, */*;q=0.5";
        request.Headers[HttpRequestHeader.AcceptLanguage] = "en";
        request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        request.UserAgent =
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
        request.Referer = string.IsNullOrEmpty(referer) ? "http://steamcommunity.com/trade/1" : referer;
        request.Timeout = 50000; // Timeout after 50 seconds.
        request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        if (ajax)
        {
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("X-Prototype-Version", "1.7");
        }

        request.CookieContainer = _cookies;

        if (method == HttpMethod.Get || string.IsNullOrEmpty(dataString))
        {
            return request.GetResponse() as HttpWebResponse;
        }

        var dataBytes = Encoding.UTF8.GetBytes(dataString);
        request.ContentLength = dataBytes.Length;

        try
        {
            await using var requestStream = request.GetRequestStream();
            await requestStream.WriteAsync(dataBytes);
            return request.GetResponse() as HttpWebResponse;
        }
        catch (WebException ex)
        {
            if (fetchError)
            {
                if (ex.Response is HttpWebResponse resp)
                {
                    return resp;
                }
            }

            throw;
        }
    }

    private bool Authenticate(ulong steamID, EUniverse universe, string webAPIUserNonce)
    {
        var publicKey = KeyDictionary.GetPublicKey(universe);

        if (publicKey == null || publicKey.Length == 0)
        {
            return false;
        }

        var sessionKey = CryptoHelper.GenerateRandomBlock(32);

        using RSACrypto rsa = new(publicKey);
        var encryptedSessionKey = rsa.Encrypt(sessionKey);
        var loginKey = Encoding.UTF8.GetBytes(webAPIUserNonce);
        var encryptedLoginKey = CryptoHelper.SymmetricEncrypt(loginKey, sessionKey);

        Dictionary<string, object?> arguments = new(3, StringComparer.Ordinal)
        {
            { "encrypted_loginkey", encryptedLoginKey },
            { "sessionkey", encryptedSessionKey },
            { "steamid", steamID }
        };

        KeyValue? response;


        using var steamUserAuthService = WebAPI.GetInterface("ISteamUserAuth");
        try
        {
            response = steamUserAuthService.Call(HttpMethod.Post, "AuthenticateUser", args: arguments);
        }
        catch (Exception e)
        {
            return false;
        }

        var steamLogin = response["token"].AsString();
        var steamLoginSecure = response["tokensecure"].AsString();
        var sessionID = Convert.ToBase64String(Encoding.UTF8.GetBytes(steamID.ToString(CultureInfo.InvariantCulture)));

        if (string.IsNullOrEmpty(steamLogin))
        {
            return false;
        }
        

        _cookies.Add(new Cookie("sessionid", sessionID, string.Empty, SteamCommunityDomain));
        _cookies.Add(new Cookie("steamLogin", steamLogin, string.Empty, SteamCommunityDomain));
        _cookies.Add(new Cookie("steamLoginSecure", steamLoginSecure, string.Empty, SteamCommunityDomain));
        _cookies.Add(new Cookie("Steam_Language", "english", string.Empty, SteamCommunityDomain));

        return true;
    }
}