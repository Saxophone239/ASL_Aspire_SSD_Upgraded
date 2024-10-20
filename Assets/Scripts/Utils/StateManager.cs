using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private RectTransform loginState;
    [SerializeField] private RectTransform mapState;
    [SerializeField] private RectTransform arcadeState;
    private RectTransform currentState;
    private Dictionary<MenuState, RectTransform> stateDictionary;

    private void Start()
    {
        currentState = loginState;
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        stateDictionary = new Dictionary<MenuState, RectTransform>();
        stateDictionary.Add(MenuState.Login, loginState);
        stateDictionary.Add(MenuState.Map, mapState);
        stateDictionary.Add(MenuState.Arcade, arcadeState);
    }

    public void ChangeState(MenuState desiredState)
    {
        // MenuState desiredState = (MenuState)stateNum;
        if (stateDictionary[desiredState] == currentState) return;
        stateDictionary[desiredState].gameObject.SetActive(true);
        currentState.gameObject.SetActive(false);
        currentState = stateDictionary[desiredState];
    }

    public void TestFunction()
    {
        Debug.Log("Hey there?");
    }
}
