using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorStats : UnitStats
{

    public override void Init(int level)
    {
        maxHealth = 100+40*(level-1);
        currentHealth = maxHealth;
    }

}
