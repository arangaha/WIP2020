using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaEffectControl : WeaponColliderController
{
    [SerializeField] protected float damage = 0;
    [SerializeField] protected int heal = 0;
    [SerializeField] protected int bleed = 0; //bleed applied by this AoE on hit
    [SerializeField] protected int slow = 0; //seconds of slow applied
    public UnityEvent OnHitEvent = new UnityEvent();//event for when AoE hits an enemy
    public OnHitHealEvent OnHitHeal = new OnHitHealEvent();
    protected GameObject owner;
    bool exclusiveTarget = false;
    GameObject Target;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        canDetect = true;
    }


    public virtual void SetUp(float damage, int bleed, int healOnHit, GameObject own)
    {
        this.damage = damage;
        this.bleed = bleed;
        heal = healOnHit;
        owner = own;
    }

    public virtual void SetUp(float damage, float rotation, int bleed, int healOnHit, GameObject own)
    {
        this.damage = damage;
        this.bleed = bleed;
        heal = healOnHit;
        transform.localEulerAngles = new Vector3(0, 0, rotation);
        owner = own;
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.tag.Equals("Terrain"))
        {
            UnitController u = collision.transform.root.GetComponent<UnitController>();
            if (u.unitType != unitType && canDetect)
            {
                if (!hitList.Contains(u.gameObject)&&!exclusiveTarget)
                {
                    onHit(u.gameObject);

                }
                else if(!hitList.Contains(u.gameObject) && u.gameObject.Equals(Target))
                {
                    onHit(u.gameObject);
                }
            }
        }
    }

    protected virtual void onHit(GameObject g)
    {
        hitList.Add(g);
        g.GetComponent<UnitStats>().TakeDamage(damage);
        if (bleed > 0)
            g.GetComponent<UnitStats>().Bleed(bleed);
        if (slow > 0)
            g.GetComponent<UnitStats>().Slow(slow);
        WeaponCollisionEnterEvent.Invoke(g);
        OnHitEvent.Invoke();
        OnHitHeal.Invoke(heal);
    }

    /// <summary>
    /// allows projectile to slow target hit for x seconds
    /// </summary>
    /// <param name="seconds"></param>
    public void AddSlow(int seconds)
    {
        slow = seconds;
    }

    public void RemoveEffect()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// sets this effect to only be able to hit this target
    /// </summary>
    /// <param name="g"></param>
    public void SetExclusiveTarget(GameObject g)
    {
        exclusiveTarget = true;
        Target = g;
    }

}
