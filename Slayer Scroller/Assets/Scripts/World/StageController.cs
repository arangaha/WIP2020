using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class StageController : MonoBehaviour
{
    [SerializeField] Transform PlayerSpawnLocation;
    [SerializeField] List<Transform> spawnLocations;

    [SerializeField] GameObject CameraUI;
    [SerializeField] TextMeshProUGUI remainingEnemies;
    [SerializeField] TextMeshProUGUI stagetext;
    [SerializeField] TextMeshProUGUI stageCornertext;

    GameObject PlayerPrefab;
    GameObject PlayerInstance;
    GameObject Warrior;
    GameObject HellHound;
    GameObject HellHoundElite;
    GameObject Hellbat;
    GameObject EliteWarrior;
    
    GameObject StageCompletePrefab;
    GameObject StageCompleteInstance;

    GameObject UpgradeScreenPrefab;
    GameObject UpgradeScreenInstance;

    bool initialized = false;

    [SerializeField] bool upgradesComplete = false;//bool for whether player have selected all their upgrades
    [SerializeField] int upgradesLeft = 0;
    [SerializeField] public int stageNumber;

    [SerializeField] GameObject DefeatScreen;
    [SerializeField] TextMeshProUGUI retries;
    [SerializeField] GameObject retrybutton;

     Stage currentStage;
    [SerializeField] int currentEnemies = 0;
    [SerializeField] List<GameObject> EnemiesList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        Warrior = Resources.Load("Prefabs/Characters/Warrior") as GameObject;
        HellHound = Resources.Load("Prefabs/Characters/HellHound") as GameObject;
        HellHoundElite = Resources.Load("Prefabs/Characters/HellHoundVariant") as GameObject;
        Hellbat = Resources.Load("Prefabs/Characters/Hellbat") as GameObject;
        EliteWarrior = Resources.Load("Prefabs/Characters/EliteWarrior") as GameObject;

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
        EnemiesList = new List<GameObject>();
        //init player
        SpawnPlayer();

        //preset stages
        if (StageNumber <= XMLLoad.Instance.stageList.stage.Count)
        {
            //load stage based on stage number
            currentStage = XMLLoad.Instance.stageList.stage[stageNumber - 1];


  
            stagetext.text = "Stage " + StageNumber;
            stageCornertext.text = "Stage " + StageNumber;
        }
        //endlessly generated stages
        else
        { currentStage = new Stage();
            currentStage.StageNumber = StageNumber;
            if (StageNumber % 10 == 0)
            {
                currentStage.EliteWarriors = 1;
            }
            else
            {
                int tempSpawningNum = stageNumber % 4;
                switch (tempSpawningNum)
                {
                    case 0:
                        currentStage.Warriors = stageNumber / 2;
                        break;
                    case 1:
                        currentStage.Bats = stageNumber / 2 + 4;
                        break;
                    case 2:
                        currentStage.Hounds = stageNumber / 3;
                        break;
                    case 3:
                        currentStage.EliteHounds = stageNumber / 10 + 1;
                        break;
                }
            }

            currentStage.UpgradeRewards = stageNumber / 10;

            stagetext.text = "Endless Stage " + (StageNumber - XMLLoad.Instance.stageList.stage.Count);
            stageCornertext.text = "Endless Stage " + (StageNumber - XMLLoad.Instance.stageList.stage.Count);
        }
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
        for (int i = 0; i < currentStage.EliteHounds; i++)
        {
            SpawnHellHoundElite();
        }
        for (int i = 0; i < currentStage.EliteWarriors; i++)
        {
            SpawnEliteWarrior();
        }

        UpdateEnemies();
        stagetext.GetComponent<Animator>().Play("Anim");
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
        PlayerInstance.GetComponent<MainCharacterController>().onDeath.AddListener(Defeated);
    }
    
    /// <summary>
    /// spawns an enemy in a random spawn location
    /// </summary>
    /// <param name="prefab"></param>
    void SpawnEnemy(GameObject prefab)
    {
        int num = Random.Range(0, spawnLocations.Count);
        var instance = Instantiate(prefab);
        float range = Random.Range(-1, 1);
        instance.transform.position = new Vector3(spawnLocations[num].position.x + range, spawnLocations[num].position.y,1);
        instance.GetComponent<UnitStats>().OnDeath.AddListener(OneLessEnemy);
        instance.GetComponent<UnitStats>().Init(currentStage.StageNumber);
        currentEnemies++;
        EnemiesList.Add(instance);

        UpdateEnemies();
    }

    void SpawnEnemy(GameObject prefab, Vector3 position)
    {
        var instance = Instantiate(prefab);
        instance.transform.position = position;
        instance.GetComponent<UnitStats>().OnDeath.AddListener(OneLessEnemy);
        instance.GetComponent<UnitStats>().Init(currentStage.StageNumber);
        currentEnemies++;
        EnemiesList.Add(instance);

        UpdateEnemies();
    }
    /// <summary>
    /// spawns a warrior in a random spawn point
    /// </summary>
    public void SpawnWarrior()
    {
        SpawnEnemy(Warrior);
    }

    public void SpawnWarrior(Vector3 pos)
    {
        SpawnEnemy(Warrior,pos);
    }
    /// <summary>
    /// spawns a HellHound in a random spawn point
    /// </summary>
    public void SpawnHellHound()
    {
        SpawnEnemy(HellHound);
    }


    /// <summary>
    /// spawns a HellHound Elite in a random spawn point
    /// </summary>
    public void SpawnHellHoundElite()
    {
        SpawnEnemy(HellHoundElite);
    }

    /// <summary>
    /// spawns a HellHound Elite in a random spawn point
    /// </summary>
    public void SpawnEliteWarrior()
    {
        SpawnEnemy(EliteWarrior,new Vector3(0,0,1));
    }

    /// <summary>
    /// spawns a HellHound in a random spawn point
    /// </summary>
    public void SpawnHellbat()
    {
        int num = Random.Range(0, spawnLocations.Count);
        var instance = Instantiate(Hellbat);
        float range = Random.Range(-1, 1);
        instance.transform.position =  new Vector3(spawnLocations[num].position.x +range, spawnLocations[num].position.y+4,0);
        instance.GetComponent<UnitStats>().OnDeath.AddListener(OneLessEnemy);
        instance.GetComponent<UnitStats>().Init(currentStage.StageNumber);
        currentEnemies++;
        EnemiesList.Add(instance);

        UpdateEnemies();
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


        //display stage complete screen
        if (!StageCompleteInstance)
            StageCompleteInstance = Instantiate(StageCompletePrefab, CameraUI.transform);
        else
            StageCompleteInstance.SetActive(true);


        StageCompleteInstance.GetComponent<StageComplete>().Init(stageNumber, unlockedSkill,cont.GetSkillPool(),cont,currentStage.UpgradeRewards);

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


        // save upgraded version of skills and equipped slots
        cont.SaveSkills();
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

            case "RainOfBlades":
                unlockedSkill = Skill.PlayerRainOfBlades;
                break;

            case "Bladestorm":
                unlockedSkill = Skill.PlayerBladeStorm;
                break;

            case "Chakram":
                unlockedSkill = Skill.PlayerChakram;
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
        UpdateEnemies();
        if (currentEnemies == 0 && initialized)
        {
            StartCoroutine(StageClearSequence());
        }
    }

    void UpdateEnemies()
    {
        remainingEnemies.text = "Remaining Enemies: " + currentEnemies;
    }

   public void ReloadStage()
    {
        //clear remaining enemies
        foreach(GameObject g in EnemiesList)
        {
            Destroy(g);
        }
        currentEnemies = 0;
        initialized = false;
        Init(ProgressSave.Instance.CurrentLevel);
        ProgressSave.Instance.retriesLeft--;
        DefeatScreen.SetActive(false);
    }

    void Defeated()
    {
        var tries = ProgressSave.Instance.retriesLeft;
        retries.text = "Remaining Tries: " + tries;
        if (tries > 0)
            retrybutton.SetActive(true);
        else
            retrybutton.SetActive(false);
        DefeatScreen.SetActive(true);
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");

    }
}
