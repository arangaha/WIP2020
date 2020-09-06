using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressSave : Singleton<ProgressSave>
{
    public bool initialized = false;
    public List<Skill> SlottedSkills;
    public Dictionary<Skill, TempPlayerSkill> SkillProgress;
    public int CurrentLevel;


    /// <summary>
    /// initializes save, used at the beginning of stage 1
    /// </summary>
    public void Init()
    {
        CurrentLevel = 1;
        SlottedSkills = new List<Skill>();
        SlottedSkills.Add(Skill.Default);
        SlottedSkills.Add(Skill.Default);
        SlottedSkills.Add(Skill.Default);
        SlottedSkills.Add(Skill.Default);

        SkillProgress = new Dictionary<Skill, TempPlayerSkill>();
        SkillProgress.Add(Skill.PlayerNormalAttack, new TempPlayerSkill(PlayerSkill.PlayerNormalAttack));
        SkillProgress.Add(Skill.PlayerNormalRangedAttack, new TempPlayerSkill(PlayerSkill.PlayerNormalRangedAttack));
    }

    public void SaveSkills(Dictionary<Skill, TempPlayerSkill> skills, List<Skill> slottedSkills)
    {
        SlottedSkills = slottedSkills;
        SkillProgress = skills;
    }

    public void LevelUp()
    {
        CurrentLevel++;
    }

    public void SetLevel(int level)
    { CurrentLevel = level; }
}
