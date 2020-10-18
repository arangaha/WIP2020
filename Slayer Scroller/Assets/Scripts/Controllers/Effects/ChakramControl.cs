using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChakramControl : ProjectileControl
{
    GameObject Owner;

    [SerializeField] int ArmorOnHit;
    [SerializeField] int TotalArmorGained = 0;
    [SerializeField] float ReturnDamage;
    [SerializeField] int ReturnExposes;
    [SerializeField] float StationaryDamage;
    [SerializeField] float StationaryInterval = 0.1f;
    [SerializeField] protected List<GameObject> ForwardHitList = new List<GameObject>();
    [SerializeField] protected List<GameObject> ReturnHitList = new List<GameObject>();
    [SerializeField] protected List<GameObject> StationaryHitList = new List<GameObject>();
    [SerializeField] bool returning = false;
    [SerializeField] bool stationary = false;
    [SerializeField] float ReturnTimer = 0.5f; // timer before projectile starts to return
    [SerializeField] float currentReturnTimer = 0;

    protected void Start()
    {
        ReturnTimer = 0.3f;
        canDetect = true;
    }

    protected override void Update()
    {
        //head towards owner
        if(returning)
        {
            var y = (Owner.transform.position.y - transform.position.y)/ReturnTimer;
            GetComponent<Rigidbody2D>().velocity = new Vector2(-direction.x, y);
            if(Mathf.Abs(Owner.transform.position.x - transform.position.x)<1)
            {
               // Debug.Log("returned");
                Owner.GetComponent<UnitStats>().GainArmor(TotalArmorGained);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator ReturnCountdown()
    {
        yield return new WaitForSeconds(ReturnTimer);
        if (StationaryDamage > 0)
        {

            StartCoroutine(StationaryPeriod());
        }
        else
            returning = true;
       // GetComponent<Rigidbody2D>().velocity = new Vector2(-direction.x, 0);
    }

    IEnumerator StationaryPeriod()
    {
        stationary = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        while (currentReturnTimer < ReturnTimer)
        {
            currentReturnTimer += StationaryInterval;
            //reset hit list
            StationaryHitList = new List<GameObject>();
            yield return new WaitForSeconds(StationaryInterval);
        }
        stationary = false;
        returning = true;
    }

    public void UpgradesSetup(GameObject owner, int armorOnHit,int returningDamage, int returningExpose, int stationaryDamage, float damageMulti)
    {
        ReturnTimer = 0.5f;

        ArmorOnHit = armorOnHit;
        ReturnDamage = returningDamage;
        ReturnExposes = returningExpose;
        StationaryDamage = stationaryDamage;
        Owner = owner;

        damage *= damageMulti;
        ReturnDamage *= damageMulti;
        StationaryDamage *= damageMulti;

        StartCoroutine(ReturnCountdown());
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("Terrain"))
        {
            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Wall")))
            {
                OnHitEvent.RemoveAllListeners();
                OnHitHeal.RemoveAllListeners();
                Destroy(gameObject);
            }
        }
        else
        {
            UnitController u = collision.transform.root.GetComponent<UnitController>();
            if (u.unitType != unitType && canDetect)
            {
                if (!ForwardHitList.Contains(u.gameObject)&&!returning)
                {
                    onHit(u.gameObject);
                }else if(!ReturnHitList.Contains(u.gameObject)&&returning)
                {
                    onHit(u.gameObject);
                }
                if(!StationaryHitList.Contains(u.gameObject)&&stationary)
                {
                    StationaryHitList.Add(u.gameObject);
                    u.GetComponent<UnitStats>().TakeDamage(StationaryDamage);
                }
                OnHitHeal.RemoveAllListeners(); // heal can only trigger once per projectile

            }
        }
    }

    protected override void onHit(GameObject g)
    {

        base.onHit(g);
        if (!returning)
        {
            TotalArmorGained += ArmorOnHit;
            ForwardHitList.Add(g);
        }
        else
        {
            TotalArmorGained += ArmorOnHit;
            ReturnHitList.Add(g);
            //extradamage if bit both times
            if (ReturnHitList.Contains(g) && ForwardHitList.Contains(g))
            {
                g.GetComponent<UnitStats>().TakeDamage(ReturnDamage);
                g.GetComponent<UnitStats>().Expose(ReturnExposes);
            }
        }
        //expose

    }
}
