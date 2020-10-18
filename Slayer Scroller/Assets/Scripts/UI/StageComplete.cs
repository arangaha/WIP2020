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

    [SerializeField] GameObject SkillSwapButton;
    [SerializeField] SkillSwapController SkillSwapOverlay;
    MainCharacterController maincont;
    

    public void Init(int StageNumber, Skill unlockedSkill, List<Skill> AllSkills, MainCharacterController MainControl,int upgrades)
    {
        maincont = MainControl;
        GlobalVariables.Instance.HasUI = true;
        if (StageNumber <= XMLLoad.Instance.stageList.stage.Count)
        StageCompleteText.text = "Stage " + StageNumber + " Completed";
        else 
        StageCompleteText.text = "Endless Stage " + (StageNumber - XMLLoad.Instance.stageList.stage.Count) + " Complete";

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

        UpgradesAvailable.text = "Upgrades Avaialble: " + upgrades;

        if (StageNumber == 1)
            SkillSwapButton.gameObject.SetActive(false);
        else
            SkillSwapButton.gameObject.SetActive(true);
        SkillSwapOverlay.Init(maincont, maincont.GetSlottedSkills(), AllSkills);

    }



    public void CloseScreen()
    {
        GlobalVariables.Instance.HasUI = false;
        gameObject.SetActive(false);
    }
}
