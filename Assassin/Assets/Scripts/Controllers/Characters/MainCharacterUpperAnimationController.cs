using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MainCharacterUpperAnimationController : CharacterAnimationStateController
{

    public UnityEvent onNAFinish = new UnityEvent(); //invoked when player finished an NA
    public UnityEvent onRangedNAFinish = new UnityEvent();
    public UnityEvent onNACollisionStart = new UnityEvent();
    public UnityEvent onRangedNAThrow = new UnityEvent();
    public UnityEvent onDefaultRangedThrow = new UnityEvent();
    public UnityEvent onDefaultRangedFinish = new UnityEvent();

    public void DefaultThrowProjectile()
    {
        onDefaultRangedThrow.Invoke();
    }

    public void EndDefaultRangedAttack()
    {
        onDefaultRangedFinish.Invoke();
    }

    public void finishNA()
    {
        onNAFinish.Invoke();
    }

    public void startNA()
    {
        onNACollisionStart.Invoke();
    }

    public void throwRangedNA()
    {
        onRangedNAThrow.Invoke();
    }

    public void finishRangedNA()
    {
        onRangedNAFinish.Invoke();
    }


}
