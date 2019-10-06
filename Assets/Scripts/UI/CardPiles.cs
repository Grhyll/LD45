using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardPiles : MonoBehaviour
{
    public TextMeshProUGUI libraryLabel;
    public TextMeshProUGUI discardLabel;

    private void Awake()
    {
        FightManager.OnCardPilesChanged += OnCardPilesChanged;
    }

    private void OnDestroy()
    {
        FightManager.OnCardPilesChanged += OnCardPilesChanged;
    }

    void OnCardPilesChanged()
    {
        libraryLabel.text = "Pool: " + GlobalGameManager.Instance.fightManager.GetLibraryCount().ToString();
        discardLabel.text = "Discarded: " + GlobalGameManager.Instance.fightManager.GetDiscardPileCount().ToString();
    }
}
