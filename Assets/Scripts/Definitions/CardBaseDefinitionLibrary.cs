using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardCategoryInfo
{
    [HideInInspector]
    public string name;
    [HideInInspector]
    public CardCategory category;

    public string displayName;
    public Sprite sprite;

    public string pickupDescription;

    public CardCategoryInfo(CardCategory _category)
    {
        name = _category.ToString();
        category = _category;
    }
}
[System.Serializable]
public class PickupEffectInfo
{
    [HideInInspector]
    public string name;
    [HideInInspector]
    public PickupEffect effect;

    public string displayName;
    public Sprite sprite;
    public Color color;

    public string pickupDescription;

    public PickupEffectInfo(PickupEffect _effect)
    {
        name = _effect.ToString();
        effect = _effect;
    }
}

[CreateAssetMenu(fileName = "CardBaseDefinitionsLibrary", menuName = "LD45/Create card base definitions library")]
public class CardBaseDefinitionLibrary : ScriptableObject
{
    public List<CardCategoryInfo> categoriesInfo = new List<CardCategoryInfo>();
    public List<PickupEffectInfo> pickupEffectsInfo = new List<PickupEffectInfo>();
    public List<CardBaseDefinition> cardDefinitions = new List<CardBaseDefinition>();

    private void OnEnable()
    {
        List<CardCategoryInfo> newCategoriesInfoList = new List<CardCategoryInfo>();
        foreach (int type in System.Enum.GetValues(typeof(CardCategory)))
        {
            bool found = false;
            for (int i = 0; i < categoriesInfo.Count; i++)
            {
                if (((int)categoriesInfo[i].category) == type)
                {
                    newCategoriesInfoList.Add(categoriesInfo[i]);
                    categoriesInfo[i].name = ((CardCategory)type).ToString();
                    found = true;
                }
            }
            if (!found)
            {
                newCategoriesInfoList.Add(new CardCategoryInfo((CardCategory)type));
            }
        }
        categoriesInfo = newCategoriesInfoList;

        List<PickupEffectInfo> newPickupEffectsInfoList = new List<PickupEffectInfo>();
        foreach (int type in System.Enum.GetValues(typeof(PickupEffect)))
        {
            bool found = false;
            for (int i = 0; i < pickupEffectsInfo.Count; i++)
            {
                if (((int)pickupEffectsInfo[i].effect) == type)
                {
                    newPickupEffectsInfoList.Add(pickupEffectsInfo[i]);
                    found = true;
                }
            }
            if (!found)
            {
                newPickupEffectsInfoList.Add(new PickupEffectInfo((PickupEffect)type));
            }
        }
        pickupEffectsInfo = newPickupEffectsInfoList;

        List<CardBaseDefinition> newCardDefinitionsList = new List<CardBaseDefinition>();
        foreach (int type in System.Enum.GetValues(typeof(CardDefinitionType)))
        {
            bool found = false;
            for (int i = 0; i < cardDefinitions.Count; i++)
            {
                if (((int)cardDefinitions[i].cardType) == type)
                {
                    newCardDefinitionsList.Add(cardDefinitions[i]);
                    found = true;
                }
            }
            if (!found)
            {
                newCardDefinitionsList.Add(new CardBaseDefinition((CardDefinitionType)type));
            }
        }
        cardDefinitions = newCardDefinitionsList;
    }

    public CardCategoryInfo GetCategoryInfo(CardCategory category)
    {
        for (int i = 0; i < categoriesInfo.Count; i++)
        {
            if (categoriesInfo[i].category == category)
            {
                return categoriesInfo[i];
            }
        }
        Debug.LogError("Error: couldn't find category info for " + category + " in card definitions library.");
        return null;
    }


    public PickupEffectInfo GetPickupEffectInfo(PickupEffect effect)
    {
        for (int i = 0; i < pickupEffectsInfo.Count; i++)
        {
            if (pickupEffectsInfo[i].effect == effect)
            {
                return pickupEffectsInfo[i];
            }
        }
        Debug.LogError("Error: couldn't find pickup effect info for " + effect + " in card definitions library.");
        return null;
    }

    public CardBaseDefinition GetCardDefinition(CardDefinitionType cardType)
    {
        for (int i = 0; i < cardDefinitions.Count; i++)
        {
            if (cardDefinitions[i].cardType == cardType)
            {
                return cardDefinitions[i];
            }
        }
        Debug.LogError("Error: couldn't find card definition for type " + cardType + " in card definitions library.");
        return null;
    }
}
