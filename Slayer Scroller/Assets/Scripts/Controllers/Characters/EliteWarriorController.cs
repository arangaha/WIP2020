using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteWarriorController : EnemyController
{
    [SerializeField] WeaponColliderController SwordCollider;
    [SerializeField] Transform BackOpenHand;
    [SerializeField] Transform Orb;
    Animator animator;
    [SerializeField] bool ChargingForward = false;//for when this unit is charging towards a target
    int ChargeSpeed = 6;
    float SlamPositionX = 0; // x position of target when a leap starts
    [SerializeField] bool Enraged = false;
    GameObject BeamPrefab;
    GameObject MegaBeamPrefab;

    GameObject WavePrefab;
    GameObject SpikePrefab;
    GameObject StormBoltPrefab;
    GameObject StormCallPrefab;
    GameObject BarrierPrefab;
    GameObject BarrierInstance;
    GameObject PortalPrefab;

    int BarrierTimer; // timer before barrier explodes
    int PortalCount; 

    bool queueBarrier = false;
    bool failedbarrier = false;
    [SerializeField] bool inBarrier;

    bool waiting = true;

   [SerializeField] GameObject SwordGlow;
    [SerializeField] EliteWarriorStats statControl;
    // Start is called before the first frame update
    protected override void Start()
    {

        base.Start();
        animator = GetComponent<Animator>();
        BasewalkSpeed = walkSpeed = 3;
        xScale = 0.65f;
        SwordCollider.WeaponCollisionEnterEvent.AddListener(HitTarget);
        exhaustCount = 4;
        restTimer = 2;
        BarrierTimer = 60;

          SkillPool.Add(Skill.EliteWarriorJab);
         SkillPool.Add(Skill.EliteWarriorLeap);

         SkillPool.Add(Skill.EliteWarriorSwing);
         SkillPool.Add(Skill.EliteWarriorStormCall);
        for (int i = 0; i < SkillPool.Count; i++)
        {
            SkillCooldowns.Add(0);
        }

        BeamPrefab = Resources.Load("Prefabs/AoE/EliteWarriorBeam") as GameObject;
        MegaBeamPrefab = Resources.Load("Prefabs/AoE/EliteWarriorMegaBeam") as GameObject;
        WavePrefab = Resources.Load("Prefabs/AoE/EliteWarriorWave") as GameObject;
        BarrierPrefab = Resources.Load("Prefabs/AoE/EliteWarriorBarrier") as GameObject;
        SpikePrefab = Resources.Load("Prefabs/AoE/ChargeSpikes") as GameObject;
        StormBoltPrefab = Resources.Load("Prefabs/AoE/StormBolt") as GameObject;
        StormCallPrefab = Resources.Load("Prefabs/AoE/StormCall") as GameObject;
        PortalPrefab = Resources.Load("Prefabs/Characters/Portal") as GameObject;


        //Enraged = true;
        inAction = true; // wait for play to approach
        animator.Play("Wait");
    }


    protected override void Update()
    {
        if(waiting&&getUnitDistance( ClosestEnemy) < 8)
        {
            waiting = false;
            animator.Play("Entry");
        }
        base.Update();
        //if distance with target is more than 1? unit, move towards it
        if (ChargingForward )
        {if (Mathf.Abs(getUnitDistance(TrackedUnit)) > 3)
            if (moveRight)
            {
                rigidbody.velocity = new Vector2(ChargeSpeed, 0);
            }
            else
                rigidbody.velocity = new Vector2(-ChargeSpeed, 0);
            else
                StopCharging();
        }
        //enrage
    if(statControl.PercentageHealth()<=0.5f&!Enraged)
        {
            statControl.SetInvulnerable();
            Enraged = true;
            if (inAction)
                queueBarrier = true;
            else
                StartBarrier();
                
        }


    }

    protected override void pickAction()
    {
        if (queueBarrier)
            StartBarrier();
        else
            base.pickAction();
    }

    protected override void startTriggeredAction(Skill s)
    {
        if (s == Skill.EliteWarriorLeap)
            LeapAtack();
        else if (s == Skill.EliteWarriorSwing)
            SwingAttack();
        else if (s == Skill.EliteWarriorJab)
            JabAttack();
        else if (s == Skill.EliteWarriorSpikes)
            SpikeAttack();
        else if (s == Skill.EliteWarriorStormCall)
            StormAttack();
        else if (s == Skill.EliteWarriorMegaBeam)
            MegaBeam();

    }

    public void TurnWeaponDetectionOn()
    {
        SwordCollider.DetectionOn();
    }

    public void TurnWeaponDetectionOff()
    {
        SwordCollider.DetectionOff();
    }

    void StartCharging()
    {
        ChargingForward = true;

    }

    void StopCharging()
    {
        ChargingForward = false;
        rigidbody.velocity = new Vector2(0, 0);
    }


    /// <summary>
    /// shoot beam from back hand
    /// </summary>
    void ShootBeam()
    {
        var instance = Instantiate(BeamPrefab);
        instance.transform.position = BackOpenHand.position;
        instance.transform.eulerAngles = new Vector3(0,0, -BackOpenHand.eulerAngles.z);
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        Skill beamskill = Skill.EliteWarriorBeam;
        instance.GetComponent<AreaEffectControl>().SetUp(beamskill.Amount * GetComponent<UnitStats>().DamageMulti(), 0, 0, gameObject);

    }

    /// <summary>
    /// release a wave base on position
    /// </summary>
    void ReleaseWave()
    {
        var instance = Instantiate(WavePrefab);
        instance.transform.position = transform.position;
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        Skill waveskill = Skill.EliteWarriorWave;
        instance.GetComponent<AreaEffectControl>().SetUp(waveskill.Amount * GetComponent<UnitStats>().DamageMulti(), 0, 0, gameObject);
    }

    void ReleaseSpikes()
    {
        var instance = Instantiate(SpikePrefab);
        instance.transform.position = new Vector3( transform.position.x,-4,0);
        instance.GetComponent<EliteWarriorSpikesControl>().setUnitType(unitType);
        Skill spikeskill = Skill.EliteWarriorSpikes;
        instance.GetComponent<EliteWarriorSpikesControl > ().SetUp(spikeskill.Amount * GetComponent<UnitStats>().DamageMulti(), 0, 0, gameObject);
        instance.GetComponent<EliteWarriorSpikesControl>().SetupSpike(facingRight, 12);
    }
    
    void StormCall()
    {
        if (!Enraged)
            StormBolt();
        else
        {
            var instance = Instantiate(StormCallPrefab);
            instance.transform.position = new Vector3(TrackedUnit.transform.position.x, 6, 0);
            instance.GetComponent<EliteWarriorStormCallControl>().Setup();
        }
    }

    void StormBolt()
    {

        var instance = Instantiate(StormBoltPrefab);
        instance.transform.position = new Vector3(TrackedUnit.transform.position.x, 6, 0);
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        Skill stormskill = Skill.EliteWarriorStormCall;
        instance.GetComponent<AreaEffectControl>().SetUp(stormskill.Amount * GetComponent<UnitStats>().DamageMulti(), 0, 0, gameObject);
    }

    #region Storm Attack
    void StormAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.EliteWarriorStormCall;
            animator.Play("StormCall");
       
    }
    #endregion

    #region Swing Attack
    void SwingAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.EliteWarriorSwing;
        if (Enraged)
            animator.Play("ChargeAttack&Beam");
        else
            animator.Play("ChargeAttack");
    }
    #endregion

    #region Jab Attack
    void JabAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.EliteWarriorJab;
        if (Enraged)
            animator.Play("Stab&Swing");
        else
            animator.Play("Stab");
    }
    #endregion

    #region Spike Attack
    void SpikeAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.EliteWarriorSpikes;

            animator.Play("Spikes");
    }
    #endregion

    #region Mega Beam

    void MegaBeam()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.EliteWarriorMegaBeam;
        animator.Play("MegaBeam");
    }

    void StartMegaBeam()
    {
        var instance = Instantiate(MegaBeamPrefab);
        instance.transform.position = Orb.position;
        if(facingRight)
        instance.transform.eulerAngles= new Vector3(0,0,-90) ;
        else
            instance.transform.eulerAngles = new Vector3(0, 0, 90);
        instance.transform.SetParent(BackOpenHand);
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        Skill beamskill = Skill.EliteWarriorMegaBeam;
        instance.GetComponent<AreaEffectControl>().SetUp(beamskill.Amount * GetComponent<UnitStats>().DamageMulti(), 0, 0, gameObject);


    }

    void MoveForward()
    {
        ChargingForward = true;
    }

    void StopMoving()
    {
        ChargingForward = false;
    }

    #endregion
    #region leap attack
    void LeapAtack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.EliteWarriorLeap;
        animator.Play("LeapAttack");
    }

    void LeapJump()
    {
        SlamPositionX = TrackedUnit.transform.position.x;
        rigidbody.gravityScale = 0;
        rigidbody.velocity = new Vector2(0, 0);
    }

    void LeapDown()
    {
       // SlamPositionX = TrackedUnit.transform.position.x;
        float XSpeed = SlamPositionX - transform.position.x;
        rigidbody.velocity = new Vector2(XSpeed * 2.5f, 0);
        FaceTarget();
        TurnWeaponDetectionOn();
    }

    void EndLeap()
    {
        rigidbody.gravityScale = 4;
        TurnWeaponDetectionOff();
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

    void FinishLeapAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

    #endregion



    #region barrier
    void StartBarrier()
    {
        animator.Play("Barrier");
        inAction = true;
        queueBarrier = false;
    }

    void SpawnBarrier()
    {
        inBarrier = true;
        failedbarrier = false;
        var instance = Instantiate(BarrierPrefab);
        instance.transform.position = new Vector3( transform.position.x,-2.5f,1);
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        instance.GetComponent<AreaEffectControl>().SetUp(100 * GetComponent<UnitStats>().DamageMulti(), 0, 0, gameObject);
        BarrierInstance = instance;
        StartCoroutine(InBarrierCoroutine());
        SpawnPortals();
    }

    IEnumerator InBarrierCoroutine()
    {
        while(inBarrier)
        {
            statControl.Heal(statControl.MaxHealth()/200);
            yield return new WaitForSeconds(1);
        }
    }


    void SpawnPortals()
    {
        var instance = Instantiate(PortalPrefab);
        instance.transform.position = new Vector3(transform.position.x - 5, -1.35f, 1);
        instance.GetComponent<UnitStats>().Init(ProgressSave.Instance.CurrentLevel);
        instance.GetComponent<UnitController>().onDeath.AddListener(MinusPortal);
        instance = Instantiate(PortalPrefab);
        instance.transform.position = new Vector3(transform.position.x + 5, -1.35f, 1);
        instance.GetComponent<UnitStats>().Init(ProgressSave.Instance.CurrentLevel);
        instance.GetComponent<UnitController>().onDeath.AddListener(MinusPortal);
        PortalCount = 2;
        StartCoroutine(BarrierCountdown());
        StartCoroutine(PortalCheck());


    }
    IEnumerator BarrierCountdown()
    {
        int currentTimer = BarrierTimer;
        while(currentTimer>0&&inBarrier&&BarrierInstance)
        {
           
            yield return new WaitForSeconds(1);
           
            currentTimer -= 1;
            BarrierInstance.GetComponent<EliteWarriorBarrier>().UpdateTimer(currentTimer);
        }
        if (BarrierInstance&&!failedbarrier)
        {
            BarrierInstance.GetComponent<EliteWarriorBarrier>().Explode();
            statControl.GainArmor(statControl.MaxHealth() * 0.2f);
            animator.Play("BarrierExit");
        }

    }

    /// <summary>
    /// checks remaining portals, if count is 0, barrier phase fails
    /// </summary>
    /// <returns></returns>
    IEnumerator PortalCheck()
    {
        while (PortalCount > 0)
        {
            yield return null;
        }
        if (BarrierInstance)
        {
            BarrierInstance.GetComponent<EliteWarriorBarrier>().Fail();
            animator.Play("BarrierExit");
            failedbarrier = true;
        }
    }

    void Enrage()
    {
        Enraged = true;
        SwordGlow.SetActive(true);
        statControl.SetVulnerable();
        SkillPool.Add(Skill.EliteWarriorSpikes);
        SkillPool.Add(Skill.EliteWarriorMegaBeam);
        SkillCooldowns.Add(0);
        SkillCooldowns.Add(0);
        for (int i = 0; i < SkillCooldowns.Count; i++)
        {
            SkillCooldowns[i] = 0;
        }

        inBarrier = false;
    }

    void MinusPortal()
    {
        PortalCount--;
    }

    #endregion
    public override void Death()
    {
        inAction = true;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        animator.Play("Death");
    }

    public void Disappear()
    {
        onDeath.Invoke();
        statControl.FadeAway();
        Destroy(gameObject);
    }
    }
