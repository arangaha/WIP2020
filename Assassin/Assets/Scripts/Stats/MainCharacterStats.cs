using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterStats : UnitStats
{

    [SerializeField] protected Transform HealthBar;
    [SerializeField] protected Transform EnergyBar;
    [SerializeField] protected Transform StaminaBar;
    protected override void Start()
    {
        base.Start();
        maxStamina = 100;
        currentStamina = maxStamina;
        maxEnergy = 60;
        currentEnergy = 0;
        HealthBar = GameObject.Find("PlayerHealthBar").transform;
        StaminaBar = GameObject.Find("PlayerStaminaBar").transform;
        EnergyBar = GameObject.Find("PlayerEnergyBar").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentEnergy < maxEnergy)
            currentEnergy += 0.02f;
        if (currentStamina < maxStamina)
            currentStamina += 0.3f;
        UpdateResourceBar();
    }


    /// <summary>
    /// updates visuals of resource bars in UI
    /// </summary>
    void UpdateResourceBar()
    {
        HealthBar.localScale = new Vector3(currentHealth / maxHealth *0.4f, 0.5f, 1);
        StaminaBar.localScale = new Vector3(currentStamina/ maxStamina * 0.365f, 0.2f, 1);
        EnergyBar.localScale = new Vector3(currentEnergy / maxEnergy * 0.3f, 0.15f, 1);
    }

    
}
