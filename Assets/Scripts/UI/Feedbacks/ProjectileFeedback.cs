using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFeedback : CardFeedbackHandler
{
    const float speed = 500f;
    const float castDuration = 0.5f;
    const float castOffset = 40f;

    float startDate;
    Vector3 startLocalPosition;
    Vector3 targetLocalPosition;
    Action onHitAction;
    Action onEndAction;

    public void FireProjectile(Card owner, FightCreature caster, GridSpot target, Action onHit, Action onEnd)
    {
        onHitAction = onHit;
        onEndAction = onEnd;

        InitElements(owner.cardDefinition.cardType);
        transform.position = caster.transform.position;
        transform.localScale = Vector3.one * 0.0001f;

        startDate = Time.time;
        startLocalPosition = transform.localPosition;
        targetLocalPosition = transform.parent.InverseTransformPoint(target.transform.position);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Time.time - startDate <= castDuration)
        {
            float castProgress = (Time.time - startDate) / castDuration;
            castProgress = 1f - (1f - castProgress) * (1f - castProgress);
            transform.localPosition = Vector3.Lerp(startLocalPosition, startLocalPosition + Vector3.up * castOffset, castProgress);
            transform.localScale = Vector3.one * castProgress;
        }
        else
        {
            Vector3 toTarget = targetLocalPosition - transform.localPosition;
            float distance = toTarget.magnitude;
            transform.up = toTarget;
            transform.localScale = Vector3.one;
            if (distance < Time.deltaTime * speed)
            {
                onHitAction();
                onEndAction();
                Destroy(gameObject);
            }
            else
            {
                transform.localPosition = transform.localPosition + toTarget * Time.deltaTime * speed / distance;
            }
        }
    }
}
