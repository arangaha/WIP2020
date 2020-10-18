using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MainCharacterLowerAnimationController : CharacterAnimationStateController
{
    bool inDash = false; //whether this animator is dashing
    public UnityEvent onDashFinish = new UnityEvent();
    public UnityEvent onDashTransition = new UnityEvent();
    public UnityEvent onDashAttackFinish = new UnityEvent();

    public void startDash()
    {
        animator.SetBool("inDash", true);
        inDash = true;
        startAction();
    }

    public void finishDash()
    {
        animator.SetBool("inDash", false);
        inDash = false;
        onDashFinish.Invoke();
       
    }

    /// <summary>
    /// called to tell animator it is ready to transition to another animation from dash. 
    /// use: transitioning from dash to dash attack
    /// </summary>

    public void dashTransition()
    {
        animator.SetBool("inDash", false);
        inDash = false;
        onDashTransition.Invoke();
    }

    public void finishDashAction()
    {
        onDashAttackFinish.Invoke();
    }
}
