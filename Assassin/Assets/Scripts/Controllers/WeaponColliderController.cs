using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// collider for weapons of units
/// </summary>
public class WeaponColliderController : MonoBehaviour
{
    public CollisionEnterEvent WeaponCollisionEnterEvent = new CollisionEnterEvent();
    [SerializeField] protected UnitType unitType = UnitType.Enemy;
    [SerializeField] protected List<GameObject> hitList = new List<GameObject>();
   [SerializeField] protected bool canDetect = false;
    
    /// <summary>
    /// on colliding with another unit, deal damage if it is the opposing side
    /// </summary>
    /// <param name="collision"></param>
  protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        UnitController u = collision.transform.root.GetComponent<UnitController>();
        if (u.unitType != unitType && canDetect)
        {
            if (!hitList.Contains(u.gameObject))
            {
                hitList.Add(u.gameObject);
                WeaponCollisionEnterEvent.Invoke(u.gameObject);
            }
        }
    }
    public void setUnitType(UnitType u)
    {
        unitType = u;
    }
    public void ResetHitList()
    {
        hitList = new List<GameObject>();
    }

    public void DetectionOn()
    {
        canDetect = true;
        ResetHitList();
    }

    public void DetectionOff()
    {
        canDetect = false;
    }

}
public class CollisionEnterEvent : UnityEvent<GameObject> { };
