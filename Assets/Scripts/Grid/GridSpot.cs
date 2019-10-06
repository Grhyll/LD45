using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSpot : MonoBehaviour
{
    public enum GridSpotVisualState
    {
        MoveDestination = 1 << 0,
        Enemy = 1 << 1,
        AllyRemainingMove = 1 << 2,
        AllyNoRemainingMove = 1 << 3,
        ValidTarget = 1 << 4,
    }

    public Image spotBg;
    public Image availableMoveBg;
    public Image enemyBg;
    public Image allyRemainingMoveBg;
    public Image allyNoRemainingMoveBg;
    public Image validTarget;

    public int CoordX { get; private set; }
    public int CoordY { get; private set; }

    public GridEntity gridEntity { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        UpdateVisuals(0);
    }

    public void Init(int coordX, int coordY)
    {
        CoordX = coordX;
        CoordY = coordY;
    }

    public void OnEntityEnters(GridEntity entity)
    {
        if (gridEntity != null)
        {
            Debug.LogError("Error: Entity " + entity + " trying to enter grid spot but there was already entity " + gridEntity, gameObject);
        }
        gridEntity = entity;
    }
    public void OnEntityLeave(GridEntity entity)
    {
        if (gridEntity != entity)
        {
            Debug.LogError("Error: entity " + entity + " trying to leave grid spot but grid spot entity was " + gridEntity, gameObject);
        }
        gridEntity = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsFree()
    {
        return gridEntity == null;
    }

    public void OnSpotClick()
    {
        GlobalGameManager.Instance.OnGridSpotClick(this);
    }

    public void UpdateVisuals()
    {
        GridSpotVisualState visualState = GlobalGameManager.Instance.GetGridSpotVisualState(this) | (gridEntity != null ? gridEntity.GetSpotVisualStateMask() : 0);
        UpdateVisuals(visualState);
    }
    public void Clear()
    {
        gridEntity = null;
        UpdateVisuals(0);
    }

    void UpdateVisuals(GridSpotVisualState visualState)
    {
        availableMoveBg.gameObject.SetActive((visualState & GridSpotVisualState.MoveDestination) != 0);
        enemyBg.gameObject.SetActive((visualState & GridSpotVisualState.Enemy) != 0);
        allyRemainingMoveBg.gameObject.SetActive((visualState & GridSpotVisualState.AllyRemainingMove) != 0);
        allyNoRemainingMoveBg.gameObject.SetActive((visualState & GridSpotVisualState.AllyNoRemainingMove) != 0);
        validTarget.gameObject.SetActive((visualState & GridSpotVisualState.ValidTarget) != 0);
    }
}
