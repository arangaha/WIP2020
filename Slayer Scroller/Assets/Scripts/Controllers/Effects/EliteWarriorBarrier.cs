using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EliteWarriorBarrier : AreaEffectControl 
{
    [SerializeField] int Timer;
    [SerializeField] TextMeshPro TimerText;

    public void UpdateTimer(int i)
    {
        Timer = i;

        TimerText.text = i+"";
    }

    public void Fail()
    {
        TimerText.gameObject.SetActive(false);
        GetComponent<Animator>().Play("Fail");
    }

    public void Explode()
    {
        TimerText.gameObject.SetActive(false);
        GetComponent<Animator>().Play("Explode");
    }
}
