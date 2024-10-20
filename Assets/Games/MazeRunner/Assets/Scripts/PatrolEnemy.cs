using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// [RequireComponent(typeof(NavMeshAgent))]
public class PatrolEnemy : MonoBehaviour
{
	[Header("References")]
    [SerializeField] private NavMeshAgent enemyAgent;
	[SerializeField] private Animator enemyAnimator;
	[SerializeField] private GameObject alertCanvas;
    // [SerializeField] private Player player;
	public MRPlayer player;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    // Patrolling
    public Vector3 walkPoint;
    [SerializeField] private bool isWalkPointSet;
    public float walkPointRange;


    // States
    public float renderRange; // Enemy only moves if player can see it, done for performance purposes
    public bool isPlayerInRenderRange;
    public float sightRange;
    public bool isPlayerInSightRange;

	[Header("Debugging Variables")]
	[SerializeField] private float enemySpeed;
	[SerializeField] private Vector3 enemyVelocity;
	[SerializeField] private Vector3 enemyDesiredVelocity;
	[SerializeField] private Vector3[] pathCorners;
	[SerializeField] private NavMeshPathStatus pathStatus;
	[SerializeField] private bool isChasingPlayer;
	[SerializeField] private bool isPatrolling;

    // Start is called before the first frame update
    private void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(Delay());
    }

	private void Update()
	{
		if (player == null)
		{
			// Player is not yet found, find player
			if (GameObject.FindGameObjectWithTag("Player").TryGetComponent<MRPlayer>(out MRPlayer playerComponent))
			{
				player = playerComponent;
			}
			return;
		}
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
        isPlayerInRenderRange = Physics.CheckSphere(transform.position, renderRange, playerLayer);
        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);

        if (isPlayerInRenderRange)
		{
            if (isPlayerInSightRange)
			{
                // If the coin is touched but the enemy hasn't stopped
                if (!enemyAgent.isStopped && player.IsCoinTouched)
				{
                    enemyAgent.isStopped = true;
                }
				else if (enemyAgent.isStopped && !player.IsCoinTouched)
				{
                    enemyAgent.isStopped = false;
                }
				else
				{
					// Once enemy sees player, they'll keep chasing until player runs out of range
					if (isChasingPlayer)
					{
						Debug.Log("I saw player already, I'm gonna keep chasing them");
						ChasePlayer();
					}
					else
					{
						// Check if enemy can see player
						Vector3 directionEnemyToPlayer = player.transform.position - transform.position;
						if (Physics.Raycast(transform.position, directionEnemyToPlayer, out RaycastHit hit, 8.0f))
						{
							if (hit.collider.tag.Equals("MR-Player"))
							{
								// Enemy can see player
								Debug.Log("Player is in my range and I can see them");
								Debug.DrawRay(transform.position, directionEnemyToPlayer * hit.distance, Color.red);
								ChasePlayer();
							}
							else
							{
								// Enemy can't see player
								Debug.Log("Player is in my range but I can't see them");
								Debug.DrawRay(transform.position, directionEnemyToPlayer * 8.0f, Color.yellow);
								Patrol();
							}
						}
						else
						{
							Debug.Log("Player is in my range but I can't see them");
							Patrol();
						}
					}
					
                }
            }
			else
			{
				// Player is not within range of sight
                Patrol();
            }
        }

		if (enemyAgent.velocity.magnitude >= 1f)
		{
			enemyAnimator.SetBool("isJogging", true);
		}
		else
		{
			enemyAnimator.SetBool("isJogging", false);
		}

		enemySpeed = enemyAgent.speed;
		enemyVelocity = enemyAgent.velocity;
		enemyDesiredVelocity = enemyAgent.desiredVelocity;
		pathCorners = enemyAgent.path.corners;
		// pathStatus = enemyAgent.path.status;
    }

	// Called everytime FixedUpdate() is called
    private void Patrol()
    {
        if (!isWalkPointSet) SearchWalkPoint();
        else
		{
			isChasingPlayer = false;
			isPatrolling = true;
			enemyAgent.SetDestination(walkPoint);
			pathStatus = enemyAgent.path.status;
			if (alertCanvas.activeSelf) alertCanvas.SetActive(false);
		}

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 3f || pathStatus != NavMeshPathStatus.PathComplete)
        {
            isWalkPointSet = false;
        }
        
    }

    private void SearchWalkPoint()
    {
        // Collect random point in range
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

		walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
		
		if (Physics.Raycast(walkPoint, -transform.up, 2.0f, groundLayer) && IsPathAchievable(walkPoint))
		{
			// if (NavMesh.SamplePosition(walkPoint, out NavMeshHit hit, 4.0f, NavMesh.AllAreas))
			// {
			// 	walkPoint = hit.position;
			// 	isWalkPointSet = true;
			// }

			// pathStatus = NavMeshPathStatus.PathComplete;
			isWalkPointSet = true;
		}
    }

    private void ChasePlayer()
    {
		if (IsPathAchievable(player.gameObject.transform.position))
		{
			isChasingPlayer = true;
			isPatrolling = false;
			enemyAgent.SetDestination(player.gameObject.transform.position);
			pathStatus = enemyAgent.path.status;
			if (!alertCanvas.activeSelf) alertCanvas.SetActive(true);
		}
    }

	private bool IsPathAchievable(Vector3 walkpoint)
	{
		var path = new NavMeshPath();
		enemyAgent.CalculatePath(walkpoint, path);
		if (path.status != NavMeshPathStatus.PathComplete)
		{
			return false;
		}
		return true;
	}

    private IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(3.0f);
    }

	private void OnDrawGizmos()
	{
		if (isChasingPlayer) Gizmos.color = Color.red;
		else Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, sightRange);

		Gizmos.color = Color.blue;
		if (pathCorners.Length >= 1)
		{
			Gizmos.DrawLine(transform.position, pathCorners[0]);
		}
		for (int i = 0; i < pathCorners.Length - 1; i++)
		{
			Gizmos.DrawLine(pathCorners[i], pathCorners[i+1]);
		}
	}
}
