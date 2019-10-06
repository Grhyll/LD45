using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFeedbackElement : MonoBehaviour
{
    public List<CardDefinitionType> types;

    public void UpdateForCard(CardDefinitionType cardType)
    {
        gameObject.SetActive(types.Contains(cardType));
    }
}
