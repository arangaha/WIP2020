using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
public class UpgradeOption : MonoBehaviour
{
    public OnUpgradeChosen onChosen = new OnUpgradeChosen();
    Skill Skill;
    SkillUpgrade Upgrade;
    [SerializeField] TextMeshProUGUI SkillName;
    [SerializeField] TextMeshProUGUI UpgradeDesc;
    [SerializeField] TextMeshProUGUI CurrentUpgrades;
    [SerializeField] IconController icon;
    // Start is called before the first frame update

    public void Init(Skill skill, SkillUpgrade upgrade, int currentUpgradeAmount)
    {
        icon.Initialize(skill.Icon.IconName);
        Skill = skill;
        Upgrade = upgrade;
        SkillName.text = skill.Name;
        UpgradeDesc.text = upgrade.Description;
        CurrentUpgrades.text = "Current Upgrades: " +currentUpgradeAmount + "/" + upgrade.MaxUpgrades;
    }

    public void ChooseThisOption()
    {
        onChosen.Invoke(Skill,Upgrade);
       // Debug.Log("choose");
        onChosen.RemoveAllListeners();
    }


}

public class OnUpgradeChosen : UnityEvent<Skill, SkillUpgrade> { };