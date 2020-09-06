using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : EnemyController
{

    [SerializeField] WeaponColliderController SwordCollider;
    Animator animator;
    int SwingAdvanceSpeed = 10;
    int SwingAdvanceSpeed2 = 30;
    int JabAdvanceSpeed = 25;
    protected override void Start()
    {

        base.Start();
        animator = GetComponent<Animator>();
      BasewalkSpeed =   walkSpeed = 8;
        xScale = 0.5f;
        SwordCollider.WeaponCollisionEnterEvent.AddListener(HitTarget);
        SkillPool.Add(Skill.WarriorSwing);
        SkillPool.Add(Skill.WarriorJab);
        SkillPool.Add(Skill.WarriorNA);
        for (int i = 0; i < SkillPool.Count; i++)
        {
            SkillCooldowns.Add(0);
        }

    }

    protected override void Update()
    {
        base.Update();
        
    }

    protected override void startTriggeredAction(Skill s)
    {
        if (s == Skill.WarriorSwing)
            SwingAttack();
        else if (s == Skill.WarriorJab)
            JabAttack();
        else if (s == Skill.WarriorNA)
            NormalAttack();

    }


    #region Normal Attack
    void NormalAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.WarriorNA;
        animator.Play("NormalAttack");

    }

    void finishNormalAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
    }



    #endregion


    #region SwingAttack
    /// <summary>
    /// start swing attack
    /// </summary>
    void SwingAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.WarriorSwing;
        animator.Play("WarriorSwingAttack");
        if (moveRight)
        {
            rigidbody.velocity = new Vector2(SwingAdvanceSpeed, rigidbody.velocity.y);
        }
        else
        {
            rigidbody.velocity = new Vector2(-SwingAdvanceSpeed, rigidbody.velocity.y);
        }
    }


    /// <summary>
    /// below 4 functions are called by animation to trigger changes in states etc.
    /// </summary>
    void finishSwingAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
    }

    public void EndSwingAdvance()
    {
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

    public void StartSwing()
    {
        TurnWeaponDetectionOn();
        if (moveRight)
        {
            rigidbody.velocity = new Vector2(SwingAdvanceSpeed2, rigidbody.velocity.y);
        }
        else
        {
            rigidbody.velocity = new Vector2(-SwingAdvanceSpeed2, rigidbody.velocity.y);
        }
    }

    public void EndSwing()
    {
        TurnWeaponDetectionOff();
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

    #endregion

    #region Jab Attack

    void JabAttack()
    {
        isAttacking = true;
        inAction = true;
        currentSkill = Skill.WarriorJab;
        animator.Play("WarriorJabAttack");
    }

    /// <summary>
    /// below 3 functions are called by animation to trigger changes in states etc.
    /// </summary>
    void finishJabAttack()
    {
        isAttacking = false;
        inAction = false;
        currentSkill = Skill.Default;
    }

    public void StartJab()
    {
        TurnWeaponDetectionOn();
        if (moveRight)
        {
            rigidbody.velocity = new Vector2(JabAdvanceSpeed, rigidbody.velocity.y);
        }
        else
        {
            rigidbody.velocity = new Vector2(-JabAdvanceSpeed, rigidbody.velocity.y);
        }
    }

    public void EndJab()
    {
        TurnWeaponDetectionOff();
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }
    #endregion

    public void TurnWeaponDetectionOn()
    {
        SwordCollider.DetectionOn();
    }

    public void TurnWeaponDetectionOff()
    {
        SwordCollider.DetectionOff();
    }

    
    public override void Death()
    {
        inAction = true;
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        animator.Play("Death");
        StartCoroutine(DeathDecay());
    }



}
