using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FightCreature : GridEntity
{
    public Image creatureImage;
    public TextMeshProUGUI healthLabel;
    public TextMeshProUGUI attackLabel;

    public int health { get; private set; }
    public int damage { get; private set; }
    public int range { get; private set; }
    public int moves { get; private set; }

    public bool isAlly { get; private set; }
    public bool isMC { get; private set; }

    public GridSpot currentGridSpot { get; private set; }
    public int turnRemainingMoves { get; private set; }

    public Card card { get; set; }

    List<GridSpot> currentPath = new List<GridSpot>();

    List<FightCreature> attackTargets = new List<FightCreature>();

    float currentMoveTimer = -1f;
    const float oneMoveDuration = 0.5f;

    float currentAttackStartDate;
    const float attackDuration = 0.5f;

    public void Init(Card creatureCard, GridSpot initialSpot, bool ally)
    {
        health = creatureCard.Health;
        damage = creatureCard.Damage;
        range = creatureCard.Range;
        moves = creatureCard.Moves;

        creatureImage.sprite = creatureCard.cardDefinition.sprite;
        float spriteScale = creatureCard.cardDefinition.scale;
        if (spriteScale < 0.1f)
            spriteScale = 1f;
        creatureImage.transform.localScale = Vector3.one * creatureImage.transform.localScale.x * spriteScale;
        healthLabel.text = health.ToString();
        attackLabel.text = damage.ToString();

        isAlly = ally;

        isMC = creatureCard.cardDefinition.cardType == CardDefinitionType.MC;

        card = creatureCard;

        currentGridSpot = initialSpot;

        turnRemainingMoves = moves;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPath.Count > 0)
        {
            transform.position = transform.position + Mathf.Clamp01(Time.deltaTime / currentMoveTimer) * (currentPath[0].transform.position - transform.position);
            currentMoveTimer -= Time.deltaTime;
            if (currentMoveTimer <= 0)
            {
                currentPath.RemoveAt(0);
                StartPathStep();
            }
        }

        if (attackTargets.Count > 0)
        {
            float currentAttackProgress = Mathf.Clamp01((Time.time - currentAttackStartDate) / attackDuration);
            float toTarget = (0.5f - Mathf.Abs(0.5f - currentAttackProgress)) * 2f;
            toTarget *= toTarget;
            transform.position = Vector3.Lerp(currentGridSpot.transform.position, attackTargets[0].transform.position, toTarget);
            if (currentAttackProgress > 0.5f && Mathf.Clamp01((Time.time - Time.deltaTime - currentAttackStartDate) / attackDuration) <= 0.5f)
            {
                attackTargets[0].TakeDamage(damage);
            }
            if (currentAttackProgress >= 1f)
            {
                attackTargets[0].ProcessHealth();
                attackTargets.RemoveAt(0);
                while (attackTargets.Count > 0 && (attackTargets[0] == null || !attackTargets[0].CanBeTargeted()))
                {
                    attackTargets.RemoveAt(0);
                }
                if (attackTargets.Count > 0)
                {
                    StartNextAttack();
                }
            }
        }
    }

    public void ReapReward()
    {
        GlobalGameManager.Instance.CoinsAmount += card.cardDefinition.goldReward;
        PickupDefinition earnedPickup = null;
        if (GlobalGameManager.Instance.tutoManager.CurrentTutoStep == TutoManager.TutoStep.ToCardCrafting &&
            GlobalGameManager.Instance.pickupCollection.GetCurrentPickupsAmount() == 0)
        {
            earnedPickup = new PickupCategoryDefinition(CardCategory.Creature);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (Random.Range(0f, 1f) < 0.6f)
                {
                    if (Random.Range(0f, 1f) < 0.3f)
                    {
                        System.Array categories = System.Enum.GetValues(typeof(CardCategory));
                        int categoriesAmount = categories.Length;
                        CardCategory category = CardCategory.None;
                        while (category == CardCategory.None)
                        {
                            category = (CardCategory)categories.GetValue(Random.Range(0, categoriesAmount));
                        }
                        earnedPickup = new PickupCategoryDefinition(category);
                    }
                    else
                    {
                        System.Array effects = System.Enum.GetValues(typeof(PickupEffect));
                        int effectsAmount = effects.Length;
                        PickupEffect effect = PickupEffect.None;
                        while (effect == PickupEffect.None)
                        {
                            effect = (PickupEffect)effects.GetValue(Random.Range(0, effectsAmount));
                        }
                        earnedPickup = new PickupEffectDefinition(effect);
                    }
                }
            }
        }
        if(earnedPickup != null)
            GlobalGameManager.Instance.pickupCollection.EarnPickup(earnedPickup);
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        healthLabel.text = health.ToString();
    }
    public void ProcessHealth() 
    {
        if (health <= 0)
        {
            FightManager.instance.OnDeadCreature(this);
        }
    }
    public bool CanBeTargeted()
    {
        return health > 0;
    }

    public void OnNewTurn()
    {
        turnRemainingMoves = moves;
    }

    public void GoTo(GridSpot destination, PlayGrid.MoveSet moveSet)
    {
        if(moveSet.GetPath(destination, ref currentPath))
        {
            turnRemainingMoves -= currentPath.Count;
            GlobalGameManager.Instance.OnBusyElement(this);
            StartPathStep();
        }
    }
    public bool TryAndMoveAI(PlayGrid.MoveSet moveSet)
    {
        if (turnRemainingMoves > 0)
        {
            FightCreature target = FightManager.instance.GetClosestOpponentCreature(this);
            if (target != null)
            {
                GridSpot destination = moveSet.GetClosestSpot(target.currentGridSpot, turnRemainingMoves);
                if (destination != currentGridSpot)
                {
                    GoTo(destination, moveSet);
                    return true;
                }
            }
        }
        return false;
    }
    void StartPathStep()
    {
        if (currentPath.Count > 0)
        {
            currentMoveTimer = oneMoveDuration;
            currentGridSpot.OnEntityLeave(this);
            currentPath[0].OnEntityEnters(this);
            currentGridSpot = currentPath[0];
        }
        else
        {
            GlobalGameManager.Instance.tutoManager.OnTutoEvent(TutoManager.TutoStep.JustMoved);
        }
    }

    public bool Attack()
    {
        attackTargets.Clear();
        for (int i = 1; i <= range; i++)
        {
            int passSpotsAmount = i * 2;
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < passSpotsAmount; k++)
                {
                    int x = currentGridSpot.CoordX;
                    int y = currentGridSpot.CoordY;
                    switch (j)
                    {
                        case 0:
                            x += k - i + 1;
                            y += i;
                            break;
                        case 1:
                            x += i;
                            y -= k - i + 1;
                            break;
                        case 2:
                            x -= k - i + 1;
                            y -= i;
                            break;
                        case 3:
                            x -= i;
                            y += k - i + 1;
                            break;
                    }
                    GridSpot spot = PlayGrid.Instance.GetSpot(x, y);
                    if (spot != null &&
                        spot.gridEntity != null && spot.gridEntity is FightCreature &&
                        (spot.gridEntity as FightCreature).isAlly != isAlly)
                    {
                        attackTargets.Add(spot.gridEntity as FightCreature);
                    }
                }
            }
        }
        if (attackTargets.Count > 0)
        {
            GlobalGameManager.Instance.OnBusyElement(this);
            StartNextAttack();
            return true;
        }
        return false;
    }
    void StartNextAttack()
    {
        currentAttackStartDate = Time.time;
    }

    public override bool IsBusy()
    {
        return base.IsBusy() || currentPath.Count > 0 || attackTargets.Count > 0;
    }
    public override GridSpot.GridSpotVisualState GetSpotVisualStateMask()
    {
        if (IsBusy())
        {
            return 0;
        }

        GridSpot.GridSpotVisualState result = isAlly ? (turnRemainingMoves > 0 ? GridSpot.GridSpotVisualState.AllyRemainingMove : GridSpot.GridSpotVisualState.AllyNoRemainingMove)
            : GridSpot.GridSpotVisualState.Enemy;
        return result | base.GetSpotVisualStateMask();
    }
}
