using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNEnemy : MonoBehaviour
{
    private GameObject player;
    private NNSpawnManager sM;
    public GameObject projectilePrefab;

    public float SpawnRate = 1.5f;

    private Vector3 directionToPlayer;
    private Vector3 defaultPosition;
    private Vector3 startingPosition;
    private float slerpPercPos = 0.0f;
    private bool isStartingAnimationActive = false;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sM = GameObject.Find("Spawn Manager").GetComponent<NNSpawnManager>();

        defaultPosition = transform.position;
        startingPosition = defaultPosition + new Vector3(8, 0, 0);
        slerpPercPos = 0.0f;
        isStartingAnimationActive = true;

        StartCoroutine(FireProjectile());
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            directionToPlayer = player.transform.position - this.transform.position;
            this.transform.right = directionToPlayer;
        }

        if (isStartingAnimationActive)
        {
            // Save progress of animation/slerping
            slerpPercPos = Mathf.MoveTowards(slerpPercPos, 1f, Time.deltaTime * 2.0f);

            // Begin slerping
            transform.position = Vector3.Slerp(startingPosition, defaultPosition, slerpPercPos);

            // Check if slerping finished
            if (slerpPercPos >= 1f)
            {
                isStartingAnimationActive = false;
            }
        }
    }

    private IEnumerator FireProjectile()
    {
        if (sM.currentState == NNSpawnManager.SpawnState.Obstacles)
        {
            GameObject tmp = Instantiate(projectilePrefab, this.transform.position + this.transform.right * 0.75f, this.transform.rotation);
            Destroy(tmp, 5.0f);
        }
        
        yield return new WaitForSeconds(SpawnRate);

        StartCoroutine(FireProjectile());
    }
}
