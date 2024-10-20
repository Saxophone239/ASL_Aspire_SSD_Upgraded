using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class BasketController : MonoBehaviour
{
	private float previousMovementInput;
	private bool isGrounded;

	[Header("Settings")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float maximumVelocity = 5f;
    [SerializeField] private float jumpAmount = 6f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float fallingGravityScale = 2f;

    // Managers
    [Header("Managers")]
    [SerializeField] private Spawner spawner;
	[SerializeField] private SignItGameManager gameManager;
    [SerializeField] private PowerupTimer powerupTimer;
	[SerializeField] private PhysicsMaterial2D material;

    private Rigidbody2D rb;
	private Animator basketAnimator;
	private SpriteRenderer playerRenderer;
	private bool isPlayerInvincible;

    // Powerup tags
    private string lightningTag = "SICI-lightning_bolt";
    private string stopwatchTag = "SICI-stopwatch";
    private string burgerTag = "SICI-burger";
    private string multiplierTag = "SICI-multiplication";

    // Booleans for powerups
    private bool isLightning = false;
    private bool isStopwatch = false;
    private bool isBurger = false;
    private bool isMultiplier = false;

    // Sprite for tracking last powerup collected
    private Sprite lastPowerupSprite;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
		basketAnimator = GetComponent<Animator>();
		playerRenderer = GetComponent<SpriteRenderer>();
    }

	private void Update()
	{
		// Handle falling jump logic
        if (rb.velocity.y >= 0)
        {
            rb.gravityScale = gravityScale;
        }
        else if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallingGravityScale;
        }
	}

	// FixedUpdate is called 50 times every second and is useful for physics stuff
    private void FixedUpdate()
    {
		// Handle max speed if speed is less than max value or if input pushes player to opposite direction
        if (math.abs(rb.velocity.x) < maximumVelocity || rb.velocity.x * previousMovementInput < 0)
        {
            rb.AddForce(Vector2.right * previousMovementInput * movementSpeed, ForceMode2D.Force);
        }
    }

	public void HandleMove(InputAction.CallbackContext context)
	{
		previousMovementInput = context.ReadValue<float>();
	}

	public void HandleJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "SICI-ground")
        {
            if (rb.velocity.y <= 0)
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "SICI-ground")
        {
            isGrounded = false;
        }
    }

    public int AddScore()
    {
        int newScore = gameManager.AddScore();
        return newScore;
    }

    public int LoseLife()
    {
        int newLives = gameManager.LoseLife();
        return newLives;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject colliderGameObject = collision.gameObject;
        string tag = collision.tag;
        if (tag.Equals("SICI-word_greyed"))
        {
            return;
        }

        Destroy(collision.gameObject);
        if (!tag.Equals("SICI-word"))
        {
            // We have collected a powerup
			Debug.Log("powerup collected");
            lastPowerupSprite = collision.gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
			basketAnimator.SetTrigger("CollectedRight");
            if (tag.Equals(lightningTag))
            {
                StartCoroutine(LightningBoltPowerup(15, 0.05f, 20f, 10));
            }
            else if (tag.Equals(stopwatchTag))
            {
                StartCoroutine(StopwatchPowerup(0.05f, 10));
            }
            else if (tag.Equals(burgerTag))
            {
                StartCoroutine(BurgerPowerup(1.5f, 10));
            }
            else if (tag.Equals(multiplierTag))
            {
                StartCoroutine(MultiplicationPowerup(2, 10));
            }
        }
        else
        {
			// We have collected a word
			Debug.Log("word collected");
            TextMeshPro textMeshPro = colliderGameObject.GetComponent<TextMeshPro>();
            if (textMeshPro == null)
            {
                throw new System.Exception("Falling word does not contain a TextMeshPro element.");
            }

            if (spawner == null)
            {
                throw new System.Exception("Ensure a spawner GameObject exists and is linked to this script.");
            }

            string textWord = textMeshPro.text;

            if (string.Equals(textWord, spawner.CorrectWord))
            {
                // Player caught the correct word, increase score
				Debug.Log("word is correct");
                AddScore();
				basketAnimator.SetTrigger("CollectedRight");

                // Choose the next word to catch
                spawner.ChangeCorrectWord();

                // Gray out the current words on the screen so players don't accidentally catch them
                GameObject[] currentWords = GameObject.FindGameObjectsWithTag("SICI-word");
                foreach (GameObject word in currentWords)
                {
                    TextMeshPro wordTextMeshPro = word.GetComponent<TextMeshPro>();
                    wordTextMeshPro.color = Color.gray;
                    word.tag = "SICI-word_greyed";
                    SpriteRenderer sprite = word.GetComponentInChildren<SpriteRenderer>();
                    sprite.color = new Color32(255, 255, 255, 36);
                    //CircleCollider2D circleCollider = word.GetComponent<CircleCollider2D>();
                    //circleCollider.enabled = false;
                }
            }
            else
            {
				if (isPlayerInvincible) return;
                // Player caught the wrong word, decrease lives
				Debug.Log("word is incorrect");
                LoseLife();
				basketAnimator.SetTrigger("CollectedWrong");
				StartCoroutine(TemporaryInvincibility());
            }
        }
    }

    IEnumerator LightningBoltPowerup(float newMaxSpeed, float newFriction, float newMovementSpeed, float duration)
    {
        if (isLightning)
        {
            yield break;
        }
        isLightning = true;

        float oldMaxSpeed = maximumVelocity;
		float oldFriction = material.friction;
		float oldMovementSpeed = movementSpeed;
        maximumVelocity = newMaxSpeed;
		material.friction = newFriction;
		movementSpeed = newMovementSpeed;

        powerupTimer.RestartTimer(duration, lastPowerupSprite);
        yield return new WaitForSeconds(duration);

        maximumVelocity = oldMaxSpeed;
        material.friction = oldFriction;
		movementSpeed = oldMovementSpeed;

        isLightning = false;
    }

    IEnumerator StopwatchPowerup(float newFallingSpeed, float duration)
    {
        if (isStopwatch)
        {
            yield break;
        }
        isStopwatch = true;

        float oldFallingSpeed = spawner.fallingSpeed;
        spawner.fallingSpeed = newFallingSpeed;
        GameObject[] currentWords = GameObject.FindGameObjectsWithTag("SICI-word");
        foreach (GameObject word in currentWords)
        {
            Rigidbody2D rigidBody = word.GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = newFallingSpeed;
        }

        powerupTimer.RestartTimer(duration, lastPowerupSprite);
        yield return new WaitForSeconds(duration);

        spawner.fallingSpeed = oldFallingSpeed;
        currentWords = GameObject.FindGameObjectsWithTag("SICI-word");
        foreach (GameObject word in currentWords)
        {
            Rigidbody2D rigidBody = word.GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = oldFallingSpeed;
        }

        isStopwatch = false;
    }

    IEnumerator BurgerPowerup(float scaleFactor, float duration)
    {
        if (isBurger)
        {
            yield break;
        }
        isBurger = true;

        Vector3 oldTransform = transform.localScale;
        transform.localScale = transform.localScale * scaleFactor;

        powerupTimer.RestartTimer(duration, lastPowerupSprite);
        yield return new WaitForSeconds(duration);

        transform.localScale = oldTransform;

        isBurger = false;
    }

    IEnumerator MultiplicationPowerup(int multiplier, float duration)
    {
        if (isMultiplier)
        {
            yield break;
        }
        isMultiplier = true;

        int oldValue = SignItGameManager.ScoreIncrementValue;
		SignItGameManager.ScoreIncrementValue = SignItGameManager.ScoreIncrementValue * multiplier;

        powerupTimer.RestartTimer(duration, lastPowerupSprite);
        yield return new WaitForSeconds(duration);

        SignItGameManager.ScoreIncrementValue = oldValue;

        isMultiplier = false;
    }

	//Makes the player invincible for a second so they can't get damage
	private IEnumerator TemporaryInvincibility()
	{
		isPlayerInvincible = true;

		var whenAreWeDone = Time.time + 1.5f;
		while (Time.time < whenAreWeDone)
		{
			yield return new WaitForSeconds(0.1f);
			playerRenderer.enabled = !playerRenderer.enabled;
		}
		yield return new WaitForSeconds(0.1f);
		playerRenderer.enabled=true;
		
		isPlayerInvincible = false;
	}
}
