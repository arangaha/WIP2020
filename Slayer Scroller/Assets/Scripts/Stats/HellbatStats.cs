using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellbatStats : UnitStats
{

    public override void Init(int level)
    {

        Level = level;
        maxHealth = 40 + 20 * (level - 1);
        currentHealth = maxHealth;
    }
}
