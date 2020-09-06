using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// class for controlling the health bar of a unit
/// </summary>
public class HealthBar : MonoBehaviour
{
   [SerializeField] SpriteRenderer health;
    [SerializeField] GameObject ArmorCover;
    [SerializeField] TextMeshPro ArmorAmount;
    [SerializeField] TextMeshPro HealthAmount;
    int maxhealth = 0;
    public void InitMaxHealth(int Health)
    {
        maxhealth = Health;
    }

    /// <summary>
    /// updates the health bar based on health percentage of owner
    /// </summary>
    /// <param name="percentage">percentage of health of owner. must be between 0 and 1</param>
    public void UpdateHealth(float percentage, float amount)
    {
        health.material.color = Color.Lerp(Color.green, Color.red, 1 - percentage);
        health.transform.localScale =  new Vector3(percentage, 1, 1);
        health.transform.localPosition = new Vector3(-10 + 10 * percentage, 0, 1);
        HealthAmount.text = (int)amount + " / " + maxhealth;
    }

    public void UpdateArmor(int amount)
    {
        ArmorAmount.text = ""+amount;
        if(amount>0)
        {
            ArmorCover.SetActive(true);
        }
        else
        {
            ArmorCover.SetActive(false);
        }
    }

}
 