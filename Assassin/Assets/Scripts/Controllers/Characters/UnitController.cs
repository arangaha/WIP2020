using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class UnitController : MonoBehaviour
{
    [SerializeField] protected bool Grounded = false; // whether unit is grounded
    [SerializeField] protected bool Idle = true; // whether unit is idle (not performing any actions)
    [SerializeField] protected bool inAction = false;

    [SerializeField] protected bool isAttacking = false;
    [SerializeField] protected float walkSpeed = 10f;
    [SerializeField] protected bool WalkLeft = false;
    [SerializeField] protected bool WalkRight = false;
    [SerializeField] protected bool facingRight = false;
    [SerializeField] protected float xScale = 1;
    protected Rigidbody2D rigidbody;
    public UnitType unitType = UnitType.Enemy;
    [SerializeField] protected Skill currentSkill = Skill.Default;
    protected UnitStats statController;
    /// <summary>
    /// below 4 variables are for target detection
    /// </summary>
    [SerializeField] protected List<GameObject> NearbyAllies = new List<GameObject>();
    [SerializeField] protected List<GameObject> NearbyEnemies = new List<GameObject>();
    [SerializeField] protected GameObject ClosestAlly;
    [SerializeField] protected GameObject ClosestEnemy;

    [SerializeField] protected List<Skill> SkillPool = new List<Skill>(); //pool of skills this unit would use
    [SerializeField] protected List<float> SkillCooldowns = new List<float>(); //cooldown for skills

    public OnDestroyEvent onDestroy = new OnDestroyEvent();

    #region body parts
    [SerializeField] protected List<GameObject> BodyParts;
    [SerializeField] protected float transparency = 1;
    #endregion


    #region HealthBar
    protected GameObject HealthbarPrefab;
    [SerializeField] protected GameObject HealthbarInstance;
    protected float HealthbarOffsetX = 0;
    protected float HealthbarOffsetY = 4;
    #endregion

    #region abstract methods
    public abstract void Death();
    #endregion

    protected virtual void Start()
    {
        statController = GetComponent<UnitStats>();
        rigidbody = GetComponent<Rigidbody2D>();
        //instantiate health bar
        HealthbarPrefab = Resources.Load("Prefabs/Healthbar") as GameObject;
        HealthbarInstance = Instantiate(HealthbarPrefab, transform);
        HealthbarInstance.transform.localPosition = new Vector3(HealthbarOffsetX, HealthbarOffsetY, 0);


    }

    protected virtual void Update()
    {
        //reduce cooldown time for all skills
        for (int i = 0; i < SkillCooldowns.Count; i++)
        {
            if (SkillCooldowns[i] > 0)
                SkillCooldowns[i] -= Time.deltaTime;
        }

    }
    public virtual void HitTarget(GameObject target)
    {
      //  Debug.Log("hit: " + target.name);
        statController.DealDamage(target, currentSkill.Amount);
        statController.Heal(currentSkill.HealthOnHit);

        statController.BleedTarget(target, currentSkill.Bleed);
        statController.GainEnergy(currentSkill.EnergyOnHit);
    }

    /// <summary>
    /// gets distance between this unit and another
    /// </summary>
    /// <param name="unit">unit to check distance with</param>
    /// <returns>distance between units </returns>
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
    protected void stopAttacking()
    {
        isAttacking = false;
    }

    /// <summary>
    /// updates the health bar based on health percentage of owner
    /// </summary>
    /// <param name="percentage">percentage of health of owner. must be between 0 and 1</param>
    public void UpdateHealth(float percentage)
    {
        HealthbarInstance.GetComponent<HealthBar>().UpdateHealth(percentage);
    }

    /// <summary>
    /// flips character sprite
    /// </summary>
    protected void FlipCharacter(bool Right)
    {
        if (Right)
        {
            transform.localScale = new Vector3(-xScale, transform.localScale.y, 1);
            HealthbarInstance.transform.localScale = new Vector3(-1, 1, 1);

        }
        else
        {
            transform.localScale = new Vector3(xScale, transform.localScale.y, 1);
            HealthbarInstance.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    #region Death related
    protected IEnumerator DeathDecay()
    {
        while (transparency > 0)
        {

            transparency -= 0.006f;
            for(int i = 0; i<BodyParts.Count;i++)
            {
                BodyParts[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, transparency);
            }
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    #endregion

}

public class OnDestroyEvent : UnityEvent<GameObject> { };
