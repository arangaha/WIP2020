using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonStats : UnitStats
{
    public override void Init(int level)
    {

        Level = level;
        maxHealth = 200 + 80 * (level - 1);
        currentHealth = maxHealth;
    }

    public override float DamageMulti()
    {
        return 1 + (Level - 10) * damageScaling;
    }
}
