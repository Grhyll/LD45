using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateCardUI : MonoBehaviour
{
    public Button createCardButton;
    public TextMeshProUGUI createCardLabel;

    public TextMeshProUGUI currentInstructionsLabel;

    public PickupSimpleUI categorySimpleUI;
    public PickupSimpleUI effect1SimpleUI;

    public void Start()
    {
        categorySimpleUI.SetManager(this);
        effect1SimpleUI.SetManager(this);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        categorySimpleUI.Clear();
        effect1SimpleUI.Clear();
        GlobalGameManager.Instance.pickupCollection.OnNewState(PickupCollection.CollectionState.Creating);
        CheckState();
    }
    public void Hide()
    {
        GlobalGameManager.Instance.pickupCollection.OnNewState(PickupCollection.CollectionState.Consulting);
        gameObject.SetActive(false);
    }

    public bool CanSelectPickup(PickupDefinition pickup)
    {
        if (categorySimpleUI.currentPickup == null)
        {
            return pickup.type == PickupType.Category;
        }
        else if (effect1SimpleUI.currentPickup == null)
        {
            return pickup.type == PickupType.Effect;
        }
        //TODO if multiple possible effects; disable already selected ones
        return false;
    }
    public void OnSelectedPickup(PickupUI pickup)
    {
        if (categorySimpleUI.currentPickup == null)
        {
            if (pickup.pickupDefinition.type == PickupType.Category)
            {
                categorySimpleUI.SetPickup(pickup);
            }
            else
            {
                Debug.LogError("Error: Selected a non-category pick up but a category one was expected.");
            }
        }
        else
        {
            if (effect1SimpleUI.currentPickup == null)
            {
                if (pickup.pickupDefinition.type == PickupType.Effect)
                {
                    effect1SimpleUI.SetPickup(pickup);
                }
                else
                {
                    Debug.LogError("Error: Selected a non-effect pick up but an effect one was expected.");
                }
            }
            else
            {
                Debug.LogError("Error: Selected an effect pick up but the effect field is already full.");
            }
        }
        CheckState();
    }
    public void OnSimplePickupCleared(PickupSimpleUI simplePickup)
    {
        CheckState();
    }

    void CheckState()
    {
        bool canCreate = false;
        if (categorySimpleUI.currentPickup == null)
        {
            currentInstructionsLabel.text = "Select a card category.";
        }
        else if (effect1SimpleUI.currentPickup == null)
        {
            if (categorySimpleUI.currentPickup.pickupDefinition.type == PickupType.Category &&
                (categorySimpleUI.currentPickup.pickupDefinition as PickupCategoryDefinition).category == CardCategory.Creature)
            {
                currentInstructionsLabel.text = "Select an effect or create your card.";
                canCreate = true;
            }
            else
            {
                currentInstructionsLabel.text = "Select an effect.";
            }
        }
        else
        {
            canCreate = true;
            currentInstructionsLabel.text = "You can now create your card.";
        }
        createCardButton.gameObject.SetActive(canCreate);
        if (canCreate)
        {
            int cost = CalculateCost();
            bool canPurchase = cost <= GlobalGameManager.Instance.CoinsAmount;
            createCardButton.interactable = canPurchase;
            createCardLabel.text = "Create for " + cost.ToString();
            createCardLabel.color = canPurchase ? Color.black : Color.red;
        }
        GlobalGameManager.Instance.pickupCollection.UpdatePickupStates();
        //TODO: Update edited card if needed (known or unknown)

        if (canCreate)
        {
            CardDefinitionType cardDefinitionType = GetCurrentCardDefinitionType();
            Card resultCard = new Card(cardDefinitionType);
            GlobalGameManager.Instance.deckBuildingManager.editedCard.ShowCard(resultCard);
        }
        else
        {
            GlobalGameManager.Instance.deckBuildingManager.editedCard.ShowUnknownCard();
        }
    }

    public void OnCreateButton()
    {
        CardDefinitionType cardDefinitionType = GetCurrentCardDefinitionType();
        if (cardDefinitionType != CardDefinitionType.None)
        {
            GlobalGameManager.Instance.CoinsAmount -= CalculateCost();
            GlobalGameManager.Instance.pickupCollection.OnPickupConsumed(categorySimpleUI.currentPickup);
            if (effect1SimpleUI.currentPickup != null)
            {
                GlobalGameManager.Instance.pickupCollection.OnPickupConsumed(categorySimpleUI.currentPickup);
            }
            Card resultCard = new Card(cardDefinitionType);
            GlobalGameManager.Instance.deckBuildingManager.OnCardCreated(resultCard);
        }
    }

    int CalculateCost()
    {
        return 2 + (effect1SimpleUI.currentPickup != null ? 1 : 0);
    }

    CardDefinitionType GetCurrentCardDefinitionType()
    {
        return CardBaseDefinition.GetCardDefinitionType(
            categorySimpleUI.currentPickup != null ? (categorySimpleUI.currentPickup.pickupDefinition as PickupCategoryDefinition).category : CardCategory.None,
            effect1SimpleUI.currentPickup != null ? (effect1SimpleUI.currentPickup.pickupDefinition as PickupEffectDefinition).effect : PickupEffect.None);
    }
}
