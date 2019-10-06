using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuildingUI : MonoBehaviour
{
    public GameObject startFightButton;

    public CreateCardUI createCardUI;
    public ImprovingCardUI improvingCardUI;

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
        PlayGrid.Instance.Clear();
        GlobalGameManager.Instance.deckBuildingManager.editedCard.Close();
        GlobalGameManager.Instance.StartFight();
    }
}
