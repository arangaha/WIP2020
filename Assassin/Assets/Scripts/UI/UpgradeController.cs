using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// controller that provides player with upgrade choices and applies those upgrades once chosen
/// </summary>
public class UpgradeController : MonoBehaviour
{
    int UpgradesLeft = 0;
    [SerializeField] TextMeshProUGUI UpgradesLeftText;
    List<SkillUpgrade> UpgradePool; //pool of skill upgrades that can be upgraded. final upgrade options are chosen from this list
                                    // List<SkillUpgrade> UpgradeOptions;//upgrade options to be chosen by the player
    Dictionary<SkillUpgrade, int> UpgradePoolWithCount;
    List<TempPlayerSkill> UpgradableSkills; //skills that can be upgraded this choice
    GameObject UpgradeOptionPanel;
    [SerializeField] Transform SpawnPointLeft;
    [SerializeField] Transform SpawnPointRight;
    StageController stageCtrl;
    /// <summary>
    /// initializies the current upgrade choices. must be called everytime new upgrade choices are presented
    /// </summary>
    /// <param name="SkillPool">skill pool of player's current skills, including their current upgrades</param>
    /// <param name="SlottedSkills">skills that are slotted/equipped by player, skill upgrades can only be chosen from these skills (with the exception of normal attacks)</param>
    /// <param name="upgrades">amount of upgrades that can be gained</param>
    public void Init(Dictionary<Skill, TempPlayerSkill> SkillPool, List<Skill> SlottedSkills, int upgrades,StageController control)
    {
        GlobalVariables.Instance.HasUI = true;
        UpgradePool = new List<SkillUpgrade>();
        UpgradePoolWithCount = new Dictionary<SkillUpgrade, int>();
       // UpgradeOptions = new List<SkillUpgrade>();
        //pick skills among the player's skill pool that are equipped
        UpgradableSkills = new List<TempPlayerSkill>();
        UpgradableSkills.Add(SkillPool[Skill.PlayerNormalAttack]);
        UpgradableSkills.Add(SkillPool[Skill.PlayerNormalRangedAttack]);
        for (int i = 0; i < SlottedSkills.Count; i++)
        {
            if(SlottedSkills[i]!=Skill.Default)
            UpgradableSkills.Add(SkillPool[SlottedSkills[i]]);
        }
        //add skill upgrades from equipped skill pool that are available to be chosen (fully upgraded skills will not be added)
        UpgradeOptionPanel = Resources.Load("Prefabs/UI/Overlays/UpgradeOption") as GameObject;
        stageCtrl = control;
        //populate upgrade pool from available upgrades
        for(int i =0;i<UpgradableSkills.Count;i++)
        {
            foreach(KeyValuePair< SkillUpgrade,int> s in UpgradableSkills[i].Upgrades)
            {
                if(s.Value<s.Key.MaxUpgrades)
                {
                    UpgradePool.Add(s.Key);
                    UpgradePoolWithCount.Add(s.Key, s.Value);
                 //   Debug.Log(s.Key.Description + "," + s.Value);

                }
            }
        }

        UpgradesLeftText.text = "Upgrades Remaining: " + upgrades;
        InitOptions();
    }

    /// <summary>
    /// initializes upgrade options. use every time a upgrade can be chosen
    /// </summary>
    private void InitOptions()
    {
      

        //randomly pick 3 (4 with perk) skill upgrades to be chosen by the player
        for (int i =0;i<3;i++)
        {
            GameObject instance = Instantiate(UpgradeOptionPanel,transform);
            if (i == 0)
                instance.transform.position = SpawnPointLeft.position;
            else if (i == 2)
                instance.transform.position = SpawnPointRight.position;
            else
                instance.transform.localPosition = new Vector3(0, 0, 0);
            //randomly pick an upgrade. if an upgrade is full, it cannot be picked
            
            SkillUpgrade option = UpgradePool[Random.Range(0, UpgradePool.Count)];

            instance.GetComponent<UpgradeOption>().Init(option.Skill, option, UpgradePoolWithCount[option]);
            instance.GetComponent<UpgradeOption>().onChosen.AddListener(OnUpgrade);

            UpgradePool.Remove(option);
            UpgradePoolWithCount.Remove(option);
        }
        //instantiate the upgrade options into buttons and init them(add listener to each button for if they are clicked)
    }



    private void OnUpgrade(Skill s, SkillUpgrade u)
    {
        stageCtrl.ApplyUpgrade(s, u);
    }

    void OnDisable()
    {
        GlobalVariables.Instance.HasUI = false;
    }
}
