using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class MainCharacterStats : UnitStats
{

    [SerializeField] protected Transform HealthBar;
    [SerializeField] protected Transform EnergyBar;
    [SerializeField] protected Transform StaminaBar;
    [SerializeField] protected TextMeshProUGUI HealthNumber;
    [SerializeField] protected TextMeshProUGUI StaminaNumber;
    [SerializeField] protected TextMeshProUGUI EnergyNumber;
    public UnityEvent onHealthLost = new UnityEvent();
    protected override void Start()
    {
        base.Start();

        HealthBar = GameObject.Find("PlayerHealthBar").transform;
        StaminaBar = GameObject.Find("PlayerStaminaBar").transform;
        EnergyBar = GameObject.Find("PlayerEnergyBar").transform;

        HealthNumber = GameObject.Find("PlayerHealthNumber").GetComponent<TextMeshProUGUI>();
        StaminaNumber = GameObject.Find("PlayerStaminaNumber").GetComponent<TextMeshProUGUI>();
        EnergyNumber = GameObject.Find("PlayerEnergyNumber").GetComponent<TextMeshProUGUI>();
    }

    public override void Init(int level)
    {
        Level = level;
        //init stats based on player level
        maxHealth = 100+10*(level-1);
        currentHealth = maxHealth;
        armor = 0;
        maxStamina = 100;
        currentStamina = maxStamina;
        maxEnergy = 60;
        currentEnergy = 0; //start at 0
    }

    // Update is called once per frame
    void Update()
    {
        if (currentEnergy < maxEnergy)
            currentEnergy += 0.02f;
        if (currentStamina < maxStamina)
            currentStamina += 0.1f+0.01f * (Level - 1);
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
        HealthNumber.text = "" + (int)currentHealth;
        EnergyNumber.text = "" + (int)currentEnergy;
        StaminaNumber.text = "" + (int)currentStamina;
    }
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        if(armor<=0)
        {
            onHealthLost.Invoke();
        }
    }

}
