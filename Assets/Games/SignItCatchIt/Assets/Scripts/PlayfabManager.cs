// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using PlayFab;
// using PlayFab.ClientModels;
// using TMPro;

// public class PlayfabManager : MonoBehaviour
// {
//     public BasketController basket;

//     [SerializeField] private TextMeshProUGUI[] leaderboardEntries;

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
//             CreateAccount = true,
//         };
//         PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
//     }

//     void OnSuccess(LoginResult result)
//     {
//         Debug.Log("Successful login/account create!");
//     }

//     void OnError(PlayFabError error)
//     {
//         Debug.Log("Playfab - Error while processing user request!");
//         Debug.Log(error.GenerateErrorReport());
//     }

//     public void SendLeaderboard(int score)
//     {
//         var request = new UpdatePlayerStatisticsRequest
//         {
//             Statistics = new List<StatisticUpdate>
//             {
//                 new StatisticUpdate
//                 {
//                     StatisticName = "PlatformScore",
//                     Value = score,
//                 }
//             }
//         };
//         PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
//     }

//     void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
//     {
//         Debug.Log("Successful leaderboard sent");
//     }

//     public void GetLeaderboard()
//     {
//         var request = new GetLeaderboardRequest
//         {
//             StatisticName = "PlatformScore",
//             StartPosition = 0,
//             MaxResultsCount = 10,
//         };
//         PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
//     }

//     void OnLeaderboardGet(GetLeaderboardResult result)
//     {
//         foreach (var item in result.Leaderboard)
//         {
//             Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
//             SignItGlobals.leaderboardEntries.Add(item);
//         }
//         for (int i = 0; i < result.Leaderboard.Count; i++)
//         {
//             Debug.Log($"Value: ${result.Leaderboard[i].StatValue}");
//             leaderboardEntries[i].text = result.Leaderboard[i].StatValue.ToString();
//         }
//         if (result.Leaderboard.Count < leaderboardEntries.Length)
//         {
//             for (int i = result.Leaderboard.Count; i < leaderboardEntries.Length; i++)
//             {
//                 leaderboardEntries[i].text = "0";
//             }
//         }
//     }

//     PlayerLeaderboardEntry[] OnLeaderboardGetReturn(GetLeaderboardResult result)
//     {
//         PlayerLeaderboardEntry[] results = new PlayerLeaderboardEntry[result.Leaderboard.Count];

//         for (int i = 0; i < result.Leaderboard.Count; i++)
//         {
//             results[i] = result.Leaderboard[i];
//         }

//         return results;
//     }

//     public void GetScore()
//     {
//         PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnError);
//     }

//     void OnDataReceived(GetUserDataResult result)
//     {
//         Debug.Log("Received user data!");
//         if (result.Data != null &&
//             result.Data.ContainsKey("Score"))
//         {
//             Debug.Log("basket score: " + result.Data["Score"].Value);
//         } else
//         {
//             Debug.Log("Player data not complete!");
//         }
//     }

//     public void SaveScore(int score)
//     {
//         var request = new UpdateUserDataRequest {
//             Data = new Dictionary<string, string>
//             {
//                 {"Score", score.ToString()},
//             }
//         };
//         PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
//     }

//     void OnDataSend(UpdateUserDataResult result)
//     {
//         Debug.Log("Successful user data send!");
//     }
// }
