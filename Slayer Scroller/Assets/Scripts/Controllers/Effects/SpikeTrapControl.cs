using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapControl : AreaEffectControl
{
   [SerializeField] int spikes;
    [SerializeField] int exposes;
    GameObject spikePrefab;
    float slowDamage = 0;
    float incBleedDmg;
    protected void Start()
    {
        spikePrefab= Resources.Load("Prefabs/Projectiles/SpikeProjectile") as GameObject;

    }

    public void Init(float SlowDamage, float increasedBleedDamage,int spikeProjectiles, int Exposes)
    {
        spikes = spikeProjectiles;
        exposes = Exposes;
        slowDamage = SlowDamage;
        incBleedDmg = increasedBleedDamage;
        StartCoroutine(ExpireCountdown());
    }

    protected override void onHit(GameObject g)
    {
        if (g.GetComponent<UnitStats>().isSlowed())
            damage *= 1 + slowDamage;
        base.onHit(g);
        g.GetComponent<UnitStats>().Expose(exposes);
        if (incBleedDmg > 0)
            g.GetComponent<UnitStats>().IncreaseBleedTaken(incBleedDmg);
        Detonate();

    }
   public void Deployed()
    {
        canDetect = true;
    }

    IEnumerator ExpireCountdown()
    {
        yield return new WaitForSeconds(12);
        Detonate();
    }

    void Detonate()
    {

        GetComponent<Animator>().Play("Spring");
        canDetect = false;
        if (spikes > 0)
        {
            for (int i = 0; i < spikes; i++)
            {
                var instance = Instantiate(spikePrefab);
                ProjectileControl p = instance.GetComponent<ProjectileControl>();
                p.setUnitType(UnitType.Ally);
                float xDir = Random.Range(-20, 20);
                float yDir = 20 - Mathf.Abs(xDir);
                p.SetUp(5, new Vector2(xDir, yDir), true, 0, 0);
                p.DontCollideWithWalls(1);
                p.transform.position = transform.position;

            }
        }
    }


}
