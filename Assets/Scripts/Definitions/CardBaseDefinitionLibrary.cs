using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardCategoryInfo
{
    public string name;
    public CardCategory category;

    public string displayName;
    public Sprite sprite;

    public CardCategoryInfo(CardCategory _category)
    {
        name = _category.ToString();
        category = _category;
    }
}

[CreateAssetMenu(fileName = "CardBaseDefinitionsLibrary", menuName = "LD45/Create card base definitions library")]
public class CardBaseDefinitionLibrary : ScriptableObject
{
    public List<CardCategoryInfo> categoriesInfo = new List<CardCategoryInfo>();
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
                    found = true;
                }
            }
            if (!found)
            {
                newCategoriesInfoList.Add(new CardCategoryInfo((CardCategory)type));
            }
        }
        categoriesInfo = newCategoriesInfoList;

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
