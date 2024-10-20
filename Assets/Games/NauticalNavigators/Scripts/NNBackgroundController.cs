using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNBackgroundController : MonoBehaviour
{
    public float Speed;

    [SerializeField]
    private Renderer bgRenderer;
    [SerializeField]
    private NNGameManager gM;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("Game Manager").GetComponent<NNGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        bgRenderer.material.mainTextureOffset += new Vector2(Speed * Time.deltaTime, 0);
        if (gM.IsGameOver) Speed = 0;
    }
}
