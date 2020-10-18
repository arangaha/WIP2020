using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellHoundStats : UnitStats
{

    public override void Init(int level)
    {
        maxHealth = 200 + 80 * (level - 1);
        currentHealth = maxHealth;
    }

}
