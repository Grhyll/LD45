using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    public enum GameState
    {
        None, 
        Fight, 
        Match3, 
        CardBuilding,
    }

    public CreatureDefinitionsLibrary creatureDefinitionsLibrary;

    public GameObject busyRaycastBlocker;

    public FightManager fightManager { get; set; }
    public PlayGrid grid { get; set; }

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
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentGameState == GameState.None)
        {
            busyRaycastBlocker.SetActive(false);
            CurrentGameState = GameState.Fight;
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

    // If this wasn't a game jam, this function would definitely not be there
    public void OnGridSpotClick(GridSpot spot)
    {
        switch (CurrentGameState)
        {
            case GameState.Fight:
                fightManager.OnGridSpotClick(spot);
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