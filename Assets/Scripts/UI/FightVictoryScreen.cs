using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightVictoryScreen : MonoBehaviour
{
    public GameObject panel;

    Animator animator;

    bool waitingToStart = false;
    bool didStart = false;

    // Start is called before the first frame update
    void Start()
    {
        FightManager.OnNewFightState += OnNewFightState;
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        FightManager.OnNewFightState -= OnNewFightState;
    }

    // Update is called once per frame
    void Update()
    {
        if (panel.activeSelf && waitingToStart)
        {
            waitingToStart = false;
            didStart = true;
        }
        else if (!panel.activeSelf && didStart)
        {
            didStart = false;
            GlobalGameManager.Instance.OnFightVictoryScreenEnded();
        }
    }

    void OnNewFightState(FightManager.FightState newFightState)
    {
        if (newFightState == FightManager.FightState.FightVictoryScreen)
        {
            animator.SetTrigger("Start");
            waitingToStart = true;
            didStart = false;
        }
    }
}
