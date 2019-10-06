using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuildingManager : MonoBehaviour
{
    public EditedCard editedCard;

    // Start is called before the first frame update
    void Start()
    {
        editedCard.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
