using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditedCard : MonoBehaviour
{
    public CardUI cardUI;

    public void ShowCard(Card card)
    {
        if (card != null && GlobalGameManager.Instance.DoesPlayerKnowCard(card.cardDefinition.cardType))
        {
            gameObject.SetActive(true);
            cardUI.gameObject.SetActive(true);
            cardUI.Init(card);
        }
        else
        {
            ShowUnknownCard();
        }
    }

    public void ShowUnknownCard()
    {
        gameObject.SetActive(true);
        cardUI.InitUnknown();
    }

    public void OnBackgroundClick()
    {
        gameObject.SetActive(false);
        GlobalGameManager.Instance.deckBuildingManager.OnClosedEditedCard();
    }

    public void Close()
    {
        OnBackgroundClick();
    }
}
