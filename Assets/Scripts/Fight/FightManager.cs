using System;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public enum FightState
    {
        None,
        PlayerTurnOperations, 
        PlayerTurnResolution,
        EnemyTurnMove,
        EnemyTurnResolution,
        FightVictoryScreen,
        GameOverScreen,
    }

    public static FightManager instance
    {
        get { return GlobalGameManager.Instance.fightManager; }
    }

    PlayGrid _grid;
    PlayGrid grid
    {
        get
        {
            if (_grid == null)
                _grid = GlobalGameManager.Instance.grid;
            return _grid;
        }
    }

    FightState _currentFightState = FightState.None;
    public FightState CurrentFightState
    {
        get
        {
            return _currentFightState;
        }
        private set
        {
            if (value != _currentFightState)
            {
                currentStateElementIndex = 0;
                _currentFightState = value;
                OnNewFightState?.Invoke(_currentFightState);
                HandleNewFightState();
            }
        }
    }

    List<FightCreature> allyCreatures = new List<FightCreature>();
    List<FightCreature> enemyCreatures = new List<FightCreature>();

    FightCreature selectedBoardFightCreature;
    PlayGrid.MoveSet selectedFightCreatureMoveSet = new PlayGrid.MoveSet();

    Card selectedHandCard;

    int _currentMana;
    public int CurrentMana
    {
        get { return _currentMana; }
        private set
        {
            if (_currentMana != value)
            {
                _currentMana = value;
                OnNewCurrentMana?.Invoke(_currentMana);
            }
        }
    }
    public static Action<int> OnNewCurrentMana;
    public static Action OnNotEnoughMana;

    List<Card> library = new List<Card>();
    public List<Card> hand { get; private set; } = new List<Card>();
    List<Card> discardPile = new List<Card>();
    public static Action OnCardPilesChanged;

    public static Action<FightState> OnNewFightState;

    int currentStateElementIndex = 0;

    private void Awake()
    {
        GlobalGameManager.OnNewGameState += OnNewGameState;
    }

    private void OnDestroy()
    {
        GlobalGameManager.OnNewGameState -= OnNewGameState;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentFightState)
        {
            case FightState.PlayerTurnResolution:
                UpdatePlayerTurnResolution();
                break;

            case FightState.EnemyTurnMove:
                UpdateEnemyTurnMove();
                break;

            case FightState.EnemyTurnResolution:
                UpdateEnemyTurnResolution();
                break;
        }
    }

    void OnNewGameState(GlobalGameManager.GameState previousGameState, GlobalGameManager.GameState newGameState)
    {
        if (newGameState == GlobalGameManager.GameState.Fight)
        {
            StartFight();
        }
        else if (previousGameState == GlobalGameManager.GameState.Fight)
        {
            EndFight();
        }
    }

    void StartFight()
    {
        GlobalGameManager.OnNotBusyAnymore += OnNotBusyAnymore;

        library.Clear();
        hand.Clear();
        discardPile.Clear();

        library.AddRange(GlobalGameManager.Instance.ownedCards);
        ShuffleLibrary();
        OnCardPilesChanged?.Invoke();

        allyCreatures.Add(grid.SpawnFightCreature(GlobalGameManager.Instance.mcCard, grid.GetSpot(PlayGrid.size / 2, PlayGrid.size / 2), true));

        for(int i = 0; i < GlobalGameManager.Instance.currentTurn * 2; i++)
            SpawnCreature(CardDefinitionType.BaseEnemy, false);

        UpdateGridVisuals();
        CurrentFightState = FightState.PlayerTurnOperations;
    }

    void EndFight()
    {
        GlobalGameManager.OnNotBusyAnymore -= OnNotBusyAnymore;
        for (int i = 0; i < allyCreatures.Count; i++)
        {
            Destroy(allyCreatures[i].gameObject);
        }
        for (int i = 0; i < enemyCreatures.Count; i++)
        {
            Destroy(enemyCreatures[i].gameObject);
        }
        allyCreatures.Clear();
        enemyCreatures.Clear();
        grid.Clear();
    }

    void HandleNewFightState()
    {
        switch (CurrentFightState)
        {
            case FightState.PlayerTurnOperations:
                OnStartPlayerTurn();
                break;
            case FightState.EnemyTurnMove:
                OnStartEnemyTurn();
                break;
            //case FightState.FightVictoryScreen:
            //    // Handled by FightVictoryScreen
            //    break;
            //case FightState.GameOverScreen:
            //    // Handled by GameOverScreen
            //    break;
        }
    }

    void OnStartPlayerTurn()
    {
        CurrentMana = Mathf.Max(CurrentMana, GetMaxManaPerTurn());

        while (hand.Count < GetHandMaxSize())
        {
            if (!DrawCard())
                break;
        }

        for (int i = 0; i < allyCreatures.Count; i++)
        {
            allyCreatures[i].OnNewTurn();
        }

        ProcessSelectedCreature();
        UpdateGridVisuals();
    }
    void OnStartEnemyTurn()
    {
        for (int i = 0; i < enemyCreatures.Count; i++)
        {
            enemyCreatures[i].OnNewTurn();
        }

        UpdateGridVisuals();
    }

    void UpdatePlayerTurnResolution()
    {
        if (!GlobalGameManager.Instance.IsBusy())
        {
            while (currentStateElementIndex < allyCreatures.Count && !allyCreatures[currentStateElementIndex].Attack())
            {
                currentStateElementIndex++;
            }
            if (currentStateElementIndex == allyCreatures.Count)
            {
                if (enemyCreatures.Count > 0)
                {
                    CurrentFightState = FightState.EnemyTurnMove;
                }
            }
            else
            {
                currentStateElementIndex++;
            }

            UpdateGridVisuals();
        }
    }
    void UpdateEnemyTurnMove()
    {
        if (!GlobalGameManager.Instance.IsBusy())
        {
            while (currentStateElementIndex < enemyCreatures.Count && !TryAndMoveAI(currentStateElementIndex))
            {
                currentStateElementIndex++;
            }
            if (currentStateElementIndex == enemyCreatures.Count)
            {
                if (enemyCreatures.Count > 0)
                {
                    CurrentFightState = FightState.EnemyTurnResolution;
                }
            }
            else
            {
                currentStateElementIndex++;
            }
            UpdateGridVisuals();
        }
    }
    bool TryAndMoveAI(int index)
    {
        if (index < enemyCreatures.Count)
        {
            grid.GetMoveSet(enemyCreatures[index].currentGridSpot, 20, ref selectedFightCreatureMoveSet);
            return enemyCreatures[index].TryAndMoveAI(selectedFightCreatureMoveSet);
        }
        return false;
    }
    void UpdateEnemyTurnResolution()
    {
        if (!GlobalGameManager.Instance.IsBusy())
        {
            while (currentStateElementIndex < enemyCreatures.Count && !enemyCreatures[currentStateElementIndex].Attack())
            {
                currentStateElementIndex++;
            }
            if (currentStateElementIndex == enemyCreatures.Count)
            {
                if (enemyCreatures.Count > 0)
                {
                    CurrentFightState = FightState.PlayerTurnOperations;
                }
            }
            else
            {
                currentStateElementIndex++;
            }
            UpdateGridVisuals();
        }
    }

    void OnNotBusyAnymore()
    {
        ProcessSelectedCreature();
        UpdateGridVisuals();
    }

    void SpawnCreature(CardDefinitionType creatureType, bool ally)
    {
        Card creatureCard = new Card(creatureType);
        if (creatureCard.cardDefinition.cardCategory != CardCategory.Creature)
        {
            Debug.LogError("Error: trying to spawn " + creatureType + " but it's not a creature.");
            return;
        }

        GridSpot spawnSpot = null;
        int remainingTries = 20;
        while ((spawnSpot == null || !creatureCard.IsValidTarget(spawnSpot)) && remainingTries > 0)
        {
            int x = UnityEngine.Random.Range(0, PlayGrid.size);
            int y = UnityEngine.Random.Range(0, PlayGrid.size);
            spawnSpot = grid.GetSpot(x, y);
        }
        if (spawnSpot != null)
        {
            FightCreature newCreature = grid.SpawnFightCreature(creatureCard, spawnSpot, ally);
            if (ally)
            {
                allyCreatures.Add(newCreature);
            }
            else
            {
                enemyCreatures.Add(newCreature);
            }
        }
    }

    public void OnDeadCreature(FightCreature creature)
    {
        if (creature == selectedBoardFightCreature)
        {
            UnselectSelectedFightCreature();
        }
        if (creature.isAlly)
        {
            int i = allyCreatures.IndexOf(creature);
            if (CurrentFightState == FightState.PlayerTurnResolution)
            {
                if (i <= currentStateElementIndex)
                    currentStateElementIndex--;
            }
            allyCreatures.RemoveAt(i);
            if (creature.isMC)
            {
                CurrentFightState = FightState.GameOverScreen;
            }
        }
        else
        {
            int i = enemyCreatures.IndexOf(creature);
            if (CurrentFightState == FightState.EnemyTurnMove ||
                CurrentFightState == FightState.EnemyTurnResolution)
            {
                if (i <= currentStateElementIndex)
                    currentStateElementIndex--;
            }
            GlobalGameManager.Instance.CoinsAmount += creature.coinsGain;
            enemyCreatures.RemoveAt(i);
            if (enemyCreatures.Count == 0)
            {
                CurrentFightState = FightState.FightVictoryScreen;
            }
        }
        creature.currentGridSpot.OnEntityLeave(creature);
        Destroy(creature.gameObject);
        UpdateGridVisuals();
    }

    public FightCreature GetClosestOpponentCreature(FightCreature attacker)
    {
        List<FightCreature> opponents = attacker.isAlly ? enemyCreatures : allyCreatures;
        float bestSqrDist = float.PositiveInfinity;
        FightCreature bestCandidate = null;
        for (int i = 0; i < opponents.Count; i++)
        {
            float sqrDist = (opponents[i].transform.position - attacker.transform.position).sqrMagnitude;
            if (sqrDist < bestSqrDist)
            {
                bestSqrDist = sqrDist;
                bestCandidate = opponents[i];
            }
        }
        return bestCandidate;
    }

    public void OnGridSpotClick(GridSpot spot)
    {
        if (selectedHandCard != null)
        {
            if (selectedHandCard.IsValidTarget(spot))
            {
                CurrentMana = Mathf.Max(0, CurrentMana - selectedHandCard.ManaCost);
                selectedHandCard.Cast(spot);

                UpdateGridVisuals();
            }
        }
        else
        {
            if (spot.gridEntity != selectedBoardFightCreature)
            {
                if (selectedFightCreatureMoveSet.GetElement(spot) != null)
                {
                    selectedBoardFightCreature.GoTo(spot, selectedFightCreatureMoveSet);
                }
                else
                {
                    SelectEntity(spot.gridEntity);
                }

                UpdateGridVisuals();
            }
        }
    }

    public void OnCardCastEnded()
    {
        hand.Remove(selectedHandCard);
        discardPile.Add(selectedHandCard);
        selectedHandCard = null;
        OnCardPilesChanged?.Invoke();
    }

    public void SelectEntity(GridEntity gridEntity)
    {
        if (selectedBoardFightCreature != null)
        {
            // The currently selected fight creature will be unselected
            selectedFightCreatureMoveSet.Clear();
            UnselectSelectedFightCreature();
        }

        if (gridEntity != null)
        {
            if (gridEntity is FightCreature)
            {
                selectedBoardFightCreature = gridEntity as FightCreature;
                ProcessSelectedCreature();
            }
            else
            {
                Debug.LogWarning("Warning: entity of type " + gridEntity + " selected but only FightCreature supported by FightManager");
            }
        }
        else
        {
        }
    }
    void UnselectSelectedFightCreature()
    {
        selectedBoardFightCreature = null;
        selectedFightCreatureMoveSet.Clear();
    }

    public void OnSelectedHandCard(Card card)
    {
        if (selectedBoardFightCreature != null)
        {
            UnselectSelectedFightCreature();
        }
        selectedHandCard = card;
        UpdateGridVisuals();
    }
    public void OnUnselectedHandCard()
    {
        selectedHandCard = null;
        UpdateGridVisuals();
    }

    void ProcessSelectedCreature()
    {
        selectedFightCreatureMoveSet.Clear();
        if (selectedBoardFightCreature != null)
        {
            if (selectedBoardFightCreature.isAlly && CurrentFightState == FightState.PlayerTurnOperations)
            {
                grid.GetMoveSet(selectedBoardFightCreature.currentGridSpot, selectedBoardFightCreature.turnRemainingMoves, ref selectedFightCreatureMoveSet);
            }
        }
    }

    public GridSpot.GridSpotVisualState GetGridSpotVisualState(GridSpot spot)
    {
        if (GlobalGameManager.Instance.IsBusy())
            return 0;

        if (selectedFightCreatureMoveSet.GetElement(spot, false) != null)
        {
            return GridSpot.GridSpotVisualState.MoveDestination;
        }
        if (selectedHandCard != null && selectedHandCard.IsValidTarget(spot))
        {
            return GridSpot.GridSpotVisualState.ValidTarget;
        }
        return 0;
    }

    public void OnEndPlayerOperations()
    {
        CurrentFightState = FightState.PlayerTurnResolution;
        ProcessSelectedCreature();
        UpdateGridVisuals();
    }

    void UpdateGridVisuals()
    {
        grid.UpdateAllSpotVisuals();
    }

    /* CARDS */

    void ShuffleLibrary()
    {
        if (library.Count > 1)
        {
            for (int i = 0; i < library.Count * 2; i++)
            {
                int i1 = UnityEngine.Random.Range(0, library.Count);
                int i2 = UnityEngine.Random.Range(0, library.Count);
                while(i2 == i1)
                    i2 = UnityEngine.Random.Range(0, library.Count);
                Card c1 = library[i1];
                library[i1] = library[i2];
                library[i2] = c1;
            }
        }
    }
    bool DrawCard()
    {
        if (library.Count == 0)
        {
            if (discardPile.Count == 0)
            {
                return false;
            }
            library.AddRange(discardPile);
            discardPile.Clear();
            ShuffleLibrary();
        }
        hand.Add(library[0]);
        library.RemoveAt(0);
        OnCardPilesChanged?.Invoke();
        return true;
    }

    public int GetLibraryCount()
    {
        return library.Count;
    }
    public int GetDiscardPileCount()
    {
        return discardPile.Count;
    }

    int GetHandMaxSize()
    {
        return 5;
    }
    int GetMaxManaPerTurn()
    {
        return 3;
    }
}
