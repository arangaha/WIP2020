using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleScreenControler : MonoBehaviour
{
    [SerializeField] GameObject Controls;
   [SerializeField] GameObject Rules;
    [SerializeField] GameObject TitlePage;

    public void ShowTitlePage()
    {
        TitlePage.SetActive(true);
        Rules.SetActive(false);
        Controls.SetActive(false);
    }

    public void ShowRules()
    {
        TitlePage.SetActive(false);
        Rules.SetActive(true);
        Controls.SetActive(false);
    }

    public void ShowControls()
    {
        TitlePage.SetActive(false);
        Rules.SetActive(false);
        Controls.SetActive(true);
    }

    public void LoadRun()
    {
        ProgressSave.Instance.initialized = false;
        SceneManager.LoadScene("Stage One");
    }

}
