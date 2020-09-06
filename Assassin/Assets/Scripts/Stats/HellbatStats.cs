﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellbatStats : UnitStats
{

    public override void Init(int level)
    {
        maxHealth = 40 + 15 * (level - 1);
        currentHealth = maxHealth;
    }
}
