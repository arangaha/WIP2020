using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonController : EnemyController
{
    [SerializeField] protected WeaponColliderController FrontClaw;
    [SerializeField] protected WeaponColliderController BackClaw;
    protected Animator animator;
    bool Charging1 = false;
    bool Charging2 = false;
    bool Charging3 = false;
    int Charge1Speed = 5;
    int Charge2Speed = 20;
    int Charge3Speed = 30;

    GameObject BurstPrefab;
    GameObject FlamePrefab;
    protected override void Start()
    {
        HealthbarOffsetY = 8;
        //  xScale = 0.7f;
        base.Start();

        animator = GetComponent<Animator>();
        restTimer = 2;
        SkillPool.Add(Skill.DemonNA);
        SkillPool.Add(Skill.DemonPunch);
        SkillPool.Add(Skill.DemonGroundBurst);
        SkillPool.Add(Skill.DemonFlameBreath);
        for (int i = 0; i < SkillPool.Count; i++)
        {
            SkillCooldowns.Add(0);
        }

        xScale = 0.5f;

        FrontClaw.WeaponCollisionEnterEvent.AddListener(HitTarget);
        BackClaw.WeaponCollisionEnterEvent.AddListener(HitTarget);
        BurstPrefab = Resources.Load("Prefabs/AoE/DemonGroundBurst") as GameObject;
        FlamePrefab = Resources.Load("Prefabs/AoE/DemonFlameBreath") as GameObject;
    }

    protected override void Update()
    {
        base.Update();
        if (Charging1 || Charging2 || Charging3)
        {
            int chargeSpeed = 0;
            if (Charging1)
            {
                chargeSpeed = Charge1Speed;
            }
            else if (Charging2)
            {
                chargeSpeed =  Charge2Speed;
            }
            else if (Charging3)
            {
                chargeSpeed = Charge3Speed;
            }
            if (Mathf.Abs(getUnitDistance(TrackedUnit)) > 3)
                if (moveRight)
                {
                    rigidbody.velocity = new Vector2(chargeSpeed, 0);
                }
                else
                    rigidbody.velocity = new Vector2(-chargeSpeed, 0);

        }
        else
            StopCharging();
    }
    protected override void startTriggeredAction(Skill s)
    {
        if (s == Skill.DemonNA)
            NormalAttack();
        else if (s == Skill.DemonPunch)
            PunchAttack();
        else if (s == Skill.DemonFlameBreath)
            FlameBreath();
        else if (s == Skill.DemonGroundBurst)
            GroundSlam();
    }

    void StartCharging1()
    {
        Charging1 = true;
        Charging2 = false;
        Charging3 = false;
    }
    void StartCharging2()
    {
        Charging1 = false;
        Charging2 = true;
        Charging3 = false;

    }
    void StartCharging3()
    {
        Charging1 = false;
        Charging2 = false;
        Charging3 = true;

    }

    void StopCharging()
    {
        Charging1 = false;

        Charging2 = false;
        Charging3 = false;

        rigidbody.velocity = new Vector2(0, 0);
    }
    public override void Death()
    {
        inAction = true;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        animator.Play("Death");
        StartCoroutine(DeathDecay());
    }

    #region Normal Attack
    void NormalAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.DemonNA;

            animator.Play("NormalAttack");

    }

    protected void finishNormalAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
    }
    #endregion

    #region punch attack
    void PunchAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.DemonPunch;
        if(getUnitDistance(TrackedUnit)<10)
        {
            animator.Play("PunchAttack");
        }
        else
        {
            animator.Play("LeapPunchAttack");
        }
    }
    #endregion

    #region ground smash
    void GroundSlam()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.DemonGroundBurst;
        if (getUnitDistance(TrackedUnit) < 6)
        {
            animator.Play("GroundSmash");
        }
        else
        {
            animator.Play("LeapGroundSmash");
        }
    }

    void CreateGroundBurst()
    {
        var instance = Instantiate(BurstPrefab);
        instance.transform.position = transform.position;
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        Skill waveskill = Skill.DemonGroundBurst;
        instance.GetComponent<AreaEffectControl>().SetUp(waveskill.Amount * GetComponent<UnitStats>().DamageMulti(), 0, 0, gameObject);

    }
    #endregion

    #region flame breath
    void FlameBreath()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.DemonFlameBreath;
        animator.Play("FlameBreath");
    }

    void CreateFlameBreath()
    {
        var instance = Instantiate(FlamePrefab);
        instance.transform.position = transform.position;
        Skill waveskill = Skill.DemonFlameBreath;
        instance.GetComponent<DemonFlameBreath>().SetUp((int)(waveskill.Amount * GetComponent<UnitStats>().DamageMulti()), 1, gameObject);
        if(facingRight)
            instance.transform.localScale = new Vector3(-1,1,1);
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

    public void TurnDetectionOnFrontClaw()
    {
        FrontClaw.DetectionOn();
    }

    public virtual void TurnDetetectionOff()
    {
        FrontClaw.DetectionOff();
        BackClaw.DetectionOff();
    }

    #endregion

}



