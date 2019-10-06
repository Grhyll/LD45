using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Card : IGameElement
{
    public CardBaseDefinition cardDefinition { get; private set; }

    int manaCostBonus = 0;

    int healthBonus = 0;
    int damageBonus = 0;
    int movesBonus = 0;

    public Card(CardDefinitionType _cardDefinitionType)
    {
        cardDefinition = GlobalGameManager.Instance.cardBaseDefinitionsLibrary.GetCardDefinition(_cardDefinitionType);
    }

    public int ManaCost { get { return cardDefinition.manaCost + manaCostBonus; } }

    public int Health { get { return cardDefinition.health + healthBonus; } }
    public int Damage { get { return cardDefinition.damage + damageBonus; } }
    public int Range { get { return cardDefinition.range; } }
    public int Moves { get { return cardDefinition.moves + movesBonus; } }

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
        switch (cardDefinition.cardCategory)
        {
            case CardCategory.Creature:
                FightManager.instance.SpawnCreature(this, true, spot);
                OnCardFinishedCasting();
                break;

            case CardCategory.InstantSpell:
                switch (cardDefinition.cardType)
                {
                    case CardDefinitionType.Fireball:
                        castTarget = spot;
                        GlobalGameManager.Instance.OnBusyElement(this);
                        PlayGrid.Instance.gridFeedbackManager.FireProjectile(this, PlayGrid.Instance.mc, spot, OnFeedbackHit, OnCardFinishedCasting);
                        break;


                    default:
                        Debug.LogError("Error: card type " + cardDefinition.cardType + " isn't handled in Cast method.");
                        break;
                }
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
    void OnCardFinishedCasting()
    {
        if (castTarget != null && castTarget.gridEntity != null && castTarget.gridEntity is FightCreature)
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

    public bool AddBonus(PickupDefinition pickup, bool apply = false)
    {
        if (pickup.type == PickupType.Category)
        {
            CardCategory pickupCategory = (pickup as PickupCategoryDefinition).category;
            switch (pickupCategory)
            {
                // Applying a Creature pickup
                case CardCategory.Creature:
                    if (cardDefinition.cardCategory == CardCategory.Creature)
                    {
                        if (apply)
                        {
                            healthBonus += 1;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                // Applying an InstantSpell pickup
                case CardCategory.InstantSpell:
                    if (cardDefinition.cardCategory == CardCategory.Creature)
                    {
                        if (apply)
                        {
                            movesBonus += 1;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                default:
                    Debug.LogError("Pickup category " + pickupCategory + " not handled in AddBonus");
                    return false;
            }
        }
        else if (pickup.type == PickupType.Effect)
        {
            PickupEffect pickupEffect = (pickup as PickupEffectDefinition).effect;
            switch (pickupEffect)
            {
                // Applying a Creature pickup
                case PickupEffect.Damage:
                    if (cardDefinition.cardCategory == CardCategory.Creature)
                    {
                        if (apply)
                        {
                            damageBonus += 1;
                        }
                        return true;
                    }
                    else
                    {
                        switch (cardDefinition.cardType)
                        {
                            case CardDefinitionType.Fireball:
                                if (apply)
                                {
                                    damageBonus += 1;
                                }
                                return true;

                            default:
                                Debug.LogError("Pickup effect " + pickupEffect + " not handled for card type " + cardDefinition.cardType + " in AddBonus");
                                return false;

                        }
                    }

                default:
                    Debug.LogError("Pickup effect " + pickupEffect + " not handled in AddBonus");
                    return false;
            }
        }
        else
        {
            Debug.LogError("Wtf, what this pickup of type " + pickup.type + "?!");
        }
        return false;
    }

}
