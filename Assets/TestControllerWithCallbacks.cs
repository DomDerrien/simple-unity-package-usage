using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tradelite.SDK.Model;
using Tradelite.SDK.Model.ConfigScope;
using Tradelite.SDK.Model.UserScope;
using Tradelite.SDK.Service.UserScope;
using Tradelite.SDK.Service.ConfigScope;


public class TestControllerWithCallbacks : MonoBehaviour
{
#if LOCAL_DEV
    private static GameIdentifier gameId = GameIdentifier.StockTiles_Local;
#else
    private static GameIdentifier gameId = GameIdentifier.StockTiles_Integration;
#endif

    private GameConfiguration gameConfig;
    private string authToken;
    private bool forceReload = true;

    public void ToggleForceReload()
    {
        forceReload = !forceReload;
        Debug.Log(forceReload ? "Services will be now forced to reload" : "Services will be now always using the initial instance");
    }

    [ContextMenu("Test Configuration Service (CB)")]
    public void TestConfigServiceCB()
    {
        GameConfigurationService service = GameConfigurationService.GetInstance(gameId, forceReload);
        service.Get(
            (GameConfiguration gC) => { gameConfig = gC; Debug.Log("Configuration: " + gameConfig); },
            (BaseError e) => { Debug.Log($"Error: {e.message}"); }
        );
    }

    [ContextMenu("Test Authentication Service (CB)")]
    public void TestAuthenticationServiceCB()
    {
        AuthenticationService service = AuthenticationService.GetInstance(gameConfig, forceReload);
        service.Authenticate(
            gameConfig.GetCredential("playerUser"),
            gameConfig.GetCredential("playerPsw"),
            (string aT) => { authToken = aT; Debug.Log("Token: " + authToken); },
            (BaseError e) => { Debug.Log($"Error: {e.message}"); }
        );
    }

    // [ContextMenu("Test User Service (CB)")]
    // public void TestUserServiceCB()
    // {
    //     Debug.Log($"Instantiating the UserService from the Tradelite SDK");
    //     UserService service = UserService.GetInstance(gameConfig, authToken, forceReload);

    //     Debug.Log("About to get the user info");
    //     User user = await service.Get("12345");
    //     Debug.Log($"User: {user}");

    //     User[] users = await service.Select();
    //     Debug.Log($"Users.Length: {users.Length}");
    //     Debug.Log($"Users[0]: {users[0]}");
    // }
}