using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CardCategory
{
    Creature = 0,
    InstantSpell = 1,
}
public enum CardDefinitionType
{
    // Special
    MC = 0,

    // Enemies
    BaseEnemy = 100,

    // Instant spells
    Fireball = 1000,
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
}
