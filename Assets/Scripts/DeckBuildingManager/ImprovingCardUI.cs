using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImprovingCardUI : MonoBehaviour
{
    public GameObject breakCardButton;
    public Button improveButton;
    public TextMeshProUGUI improveButtonLabel;

    public PickupSimpleUI pickupSimpleUI;

    Card improvedCard;

    const int improvingCost = 1;

    public void Start()
    {
        pickupSimpleUI.SetManager(this);
    }

    public void Show(Card card)
    {
        improvedCard = card;
        gameObject.SetActive(true);
        pickupSimpleUI.Clear();
        GlobalGameManager.Instance.pickupCollection.OnNewState(PickupCollection.CollectionState.Improving);
        CheckState();
    }
    public void Hide()
    {
        GlobalGameManager.Instance.pickupCollection.OnNewState(PickupCollection.CollectionState.Consulting);
        gameObject.SetActive(false);
    }

    public bool CanSelectPickup(PickupUI pickup)
    {
        return improvedCard.AddBonus(pickup.pickupDefinition) && pickupSimpleUI.currentPickup != pickup;
    }

    public void OnSelectedPickup(PickupUI pickup)
    {
        pickupSimpleUI.SetPickup(pickup);
        CheckState();
    }
    public void OnSimplePickupCleared(PickupSimpleUI simplePickup)
    {
        CheckState();
    }

    void CheckState()
    {
        breakCardButton.SetActive(false);
        if (pickupSimpleUI.currentPickup != null)
        {
            improveButton.gameObject.SetActive(true);
            bool canPurchase = improvingCost <= GlobalGameManager.Instance.CoinsAmount;
            improveButton.interactable = canPurchase;
            improveButtonLabel.color = canPurchase ? Color.black : Color.red;
        }
        else
        {
            improveButton.gameObject.SetActive(false);
        }
        GlobalGameManager.Instance.pickupCollection.UpdatePickupStates();
    }

    public void OnBreakCard()
    {
    }

    public void OnApplyBonus()
    {
        GlobalGameManager.Instance.CoinsAmount -= improvingCost;
        improvedCard.AddBonus(pickupSimpleUI.currentPickup.pickupDefinition, true);
        GlobalGameManager.Instance.pickupCollection.OnPickupConsumed(pickupSimpleUI.currentPickup);
        GlobalGameManager.Instance.deckBuildingManager.editedCard.ShowCard(improvedCard);
        pickupSimpleUI.Clear();
        CheckState();
    }

}
