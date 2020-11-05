using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class UnitStats : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected float currentHealth = 100;
    [SerializeField] protected float maxStamina = 100;
    [SerializeField] protected float currentStamina = 100;
    [SerializeField] protected float maxEnergy = 60;
    [SerializeField] protected float currentEnergy = 0;
    [SerializeField] protected float attack = 10;
    [SerializeField] protected float armor = 0; //prevents damage from taken, lost after taken damage equal to amount
    [SerializeField] protected UnitController unitController;
    [SerializeField] protected float protection = 1; //multiplier for the next hit received

    #region Debuffs
    [SerializeField] protected int BleedAmount= 0;
    [SerializeField] protected int BleedCounter = 4; //countdown for bleed
    [SerializeField] protected float extraBleedMulti = 0;
    [SerializeField] protected float extraBleedCounter = 8;
    [SerializeField] protected int slowCounter = 0; //countdown for slow
    #endregion

    [SerializeField] protected int Level = 0;

    [SerializeField] protected int ExposeCounter = 0; // exposed for # of hits the unit will receieve. exposed hits deal 20% more damage
    public UnityEvent OnDeath = new UnityEvent();
    UIController uicontroller;
    protected float  damageScaling = 0.05f; //added multiplier to damage per stage

    // Start is called before the first frame update
    protected virtual void Start()
    {
        uicontroller = GameObject.Find("WorldUI").GetComponent<UIController>();

        unitController = GetComponent<UnitController>();
        StartCoroutine(PerSecondUpdate());
    }

    public abstract void Init(int level);


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
                TakeBleedDamage(BleedAmount);
                BleedCounter -= 1;
                if (BleedCounter <= 0)
                    BleedAmount = 0;
            }
            if(slowCounter>0)
            {
                unitController.SlowSpeed();
                slowCounter -= 1;
            }
            else
            {
                unitController.NormalSpeed();
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
    public virtual void TakeDamage(float amount)
    {
        float totalAmount = amount;
        //if unit is exposed, -1 exposure and cause this hit to deal 20% more
        if(ExposeCounter>0)
        {
            ExposeCounter --;
            totalAmount *= 1.2f;
            if (ExposeCounter <= 0)
                unitController.HideExposeEffect();
        }
        if(protection!=1)
        {
            totalAmount *= protection;
            protection = 1;
            unitController.HideProtectionEffect();

        }
        if (totalAmount > armor)
        {
            currentHealth -= (totalAmount - armor);
            armor = 0;
        }
        else
        {
            armor -= totalAmount;
        }

        UpdateHealth();
        unitController.UpdateArmor(armor);
        uicontroller.CreateDamageText(totalAmount, transform.position);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
    public virtual void TakeBleedDamage(float amount)
    {
        float totalAmount = amount*(1+extraBleedMulti);

            currentHealth -=totalAmount ;
        if (extraBleedMulti > 0)
            extraBleedCounter--;
        if (extraBleedCounter <= 0)
            extraBleedMulti = 0;

        UpdateHealth();
        uicontroller.CreateDamageText(totalAmount, transform.position, true);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
    /// <summary>
    /// gets the percentage of current health to max health
    /// </summary>
    /// <returns>number between 0 and 1 to represent the percentage</returns>
    public float PercentageHealth()
    {
        return currentHealth / maxHealth;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        UpdateHealth();
        uicontroller.CreateDamageText(-amount, transform.position);
    }

    public void GainArmor(float amount)
    {
        armor += amount;
        unitController.UpdateArmor(armor);
    }

    public float CurrentArmor()
    {
        return armor;
    }

    public float CurrentHealth()
    {
        return currentHealth;
    }

    public float MaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// causes the hit recieved to have this multiplier instead. (float below 1 for damage reduction)
    /// </summary>
    /// <param name="multi"></param>
    public void Protect (float multi)
    {
        protection = multi;
        if(protection<1)
        unitController.ShowProtectionEffect();
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

    public bool isBleeding()
    {
        if (BleedAmount > 0)
            return true;
        else
            return false;

    }

    public void RefreshBleed()
    {if(BleedAmount>0)
        BleedCounter = 4;
    }

    public void RemoveBleed()
    {
        BleedCounter = 0;
        BleedAmount = 0;
    }

    public void IncreaseBleedTaken(float multi)
    {
        extraBleedMulti = multi;
        extraBleedCounter = 8;
    }
    #endregion

    #region debuffs related
    /// <summary>
    /// expose this unit for i amount of hits
    /// </summary>
    /// <param name="i"></param>
    public void Expose(int i)
    {
        if (i > 0)
        {
            ExposeCounter += i;
            unitController.ShowExposeEffect();
        }
    }

    /// <summary>
    /// slows target i amount of seconds, if slow is less than current slow, slow is ignored
    /// </summary>
    /// <param name="i"></param>
    public void Slow(int i)
    {
        if(slowCounter<i)
        {
            unitController.SlowSpeed();
            slowCounter = i;
        }
    }

    public bool isSlowed()
    {
        return slowCounter > 0 ? true : false;
    }
    #endregion
    protected virtual void Die()
    {
        unitController.Death();
        OnDeath.Invoke();
        OnDeath.RemoveAllListeners();
    }

   protected virtual void UpdateHealth()
    {
        unitController.UpdateHealth(currentHealth / maxHealth,currentHealth);
    }

    public virtual float DamageMulti()
    {
        return 1 + (Level - 1) * damageScaling;
    }

    /// <summary>
    /// causes unit to take their total bleed times multiplier as damage, removing the bleeds
    /// </summary>
    /// <param name="multiplier"></param>
    public virtual void ConsumeBleed ( int multiplier)
    {
        TakeDamage(BleedAmount * BleedCounter * multiplier);

        BleedCounter = 0;
        BleedAmount = 0;
    }
}

