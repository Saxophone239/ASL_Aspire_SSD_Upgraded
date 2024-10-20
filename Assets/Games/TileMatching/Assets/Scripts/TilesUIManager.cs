using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TilesUIManager : MonoBehaviour
{


    public void QuitGame(){
        UpdateCoins();
        SceneManager.LoadScene("Arcade");
    }

    public void UpdateCoins(){
        int numCoins = TileBehavior.score/10;
		// TODO: uncomment below line
        // GlobalManager.student.coins += numCoins;
    }

}
