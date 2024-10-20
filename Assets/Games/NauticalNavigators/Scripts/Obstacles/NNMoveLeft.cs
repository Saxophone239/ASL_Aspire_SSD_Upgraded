using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNMoveLeft : MonoBehaviour
{
    private NNGameManager gM;

    public static float LeftSpeed = 5.0f;
    public float DestroyPosX = -15.0f;

    private void Start()
    {
        gM = GameObject.Find("Game Manager").GetComponent<NNGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the object left
        this.transform.position += Vector3.left * LeftSpeed * Time.deltaTime;

        // Destroy the object if it leaves the scene
        if (this.transform.position.x <= DestroyPosX) Destroy(gameObject);

        // Gradually increase speed
        LeftSpeed += Time.deltaTime / 100;

        // Stop moving if game over
        if (gM.IsGameOver) LeftSpeed = 0;
    }
}
