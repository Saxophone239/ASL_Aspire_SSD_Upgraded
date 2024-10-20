using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNObstacleAnimationScript : MonoBehaviour
{
    public Sprite sprite1;
    public Sprite sprite2;

    private SpriteRenderer sR;

    // Start is called before the first frame update
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        StartCoroutine(ChangeSprite());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * 1 * Time.deltaTime;
    }

    IEnumerator ChangeSprite()
    {
        sR.sprite = sprite1;

        yield return new WaitForSecondsRealtime(0.5f);

        sR.sprite = sprite2;

        yield return new WaitForSecondsRealtime(0.5f);

        StartCoroutine(ChangeSprite());
    }
}
