using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsAmount : MonoBehaviour
{
    public TextMeshProUGUI amountLabel;

    // Start is called before the first frame update
    void Awake()
    {
        GlobalGameManager.OnNewCoinsAmount += OnNewCoinsAmount;
        OnNewCoinsAmount(0);
    }

    private void OnDestroy()
    {
        GlobalGameManager.OnNewCoinsAmount -= OnNewCoinsAmount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnNewCoinsAmount(int newCoinsAmount)
    {
        amountLabel.text = newCoinsAmount.ToString();
    }
}
