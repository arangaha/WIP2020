using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// detection for AI 
/// </summary>
public class TargetDetection : MonoBehaviour
{
    [SerializeField] List<GameObject> Allies = new List<GameObject>();
    [SerializeField] List<GameObject> Enemies = new List<GameObject>();
    UnitType unitType = UnitType.Enemy;


    // Update is called once per frame
    void Update()
    {
        
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
        var u = transform.parent.GetComponent<UnitController>();
        u.updateNearbyAllies(Allies);
        u.updateNearbyEnemies(Enemies);
    }
}
