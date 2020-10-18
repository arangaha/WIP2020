using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class IconController : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] float cost;
    [SerializeField] TextMeshProUGUI energyCost;
    [SerializeField] GameObject CooldownBlock;
    [SerializeField] GameObject CostBlock;

    [SerializeField] TextMeshProUGUI cooldownTimer;

    int cooldown = 0;

    public void Initialize(string iconName, float Cost, int cooldown)
    {
        if (iconName != "")
        {
            icon.sprite = Resources.Load<Sprite>("Sprites/Icons/" + iconName);
            energyCost.text = "" + (int)Cost;
            cost = Cost;
            this.cooldown = cooldown;
        }
        else
        {
            icon.sprite = Resources.Load<Sprite>("Sprites/Icons/Default");
            energyCost.text = "";
        }
    }

    public void Initialize(string iconName)
    {
        icon.sprite = Resources.Load<Sprite>("Sprites/Icons/" + iconName);
    }
    public void canUse()
    {
        CostBlock.SetActive(false);
    }

    public void cannotUse()
    {
        CostBlock.SetActive(true);
    }

    public void setCooldown(float RemainingCooldown)
    {
        CooldownBlock.transform.localPosition = new Vector3(0, -70 * (1-RemainingCooldown));
        var cd = RemainingCooldown * cooldown;
        if (cd >= 1)
            cooldownTimer.text = "" + (int)(cd);
        else
            cooldownTimer.text = "" +  cd.ToString("0.#");
        if (RemainingCooldown > 0.1f)
            cooldownTimer.gameObject.SetActive(true);
        else
            cooldownTimer.gameObject.SetActive(false);
    }

}
