using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : UnitController
{

    [SerializeField] bool isJumpingUp = false;
    [SerializeField] bool isFalling = false;

    [SerializeField] bool isDashing = false;
    [SerializeField] bool queueDashAttack = false; // check if an attack is queued after a dash
    [SerializeField] bool queueDashRangedAttack = false; // check if an attack is queued after a dash
    [SerializeField] bool canQueueDashAttack = false; //checker to see if user can queue for a dash attack
    [SerializeField] bool isDashAttacking = false;
    [SerializeField] bool isDashFalling = false; //checker to see if unit is falling after a dash
    [SerializeField] bool isSlashing = false; //whether player is using slash

    [SerializeField] bool isPerformingMovementSkill = false;

    [SerializeField] bool isNormalAttacking = false;
    [SerializeField] int NormalAttackRotation = 1; // which normal attack the character is currently on
    [SerializeField] int NormalRangedAttackRotation = 1; // which normal ranged attack the character is currently on

    [SerializeField] Vector2  slashSpeed;
    [SerializeField] float dashSpeed = 25f;
    [SerializeField] float JumpForce = 15f;
    [SerializeField] bool sprinting = false;
    [SerializeField] bool sprintAgain = true; // can sprint again, used to prevent sprint > walk > sprint in a short frame of time

    [SerializeField] private GameObject UpperBody;
    [SerializeField] private GameObject LowerBody;
    [SerializeField] private GameObject FrontHand;
    [SerializeField] private GameObject BackHand;
    [SerializeField] private GameObject FrontBlade;
    TargetDetection targetDetect;

    [SerializeField] private GroundColliderController GroundCollider;
    private MainCharacterUpperAnimationController UpperBodyAnimationStateController;
    private MainCharacterLowerAnimationController LowerBodyAnimationStateController;
    [SerializeField] private WeaponColliderController FrontBladeCollider;
    [SerializeField] private WeaponColliderController BackBladeCollider;
    private MainCharacterStats MainStatController;

    Vector3 RoBLocation;

    #region skill prefabs
    GameObject currentProjectilePrefab;
    private GameObject BladeProjectilePrefab;
    private GameObject RazorBladePrefab;
    private GameObject CullEffectPrefab;
    private GameObject RainOfBladesPrefab;
    private GameObject BladeStormPrefab;
    private GameObject ChakramPrefab;
    private GameObject PerforatePrefab;
    private GameObject FortifyPrefab;
    private GameObject SpikeTrapPrefab;
    #endregion

    #region BladeStorm autocast info
    [SerializeField] int AutoCastChance = 0;
    TempPlayerSkill BladestormInstance;
    #endregion

    #region perforate blades info
    TempPlayerSkill perforatePrefab;
    #endregion

    #region NA info
    bool NAHitOnce = false; // checker for if NA already hit once in the current attack
    int NAFortifyCooldownReduction = 0;
    #endregion

    UIController uicontroller;
    #region player skills
    protected Dictionary<Skill, TempPlayerSkill> Skills = new Dictionary<Skill, TempPlayerSkill>(); //skill list for players. key: skill, value: skill containing the upgrades
    protected TempPlayerSkill currentUpgradedSkill = new TempPlayerSkill(PlayerSkill.Default);
    List<Skill> SlottedSkills = new List<Skill>();
    public bool fullySlotted = false;

    #endregion
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        uicontroller = GameObject.Find("WorldUI").GetComponent<UIController>();
        MainStatController = GetComponent<MainCharacterStats>();
        MainStatController.onHealthLost.AddListener(OnHealthLost);
        unitType = UnitType.Ally;
        targetDetect = transform.Find("TargetDetection").GetComponent<TargetDetection>();
        targetDetect.ChangeUnitType(UnitType.Ally);
        targetDetect.onUpdateAllies.AddListener(updateNearbyAllies);
        targetDetect.onUpdateEnemies.AddListener(updateNearbyEnemies);
        BasewalkSpeed = walkSpeed = 8;
        dashSpeed = 20;
        JumpForce = 1000;
        xScale = 0.5f;
        FlipCharacter(true);
        UpperBodyAnimationStateController = UpperBody.GetComponent<MainCharacterUpperAnimationController>();
        UpperBodyAnimationStateController.onActionFinish.AddListener(FinishAction);
        UpperBodyAnimationStateController.onNACollisionStart.AddListener(TurnNAWeaponDetectionOn);
        UpperBodyAnimationStateController.onNAFinish.AddListener(finishNA);
        UpperBodyAnimationStateController.onAttackFinish.AddListener(StopAttacking);
        UpperBodyAnimationStateController.onRangedNAThrow.AddListener(NormalRangedAttackThrow);
        UpperBodyAnimationStateController.onRangedNAFinish.AddListener(finishRangedNA);
        UpperBodyAnimationStateController.onDefaultRangedFinish.AddListener(FinishDefaultRangedAttack);
        UpperBodyAnimationStateController.onDefaultSpellUse.AddListener(DefaultSpellCast);
        UpperBodyAnimationStateController.onDefaultRangedThrow.AddListener(DefaultRangedThrow);
        UpperBodyAnimationStateController.WeaponDetectionOnFront.AddListener(TurnDetectionOnFrontBlade);
        UpperBodyAnimationStateController.WeaponDetectionOnBack.AddListener(TurnDetectionOnBackBlade);
        UpperBodyAnimationStateController.WeaponDetectionOffFront.AddListener(TurnDetectionOffFrontBlade);
        UpperBodyAnimationStateController.WeaponDetectionOffBack.AddListener(TurnDetectionOffBackBlade);
        UpperBodyAnimationStateController.onSlashFinish.AddListener(FinishSlash);
        UpperBodyAnimationStateController.onCullEffect.AddListener(CreateCull);
        UpperBodyAnimationStateController.onPerforate.AddListener(CreatePerforateBlades);

        LowerBodyAnimationStateController = LowerBody.GetComponent<MainCharacterLowerAnimationController>();
        LowerBodyAnimationStateController.onDashFinish.AddListener(FinishDash);
        LowerBodyAnimationStateController.onDashTransition.AddListener(canDashAttack);
        LowerBodyAnimationStateController.onDashAttackFinish.AddListener(finishDashAttack);
        BackBladeCollider.setUnitType(unitType);
        FrontBladeCollider.setUnitType(unitType);
        BackBladeCollider.ResetHitList();
        FrontBladeCollider.ResetHitList();
        BackBladeCollider.WeaponCollisionEnterEvent.AddListener(HitTarget);
        FrontBladeCollider.WeaponCollisionEnterEvent.AddListener(HitTarget);

        //loading skill prefabs
        BladeProjectilePrefab = Resources.Load("Prefabs/Projectiles/BladeProjectile") as GameObject;
        RazorBladePrefab = Resources.Load("Prefabs/Projectiles/RazorBlade") as GameObject;
        ChakramPrefab = Resources.Load("Prefabs/Projectiles/Chakram") as GameObject;
        CullEffectPrefab = Resources.Load("Prefabs/AoE/CullEffect") as GameObject;
        RainOfBladesPrefab = Resources.Load("Prefabs/SkillObjects/RainOfBlades") as GameObject;
        BladeStormPrefab = Resources.Load("Prefabs/SkillObjects/BladeStorm") as GameObject;
        PerforatePrefab = Resources.Load("Prefabs/AoE/Perforate") as GameObject;
        FortifyPrefab = Resources.Load("Prefabs/Effects/FortifyEffect") as GameObject;
        SpikeTrapPrefab = Resources.Load("Prefabs/AoE/SpikeTrap") as GameObject;

    }

    public void Init(Dictionary<Skill, TempPlayerSkill> skills, List<Skill> slottedSkills)
    {
        uicontroller = GameObject.Find("WorldUI").GetComponent<UIController>();
        Skills = skills;

        foreach (KeyValuePair<Skill, TempPlayerSkill> s in skills)
        {
            if (s.Key != Skill.PlayerNormalAttack && s.Key != Skill.PlayerNormalRangedAttack && s.Key != Skill.Default)
            {
                SkillPool.Add(s.Key);
                SkillCooldowns.Add(0);
                UpdateSkill(s.Key);
            }
        }

        SlottedSkills = new List<Skill>();
        SlottedSkills.Add(Skill.Default);
        SlottedSkills.Add(Skill.Default);
        SlottedSkills.Add(Skill.Default);
        SlottedSkills.Add(Skill.Default);
        for (int i = 0; i < slottedSkills.Count; i++)
        {
            SlotSkill(slottedSkills[i], i, false);
        }

        uicontroller.UpdateCooldown(1, 0);
        uicontroller.UpdateCooldown(2, 0);
        uicontroller.UpdateCooldown(3, 0);
        uicontroller.UpdateCooldown(4, 0);
        //manual unlocks and upgrades
        // LearnSkill(Skill.PlayerRazorBlades);
        //LearnSkill(Skill.PlayerPuncture);
        //  LearnSkill(Skill.PlayerSlash);
        //LearnSkill(Skill.PlayerCull);
        // // LearnSkill(Skill.PlayerRainOfBlades);
        //LearnSkill(Skill.PlayerBladeStorm);
        //  LearnSkill(Skill.PlayerChakram);
        //   LearnSkill(Skill.PlayerPerforate);
        //LearnSkill(Skill.PlayerFortify);
        //LearnSkill(Skill.PlayerSpikeTrap);
        // UpgradeSkill(Skill.PlayerSpikeTrap, SkillUpgrade.SpikeTrapMultipleTraps);
        //UpgradeSkill(Skill.PlayerSpikeTrap, SkillUpgrade.SpikeTrapSlowDamage);
        //UpgradeSkill(Skill.PlayerSpikeTrap, SkillUpgrade.SpikeTrapIncBleedDmg);
        
        //UpgradeSkill(Skill.PlayerSpikeTrap, SkillUpgrade.SpikeTrapMultipleTraps);
        //UpgradeSkill(Skill.PlayerSpikeTrap, SkillUpgrade.SpikeTrapSpikes);
        //UpgradeSkill(Skill.PlayerSpikeTrap, SkillUpgrade.SpikeTrapSpikes);
        //UpgradeSkill(Skill.PlayerSpikeTrap, SkillUpgrade.SpikeTrapSpikes);
        //UpgradeSkill(Skill.PlayerFortify, SkillUpgrade.FortifyProtection);
        //UpgradeSkill(Skill.PlayerPerforate, SkillUpgrade.PerforateHeight);
        //UpgradeSkill(Skill.PlayerPerforate, SkillUpgrade.PerforateBlades);
        //UpgradeSkill(Skill.PlayerPerforate, SkillUpgrade.PerforateSlow);
        // UpgradeSkill(Skill.PlayerBladeStorm, SkillUpgrade.BladeStormCost);
        // UpgradeSkill(Skill.PlayerBladeStorm, SkillUpgrade.BladeStormCost);
        // UpgradeSkill(Skill.PlayerBladeStorm, SkillUpgrade.BladeStormCost);
        // UpgradeSkill(Skill.PlayerBladeStorm, SkillUpgrade.BladeStormCost);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Grounded = GroundCollider.grounded;



        //if not in action, these conditions are checked
        if (!inAction && !GlobalVariables.Instance.HasUI)
        {
            //sets animation of lower body
            if (Grounded)
            {
                if (WalkLeft || WalkRight)
                    Walk();
                else if(!isAttacking && !sprinting && !isDashing)
                    SetIdle();
            }
            else if (isJumpingUp)
                Jump();
            else if (isFalling)
            {
                if (isDashFalling)
                    DashFalling();
                else
                    Falling();
            }
            //sets states based on buttons pressed
            if (Input.GetMouseButtonDown(0))
            {

                NormalAttack();

            }
            else if (Input.GetMouseButtonDown(1))
            {

                NormalRangedAttack();
            }
            else if (Input.GetKeyDown("q"))
            {
                UseSkillInSlot(0);
            }
            else if (Input.GetKeyDown("w"))
            {
                UseSkillInSlot(1);
            }

            else if (Input.GetKeyDown("e"))
            {
                UseSkillInSlot(2);
            }

            else if (Input.GetKeyDown("r"))
            {
                UseSkillInSlot(3);
            }

            else if (Input.GetKeyDown("space") && Grounded)
            {
                StartJump();

            }
            else if (Input.GetKeyDown(KeyCode.LeftControl) && !isDashing)
            {
                Dash();

            }

            //movement related, i.e., walking, spriting, jumping
            if (Input.GetKey("a"))
            {
                WalkLeft = true;
                WalkRight = false;

            }
            else if (Input.GetKey("d"))
            {
                WalkLeft = false;
                WalkRight = true;
            }
            else
            {
                //sets idle if no actions are being called
                Idle = true;
                WalkLeft = false;
                WalkRight = false;
                sprinting = false;

            }

            if (!isAttacking && !sprinting && !isDashing)

                UpperBody.GetComponent<Animator>().Play("IdleUpper");

        }


        if (Input.GetKeyUp("a"))
        {
            WalkLeft = false;
        }
        if (Input.GetKeyUp("d"))
        {
            WalkRight = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            sprintAgain = true;
        }
        if (MainStatController.CurrentStamina() < 1)
        {
            sprintAgain = false;
        }

     //   if (!isPerformingMovementSkill)
     //   {
            //changes character velocity/position based on states
            if(isSlashing)
            {
                rigidbody.velocity = slashSpeed;
            }
            else if (isDashing)
            {
                if (!facingRight)
                    rigidbody.velocity = new Vector2(-dashSpeed, 0);
                else
                    rigidbody.velocity = new Vector2(dashSpeed, 0);
                // if player left clicks during a dash, queue for a dash attack
                if (Input.GetMouseButtonDown(0))
                {
                    queueDashAttack = true;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    queueDashRangedAttack = true;
                }
                if (canQueueDashAttack && queueDashAttack)
                {
                    NormalAttack();
                    canQueueDashAttack = false;
                    queueDashRangedAttack = false;
                    queueDashAttack = false;
                }
                else if (canQueueDashAttack && queueDashRangedAttack)
                {
                    NormalRangedAttack();
                    canQueueDashAttack = false;
                    queueDashRangedAttack = false;
                    queueDashAttack = false;
                }

            }
            else if (WalkLeft)
            {
                if (sprinting)
                {
                    MainStatController.SpendStamina((float)PlayerStaminaCost.Sprint * Time.deltaTime);
                    rigidbody.velocity = new Vector2(-walkSpeed * 1.5f, rigidbody.velocity.y);
                }
                else
                    rigidbody.velocity = new Vector2(-walkSpeed, rigidbody.velocity.y);

            }
            else if (WalkRight)
            {
                if (sprinting)
                {
                    rigidbody.velocity = new Vector2(walkSpeed * 1.5f, rigidbody.velocity.y);
                    MainStatController.SpendStamina((float)PlayerStaminaCost.Sprint * Time.deltaTime);
                }
                else
                    rigidbody.velocity = new Vector2(walkSpeed, rigidbody.velocity.y);
            }
            else if (Grounded)
            {
                rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
            }
        //}

        if (rigidbody.velocity.y < 0 && !Grounded)
        {
            isJumpingUp = false;
            isFalling = true;
        }
        else if (Grounded)
        {
            isFalling = false;
            isDashFalling = false;
        }

        //sets unit to not be affected by gravity if it is in action
        if (isDashing||isPerformingMovementSkill)
        {
            rigidbody.gravityScale = 0;


        }
        else
        {
            rigidbody.gravityScale = 4;
        }


        //updates icons for skills
        for(int i = 0; i< SlottedSkills.Count; i++)
        {
            if (SlottedSkills[i] != Skill.Default)
            {
                float cooldown = SkillCooldowns[SkillPool.IndexOf(SlottedSkills[i])];
                if (cooldown > 0)
                    uicontroller.UpdateCooldown(i + 1, cooldown / Skills[SkillPool[SkillPool.IndexOf(SlottedSkills[i])]].Cooldown);
                if (MainStatController.CurrentEnergy() < Skills[SkillPool[SkillPool.IndexOf(SlottedSkills[i])]].EnergyCost)
                    uicontroller.cannotUseSkill(i + 1);
                else
                    uicontroller.canUseSkill(i + 1);
            }

        }

    }



    #region Normal Attack
    /// <summary>
    /// performs a normal attack.
    /// picks an animation based on normal attack rotation
    /// </summary>
    void NormalAttack()
    {
        if (!isNormalAttacking&&!isAttacking)
        {
          //  rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
            switch (NormalAttackRotation)
            {
                case 1:
                    UpperBody.GetComponent<Animator>().Play("NormalAttack1");
                    break;
                case 2:
                    UpperBody.GetComponent<Animator>().Play("NormalAttack2");
                    break;


            }
            isNormalAttacking = true;
            currentSkill = Skill.PlayerNormalAttack;
            currentUpgradedSkill = Skills[currentSkill];
            inAction = true;
            isAttacking = true;
            NAHitOnce = false;
            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
            {
                facingRight = true;
                FlipCharacter(true);
            }
            else
            {
                facingRight = false;
                FlipCharacter(false);
            }
            if ((WalkLeft || WalkRight) && Grounded)
                Walk();
        }
    }


    public void finishNA()
    {
        isNormalAttacking = false;
        inAction = false;
        TurnNAWeaponDetectionOff();
        switch (NormalAttackRotation)
        {
            case 1:
                NormalAttackRotation = 2;
                break;
            case 2:
                NormalAttackRotation = 1;
                break;


        }
        currentSkill = Skill.Default;
    }
    #endregion


    #region Normal Ranged Attack

    /// <summary>
    /// performs a normal ranged attack.
    /// picks an animation based on normal attack rotation
    /// </summary>
    void NormalRangedAttack()
    {
        if (!isNormalAttacking && !isAttacking)
        {
            //  rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
            switch (NormalRangedAttackRotation)
            {
                case 1:
                    UpperBody.GetComponent<Animator>().Play("NormalRangedAttack1");
                    break;
                case 2:
                    UpperBody.GetComponent<Animator>().Play("NormalRangedAttack2");
                    break;


            }
            isNormalAttacking = true;
            currentSkill = Skill.PlayerNormalRangedAttack;
            currentUpgradedSkill = Skills[currentSkill];
            currentProjectilePrefab = BladeProjectilePrefab;
            inAction = true;
            isAttacking = true;

            //gain energy if the upgrade is taken
            MainStatController.GainEnergy(currentUpgradedSkill.Upgrades[SkillUpgrade.NormalRangedAttackEnergy]);
           
            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
            {
                facingRight = true;
                FlipCharacter(true);
            }
            else
            {
                facingRight = false;
                FlipCharacter(false);
            }

            if ((WalkLeft || WalkRight )&&Grounded)
                Walk();
        }
    }

    /// <summary>
    /// throws ranged attack blade
    /// </summary>
    void NormalRangedAttackThrow()
    {
        Vector3 startingLocation = new Vector3(0,0,0);
        switch (NormalRangedAttackRotation)
        {
            case 1:
                startingLocation = FrontHand.transform.position;
                break;
            case 2:
                startingLocation = BackHand.transform.position;
                break;
        }

        ThrowProjectile(BladeProjectilePrefab, startingLocation);
        int ExtraProjUpgrades = currentUpgradedSkill.Upgrades[SkillUpgrade.NormalRangedAttackAdditionalProjectiles];
        if (ExtraProjUpgrades > 0)
            ThrowProjectile(BladeProjectilePrefab, startingLocation - new Vector3(0, 0.35f));
        if (ExtraProjUpgrades > 1)
            ThrowProjectile(BladeProjectilePrefab, startingLocation + new Vector3(0, 0.35f));

    }

    public void finishRangedNA()
    {
        isNormalAttacking = false;
        inAction = false;

        switch (NormalRangedAttackRotation)
        {
            case 1:
                NormalRangedAttackRotation = 2;
                break;
            case 2:
                NormalRangedAttackRotation = 1;
                break;


        }
        currentSkill = Skill.Default;
    }
    #endregion

    #region Razor Blades

    void RazorBlades()
    {
        if (EnergyCanUse(Skills[Skill.PlayerRazorBlades].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerRazorBlades)]<=0)
        {
            currentSkill = Skill.PlayerRazorBlades;
            currentUpgradedSkill = Skills[currentSkill];
            currentProjectilePrefab = RazorBladePrefab;
            DefaultRangedAttack();
            MainStatController.SpendEnergy(Skills[Skill.PlayerRazorBlades].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerRazorBlades)] = Skills[Skill.PlayerRazorBlades].Cooldown;
        }
    }

    #endregion

    #region Chakram
    void Chakram()
    {

        if (EnergyCanUse(Skills[Skill.PlayerChakram].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerChakram)] <= 0)
        {
            currentSkill = Skill.PlayerChakram;
            currentUpgradedSkill = Skills[currentSkill];
            currentProjectilePrefab = ChakramPrefab;
            DefaultRangedAttack();
            MainStatController.SpendEnergy(Skills[Skill.PlayerChakram].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerChakram)] = Skills[Skill.PlayerChakram].Cooldown;
        }
    }
    #endregion

    #region spike trap
    void SpikeTrap()
    {
        if (EnergyCanUse(Skills[Skill.PlayerSpikeTrap].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerSpikeTrap)] <= 0)
        {
            currentSkill = Skill.PlayerSpikeTrap;
            currentUpgradedSkill = Skills[currentSkill];
            currentProjectilePrefab = SpikeTrapPrefab;
            DefaultRangedAttack();
            MainStatController.SpendEnergy(Skills[Skill.PlayerSpikeTrap].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerSpikeTrap)] = Skills[Skill.PlayerSpikeTrap].Cooldown;
        }
    }
    #endregion
    #region Puncture
    void Puncture()
    {
        if (EnergyCanUse(Skills[Skill.PlayerPuncture].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerPuncture)] <= 0)
        {
            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
            {
                facingRight = true;
                FlipCharacter(true);
            }
            else
            {
                facingRight = false;
                FlipCharacter(false);
            }
            if ((WalkLeft || WalkRight) && Grounded)
                Walk();
            currentSkill = Skill.PlayerPuncture;
            currentUpgradedSkill = Skills[currentSkill];
            inAction = true;
            isAttacking = true;
            UpperBody.GetComponent<Animator>().Play("Puncture");
            //LowerBody.GetComponent<Animator>().Play("ForwardAction");
            MainStatController.SpendEnergy(Skills[Skill.PlayerPuncture].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerPuncture)] = Skills[Skill.PlayerPuncture].Cooldown;
        }
    }
    #endregion

    #region Slash

    void Slash()
    {
        if (EnergyCanUse(Skills[Skill.PlayerSlash].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerSlash)] <= 0)
        {
            ClearCheckers();
            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
            {
                facingRight = true;
                FlipCharacter(true);
            }
            else
            {
                facingRight = false;
                FlipCharacter(false);
            }
            if ((WalkLeft || WalkRight) && Grounded)
                Walk();
            currentSkill = Skill.PlayerSlash;
            currentUpgradedSkill = Skills[currentSkill];
            inAction = true;
            isAttacking = true;
            isSlashing = true;
            isDashing = false;
            isPerformingMovementSkill = true;
            MainStatController.SpendEnergy(Skills[Skill.PlayerSlash].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerSlash)] = Skills[Skill.PlayerSlash].Cooldown;
  
            float dashDistanceMultiplier = 1 + SkillUpgrade.SlashDistance.SpecialAmount * (currentUpgradedSkill.Upgrades[SkillUpgrade.SlashDistance]);
            
            if (!facingRight)
                slashSpeed =  new Vector2(-dashSpeed * 1.3f*dashDistanceMultiplier, 0);
            else
                slashSpeed  = new Vector2(dashSpeed * 1.3f * dashDistanceMultiplier, 0);

            UpperBody.GetComponent<Animator>().Play("Slash");
            LowerBody.GetComponent<Animator>().Play("SlashLower");
            float armorGained = currentUpgradedSkill.Upgrades[SkillUpgrade.SlashGainArmor]*SkillUpgrade.SlashGainArmor.SpecialAmount;
            MainStatController.GainArmor(armorGained);

        }
    }

    void FinishSlash()
    {
        isAttacking = false;
        isPerformingMovementSkill = false;
        inAction = false;
        isDashing = false;
        isSlashing = false;
        if (!Grounded)
        {
            if (!facingRight)
                rigidbody.velocity = new Vector2(-10, 0);
            else
                rigidbody.velocity = new Vector2(10, 0);
            isDashFalling = true;
        }
        ClearCheckers();
    }
    #endregion

    #region Cull

    void Cull()
    {
        if (EnergyCanUse(Skills[Skill.PlayerCull].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerCull)] <= 0)
        {
            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
            {
                facingRight = true;
                FlipCharacter(true);
            }
            else
            {
                facingRight = false;
                FlipCharacter(false);
            }
            if ((WalkLeft || WalkRight) && Grounded)
                Walk();
            currentSkill = Skill.PlayerCull;
            currentUpgradedSkill = Skills[currentSkill];
            inAction = true;
            isAttacking = true;
            UpperBody.GetComponent<Animator>().Play("Cull");
            LowerBody.GetComponent<Animator>().Play("ForwardAction");
            MainStatController.SpendEnergy(Skills[Skill.PlayerCull].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerCull)] = Skills[Skill.PlayerCull].Cooldown;
        }

    }

    void CreateCull()
    {
        Vector3 startingLocation = new Vector3(0, 0, 0);
        startingLocation = FrontBlade.transform.position;
        StartAoE(CullEffectPrefab, startingLocation);
    }
    #endregion

    #region Perforate
    void Perforate()
    {
        if (EnergyCanUse(Skills[Skill.PlayerPerforate].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerPerforate)] <= 0)
        {
            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
            {
                facingRight = true;
                FlipCharacter(true);
            }
            else
            {
                facingRight = false;
                FlipCharacter(false);
            }
            if ((WalkLeft || WalkRight) && Grounded)
                Walk();
            currentSkill = Skill.PlayerPerforate;
            currentUpgradedSkill = Skills[currentSkill];
            inAction = true;
            isAttacking = true;
            UpperBody.GetComponent<Animator>().Play("Perforate");
            LowerBody.GetComponent<Animator>().Play("Perforate");
            MainStatController.SpendEnergy(Skills[Skill.PlayerPerforate].EnergyCost);

            var cooldownReduction = currentUpgradedSkill.Upgrades[SkillUpgrade.PerforateCooldown] * SkillUpgrade.PerforateCooldown.SpecialAmount; ;
            if(statController.CurrentEnergy()<30)
            {
                cooldownReduction = 0;
            }

            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerPerforate)] = Skills[Skill.PlayerPerforate].Cooldown*(1-cooldownReduction);
            if(!Grounded)
            rigidbody.AddForce(new Vector2(0, -1000));
        }
    }

    void CreatePerforateBlades()
    {
        List<GameObject> EnemiesInRange = new List<GameObject>();
        foreach (GameObject g in NearbyEnemies)
        {
            if (getUnitDistance(g) <= 12)
                EnemiesInRange.Add(g);
        }
        foreach (GameObject g in EnemiesInRange)
        {
            perforatePrefab = currentUpgradedSkill;
            CreatePerforateBlade(g, 1, 1);
        }

        var followBlades = currentUpgradedSkill.Upgrades[SkillUpgrade.PerforateBlades] * SkillUpgrade.PerforateBlades.SpecialAmount;

        if (followBlades > 0)
            StartCoroutine(PerforateFollowBlades(followBlades));

    }

    IEnumerator PerforateFollowBlades(float duration)
    {
        int counter = 0;
        while(counter<duration)
        {
            yield return new WaitForSeconds(1);
            List<GameObject> EnemiesInRange = new List<GameObject>();
            foreach (GameObject g in NearbyEnemies)
            {
                if (getUnitDistance(g) <= 12)
                    EnemiesInRange.Add(g);
            }
            foreach (GameObject g in EnemiesInRange)
            {
                CreatePerforateBlade(g, 0.3f, 0.6f);
            }
            counter += 1;
        }
    }

    /// <summary>
    /// spawn a perforate blade at position with specific damage scaling
    /// </summary>
    /// <param name="pos"></param>
    void CreatePerforateBlade(GameObject target, float DamageScaling, float Scale)
    {
        var instance = Instantiate(PerforatePrefab);
        instance.transform.position = new Vector3(target.transform.position.x, -1 + (Scale*3-3));
        instance.transform.localScale = new Vector3(Scale, Scale, Scale);
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        instance.GetComponent<AreaEffectControl>().SetUp(perforatePrefab.Amount*DamageScaling, 0, perforatePrefab.Bleed*(int)DamageScaling, perforatePrefab.HealthOnHit, gameObject);
        instance.GetComponent<AreaEffectControl>().OnHitHeal.AddListener(Heal);

        var armorGain = perforatePrefab.Upgrades[SkillUpgrade.PerforateArmor] * SkillUpgrade.PerforateArmor.SpecialAmount;
        var bleed = perforatePrefab.Upgrades[SkillUpgrade.PerforateHeight] > 0 ? true : false;
        var slow = perforatePrefab.Upgrades[SkillUpgrade.PerforateSlow] * SkillUpgrade.PerforateSlow.SpecialAmount;
        var bladesDuration = perforatePrefab.Upgrades[SkillUpgrade.PerforateBlades] * SkillUpgrade.PerforateBlades.SpecialAmount;

        if (bleed)
            instance.transform.localScale = new Vector3(1*Scale, 2* Scale, 1* Scale);
        instance.GetComponent<PerforateControl>().UpdradesSetup((int)armorGain, bleed);
        instance.GetComponent<PerforateControl>().SetExclusiveTarget(target);//blade can only hit the enemy it spawned on to prevent overlapping shotgun
        instance.GetComponent<PerforateControl>().AddSlow((int)slow);
    }
    #endregion

    #region Fortify
    void Fortify()
    {
        if (EnergyCanUse(Skills[Skill.PlayerFortify].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerFortify)] <= 0)
        {

            currentSkill = Skill.PlayerFortify;
            currentUpgradedSkill = Skills[currentSkill];

            var instance = Instantiate(FortifyPrefab,transform);
            instance.transform.localScale /= xScale;
            MainStatController.SpendEnergy(Skills[Skill.PlayerFortify].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerFortify)] = Skills[Skill.PlayerFortify].Cooldown;

            var protection = 1-currentUpgradedSkill.Upgrades[SkillUpgrade.FortifyProtection] * SkillUpgrade.FortifyProtection.SpecialAmount;
            var singleEnemyExtraArmor = currentUpgradedSkill.Upgrades[SkillUpgrade.FortifyExtraArmor] * SkillUpgrade.FortifyExtraArmor.SpecialAmount;
            var energyGain = currentUpgradedSkill.Upgrades[SkillUpgrade.FortifyEnergyGain] * SkillUpgrade.FortifyEnergyGain.SpecialAmount;
            bool bleedRemoval = currentUpgradedSkill.Upgrades[SkillUpgrade.FortifyRemoveBleed]>0?true:false;
            if(bleedRemoval)
            {
                statController.RemoveBleed();
            }
            List<GameObject> EnemiesInRange = new List<GameObject>();
            foreach (GameObject g in NearbyEnemies)
            {
                if (getUnitDistance(g) <= currentUpgradedSkill.Range)
                    EnemiesInRange.Add(g);
            }
            foreach (GameObject g in EnemiesInRange)
            {
                statController.GainArmor(currentUpgradedSkill.Amount);
                statController.GainEnergy(energyGain);
            }
            if(EnemiesInRange.Count>0)
            {
                statController.Protect(protection);
            }
            if(EnemiesInRange.Count==1)
            {
                statController.GainArmor(singleEnemyExtraArmor);
            }
            if(!(bleedRemoval&&statController.CurrentHealth()/statController.MaxHealth()<0.5f))
            statController.Slow(2);
        }
    }
    #endregion

    #region Rain Of Blades
    void RainOfBlades()
    {
        if (EnergyCanUse(Skills[Skill.PlayerRainOfBlades].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerRainOfBlades)] <= 0)
        {
            currentSkill = Skill.PlayerRainOfBlades;
            currentUpgradedSkill = Skills[currentSkill];
            RoBLocation = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, transform.position.y + 6);
       
            MainStatController.SpendEnergy(Skills[Skill.PlayerRainOfBlades].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerRainOfBlades)] = Skills[Skill.PlayerRainOfBlades].Cooldown;
            DefaultSpellAttack();

        }
    }
    #endregion

    #region BladeStorm
    void BladeStorm()
    {
        if (EnergyCanUse(Skills[Skill.PlayerBladeStorm].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerBladeStorm)] <= 0)
        {
            currentSkill = Skill.PlayerBladeStorm;
            currentUpgradedSkill = Skills[currentSkill];
            RoBLocation = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, transform.position.y + 6);
            DefaultSpellAttack();
            MainStatController.SpendEnergy(Skills[Skill.PlayerBladeStorm].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerBladeStorm)] = Skills[Skill.PlayerBladeStorm].Cooldown;

        }
    }

    public void AutoCastBladeStorm()
    {
        if(Random.Range(0,100)<AutoCastChance)
        {
            CreateBladeStormBlade(BladestormInstance);
        }
    }

    void CreateBladeStormBlade(TempPlayerSkill t)
    {
        int armorGain = t.Upgrades[SkillUpgrade.BladeStormArmor] * (int)SkillUpgrade.BladeStormArmor.SpecialAmount;
        float sizeMulti = 1 + t.Upgrades[SkillUpgrade.BladeStormSize] * SkillUpgrade.BladeStormSize.SpecialAmount;
        var instance = Instantiate(BladeStormPrefab, transform);
        instance.transform.localPosition = new Vector3(0, Random.Range(-1f, 2f));
        instance.transform.localScale = new Vector3(Random.Range(1.8f, 3)*sizeMulti, 2.5f);
        instance.GetComponent<BladeStormControl>().Init(t, gameObject);
        statController.GainArmor(armorGain);
        
    }
    #endregion

    #region AoE

    protected void StartAoE(GameObject effect, Vector3 startingLocation)
    {
        if(facingRight)
            CreateAoE(effect, startingLocation, 270);
        else
            CreateAoE(effect, startingLocation, 90);

    }

    protected void StartAoE(GameObject effect, Vector3 startingLocation, float rotation)
    {
        CreateAoE(effect, startingLocation, rotation);

    }

    void CreateAoE(GameObject effect, Vector3 startingLocation, float rotation)
    {
        GameObject instance = Instantiate(effect);
        instance.transform.position = startingLocation;
      //  instance.transform.parent = transform;
        instance.GetComponent<AreaEffectControl>().setUnitType(unitType);
        instance.GetComponent<AreaEffectControl>().SetUp(currentUpgradedSkill.Amount,  rotation, currentUpgradedSkill.Bleed, currentUpgradedSkill.HealthOnHit, gameObject);
        instance.GetComponent<AreaEffectControl>().OnHitHeal.AddListener(Heal);
        if (currentSkill.Equals(Skill.PlayerCull))
        {
            var armor = currentUpgradedSkill.Upgrades[SkillUpgrade.CullArmorOnKill] * SkillUpgrade.CullArmorOnKill.SpecialAmount;
            //var heal = currentUpgradedSkill.Upgrades[SkillUpgrade.CullHealthOnKill] * SkillUpgrade.CullHealthOnKill.SpecialAmount;
            var AoE = currentUpgradedSkill.Upgrades[SkillUpgrade.CullSize] * SkillUpgrade.CullSize.SpecialAmount;
            int BleedConsume = currentUpgradedSkill.Upgrades[SkillUpgrade.CullBleedConsume] * (int)SkillUpgrade.CullBleedConsume.SpecialAmount;
            var slowMulti = 1+ currentUpgradedSkill.Upgrades[SkillUpgrade.CullSlowDamage] * SkillUpgrade.CullSlowDamage.SpecialAmount;
            instance.GetComponent<CullEffectControl>().UpgradesSetup(0, (int)armor,slowMulti,BleedConsume);
            instance.transform.localScale *= 1 + 1 * (AoE);
        }

        }


    #endregion

    #region projectiles


    /// <summary>
    /// creates projectile
    /// </summary>
    /// <param name="proj"></param>
    /// <param name="startingLocation"></param>
    /// <param name="direction"></param>
    /// <param name="rotation"></param>
   protected override void CreateProjectile(GameObject proj, Vector3 startingLocation, Vector3 direction, float rotation)
    {
        GameObject instance = Instantiate(proj);
        instance.transform.position = startingLocation;
        instance.GetComponent<ProjectileControl>().setUnitType(unitType);
        instance.GetComponent<ProjectileControl>().SetUp(currentUpgradedSkill.Amount, direction, rotation, currentUpgradedSkill.Piercing, currentUpgradedSkill.Bleed, currentUpgradedSkill.HealthOnHit);
        instance.GetComponent<ProjectileControl>().OnHitHeal.AddListener(Heal);
        if (currentProjectilePrefab.Equals(ChakramPrefab))
        {
            ChakramControl ctrl = instance.GetComponent<ChakramControl>();
            {
                //upgrades
                var returnDamageUpgrades = currentUpgradedSkill.Upgrades[SkillUpgrade.ChakramReturnDamage] * (int)SkillUpgrade.ChakramReturnDamage.SpecialAmount;
                var returnExposes = currentUpgradedSkill.Upgrades[SkillUpgrade.ChakramReturnDamage];
                var armorGain = currentUpgradedSkill.Upgrades[SkillUpgrade.ChakramArmor] * (int)SkillUpgrade.ChakramArmor.SpecialAmount;
                var DamageMulti = 1 - 0.05f * currentUpgradedSkill.Upgrades[SkillUpgrade.ChakramCooldownLessDamage];
                var StationaryDamage = currentUpgradedSkill.Upgrades[SkillUpgrade.ChakramStationarySpin] * (int)SkillUpgrade.ChakramStationarySpin.SpecialAmount;

                ctrl.UpgradesSetup(gameObject, armorGain, returnDamageUpgrades, returnExposes, StationaryDamage ,DamageMulti);

            }
        }
        else if (currentProjectilePrefab.Equals(BladeProjectilePrefab)) // if current projecitle is normal attack
        {
            var slowUpgrades = currentUpgradedSkill.Upgrades[SkillUpgrade.NormalRangedAttackSlow] * (int)SkillUpgrade.NormalRangedAttackSlow.SpecialAmount;
            instance.GetComponent<ProjectileControl>().AddSlow(slowUpgrades);
        }
    }
    protected override void CreateProjectile(GameObject proj, Vector3 startingLocation, Vector3 direction)
    {
        GameObject instance = Instantiate(proj);
        instance.transform.position = startingLocation;
        instance.GetComponent<ProjectileControl>().setUnitType(unitType);
        instance.GetComponent<ProjectileControl>().SetUp(currentUpgradedSkill.Amount, direction,  currentUpgradedSkill.Piercing, currentUpgradedSkill.Bleed, currentUpgradedSkill.HealthOnHit);
        instance.GetComponent<ProjectileControl>().OnHitHeal.AddListener(Heal);
        if(currentProjectilePrefab.Equals(BladeProjectilePrefab)) // if current projecitle is normal attack
        {
            var slowUpgrades = currentUpgradedSkill.Upgrades[SkillUpgrade.NormalRangedAttackSlow] * (int)SkillUpgrade.NormalRangedAttackSlow.SpecialAmount;
            instance.GetComponent<ProjectileControl>().AddSlow(slowUpgrades);
        }
    }

    void DefaultRangedAttack()
    {

            UpperBody.GetComponent<Animator>().Play("DefaultRangedAttack");
            currentUpgradedSkill = Skills[currentSkill];
            inAction = true;
            isAttacking = true;
            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
            {
                facingRight = true;
                FlipCharacter(true);
            }
            else
            {
                facingRight = false;
                FlipCharacter(false);
            }
            if ((WalkLeft || WalkRight) && Grounded)
                Walk();
        }
     

    void DefaultRangedThrow()
    {
        Vector3 startingLocation = new Vector3(0, 0, 0);
        startingLocation = FrontHand.transform.position;
        if(currentProjectilePrefab== RazorBladePrefab)
        {
            Vector3 dir;
            if (facingRight)
            {
                if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] != 1 && currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] != 3)
                {
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, 0, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 270);
                }
                else
                {
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.033f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 271.7f);
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.033f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 268.3f);
                }
                if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 2 || currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 4)
                {
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.2f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 280);
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.2f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 260);
                }
                if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 4)
                {
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.3f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 285);
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.3f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 255);
                } else if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 3)
                {
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.166f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 278.3f);
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.166f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 261.7f);
                }
                dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.1f, 0);
                ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 275);
                dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.1f, 0);
                ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 265);

            }
            else
            {
                if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] != 1 && currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] != 3)
                {
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, 0, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 90);

                }
                else
                {
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.033f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 88.3f);
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.033f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 91.7f);
                }
                if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 2 || currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 4)
                {
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.2f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 80);
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.2f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 100);
                }
                if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 4)
                {
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.3f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 75);
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.3f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 105);
                }
                else if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 3)
                {
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.166f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 81.7f);
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.166f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 98.3f);
                }
                dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.1f, 0);
                ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 85);
                dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.1f, 0);
                ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 95);
            }
        }
        else if(currentProjectilePrefab == ChakramPrefab)
        {
            ThrowProjectile(currentProjectilePrefab, startingLocation);
        }
        else if(currentProjectilePrefab == SpikeTrapPrefab)
        {
            float projSpeed;
            if (facingRight)
            {
                projSpeed = 400;
                
            }
            else
            {
                projSpeed = -400;

            }

            var traps = currentUpgradedSkill.Upgrades[SkillUpgrade.SpikeTrapMultipleTraps];
            for (int i = 0; i <= traps; i++)
            {


                var instance = Instantiate(currentProjectilePrefab);

                instance.transform.position = startingLocation;
                instance.GetComponent<SpikeTrapControl>().setUnitType(unitType);
                instance.GetComponent<Rigidbody2D>().AddForce(new Vector2(projSpeed, 10));
                instance.GetComponent<SpikeTrapControl>().SetUp(currentUpgradedSkill.Amount, 0, currentUpgradedSkill.Bleed, currentUpgradedSkill.HealthOnHit, gameObject);
                instance.GetComponent<SpikeTrapControl>().AddSlow(6);

                var spikes = currentUpgradedSkill.Upgrades[SkillUpgrade.SpikeTrapSpikes] * (int)SkillUpgrade.SpikeTrapSpikes.SpecialAmount;
                var bleedDmg = currentUpgradedSkill.Upgrades[SkillUpgrade.SpikeTrapIncBleedDmg] * SkillUpgrade.SpikeTrapIncBleedDmg.SpecialAmount;
                var slowDmg = currentUpgradedSkill.Upgrades[SkillUpgrade.SpikeTrapSlowDamage] * SkillUpgrade.SpikeTrapSlowDamage.SpecialAmount;

                instance.GetComponent<SpikeTrapControl>().Init(slowDmg,bleedDmg,spikes, 4 - (traps * 2));

                if (facingRight)
                    projSpeed += 200;
                else
                    projSpeed -= 200;
            }
        }
        else
            ThrowProjectile(currentProjectilePrefab, startingLocation);
    }

    void FinishDefaultRangedAttack()
    {
        inAction = false;
        currentSkill = Skill.Default;
    }
    #endregion

    #region SpellCast

    void DefaultSpellAttack()
    {

        UpperBody.GetComponent<Animator>().Play("SpellUse");
        currentUpgradedSkill = Skills[currentSkill];
        inAction = true;
        isAttacking = true;
        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
        {
            facingRight = true;
            FlipCharacter(true);
        }
        else
        {
            facingRight = false;
            FlipCharacter(false);
        }
        if ((WalkLeft || WalkRight) && Grounded)
            Walk();
    }

    void DefaultSpellCast()
    {
        if (currentSkill.Equals(Skill.PlayerRainOfBlades))
        {
            var instance = Instantiate(RainOfBladesPrefab);
            instance.transform.position = RoBLocation;
            instance.GetComponent<RainOfBladesControl>().Init(currentUpgradedSkill);

        }
        else if (currentSkill.Equals(Skill.PlayerBladeStorm))
        {
            int blades = currentUpgradedSkill.Upgrades[SkillUpgrade.BladeStormBlades] + 3; // 3 is base blades
            for (int i = 0; i < blades; i++)
            {
                CreateBladeStormBlade(currentUpgradedSkill);
            }
        }
    }
    #endregion

    #region Jump related

    /// <summary>
    /// calls player to jump
    /// </summary>
    void StartJump()
    {
        if (StaminaCanUse((float)PlayerStaminaCost.Jump))
        {
            isJumpingUp = true;
            rigidbody.AddForce(new Vector2(0, JumpForce));

            MainStatController.SpendStamina((float)PlayerStaminaCost.Jump);

        }
    }

    /// <summary>
    /// plays jump animation for lower body
    /// </summary>
    void Jump()
    {
        if (!inAction && !isAttacking)
            UpperBody.GetComponent<Animator>().Play("IdleUpper");
        LowerBody.GetComponent<Animator>().Play("JumpUpwardsLower"); //play upwards animation

    }

    /// <summary>
    /// plays falling animation for lower body
    /// </summary>
    void Falling()
    {
        LowerBody.GetComponent<Animator>().Play("JumpDownwardsLower"); //play upwards animation
    }

    #endregion
    #region Dash related

    void DashFalling()
    {
        LowerBody.GetComponent<Animator>().Play("DashFallingLower"); //play upwards animation
    }

    /// <summary>
    /// Calls player to Dash forward
    /// plays dash animation for lower body
    /// changes player's transform/physics to match animation
    /// </summary>
    void Dash()
    {
        if (StaminaCanUse((float)PlayerStaminaCost.Dash))
        {
            ClearCheckers();
            if (WalkRight)
            {
                facingRight = true;
                FlipCharacter(true);
            }
            else if (WalkLeft)
            {
                facingRight = false;
                FlipCharacter(false);
            }
            UpperBody.GetComponent<Animator>().Play("DashUpper");
            LowerBody.GetComponent<Animator>().Play("DashLower");
            inAction = true;
            isDashing = true;
            isJumpingUp = false;

            LowerBodyAnimationStateController.startDash();
            MainStatController.SpendStamina((float)PlayerStaminaCost.Dash);

        }
    }


    /// <summary>
    /// called when a dash is finished. invoked from onDashFinish in MainCharacterLowerAnimationController.cs
    /// sets inaction as false
    /// </summary>
    public void FinishDash()
    {
        isDashing = false;
        
        inAction = false;
        if (!Grounded)
        {
            if (!facingRight)
                rigidbody.velocity = new Vector2(-10, 0);
            else
                rigidbody.velocity = new Vector2(10, 0);
            isDashFalling = true;
        }
    }

    public void canDashAttack()
    {
        canQueueDashAttack = true;
    }

    public void finishDashAttack()
    {
        // inAction = false;
        isDashAttacking = false;
    }

    #endregion
    #region Walking
    /// <summary>
    /// Calls player to walk 
    /// plays walking animation for lower body
    /// </summary>
    void Walk()
    {
        isDashing = false;
        if (!isAttacking)
        {
            if (WalkRight)
            {
                FlipCharacter(true);
                facingRight = true;
            }
            else if (WalkLeft)
            {
                FlipCharacter(false);
                facingRight = false;
            }

        }
        if (!isAttacking && !inAction && Input.GetKey(KeyCode.LeftShift)&&MainStatController.CurrentStamina()>0&&sprintAgain)
        {
            UpperBody.GetComponent<Animator>().Play("SprintUpper");

            LowerBody.GetComponent<Animator>().Play("Sprint");

            sprinting = true;
        }

        else
        {
            
            sprinting = false;
            if (facingRight && WalkRight || !facingRight && WalkLeft)
                LowerBody.GetComponent<Animator>().Play("Walk");
            else
                LowerBody.GetComponent<Animator>().Play("WalkBackwards");
        }
    }
    #endregion
    #region checkers
    /// <summary>
    /// Calls player to play idle animation
    /// </summary>
    void SetIdle()
    {
        //UpperBody.GetComponent<Animator>().Play("IdleUpper");
      //  if(!LowerBody.GetComponent<Animator>().GetBool("inAction"))
        LowerBody.GetComponent<Animator>().Play("IdleLower");
    }

    /// <summary>
    /// called when an action is finished. invoked from onActionFinish in CharacterAnimationStateController.cs
    /// sets inaction as false
    /// </summary>
    public void FinishAction()
    {

        inAction = false;
        finishDashAttack();
    }



    /// <summary>
    /// clears action checkers when starting a new action to prevent errors
    /// </summary>
    void ClearCheckers()
    {
        inAction = false;
        isAttacking = false;
        isDashing = false;
        isSlashing = false;
        isNormalAttacking = false;
        isJumpingUp = false;
        queueDashAttack = false;
        canQueueDashAttack = false;
        queueDashRangedAttack = false;
        isPerformingMovementSkill = false;
    }


    protected override void StopAttacking()
    {
        isAttacking = false;
        isDashing = false;
        isSlashing = false;
        inAction = false;
        currentSkill = Skill.Default;

        finishDashAttack();

    }

    /// <summary>
    /// whether the stat controller has enough energy to use cost
    /// </summary>
    /// <param name="cost">amount of energy that needs to be used</param>
    /// <returns></returns>
    bool EnergyCanUse(float cost)
    {
        return MainStatController.CurrentEnergy() > cost;
    }

    /// <summary>
    /// whether the stat controller has enough stamina to use cost
    /// </summary>
    /// <param name="cost">amount of stamina that needs to be used</param>
    /// <returns></returns>
    bool StaminaCanUse(float cost)
    {
        return MainStatController.CurrentStamina() > cost;
    }
    #endregion
    #region Detection

    /// <summary>
    /// below functions are for turning on/off collision on weapons
    /// used when unit is attacking 
    /// </summary>
    private void TurnNAWeaponDetectionOn()
    {

        switch(NormalAttackRotation)
        {
            case 1:
                TurnDetectionOnFrontBlade();
                break;
            case 2:
                TurnDetectionOnBackBlade();
                break;
        }
  
    }
    private void TurnNAWeaponDetectionOff()
    {

        switch (NormalAttackRotation)
        {
            case 1:
                TurnDetectionOffFrontBlade();
                break;
            case 2:
                TurnDetectionOffBackBlade();
                break;
        }
        
                

    }


    protected void TurnDetectionOnFrontBlade()
    {
        FrontBladeCollider.DetectionOn();
    }


    protected void TurnDetectionOffFrontBlade()
    {
        FrontBladeCollider.DetectionOff();
    }

    protected void TurnDetectionOnBackBlade()
    {
        BackBladeCollider.DetectionOn();
    }


    protected void TurnDetectionOffBackBlade()
    {
        BackBladeCollider.DetectionOff();
    }
    /// <summary>
    /// overrides function to give player energy when a normal attack strikes a target
    /// </summary>
    /// <param name="target"></param>
    public override void HitTarget(GameObject target)
    {
        float currentAmount = currentUpgradedSkill.Amount;
        if (currentSkill == Skill.PlayerPuncture)
        {
            int BleedDamageUpgrades = currentUpgradedSkill.Upgrades[SkillUpgrade.PunctureExtraDamage]; //deals extra damage if enemy is bleeding
            int HealUpgrades = currentUpgradedSkill.Upgrades[SkillUpgrade.PunctureHeal]; //heals if enemy is bleeding
            int ExposeUpgrade = currentUpgradedSkill.Upgrades[SkillUpgrade.PunctureExpose]; // exposes enemy on hit if taken
            UnitStats targetStats = target.GetComponent<UnitStats>();
            if (targetStats.isBleeding())
            {
                currentAmount += SkillUpgrade.PunctureExtraDamage.SpecialAmount* BleedDamageUpgrades;
                MainStatController.Heal(SkillUpgrade.PunctureHeal.SpecialAmount * HealUpgrades);
            }
            if(ExposeUpgrade>0)
            {
                targetStats.Expose((int)SkillUpgrade.PunctureExpose.SpecialAmount);
            }
        }
        else if(currentSkill == Skill.PlayerSlash)
        {
            int CooldownOnHitUpgrades = currentUpgradedSkill.Upgrades[SkillUpgrade.SlashCooldownOnHit]; //cooldown reduction per enemy hit
            int ExposeUpgrade = currentUpgradedSkill.Upgrades[SkillUpgrade.SlashExpose]; // exposes enemy on hit if taken
            UnitStats targetStats = target.GetComponent<UnitStats>();
            if (CooldownOnHitUpgrades>0)
            {

                SkillCooldowns[SkillPool.IndexOf(Skill.PlayerSlash)] -= SkillUpgrade.SlashCooldownOnHit.SpecialAmount;
            }
            if (ExposeUpgrade > 0)
            {
                targetStats.Expose((int)SkillUpgrade.SlashExpose.SpecialAmount);
            }
        }
        else if(currentSkill == Skill.PlayerNormalAttack)
        {
            int armorUpgrades = currentUpgradedSkill.Upgrades[SkillUpgrade.NormalAttackArmor];
            if(armorUpgrades>0&&statController.CurrentArmor()<=0)
            {
                statController.GainArmor(armorUpgrades * SkillUpgrade.NormalAttackArmor.SpecialAmount);
            }
            //if this NA havent hit anyone yet (trigger one time effects)
            if(!NAHitOnce)
            {
                if(SlottedSkills.Contains(Skill.PlayerFortify))
                {
                    SkillCooldowns[SkillPool.IndexOf(Skill.PlayerFortify)] -= NAFortifyCooldownReduction;
                }


            }
            NAHitOnce = true;
        }
        MainStatController.DealDamage(target, currentAmount);
        MainStatController.Heal(currentUpgradedSkill.HealthOnHit);
        MainStatController.BleedTarget(target, currentUpgradedSkill.Bleed);
        MainStatController.GainEnergy(currentUpgradedSkill.EnergyOnHit);




    }
    #endregion


    #region Skill slots

    void UseSkillInSlot(int index)
    {

        isDashing = false;
        isSlashing = false;
        Skill skillInSlot = SlottedSkills[index];

        if (skillInSlot == Skill.PlayerRazorBlades)
            RazorBlades();
        else if (skillInSlot == Skill.PlayerSlash)
            Slash();
        else if (skillInSlot == Skill.PlayerPuncture)
            Puncture();
        else if (skillInSlot == Skill.PlayerCull)
            Cull();
        else if (skillInSlot == Skill.PlayerRainOfBlades)
            RainOfBlades();
        else if (skillInSlot == Skill.PlayerBladeStorm)
            BladeStorm();
        else if (skillInSlot == Skill.PlayerChakram)
            Chakram();
        else if (skillInSlot == Skill.PlayerPerforate)
            Perforate();
        else if (skillInSlot == Skill.PlayerFortify)
            Fortify();
        else if (skillInSlot == Skill.PlayerSpikeTrap)
            SpikeTrap();
    }
    #endregion
    #region player skill upgrades

    /// <summary>
    /// learns a skill, adding it to player's skill pool
    /// </summary>
    /// <param name="s"></param>
    public void LearnSkill(Skill s)
    {


        if (s == Skill.PlayerNormalAttack)
        {
            Skills.Add(Skill.PlayerNormalAttack, new TempPlayerSkill(PlayerSkill.PlayerNormalAttack));
        }
        else if (s == Skill.PlayerNormalRangedAttack)
        {
            Skills.Add(Skill.PlayerNormalRangedAttack, new TempPlayerSkill(PlayerSkill.PlayerNormalRangedAttack));
        }
        else if (s == Skill.PlayerRazorBlades)
        {
            Skills.Add(Skill.PlayerRazorBlades, new TempPlayerSkill(PlayerSkill.PlayerRazorBlades));
        }
        else if (s == Skill.PlayerPuncture)
        {
            Skills.Add(Skill.PlayerPuncture, new TempPlayerSkill(PlayerSkill.PlayerPuncture));
        }
        else if (s == Skill.PlayerSlash)
        {
            Skills.Add(Skill.PlayerSlash, new TempPlayerSkill(PlayerSkill.PlayerSlash));
        }
        else if (s == Skill.PlayerCull)
        {
            Skills.Add(Skill.PlayerCull, new TempPlayerSkill(PlayerSkill.PlayerCull));
        }
        else if (s == Skill.PlayerRainOfBlades)
        {
            Skills.Add(Skill.PlayerRainOfBlades, new TempPlayerSkill(PlayerSkill.PlayerRainOfBlades));
        }
        else if (s == Skill.PlayerBladeStorm)
        {
            Skills.Add(Skill.PlayerBladeStorm, new TempPlayerSkill(PlayerSkill.PlayerBladeStorm));
        }
        else if (s == Skill.PlayerChakram)
        {
            Skills.Add(Skill.PlayerChakram, new TempPlayerSkill(PlayerSkill.PlayerChakram));
        }
        else if (s == Skill.PlayerPerforate)
        {
            Skills.Add(Skill.PlayerPerforate, new TempPlayerSkill(PlayerSkill.PlayerPerforate));
        }
        else if (s == Skill.PlayerFortify)
        {
            Skills.Add(Skill.PlayerFortify, new TempPlayerSkill(PlayerSkill.PlayerFortify));
        }
        else if(s== Skill.PlayerSpikeTrap)
        {
            Skills.Add(Skill.PlayerSpikeTrap, new TempPlayerSkill(PlayerSkill.PlayerSpikeTrap));
        }
        if (!(s.Equals(Skill.PlayerNormalAttack)) && !(s.Equals(Skill.PlayerNormalRangedAttack)))
        {
            SkillPool.Add(s);
            SkillCooldowns.Add(0);
            //if there is an empty skill slot, slot the skill in it
            for (int i = 0; i < SlottedSkills.Count; i++)
            {
                if (SlottedSkills[i] == Skill.Default)
                {
                    SlotSkill(s, i,false);
                    break;
                }
            }
        }

    }
    
    /// <summary>
    /// slots a skill  
    /// </summary>
    /// <param name="s"></param>
    /// <param name="index">index of skill</param>
    public void SlotSkill(Skill s,int index, bool swapping)
    {
        int finalIndex = 0;
        bool fullslots = true;
        //if slotted skills contains this skill, swap it with the skill in selected index
        if (s!=Skill.Default && SlottedSkills.Contains(s)&&swapping)
        {
           // Debug.Log("s");
            finalIndex = index;
            var otherIndex = SlottedSkills.IndexOf(s);
            SlottedSkills[otherIndex] = SlottedSkills[index];
            InitUIcontrollerIcon(otherIndex, SlottedSkills[index]);
            SlottedSkills[index] = s;
           

        }
        else
        {
            for (int i = 0; i < SlottedSkills.Count; i++)
            { if (i == index)
                {
                    SlottedSkills[i] = s;
                    finalIndex = i;
                }

            }

        }

        fullySlotted = fullslots;

        InitUIcontrollerIcon(finalIndex, s);
    }

    /// <summary>
    /// inits an icon in ui controller of the currently equipped skill
    /// </summary>
    /// <param name="index"></param>
    /// <param name="s"></param>
    void InitUIcontrollerIcon(int index, Skill s)
    {
        if (s != Skill.Default)
        {
            //Debug.Log(s.Icon.IconName);
            uicontroller.InitIcon(index + 1, s.Icon.IconName, Skills[s].EnergyCost, (int)Skills[s].Cooldown);
        }
        else
        {
            //Debug.Log("default");
            uicontroller.InitIcon(index + 1, "", 0, 0);
        }
    }
 
    /// <summary>
    /// updates skill to include stats of all its upgrades
    /// </summary>
    /// <param name="s"></param>
    protected void UpdateSkill(Skill s)
    {
        Skills[s].ResetToBase();

        foreach(KeyValuePair<SkillUpgrade,int> upgrade in Skills[s].Upgrades)
        {
            for (int i = 0; i < upgrade.Value; i++)
            {
                Skills[s].Amount += upgrade.Key.IncreasedAmount;
                Skills[s].Cooldown -= Skills[s].BaseSkill.UpgradableSkill.Cooldown*(1-upgrade.Key.CooldownMultiplier);
                Skills[s].EnergyCost -= Skills[s].BaseSkill.UpgradableSkill.EnergyCost*(1- upgrade.Key.CostMultiplier);
                Skills[s].ProjectileSpeed *= upgrade.Key.ProjectileSpeedMultiplier;
                Skills[s].Bleed += upgrade.Key.BleedAmount;
                Skills[s].HealthOnHit += upgrade.Key.HealthOnHit;
                Skills[s].EnergyOnHit += upgrade.Key.EnergyOnHit;
                Skills[s].Piercing = upgrade.Key.ProjectilePierce;
            }
       

        }
        //if it is bladestorm, and cast on hit is taken, save a copy of the skill for later use
        if(s.Equals(Skill.PlayerBladeStorm))
        {
           AutoCastChance =  Skills[s].Upgrades[SkillUpgrade.BladeStormAutoCast]*(int)SkillUpgrade.BladeStormAutoCast.SpecialAmount;
            if(AutoCastChance>0)
            {
                BladestormInstance = Skills[s];
            }
        }
        else if(s.Equals(Skill.PlayerFortify))
        {
            NAFortifyCooldownReduction = Skills[s].Upgrades[SkillUpgrade.FortifyCooldownOnNA] * (int)SkillUpgrade.FortifyCooldownOnNA.SpecialAmount;
        }
        //need to add update icon as well
        uicontroller.InitIcon(SlottedSkills.IndexOf(s)+1, s.Icon.IconName, Skills[s].EnergyCost, (int)Skills[s].Cooldown);
    }

    /// <summary>
    /// adds a point in an upgrade for a skill
    /// </summary>
    /// <param name="s">skill to be upgraded</param>
    /// <param name="index">index of upgrade</param>
    public void UpgradeSkill(Skill s, SkillUpgrade u)
    {
        if(Skills[s].Upgrades.ContainsKey(u))
            Skills[s].Upgrades[u] += 1;
        UpdateSkill(s);
    }

    public void SaveSkills()
    {
        ProgressSave.Instance.SaveSkills(Skills, SlottedSkills);
    }

    public Dictionary<Skill,TempPlayerSkill> GetSkillsWithUpgrades(){return Skills;}
    public List<Skill> GetSlottedSkills() { return SlottedSkills; }
    public List<Skill> GetSkillPool() { return SkillPool; }

    #endregion


    public override void Death()
    {
        inAction = true;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        UpperBody.GetComponent<Animator>().Play("DeathUpper");
        LowerBody.GetComponent<Animator>().Play("DeathLower");
        StartCoroutine(DeathDecay());

    }

    void Heal(int amount)
    {
        MainStatController.Heal(amount);
    }

    void OnHealthLost()
    {
        AutoCastBladeStorm();
    }

}
