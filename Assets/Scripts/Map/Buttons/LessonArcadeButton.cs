using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessonArcadeButton : MapButton
{
	[SerializeField] private MapManager mapManager;

	private void Start()
	{
		mapManager = FindObjectOfType<MapManager>();
		SetTooltipText($"Packet {packetIDDisplayed} Arcade", true);
		TurnOnSpinner(false);
		SetCamera();
		IsLocked = _isLocked;
	}

	public override void OnButtonClick()
	{
		// Update LoginSession
		GlobalManager.Instance.currentLoginSession.packetsInteractedWith.Add(packetIDDisplayed);
		PlayfabPostManager.Instance.PostAllLoginSessions(GlobalManager.Instance.allLoginSessions);
		
		// set backend to lesson packet and go to arcade
		Debug.Log($"set backend to lesson packet up to packet {packetIDDisplayed} and go to arcade");

		GlobalManager.Instance.CurrentPacket = packetIDDisplayed;
		GlobalManager.Instance.ReviewPreviousPackets = false;

		mapManager.StartArcade();
	}

	private void OnValidate()
	{
		SetTooltipText($"Packet {packetIDDisplayed} Arcade", false);
	}
}
