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

    public int coinsGain = 1;

    public GridSpot currentGridSpot { get; private set; }
    public int turnRemainingMoves { get; private set; }

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
        healthLabel.text = health.ToString();
        attackLabel.text = damage.ToString();

        isAlly = ally;

        isMC = creatureCard.cardDefinition.cardType == CardDefinitionType.MC;

        currentGridSpot = initialSpot;
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
                attackTargets[0].ProcessDamageTaken();
                attackTargets.RemoveAt(0);
                while (attackTargets.Count > 0 && !attackTargets[0].CanBeTargeted())
                {
                    attackTargets.RemoveAt(0);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        healthLabel.text = health.ToString();
    }
    public bool ProcessDamageTaken()    // Returns true if there's a feedback to play stopping the game
    {
        if (health <= 0)
        {
            GlobalGameManager.Instance.fightManager.OnDeadCreature(this);
        }
        return false;
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
        FightCreature target = GlobalGameManager.Instance.fightManager.GetClosestOpponentCreature(this);
        if (target != null)
        {
            GridSpot destination = moveSet.GetClosestSpot(target.currentGridSpot, turnRemainingMoves);
            if (destination != currentGridSpot)
            {
                GoTo(destination, moveSet);
                return true;
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
                    GridSpot spot = GlobalGameManager.Instance.grid.GetSpot(x, y);
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
