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
    [SerializeField] protected int slow = 0; //seconds of slow applied
    public UnityEvent OnHitEvent = new UnityEvent();//event for when projectile hits an enemy
    public OnHitHealEvent OnHitHeal = new OnHitHealEvent();

    // Start is called before the first frame update
    protected void Start()
    {
        canDetect = true;
    }

    // Update is called once per frame
   protected virtual void Update()
    {
        Timer -= Time.deltaTime;
        if(Timer<=0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// setup projectile. with setting rotation manually
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="direction"></param>
    /// <param name="rotation"></param>
    /// <param name="piercing"></param>
    /// <param name="bleed"></param>
    /// <param name="healOnHit"></param>
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

    /// <summary>
    /// setup projectile. with auto rotation facing direction its travelling 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="direction"></param>
    /// <param name="piercing"></param>
    /// <param name="bleed"></param>
    /// <param name="healOnHit"></param>
    public void SetUp(float damage, Vector2 direction,  bool piercing, int bleed, int healOnHit)
    {
        this.damage = damage;
        this.direction = direction;
        this.piercing = piercing;
        this.bleed = bleed;
        heal = healOnHit;
        GetComponent<Rigidbody2D>().velocity = direction;
        // transform.localEulerAngles = new Vector3(0, 0, rotation);
        transform.up = GetComponent<Rigidbody2D>().velocity;
    }

    /// <summary>
    /// allows projectile to slow target hit for x seconds
    /// </summary>
    /// <param name="seconds"></param>
    public void AddSlow(int seconds)
    {
        slow = seconds;
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("Terrain"))
        {
            OnHitEvent.RemoveAllListeners();
            OnHitHeal.RemoveAllListeners();
            Destroy(gameObject);
        }
        else
        {
            UnitController u = collision.transform.root.GetComponent<UnitController>();
            if (u.unitType != unitType && canDetect) 
            {
                if (!hitList.Contains(u.gameObject))
                {
                    onHit(u.gameObject);
                }
                OnHitHeal.RemoveAllListeners(); // heal can only trigger once per projectile
                if (!piercing)
                {
                    OnHitEvent.RemoveAllListeners();

                    Destroy(gameObject);
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


}

public class OnHitHealEvent : UnityEvent<int> { };