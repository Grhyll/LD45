using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupUI : MonoBehaviour
{
    public GameObject selectedBackground;

    public Image backgroundImage;
    public Image iconImage;

    public GameObject description;
    public TextMeshProUGUI descriptionLabel;

    public GameObject disabledImage;

    public PickupDefinition pickupDefinition { get; private set; }

    PickupCollection collection;

    bool isDisabled = false;

    public void Init(PickupDefinition pickup, PickupCollection _collection)
    {
        backgroundImage.color = pickup.GetBackgroundColor();
        iconImage.sprite = pickup.GetSprite();

        descriptionLabel.text = pickup.GetDescription();
        description.SetActive(false);

        collection = _collection;

        selectedBackground.SetActive(false);

        pickupDefinition = pickup;

        disabledImage.SetActive(false);
    }

    public void OnPointerEnter()
    {
        if (!isDisabled)
        {
            description.SetActive(true);
        }
    }
    public void OnPointerExit()
    {
        description.SetActive(false);
    }
    public void OnClick()
    {
        if (!isDisabled)
        {
            collection.OnPickupClicked(this);
        }
    }

    public void Select()
    {
        selectedBackground.SetActive(true);
    }
    public void Unselect()
    {
        selectedBackground.SetActive(false);
    }

    public void DisableSelection()
    {
        disabledImage.SetActive(true);
        isDisabled = true;
    }
    public void EnableSelection()
    {
        disabledImage.SetActive(false);
        isDisabled = false;
    }
}
