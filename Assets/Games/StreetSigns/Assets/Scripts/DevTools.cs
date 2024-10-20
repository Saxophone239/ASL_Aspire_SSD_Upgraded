using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTools : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameMechanics gM;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
        gM = GameObject.FindObjectOfType<GameMechanics>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            gM.LoseLife();
        }
    }
}
