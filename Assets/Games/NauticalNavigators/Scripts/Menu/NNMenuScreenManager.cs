using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NNMenuScreenManager : MonoBehaviour
{
    [SerializeField] private NNSpawnManager sM;
    [SerializeField] private GameObject menuPanel;
    public NNGameManager nnGameManager;
    // Start is called before the first frame update
    void Start()
    {
        sM.currentState = NNSpawnManager.SpawnState.Off;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButtonClick()
    {
        Debug.Log("Clicked play button");
        sM.currentState = NNSpawnManager.SpawnState.Obstacles;
        menuPanel.GetComponent<NNMenuAnimationManager>().DisableMenuScreen();
        menuPanel.SetActive(false);

    }

    public void OnTutorialButtonClick()
    {
        Debug.Log("Clicked tutorial button");
    }

    public void OnBackToArcadeButtonClick()
    {
        Debug.Log("Clicked back to arcade button");
        UpdateGlobalCoins(true);
        // GlobalManager.currentLesson.game_completed = true;
        Debug.Log("Coins updated!");
        SceneManager.LoadScene("Arcade");

    }

    public void UpdateGlobalCoins(bool gameFinished = true){
        int newCoins = nnGameManager.Score/10; 
        // GlobalManager.coinsRecentlyAdded += newCoins;
        // if (gameFinished){
        //     GlobalManager.student.coins += GlobalManager.coinsRecentlyAdded;

        // }
    }

    
}
