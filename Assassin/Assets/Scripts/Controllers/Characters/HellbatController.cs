using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellbatController : EnemyController
{
    [SerializeField] WeaponColliderController FrontClaw;
    [SerializeField] WeaponColliderController BackClaw;
    [SerializeField] Transform Head;
    Animator animator;
    Vector3 waveStartingLoc;
    Vector3 WaveDirection;
    GameObject SonicWavePrefab;

    // Start is called before the first frame update
    protected override void Start()
    {

        base.Start();
        xScale = 0.5f;
        animator = GetComponent<Animator>();
       BasewalkSpeed= walkSpeed = 5;
        FrontClaw.WeaponCollisionEnterEvent.AddListener(HitTarget);
        BackClaw.WeaponCollisionEnterEvent.AddListener(HitTarget);
        SkillPool.Add(Skill.HellbatGouge);
        SkillPool.Add(Skill.HellbatSonicWave);
        SkillPool.Add(Skill.HellbatNA);
        for (int i = 0; i < SkillPool.Count; i++)
        {
            SkillCooldowns.Add(0);
        }


        SonicWavePrefab = Resources.Load("Prefabs/Projectiles/SonicWave") as GameObject;


    }

    protected override void startTriggeredAction(Skill s)
    {
        if (s == Skill.HellbatNA)
            NormalAttack();
        else if (s == Skill.HellbatGouge)
            Gouge();
        else if (s == Skill.HellbatSonicWave)
            SonicWave();
    }


    public override void Death()
    {
        inAction = true;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        animator.Play("Death");
        StartCoroutine(DeathDecay());
    }
    #region NA
    void NormalAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.HellbatNA;
        animator.Play("NormalAttack");
    }

    void finishNormalAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
    }
    #endregion

    #region Gouge
    void Gouge()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.HellbatGouge;
        animator.Play("Gouge");

    }
    #endregion
    #region Sonic Wave
    void SonicWave()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.HellbatSonicWave;
        animator.Play("SonicWave");
    }

    void FirstWave()
    {
        WaveDirection= GetProjectileDirection(Head.position, TrackedUnit.transform.position,currentSkill.ProjectileSpeed);
        waveStartingLoc = Head.position;
        Quaternion  rotation = Quaternion.AngleAxis(Mathf.Atan2(WaveDirection.y, WaveDirection.x) * Mathf.Rad2Deg, Vector3.forward);
       // Debug.Log(rotation.x + "," + rotation.y + "," + rotation.z);
        ThrowProjectile(SonicWavePrefab, Head.position, WaveDirection);
    }

    void FollowWave()
    {
        Quaternion rotation = Quaternion.AngleAxis(Mathf.Atan2(WaveDirection.y, WaveDirection.x) * Mathf.Rad2Deg, Vector3.forward);
        ThrowProjectile(SonicWavePrefab, waveStartingLoc, WaveDirection);

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

}
