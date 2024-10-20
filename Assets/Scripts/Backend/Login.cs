using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using TMPro; 
public class Login : MonoBehaviour
{


    void Start(){
        // StudentLoginActivate();
    }
    public void StudentLoginActivate(string customID) {
        var request = new LoginWithCustomIDRequest {
        CustomId = customID
        };
        PlayFabClientAPI.LoginWithCustomID(request, StudentOnLoginSuccess, OnError);
        
        Debug.Log("Login sent");

    }



     void StudentOnLoginSuccess(LoginResult result) {
        Debug.Log("Login success!");
    }

    void OnError(PlayFabError error) {
         Debug.Log(error.ErrorMessage);
    }

}
