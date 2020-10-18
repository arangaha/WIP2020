using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// detection for AI 
/// </summary>
public class TargetDetection : MonoBehaviour
{
    [SerializeField] List<GameObject> Allies = new List<GameObject>();
    [SerializeField] List<GameObject> Enemies = new List<GameObject>();
    UnitType unitType = UnitType.Enemy;
    public onUpdateNearby onUpdateAllies = new onUpdateNearby();
    public onUpdateNearby onUpdateEnemies = new onUpdateNearby();

    public void ChangeUnitType(UnitType u)
    {
        unitType = u;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        UnitController u = collision.transform.root.GetComponent<UnitController>();
        if (unitType == UnitType.Enemy && u.unitType == UnitType.Ally || unitType == UnitType.Ally && u.unitType == UnitType.Enemy)
        { 
            if (!Enemies.Contains(u.gameObject))
            {
                Enemies.Add(u.gameObject);
                
            }
        }
        else if(unitType == UnitType.Enemy && u.unitType == UnitType.Enemy || unitType == UnitType.Ally && u.unitType == UnitType.Ally)
        {
            if (!Allies.Contains(u.gameObject))
            {
                Allies.Add(u.gameObject);
            }
        }
        updateNearby();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (Enemies.Contains(collision.gameObject))
        {
            Enemies.Remove(collision.gameObject);
        }
        else if (Allies.Contains(collision.gameObject))
        {
            Allies.Remove(collision.gameObject);
        }
        updateNearby();
    }

    void updateNearby()
    {


        onUpdateAllies.Invoke(Allies);
        onUpdateEnemies.Invoke(Enemies);
    }

    public class onUpdateNearby : UnityEvent<List<GameObject>> { };
}
