using System;
using System.Collections.Generic;
using UnityEngine;

public class FeedbacksManager : MonoBehaviour
{
    ProjectileFeedback projectileModel;

    // Start is called before the first frame update
    void Start()
    {
        projectileModel = GetComponentInChildren<ProjectileFeedback>();
        projectileModel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FireProjectile(Card owner, FightCreature caster, GridSpot target, Action onHit, Action onEnd)
    {
        ProjectileFeedback newProjectile = Instantiate(projectileModel, projectileModel.transform.parent).GetComponent<ProjectileFeedback>();
        newProjectile.gameObject.SetActive(true);
        newProjectile.FireProjectile(owner, caster, target, onHit, onEnd);
    }
}
