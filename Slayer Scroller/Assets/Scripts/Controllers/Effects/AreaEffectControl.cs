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
    // Start is called before the first frame update
    void Start()
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
                if (!hitList.Contains(u.gameObject))
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

    public void RemoveEffect()
    {
        Destroy(gameObject);
    }


}
