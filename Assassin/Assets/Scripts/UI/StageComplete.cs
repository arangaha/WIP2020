using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageComplete : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI StageCompleteText;

    [SerializeField] GameObject SkillUnlocked;
    [SerializeField] IconController icon;
    [SerializeField] TextMeshProUGUI SkillUnlockedName;
    [SerializeField] TextMeshProUGUI SkillUnlockedDescription;

    [SerializeField] TextMeshProUGUI NewMaxHealth;
    [SerializeField] TextMeshProUGUI NewStamRegen;

    [SerializeField] TextMeshProUGUI UpgradesAvailable;

    Stage completedStage;

    public void Init(int StageNumber, Skill unlockedSkill)
    {
        GlobalVariables.Instance.HasUI = true;
        completedStage = XMLLoad.Instance.stageList.stage[StageNumber - 1];

        StageCompleteText.text = "Stage " + StageNumber + " Completed";

        if (unlockedSkill == Skill.Default)
            SkillUnlocked.SetActive(false);
        else
        {
            SkillUnlocked.SetActive(true);
            icon.Initialize(unlockedSkill.Icon.IconName);
            SkillUnlockedName.text = unlockedSkill.Name;
            SkillUnlockedDescription.text = unlockedSkill.Icon.Description;
        }

        NewMaxHealth.text = "Max Health: " + (100 + 10 * (StageNumber ));
        NewStamRegen.text = (10 + 1 * (StageNumber)) + " Stamina/Second";

        UpgradesAvailable.text = "Upgrades Avaialble: " + completedStage.UpgradeRewards;

    }

    public void CloseScreen()
    {
        GlobalVariables.Instance.HasUI = false;
        gameObject.SetActive(false);
    }
}
