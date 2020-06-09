using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAnimationStateController : MonoBehaviour
{
   protected bool inAction = false; //whether this animator is in action
   protected Animator animator;
    public UnityEvent onActionFinish = new UnityEvent();
    public UnityEvent onAttackFinish = new UnityEvent();

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void startAction()
    {
        animator.SetBool("inAction", true);
        inAction = true;
    }

    public void finishAction()
    {
        animator.SetBool("inAction", false);
        inAction = false;
        onActionFinish.Invoke();
    }

    public void finishAttack()
    {
        onAttackFinish.Invoke();
    }

}
