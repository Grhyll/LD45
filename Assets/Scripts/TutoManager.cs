using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutoManager : MonoBehaviour, IGameElement
{
    public enum TutoStep
    {
        Intro = 0, 
        JustMoved = 1,
        ToCardCrafting = 2,
        CardCrafted = 3,
        FightWithCards = 4,
        Max
    }

    public TutoStep CurrentTutoStep { get; set; } = TutoStep.Intro;

    public TextMeshProUGUI tutoText;

    // Tuto objects
    public GameObject tutoObjectsContainer;
    GameObject[] tutoObjects;
    public GameObject mc;

    public string[] intro { get; set; } = { "This is you.",
        "You're on a grid with enemies.",
        "At the end of your turn, you will attack all enemies within range.",
        "Click on yourself to see where you can move!",
        "Beware, at the end of your turn, enemies will move as well and attack you if you're within range!" };
    public string[] justMoved { get; set; } = { "Great job!",
        "You can press the button on the top right corner to end your turn when you're ready." };
    public string[] toCardCrafting { get; set; } = { "Amazing, you killed some enemies!",
        "Look how you earned some coins as well as some bonuses by doing so!",
        "On the left is your card collection. Not great right now, but any empty square can become a card!",
        "Click on a empty square and use those bonuses to craft a card for the next fight!" };
    public string[] cardCrafted { get; set; } = { "Amazing! Once crafted, you can improve your cards with the same bonuses!" };
    public string[] fightWithCards { get; set; } = { "Now you have cards to use in the battle.",
        "Select a card in the bottom right and cast it on a square!" };

    int currentTutoIndex = 0;

    public static Action<TutoStep> OnTutoStepStart;
    public static Action<TutoStep> OnTutoStepFinished;

    public void OnTutoEvent(TutoStep step)
    {
        if (step == CurrentTutoStep && !IsBusy())
        {
            gameObject.SetActive(true);
            currentTutoIndex = 0;

            if (tutoObjects == null)
            {
                tutoObjects = new GameObject[tutoObjectsContainer.transform.childCount];
                for (int i = 0; i < tutoObjects.Length; i++)
                    tutoObjects[i] = tutoObjectsContainer.transform.GetChild(i).gameObject;
            }
            for (int i = 0; i < tutoObjects.Length; i++)
            {
                tutoObjects[i].SetActive(false);
            }
            HandleNewStepObjects(CurrentTutoStep);
            GlobalGameManager.Instance.OnBusyElement(this);

            OnClick();
            OnTutoStepStart.Invoke(CurrentTutoStep);
        }
    }

    public void OnClick()
    {
        string[] texts = GetStepTexts(CurrentTutoStep);
        if (texts != null && texts.Length > currentTutoIndex)
        {
            tutoText.text = texts[currentTutoIndex];
            currentTutoIndex++;
        }
        else
        {
            OnTutoStepFinished?.Invoke(CurrentTutoStep);
            CurrentTutoStep++;
            gameObject.SetActive(false);
        }
    }

    string[] GetStepTexts(TutoStep step)
    {
        switch (step)
        {
            case TutoStep.Intro:
                return intro;
            case TutoStep.JustMoved:
                return justMoved;
            case TutoStep.ToCardCrafting:
                return toCardCrafting;
            case TutoStep.CardCrafted:
                return cardCrafted;
            case TutoStep.FightWithCards:
                return fightWithCards;

            default:
                Debug.LogError("Tuto step " + step + " doesn't have texts.");
                return null;
        }
    }
    void HandleNewStepObjects(TutoStep step)
    {
        switch (step)
        {
            case TutoStep.Intro:
                mc.SetActive(true);
                break;
        }
    }

    public bool IsBusy()
    {
        return gameObject.activeSelf;
    }
}
