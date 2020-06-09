using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected float currentHealth = 100;
    [SerializeField] protected float maxStamina = 100;
    [SerializeField] protected float currentStamina = 100;
    [SerializeField] protected float maxEnergy = 60;
    [SerializeField] protected float currentEnergy = 0;
    [SerializeField] protected float attack = 10;
    [SerializeField] protected float armor = 0;
    [SerializeField] protected UnitController unitController;

    [SerializeField] protected int BleedAmount= 0;
    [SerializeField] protected int BleedCounter = 4;

    UIController uicontroller;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        uicontroller = GameObject.Find("WorldUI").GetComponent<UIController>();
        maxHealth = 100;
        currentHealth = maxHealth;
        attack = 0;
        armor = 0;
        unitController = GetComponent<UnitController>();
        StartCoroutine(PerSecondUpdate());
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// update that runs through per second
    /// </summary>
    /// <returns></returns>
    IEnumerator PerSecondUpdate()
    {
        while(true)
        {
            if (BleedAmount >0&&BleedCounter>0)
            {
                TakeDamage(BleedAmount);
                BleedCounter -= 1;
                if (BleedCounter <= 0)
                    BleedAmount = 0;
            }
            yield return new WaitForSeconds(1);
        }
    }
    #region health related
    /// <summary>
    /// deals damage to target
    /// </summary>
    /// <param name="g"></param>
    /// <param name="amount"></param>
    public void DealDamage(GameObject g, float amount)
    {
        UnitStats u = g.GetComponent<UnitStats>();
        u.TakeDamage(amount);
    }

    /// <summary>
    /// receives damage
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {

        currentHealth -= (amount - armor);
        UpdateHealth();
        uicontroller.CreateDamageText(amount, transform.position);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        UpdateHealth();
        uicontroller.CreateDamageText(-amount, transform.position);
    }

    #endregion

    #region stamina related
    public void SpendStamina(float amount)
    {
        currentStamina -= amount;
    }

    public float CurrentStamina()
    {
        return currentStamina;

    }
#endregion
    #region energy related
    public void SpendEnergy(float amount)

    {
        currentEnergy -= amount;
    }

    public void GainEnergy(float amount)
    {
        currentEnergy += amount;
        if (currentEnergy > maxEnergy)
            currentEnergy = maxEnergy;
    }

    public float CurrentEnergy()
    {
        return currentEnergy;
    }
    #endregion

    #region bleed related
    public void BleedTarget(GameObject g, int amount)
    {
        UnitStats u = g.GetComponent<UnitStats>();
        u.Bleed(amount);
    }

    public void Bleed(int amount)
    {
        BleedCounter = 4;
        BleedAmount += amount;
    }
    #endregion
    void Die()
    {
        unitController.Death();
    }

   protected virtual void UpdateHealth()
    {
        unitController.UpdateHealth(currentHealth / maxHealth);
    }
}

