using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteWarriorStormCallControl : MonoBehaviour
{
    [SerializeField] List<Transform> SpawnLocations;
    [SerializeField] GameObject BoltPrefab;
    int gapLocation = 0;
    GameObject checkInstance;

    public void Setup()
    {
        gapLocation = Random.Range(2, SpawnLocations.Count - 3);
        for( int i = 0;i<SpawnLocations.Count;i++)
        {
            if(i!=gapLocation)
            {

                var instance = Instantiate(BoltPrefab);
                instance.transform.position = new Vector3(SpawnLocations[i].position.x, 6, 0);
                instance.GetComponent<AreaEffectControl>().setUnitType(UnitType.Enemy);
                Skill stormskill = Skill.EliteWarriorStormCall;
                instance.GetComponent<AreaEffectControl>().SetUp(stormskill.Amount, 0, 0, gameObject);
                checkInstance = instance;
            }
        }
        StartCoroutine(CheckDestroy());

    }

    IEnumerator CheckDestroy()
    {
        while (checkInstance)
            yield return new WaitForEndOfFrame();
    }

}
