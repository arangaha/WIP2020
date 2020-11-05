using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteWarriorStats : UnitStats
{
    [SerializeField] bool invulnerable;

    public override void Init(int level)
    {
        Level = level;
        maxHealth = 600 * (level - 1);
        currentHealth = maxHealth;
        damageScaling = 0.1f;
    }

    public override void TakeDamage(float amount)
    {
        if(!invulnerable)
            base.TakeDamage(amount);
    }

    public void SetInvulnerable()
    {
        invulnerable = true;
    }

    public void SetVulnerable()
    {
        invulnerable = false;
    }

    protected override void Die()
    {
        unitController.Death();
    }

    public override float DamageMulti()
    {
        return 1 + (Level - 10) * damageScaling;
    }
    public void FadeAway()
    {
        OnDeath.Invoke();
        OnDeath.RemoveAllListeners();
    }
}
