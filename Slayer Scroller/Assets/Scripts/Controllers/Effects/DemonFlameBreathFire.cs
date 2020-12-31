using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFlameBreathFire : AreaEffectControl
{
    int burn = 0;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(RefreshHit());
    }

    IEnumerator RefreshHit()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.6f);
            hitList = new List<GameObject>();
        }
    }

    public void SetBurn(int b)
    {
        burn = b;
    }

    protected override void onHit(GameObject g)
    {
        base.onHit(g);
        g.GetComponent<UnitStats>().Burn(burn);
    }
}
