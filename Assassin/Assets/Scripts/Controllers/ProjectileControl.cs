using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// controller for projectiles
/// </summary>
public class ProjectileControl : WeaponColliderController
{
    [SerializeField] protected float damage = 0;
    [SerializeField] protected int heal = 0;
    [SerializeField] protected Vector2 direction = new Vector2(0, 0);
    [SerializeField] protected bool piercing = false; //whether projectile pierces its target
    [SerializeField] protected float Timer = 10; //timer for projectile, projectile disappears when timer reaches 0. default timer is 10 seconds 
    [SerializeField] protected int bleed = 0; //bleed applied by this projectile on hit
    public UnityEvent OnHitEvent = new UnityEvent();//event for when projectile hits an enemy
    public OnHitHealEvent OnHitHeal = new OnHitHealEvent();

    // Start is called before the first frame update
    protected void Start()
    {
        canDetect = true;
    }

    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;
        if(Timer<=0)
        {
            Destroy(gameObject);
        }
    }

    public void SetUp(float damage, Vector2 direction, float rotation, bool piercing, int bleed, int healOnHit)
    {
        this.damage = damage;
        this.direction = direction;
        this.piercing = piercing;
        this.bleed = bleed;
        heal = healOnHit;
        GetComponent<Rigidbody2D>().velocity = direction;
        transform.localEulerAngles = new Vector3(0, 0, rotation);
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        UnitController u = collision.transform.root.GetComponent<UnitController>();
        if (u.unitType != unitType && canDetect)
        {
            if (!hitList.Contains(u.gameObject))
            {
                hitList.Add(u.gameObject);
                u.gameObject.GetComponent<UnitStats>().TakeDamage(damage);
                if(bleed>0)
                    u.gameObject.GetComponent<UnitStats>().Bleed(bleed);
                WeaponCollisionEnterEvent.Invoke(u.gameObject);
                OnHitEvent.Invoke();
                OnHitHeal.Invoke(heal);
            }
            if(!piercing)
            {
                OnHitEvent.RemoveAllListeners();
                OnHitHeal.RemoveAllListeners();
                Destroy(gameObject);
            }
        }
    }



    public class OnHitHealEvent : UnityEvent<int> { };

}
