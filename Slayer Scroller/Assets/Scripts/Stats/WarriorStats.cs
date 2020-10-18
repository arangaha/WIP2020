using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorStats : UnitStats
{

    public override void Init(int level)
    {
        Level = level;
        maxHealth = 100+60*(level-1);
        currentHealth = maxHealth;
    }

}
