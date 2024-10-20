using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NNPlayerLivesBar : MonoBehaviour
{
    public GameObject HeartPrefab;
    public ParticleSystem HeartExplosionParticle;

    private NNPlayerController player;
    private NNGameManager gM;
    private Camera cam;
    private List<GameObject> activeHearts = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<NNPlayerController>();
        gM = GameObject.FindObjectOfType<NNGameManager>();
        cam = GameObject.FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DrawLife()
    {
        GameObject tmp = Instantiate(HeartPrefab);
        tmp.transform.SetParent(transform);
        tmp.transform.localScale = Vector3.one;
        activeHearts.Add(tmp);
    }

    private void RemoveLife()
    {
        if (activeHearts.Count > 0)
        {
            PlayRemoveHeartParticle(activeHearts[^1].GetComponent<Image>().rectTransform);
            Destroy(activeHearts[0]);
            activeHearts.RemoveAt(0);
        }
    }

    private void PlayRemoveHeartParticle(RectTransform rectTransform)
    {
        //RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, rectTransform.position, cam, out Vector3 worldPoint);
        //HeartExplosionParticle.transform.position = worldPoint;

        HeartExplosionParticle.transform.position = rectTransform.position;
        HeartExplosionParticle.Play();
    }

    public void UpdateBar(int lives)
    {
        if (lives < 0) return;
        if (lives > activeHearts.Count)
        {
            while (lives != activeHearts.Count) DrawLife();
        }
        else if (lives < activeHearts.Count)
        {
            while (lives != activeHearts.Count) RemoveLife();
        }
    }
}
