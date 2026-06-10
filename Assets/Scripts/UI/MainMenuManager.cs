using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public GameObject settingsMenu;
    public GameObject mainMenu;
    public GameObject howToPlayMenu;
    public GameObject stageSelectMenu;

    public void LoadLevel1Scene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadLevel2Scene()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void OpenHowToPlauMenu()
    {
        howToPlayMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void CloseHowToPlauMenu()
    {
        howToPlayMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenStageSelectMenu()
    {
        stageSelectMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void CloseStageSelectuMenu()
    {
        stageSelectMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
