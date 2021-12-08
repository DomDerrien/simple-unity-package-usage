using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tradelite.SDK.Model.ConfigScope;
using Tradelite.SDK.Model.UserScope;
using Tradelite.SDK.Service.UserScope;
using Tradelite.SDK.Service.ConfigScope;


public class TestControllerAsync : MonoBehaviour
{
#if LOCAL_DEV
    private static GameIdentifier gameId = GameIdentifier.StockTiles_Local;
#else
    private static GameIdentifier gameId = GameIdentifier.StockTiles_Integration;
#endif

    private GameConfiguration gameConfig;
    private string authToken;
    private bool forceReload = true;

    [ContextMenu("Force Singleton Reload")]
    public void _Z_ToggleForceReload()
    { 
        forceReload = !forceReload;
        Debug.Log(forceReload ? "Services will be now forced to reload" : "Services will be now always using the initial instance");
    }   

    [ContextMenu("Load Config (async)")]
    public async void _A_LoadConfig()
    { 
        GameConfigurationService service = GameConfigurationService.GetInstance(gameId, forceReload);
        gameConfig = await service.Get();
        Debug.Log("Configuration: " + gameConfig);
    }

    [ContextMenu("Authenticate Admin (async)")]
    public async void _B_AuthenticateAdmin()
    { 
        AuthenticationService service = AuthenticationService.GetInstance(gameConfig, forceReload);
        authToken = await service.Authenticate(gameConfig.GetCredential("adminName"), gameConfig.GetCredential("adminPsw"));
        Debug.Log("Token: " + authToken);
    }

    [ContextMenu("Authenticate Player (async)")]
    public async void _B_AuthenticatePlayer()
    { 
        AuthenticationService service = AuthenticationService.GetInstance(gameConfig, forceReload);
        authToken = await service.Authenticate(gameConfig.GetCredential("playerName"), gameConfig.GetCredential("playerPsw"));
        Debug.Log("Token: " + authToken);
    }

    [ContextMenu("Create Random User (async)")]
    public async void _C_CreateRandomUser()
    { 
        UserService service = UserService.GetInstance(gameConfig, authToken, forceReload);

        User user = new User();
        user.id = null;
        int suffix = Random.Range(1, 10000);
        user.firstname = "Fable";
        user.lastname = "Vader";
        user.email = user.firstname + "_" + user.lastname + "_" + suffix + "@example.com";
        user.password = "Password" + suffix;
        user.dateAcceptConditions = "2000-01-01T00:00:00.0Z";
        user.acceptConditionsVersion = "v1.0";
        user.ageVerification = true;

        string id = await service.Create(user);
        Debug.Log($"Just created user id: {id}");

        User copy = await service.Get(id);
        Debug.Log($"Just retrieved user: {copy}");
    }

    [ContextMenu("Get Logged User Record (async)")]
    public async void _D_GetLoggedUserRecord()
    { 
        AuthenticationService service = AuthenticationService.GetInstance(gameConfig, forceReload);

        User user = await service.GetLoggedUser();
        Debug.Log($"Logged user: {user}");
    }
}