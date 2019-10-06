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

    PickupCollection collection;

    public void Init(PickupDefinition pickup, PickupCollection _collection)
    {
        backgroundImage.color = pickup.GetBackgroundColor();
        iconImage.sprite = pickup.GetSprite();

        descriptionLabel.text = pickup.GetDescription();
        description.SetActive(false);

        collection = _collection;

        selectedBackground.SetActive(false);
    }

    public void OnPointerEnter()
    {
        description.SetActive(true);
    }
    public void OnPointerExit()
    {
        description.SetActive(false);
    }
    public void OnClick()
    {
        collection.OnPickupClicked(this);
    }

    public void Select()
    {
        selectedBackground.SetActive(true);
    }
    public void Unselect()
    {
        selectedBackground.SetActive(false);
    }
}
