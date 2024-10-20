using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    // private static GlobalManager instance;
	// public static GlobalManager Instance
	// {
	// 	get
	// 	{
	// 		if (instance != null) return instance;

	// 		instance = FindObjectOfType<GlobalManager>();
	// 		if (instance == null)
	// 		{
	// 			Debug.LogWarning("No GlobalManager in the scene!");
	// 			return null;
	// 		}

	// 		return instance;
	// 	}
	// }

	public static GlobalManager Instance;
	public bool firstTimeEntrance;

	public AllLoginSessions allLoginSessions;

	public LoginSession currentLoginSession;
	//public StudentData globalStudentData

	// Data to pass into arcade games
	public int CurrentPacket = 0; // Packet_0 starts at index 0
	public LessonData currentLessonData;
	public float GameStartTime = 0;

	public int CurrentReview = 0; // Review_0 starts at index 0
	public ReviewData currentReviewData;
	public bool ReviewPreviousPackets = false;

	// Map data
	public List<bool> MapIconIsLockedStatus = new List<bool>();
	public int TotalTicketsPlayerHas = 0;
	public bool DisplayTicketsCollectedPanel = false;
	public int TicketsRecentlyCollected = 0;

	[SerializeField] private IconManager iconManager;

	// public PlayfabGetManager getManager;


	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject); //Bug fix to ensure intance isn't duplicated
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}


	public Sprite GetIcon(int id)
    {
		return iconManager.GetIcon(id);
    }

	public void UpdateGlobalTickets(int ticketsToAdd)
	{
		Debug.Log($"Updating global coins by {ticketsToAdd}");
		TotalTicketsPlayerHas += ticketsToAdd;
		TicketsRecentlyCollected += ticketsToAdd;
		DisplayTicketsCollectedPanel = true;
		PlayfabPostManager.Instance.PostTotalPlayerTickets(TotalTicketsPlayerHas);
	}
}
