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

    public int ManaCost { get { return cardDefinition.manaCost + manaCostBonus; } }

    public int Health { get { return cardDefinition.health; } }
    public int Damage { get { return cardDefinition.damage + damageBonus; } }
    public int Range { get { return cardDefinition.range; } }
    public int Moves { get { return cardDefinition.moves; } }

    GridSpot castTarget;

    const string damageKey = "{damage}";

    public string GetDescription()
    {
        string result = cardDefinition.description;
        result = result.Replace(damageKey, Damage.ToString());
        if (cardDefinition.cardCategory == CardCategory.Creature)
        {
            result += "\nMoves per turn: " + Moves.ToString();
            result += "\nAttack range: " + Range.ToString();
        }
        return result;
    }

    public void Cast(GridSpot spot)
    {
        switch (cardDefinition.cardType)
        {
            case CardDefinitionType.Fireball:
                castTarget = spot;
                GlobalGameManager.Instance.OnBusyElement(this);
                PlayGrid.Instance.gridFeedbackManager.FireProjectile(this, PlayGrid.Instance.mc, spot, OnFeedbackHit, OnFeedbackEnd);
                break;


            default:
                Debug.LogError("Error: card type " + cardDefinition.cardType + " isn't handled in Cast method.");
                break;
        }
    }
    void OnFeedbackHit()
    {
        // Apply effect
        switch (cardDefinition.cardType)
        {
            case CardDefinitionType.Fireball:
                if (castTarget.gridEntity != null && castTarget.gridEntity is FightCreature)
                {
                    (castTarget.gridEntity as FightCreature).TakeDamage(Damage);
                }
                break;


            default:
                Debug.LogError("Error: card type " + cardDefinition.cardType + " isn't handled in OnFeedbackHit callback.");
                break;
        }
    }
    void OnFeedbackEnd()
    {
        if (castTarget.gridEntity != null && castTarget.gridEntity is FightCreature)
        {
            (castTarget.gridEntity as FightCreature).ProcessHealth();
        }
        castTarget = null;
        FightManager.instance.OnCardCastEnded();
    }

    public bool IsBusy()
    {
        return castTarget != null;
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
