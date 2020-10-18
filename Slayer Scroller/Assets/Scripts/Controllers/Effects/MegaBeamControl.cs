using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBeamControl : AreaEffectControl
{
    void Start()
    {
        canDetect = true;
        StartCoroutine(ResetHt());
        GetComponent<Animator>().Play("MegaBeam");
    }

    IEnumerator ResetHt ()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            hitList = new List<GameObject>();
        }
    }
}
