using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : UnitController
{
    [SerializeField] protected GameObject TrackedUnit;
    [SerializeField] protected bool needToFaceTrackedUnit = false;
    [SerializeField] protected bool moveRight = true;
    [SerializeField] protected float AggroRange = 30;//range for unit to start tracking a target
    [SerializeField] protected bool tracking = false; //whether this unit is tracking a target
    [SerializeField] protected bool moving = false;
    [SerializeField] protected Skill queuedSkill = Skill.Default;
    [SerializeField] protected int totalSkillWeight = 0; //weight for which skills are being used 
    [SerializeField] protected float CooldownMultiplier = 1;//multiplier for cooldowns

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CooldownMultiplier = (float )Random.Range(5, 15) / 10;
        unitType = UnitType.Enemy;
    }

    protected override void Update()
    {
        base.Update();

        if (!inAction)
        {
            //set animation to moving if unit is moving
            if (moving)
            {
                Walk();
            }
            else
            {
                stopMoving();
            }
            //set animation to idle if unit is not moving
            if (Grounded && rigidbody.velocity.x != 0)
            {
                GetComponent<Animator>().Play("Idle");
            }
            //if a skill is queued and is tracking, start track process
            if (tracking)
            {
                TrackUnit();

            }
            //if not pick a new action
            else
            {
                pickAction();
            }
        }
    }

    protected abstract void startTriggeredAction(Skill s); //method to use a skill when unit is in range to use it



    #region Tracking
    /// <summary>
    /// tracks unit targeted by skill.
    /// moves towards units if it is in aggro range
    /// </summary>
    protected virtual void TrackUnit()
    {
        float trackingDistance = Mathf.Abs(getUnitDistance(TrackedUnit));
        //if unit is in aggro range
        if (trackingDistance <= AggroRange)
        {
            //if target is within skill distance, use it
            if (trackingDistance <= queuedSkill.Range && !queuedSkill.Equals(Skill.Default))
            {

                stopMoving();

                if (!inAction)
                {
                    if (TrackedUnit)
                        FaceTarget();
                    startTriggeredAction(queuedSkill);
                    tracking = false;
                   // pickAction();
                }
            }
            //if not move towards target
            else {
                setDirectionTarget(TrackedUnit);
                if (!moving)
                {
                    startMoving();
                }
            }
        }
    }
    /// <summary>
    /// sets a unit to be tracked and sets a range to which the skill needs to be in to be used
    /// </summary>
    /// <param name="g">unit to be tracked</param>
    protected void SetTrackedUnit(GameObject g)
    {
        tracking = true;
        TrackedUnit = g;

    }
    #endregion

    #region Skills
    /// <summary>
    /// queue skill to be used 
    /// </summary>
    /// <param name="s">skill to be queued</param>
    protected void QueueSkill(Skill s)
    {
        queuedSkill = s;
    }

    /// <summary>
    /// picks an action based on cooldowns of skills
    /// </summary>
    protected virtual void pickAction()
    {
        queuedSkill = Skill.Default;
        for (int i = 0; i < SkillPool.Count; i++)
        {
            if (SkillCooldowns[i] <= 0)
            {
                if (SkillPool[i].TargetAlly && ClosestAlly != null || !SkillPool[i].TargetAlly && ClosestEnemy != null)
                {
                    //queue for action based on skill selected
                    QueueSkill(SkillPool[i]);
                    //picks a target
                    GameObject target;
                    if (queuedSkill.TargetAlly)
                        target = ClosestAlly;
                    else
                        target = ClosestEnemy;
                    //start tracking target
                    SetTrackedUnit(target);

                    SkillCooldowns[i] = SkillPool[i].Cooldown * CooldownMultiplier;
                    break;
                }
            }
        }
        //if (SkillPool.Count > 0&&SkillCooldowns.Count>0)
        //{
        //    bool pickedSkill = false;
        //    while (!pickedSkill)
        //    {
        //        int index = Random.Range(0, SkillPool.Count - 1);
        //        if (SkillCooldowns[index] <= 0)
        //        {
        //            if (SkillPool[index].TargetAlly && ClosestAlly != null || !SkillPool[index].TargetAlly && ClosestEnemy != null)
        //            {
        //                //queue for action based on skill selected
        //                QueueSkill(SkillPool[index]);
        //                //picks a target
        //                GameObject target;
        //                if (queuedSkill.TargetAlly)
        //                    target = ClosestAlly;
        //                else
        //                    target = ClosestEnemy;
        //                //start tracking target
        //                SetTrackedUnit(target);

        //                SkillCooldowns[index] = SkillPool[index].Cooldown * CooldownMultiplier;

        //            }
        //            pickedSkill = true;
        //        }

        //    }
        //}

    }
    #endregion
    #region Moving
    /// <summary>
    /// sets this unit's direction towards a target to face it
    /// </summary>
    /// <param name="g"></param>
    protected virtual void setDirectionTarget(GameObject g)
    {
        float direction = transform.position.x - g.transform.position.x;

        if (direction < 0)
        {
            moveRight = true;

        }
        else
        { 
            moveRight = false;
        }
    }

    /// <summary>
    /// changes velocity and direction to start walking
    /// </summary>
    protected virtual void Walk()
    {
        if(!inAction)
        {
            if(moveRight)
            {
                FlipCharacter(true);
                rigidbody.velocity = new Vector2(walkSpeed, rigidbody.velocity.y);
            }
            else
            {
                FlipCharacter(false);
                rigidbody.velocity = new Vector2(-walkSpeed, rigidbody.velocity.y);
            }
        }
    }

    /// <summary>
    /// turns to face the tracked target
    /// </summary>
    protected void FaceTarget()
    {
        Transform target = TrackedUnit.transform;
        if (target.position.x < transform.position.x)
        {
            FlipCharacter(false);
            moveRight = false;
        }
        else
        {
            FlipCharacter(true);
            moveRight = true;
        }
    }

    /// <summary>
    /// below 2 functions are to tell the unit to move towards target or not
    /// </summary>
    protected void startMoving()
    {

        moving = true;
        GetComponent<Animator>().Play("Walk");
    }
    protected void stopMoving()
    {

        moving = false;
    }
    #endregion
}
