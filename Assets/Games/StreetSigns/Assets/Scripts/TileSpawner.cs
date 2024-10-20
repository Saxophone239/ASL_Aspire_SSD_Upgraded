using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SSQuestionManager;

public class TileSpawner : MonoBehaviour
{
    /**
    * Overall, just remember that when spawning obstacle tiles, spawning is done randomly,
    * but when spawning question tiles, that is done sequentially and depends on the order
    * of the array.
    */

    [Header("Managers")]
    [SerializeField] private GameMechanics gameMechanics;
	[SerializeField] private SSQuestionManager qM;

    [Header("Parameters")]
    [SerializeField] private GameObject[] tilePrefabs;
    [SerializeField] private GameObject[] questionTilePrefabs;
	[SerializeField] private GameObject[] questionTileDefinitionPrefabs; // definition vids are long, so add some space
    [SerializeField] private GameObject[] menuTilePrefabs;
    [SerializeField] private GameObject[] powerupsPrefabs;
    [SerializeField] private Transform playerTransform;
    public int NumberOfTilesUntilQuestion; // Unchanged var of # of tiles to pass until question generates
    private int numberOfTilesTraversed; // Private counter counting number of tiles passed (useful for determining when to spawn question tiles)
    private bool isQuestionActive = false;
    private int questionTileCounter = 0; // Private counter counting number of tiles within question sequence passed
    [SerializeField] private float zSpawn = 0;
    public float TileLength;
    public int NumberOfCurrentlySpawnedTilesGameplay; // Unchanged var of the total # of tiles to spawn at a time
    private int numberOfCurrentlySpawnedTiles; // Private var which could change depending on whether IsMainMenu is active
    private List<GameObject> activeTiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // I want a tile to spawn behind the player so the main menu looks better
        zSpawn -= TileLength;

        if (gameMechanics.IsMainMenu)
        {
            // Spawn only a few tiles, enough to make the main menu look good
            numberOfCurrentlySpawnedTiles = 3;
            SpawnTile(menuTilePrefabs[0]);
            SpawnTile(menuTilePrefabs[0]);
            SpawnTile(menuTilePrefabs[0]);
        }
        else
        {
            // (this else statement might never be entered unless for dev purposes)
            // Spawn enough times for gameplay
            numberOfCurrentlySpawnedTiles = NumberOfCurrentlySpawnedTilesGameplay;
            SpawnTile(tilePrefabs[0]);
            SpawnTile(tilePrefabs[0]);

            for (int i = 0; i < NumberOfCurrentlySpawnedTilesGameplay; i++)
            {
                SpawnTile(tilePrefabs[Random.Range(0, tilePrefabs.Length)]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMechanics.IsMainMenu)
        {
            // Handle logic to continuously spawn main menu tiles
            if (playerTransform.position.z - TileLength > zSpawn - ((numberOfCurrentlySpawnedTiles - 1) * TileLength))
            {
                SpawnTile(menuTilePrefabs[Random.Range(0, menuTilePrefabs.Length)]);
                DeleteTile();
            }
        }
        else
        {
            // Check if we've passed enough tiles to generate a question
            if (numberOfTilesTraversed > NumberOfTilesUntilQuestion) isQuestionActive = true;

            // Handle logic to spawning the next tile (spawn once player reaches certain zSpawn point)
            if (playerTransform.position.z - TileLength > zSpawn - (numberOfCurrentlySpawnedTiles * TileLength))
            {
                SpawnCorrectTile();
                DeleteTile();
            }
        }
    }

    public void SpawnTile(GameObject tile)
    {
        GameObject tmp = Instantiate(tile, transform.forward * zSpawn, transform.rotation);
        tmp.transform.SetParent(transform);
        zSpawn += TileLength;
        activeTiles.Add(tmp);

        // Spawn Powerup Logic
        if (!isQuestionActive)
        {
            if (Random.Range(0, 15) == 1)
            {
                GameObject tmp2 = Instantiate(
                    powerupsPrefabs[Random.Range(0, powerupsPrefabs.Length)], 
                    transform.forward * zSpawn + new Vector3(0, 7, -10), 
                    transform.rotation
                    );
                tmp2.transform.SetParent(tmp.transform);
            }
        }
    }

    public void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    private void SpawnCorrectTile()
    {
        // First, check if question tile spawning is active
        if (!isQuestionActive)
        {
            // Question isn't active, spawn normal obstacle tiles
            SpawnTile(tilePrefabs[Random.Range(0, tilePrefabs.Length)]);
            numberOfTilesTraversed++;
        }
        else
        {
            // Question is active, spawn sequence of question tiles
			if (qM.SelectedQuestionType == SSQuestionType.EnglishDefinitionToEnglishWord)
			{
				// This is a definition question
				if (questionTileCounter < questionTileDefinitionPrefabs.Length)
				{
					// Check the sequence, if we haven't reached the end then spawn next tile (normal)
					SpawnTile(questionTileDefinitionPrefabs[questionTileCounter]);
					questionTileCounter++;
				}
				else
				{
					// We have reached the end of the question tile sequence, begin to spawn normal obstacle tiles
					questionTileCounter = 0;
					numberOfTilesTraversed = 0;
					isQuestionActive = false;
					SpawnTile(tilePrefabs[Random.Range(0, tilePrefabs.Length)]);
				}
			}
			else
			{
				// This is a normal question
				if (questionTileCounter < questionTilePrefabs.Length)
				{
					// Check the sequence, if we haven't reached the end then spawn next tile (normal)
					SpawnTile(questionTilePrefabs[questionTileCounter]);
					questionTileCounter++;
				}
				else
				{
					// We have reached the end of the question tile sequence, begin to spawn normal obstacle tiles
					questionTileCounter = 0;
					numberOfTilesTraversed = 0;
					isQuestionActive = false;
					SpawnTile(tilePrefabs[Random.Range(0, tilePrefabs.Length)]);
				}
			}
        }
    }

    public void GenerateGameplayTiles()
    {
        for (int i = numberOfCurrentlySpawnedTiles; i < NumberOfCurrentlySpawnedTilesGameplay; i++)
        {
            // For the first tile spawned spawn a default tile
            if (i == 0)
            {
                SpawnTile(tilePrefabs[0]);
                numberOfTilesTraversed++;
            }

            // Check if we've passed enough tiles to generate a question
            if (numberOfTilesTraversed > NumberOfTilesUntilQuestion) isQuestionActive = true;
            SpawnCorrectTile();
            Debug.Log("tile has been spawned");
        }
        numberOfCurrentlySpawnedTiles = NumberOfCurrentlySpawnedTilesGameplay;
    }
}
