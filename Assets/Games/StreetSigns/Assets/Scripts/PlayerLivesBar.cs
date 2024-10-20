using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLivesBar : MonoBehaviour
{
    public GameObject HeartPrefab;

    private PlayerController player;
    private GameMechanics gM;
    private List<GameObject> activeHearts = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
        gM = GameObject.FindObjectOfType<GameMechanics>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawLife()
    {
        GameObject tmp = Instantiate(HeartPrefab);
        tmp.transform.SetParent(transform);
        tmp.transform.localScale = Vector3.one;
        activeHearts.Add(tmp);
    }

    public void RemoveLife()
    {
        if (activeHearts.Count > 0)
        {
            Destroy(activeHearts[0]);
            activeHearts.RemoveAt(0);
        }
    }

    public void UpdateBar(int lives)
    {
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
