using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainOfBladesControl : MonoBehaviour
{

    GameObject ProjectilePrefab;
   [SerializeField] float SpawnInterval = 0.1f; //interval of frequency of blade spawns
    [SerializeField] float currentSpawnCount = 0; 
    TempPlayerSkill UpgradedSkill;
    [SerializeField] float MaxDuration = 3;//duration of portal
    [SerializeField] int bigBladeCounter = 0; // counter for spawning big blade if upgrade is taken
    [SerializeField] int movementSpeed = 0;

    /// <summary>
    /// below 5 variables are for target detection
    /// </summary>
    /// 
    [SerializeField] TargetDetection targetDetect;
    [SerializeField] protected List<GameObject> NearbyAllies = new List<GameObject>();
    [SerializeField] protected List<GameObject> NearbyEnemies = new List<GameObject>();
    [SerializeField] protected GameObject ClosestAlly;
    [SerializeField] protected GameObject ClosestEnemy;

    public void Init(TempPlayerSkill t)
    {
        ProjectilePrefab = Resources.Load("Prefabs/Projectiles/BladeProjectile") as GameObject;
        UpgradedSkill = t;
        MaxDuration += t.Upgrades[SkillUpgrade.RoBDuration] * SkillUpgrade.RoBDuration.SpecialAmount; //extend duration if upgraded
        SpawnInterval = 1 / (10 + t.Upgrades[SkillUpgrade.RoBInterval] * SkillUpgrade.RoBInterval.SpecialAmount); //increase spawn rate of blades if upgraded
        movementSpeed =t.Upgrades[SkillUpgrade.RoBMove] * (int)SkillUpgrade.RoBMove.SpecialAmount;
        StartCoroutine(SpawnBlades());
        targetDetect.ChangeUnitType(UnitType.Ally);
        targetDetect.onUpdateAllies.AddListener(updateNearbyAllies);
        targetDetect.onUpdateEnemies.AddListener(updateNearbyEnemies);
       
    }
    

    IEnumerator SpawnBlades()
    {
        while(currentSpawnCount <MaxDuration)
        {
            var instance = Instantiate(ProjectilePrefab);
            ProjectileControl p = instance.GetComponent<ProjectileControl>();
            p.setUnitType(UnitType.Ally);
            bool bleed = Random.Range(0,100)< UpgradedSkill.Upgrades[SkillUpgrade.RoBBleed] * SkillUpgrade.RoBBleed.SpecialAmount;//whether his blade will cause bleed
            bool piercing = false;
            int dmgMulti = 1;
            //fourth big blade counter
            if (UpgradedSkill.Upgrades[SkillUpgrade.RoBFourth]>0)
            {
                piercing = true;
                bigBladeCounter++;
                if(bigBladeCounter==4)
                {
                    bigBladeCounter = 0;
                    p.transform.localScale *= 1.5f;
                    dmgMulti = 1 + (int)(UpgradedSkill.Upgrades[SkillUpgrade.RoBFourth] * SkillUpgrade.RoBFourth.SpecialAmount);
                }
            }
            int xDir = Random.Range(-(int)(UpgradedSkill.ProjectileSpeed * 0.6f), (int)(UpgradedSkill.ProjectileSpeed * 0.6f));
            p.SetUp(UpgradedSkill.Amount * dmgMulti, new Vector2(xDir, -UpgradedSkill.ProjectileSpeed), piercing, bleed ? 1 : 0, 0);
            p.transform.position = transform.position;
            if(UpgradedSkill.Upgrades[SkillUpgrade.RoBSlow]>0)
            {
                p.AddSlow(UpgradedSkill.Upgrades[SkillUpgrade.RoBSlow] * (int)SkillUpgrade.RoBSlow.SpecialAmount);
            }

            currentSpawnCount+=SpawnInterval;

            //movement
            if(movementSpeed>0&& ClosestEnemy != null)
            {
                if(ClosestEnemy.transform.position.x>transform.position.x+0.5f)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(movementSpeed, 0);
                }
                else if(ClosestEnemy.transform.position.x < transform.position.x - 0.5f)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(-movementSpeed, 0);
                }
                else
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                }
            }
            else
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            yield return new WaitForSeconds(SpawnInterval);
        }
        Destroy(gameObject);
    }

    public virtual void updateNearbyAllies(List<GameObject> allies)
    {
        NearbyAllies = allies;
        ClosestAlly = closestUnit(NearbyAllies);
    }
    public virtual void updateNearbyEnemies(List<GameObject> enemies)
    {
        NearbyEnemies = enemies;
        ClosestEnemy = closestUnit(NearbyEnemies);
    }
    public GameObject closestUnit(List<GameObject> listOfUnits)
    {
        if (listOfUnits.Count > 0)
        {
            float closestDistance = getUnitDistance(listOfUnits[0]);
            int index = 0;
            for (int i = 1; i < listOfUnits.Count; i++)
            {
                if (getUnitDistance(listOfUnits[i]) < closestDistance)
                {

                    closestDistance = getUnitDistance(listOfUnits[i]);
                    index = i;

                }
            }
            return listOfUnits[index];
        }
        else
        {
            return null;
        }
    }
    protected float getUnitDistance(GameObject unit)
    {

        if (unit)
        {
            float tempx = unit.transform.position.x - transform.position.x;
            float tempy = unit.transform.position.y - transform.position.y;
            float unitDistance = Mathf.Sqrt(tempx * tempx + tempy * tempy);

            return unitDistance;
        }
        else
            return Mathf.Infinity;
    }
}
