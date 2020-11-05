using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellhoundVariantController : HellhoundController
{
    [SerializeField] WeaponColliderController Jaw;
    [SerializeField] GameObject BigFireballPrefab;
    [SerializeField] GameObject SlamSpikesPrefab;
    float SlamPositionX = 0; // x position of target when a slam starts
    [SerializeField] HellHoundEliteStats variantStats;
    protected override void Start()
    {
        base.Start();

        Jaw.WeaponCollisionEnterEvent.AddListener(HitTarget);
        SkillPool = new List<Skill>();
        SkillCooldowns = new List<float>();


        SkillPool.Add(Skill.HellHoundVariantSlam);
        SkillPool.Add(Skill.HellHoundVariantFireball);
        SkillPool.Add(Skill.HellHoundVariantNA);

        for (int i = 0; i < SkillPool.Count; i++)
        {
            SkillCooldowns.Add(0);
        }

        BigFireballPrefab = Resources.Load("Prefabs/Projectiles/EliteFireBall") as GameObject;
        SlamSpikesPrefab = Resources.Load("Prefabs/AoE/SlamSpikes") as GameObject;

        EffectXOffset = 2;
        EffectWidth = 3;
    }



    public void TurnDetectionOnHead()
    {
        Jaw.DetectionOn();
    }


    public override void TurnDetetectionOff()
    {
        base.TurnDetetectionOff();
        Jaw.DetectionOff();
    }

    protected override void startTriggeredAction(Skill s)
    {
        if (s == Skill.HellHoundVariantNA)
            VariantNormalAttack();
        else if (s == Skill.HellHoundVariantFireball)
            BigFireBallAttack();
        else if (s == Skill.HellHoundVariantLeap)
            EnrageLeapAttack();
        else if (s == Skill.HellHoundVariantSlam)
            SlamAttack();
    }

    #region Normal Attack
    void VariantNormalAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.HellHoundVariantNA;
        if (Enraged)
            animator.Play("VariantNA");
        else
            animator.Play("VariantNA");

    }
    #endregion

    #region FireBall
    void BigFireBallAttack()
    {
        currentSkill = Skill.HellHoundVariantFireball;
        inAction = true;
        isAttacking = true;
        if (Enraged)
            animator.Play("EnrageFireBallVariant");
        else
            animator.Play("FireBallVariant");

    }

    void ShootBigFireBall()
    {
        Vector3 startingLocation = Head.position;
        ThrowProjectile(BigFireballPrefab, startingLocation);
    }
    #endregion

    #region Leap
    void EnrageLeapAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.HellHoundVariantLeap;
            animator.Play("VariantLeap");
    }


    #endregion

    #region Slam
    void SlamAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.HellHoundVariantSlam;
        if (Enraged)
            animator.Play("EnragetSlam");
        else
            animator.Play("Slam");

    }

    void SlamJump()
    {
        SlamPositionX = TrackedUnit.transform.position.x;
        rigidbody.gravityScale = 0;
        rigidbody.velocity = new Vector2(0, 0);
    }

    void StartSlam()
    {
        float XSpeed = SlamPositionX - transform.position.x;
        rigidbody.velocity = new Vector2(XSpeed*6, 0);
        FaceTarget();

    }


    void EndSlam()
    {
        //create effect
        var instance = Instantiate(SlamSpikesPrefab);
        instance.transform.position =  new Vector3( FrontClaw.transform.position.x,-4,0);
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        instance.GetComponent<AreaEffectControl>().SetUp(currentSkill.Amount * GetComponent<UnitStats>().DamageMulti(), currentSkill.Bleed, currentSkill.HealthOnHit,gameObject);

        rigidbody.gravityScale = 4;

        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

    void BigSlam()
    {
        //create effect
        var instance = Instantiate(SlamSpikesPrefab);
        instance.transform.position = new Vector3(FrontClaw.transform.position.x, -4, 0);
        instance.transform.localScale = new Vector3(1, 1, 0);
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        instance.GetComponent<AreaEffectControl>().SetUp(currentSkill.Amount*2 * GetComponent<UnitStats>().DamageMulti(), currentSkill.Bleed, currentSkill.HealthOnHit, gameObject);

        currentExhaustCount = 0;

        currentRestTimer = restTimer;
        rigidbody.gravityScale = 4;

        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

    void FinishSlamAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }
    #endregion

    #region Enrage
    protected override void EnrageRoar()
    {
        inAction = true;
        animator.Play("VariantEnrage");
        //add enraged skills to skillpool
        SkillPool.Add(Skill.HellHoundVariantLeap);
        SkillCooldowns.Add(0);

    }

    protected override void EnrageBuffs()
    {
        base.EnrageBuffs();
        statController.GainArmor(statController.MaxHealth() * 0.2f); //get more armor than usual
    }
    #endregion

    #region movement

    protected override void startMoving()
    {
        chaseTimer = 2;
        moving = true;
        GetComponent<Animator>().Play("Walk");
    }
    protected override void Walk()
    {
        if (!inAction)
        {
            if (moveRight)
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
    #endregion

    #region death
    public  void Death()
    {
        inAction = true;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        animator.Play("VariantDeath");
    }

    void StartDeathDecay()
    {
        StartCoroutine(DeathDecay());
        variantStats.FadeAway();
    }
    #endregion
}
