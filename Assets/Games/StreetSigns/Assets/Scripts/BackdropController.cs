using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackdropController : MonoBehaviour
{
    private PlayerController player;
    public Camera cam;
    public float DistanceAwayFromPlayer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z + DistanceAwayFromPlayer);
        //transform.position = new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z + DistanceAwayFromPlayer);
    }
}
