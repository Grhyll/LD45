using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour, IGameElement
{
    public virtual GridSpot.GridSpotVisualState GetSpotVisualStateMask()
    {
        return 0;
    }

    public virtual bool IsBusy()
    {
        return false;
    }
}
