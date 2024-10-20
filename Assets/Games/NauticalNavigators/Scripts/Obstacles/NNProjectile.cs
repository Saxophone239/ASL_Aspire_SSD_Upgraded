using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNProjectile : MonoBehaviour
{
    public float Speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition += this.transform.right * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Destroy(gameObject);
        }
    }
}
