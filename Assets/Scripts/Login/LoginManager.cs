using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class LoginManager : MonoBehaviour
{
    [SerializeField] TMP_InputField idInput;
    [SerializeField] TextMeshProUGUI failText;
	[SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] Button loginButton;
	[SerializeField] GameObject loadingWheel;

    // private StateManager stateManager;

    // Start is called before the first frame update
    void Start()
    {
        // stateManager = FindObjectOfType<StateManager>();

		loadingWheel.SetActive(false);
		loadingText.gameObject.SetActive(false);

		idInput.onValueChanged.AddListener(delegate {OnInputChanged();});
    }

	private void Update()
	{
		if (Keyboard.current[Key.Enter].wasPressedThisFrame) Login();
	}

	private void OnInputChanged()
	{
		if (failText.IsActive()) failText.gameObject.SetActive(false);
	}

    public void Login()
    {
		if (!idInput.text.Equals(string.Empty))
		{
			StudentLoginActivate(idInput.text);
		}
		else
		{
			LoginFail();
		}
    }

    private void LoginFail()
    {
        failText.gameObject.SetActive(true);
    }

	public void StudentLoginActivate(string customID)
	{
		loadingWheel.SetActive(true);
        var request = new LoginWithCustomIDRequest {
        	CustomId = customID
        };
        PlayFabClientAPI.LoginWithCustomID(request, StudentOnLoginSuccess, OnError);
        
        Debug.Log("Login sent");
    }


    void StudentOnLoginSuccess(LoginResult result) {
        Debug.Log("Login success!");
		// stateManager.ChangeState(MenuState.Map);
		loadingText.gameObject.SetActive(true);
		StartCoroutine(LoadYourSceneAsync("Bootstrap"));
    }

    void OnError(PlayFabError error) {
        Debug.Log(error.ErrorMessage);
		loadingWheel.SetActive(false);
		LoginFail();
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

	private void OnDestroy()
	{
		idInput.onValueChanged.RemoveAllListeners();
	}
}
