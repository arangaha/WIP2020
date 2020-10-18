using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullEffectControl : AreaEffectControl
{

    int healOnKillAmount = 0; // heal to user if this skill kills
    int armorOnKillAmount = 0; //armor to user if this skill kills
    float slowMulti = 1;
    int bleedConsumeMultiplier = 0;
    public void UpgradesSetup(int healOnkill, int armorOnKill, float slowDmg, int bleedConsume)
    {
        healOnKillAmount = healOnkill;
        armorOnKillAmount = armorOnKill;
        slowMulti = slowDmg;
        bleedConsumeMultiplier = bleedConsume;
    }


    protected override void onHit(GameObject g)
    {
        hitList.Add(g);
        UnitStats targetStats = g.GetComponent<UnitStats>();
        float increasedDamage = damage + damage * (1 - targetStats.PercentageHealth()); //deal increased damage equal to percentage missing health

        if (slowMulti > 1 && targetStats.isSlowed())
        {
            increasedDamage *= slowMulti;
        }
        targetStats.TakeDamage(increasedDamage);
        if (targetStats.CurrentHealth() <= 0)
        {
            owner.GetComponent<UnitStats>().GainArmor(armorOnKillAmount);
            owner.GetComponent<UnitStats>().Heal(healOnKillAmount);
        }
        if (bleed > 0)
            targetStats.Bleed(bleed);
        if (slow > 0)
            targetStats.Slow(slow);
        if(bleedConsumeMultiplier>0)
        {
            targetStats.ConsumeBleed(bleedConsumeMultiplier);
        }
        WeaponCollisionEnterEvent.Invoke(g);
        OnHitEvent.Invoke();
        OnHitHeal.Invoke(heal);
    }


}
