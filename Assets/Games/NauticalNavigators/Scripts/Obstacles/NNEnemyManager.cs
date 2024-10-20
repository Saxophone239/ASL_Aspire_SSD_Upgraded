using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNEnemyManager : MonoBehaviour
{
    [SerializeField] private NNSpawnManager sM;
    public GameObject enemyPrefab;

    private int currentNumberOfEnemies = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sM.GetCurrentWave() == 5 && currentNumberOfEnemies == 0)
        {
            GameObject tmp = Instantiate(enemyPrefab, new Vector3(6, 4.5f, 0), enemyPrefab.transform.rotation);
            tmp.transform.SetParent(transform);
            currentNumberOfEnemies++;
        }

        if (sM.GetCurrentWave() == 10 && currentNumberOfEnemies == 1)
        {
            GameObject tmp = Instantiate(enemyPrefab, new Vector3(6, -4.5f, 0), enemyPrefab.transform.rotation);
            tmp.transform.SetParent(transform);
            currentNumberOfEnemies++;
        }
    }
}
