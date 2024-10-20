// using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SignItGlobals : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
	
    public static float spawnRate = 0.9f;
    public static Difficulty difficulty = Difficulty.Easy;
}
