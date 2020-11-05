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
    [SerializeField]List< Skill > SkillsUnlocked;
    [SerializeField] GameObject SkillUnlockLeftButton;
    [SerializeField] GameObject SkillUnlockRightButton;
    [SerializeField] int CurrentSkillIndex = 0;
    [SerializeField] TextMeshProUGUI SkillUnlockedText;

    [SerializeField] TextMeshProUGUI NewMaxHealth;
    [SerializeField] TextMeshProUGUI NewStamRegen;

    [SerializeField] TextMeshProUGUI UpgradesAvailable;

    [SerializeField] GameObject SkillSwapButton;
    [SerializeField] SkillSwapController SkillSwapOverlay;
    MainCharacterController maincont;


    

    public void Init(int StageNumber, List<Skill> unlockedSkills, List<Skill> AllSkills, MainCharacterController MainControl,int upgrades)
    {
        maincont = MainControl;
        GlobalVariables.Instance.HasUI = true;
        if (StageNumber <= XMLLoad.Instance.stageList.stage.Count)
        StageCompleteText.text = "Stage " + StageNumber + " Completed";
        else 
        StageCompleteText.text = "Endless Stage " + (StageNumber - XMLLoad.Instance.stageList.stage.Count) + " Complete";

        SkillsUnlocked = new List<Skill>();
        SkillsUnlocked = unlockedSkills;

        if (unlockedSkills.Count<1)
            SkillUnlocked.SetActive(false);
        else
        {
            SkillUnlockLeftButton.SetActive(false);
            SkillUnlockRightButton.SetActive(false);
            SkillUnlocked.SetActive(true);
            DisplaySkill(unlockedSkills[0]);
            if(unlockedSkills.Count>1)
            {
                SkillUnlockedText.text =  SkillsUnlocked.Count + " New Skills Unlocked!";
                SkillUnlockRightButton.SetActive(true);
            }
            else
            {
                SkillUnlockedText.text = "New Skill Unlocked!";
            }
        }

        NewMaxHealth.text = "Max Health: " + (100 + 10 * (StageNumber ));
        NewStamRegen.text = (10 + 1 * (StageNumber)) + " Stamina/Second";

        UpgradesAvailable.text = "Upgrades Avaialble: " + upgrades;

        if (StageNumber == 1)
            SkillSwapButton.gameObject.SetActive(false);
        else
            SkillSwapButton.gameObject.SetActive(true);
        SkillSwapOverlay.Init(maincont, maincont.GetSlottedSkills(), AllSkills);

         CurrentSkillIndex = 0;

}

    public void LeftButton()
    {
        SkillUnlockRightButton.SetActive(true);
        CurrentSkillIndex--;
        if (CurrentSkillIndex == 0)
            SkillUnlockLeftButton.SetActive(false);
        DisplaySkill(SkillsUnlocked[CurrentSkillIndex]);
    }

    public void RightButton()
    {
        SkillUnlockLeftButton.SetActive(true);
        CurrentSkillIndex++;
        if (CurrentSkillIndex >= SkillsUnlocked.Count-1)
            SkillUnlockRightButton.SetActive(false);
        DisplaySkill(SkillsUnlocked[CurrentSkillIndex]);
    }

    void DisplaySkill(Skill s)
    {
        icon.Initialize(s.Icon.IconName);
        SkillUnlockedName.text = s.Name;
        SkillUnlockedDescription.text = s.Icon.Description;
    }

    public void CloseScreen()
    {
        GlobalVariables.Instance.HasUI = false;
        gameObject.SetActive(false);
    }
}
