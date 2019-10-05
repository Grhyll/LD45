using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardBaseDefinitionsLibrary", menuName = "LD45/Create card base definitions library")]
public class CardBaseDefinitionLibrary : ScriptableObject
{
    public List<CardBaseDefinition> cardDefinitions = new List<CardBaseDefinition>();

    private void OnEnable()
    {
        List<CardBaseDefinition> newList = new List<CardBaseDefinition>();
        foreach (int type in System.Enum.GetValues(typeof(CardDefinitionType)))
        {
            bool found = false;
            for (int i = 0; i < cardDefinitions.Count; i++)
            {
                if (((int)cardDefinitions[i].cardType) == type)
                {
                    newList.Add(cardDefinitions[i]);
                    found = true;
                }
            }
            if (!found)
            {
                newList.Add(new CardBaseDefinition((CardDefinitionType) type));
            }
        }
        cardDefinitions = newList;
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
