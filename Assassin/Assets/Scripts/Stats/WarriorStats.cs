using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorStats : UnitStats
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        maxHealth = 150;
        currentHealth = maxHealth;
    }

}
