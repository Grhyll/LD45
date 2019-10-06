using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupSimpleUI : MonoBehaviour
{
    public Image backgroundImage;
    public Image iconImage;

    public PickupUI currentPickup;

    CreateCardUI createCardUI;

    public void SetManager(CreateCardUI _createCardUI)
    {
        createCardUI = _createCardUI;
    }

    public void SetPickup(PickupUI pickup)
    {
        iconImage.gameObject.SetActive(true);
        backgroundImage.color = pickup.pickupDefinition.GetBackgroundColor();
        iconImage.sprite = pickup.pickupDefinition.GetSprite();

        currentPickup = pickup;
    }
    public void Clear()
    {
        iconImage.gameObject.SetActive(false);
        backgroundImage.color = Color.gray;

        currentPickup = null;
    }

    public void OnClik()
    {
        if(currentPickup != null)
        {
            Clear();

            if (createCardUI != null)
            {
                createCardUI.OnSimplePickupCleared(this);
            }
        }
    }
}
