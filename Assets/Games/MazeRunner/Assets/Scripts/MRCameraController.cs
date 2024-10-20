using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MRCameraController : MonoBehaviour
{
	[Header("Camera Positions")]
	[SerializeField] private Transform mainMenuLocation;
	[SerializeField] private Transform gameplayStartLocation;

	[Header("References")]
	[SerializeField] private Animator canvasAnimator;
	[SerializeField] private MRPlayer player;
	[SerializeField] private MazeMenuScreenManager menuScreenManager;
	[SerializeField] private MazeGameMechanics mazeGameMechanics;

	private const float SLERP_DISTANCE_THRESHOLD = 1.0f;
	private bool beginPlayerCameraTracking;
	private Vector3 cameraEndPoint; // Direction from (0, 0, 0)
    private Vector3 cameraDirection; // Direction from player
	private Vector3 cameraGameplayRotation;

    // Start is called before the first frame update
    private void Start()
    {
		menuScreenManager.OnGameActivated += MoveCameraToGameplayPosition;

		// Set default camera angle and starting point
		cameraDirection = (Vector3.up * 4 + Vector3.back) * 4;
		cameraEndPoint = cameraDirection;
		cameraGameplayRotation = new Vector3(75, 0, 0);
    }

	private void FixedUpdate()
	{
		if (beginPlayerCameraTracking)
		{
			HandlePlayerCameraTracking();
		}
	}

	public void BeginIntroAnimation()
	{
		transform.position = mainMenuLocation.position;
		transform.rotation = mainMenuLocation.rotation;

		transform.Rotate(new Vector3(-55, 0, 0));

		StartCoroutine(AnimateIntroAnimation(mainMenuLocation.position, mainMenuLocation.rotation, 1.0f));
	}

	private IEnumerator AnimateIntroAnimation(Vector3 endingPositionVals, Quaternion endingRotationVals, float durationSeconds)
	{
		float elapsedTime = 0.0f;

		while (elapsedTime < durationSeconds)
		{
			elapsedTime += Time.deltaTime;
			transform.position = Vector3.Slerp(transform.position, endingPositionVals, (elapsedTime / durationSeconds));
			transform.rotation = Quaternion.Slerp(transform.rotation, endingRotationVals, 0.2f);
			// yield return new WaitForEndOfFrame();
			yield return null;
		}

		canvasAnimator.SetTrigger("TriggerMenuScreen");
		yield return 0;
	}

	public void MoveCameraToGameplayPosition()
	{
		canvasAnimator.SetTrigger("TriggerMenuScreen");

		transform.position = gameplayStartLocation.position;
		transform.rotation = Quaternion.Euler(cameraGameplayRotation);
		
		StartCoroutine(WaitHalfASecond());

		beginPlayerCameraTracking = true;
	}

	private void HandlePlayerCameraTracking()
	{
        // Check if the distance between the player and the camera is big enough to start moving
        if (Vector3.Distance(transform.position, player.transform.position) > SLERP_DISTANCE_THRESHOLD)
        {
            // Start moving towards the player
            transform.position = Vector3.Slerp(transform.position, cameraEndPoint, 5 * Time.deltaTime);
        }
        cameraEndPoint = player.transform.position + cameraDirection;

		// if (cutsceneMode) transform.LookAt(player.transform);
    }

	private IEnumerator WaitHalfASecond()
	{
		yield return new WaitForSeconds(0.5f);
	}

	private void OnDestroy()
	{
		menuScreenManager.OnGameActivated += MoveCameraToGameplayPosition;
	}
}
