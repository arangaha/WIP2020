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
    public UnityEvent WeaponDetectionOnFront = new UnityEvent();
    public UnityEvent WeaponDetectionOnBack = new UnityEvent();
    public UnityEvent WeaponDetectionOffFront = new UnityEvent();
    public UnityEvent WeaponDetectionOffBack = new UnityEvent();
    public UnityEvent onSlashFinish = new UnityEvent();
    public UnityEvent onCullEffect = new UnityEvent();
    public UnityEvent onDefaultSpellUse = new UnityEvent();

    #region skill event triggers
    public void DefaultSpellUse()
    {
        onDefaultSpellUse.Invoke();
    }

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

    public void finishSlash()
    {
        onSlashFinish.Invoke();
    }

    public void StartCullEffect()
    {
        onCullEffect.Invoke();
    }
    #endregion

    #region detections
    public void WeaponOnFront()
    {
        WeaponDetectionOnFront.Invoke();
    }

    public void WeaponOnBack()
    {
        WeaponDetectionOnBack.Invoke();
    }

    public void WeaponOffFront()
    {
        WeaponDetectionOffFront.Invoke();
    }

    public void WeaponOffBack()
    {
        WeaponDetectionOffBack.Invoke();
    }
    #endregion

}
