using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuildingUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GlobalGameManager.OnNewGameState += OnNewGameState;
    }

    private void OnDestroy()
    {
        GlobalGameManager.OnNewGameState -= OnNewGameState;
    }

    void OnNewGameState(GlobalGameManager.GameState previousGameState, GlobalGameManager.GameState newGameState)
    {
        gameObject.SetActive(newGameState == GlobalGameManager.GameState.Deckbuilding);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartFightButton()
    {
        GlobalGameManager.Instance.StartFight();
    }
}
