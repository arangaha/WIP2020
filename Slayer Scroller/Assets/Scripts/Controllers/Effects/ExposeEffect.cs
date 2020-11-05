using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExposeEffect : MonoBehaviour
{
    [SerializeField] GameObject LeftEffect;
    [SerializeField] GameObject RightEffect;
    public void setXOffset(float x)
    {
        transform.localPosition = new Vector3(x, 0, 0);
    }

    public void setWidth(float multi)
    {
        LeftEffect.transform.localPosition= new Vector3(LeftEffect.transform.localPosition.x * multi, 0, 0);
        RightEffect.transform.localPosition= new Vector3(RightEffect.transform.localPosition.x * multi, 0, 0);
    }
}
