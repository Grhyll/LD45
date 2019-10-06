using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FightPanels : MonoBehaviour
{
    public GameObject endTurnButton;

    public GameObject selectedCardDescription;
    public TextMeshProUGUI selectedCardDescriptionText;
    public UnityEngine.UI.Image selectedCardImage;

    bool canShowEndTurn = false;

    FightCreature currentlyShownFightCreature;

    private void Awake()
    {
        GlobalGameManager.OnNewGameState += OnNewGameState;
        FightManager.OnNewFightState += OnNewFightState;
        TutoManager.OnTutoStepStart += OnTutoStepStart;
    }

    private void OnDestroy()
    {
        GlobalGameManager.OnNewGameState -= OnNewGameState;
        FightManager.OnNewFightState -= OnNewFightState;
        TutoManager.OnTutoStepStart -= OnTutoStepStart;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // This is horrible
        UpdateSelectedCardDescription();
    }

    void OnNewGameState(GlobalGameManager.GameState previousGameState, GlobalGameManager.GameState newGameState)
    {
        gameObject.SetActive(newGameState == GlobalGameManager.GameState.Fight);
        selectedCardDescription.gameObject.SetActive(false);
        currentlyShownFightCreature = null;
    }

    void OnNewFightState(FightManager.FightState newFightState)
    {
        endTurnButton.SetActive(canShowEndTurn && newFightState == FightManager.FightState.PlayerTurnOperations);
    }

    public void OnSelectedBoardEntity(FightCreature fightCreature)
    {
        selectedCardDescription.gameObject.SetActive(true);
        currentlyShownFightCreature = fightCreature;
        selectedCardImage.sprite = fightCreature.card.cardDefinition.sprite;
        UpdateSelectedCardDescription();
    }
    public void OnUnselectedBoardEntity()
    {
        selectedCardDescription.gameObject.SetActive(false);
    }
    void UpdateSelectedCardDescription()
    {
        if (selectedCardDescription.activeSelf && currentlyShownFightCreature != null)
        {
            string description = currentlyShownFightCreature.card.cardDefinition.displayName
                + "\n" + currentlyShownFightCreature.card.cardDefinition.description 
                + "\nAttack: " + currentlyShownFightCreature.damage
                + "\nHealth: " + currentlyShownFightCreature.health
                + "\nRange: " + currentlyShownFightCreature.range
                + "\nMoves: " + currentlyShownFightCreature.moves;
            selectedCardDescriptionText.text = description;
        }
    }

    public void OnEndTurnButton()
    {
        FightManager.instance.OnEndPlayerOperations();
    }

    void OnTutoStepStart(TutoManager.TutoStep finishedStep)
    {
        if (finishedStep == TutoManager.TutoStep.JustMoved)
        {
            endTurnButton.SetActive(true);
            canShowEndTurn = true;
        }
    }
}
