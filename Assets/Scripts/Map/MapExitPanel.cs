using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapExitPanel : MonoBehaviour
{
	public void OnExitMapButtonClick()
	{
		PlayfabPostManager.Instance.PostAllLoginSessions(GlobalManager.Instance.allLoginSessions);
		StartCoroutine(LoadYourSceneAsync("LoginScene"));
	}
    
	private IEnumerator LoadYourSceneAsync(string sceneName)
	{
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
	}
}
