using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SICIMainMenuAnimatorScript : MonoBehaviour
{
	[SerializeField] private SignItUIManager uiManager;

    public void SetUpGameOverScreen()
	{
		uiManager.SetUpGameOverScreen();
	}
}
