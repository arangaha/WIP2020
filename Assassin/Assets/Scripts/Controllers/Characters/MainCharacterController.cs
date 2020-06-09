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

    [SerializeField] bool isNormalAttacking = false;
    [SerializeField] int NormalAttackRotation = 1; // which normal attack the character is currently on
    [SerializeField] int NormalRangedAttackRotation = 1; // which normal ranged attack the character is currently on

    [SerializeField] float dashSpeed = 40f;
    [SerializeField] float JumpForce = 15f;


    [SerializeField] private GameObject UpperBody;
    [SerializeField] private GameObject LowerBody;
    [SerializeField] private GameObject FrontHand;
    [SerializeField] private GameObject BackHand;

    [SerializeField] private GroundColliderController GroundCollider;
    private MainCharacterUpperAnimationController UpperBodyAnimationStateController;
    private MainCharacterLowerAnimationController LowerBodyAnimationStateController;
  [SerializeField]  private WeaponColliderController FrontBladeCollider;
    [SerializeField] private WeaponColliderController BackBladeCollider;
    private MainCharacterStats MainStatController;

    #region skill prefabs
    GameObject currentProjectilePrefab;
    private GameObject BladeProjectilePrefab;
    private GameObject RazorBladePrefab;

    #endregion



    UIController uicontroller;
    #region player skills
    protected Dictionary<Skill, TempPlayerSkill> Skills = new Dictionary<Skill, TempPlayerSkill>(); //skill list for players. key: skill, value: skill containing the upgrades
    protected TempPlayerSkill currentUpgradedSkill = new TempPlayerSkill(PlayerSkill.Default);
    
    #endregion
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        uicontroller = GameObject.Find("WorldUI").GetComponent<UIController>();
        MainStatController = GetComponent<MainCharacterStats>();
        unitType = UnitType.Ally;
        walkSpeed = 12;
        dashSpeed = 25;
        JumpForce = 1000;
        xScale = 0.5f;

        SkillPool.Add(Skill.PlayerRazorBlades);
   


        Skills.Add(Skill.PlayerNormalAttack, new TempPlayerSkill(PlayerSkill.PlayerNormalAttack));
        Skills.Add(Skill.PlayerNormalRangedAttack, new TempPlayerSkill(PlayerSkill.PlayerNormalRangedAttack));
        Skills.Add(Skill.PlayerRazorBlades, new TempPlayerSkill(PlayerSkill.PlayerRazorBlades));



        //below are manually acquired upgrades
      //  UpgradeSkill(Skill.PlayerNormalAttack, SkillUpgrade.NormalAttackHeal);
      //  UpgradeSkill(Skill.PlayerNormalAttack, SkillUpgrade.NormalAttackBleed);
        //UpgradeSkill(Skill.PlayerNormalRangedAttack, SkillUpgrade.NormalRangedAttackAdditionalProjectiles);
        //UpgradeSkill(Skill.PlayerNormalRangedAttack, SkillUpgrade.NormalRangedAttackPierce);
       //  UpgradeSkill(Skill.PlayerRazorBlades, SkillUpgrade.RazorBladeBleed);
      //  UpgradeSkill(Skill.PlayerRazorBlades, SkillUpgrade.RazorBladeHeal);
        //  UpgradeSkill(Skill.PlayerRazorBlades, SkillUpgrade.RazorBladesCooldown);
        // UpgradeSkill(Skill.PlayerRazorBlades, SkillUpgrade.RazorBladesCooldown);
       // UpgradeSkill(Skill.PlayerRazorBlades, SkillUpgrade.RazorBladeAdditionalProjectiles);
      //  UpgradeSkill(Skill.PlayerRazorBlades, SkillUpgrade.RazorBladeAdditionalProjectiles);

        uicontroller.InitIcon(1, Icon.RazorBlades.IconName, Skills[Skill.PlayerRazorBlades].EnergyCost, (int)Skills[Skill.PlayerRazorBlades].Cooldown);
        for (int i = 0; i < SkillPool.Count; i++)
        {
            SkillCooldowns.Add(0);
        }

        UpperBodyAnimationStateController = UpperBody.GetComponent<MainCharacterUpperAnimationController>();
        UpperBodyAnimationStateController.onActionFinish.AddListener(FinishAction);
        UpperBodyAnimationStateController.onNACollisionStart.AddListener(TurnNAWeaponDetectionOn);
        UpperBodyAnimationStateController.onNAFinish.AddListener(finishNA);
        UpperBodyAnimationStateController.onAttackFinish.AddListener(stopAttacking);
        UpperBodyAnimationStateController.onRangedNAThrow.AddListener(NormalRangedAttackThrow);
        UpperBodyAnimationStateController.onRangedNAFinish.AddListener(finishRangedNA);
        UpperBodyAnimationStateController.onDefaultRangedFinish.AddListener(FinishDefaultRangedAttack);
        UpperBodyAnimationStateController.onDefaultRangedThrow.AddListener(DefaultRangedThrow);


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
                uicontroller = GameObject.Find("WorldUI").GetComponent<UIController>();
    }

    // Update is called once per frame
   protected override void Update()
    {
        base.Update();

        Grounded = GroundCollider.grounded;



        //if not in action, these conditions are checked
        if (!inAction)
        {
            //sets animation of lower body
            if (Grounded)
            {
                if (WalkLeft || WalkRight)
                    Walk();
                else 
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
            else if(Input.GetMouseButtonDown(1))
            {

                NormalRangedAttack();
            }
            else if(Input.GetKeyDown("q"))
            {
                RazorBlades();
            }
            else if (Input.GetKeyDown("space") && Grounded)
            {
                StartJump();

            }
            else if (Input.GetKeyDown(KeyCode.LeftControl)&&!isDashing)
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


            }


        }


        if (Input.GetKeyUp("a"))
        {
            WalkLeft = false;
        }
        if (Input.GetKeyUp("d"))
        {
            WalkRight = false;
        }

        //changes character velocity/position based on states
        if (isDashing)
        {
            if(!facingRight)
                rigidbody.velocity = new Vector2(-dashSpeed, 0);
            else
                rigidbody.velocity = new Vector2(dashSpeed, 0);
        // if player left clicks during a dash, queue for a dash attack
            if (Input.GetMouseButtonDown(0))
            {
                queueDashAttack=true;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                queueDashRangedAttack = true;
            }
                if (canQueueDashAttack&&queueDashAttack)
            {
                NormalAttack();
                canQueueDashAttack = false;
                queueDashRangedAttack = false;
                queueDashAttack = false;
            }
            else if(canQueueDashAttack&&queueDashRangedAttack)
            {
                NormalRangedAttack();
                canQueueDashAttack = false;
                queueDashRangedAttack = false;
                queueDashAttack = false;
            }
        }
       else if(WalkLeft)
        {
            rigidbody.velocity = new Vector2(-walkSpeed, rigidbody.velocity.y);
            
        }
        else if(WalkRight)
        {
            rigidbody.velocity = new Vector2(walkSpeed, rigidbody.velocity.y);
        }
        else if(Grounded)
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }

        if (rigidbody.velocity.y<0&&!Grounded)
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
        if(inAction)
        {
            rigidbody.gravityScale = 0;
        }
        else
        {
            rigidbody.gravityScale = 4;
        }


        //updates icons for skills
        for(int i = 0; i<SkillPool.Count; i++)
        {
            float cooldown = SkillCooldowns[i];
            if (cooldown > 0)
                uicontroller.UpdateCooldown(i + 1, cooldown/Skills[SkillPool[i]].Cooldown);
            if (MainStatController.CurrentEnergy() < Skills[SkillPool[i]].EnergyCost)
                uicontroller.cannotUseSkill(i + 1);
            else
                uicontroller.canUseSkill(i + 1);

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
        if (!isAttacking&&EnergyCanUse((float)Skills[Skill.PlayerRazorBlades].EnergyCost) && SkillCooldowns[SkillPool.IndexOf(Skill.PlayerRazorBlades)]<=0)
        {
            currentSkill = Skill.PlayerRazorBlades;
            currentUpgradedSkill = Skills[currentSkill];
            currentProjectilePrefab = RazorBladePrefab;
            DefaultRangedAttack();
            MainStatController.SpendEnergy((float)Skills[Skill.PlayerRazorBlades].EnergyCost);
            SkillCooldowns[SkillPool.IndexOf(Skill.PlayerRazorBlades)] = (float)Skills[Skill.PlayerRazorBlades].Cooldown;
        }
    }

    #endregion

    #region projectiles

    /// <summary>
    /// throws a projectile with default rotation and direction values
    /// default rotation: 270 if facing right, 90 if facing left
    /// default direction: 0 degree of the facing direction
    /// </summary>
    /// <param name="proj"></param>
    /// <param name="startingLocation"></param>
    protected void ThrowProjectile(GameObject proj, Vector3 startingLocation)
    {

        Vector3 dir = new Vector3(0, 0, 0);
        float projSpeed;
        float rotation;

        if (facingRight)
        {
            projSpeed = currentUpgradedSkill.ProjectileSpeed;
            rotation = 270;
        }
        else
        {
            projSpeed = -currentUpgradedSkill.ProjectileSpeed;
            rotation = 90;

        }
        dir = new Vector3(projSpeed, 0, 0);
        CreateProjectile(proj, startingLocation, dir, rotation);

    }

    /// <summary>
    /// throws a projectile with specified rotation and direction values
    /// </summary>
    protected void ThrowProjectile(GameObject proj, Vector3 startingLocation, Vector3 direction, float rotation)
    {

        CreateProjectile(proj, startingLocation, direction, rotation);

    }

    /// <summary>
    /// creates projectile
    /// </summary>
    /// <param name="proj"></param>
    /// <param name="startingLocation"></param>
    /// <param name="direction"></param>
    /// <param name="rotation"></param>
    void CreateProjectile(GameObject proj, Vector3 startingLocation, Vector3 direction, float rotation)
    {
        GameObject instance = Instantiate(proj);
        instance.transform.position = startingLocation;
        instance.GetComponent<ProjectileControl>().setUnitType(unitType);
        instance.GetComponent<ProjectileControl>().SetUp(currentUpgradedSkill.Amount, direction, rotation, currentUpgradedSkill.Piercing, currentUpgradedSkill.Bleed, currentUpgradedSkill.HealthOnHit);
        instance.GetComponent<ProjectileControl>().OnHitHeal.AddListener(Heal);
    }

    void DefaultRangedAttack()
    {
        if (!isNormalAttacking && !isAttacking)
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
     }

    void DefaultRangedThrow()
    {
        Vector3 startingLocation = new Vector3(0, 0, 0);
        startingLocation = FrontHand.transform.position;
        if(currentSkill.Equals(Skill.PlayerRazorBlades))
        {
            Vector3 dir;
            if (facingRight)
            {
                if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] != 1)
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
                if(currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 2)
                {
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.2f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 280);
                    dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.2f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 260);
                }
                dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.1f, 0);
                ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 275);
                dir = new Vector3(currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.1f, 0);
                ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 265);
            }
            else
            {
                if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] != 1)
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
                if (currentUpgradedSkill.Upgrades[SkillUpgrade.RazorBladeAdditionalProjectiles] == 2)
                {
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.2f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 80);
                    dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.2f, 0);
                    ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 100);
                }

                dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * 0.1f, 0);
                ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 85);
                dir = new Vector3(-currentUpgradedSkill.ProjectileSpeed, currentUpgradedSkill.ProjectileSpeed * -0.1f, 0);
                ThrowProjectile(currentProjectilePrefab, startingLocation, dir, 95);
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
        if (facingRight&&WalkRight||!facingRight&&WalkLeft)
        LowerBody.GetComponent<Animator>().Play("Walk");
        else
            LowerBody.GetComponent<Animator>().Play("WalkBackwards");
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
        isNormalAttacking = false;
        queueDashAttack = false;
        canQueueDashAttack = false;
        queueDashRangedAttack = false;
    }


    public void stopAttacking()
    {
        isAttacking = false;
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

    protected  void TurnDetectionOnFrontBlade()
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
        MainStatController.DealDamage(target, currentUpgradedSkill.Amount);
        MainStatController.Heal(currentUpgradedSkill.HealthOnHit);
        MainStatController.BleedTarget(target, currentUpgradedSkill.Bleed);
        MainStatController.GainEnergy(currentUpgradedSkill.EnergyOnHit);
    }
    #endregion
    #region player skill upgrades

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
                Skills[s].Cooldown -= (1-upgrade.Key.CooldownMultiplier);
                Skills[s].EnergyCost *= upgrade.Key.CostMultiplier;
                Skills[s].ProjectileSpeed *= upgrade.Key.ProjectileSpeedMultiplier;
                Skills[s].Bleed += upgrade.Key.BleedAmount;
                Skills[s].HealthOnHit += upgrade.Key.HealthOnHit;
                Skills[s].EnergyOnHit += upgrade.Key.EnergyOnHit;
                Skills[s].Piercing = upgrade.Key.ProjectilePierce;
            }
        }
    }

    /// <summary>
    /// adds a point in an upgrade for a skill
    /// </summary>
    /// <param name="s">skill to be upgraded</param>
    /// <param name="index">index of upgrade</param>
    protected void UpgradeSkill(Skill s, SkillUpgrade u)
    {
        if(Skills[s].Upgrades.ContainsKey(u))
            Skills[s].Upgrades[u] += 1;
        UpdateSkill(s);
    }
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


}
