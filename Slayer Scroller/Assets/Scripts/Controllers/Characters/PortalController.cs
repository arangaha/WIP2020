using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PortalController : UnitController
{

    bool Active = true;
    StageController stageControl;


    protected override void Start()
    {
        base.Start();
        stageControl= GameObject.Find("World").GetComponent<StageController>();
        StartCoroutine(SpawnEnemies());
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    public override void Death()
    {
        onDeath.Invoke();
        GetComponent<Animator>().Play("PortalClose");
        Active = false;
    }
    protected override void CreateProjectile(GameObject proj, Vector3 startingLocation, Vector3 direction, float rotation)
    {
        GameObject instance = Instantiate(proj);
        instance.transform.position = startingLocation;
        instance.GetComponent<ProjectileControl>().setUnitType(unitType);

        instance.GetComponent<ProjectileControl>().SetUp(currentSkill.Amount * GetComponent<UnitStats>().DamageMulti(), direction, rotation, currentSkill.Piercing, currentSkill.Bleed * (int)GetComponent<UnitStats>().DamageMulti(), currentSkill.HealthOnHit);

    }


    protected override void CreateProjectile(GameObject proj, Vector3 startingLocation, Vector3 direction)
    {
        GameObject instance = Instantiate(proj);
        instance.transform.position = startingLocation;
        instance.GetComponent<ProjectileControl>().setUnitType(unitType);
        instance.GetComponent<ProjectileControl>().SetUp(currentSkill.Amount * GetComponent<UnitStats>().DamageMulti(), direction, currentSkill.Piercing, currentSkill.Bleed * (int)GetComponent<UnitStats>().DamageMulti(), currentSkill.HealthOnHit);

    }

    /// <summary>
    /// coroutine that periodidcally spawns enemies
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(3);
        while(Active)
        {
            stageControl.SpawnWarrior(transform.position);
            yield return new WaitForSeconds(10);
        }
    }
}
