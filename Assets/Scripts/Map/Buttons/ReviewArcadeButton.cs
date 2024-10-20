using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewArcadeButton : MapButton
{
	[SerializeField] private MapManager mapManager;

	private void Start()
	{
		mapManager = FindObjectOfType<MapManager>();
		SetTooltipText($"Review {reviewNumber} Arcade", true);
		TurnOnSpinner(false);
		SetCamera();
		IsLocked = _isLocked;
	}

	public override void OnButtonClick()
	{
		// Update LoginSession
		GlobalManager.Instance.currentLoginSession.packetsInteractedWith.Add(reviewNumber + 101000);
		PlayfabPostManager.Instance.PostAllLoginSessions(GlobalManager.Instance.allLoginSessions);

		// set backend to review packet and go to arcade
		Debug.Log($"set backend to review packet up to packet {packetIDDisplayed} and go to arcade");

		GlobalManager.Instance.CurrentPacket = packetIDDisplayed;
		GlobalManager.Instance.ReviewPreviousPackets = true;

		mapManager.StartArcade();
	}

	private void OnValidate()
	{
		SetTooltipText($"Review {reviewNumber} Arcade", false);
	}
}
