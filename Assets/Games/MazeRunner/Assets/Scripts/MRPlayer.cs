using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MRPlayer : MonoBehaviour {

	[Header("External Components")]
	// public GameObject ViewCamera = null;
	public AudioClip JumpSound = null;
	public AudioClip HitSound = null;
	public AudioClip CoinSound = null;
    // public Joystick joystick;
	public MazeSpawner MazeSpawner;

	public ParticleSystem PlasmaExplosion = null;
	public ParticleSystem EnergyExplosion = null;

    [Header("Player Settings")]
    public int Speed = 5;
    public int TurningSpeed = 1000;
	public int JumpAmount = 5;

    // Components
    private Rigidbody rigidBody = null;
	private AudioSource audioSource = null;
	private Animator animator = null;
	private Renderer playerRenderer = null;
	private int isRunningHash = 0;
	[SerializeField] private GameObject shield = null;
	[SerializeField] private LayerMask whatIsGround;

    // Parameters
    private const float SLERP_DISTANCE_THRESHOLD = 1.0f;

	private Vector2 previousMovementInput;
    // [SerializeField] private bool isFloorTouched = false;
    // private int numFloorsTouched = 0;
    public bool IsCoinTouched = false;
    // private Vector3 cameraEndPoint; // Direction from (0, 0, 0)
    // private Vector3 cameraDirection; // Direction from player

    // References
    public GameObject CurrentCoinCollectible;
	public GameObject currentPanel;

	[Header("Managers")]
	[SerializeField] private UIManager uiManager;
	[SerializeField] private MazeGameMechanics gameMechanics;

	// Others
	private bool shieldActive = false;
	

	private void Start()
	{
		// Get components
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		playerRenderer = gameObject.transform.GetChild(0).GetComponent<Renderer>();
		isRunningHash = Animator.StringToHash("isRunning");
		shield.SetActive(false);

		// // Set initial camera angle and starting point
		// cameraDirection = (Vector3.up * 4 + Vector3.back) * 4;
		// cameraEndPoint = transform.position + cameraDirection;
		// ViewCamera.transform.position = transform.position + cameraDirection;
		// ViewCamera.transform.LookAt(transform.position);

		// Others
		// numFloorsTouched = 0;

	}

    private void Update()
    {
        // // Handle isFloorTouched
		// if (numFloorsTouched > 0)
		// {
		// 	isFloorTouched = true;
		// } else
		// {
		// 	isFloorTouched = false;
		// }

		// Handle jumping input
		// if (Input.GetButtonDown("Jump") && isFloorTouched) isJump = true;
    }

    private void FixedUpdate()
	{
		if (!gameMechanics.IsGameplayActive)
		{
			return;
		}

		HandlePlayerMovement();
		// HandleCameraMovement();

		//Vector3 p = transform.position;
		//if (transform.position.y > 3)
		//{
		//	p.y = 3;
		//	transform.position = p;
		//}
    }

	public void HandleMove(InputAction.CallbackContext context)
	{
		previousMovementInput = context.ReadValue<Vector2>();
	}

	public void HandleJump(InputAction.CallbackContext context)
	{
		if (!context.performed) return;

		onJumpButtonPress();
	}

	private void HandlePlayerMovement()
	{
		if (!gameMechanics.IsGameplayActive)
		{
			return;
		}
        // Handle player movement with WASD
        // float horizontalInput = Input.GetAxis("Horizontal");
        // float verticalInput = Input.GetAxis("Vertical");
		float horizontalInput = previousMovementInput.x;
        float verticalInput = previousMovementInput.y;

        // Handle touch input
        if (Input.touchCount > 0)
        {
            // horizontalInput = joystick.Horizontal;
            // verticalInput = joystick.Vertical;
        }

        // If no touch input, just use keyboard's GetAxis() values
        Vector3 movementDirection = new Vector3(horizontalInput * Speed, rigidBody.velocity.y, verticalInput * Speed);
        rigidBody.velocity = movementDirection;

		// Handle animation
		movementDirection.y = 0;
		if (movementDirection == Vector3.zero)
		{
			animator.SetBool(isRunningHash, false);
		} else
		{
			animator.SetBool(isRunningHash, true);
		}

        // Handle player turning
        Vector3 newRotation = new Vector3(movementDirection.x, 0, movementDirection.z);
        if (newRotation != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(newRotation, Vector3.up);
            rigidBody.MoveRotation(toRotation);
        }
    }

	// private void HandleCameraMovement()
	// {
    //     if (ViewCamera == null)
    //     {
    //         return;
    //     }

    //     // Smooth camera moving
    //     // Check if the distance between the player and the camera is big enough to start moving
    //     if (Vector3.Distance(transform.position, ViewCamera.transform.position) > SLERP_DISTANCE_THRESHOLD)
    //     {
    //         // Start moving towards the player
    //         ViewCamera.transform.position = Vector3.Lerp(ViewCamera.transform.position, cameraEndPoint, 5 * Time.deltaTime);
    //     }
    //     cameraEndPoint = transform.position + cameraDirection;
    // }

	private void OnCollisionEnter(Collision coll)
	{
		// if (coll.gameObject.tag.Equals("MR-Floor"))
		// {
		// 	numFloorsTouched++;
		// 	// if (audioSource != null && HitSound != null && coll.relativeVelocity.y > .5f) {
		// 	// 	audioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
		// 	// }
		// } 

		if (coll.gameObject.tag.Equals("MR-Spike"))
		{
			if (shieldActive)
			{
				// De-activates the spike collider so you can pass through them
				coll.gameObject.GetComponent<Collider>().enabled = false;
				StartCoroutine(DeactivateSpike(coll.gameObject));
			}
			else
			{
				Debug.Log("player has touched spike");
				Instantiate(EnergyExplosion, transform.position, Quaternion.Euler(0, 0, 0));
				// uiManager.PlayRemoveTimeAnimation(10.0f);
				// uiManager.UpdateTimerUI(gameMechanics.TimeRemaining, gameMechanics.TimeRemaining - 10.0f);
				gameMechanics.RemoveTime(10.0f);
				StartCoroutine(TemporaryInvincibility());
			}
		}

		if (coll.gameObject.tag.Equals("MR-PatrolEnemy"))
		{
			if (shieldActive) return;
			Debug.Log("player has touched enemy");
			RandomTeleportPlayer();
			// Turns on the shield (invisibly) so that the player is temporarily invincible after teleporting 
			shieldActive = true;
			StartCoroutine(TemporaryInvincibility());
			// uiManager.PlayRemoveTimeAnimation(10.0f);
			// uiManager.UpdateTimerUI(gameMechanics.TimeRemaining, gameMechanics.TimeRemaining - 10.0f);
			gameMechanics.RemoveTime(10.0f);
		}
	}

	//Re-activates the colliders of the spikes after half a second
	private IEnumerator DeactivateSpike(GameObject spike)
	{
		yield return new WaitForSeconds(.5f);
		spike.GetComponent<Collider>().enabled = true;
	}

	//Makes the player invincible for half a second so they don't get damage or teleport again
	//if they teleport on top of a spike or a grandma  
	private IEnumerator TemporaryInvincibility()
	{
		var whenAreWeDone = Time.time + 1.5f;
		while(Time.time < whenAreWeDone){
			yield return new WaitForSeconds(0.1f);
			playerRenderer.enabled = !playerRenderer.enabled;
		}
		yield return new WaitForSeconds(0.1f);
		playerRenderer.enabled=true;
		shieldActive = false;
	}

	private void RandomTeleportPlayer()
	{
		int randomRow = Random.Range(0, MazeSpawner.Rows);
		int randomColumn = Random.Range(0, MazeSpawner.Columns);
		float x = randomColumn * (MazeSpawner.CellWidth + (MazeSpawner.AddGaps ? .2f : 0));
		float z = randomRow * (MazeSpawner.CellHeight + (MazeSpawner.AddGaps ? .2f : 0));
		Vector3 teleportPos = new Vector3(x, 4, z);
		transform.position = teleportPos;
		Physics.SyncTransforms();
	}

	private void OnCollisionExit(Collision coll)
	{
		// if (coll.gameObject.tag.Equals("MR-Floor"))
		// {
		// 	numFloorsTouched--;
		// }
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag.Equals("MR-Coin"))
		{
			if (!IsCoinTouched)
            {
				IsCoinTouched = true;
				CurrentCoinCollectible = other.gameObject;
				rigidBody.velocity = Vector3.zero;
				// if (audioSource != null && CoinSound != null)
				// {
				// 	audioSource.PlayOneShot(CoinSound);
				// }
				// Instantiate game panel
				gameMechanics.IsGameplayActive = false;
				Debug.Log("player has touched coin");
				gameMechanics.ShowQuestionPanel();
				// uiManager.InstantiateGamePanel();
				if (shield.activeInHierarchy)
				{
					shield.GetComponentInChildren<MazePowerupTimer>().PauseTimer();
				}
			}
		}
		if (other.gameObject.tag.Equals("MR-Powerup_Shield"))
		{
			Destroy(other.gameObject);
			ActivateShieldPowerup(10.0f);
		}
		if (other.gameObject.tag.Equals("MR-Powerup_Clock"))
		{
			Destroy(other.gameObject);
			// uiManager.PlayAddTimeAnimation(10.0f);
			// uiManager.UpdateTimerUI(gameMechanics.TimeRemaining, gameMechanics.TimeRemaining + 10.0f);
			gameMechanics.AddTime(10.0f);
		}
	}

	/// <summary>
	/// Upon collection of a shield powerup, creates a shield around the player and its associated timer
	/// </summary>
	/// <param name="durationSeconds">Duration of the powerup in seconds</param>
	public void ActivateShieldPowerup(float durationSeconds)
	{
		shieldActive = true;
		shield.SetActive(true);
		MazePowerupTimer powerupTimer = shield.GetComponentInChildren<MazePowerupTimer>();
		powerupTimer.RestartTimer(durationSeconds);
	}

	/// <summary>
	/// Upon expiration of a shield powerup, destroys the shield around the player
	/// </summary>
	public void DisableShieldPowerup()
	{
		shieldActive = false;
		shield.SetActive(false);
	}

	//If the shield was active when a coin was touched, restart it after the quiz ends.
	public void UnpauseShieldTimer()
	{
		if (shieldActive)
		{
			shield.GetComponentInChildren<MazePowerupTimer>().UnpauseTimer();
		}
	}

	/// <summary>
	/// Handles logic for making player jump, including adding force and detecting ground
	/// </summary>
	public void onJumpButtonPress()
	{
		if (!gameMechanics.IsGameplayActive) return;

		if (EventSystem.current.IsPointerOverGameObject(0)) return;
		
		if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, whatIsGround))
		{
			// we are currently grounded
			rigidBody.AddForce(Vector3.up * JumpAmount, ForceMode.Impulse);
		}

		// if (!isFloorTouched || !gameMechanics.IsGameplayActive)
		// {
		// 	return;
		// }
		// isFloorTouched = false;
        // rigidBody.AddForce(Vector3.up * JumpAmount, ForceMode.Impulse);
	}
}
