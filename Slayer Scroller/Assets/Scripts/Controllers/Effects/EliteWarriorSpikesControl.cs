using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteWarriorSpikesControl : AreaEffectControl
{
    bool FacingRight = false;
    int RemainingWaves=0;
    public void SetupSpike(bool facingRight, int remainingWaves)
    {
        if (facingRight)
            transform.localScale = new Vector3(-1, 1, 1);
        FacingRight = facingRight;
        RemainingWaves = remainingWaves;
    }

    void CreateNewWave()
    {
        if (RemainingWaves > 0)
        {
            var instance = Instantiate(gameObject);
        instance.GetComponent<EliteWarriorSpikesControl>().setUnitType(unitType);
        instance.GetComponent<EliteWarriorSpikesControl>().SetUp(damage, bleed, heal, owner);
            instance.GetComponent<EliteWarriorSpikesControl>().SetupSpike(FacingRight, RemainingWaves - 1);
            if (FacingRight)
                instance.transform.position = new Vector3(transform.position.x + 2, transform.position.y, 1);
            else
                instance.transform.position = new Vector3(transform.position.x - 2, transform.position.y, 1);

        }
    }
}
