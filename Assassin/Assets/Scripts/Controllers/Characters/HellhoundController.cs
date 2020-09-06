using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellhoundController : EnemyController
{
    [SerializeField] WeaponColliderController FrontClaw;
    [SerializeField] WeaponColliderController BackClaw;
    [SerializeField] Transform Head;
    [SerializeField] GameObject FireballPrefab;
    [SerializeField] GameObject AngryEyes;
    float LeapSpeed = 0;
    float SprintSpeed = 10;
    float BaseSprintSpeed = 10;

    Animator animator;
    [SerializeField] bool EnrageAnim = false; //whether engrage animation has been played
    [SerializeField] bool Enraged = false;

    protected override void Start()
    {
      //  xScale = 0.7f;
        base.Start();

        animator = GetComponent<Animator>();
       BasewalkSpeed =  walkSpeed = 5;
       BaseSprintSpeed = SprintSpeed = 12;
        LeapSpeed = 22;

        FrontClaw.WeaponCollisionEnterEvent.AddListener(HitTarget);
        BackClaw.WeaponCollisionEnterEvent.AddListener(HitTarget);
        SkillPool.Add(Skill.HellHoundLeap);
        SkillPool.Add(Skill.HellHoundFireball);
        SkillPool.Add(Skill.HellHoundNA);
        for (int i = 0; i < SkillPool.Count; i++)
        {
            SkillCooldowns.Add(0);
        }

        FireballPrefab = Resources.Load("Prefabs/Projectiles/FireBall") as GameObject;
    }


    protected override void Update()
    {
        base.Update();
        if (statController.PercentageHealth() < 0.35f && !Enraged)
        {
            Enraged = true;

        }
    }

    protected override void pickAction()
    {
        if(Enraged&&!EnrageAnim)
        {
            chaseTimer = 0;
            restTimer = 1;
            exhaustCount = 4;
            currentExhaustCount = 4;
            EnrageRoar();
            EnrageAnim = true;
        }
        else
        base.pickAction();
    }

    protected override void startTriggeredAction(Skill s)
    {
        if (s == Skill.HellHoundNA)
            NormalAttack();
        else if (s == Skill.HellHoundFireball)
            FireBallAttack();
        else if (s == Skill.HellHoundLeap)
            LeapAttack();
    }
    #region Normal Attack
    void NormalAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.HellHoundNA;
        if(Enraged)
            animator.Play("SprintNormalAttack");
        else
            animator.Play("NormalAttack");

    }

    void finishNormalAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
    }
    #endregion
    #region FireBall

  protected virtual void FireBallAttack()
    {
        currentSkill = Skill.HellHoundFireball;
        inAction = true;
        isAttacking = true;
        if (Enraged)
            animator.Play("QuickFireBall");
        else
            animator.Play("FireBall");

    }

    protected virtual void ShootFireBall()
    {
        Vector3 startingLocation = Head.position;
        ThrowProjectile(FireballPrefab, startingLocation);

    }

    void finishFireBallAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
    }
    #endregion

    #region Leap Attack
    void LeapAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.HellHoundLeap;
        if (Enraged)
            animator.Play("SprintLeap");
        else
            animator.Play("Leap");
    }

    void finishLeapAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

    void StartLeap()
    {
        if (moveRight)
        {
            rigidbody.velocity = new Vector2(LeapSpeed, rigidbody.velocity.y);
        }
        else
        {
            rigidbody.velocity = new Vector2(-LeapSpeed, rigidbody.velocity.y);
        }
        rigidbody.AddForce(new Vector2(0, 700));
    }

    void EndLeap()
    {
        if (moveRight)
        {
            rigidbody.velocity = new Vector2(walkSpeed, rigidbody.velocity.y);
        }
        else
        {
            rigidbody.velocity = new Vector2(-walkSpeed, rigidbody.velocity.y);
        }
    }
    #endregion

    #region Enrage Roar
    void EnrageRoar()
    {
        inAction = true;
        animator.Play("Enrage");
    }

    void EnrageBuffs()
    {
        AngryEyes.SetActive(true);
        statController.GainArmor(statController.MaxHealth() * 0.2f);
    }

    void FinishRoar()
    {
        inAction = false;
    }
    #endregion

    #region detection
    public void TurnDetectionOnBothClaws()
    {
        FrontClaw.DetectionOn();
        BackClaw.DetectionOn();
    }

    public void TurnDetectionOnBackClaw()
    {
        BackClaw.DetectionOn();
    }

    public void TurnDetetectionOff()
    {
        FrontClaw.DetectionOff();
        BackClaw.DetectionOff();
    }

    #endregion

    #region movement 
    protected override void startMoving()
    {
        if(!Enraged)
        base.startMoving();
        else
        {
            moving = true;
            GetComponent<Animator>().Play("Sprint");
        }
    }

    protected override void Walk()
    {
        if(!Enraged)
        base.Walk();
        else
        {
            if (!inAction)
            {
                if (moveRight)
                {
                    FlipCharacter(true);
                    rigidbody.velocity = new Vector2(SprintSpeed, rigidbody.velocity.y);
                }
                else
                {
                    FlipCharacter(false);
                    rigidbody.velocity = new Vector2(-SprintSpeed, rigidbody.velocity.y);
                }
            }
        }
    }

    public override void NormalSpeed()
    {
        base.NormalSpeed();
        SprintSpeed = BaseSprintSpeed;
    }

    public override void SlowSpeed()
    {
        base.SlowSpeed();
        SprintSpeed = 0.6f * BaseSprintSpeed;
    }
    #endregion
    public override void Death()
    {
        inAction = true;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        animator.Play("Death");
        StartCoroutine(DeathDecay());
    }
}
