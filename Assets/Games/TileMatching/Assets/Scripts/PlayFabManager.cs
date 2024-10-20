// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using PlayFab;
// using PlayFab.ClientModels;

// public class PlayFabManager : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
//         Login();
//     }

//     void Login()
//     {
//         var request = new LoginWithCustomIDRequest
//         {
//             CustomId = SystemInfo.deviceUniqueIdentifier,
//             CreateAccount = true
//         };
//         PlayFabClientAPI.LoginWithCustomID(request, OnAccountSuccess, OnError);
//     }

//     void OnAccountSuccess(LoginResult result)
//     {
//         Debug.Log("Account created!");
//     }

//     static void OnError(PlayFabError error)
//     {
//         Debug.Log("Error");
//         Debug.Log(error.GenerateErrorReport());
//     }

//     public static void saveScore(int score)
//     {
//         var request = new UpdateUserDataRequest
//         {
//             Data = new Dictionary<string, string>
//             {
//                 {"Score", score.ToString()}
//             }
//         };
//         PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
//     }

//     static void OnDataSend(UpdateUserDataResult result)
//     {
//         Debug.Log("Data sent!");
//     }
// }