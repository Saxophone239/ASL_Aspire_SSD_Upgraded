using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameMechanics gameMechanics;
    [SerializeField] private SwipeManager swipeManager;
	[SerializeField] private SSQuestionManager questionManager;

    [Header("Player parameters")]
    public float ForwardSpeed = 4;
	public float MaxSpeed = 15;
    public int CurrentLane = 1; // 0, 1, and 2 are used
    public float LaneDistance = 2.75f;
    private Rigidbody rigidBody;
    private CapsuleCollider capCollider;
    private float distanceToGround;
    [SerializeField] private GameObject meshModel;
    [SerializeField] private Animator playerAnimator;
	[SerializeField] private ParticleSystem dustExplosion;
	[SerializeField] private ParticleSystem confettiExplosion;

    [SerializeField] private bool isPlayerGrounded;
    private Vector3 playerVelocity;
    private float jumpHeight = 3.0f;
    private float gravityValue = -9.81f;
    public float GravityScale = 4.0f;
    private Quaternion origRotation;

    // Booleans
    private bool isJumpCalled = false;
    private bool isJumping = false;
    private bool canJump = true;
    private bool isSlideCalled = false;
    private bool isSliding = false;
    private bool inNoLaneSwitchZone = false;
    private bool isInvincibilityActive = false;

    // Slide timers
    private float slideStartingTime = 1.0f;
    private float slideCurrentTime = 1.0f;
    private bool isSlideTimerActive = false;

    // Invincibility parameters
    private float invincibilityDurationSeconds = 1.5f;
    private float invincibilityDeltaTime = 0.1f;

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        capCollider = GetComponentInChildren<CapsuleCollider>();
        playerAnimator = GetComponentInChildren<Animator>();

        distanceToGround = capCollider.bounds.extents.y;
        origRotation = rigidBody.rotation;
    }

    // Update is called once per frame
    private void Update()
    {   
        if (!gameMechanics.IsGameOver)
        {
            // // Listeners to move left and right and change current lane
            // if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || swipeManager.swipeLeft) && !inNoLaneSwitchZone)
            // {
            //     if (CurrentLane > 0 && !gameMechanics.IsMainMenu)
            //     {
            //         CurrentLane--;
            //     }
            // }
            // if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || swipeManager.swipeRight) && !inNoLaneSwitchZone)
            // {
            //     if (CurrentLane < 2 && !gameMechanics.IsMainMenu)
            //     {
            //         CurrentLane++;
            //     }
            // }
            // if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || swipeManager.swipeUp) && isPlayerGrounded && canJump)
            // {
            //     isJumpCalled = true;
            // }

            // if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || swipeManager.swipeDown)
            // {
            //     isSlideCalled = true;
            // }

            // Increase score (based on z position)
            gameMechanics.UpdateScore((int) transform.position.z);

            // Increase player speed gradually with time
            ForwardSpeed += Time.deltaTime * 0.1f;
			ForwardSpeed = Mathf.Clamp(ForwardSpeed, ForwardSpeed, MaxSpeed);
        }
        else ForwardSpeed = 0.0f;
    }

    private void FixedUpdate()
    {
        // constant move character forward
        Vector3 currentVel = rigidBody.velocity;
        currentVel.z = ForwardSpeed;
        rigidBody.velocity = currentVel;

        // Calculate next position to travel to depending on current lane
        Vector3 currentTransform = transform.position;
        switch (CurrentLane)
        {
            case 0:
                currentTransform.x = -LaneDistance;
                break;
            case 1:
                currentTransform.x = 0f;
                break;
            case 2:
                currentTransform.x = LaneDistance;
                break;
        }
        rigidBody.MovePosition(Vector3.Slerp(transform.position, currentTransform, 0.4f));
        Physics.SyncTransforms();

        // Adds jump functionality to our player
        isPlayerGrounded = IsGrounded();

        if (isJumpCalled && isPlayerGrounded)
        {
            playerAnimator.SetBool("isJumping", true);
            isPlayerGrounded = false;
        }
        if (isJumpCalled)
        {
            rigidBody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -3.0f * gravityValue * GravityScale), ForceMode.Impulse);
            
            isJumping = true;
            isJumpCalled = false;

            // Check if player is sliding, then cancel sliding
            if (isSliding)
            {
                EndSlideAnimation();
            }
        }
        rigidBody.AddForce(Vector3.up * gravityValue * GravityScale); // Gravity

        if (isPlayerGrounded && isJumping)
        {
            playerAnimator.SetBool("isJumping", false);
            isJumping = false;
        }
        
        // Adds slide functionality to our player
        if (isSlideCalled)
        {
            rigidBody.AddForce(Vector3.up * jumpHeight * gravityValue * GravityScale, ForceMode.Impulse);
            slideCurrentTime = slideStartingTime;
            isSlideTimerActive = true;
            if (!isSliding)
            {
                StartSlideAnimation();
            }
            isSlideCalled = false;
        }

        // Manage slide timer
        if (isSlideTimerActive)
        {
            if (slideCurrentTime > 0) slideCurrentTime -= Time.fixedDeltaTime;
            else
            {
                EndSlideAnimation();
                isSlideTimerActive = false;
            }
        }
    }

	public void HandleSwipeLeft(InputAction.CallbackContext context)
	{
		if (!context.performed) return;

		if (gameMechanics.IsGameOver || inNoLaneSwitchZone) return;

		if (CurrentLane > 0 && !gameMechanics.IsMainMenu)
		{
			CurrentLane--;
		}
	}

	public void HandleSwipeRight(InputAction.CallbackContext context)
	{
		if (!context.performed) return;

		if (gameMechanics.IsGameOver || inNoLaneSwitchZone) return;

		if (CurrentLane < 2 && !gameMechanics.IsMainMenu)
		{
			CurrentLane++;
		}
	}

	public void HandleSwipeUp(InputAction.CallbackContext context)
	{
		if (!context.performed) return;

		if (gameMechanics.IsGameOver || !isPlayerGrounded || !canJump) return;

		isJumpCalled = true;
	}

	public void HandleSwipeDown(InputAction.CallbackContext context)
	{
		if (!context.performed) return;

		if (gameMechanics.IsGameOver) return;

		isSlideCalled = true;
	}

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("SS-Obstacle-Untouchable"))
        {
			Debug.Log($"hit an obstacle of type {other.gameObject.name}");
			Collider[] colliders = other.gameObject.transform.parent.gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider coll in colliders)
			{
				coll.enabled = false;
			}
            if (!isInvincibilityActive)
            {
                gameMechanics.LoseLife();
				Instantiate(dustExplosion, other.GetContact(0).point, Quaternion.identity);
                if (gameMechanics.Lives > 0)
                {
                    StartCoroutine(StartInvincibility());
                }
            }
        }
    }

    private void StartSlideAnimation()
    {
        isSliding = true;
        playerAnimator.SetBool("isSliding", true);
        rigidBody.rotation = Quaternion.Euler(-90, 0, 0);
    }
    private void EndSlideAnimation()
    {
        rigidBody.MoveRotation(origRotation);
        playerAnimator.SetBool("isSliding", false);
        isSliding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("SS-NoLaneSwitchZone"))
        {
            Debug.Log("Entering no lane switch zone");
            inNoLaneSwitchZone = true;
        }
        if (other.gameObject.tag.Equals("SS-WrongAnswerTrigger"))
        {
            Debug.Log("went into wrong tunnel, losing life");
            gameMechanics.LoseLife();
			Instantiate(dustExplosion, transform.position, Quaternion.identity);
        }
        if (other.gameObject.tag.Equals("SS-RightAnswerTrigger"))
        {
            Debug.Log("went into right tunnel!");
			Instantiate(confettiExplosion, transform.position, Quaternion.identity);
        }
        if (other.gameObject.tag.Equals("SS-HealthPowerup"))
        {
            Debug.Log("collected health powerup");
            gameMechanics.AddLife();
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag.Equals("SS-NoJumpZone"))
        {
            Debug.Log("Entering no jump zone");
            canJump = false;
        }
		if (other.gameObject.tag.Equals("SS-TriggerUIQuestion"))
		{
			Debug.Log("Toggling show question UI panel");
			questionManager.HandleToggleShowQuestion();
		}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("SS-NoLaneSwitchZone"))
        {
            Debug.Log("Exiting no lane switch zone");
            inNoLaneSwitchZone = false;
        }
        if (other.gameObject.tag.Equals("SS-NoJumpZone"))
        {
            Debug.Log("Exiting no jump zone");
            canJump = true;
        }
    }

    private IEnumerator StartInvincibility()
    {
        Debug.Log("player became invincible!");
        isInvincibilityActive = true;

        for (float i = 0; i < invincibilityDurationSeconds; i += invincibilityDeltaTime)
        {
            // Alternate between 0 and 1 scale to simulate flashing
            if (meshModel.transform.localScale == Vector3.one)
            {
                ScaleModelTo(Vector3.zero);
            }
            else
            {
                ScaleModelTo(Vector3.one);
            }
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }

        isInvincibilityActive = false;
        ScaleModelTo(Vector3.one);
    }

    private void ScaleModelTo(Vector3 scale)
    {
        meshModel.transform.localScale = scale;
    }

    public IEnumerator StartDeathCoroutine()
    {
        playerAnimator.SetTrigger("isDead");
        yield return new WaitForSeconds(2.2f);
        gameMechanics.ShowGameOverScreen();
    }
}
