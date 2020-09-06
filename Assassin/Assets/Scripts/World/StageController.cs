using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] Transform PlayerSpawnLocation;
    [SerializeField] List<Transform> spawnLocations;

    [SerializeField] GameObject CameraUI;

    GameObject PlayerPrefab;
    GameObject PlayerInstance;
    GameObject Warrior;
    GameObject HellHound;
    GameObject Hellbat;

    GameObject StageCompletePrefab;
    GameObject StageCompleteInstance;

    GameObject UpgradeScreenPrefab;
    GameObject UpgradeScreenInstance;

    bool initialized = false;

    [SerializeField] bool upgradesComplete = false;//bool for whether player have selected all their upgrades
    [SerializeField] int upgradesLeft = 0;
    [SerializeField] public int stageNumber;
     Stage currentStage;
    [SerializeField] int currentEnemies = 0;
    // Start is called before the first frame update
    void Start()
    {
        Warrior = Resources.Load("Prefabs/Characters/Warrior") as GameObject;
        HellHound = Resources.Load("Prefabs/Characters/HellHound") as GameObject;
        Hellbat = Resources.Load("Prefabs/Characters/Hellbat") as GameObject;

        PlayerPrefab = Resources.Load("Prefabs/Characters/MainCharacter") as GameObject;
        UpgradeScreenPrefab = Resources.Load("Prefabs/UI/Overlays/UpgradeOverlay") as GameObject;
        StageCompletePrefab = Resources.Load("Prefabs/UI/Overlays/StageCompleteOverlay") as GameObject;
        //load xml instance 
        if (!XMLLoad.Instance.initialized)
        {
            XMLLoad.Instance.LoadStages();
            XMLLoad.Instance.initialized = true;
        }
        if (!GlobalVariables.Instance.initialized)
        {
            GlobalVariables.Instance.initialized = true;
        }
        if (!ProgressSave.Instance.initialized)
        {
            ProgressSave.Instance.Init();
            ProgressSave.Instance.initialized = true;
          // Debug.Log("init");
        }
        Init(ProgressSave.Instance.CurrentLevel);

    }

    public void Init(int StageNumber)
    {
        stageNumber = StageNumber;
        //load stage based on stage number
        currentStage = XMLLoad.Instance.stageList.stage[stageNumber - 1];
        //init player
        SpawnPlayer();



        //spawn enemies
        for (int i = 0; i < currentStage.Warriors; i++)
        {
            SpawnWarrior();
        }
        for (int i = 0; i < currentStage.Hounds; i++)
        {
            SpawnHellHound();
        }
        for (int i = 0; i < currentStage.Bats; i++)
        {
            SpawnHellbat();
        }
        initialized = true;
    }

    public void SpawnPlayer()
    {
        if (PlayerInstance)
            Destroy(PlayerInstance);
        PlayerInstance = Instantiate(PlayerPrefab);
        PlayerInstance.transform.position = PlayerSpawnLocation.position;
        PlayerInstance.GetComponent<MainCharacterController>().Init(ProgressSave.Instance.SkillProgress, ProgressSave.Instance.SlottedSkills);
        PlayerInstance.GetComponent<MainCharacterStats>().Init(ProgressSave.Instance.CurrentLevel);
    }
    
    /// <summary>
    /// spawns an enemy in a random spawn location
    /// </summary>
    /// <param name="prefab"></param>
    void SpawnEnemy(GameObject prefab)
    {
        int num = Random.Range(0, spawnLocations.Count);
        var instance = Instantiate(prefab);
        instance.transform.position = spawnLocations[num].position;
        instance.GetComponent<UnitStats>().OnDeath.AddListener(OneLessEnemy);
        instance.GetComponent<UnitStats>().Init(currentStage.StageNumber);
        currentEnemies++;
    }

    /// <summary>
    /// spawns a warrior in a random spawn point
    /// </summary>
    public void SpawnWarrior()
    {
        SpawnEnemy(Warrior);
    }

    /// <summary>
    /// spawns a HellHound in a random spawn point
    /// </summary>
    public void SpawnHellHound()
    {
        SpawnEnemy(HellHound);
    }

    /// <summary>
    /// spawns a HellHound in a random spawn point
    /// </summary>
    public void SpawnHellbat()
    {
        int num = Random.Range(0, spawnLocations.Count);
        var instance = Instantiate(Hellbat);
        instance.transform.position =  new Vector3(spawnLocations[num].position.x, spawnLocations[num].position.y+4,0);
        instance.GetComponent<UnitStats>().OnDeath.AddListener(OneLessEnemy);
        instance.GetComponent<UnitStats>().Init(currentStage.StageNumber);
        currentEnemies++;
    }



    /// <summary>
    /// coroutine that gets ran when a stage is complete
    /// </summary>
    /// <returns></returns>
    IEnumerator StageClearSequence()
    {
        yield return new WaitForSeconds(1);

        MainCharacterController cont = PlayerInstance.GetComponent<MainCharacterController>();
        Skill unlockedSkill = UnlockSkill(currentStage.UnlockedSkill);

        //level up  stats on player and save stats 
        ProgressSave.Instance.LevelUp();

        //unlock skill for player
        if(unlockedSkill!=Skill.Default)
            cont.LearnSkill(unlockedSkill);

        // save upgraded version of skills and equipped slots
        cont.SaveSkills();

        //display stage complete screen
        if (!StageCompleteInstance)
            StageCompleteInstance = Instantiate(StageCompletePrefab, CameraUI.transform);
        else
            StageCompleteInstance.SetActive(true);
        StageCompleteInstance.GetComponent<StageComplete>().Init(stageNumber, unlockedSkill);

        //wait for stage complete screen to be closed to move on
        while (StageCompleteInstance.activeSelf)
        {
            yield return null;
        }
        // upgrades screen
        upgradesLeft = currentStage.UpgradeRewards;
        ShowUpgradeScreen();
        upgradesComplete = false;
        while(!upgradesComplete)
        {
            yield return null;
        }

        initialized = false;
        Init(ProgressSave.Instance.CurrentLevel); //place holder for next stage, in the future need to implement load scene

    }



    /// <summary>
    /// shows the upgrade screen overlay 
    /// </summary>
    public void ShowUpgradeScreen()
    {

        MainCharacterController cont = PlayerInstance.GetComponent<MainCharacterController>();
        if (!UpgradeScreenInstance)
            UpgradeScreenInstance = Instantiate(UpgradeScreenPrefab, CameraUI.transform);
        else
            UpgradeScreenInstance.SetActive(true);
        UpgradeScreenInstance.GetComponent<UpgradeController>().Init(cont.GetSkillsWithUpgrades(), cont.GetSlottedSkills(), upgradesLeft, this);
     
    }

    /// <summary>
    /// apply an upgrade to the player once chosen
    /// </summary>
    /// <param name="s">skill of the upgrade</param>
    /// <param name="u">upgrade</param>
    public void ApplyUpgrade(Skill s, SkillUpgrade u)
    {
        PlayerInstance.GetComponent<MainCharacterController>().UpgradeSkill(s, u);
        upgradesLeft--;
        if (upgradesLeft >0)
        {
            ShowUpgradeScreen();
        }
        else
        {
            UpgradeScreenInstance.SetActive(false);
            upgradesComplete = true;
        }
        
    }

    /// <summary>
    /// unlocks skill that is unlocked by completing this stage. leave at empty if there is none
    /// </summary>
    /// <param name="SkillName"></param>
    Skill UnlockSkill(string SkillName)
    {
        Skill unlockedSkill = Skill.Default;
        switch (SkillName)
        {
            case "RazorBlades":
                unlockedSkill = Skill.PlayerRazorBlades;
                break;
            case "Puncture":
                unlockedSkill = Skill.PlayerPuncture;
                break;
            case "Slash":
                unlockedSkill = Skill.PlayerSlash;
                break;
            case "Cull":
                unlockedSkill = Skill.PlayerCull;
                break;

        }
        return unlockedSkill;
    }

    /// <summary>
    /// deducts a count from an enemy, if the count is 0, show upgrade screen
    /// </summary>
    void OneLessEnemy()
    {
        currentEnemies--;
        if (currentEnemies == 0 && initialized)
        {
            StartCoroutine(StageClearSequence());
        }
    }
}
