using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNDevTools : MonoBehaviour
{
    private NNGameManager gM;
    private NNSpawnManager sM;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("Game Manager").GetComponent<NNGameManager>();
        sM = GameObject.Find("Spawn Manager").GetComponent<NNSpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            gM.RemoveLives(1);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            gM.RemoveLives(-1);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            sM.currentState = NNSpawnManager.SpawnState.Question;
        }
    }
}
