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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGridSpotClick(GridSpot spot)
    {
        selectedSpot = spot;
        // TODO: Tell to right side UI
        if (spot.gridEntity != null)
        {
            cardBeingEdited = (spot.gridEntity as GridCardUI).card;
            editedCard.ShowCard(cardBeingEdited);
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
        }
        else if (isCreatingCard)
        {
            deckbuildingUI.createCardUI.Hide();
        }
    }

    public void OnCardCreated(Card card)
    {
        deckbuildingUI.createCardUI.Hide();
        PlayGrid.Instance.OnNewGridCard(card, selectedSpot);
        cardBeingEdited = card;
        editedCard.ShowCard(card);
        isCreatingCard = false;
    }
}
