using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentMana : MonoBehaviour
{
    public TextMeshProUGUI amountLabel;
    public GameObject notEnoughManaLabel;

    float notEnoughManaTimer;
    const float notEnoughManaDuration = 2f;

    // Start is called before the first frame update
    void Awake()
    {
        FightManager.OnNewCurrentMana += OnNewCurrentMana;
        FightManager.OnNotEnoughMana += OnNewEnoughMana;
        OnNewCurrentMana(0);
    }

    private void OnEnable()
    {
        notEnoughManaLabel.SetActive(false);
    }

    private void OnDestroy()
    {
        FightManager.OnNewCurrentMana -= OnNewCurrentMana;
        FightManager.OnNotEnoughMana -= OnNewEnoughMana;
    }

    // Update is called once per frame
    void Update()
    {
        if (notEnoughManaTimer > 0)
        {
            notEnoughManaTimer -= Time.deltaTime;
            notEnoughManaLabel.SetActive(notEnoughManaTimer > 0 && (notEnoughManaTimer > notEnoughManaDuration * 0.6f || notEnoughManaTimer < notEnoughManaDuration * 0.4f));
        }
    }

    void OnNewCurrentMana(int currentMana)
    {
        amountLabel.text = currentMana.ToString();
    }
    void OnNewEnoughMana()
    {
        notEnoughManaTimer = notEnoughManaDuration;
    }
}
