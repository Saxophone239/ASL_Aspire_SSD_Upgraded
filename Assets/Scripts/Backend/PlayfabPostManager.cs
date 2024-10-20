using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System;

public class PlayfabPostManager : MonoBehaviour
{
	public static PlayfabPostManager Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}


	//Lesson specific routes and management
	public void PostLesson(LessonData lessonData)
	{
		var request = new UpdateUserDataRequest{
            Data = new Dictionary<string,string>{
                {$"Lesson {lessonData.packetID}",JsonConvert.SerializeObject(lessonData)}
            }
        
        };
        PlayFabClientAPI.UpdateUserData(request,OnLessonDataSend,OnError);

	}

	public IEnumerator PostLessonCoroutine(LessonData lessonData)
	{
		var request = new UpdateUserDataRequest
		{
            Data = new Dictionary<string,string>
			{
                {$"Lesson {lessonData.packetID}",JsonConvert.SerializeObject(lessonData)}
            }
        };
        
		bool isCompleted = false;
		PlayFabClientAPI.UpdateUserData(request,
			result =>
			{
				OnLessonDataSend(result);
				isCompleted = true;
			},
			error =>
			{
				OnError(error);
				isCompleted = true;
			}
		);

		yield return new WaitUntil(() => isCompleted);
	}


     void OnLessonDataSend(UpdateUserDataResult result){
        Debug.Log("Successful lesson user data sent!");
    }

	public void PostFirstTimeEntrance(){
        bool firstTime = true;
		var request = new UpdateUserDataRequest{
        Data = new Dictionary<string,string>{
                {$"FirstTimeEntrance",JsonConvert.SerializeObject(firstTime)}
            }
        
        };
        PlayFabClientAPI.UpdateUserData(request,OnFirstTimeSend,OnError);
	}

	public IEnumerator PostFirstTimeEntranceCoroutine()
	{
		// This is our player's first time, post data to playfab
        bool firstTime = true;
		var request = new UpdateUserDataRequest
		{
			Data = new Dictionary<string,string>
			{
				{$"FirstTimeEntrance",JsonConvert.SerializeObject(firstTime)}
			}
			
        };
		bool isCompleted = false;
        // PlayFabClientAPI.UpdateUserData(request,OnFirstTimeSend,OnError);
		PlayFabClientAPI.UpdateUserData(request,
			result =>
			{
				OnFirstTimeSend(result);
				isCompleted = true;
			},
			error =>
			{
				OnError(error);
				isCompleted = true;
			}
		);

		yield return new WaitUntil(() => isCompleted);
	}


    void OnFirstTimeSend(UpdateUserDataResult result)
	{
        Debug.Log("Welcome, new student!");
    }




	public void PostReview(ReviewData reviewData){
		var request = new UpdateUserDataRequest{
        Data = new Dictionary<string,string>{
                {$"Review {reviewData.reviewID}",JsonConvert.SerializeObject(reviewData)}
            }
        
        };
        PlayFabClientAPI.UpdateUserData(request,OnReviewDataSend,OnError);
	}

	public IEnumerator PostReviewCoroutine(ReviewData reviewData)
	{
		var request = new UpdateUserDataRequest
		{
			Data = new Dictionary<string,string>
			{
				{$"Review {reviewData.reviewID}",JsonConvert.SerializeObject(reviewData)}
			}
        };

		bool isCompleted = false;
		PlayFabClientAPI.UpdateUserData(request,
			result =>
			{
				OnReviewDataSend(result);
				isCompleted = true;
			},
			error =>
			{
				OnError(error);
				isCompleted = true;
			}
		);

		yield return new WaitUntil(() => isCompleted);
	}

	
     void OnReviewDataSend(UpdateUserDataResult result){
        Debug.Log("Successful review user data sent!");
    }



		//Lesson specific routes and management
	public void PostAllLoginSessions(AllLoginSessions allLoginSessions)
	{
		var request = new UpdateUserDataRequest{
            Data = new Dictionary<string,string>{
                {$"Login Sessions List",JsonConvert.SerializeObject(allLoginSessions)}
            }
        
        };
        PlayFabClientAPI.UpdateUserData(request,OnAllLoginSessionsSend,OnError);

	}



     void OnAllLoginSessionsSend(UpdateUserDataResult result){
        Debug.Log("Successful AllLoginSessions data playfab update!");
    }

	public void PostTotalPlayerTickets(int playerTickets)
	{
		var request = new UpdateUserDataRequest
		{
			Data = new Dictionary<string,string>
			{
				{$"TotalPlayerTickets",JsonConvert.SerializeObject(playerTickets)}
			}
		};

		PlayFabClientAPI.UpdateUserData(request, OnPlayerTicketsSend, OnError);
	}

	private void OnPlayerTicketsSend(UpdateUserDataResult result)
	{
		Debug.Log("Successful TotalPlayerTickets playfab update!");
	}

	void OnError(PlayFabError error){
        Debug.Log(error);
    }
}
