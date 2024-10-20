using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SICIPlayerLivesBar : MonoBehaviour
{
	[SerializeField] private SignItGameManager gameManager;
    [SerializeField] private GameObject HeartPrefab;
	[SerializeField] private GameObject HeartExplosionParticleDisplay;
    [SerializeField] private ParticleSystem HeartExplosionParticle;

    private Camera cam;
    private List<GameObject> activeHearts = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        cam = GameObject.FindObjectOfType<Camera>();

		gameManager.OnLivesUpdate += UpdateBar;
		gameManager.OnGameActivated += OnGameActivation;
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
		
        HeartExplosionParticleDisplay.transform.position = rectTransform.position;
        HeartExplosionParticle.Play();
    }

    public void UpdateBar(int oldLives, int newLives)
    {
        if (newLives < 0) return;
        if (newLives > activeHearts.Count)
        {
            while (newLives != activeHearts.Count) DrawLife();
        }
        else if (newLives < activeHearts.Count)
        {
            while (newLives != activeHearts.Count) RemoveLife();
        }
    }

	private void OnGameActivation()
	{
		// Initialize bar with correct number of lives
		while (activeHearts.Count != gameManager.InitialLives)
		{
			DrawLife();
		}
	}

	private void OnDestroy()
	{
		gameManager.OnLivesUpdate -= UpdateBar;
		gameManager.OnGameActivated -= OnGameActivation;
	}
}
