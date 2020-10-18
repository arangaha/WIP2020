using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeStormControl : MonoBehaviour
{
   [SerializeField] BladeStormBladeControl blade;
    float Duration=6;
    public OnHitHealEvent onHitHeal = new OnHitHealEvent();

    public void Init(TempPlayerSkill t, GameObject owner)
    {

        StartCoroutine(Countdown());
        blade.SetUp(t.Amount, 0, 0, owner);
        blade.setUnitType(UnitType.Ally);
        blade.OnHitHeal.AddListener(OnHitHeal);
        GetComponent<Animator>().Play("BladeStorm", 0, Random.Range(0.1f, 0.9f)); // play blade at random frame
    }
    
     void OnHitHeal(int i)
    {
        onHitHeal.Invoke(i);
    }

    void ResetHit()
    {
        blade.ResetHitList();
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(Duration);
        Destroy(gameObject);
    }
}
