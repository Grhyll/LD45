using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Card : IGameElement
{
    public CardBaseDefinition cardDefinition { get; private set; }

    int manaCostBonus = 0;

    int damageBonus = 0;

    public Card(CardDefinitionType _cardDefinitionType)
    {
        cardDefinition = GlobalGameManager.Instance.cardBaseDefinitionsLibrary.GetCardDefinition(_cardDefinitionType);
    }

    public int Health { get { return cardDefinition.health; } }
    public int Damage { get { return cardDefinition.damage + damageBonus; } }
    public int Range { get { return cardDefinition.range; } }
    public int Moves { get { return cardDefinition.moves; } }

    public void Cast(GridSpot spot)
    {
    }

    public bool IsBusy()
    {
        return false;
    }

    public bool IsValidTarget(GridSpot spot)
    {
        switch (cardDefinition.targettingType)
        {
            case TargettingType.AnySpot:
                return true;

            case TargettingType.AnyFreeSpot:
                return spot.IsFree();

            default:
                Debug.Log("Asking IsValidTarget for unhandled targetting type " + cardDefinition.targettingType);
                return false;
        }
    }
    //TargettingType GetTargetingType()
    //{
    //    switch (cardBase)
    //    {
    //        case CardDefinitionType.Fireball:
    //            return TargettingType.AnySpot;

    //        default:
    //            Debug.LogError("Error: card base " + cardBase + " isn't handled in GetTargetingType.");
    //            return TargettingType.AnySpot;
    //    }
    //}

}
