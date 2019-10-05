using System;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    //static FightManager _instance;
    //public static FightManager Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance = FindObjectOfType<FightManager>();
    //        }
    //        if (_instance == null)
    //        {
    //            GameObject fightManagerInstance = new GameObject("FightManager");
    //            _instance = fightManagerInstance.AddComponent<FightManager>();
    //        }
    //        return _instance;
    //    }
    //}

    //private void Awake()
    //{
    //    if (_instance != null && _instance != this)
    //    {
    //        Debug.LogError("Error: two FightManager instances.");
    //        DestroyImmediate(gameObject);
    //        return;
    //    }
    //    _instance = this;
    //}

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

    CreatureDefinitionsLibrary _creatureDefinitionsLibrary;
    CreatureDefinitionsLibrary creatureDefinitionsLibrary
    {
        get
        {
            if (_creatureDefinitionsLibrary == null)
                _creatureDefinitionsLibrary = GlobalGameManager.Instance.creatureDefinitionsLibrary;
            return _creatureDefinitionsLibrary;
        }
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
                Debug.Log("New fight state " + _currentFightState);
                HandleNewFightState();
            }
        }
    }

    List<FightCreature> allyCreatures = new List<FightCreature>();
    List<FightCreature> enemyCreatures = new List<FightCreature>();

    FightCreature selectedFightCreature;
    PlayGrid.MoveSet selectedFightCreatureMoveSet = new PlayGrid.MoveSet();

    public static Action<FightState> OnNewFightState;

    int currentStateElementIndex = 0;

    private void Awake()
    {
        GlobalGameManager.OnNewGameState += OnNewGameState;
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

        CreatureDefinition mcDef = creatureDefinitionsLibrary.GetDefinition(CreatureType.MC);
        allyCreatures.Add(GlobalGameManager.Instance.grid.SpawnFightCreature(mcDef, GlobalGameManager.Instance.grid.GetSpot(PlayGrid.size / 2, PlayGrid.size / 2), true));

        CreatureDefinition enemyDef = creatureDefinitionsLibrary.GetDefinition(CreatureType.Enemy1);
        SpawnCreature(enemyDef, false);

        UpdateGridVisuals();
        CurrentFightState = FightState.PlayerTurnOperations;
    }

    void EndFight()
    {
        GlobalGameManager.OnNotBusyAnymore -= OnNotBusyAnymore;
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
            case FightState.FightVictoryScreen:
                //TODO
                Debug.Log("FIGHT VICTORY");
                break;
            case FightState.GameOverScreen:
                //TODO
                Debug.Log("GAME OVER");
                break;
        }
    }

    void OnStartPlayerTurn()
    {
        for (int i = 0; i < allyCreatures.Count; i++)
        {
            allyCreatures[i].OnNewTurn();
        }
        ProcessSelectedCreature();
    }
    void OnStartEnemyTurn()
    {
        for (int i = 0; i < enemyCreatures.Count; i++)
        {
            enemyCreatures[i].OnNewTurn();
        }
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
                else
                {
                    CurrentFightState = FightState.FightVictoryScreen;
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
                else
                {
                    CurrentFightState = FightState.FightVictoryScreen;
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
                else
                {
                    CurrentFightState = FightState.FightVictoryScreen;
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

    void SpawnCreature(CreatureDefinition creatureDef, bool ally)
    {
        GridSpot spawnSpot = null;
        int remainingTries = 20;
        while (spawnSpot == null && remainingTries > 0)
        {
            int x = UnityEngine.Random.Range(0, PlayGrid.size);
            int y = UnityEngine.Random.Range(0, PlayGrid.size);
            spawnSpot = grid.GetSpot(x, y, true);
        }
        if (spawnSpot != null)
        {
            FightCreature newCreature = GlobalGameManager.Instance.grid.SpawnFightCreature(creatureDef, spawnSpot, ally);
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
        if (creature == selectedFightCreature)
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
            enemyCreatures.RemoveAt(i);
            //if (enemyCreatures.Count == 0)
            //{
            //    //TODO
            //    Debug.Log("FIGHT VICTORY");
            //}
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
        if (spot.gridEntity != selectedFightCreature)
        {
            if (selectedFightCreatureMoveSet.GetElement(spot) != null)
            {
                selectedFightCreature.GoTo(spot, selectedFightCreatureMoveSet);
            }
            else
            {
                SelectEntity(spot.gridEntity);
            }

            UpdateGridVisuals();
        }
    }

    public void SelectEntity(GridEntity gridEntity)
    {
        if (selectedFightCreature != null)
        {
            // The currently selected fight creature will be unselected
            selectedFightCreatureMoveSet.Clear();
            UnselectSelectedFightCreature();
        }

        if (gridEntity != null)
        {
            if (gridEntity is FightCreature)
            {
                selectedFightCreature = gridEntity as FightCreature;
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
        selectedFightCreature = null;
    }

    void ProcessSelectedCreature()
    {
        if (selectedFightCreature != null)
        {
            if (selectedFightCreature.isAlly && CurrentFightState == FightState.PlayerTurnOperations)
            {
                grid.GetMoveSet(selectedFightCreature.currentGridSpot, selectedFightCreature.turnRemainingMoves, ref selectedFightCreatureMoveSet);
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
        return 0;
    }

    public void OnEndPlayerOperations()
    {
        CurrentFightState = FightState.PlayerTurnResolution;
        selectedFightCreatureMoveSet.Clear();
        UpdateGridVisuals();
    }

    void UpdateGridVisuals()
    {
        grid.UpdateAllSpotVisuals();
    }
}
