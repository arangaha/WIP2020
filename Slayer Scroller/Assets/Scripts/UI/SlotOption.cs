using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.Events;
public class SlotOption : MonoBehaviour
{

    public OnSkillChosen onChosen = new OnSkillChosen();
    Skill Skill;
    [SerializeField] TextMeshProUGUI SkillName;
    [SerializeField] TextMeshProUGUI SkillDesc;
    [SerializeField] IconController icon;
    int slot;
    public void Init(Skill skill, int index)
    {
        slot = index;
        Skill = skill;
        if (Skill != Skill.Default)
        {
            icon.Initialize(skill.Icon.IconName);
            Skill = skill;
            SkillName.text = skill.Name;
            SkillDesc.text = skill.Icon.Description;
        }
        else
        {

            SkillName.text = "";
            SkillDesc.text = "Empty Skill Slot";
        }
    }
    public void ChooseThisOption()
    {
            onChosen.Invoke(Skill,slot);
    }
}

public class OnSkillChosen : UnityEvent<Skill, int> { };