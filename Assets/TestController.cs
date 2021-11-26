using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tradelite.SDK.Model.UserScope;
using Tradelite.SDK.Service.UserScope;


namespace Sample
{
public class TestController : MonoBehaviour
{

    [ContextMenu("Test Service")]
    // public void testService()
    public async void testService()
    { 
        Debug.Log($"Instantiating the UserService from the Tradelite SDK");
        UserService userService = UserService.GetInstance();

        Debug.Log("About to get the user info");
        User user = await userService.Get("12345");
        Debug.Log($"User: {user}");
        
        User[] users = await userService.Select();
        Debug.Log($"Users.Length: {users.Length}");
        Debug.Log($"Users[0]: {users[0]}");
    }
}
}