using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFeedbackHandler : MonoBehaviour
{
    CardFeedbackElement[] elements;

    //// Start is called before the first frame update
    //protected virtual void Start()
    //{
    //    GetElements();
    //}

    void GetElements()
    {
        if (elements == null)
        {
            elements = GetComponentsInChildren<CardFeedbackElement>();
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].gameObject.SetActive(false);
            }
        }
    }

    protected void InitElements(CardDefinitionType cardType)
    {
        GetElements();
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].UpdateForCard(cardType);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
