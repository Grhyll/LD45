using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightPanels : MonoBehaviour
{
    public GameObject endTurnButton;

    private void Awake()
    {
        GlobalGameManager.OnNewGameState += OnNewGameState;
        FightManager.OnNewFightState += OnNewFightState;
    }

    private void OnDestroy()
    {
        GlobalGameManager.OnNewGameState -= OnNewGameState;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnNewGameState(GlobalGameManager.GameState previousGameState, GlobalGameManager.GameState newGameState)
    {
        gameObject.SetActive(newGameState == GlobalGameManager.GameState.Fight);
    }

    void OnNewFightState(FightManager.FightState newFightState)
    {
        endTurnButton.SetActive(newFightState == FightManager.FightState.PlayerTurnOperations);
    }

    public void OnEndTurnButton()
    {
        FightManager.instance.OnEndPlayerOperations();
    }
}
