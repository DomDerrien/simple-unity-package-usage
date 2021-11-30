using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tradelite.SDK.Model.ConfigScope;
using Tradelite.SDK.Model.UserScope;
using Tradelite.SDK.Service.UserScope;
using Tradelite.SDK.Service.ConfigScope;


public class TestController : MonoBehaviour
{
    private GameConfiguration gameConfig;
    private static GameIdentifier gameId = GameIdentifier.StockTiles;
    private string authToken;

    [ContextMenu("Test Configuration Service")]
    public async void TestConfigService()
    { 
        Debug.Log($"Instantiating the ConfigService from the Tradelite SDK");
        GameConfigurationService service = GameConfigurationService.GetInstance(gameId);

        gameConfig = await service.Get();
        Debug.Log("Configuration: " + gameConfig);
    }

    [ContextMenu("Test Authentication Service")]
    public async void TestAuthenticationService()
    { 
        Debug.Log($"Instantiating the AuthenticationService from the Tradelite SDK");
        AuthenticationService service = AuthenticationService.GetInstance(gameConfig);

        string creds = "anything!";
        authToken = await service.Authenticate(creds, creds);
        Debug.Log("Token: " + authToken);
    }

    [ContextMenu("Test User Service")]
    public async void TestUserService()
    { 
        Debug.Log($"Instantiating the UserService from the Tradelite SDK");
        UserService service = UserService.GetInstance(gameConfig, authToken);

        Debug.Log("About to get the user info");
        User user = await service.Get("12345");
        Debug.Log($"User: {user}");
        
        User[] users = await service.Select();
        Debug.Log($"Users.Length: {users.Length}");
        Debug.Log($"Users[0]: {users[0]}");
    }
}