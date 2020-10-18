using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellHoundEliteStats : UnitStats
{
   [SerializeField] HellhoundVariantController variantControl;
    public override void Init(int level)
    {
        Level = level;
        maxHealth = 600 + 300 * (level - 1);
        currentHealth = maxHealth;
        damageScaling = 0.1f;
    }

    protected override void Die()
    {
        variantControl.Death();
    }

    public void FadeAway()
    {
        OnDeath.Invoke();
        OnDeath.RemoveAllListeners();
    }

    public override float DamageMulti()
    {
        return 1 + (Level - 5) * damageScaling;
    }
}
