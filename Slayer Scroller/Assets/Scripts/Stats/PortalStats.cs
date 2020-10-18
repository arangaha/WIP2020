using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalStats : UnitStats
{
    public override void Init(int level)
    {
        Level = level;
        maxHealth = 1000 + 100 * (level - 1);
        currentHealth = maxHealth;
    }
}
