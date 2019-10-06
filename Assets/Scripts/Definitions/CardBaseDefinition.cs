using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CardCategory
{
    None = -1,
    Creature = 0,
    InstantSpell = 1,
}
public enum CardDefinitionType
{
    None = -1,

    // Special
    MC = 0,
    
    // Enemies
    BaseEnemy = 100,

    // Instant spells
    Fireball = 1000,

    // Player creatures
    SmallPlayerCreature = 2000,
    PlayerCreatureDamage = 2001,
}
public enum TargettingType
{
    AnySpot,
    AnyFreeSpot,
}

[System.Serializable]
public class CardBaseDefinition 
{
    [HideInInspector]
    public string name;

    [HideInInspector]
    public CardDefinitionType cardType;

    public string displayName;

    public CardCategory cardCategory;

    public TargettingType targettingType;

    public Sprite sprite;

    public string description;

    [Header("Generic")]
    public int damage = 0;

    [Header("Player playable cards.")]
    public int manaCost = 1;

    [Header("Enemies")]
    public int goldReward = 1;

    [Header("Creatures")]
    public int health = 0;
    public int range = 0;
    public int moves = 0;

    public CardBaseDefinition(CardDefinitionType type)
    {
        name = type.ToString();
        cardType = type;
    }

    public static CardDefinitionType GetCardDefinitionType(CardCategory category, PickupEffect effect1)
    {
        switch (category)
        {
            case CardCategory.Creature:
                switch (effect1)
                {
                    case PickupEffect.None:
                        return CardDefinitionType.SmallPlayerCreature;

                    case PickupEffect.Damage:
                        return CardDefinitionType.PlayerCreatureDamage;
                }
                break;

            case CardCategory.InstantSpell:
                switch (effect1)
                {
                    case PickupEffect.Damage:
                        return CardDefinitionType.Fireball;
                }
                break;
        }

        Debug.LogError("Error: couldn't find CardDefinitionType for combination " + category + " - " + effect1);
        return CardDefinitionType.None;
    }
}
