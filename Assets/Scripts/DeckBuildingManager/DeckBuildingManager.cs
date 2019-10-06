using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuildingManager : MonoBehaviour
{
    public EditedCard editedCard;
    public DeckbuildingUI deckbuildingUI;

    GridSpot selectedSpot;
    bool isCreatingCard;
    Card cardBeingEdited;

    // Start is called before the first frame update
    void Start()
    {
        editedCard.gameObject.SetActive(false);
        deckbuildingUI.createCardUI.Hide();
        deckbuildingUI.improvingCardUI.Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGridSpotClick(GridSpot spot)
    {
        selectedSpot = spot;
        if (spot.gridEntity != null)
        {
            cardBeingEdited = (spot.gridEntity as GridCardUI).card;
            editedCard.ShowCard(cardBeingEdited);
            deckbuildingUI.improvingCardUI.Show(cardBeingEdited);
        }
        else
        {
            editedCard.ShowUnknownCard();
            isCreatingCard = true;
            deckbuildingUI.createCardUI.Show();
        }
    }

    public void OnClosedEditedCard()
    {
        if (cardBeingEdited != null)
        {
            deckbuildingUI.improvingCardUI.Hide();
        }
        else if (isCreatingCard)
        {
            deckbuildingUI.createCardUI.Hide();
        }
    }

    public void OnCardCreated(Card card)
    {
        GlobalGameManager.Instance.OnGainedCard(card);
        deckbuildingUI.createCardUI.Hide();
        PlayGrid.Instance.OnNewGridCard(card, selectedSpot);
        cardBeingEdited = card;
        editedCard.ShowCard(card);
        isCreatingCard = false;
        deckbuildingUI.improvingCardUI.Show(cardBeingEdited);
    }
}
