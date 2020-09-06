using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    GameObject damageTextPrefab;
   [SerializeField] IconController icon1;
    [SerializeField] IconController icon2;
    [SerializeField] IconController icon3;
    [SerializeField] IconController icon4;


    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1600, 900,true);
        damageTextPrefab = Resources.Load("Prefabs/UI/DamageText") as GameObject;
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDamageText(float amount, Vector3 position)
    {
        if (amount != 0)
        {
            GameObject instance = Instantiate(damageTextPrefab,transform);
            instance.transform.position = position;
            instance.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            instance.GetComponent<DamageText>().InitializeText(amount);
        }
    }

    /// <summary>
    /// initializes icon with a skill
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="iconName"></param>
    /// <param name="Cost"></param>
    public void InitIcon(int slot, string iconName, float Cost, int cooldown)
    {
        {
            switch (slot)
            {
                case 1:
                    icon1.Initialize(iconName, Cost, cooldown);
                    break;
                case 2:
                    icon2.Initialize(iconName, Cost, cooldown);
                    break;
                case 3:
                    icon3.Initialize(iconName, Cost, cooldown);
                    break;
                case 4:
                    icon4.Initialize(iconName, Cost, cooldown);
                    break;


            }
        }
    }



    /// <summary>
    /// updates cooldown of an icon
    /// </summary>
    /// <param name="slot">icon slot to be updated</param>
    /// <param name="percentage">percentage of cooldown</param>
    public void UpdateCooldown(int slot, float percentage)
    {
        switch (slot)
        {
            case 1:
                icon1.setCooldown(percentage);
                break;
            case 2:
                icon2.setCooldown(percentage);
                break;
            case 3:
                icon3.setCooldown(percentage);
                break;
            case 4:
                icon4.setCooldown(percentage);
                break;


        }
    }

    /// <summary>
    /// sets skill to be usable visually
    /// </summary>
    /// <param name="slot"></param>
    public void canUseSkill(int slot)
    {
        switch( slot)
        {
            case 1:
                icon1.canUse();
                break;
            case 2:
                icon2.canUse();
                break;
            case 3:
                icon3.canUse();
                break;
            case 4:
                icon4.canUse();
                break;


        }
    }

    /// <summary>
    /// sets skill to be unusable visually
    /// </summary>
    /// <param name="slot"></param>
    public void cannotUseSkill(int slot)
    {
        switch (slot)
        {
            case 1:
                icon1.cannotUse();
                break;
            case 2:
                icon2.cannotUse();
                break;
            case 3:
                icon3.cannotUse();
                break;
            case 4:
                icon4.cannotUse();
                break;


        }
    }


}
