using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
   [SerializeField] SpriteRenderer health;


    /// <summary>
    /// updates the health bar based on health percentage of owner
    /// </summary>
    /// <param name="percentage">percentage of health of owner. must be between 0 and 1</param>
    public void UpdateHealth(float percentage)
    {
        health.material.color = Color.Lerp(Color.green, Color.red, 1 - percentage);
        health.transform.localScale =  new Vector3(percentage, 1, 1);
        health.transform.localPosition = new Vector3(-10 + 10 * percentage, 0, 1);
    }
}
