using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSkillController : MonoBehaviour
{

    public OnSkillChosen onChosen = new OnSkillChosen();
    int Index;
    List<Skill> skillList;

   [SerializeField] List<Transform> slotLocations;
    GameObject SlotPrefab;

    List<GameObject> currentSlots = new List<GameObject>();

    public void Init(Skill oldSKill, List<Skill> newSkills, int index)
    {


        foreach (GameObject g in currentSlots)
        {
            Destroy(g);
        }
        currentSlots = new List<GameObject>();
        SlotPrefab = Resources.Load("Prefabs/UI/Overlays/SlotOption") as GameObject;
        Index = index;
        skillList = newSkills;

        //init icons
        int slotIndex = 0;
        for (int i =0;i<skillList.Count;i++)
        {
            if(skillList[i]!=oldSKill)
            {

                var instance = Instantiate(SlotPrefab,transform);

                currentSlots.Add(instance);
                instance.transform.position = slotLocations[slotIndex].position;

                instance.GetComponent<SlotOption>().Init(newSkills[i], i);
                instance.GetComponent<SlotOption>().onChosen.AddListener(SwapSkill);
                slotIndex++;
            }

        }
    }

    void SwapSkill(Skill skill, int index)
    {
        onChosen.Invoke(skill, Index);
        gameObject.SetActive(false);
    }
}
