using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Tradelite.SDK.Model.ConfigScope;
using Tradelite.SDK.Model.UserScope;
using Tradelite.SDK.Service.UserScope;
using Tradelite.SDK.Service.ConfigScope;

public class TestControllerAsync : MonoBehaviour
{

    private GameConfiguration gameConfig;
    private string authToken;
    private bool forceReload = true;

    [ContextMenu("Force Singleton Reload")]
    public void _Z_ToggleForceReload()
    { 
        forceReload = !forceReload;
        Debug.Log(forceReload ? "Services will be now forced to reload" : "Services will be now always using the initial instance");
    }   

    [SerializeField] TMP_Text feedbackTextField;
    [SerializeField] TMP_InputField gameIdInputField;
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TMP_InputField usernameSuffixInputField;
    [SerializeField] TMP_InputField runIdInputField;
    [SerializeField] TMP_InputField questionIdInputField;
    [SerializeField] TMP_InputField instrumentIdInputField;
    [SerializeField] TMP_InputField instrumentIdsInputField;

    void Start()
    {
        feedbackTextField.text = "Set the Game ID with something like 'StockTiles_Local' and load the configuration.";
        /**/
        gameIdInputField.text = "StockTiles_Local";
        usernameInputField.text = "ddd";
        passwordInputField.text = "ddd";
        /**/
    }

    [ContextMenu("Load Config (async)")]
    public async void _A_LoadConfig()
    { 
        string id = gameIdInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            feedbackTextField.text = "Missing Game ID!\nSet the Game ID with something like 'StockTiles_Local' and load the configuration.";
        }
        else 
        {
            try
            {
                GameConfigurationService service = GameConfigurationService.GetInstance(id, forceReload);
                gameConfig = await service.Get();
                feedbackTextField.text = "Configuration retrieved.\nTemporarily, the SDK need administrative credentials, so trigger the admin authentication.";
            }
            catch(Exception)
            {
                feedbackTextField.text = "Invalid Game ID!\nSet the Game ID with something like 'StockTiles_Local' and load the configuration.";
            }

        }
    }

    [ContextMenu("Authenticate Admin (async)")]
    public async void _B_AuthenticateAdmin()
    { 
        string username = gameConfig.GetCredential("adminName");
        string password = gameConfig.GetCredential("adminPsw");
        string message = await _Authenticate("Admin", username, password);
        feedbackTextField.text = message;
    }

    [ContextMenu("Authenticate Player (async)")]
    public async void _B_AuthenticatePlayer()
    { 
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string message = await _Authenticate("Player", username, password);
        feedbackTextField.text = message;
    }

    protected async Task<string> _Authenticate(string title, string username, string password)
    { 
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return $"Missing {title} Credentials!\nSet the {title} credentials and authorize her..";
        }

        AuthenticationService service = AuthenticationService.GetInstance(gameConfig, forceReload);
        try
        {   
            authToken = await service.Authenticate(username, password);
        }
        catch(Exception)
        {
            return $"{title} authentication failed!\nSet the {title} credentials and authorize her.";
        }

        return $"{title} successfully authorized.\nYou can create a new player or get data for a matching game.";
    }

    [ContextMenu("Create Random User (async)")]
    public async void _C_CreateRandomUser()
    { 
        UserService service = UserService.GetInstance(gameConfig, authToken, forceReload);

        User user = new User();
        user.id = null;
        string suffix = usernameSuffixInputField.text;
        if (string.IsNullOrEmpty(suffix)) 
        {
            suffix = "" + UnityEngine.Random.Range(1, 10000);
            usernameSuffixInputField.text = suffix;
        }
        user.firstname = "Fable";
        user.lastname = "Vader";
        user.email = user.firstname + "_" + user.lastname + "_" + suffix + "@example.com";
        user.password = "Password" + suffix;
        user.dateAcceptConditions = "2000-01-01T00:00:00.0Z";
        user.acceptConditionsVersion = "v1.0";
        user.ageVerification = true;
        user.mobile = true;

        try
        {
            string id = await service.Create(user);
            Debug.Log("user id: " + id);
            User copy = await service.Get(id);
            feedbackTextField.text = $"Player created with id {id} and credentials {{ {copy.email} / {user.password } }}.";
            Debug.Log($"Created Player: {user}");
        }
        catch(Exception)
        {
            feedbackTextField.text = "User creation failed...";
        }
    }

    [ContextMenu("Get Logged User Record (async)")]
    public async void _D_GetLoggedUserRecord()
    { 
        AuthenticationService service = AuthenticationService.GetInstance(gameConfig, forceReload);

        User user = await service.GetLoggedUser();
        Debug.Log($"Logged Player: {user}");
    }

    public void _E_GetMatchingRun() {}
    
    public void _E_GetMatchingQuestion() {}

    public void _F_GetInstrument() {}

    public void _F_GetInstruments() {}
}