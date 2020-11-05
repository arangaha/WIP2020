using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerforateControl : AreaEffectControl
{
    int armorGain = 0;
    bool refreshBleed = false;
    public void UpdradesSetup(int armorGainOnHit, bool RefreshBleed)
    {
        armorGain = armorGainOnHit;
        refreshBleed = RefreshBleed;
    }

    protected override void onHit(GameObject g)
    {
        owner.GetComponent<UnitStats>().GainArmor(armorGain);
        if (g.GetComponent<UnitStats>().CurrentArmor() > 0&&armorGain>0)
        {
            damage *= 1.5f;
        }
        if(refreshBleed)
        {
            g.GetComponent<UnitStats>().RefreshBleed();
        }
        base.onHit(g);

    }
}
