using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NNPlayerController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private GameObject meshModel;
    private NNGameManager gameManager;

    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private ParticleSystem gameOverExplosionParticle;
    [SerializeField] private ParticleSystem topGroundRubParticle;
    [SerializeField] private ParticleSystem bottomGroundRubParticle;
    [SerializeField] private ParticleSystem dollarSignParticle;

    public float FlyForce = 150.0f;
    public float GravityForce = -50.0f;
    public float MaxFlySpeed = 10.0f;

    private float origStartX = -4.5f;
    private float origStartRotZ = -90.0f;
    [SerializeField] private bool onTopGround = false;
    [SerializeField] private bool onBottomGround = false;

    private bool isInvincibilityActive = false;
    private float invincibilityDurationSeconds = 2.0f;
    private float invincibilityDeltaTime = 0.15f;

    private bool isFlying = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        meshModel = transform.Find("Model").gameObject;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<NNGameManager>();

        origStartX = playerRb.position.x;
        origStartRotZ = transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Manage request to make player fly
        isFlying = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0) || Input.touchCount > 0;
        
        // Manage rotations while player does/doesn't fly
        meshModel.transform.rotation = Quaternion.Euler(0, 0, playerRb.velocity.y * 2);

        // Manage ground rub particle effect
        if (onTopGround)
        {
            if (!topGroundRubParticle.isPlaying) topGroundRubParticle.Play();
        }
        else
        {
            if (!topGroundRubParticle.isStopped) topGroundRubParticle.Stop();
        }
        if (onBottomGround)
        {
            if (!bottomGroundRubParticle.isPlaying) bottomGroundRubParticle.Play();
        }
        else
        {
            if (!bottomGroundRubParticle.isStopped) bottomGroundRubParticle.Stop();
        }
    }

    private void FixedUpdate()
    {
        // Keep player x pos the same
        playerRb.position = new Vector2(origStartX, playerRb.position.y);

        // Handle flying
        if (isFlying)
        {
            playerRb.AddForce(Vector3.up * FlyForce);
        }
        
        // Custom gravity
        playerRb.AddForce(Vector3.up * GravityForce);

        // Limit player flying speed
        if (playerRb.velocity.y < -MaxFlySpeed)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -MaxFlySpeed);
        }
        if (playerRb.velocity.y > MaxFlySpeed)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, MaxFlySpeed);
        }
    }

	public void HandleFlyUp(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Canceled)
			isFlying = false;
		else
			isFlying = true;
	}

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("GroundTop")) onTopGround = true;
        if (other.gameObject.CompareTag("GroundBottom")) onBottomGround = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("GroundTop")) onTopGround = false;
        if (other.gameObject.CompareTag("GroundBottom")) onBottomGround = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (!isInvincibilityActive)
            {
                // Lose life
                gameManager.RemoveLives(1);

                // Play explosion
                Vector3 collisionPoint = other.ClosestPoint(transform.position);
                ParticleSystem tmp = Instantiate(explosionParticle, collisionPoint, explosionParticle.transform.rotation);
                tmp.Play();

                // Start invincibility
                if (!gameManager.IsGameOver) StartCoroutine(StartInvincibility());
            }
        }

        if (other.gameObject.CompareTag("ScoreTriggerPipe"))
        {
            gameManager.AddScore(1);
        }

        if (other.gameObject.CompareTag("ScoreTriggerASLTube"))
        {
            gameManager.AddScore(10);
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            ParticleSystem tmp = Instantiate(dollarSignParticle, collisionPoint, dollarSignParticle.transform.rotation);
            tmp.Play();
            Destroy(other.gameObject);
            Destroy(tmp.gameObject, 5.0f);
        }
    }

    private IEnumerator StartInvincibility()
    {
        isInvincibilityActive = true;

        Vector3 origScale = meshModel.transform.localScale;
        for (float i = 0; i < invincibilityDurationSeconds; i += invincibilityDeltaTime)
        {
            // Alternate between 0 and 1 scale to simulate flashing
            if (meshModel.transform.localScale == origScale)
            {
                ScaleModelTo(Vector3.zero);
            }
            else
            {
                ScaleModelTo(origScale);
            }
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }

        isInvincibilityActive = false;
        ScaleModelTo(origScale);
    }

    private void ScaleModelTo(Vector3 scale)
    {
        meshModel.transform.localScale = scale;
    }

    public void ExplodePlayer()
    {
        ParticleSystem tmp = Instantiate(gameOverExplosionParticle, transform.position, gameOverExplosionParticle.transform.rotation);
        tmp.Play();
        ScaleModelTo(Vector3.zero);
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
