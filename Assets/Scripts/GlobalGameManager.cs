﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    public enum GameState
    {
        None, 
        Fight, 
        Match3, 
        Deckbuilding,
    }

    //public CreatureDefinitionsLibrary creatureDefinitionsLibrary;
    public CardBaseDefinitionLibrary cardBaseDefinitionsLibrary;

    public PickupCollection pickupCollection;
    public DeckBuildingManager deckBuildingManager;
    public GameObject busyRaycastBlocker;
    public TutoManager tutoManager;
    public FightPanels fightPanels;

    public TMPro.TextMeshProUGUI gridTitle;

    public FightManager fightManager { get; set; }
    public PlayGrid grid { get; set; }

    public int currentTurn { get; private set; } = 0;

    public List<Card> ownedCards { get; set; }
    public Card mcCard;

    public List<CardDefinitionType> knownCards { get; private set; }

    int _coinsAmount = 0;
    public int CoinsAmount
    {
        get
        {
            return _coinsAmount;
        }
        set
        {
            _coinsAmount = value;
            OnNewCoinsAmount?.Invoke(_coinsAmount);
        }
    }
    public static Action<int> OnNewCoinsAmount;

    GameState _currentGameState = GameState.None;
    public GameState CurrentGameState {
        get
        {
            return _currentGameState;
        }
        private set
        {
            if (value != _currentGameState)
            {
                GameState previousGameState = _currentGameState;
                _currentGameState = value;
                OnNewGameState?.Invoke(previousGameState, _currentGameState);
            }
        }
    }
    public static Action<GameState, GameState> OnNewGameState;

    static GlobalGameManager _instance;
    public static GlobalGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GlobalGameManager>();
            }
            return _instance;
        }
    }

    List<IGameElement> busyElements = new List<IGameElement>();
    public static Action OnNotBusyAnymore;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("Error: two GlobalGameManager instances.");
            DestroyImmediate(gameObject);
            return;
        }
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        fightManager = gameObject.AddComponent<FightManager>();
        grid = FindObjectOfType<PlayGrid>();

        mcCard = new Card(CardDefinitionType.MC);

        ownedCards = new List<Card>();

        knownCards = new List<CardDefinitionType>();
        knownCards.Add(CardDefinitionType.MC);


        ////////// TESTS
        //for (int i = 0; i < 10; i++) 
        //    ownedCards.Add(new Card(CardDefinitionType.Fireball));
        //for (int i = 0; i < 4; i++)
        //    pickupCollection.EarnPickup(new PickupEffectDefinition(PickupEffect.Damage));
        //for (int i = 0; i < 4; i++)
        //    pickupCollection.EarnPickup(new PickupCategoryDefinition(CardCategory.Creature));
        //for (int i = 0; i < 4; i++)
        //    pickupCollection.EarnPickup(new PickupCategoryDefinition(CardCategory.InstantSpell));
        //CoinsAmount = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentGameState == GameState.None)
        {
            busyRaycastBlocker.SetActive(false);
            StartFight();
            tutoManager.OnTutoEvent(TutoManager.TutoStep.Intro);
        }

        if (IsBusy())
        {
            for (int i = 0; i < busyElements.Count; i++)
            {
                if (!busyElements[i].IsBusy())
                {
                    busyElements.RemoveAt(i);
                    i--;
                }
            }
            if (!IsBusy())
            {
                OnNotBusyAnymore?.Invoke();
                busyRaycastBlocker.SetActive(false);
            }
        }
    }
    public void StartFight()
    {
        currentTurn++;
        CurrentGameState = GameState.Fight;
        gridTitle.text = "FIGHT";
        tutoManager.OnTutoEvent(TutoManager.TutoStep.FightWithCards);
    }

    public void OnFightVictoryScreenEnded()
    {
        ShowCollection();
    }

    public void ShowCollection()
    {
        CurrentGameState = GameState.Deckbuilding;
        grid.PopulateWithCards();
        GlobalGameManager.Instance.tutoManager.OnTutoEvent(TutoManager.TutoStep.ToCardCrafting);
        gridTitle.text = "CARDS COLLECTION";
    }

    // If this wasn't a game jam, this function would definitely not be there
    public void OnGridSpotClick(GridSpot spot)
    {
        switch (CurrentGameState)
        {
            case GameState.Fight:
                fightManager.OnGridSpotClick(spot);
                break;
            case GameState.Deckbuilding:
                deckBuildingManager.OnGridSpotClick(spot);
                break;
        }
    }
    public GridSpot.GridSpotVisualState GetGridSpotVisualState(GridSpot spot)
    {
        switch (CurrentGameState)
        {
            case GameState.Fight:
                return fightManager.GetGridSpotVisualState(spot);
        }
        return 0;
    }

    public void OnGainedCard(Card card)
    {
        ownedCards.Add(card);
        if (!DoesPlayerKnowCard(card.cardDefinition.cardType))
        {
            knownCards.Add(card.cardDefinition.cardType);
        }
        tutoManager.OnTutoEvent(TutoManager.TutoStep.CardCrafted);
    }

    public bool DoesPlayerKnowCard(CardDefinitionType type)
    {
        return knownCards.Contains(type);
    }

    public void OnBusyElement(IGameElement busyElement)
    {
        if (!busyElements.Contains(busyElement))
        {
            busyElements.Add(busyElement);
            busyRaycastBlocker.SetActive(true);
        }
        else
        {
            Debug.LogError("Error: busy element " + busyElement + " was already busy.");
        }
    }

    public bool IsBusy()
    {
        return busyElements.Count > 0;
    }
}

public interface IGameElement
{
    bool IsBusy();
}