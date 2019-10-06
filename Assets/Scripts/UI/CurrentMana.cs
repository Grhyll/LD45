using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentMana : MonoBehaviour
{
    public TextMeshProUGUI amountLabel;

    // Start is called before the first frame update
    void Awake()
    {
        FightManager.OnNewCurrentMana += OnNewCurrentMana;
        OnNewCurrentMana(0);
    }

    private void OnDestroy()
    {
        FightManager.OnNewCurrentMana -= OnNewCurrentMana;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnNewCurrentMana(int currentMana)
    {
        amountLabel.text = currentMana.ToString();
    }
}
