using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridCardUI : GridEntity
{
    public Image cardSprite;

    public GameObject creatureContainer;
    public TextMeshProUGUI creatureHealthLabel;
    public TextMeshProUGUI creatureAttackLabel;

    public GridSpot currentGridSpot { get; private set; }

    public Card card { get; set; }


    public void Init(Card _card, GridSpot spot)
    {
        card = _card;

        cardSprite.sprite = card.cardDefinition.sprite;
        if (card.cardDefinition.cardCategory == CardCategory.Creature)
        {
            creatureContainer.SetActive(true);
            creatureHealthLabel.text = card.Health.ToString();
            creatureAttackLabel.text = card.Damage.ToString();
        }
        else
        {
            creatureContainer.SetActive(false);
        }
        
        currentGridSpot = spot;
    }
}
