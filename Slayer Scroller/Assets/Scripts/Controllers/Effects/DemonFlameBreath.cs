using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFlameBreath : MonoBehaviour
{
    [SerializeField] DemonFlameBreathFire breath;
    [SerializeField] DemonFlameBreathFire flame1;
    [SerializeField] DemonFlameBreathFire flame2;

    public void SetUp(int DPS, int burn, GameObject owner)
    {
        breath.setUnitType(UnitType.Enemy);
        flame1.setUnitType(UnitType.Enemy);
        flame2.setUnitType(UnitType.Enemy);
        breath.SetUp(DPS, 0, 0, owner);
        breath.SetBurn(burn);
        flame1.SetUp(DPS, 0, 0, owner);
        flame1.SetBurn(burn);
        flame2.SetUp(DPS, 0, 0, owner);
        flame2.SetBurn(burn);
    }

    public void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
