using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSwapController : MonoBehaviour
{
    MainCharacterController maincont;
    List<Skill> slottedSkills;
    List<Skill> allSkills;
   [SerializeField] List<Transform> slotLocations;
    GameObject SlotPrefab;
    [SerializeField] SelectSkillController skillSelectCont;

    List<GameObject> currentSlots = new List<GameObject>();
    void Start()
    {
        skillSelectCont.onChosen.AddListener(SwapSkill);
    }

    public void Init(MainCharacterController Maincont, List<Skill> SlottedSkills, List<Skill> AllSkills)
    {
        foreach (GameObject g in currentSlots)
        {
            Destroy(g);
        }
        currentSlots = new List<GameObject>();
        SlotPrefab = Resources.Load("Prefabs/UI/Overlays/SlotOption") as GameObject;
        maincont = Maincont;
        slottedSkills = SlottedSkills;
        allSkills = AllSkills;
        for (int i = 0;i<slotLocations.Count;i++)
        {
            var instance = Instantiate(SlotPrefab,transform);
            currentSlots.Add(instance);
            instance.transform.position = slotLocations[i].position;
            instance.GetComponent<SlotOption>().Init(slottedSkills[i],i);
            instance.GetComponent<SlotOption>().onChosen.AddListener(SelectSlot);
        }


    }

    /// <summary>
    /// select slot to swap
    /// </summary>
    /// <param name="i"></param>
    public void SelectSlot(Skill s, int index)
    {
        //open swap screen
        skillSelectCont.Init(s, allSkills, index);
        skillSelectCont.transform.SetAsLastSibling();
        skillSelectCont.gameObject.SetActive(true);
    }

    void SwapSkill(Skill newSkill, int index)
    {
        //Debug.Log("swap");
        maincont.SlotSkill(newSkill, index,true);
        Init(maincont, maincont.GetSlottedSkills(), allSkills);
    }
}
