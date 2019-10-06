using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    Animator animator;

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
    
    void OnNewFightState(FightManager.FightState newFightState)
    {
        if (newFightState == FightManager.FightState.GameOverScreen)
        {
            animator.SetTrigger("Start");
        }
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
